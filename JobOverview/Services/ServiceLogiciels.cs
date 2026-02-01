
using JobOverview.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Version = JobOverview.Entities.Version;

namespace JobOverview.Services
{
    public interface IServiceLogiciels
   {
      Task<List<Logiciel>> ObtenirLogiciels(string codeFiliere);
      Task<Logiciel?> ObtenirLogiciel(string code);

      Task<List<Version>?> ObtenirVersionsLogiciel(string codeLogiciel, int? millesime);

        Task<Release?> ObtenirReleasesLogiciel(string codeLogiciel, float numeroVersion, short numeroRelease);

        Task<Release> AjouterRelease(string codeLogiciel, float numeroVersion, Release release);

        Task<Version> AjouterVersion(string codeLogiciel, Version version);


   }
   
   public class ServiceLogiciels : IServiceLogiciels
    {
      private readonly ContexteJobOverview _contexte;
      public ServiceLogiciels(ContexteJobOverview contexte)
      {
        _contexte = contexte;
      }

      public async Task<List<Logiciel>> ObtenirLogiciels(string codeFiliere)
      {
         var req = from l in _contexte.Logiciels
                   where l.CodeFiliere == codeFiliere
                   select l;

         return await req.ToListAsync(); // renvoie une liste
      }

      public async Task<Logiciel?> ObtenirLogiciel(string code)
      {
         //recupere logiciel et ses modules a plat
         var req = from e in _contexte.Logiciels
                   .Include(l => l.Modules)
                   .ThenInclude(m => m.SousModules)
                   where e.Code == code
                   select e;

         Logiciel? logiciel = await req.FirstOrDefaultAsync(); // renvoie un seul 

         if (logiciel == null) return null;

         //transforme l a liste des modules à plat en arborescence
         var req2 = from m in logiciel.Modules
                    where m.CodeModuleParent == null
                    select new Module
                    {
                       Code = m.Code,
                       Nom = m.Nom,
                       CodeModuleParent = m.CodeModuleParent,
                       SousModules = (from sm in m.SousModules select sm).ToList()
                    };
         logiciel.Modules = req2.ToList();

         return logiciel;

      }

      public async Task<List<Version>?> ObtenirVersionsLogiciel(string codeLogiciel, int? millesime)
      {
         //verifions si le logiciel exisite

         if (await _contexte.Logiciels.FindAsync(codeLogiciel) == null)
            return null;

         //on recupere ses versions et releases
         var req = from v in _contexte.Versions
                   .Include(v => v.Releases)
                   where v.CodeLogiciel == codeLogiciel &&
                   (millesime == null || v.Millesime == millesime)
                   select v;

         return await req.ToListAsync();

       
      }

        public async Task<Version> AjouterVersion(string codeLogiciel, Version version)
        {
            version.CodeLogiciel = codeLogiciel;

            //regles de validation
            ValidationRulesException ex = new();
            Regex regex = new Regex(@"^\d{1,3}(.\d{1,2})?$");
            if (!regex.IsMatch(version.Numero.ToString()))
                ex.Errors.Add("Numero", new string[] { $"Le numéro de version ({version.Numero}) doit avoir au maximum 3 chiffres avant la virgule et 2 après." });


            if (version.Millesime < 2020 || version.Millesime > 2100)
                ex.Errors.Add("Millesime", new string[] { $"Le millésime ({version.Millesime}) doit être compris entre 2020 et 2100 inclus" });

            if (version.DateOuverture >= version.DateSortiePrevue)
                ex.Errors.Add("DateOuverture", new string[] { $"La date d'ouverture doit être < à la date de sortie prévue." });

            if (version.DateSortieReelle != null && version.DateOuverture >= version.DateSortieReelle)
                ex.Errors.Add("DateOuverture", new string[] { $"La date d'ouverture doit être < à la date de sortie réelle." });

            if (ex.Errors.Any()) throw ex;

            //Enregistrement en base
            _contexte.Versions.Add(version);
            await _contexte.SaveChangesAsync();
            return version;
        }


        public async Task<Release?> ObtenirReleasesLogiciel(string codeLogiciel, float numeroVersion, short numeroRelease)
        {

            return await _contexte.Releases.FindAsync(numeroRelease, numeroVersion, codeLogiciel);
        }

        public async Task<Release> AjouterRelease(string codeLogiciel, float numeroVersion, Release release)
        {
            release.CodeLogiciel = codeLogiciel;
            release.NumeroVersion = numeroVersion;

            //recupere le N° de release max pour le logiciel et la version
            var req1 = from r in _contexte.Releases
                       where r.CodeLogiciel == codeLogiciel && r.NumeroVersion == numeroVersion
                       orderby r.Numero
                       select r.Numero;

            short relMax = await req1.LastOrDefaultAsync(); // renvoie 0 si pas de release

            if (relMax > 0)
            {
                if (release.Numero <= relMax)
                {
                    ValidationRulesException ex = new();
                    ex.Errors.Add("Numero", new string[] {$"LE N° de release doit être supérieur > {relMax}"});
                    throw ex;
                }

                //recuperer la date de publication de la derniere release
                var req2 = from r in _contexte.Releases
                           where r.CodeLogiciel == codeLogiciel && r.NumeroVersion == numeroVersion && r.Numero == relMax
                           select r.DatePubli;
                DateTime datePubliPresc =  await req2.FirstOrDefaultAsync();

                if (release.DatePubli <= datePubliPresc)
                {
                    ValidationRulesException ex = new();
                    ex.Errors.Add("DatePubli", new string[] { $"La date de la release doit être >= à  {datePubliPresc}" });
                    throw ex;
                }
            }




            // enregistrement en base si ok

            _contexte.Releases.Add(release);
            await _contexte.SaveChangesAsync();

            return release;
        }
    }

    
}
