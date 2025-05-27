using Microsoft.AspNetCore.Mvc;
using project_wildfire_web.Models;
using project_wildfire_web.Models.DTO;
using project_wildfire_web.ExtensionsMethods;
using project_wildfire_web.Models;
using project_wildfire_web.Models.ArcGis;

namespace project_wildfire_web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminFireController : ControllerBase
    {
        private readonly FireDataDbContext _context;

        public AdminFireController(FireDataDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] FireDTO fireDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid fire data");

            // Force IsAdminFire to true (just in case someone tries to spoof)
            await DeleteFire(fireDto.FireId);
            fireDto.IsAdminFire = true;
            var curTime = DateTime.UtcNow;
            fireDto.StartDate = curTime;
            var fireEntity = fireDto.ToFire();

            _context.Fires.Add(fireEntity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "🔥 Admin fire successfully saved.",
                fireId = fireDto.FireId
            });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteFire(string id)
        {
            var fire = await _context.Fires.FindAsync(id);
            if (fire == null)
            {
                return NotFound(); // 🟡 would give 404 — but you got 500
            }

            // 🔥 Step 1: Remove any related subscriptions
            var subscriptions = _context.UserFireSubscriptions
                .Where(s => s.FireId == id);
            _context.UserFireSubscriptions.RemoveRange(subscriptions);

            // 🔥 Step 2: Now remove the fire itself
            _context.Fires.Remove(fire);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // ⛔ This is likely where the 500 is coming from
                Console.WriteLine("🔥 Error deleting fire: " + ex.Message);
                return StatusCode(500, "Server error while deleting fire");
            }

            return Ok(new { message = "Fire deleted successfully." });
        }

        [HttpPost("createWildfire")]
        public async Task<IActionResult> CreateFire([FromBody] WildfireDTO fire)
        {
            if (fire == null || string.IsNullOrEmpty(fire.FireId))
                return BadRequest("Invalid fire data.");

            var existing = await _context.Fires.FindAsync(fire.FireId);
            if (existing != null)
                return Conflict($"Fire with ID '{fire.FireId}' already exists.");
            var fireEntity = new Fire
            {
                FireId = fire.FireId,
                Name = fire.Name,
                Latitude = fire.Latitude,
                Longitude = fire.Longitude,
                AcreageBurned = fire.AcreageBurned,
                PercentageContained = fire.PercentageContained,
                POOCounty = fire.POOCounty,
                POOState = fire.POOState,
                FireCause = fire.FireCause,
                StartDate = fire.StartDate,
                RadiativePower = fire.RadiativePower,
                IsAdminFire = fire.IsAdminFire
            };

            _context.Fires.Add(fireEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Fire '{fire.FireId}' created." });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFireById(string id)
        {
            var fire = await _context.Fires.FindAsync(id);
            if (fire == null)
                return NotFound($"Wildfire '{id}' not found in the database.");

            return Ok(fire);
        }

    }
}

