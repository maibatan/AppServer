using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhysicalDetailsController : ControllerBase
    {
        private readonly AppContext _context;
        public PhysicalDetailsController(AppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhysicalDetail>>> Get()
        {
            return await _context.PhysicalDetails
                        .Include(detail => detail.InstanceNavigation)
                        .ThenInclude(instance => instance.AssetItemNavigation)
                        .ToListAsync();
        }
        [HttpGet("Inventory/{inventoryId}/AssetItem/{assetItem}")]
        public async Task<ActionResult<IEnumerable<PhysicalDetail>>> Get(int inventoryId, int assetItem)
        {
            return await _context.PhysicalDetails
                              .Include(detail => detail.InstanceNavigation)
                              .ThenInclude(instance => instance.AssetItemNavigation)
                              .Where(detail => detail.InventoryId == inventoryId)
                              .Where(detail => detail.InstanceNavigation.AssetItemId == assetItem)
                              .Where(detail => detail.Status == true)
                              .ToListAsync();
        }
        [HttpGet("Inventory/{inventoryId}")]
        public async Task<ActionResult<IEnumerable<PhysicalDetail>>> GetByInventoryId(int inventoryId)
        {
            return await _context.PhysicalDetails
                              .Include(detail => detail.InstanceNavigation)
                              .ThenInclude(instance => instance.AssetItemNavigation)
                              .Where(detail => detail.InventoryId == inventoryId)
                              .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PhysicalDetail>> Get(int id)
        {
            var detail = await _context.PhysicalDetails
                              .Include(detail => detail.InstanceNavigation)
                              .ThenInclude(instance => instance.AssetItemNavigation)
                              .Where(detail => detail.Id == id)
                              .FirstAsync();

            if (detail == null)
            {
                return NotFound();
            }

            return detail;
        }
        [HttpPost]
        public async Task<ActionResult<PhysicalDetail>> Post(PhysicalDetail detail)
        {
            _context.PhysicalDetails.Add(detail);
            if (PhysicalDetailExists(detail.InstanceId, detail.InventoryId))
            {
                return Conflict();
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhysicalDetailExists(detail.Id))
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
        public async Task<IActionResult> Put(int id, PhysicalDetail detail)
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
                if (!PhysicalDetailExists(id))
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
        public async Task<ActionResult<PhysicalDetail>> Delete(int id)
        {
            var detail = await _context.PhysicalDetails.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }

            _context.PhysicalDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return detail;
        }
        private bool PhysicalDetailExists(int id)
        {
            return _context.PhysicalDetails.Any(e => e.Id == id);
        }
        private bool PhysicalDetailExists(int instanceId,int inventoryId)
        {
            return _context.PhysicalDetails.Any(e => e.InstanceId == instanceId && e.InventoryId == inventoryId);
        }
    }
}
