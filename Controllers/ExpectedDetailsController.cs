using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpectedDetailsController : ControllerBase
    {
        private readonly AppContext _context;
        public ExpectedDetailsController(AppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpectedDetail>>> Get()
        {
            return await _context.ExpectedDetails
                        .Include(detail => detail.AssetItemNavigation)
                        .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpectedDetail>> Get(int id)
        {
            var expectedDetail = await _context.ExpectedDetails
                              .Include(detail => detail.AssetItemNavigation)
                              .Where(detail => detail.Id == id)
                              .FirstAsync();

            if (expectedDetail == null)
            {
                return NotFound();
            }

            return expectedDetail;
        }
        [HttpPost]
        public async Task<ActionResult<ExpectedDetail>> Post(ExpectedDetail detail)
        {
            _context.ExpectedDetails.Add(detail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ExpectedDetailExists(detail.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(Get), new { id = detail.Id }, detail);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, ExpectedDetail detail)
        {
            if (id != detail.Id)
            {
                return BadRequest();
            }

            _context.Entry(detail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpectedDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ExpectedDetail>> Delete(int id)
        {
            var detail = await _context.ExpectedDetails.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.ExpectedDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        private bool ExpectedDetailExists(int id)
        {
            return _context.ExpectedDetails.Any(e => e.Id == id);
        }
    }
}
