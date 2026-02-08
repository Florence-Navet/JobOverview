using JobOverview.Data;
using JobOverview.entities;
using JobOverview.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobOverview.Services
{
    public interface IServiceTaches
    {
        Task<List<Tache>> ObtenirTaches(string? personne, string? logiciel, float? version);
        Task<Tache?> ObtenirTache(int id);
        Task<Personne?> ObtenirPersonne(string peudo);
        Task<Tache> AjouterTache(Tache tache);
        Task<Travail> AjouterTravail(int idTache, Travail tache);
    }

    public class ServiceTaches : IServiceTaches
    {
        private readonly ContexteJobOverview _contexte;

        public ServiceTaches(ContexteJobOverview contexte)
        {
            _contexte = contexte;
        }

        // Renvoie le tâches pour une personne, un logiciel et une version donnés
        public async Task<List<Tache>> ObtenirTaches(string? personne, string? logiciel, float? version)
        {
            var req = from t in _contexte.Taches
                      where (personne == null || t.Personne == personne) &&
                             (logiciel == null || t.CodeLogiciel == logiciel) &&
                             (version == null || t.NumVersion == version)
                      orderby t.CodeLogiciel, t.NumVersion
                      select t;

            return await req.ToListAsync();
        }

        // Renvoie une tâche et ses travaux associés
        public async Task<Tache?> ObtenirTache(int id)
        {
            var req = from t in _contexte.Taches
                         .Include(x => x.Travaux.OrderBy(x => x.DateTravail))
                      where t.Id == id
                      select t;

            //renvoie une seule tâche ou null si l'id n'existe pas
            return await req.FirstOrDefaultAsync();
        }

        // Renvoie une personne avec son métier et ses activités ou null si le pseudo n'existe pas
        public async Task<Personne?> ObtenirPersonne(string pseudo)
        {
            var req = from p in _contexte.Personnes
                         .Include(p => p.Metier)
                         .ThenInclude(m => m.Activites)
                      where p.Pseudo == pseudo
                      select p;

            var pers = await req.FirstOrDefaultAsync();
            return pers;
        }

        // Ajoute une tâche 
        // penser à mettre la prop de navigation
        // à null pour éviter d'ajouter les travaux associés en même temps
        public async Task<Tache> AjouterTache(Tache tache)
        {
            tache.Travaux = null!;

            //Récupère la personne et ses activités
            Personne? pers = await ObtenirPersonne(tache.Personne);
            if (pers == null)
            
                throw new ValidationRulesException("Personne", $"Persone {tache.Personne} non trouvée");

                //Verifie si le code activité de la tache fait parte de ceux de la personne
                if (pers.Metier.Activites.Find(a => a.Code == tache.CodeActivite) == null)
                    throw new ValidationRulesException("CodeActivite", $"L'activité ne correspond pas au métier de la personne");


            _contexte.Taches.Add(tache);
            await _contexte.SaveChangesAsync();

            return tache;
        }

        // Ajoute un travail sur une tâche donnée
        public async Task<Travail> AjouterTravail(int idTache, Travail travail)
        {
            ValidationRulesException vre = new();
            if(travail.DateTravail.TimeOfDay != new TimeSpan())
                vre.Errors.Add("Date", new string[] { "La partie heure de la date doite être à 0" });

            if(travail.Heures < 0.5m || travail.Heures > 8)
                vre.Errors.Add("Heures", new string[] { "Le nombre d'heures doit être compris entre 0.5 et 8" });

            if(vre.Errors.Any())
                 throw vre;

            //La tache doit exister en base 
            // on recupère la tache
            Tache? tache = await _contexte.Taches.FindAsync(idTache);
            if(tache == null)
                throw new ValidationRulesException("IdTache", $"Tache d'id {idTache} non trouvée");

            //remplacer le % de productivite reçu par celui de la pers concernée récupérée dans la table personne
            //on récupère la pers associé à la tache et ses activités
            Personne? pers = await ObtenirPersonne(tache.Personne);

            travail.IdTache = idTache;
            travail.TauxProductivite = pers!.TauxProductivite; // ! pour dire que pers ne peut pas être null car la tache existe et doit être associée à une personne

            //MAJ de la durée de travail restante sur la tache en retranchant la duree du tranavail ajouté(pas inf à 0)
            //Met à jour la duree de travail restant sur la tache
            tache.DureeRestante -= travail.Heures;
            //Si la durée restante devient négative, on la met à 0
            if (tache.DureeRestante < 0) tache.DureeRestante = 0;

            _contexte.Travaux.Add(travail);

            await _contexte.SaveChangesAsync();

            return travail;
        }
    }
}