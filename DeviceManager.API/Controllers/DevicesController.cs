using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeviceManager.API.Data;
using DeviceManager.API.Models;
using Microsoft.AspNetCore.Mvc.Routing;

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

        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignDevice(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "You must be logged in." });
            }

            var userId = int.Parse(userIdClaim.Value);
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound(new { message = "Device not found." });
            }

            if (device.UserId != null)
            {
                return Conflict(new { message = "This device is already assigend to someone." });
            }

            device.UserId = userId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Device assigned successfully." });
        }

        [HttpPost("{id}/unassign")]
        public async Task<IActionResult> UnassignDevice(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new {message = "You must be logged in." });
            }

            var userId = int.Parse(userIdClaim.Value);
            var device = await _context.Devices.FindAsync(id);

            if (device == null)
            {
                return NotFound(new { message = "Device not found." });
            }

            if (device.UserId != userId)
            {
                return BadRequest(new { message = "You can only unassign devices assigned to yourself." });
            }

            device.UserId = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Device unassigned successfully." });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Device>>> SearchDevices([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(new List<Device>());
            }

            var normalizedQuery = new string(q.ToLower()
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                .ToArray());

            var tokens = normalizedQuery
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToList();

            if (tokens.Count == 0)
            {
                return Ok(new List<Device>());
            }

            var allDevices = await _context.Devices
                .Include(d => d.User)
                .ToListAsync();

            var scoredDevices = allDevices.Select(device =>
            {
                int score = 0;

                foreach (var token in tokens)
                {
                    if (device.Name.ToLower().Contains(token))
                        score += 10;

                    if (device.Manufacturer.ToLower().Contains(token))
                        score += 8;

                    if (device.Processor.ToLower().Contains(token))
                        score += 5;

                    if (device.RAM.ToString().Contains(token))
                        score += 3;
                }

                return new { Device = device, Score = score };
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Device)
            .ToList();

            return Ok(scoredDevices);
        }
    }
}