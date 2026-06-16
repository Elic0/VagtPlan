using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiService.DBContext;
using ApiService.Models;
using ApiService.DTOS;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubstituteController : ControllerBase
    {
        private readonly AppDBContext _context;

        public SubstituteController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Substitute
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<SubstituteDTO>>> GetSubstitutes()
        {
            var substitute = await _context.Substitutes.ToListAsync();

            return Ok(substitute);
        }

        // GET: api/Substitute/5
        [HttpGet("get/{id}")]
        public async Task<ActionResult<SubstituteDTO>> GetSubstitute(int id)
        {
            var substitute = await _context.Substitutes.FindAsync(id);

            if (substitute == null)
            {
                return NotFound();
            }

            return Ok(substitute);
        }

        // PUT: api/Substitute/5
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditSubstitute([FromRoute]int id,[FromBody] SubstituteDTO substituteDTO)
        {
            var substitute = await _context.Substitutes.FindAsync(id);
            if (substitute == null)
            {  
                return NotFound(); 
            }

            substitute.Date = substituteDTO.Date;
            substitute.SubstitutedName = substituteDTO.SubstitutedName;
            substitute.SubstituteId = substituteDTO.SubstituteId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubstituteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }   
            }

            return Ok(substitute);
        }

        // POST: api/Substitute
        [HttpPost("addSubstitute")]
        public async Task<ActionResult<SubstituteDTO>> AddSubstitute(SubstituteDTO substituteDTO)
        {
            var substitute = new Substitute
            {
                Date = substituteDTO.Date,
                StartTime = substituteDTO.StartTime,
                EndTime = substituteDTO.EndTime,
                SubstitutedName = substituteDTO.SubstitutedName,
                SubstituteId = substituteDTO.SubstituteId,
                UserId = substituteDTO.UserId
            };
            _context.Substitutes.Add(substitute);
            await _context.SaveChangesAsync();

            return Ok(substitute);
        }

        // DELETE: api/Substitute/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSubstitute(int id)
        {
            var substitute = await _context.Substitutes.FindAsync(id);
            if (substitute == null)
            {
                return NotFound();
            }

            _context.Substitutes.Remove(substitute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubstituteExists(int id)
        {
            return _context.Substitutes.Any(e => e.Id == id);
        }
    }
}
