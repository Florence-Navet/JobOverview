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
using JobOverview.Entities;

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
        public async Task<ActionResult<IEnumerable<Filiere>>> GetEquipes(string codeFiliere, string codeEquipe)
        {
           var équipes = await _service.ObtenirEquipes(codeFiliere, codeEquipe);

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


        #region Requetes POST
        // POST api/Filieres/BIOV/Equipes
        [HttpGet]
        public async Task<ActionResult<Equipe>> PostEquipes(string codeFiliere, Equipe eq)
        {
            Equipe res = await _service.AjouterEquipe(codeFiliere, eq);

            //renvoie une reponse 201 avec l'entête
            // 'location <url d'accès à l'équipe> et un corps contenant l'équipe créée
            return CreatedAtAction(nameof(ObtenirEquipe), new { codeFiliere = res.CodeFiliere, codeEquipe = res.Code }, res);
        }


        // POST api/Filieres/BIOV/Equipes
        [HttpPost("{codeEquipe}")]
        public async Task<ActionResult<Equipe>> PostPersonne(string codeFiliere, string codeEquipe, Personne pers)
        {
            Personne res = await _service.AjouterPersonne(codeEquipe, pers);

            //renvoie une réponse de code 201 avec l'entête
            // "location <url d'acces à l'équipe de la personne> et un corps contenant l'equipe
            return CreatedAtAction(nameof(ObtenirEquipe), new { codeFiliere, codeEquipe }, res);
        }

        #endregion


    }
}
