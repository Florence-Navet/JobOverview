using JobOverview.Data.Migrations;
using JobOverview.Entities;
using JobOverview.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
      public async Task<ActionResult<IEnumerable<Logiciel>>> GetLogiciels()
      {
         return await _serviceLog.ObtenirLogiciels();
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

   }
}
