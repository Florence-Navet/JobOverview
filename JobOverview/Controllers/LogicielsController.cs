
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
   }
}
