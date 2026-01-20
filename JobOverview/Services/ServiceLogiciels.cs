using JobOverview.Data;
using JobOverview.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobOverview.Services
{
    public interface IServiceLogiciels
   {
      Task<List<Logiciel>> ObtenirLogiciels();
      Task<Logiciel?> ObtenirLogiciel(string code);
   }
   
   public class ServiceLogiciels : IServiceLogiciels
    {
      private readonly ContexteJobOverview _contexte;
      public ServiceLogiciels(ContexteJobOverview contexte)
      {
        _contexte = contexte;
      }

      public async Task<List<Logiciel>> ObtenirLogiciels()
      {
         return await _contexte.Logiciels.ToListAsync();
      }

      public async Task<Logiciel?> ObtenirLogiciel(string code)
      {
         return await _contexte.Logiciels.FindAsync(code);

      }
   }
}
