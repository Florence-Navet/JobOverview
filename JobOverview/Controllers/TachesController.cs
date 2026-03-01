using JobOverview.Data;
using JobOverview.entities;
using JobOverview.Entities;
using JobOverview.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobOverview.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TachesController : ControllerBase
{
    private readonly IServiceTaches _serviceTaches;

    public TachesController(IServiceTaches serviceTaches)
    {
        _serviceTaches = serviceTaches;
    }


    // GET: api/Taches?personne=x&Logiciel=y&version=z
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tache>>> GetTaches(
        [FromQuery] string? personne, [FromQuery] string? logiciel, [FromQuery] float? version)
    {
        List<Tache> taches = await _serviceTaches.ObtenirTaches(personne, logiciel, version);

        return Ok(taches);
    }


    // GET: api/Taches/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Tache>> GetTache(int id)
    {
        Tache? tache = await _serviceTaches.ObtenirTache(id);
        if (tache == null) return NotFound();

        return Ok(tache);
    }


    // GET: api/Personnes/RBREAUMONT
    [HttpGet("/api/Personnes/{pseudo}")]
    public async Task<ActionResult<Personne>> GetPersonne(string pseudo)
    {
        var pers = await _serviceTaches.ObtenirPersonne(pseudo);

        if (pers == null) return NotFound($"La personne {pers} n'existe pas");

        return Ok(pers);
    }

    // PUT : api/Taches
    [HttpPut]
    public async Task<ActionResult<Tache>> PutTache(Tache tache)
    {
        try
        {
            //modifie la tache si elle exite ou bien la crée
            // et récupère avec son Id généré automatiquement
            Tache res = await _serviceTaches.ModifierAjouterTache(tache);

            //renvoie la réponse appropriée selon que la tache a été modifiée ou maj
            if (tache.Id == 0) // tache créee
                return CreatedAtAction(nameof(GetTache), new { res.Id }, res);
            else
                return Ok(res);
        }
        catch (Exception e)
        {
            return this.CustomResponseForError(e);
        }
    }


    // POST: api/Taches // cree une tache
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Tache>> PostTache(Tache tache)
    {
        try
        {
            // Enregistre la tâche dans la base et la récupère avec son Id généré automatiquement
            Tache res = await _serviceTaches.AjouterTache(tache);
            return CreatedAtAction(nameof(GetTache), new { res.Id }, res);
        }
        catch (Exception e)
        {
            return this.CustomResponseForError(e);
        }
    }

    // POST: api/Taches/5/Travaux // ajoute un travail à une tache donnée
    [HttpPost("{idTache}/Travaux")]
    public async Task<IActionResult> PostTravail([FromRoute] int idTache, Travail travail)
    {
        try
        {
            Travail res = await _serviceTaches.AjouterTravail(idTache, travail);
            return CreatedAtAction(nameof(GetTache), new { id = res.IdTache }, res);
        }
        catch (Exception e)
        {
            return this.CustomResponseForError(e);
        }

    }

    //DELETE: api/Taches/45/Travaux/2023-11-23
    [HttpDelete("{idTache}/Travaux/{date}")]
    public async Task<IActionResult> DeleteTravail(int idTache, DateTime date)
    {
        try
        {
            await _serviceTaches.SupprimerTravail(idTache, date);
            return NoContent();
        }
        catch (Exception e)
        {
            return this.CustomResponseForError(e);
        }
    }

    //DELETE : Taches?personne=x&Logiciel=y&version=z
    [HttpDelete]
    public async Task<IActionResult> DeleteTaches(
        [FromQuery] string? personne, [FromQuery] string? logiciel, [FromQuery] float? version)
    {
        try
        {
            int nbSuppr = await _serviceTaches.SupprimerTaches(personne, logiciel, version);
            return Ok(nbSuppr + " tâche(s) supprimée(s)");
        }
        catch (Exception e)
        {
            return this.CustomResponseForError(e);
        }
    }
}