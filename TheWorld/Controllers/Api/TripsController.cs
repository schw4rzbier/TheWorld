using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("api/trips")]
    [Authorize]
    public class TripsController : Controller
    {
        private IWorldRepository _worldRepository;
        private ILogger<TripsController> _logger;

        public TripsController(IWorldRepository worldRepository, ILogger<TripsController> logger)
        {
            _logger = logger;
            _worldRepository = worldRepository;

        }

        [HttpGet("")]
        public IActionResult Get()
        {
            try
            {
                var results = _worldRepository.GetTripsByUsername(this.User.Identity.Name);
                return Ok(Mapper.Map<IEnumerable<TripViewModel>>(results));
            }
            catch (Exception ex)
            {
                // TODO logging
                _logger.LogError($"Failed to get all trips: {ex}");
                return BadRequest("Error occurred");
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]TripViewModel theTripVm)
        {
            if (ModelState.IsValid)
            {
                //Save to the database
                var newTrip = Mapper.Map<Trip>(theTripVm);

                newTrip.UserName = this.User.Identity.Name;


                _worldRepository.AddTrip(newTrip);

                if (await _worldRepository.SaveChangesAsync())
                {
                    return Created($"api/trips/{theTripVm.Name}", Mapper.Map<TripViewModel>(newTrip));
                }
            }

            return BadRequest("Failed to save the trip");

        }
    }
}
