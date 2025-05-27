

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_wildfire_web.Models.ArcGis;
using project_wildfire_web.Models;
using project_wildfire_web.Models.DTO;
using project_wildfire_web.DAL.Abstract;
using project_wildfire_web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/wildfires")]
    public class ArcGISController : ControllerBase
    {
        private readonly IArcGisService _arcGisService;
        private readonly FireDataDbContext _dbContext;
        private readonly IWildfireRepository _wildfireRepository;


        public ArcGISController(IArcGisService arcGisService, FireDataDbContext dbContext, IWildfireRepository wildfireRepository)
        {
            _arcGisService = arcGisService;
            _dbContext = dbContext;
            _wildfireRepository = wildfireRepository;


        }

        /// <summary>
        /// GET /api/wildfires
        /// Returns all active wildfires.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FireEvent>>> GetAllWildfires()
        {
            var fires = await _arcGisService.GetUsaWildfiresAsync();
            return Ok(fires);
        }

        [HttpGet("exists/{uniqueId}")]
        public async Task<ActionResult<bool>> Exists(string uniqueId)
        {

            bool isActive = await _arcGisService.FireExistsAsync(uniqueId);
            return Ok(isActive);
        }

        [HttpPost("populate")]
        public async Task<IActionResult> PopulateFires()
        {
            await _wildfireRepository.ClearWildfiresAsync();

            var fireEvents = await _arcGisService.GetUsaWildfiresAsync();

            foreach (var fireEvent in fireEvents)
            {
                var fire = new Fire
                {
                    FireId = fireEvent.UniqueFireIdentifier,
                    Name = fireEvent.Name,
                    Latitude = fireEvent.Latitude,
                    Longitude = fireEvent.Longitude,
                    AcreageBurned = fireEvent.AcreageBurned,
                    PercentageContained = fireEvent.PercentageContained,
                    POOCounty = fireEvent.POOCounty,
                    POOState = fireEvent.POOState,
                    FireCause = fireEvent.FireCause,
                    StartDate = fireEvent.StartDate
                    // RadiativePower can be set if you have a mapping for it
                };

                var existing = await _dbContext.Fires.FindAsync(fire.FireId);
                if (existing == null)
                {
                    _dbContext.Fires.Add(fire);
                }
                else
                {
                    existing.Name = fire.Name;
                    existing.Latitude = fire.Latitude;
                    existing.Longitude = fire.Longitude;
                    existing.AcreageBurned = fire.AcreageBurned;
                    existing.PercentageContained = fire.PercentageContained;
                    existing.POOCounty = fire.POOCounty;
                    existing.POOState = fire.POOState;
                    existing.FireCause = fire.FireCause;
                    existing.StartDate = fire.StartDate;
                    // Update RadiativePower if needed
                }
            }

            await _dbContext.SaveChangesAsync();
            return Ok("Fire table populated with all FireEvent attributes.");
        }
          [HttpGet("getSavedFires")]
        public async Task<IActionResult> GetSavedFires()
            {
              
                var fires = await _dbContext.Fires.ToListAsync();

               var fireDTOs = fires.Select(f => new WildfireDTO {
                    FireId = f.FireId,
                    Name = f.Name ?? "",
                    Latitude = f.Latitude,
                    Longitude = f.Longitude,
                    AcreageBurned = f.AcreageBurned ?? 0,
                    PercentageContained = f.PercentageContained ?? 0,
                    POOCounty = f.POOCounty ?? "",
                    POOState = f.POOState ?? "",
                    FireCause = f.FireCause ?? "",
                    StartDate = f.StartDate,
                    RadiativePower = f.RadiativePower ?? 0,
                    IsAdminFire = f.IsAdminFire
                }).ToList();
                return Ok(fireDTOs);
            }

        /// <summary>
        /// POST /api/wildfires/populate/{id}
        /// Inserts or updates a single wildfire record based on its UniqueFireIdentifier.
        /// </summary>

    }
}