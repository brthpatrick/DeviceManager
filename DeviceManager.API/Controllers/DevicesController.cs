using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeviceManager.API.Data;
using DeviceManager.API.Models;

namespace DeviceManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DevicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            var devices = await _context.Devices
                .Include(d => d.User)
                .ToListAsync();

            return Ok(devices);   
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(int id)
        {
            var device = await _context.Devices
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound(new { message = "Device not found." });
            }

            return Ok(device);
        }

        [HttpPost]
        public async Task<ActionResult<Device>> CreateDevice(Device device)
        {
            var exists = await _context.Devices
                        .AnyAsync(d => d.Name.ToLower() == device.Name.ToLower() 
                        && d.Manufacturer.ToLower() == device.Manufacturer.ToLower());

            if (exists)
            {
                return Conflict(new { message = "A device with this name and manufacturer already exists." });
            }

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, device);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound(new { message = "Device not found." });
            }

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, Device device)
        {
            if (id != device.Id)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Devices.AnyAsync(d => d.Id == id);
                if (!exists)
                {
                    return NotFound(new { message = "Device not found." });
                }
                throw;
            }

            return NoContent();
        }
    }
}