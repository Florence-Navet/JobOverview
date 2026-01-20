using JobOverview.entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobOverview.Services
{
   public interface IServiceEquipes
   {
      Task<List<Equipe>> ObtenirEquipes(string codeFiliere);
      Task<Equipe?> ObtenirEquipe(string codeFiliere, string codeEquipe);
   }

   public class ServiceEquipes : IServiceEquipes
   {
      private readonly ContexteJobOverview _contexte;

      public ServiceEquipes(ContexteJobOverview contexte)
      {
         _contexte = contexte;
      }

      public async Task<List<Equipe>> ObtenirEquipes(string codeFiliere)
      {
         var req = from e in _contexte.Equipes.Include(e => e.Service)
                   where e.CodeFiliere == codeFiliere
                   select e;

         return await req.ToListAsync();
      }

    
      public async Task<Equipe?> ObtenirEquipe(string codeFiliere, string codeEquipe)
      {
         var req = from e in _contexte.Equipes
                   .Include(e => e.Service)
                   .Include(e => e.Personnes)
                   .ThenInclude(p => p.Metier)
                   where e.Code == codeEquipe
                   select e;

         return await req.FirstOrDefaultAsync();
      }
   }
}
