
using JobOverview.Entities;
using JobOverview.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Version = JobOverview.Entities.Version;
using JobOverview.entities; 

namespace JobOverview.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class LogicielsController : ControllerBase
   {
      private readonly IServiceLogiciels _serviceLog;

      public LogicielsController(IServiceLogiciels service)
      {
         _serviceLog = service;
      }

      // GET: api/Logiciels
      [HttpGet]
      public async Task<ActionResult<IEnumerable<Logiciel>>> GetLogiciels([FromQuery] string codeFiliere)
      {
         var logiciels = await _serviceLog.ObtenirLogiciels(codeFiliere);
         return Ok(logiciels);
      }

      // GET: api/Logiciels/ABC
      [HttpGet("{code}")]
      public async Task<ActionResult<Logiciel>> GetLogiciel(string code)
      {
         var logiciel = await _serviceLog.ObtenirLogiciel(code);

         if (logiciel == null)
         {
            return NotFound();
         }

         return logiciel;
      }


      //GET : Logiciels/GENOMICA/versions?millesime=2018
      [HttpGet("{codeLogiciel}/versions")]
      public async Task<ActionResult<IEnumerable<Version>>> GetVersions(String codeLogiciel, [FromQuery] int? millesime)
      {
         var versions = await _serviceLog.ObtenirVersionsLogiciel(codeLogiciel, millesime);

         if (versions == null) return NotFound();

         return Ok(versions);
      }

        //GET : Logiciels/GENOMICA/versions/1.00/Releases/30
        [HttpGet("{codeLogiciel}/versions/{numeroVersion}/Releases/{numeroRelease}")]
        public async Task<ActionResult<IEnumerable<Version>>> GetRelease(string codeLogiciel, float numeroVersion, short numeroRelease)
        {
            var release = await _serviceLog.ObtenirReleasesLogiciel(codeLogiciel, numeroVersion, numeroRelease);

            if (release == null) return NotFound();

            return Ok(release);
        }

        //POST : api/logiciesls/GENOMICA/versions/1.00/Releases
        [HttpGet("{codeLogiciel}/versions/{numeroVersion}/Releases")]
        public async Task<ActionResult<Release>> PostRelease(string codeLogiciel, float numeroVersion, [FromForm] FormRelease fr)
        {
            //cree une entité du modèle à partir de l'entite DTO

            Release rel = new Release
            {
                CodeLogiciel = codeLogiciel,
                NumeroVersion = numeroVersion,
                Numero = fr.Numero,
                DatePubli = fr.DatePubli
            };

            if (fr.Notes != null)
            {
                using StreamReader reader = new(fr.Notes.OpenReadStream());
                rel.Notes = await reader.ReadToEndAsync();
            }

            Release res = await _serviceLog.AjouterRelease(codeLogiciel, numeroVersion, rel);

            //objet anonyme pour la route de la ressource créée
            object clé = new { codeLogiciel = res.CodeLogiciel, numeroVersion = res.NumeroVersion, numeroRelease = res.Numero };
            string uri = Url.Action(nameof(GetRelease), clé) ?? "";
            return Created(uri, res);
        }
    }
}
