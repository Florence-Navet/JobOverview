using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobOverview;
using JobOverview.entities;
using JobOverview.Services;

namespace JobOverview.Controllers
{
    [Route("api/Filieres/{codeFiliere}/[controller]")]
    [ApiController]
    public class EquipesController : ControllerBase
    {
        private readonly IServiceEquipes _service;

        public EquipesController(IServiceEquipes service)
        {
            _service = service;
        }

        // GET: api/Filieres/BIOH/Equipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipe>>> GetEquipes(string codeFiliere)
        {
           var équipes = await _service.ObtenirEquipes(codeFiliere);

         if (!équipes.Any()) return NotFound();
         return Ok(équipes);
        }

      // GET: api/Filiere/BIOH/Equipes/BIOH_DEV
      [HttpGet("{CodeEquipe}")]
      public async Task<ActionResult<Equipe?>> ObtenirEquipe(string codeFiliere, string codeEquipe)
      {
         var équipe = await _service.ObtenirEquipe(codeFiliere, codeEquipe);

         if(équipe == null)return NotFound();
         return Ok(équipe);
      }

        
    }
}
