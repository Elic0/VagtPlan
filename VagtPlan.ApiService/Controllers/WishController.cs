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
    public class WishController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WishController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Wishes
        [Authorize]
        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<WishDTO>>> GetWishes()
        {
            var wishes =  await _context.Wishes.ToListAsync();

            return Ok(wishes);
        }

        // GET: api/Wishes/5
        [Authorize]
        [HttpGet("get/{id}")]
        public async Task<ActionResult<WishDTO>> GetWish([FromRoute]int id)
        {
            var wish = await _context.Wishes.FindAsync(id);

            if (wish == null)
            {
                return NotFound();
            }

            return Ok(wish);
        }

        // PUT: api/Wishes/5
        [Authorize]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditWish([FromRoute]int id,[FromBody] WishDTO wishDTO)
        {
            var wish = await _context.Wishes.FindAsync(id);
            if (wish == null)
            {  
                return NotFound(); 
            }

            wish.DayOfWeek = wishDTO.DayOfWeek;
            wish.StatusId = wishDTO.StatusId;
            wish.UserId = wishDTO.UserId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(wish);
        }

        // POST: api/Wishes
        [Authorize]
        [HttpPost("createWish")]
        public async Task<ActionResult<WishDTO>> CreateWish(WishDTO wishDTO)
        {
            var wish = new Wish
            {
                DayOfWeek = wishDTO.DayOfWeek,
                StatusId = wishDTO.StatusId,
                UserId = wishDTO.UserId
            };
            _context.Wishes.Add(wish);
            await _context.SaveChangesAsync();

            return Ok(wish);
        }

        // DELETE: api/Wishes/5
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteWish([FromRoute]int id)
        {
            var wish = await _context.Wishes.FindAsync(id);
            if (wish == null)
            {
                return NotFound();
            }

            _context.Wishes.Remove(wish);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WishExists(int id)
        {
            return _context.Wishes.Any(e => e.Id == id);
        }
    }
}
