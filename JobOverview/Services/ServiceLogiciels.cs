
using JobOverview.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        public async Task<Release?> ObtenirReleasesLogiciel(string codeLogiciel, float numeroVersion, short numeroRelease)
        {

            return await _contexte.Releases.FindAsync(numeroRelease, numeroVersion, codeLogiciel);
        }

        public async Task<Release> AjouterRelease(string codeLogiciel, float numeroVersion, Release release)
        {
            release.CodeLogiciel = codeLogiciel;
            release.NumeroVersion = numeroVersion;

            _contexte.Releases.Add(release);
            await _contexte.SaveChangesAsync();

            return release;
        }
    }
}
