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
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, device);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(int id, Device device)
        {
            if (id != device.Id)
            {
                return BadRequest(new {message = "ID mismatch." });
            }

            var existingDevice = await _context.Devices.FindAsync(id);
            if (existingDevice == null)
            {
                return NotFound(new { message = "Device not found." });
            }

            existingDevice.Name = device.Name;
            existingDevice.Manufacturer = device.Manufacturer;
            existingDevice.Type = device.Type;
            existingDevice.OperatingSystem = device.OperatingSystem;
            existingDevice.OSVersion = device.OSVersion;
            existingDevice.Processor = device.Processor;
            existingDevice.RAM = device.RAM;
            existingDevice.Description = device.Description;
            existingDevice.UserId = device.UserId;
        
            await _context.SaveChangesAsync();

            return NoContent();
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
    }
}