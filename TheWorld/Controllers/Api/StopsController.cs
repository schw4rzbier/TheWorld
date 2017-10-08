using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private IWorldRepository _worldRepository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _coordsService;

        public StopsController(IWorldRepository worldRepository, ILogger<StopsController> logger, GeoCoordsService coordsService)
        {
            _coordsService = coordsService;
            _worldRepository = worldRepository;
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    //Lookup geocodes
                    var result = await _coordsService.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;
                    }

                    //save to the database
                    _worldRepository.AddStop(tripName, newStop, User.Identity.Name);

                    if (await _worldRepository.SaveChangesAsync())
                    {

                        return Created($"/api/trips/{tripName}/stops/{newStop.Name}",
                            Mapper.Map<StopViewModel>(newStop));
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to save stop: {e}");
            }
            return BadRequest("Failed to save stop");
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _worldRepository.GetUserTripByName(tripName, User.Identity.Name);

                return Ok(
                    Mapper.Map<IEnumerable<StopViewModel>>(
                        trip.Stops
                        .OrderBy(s => s.Order)
                        .ToList())
                        );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get stops: {ex}");
            }
            return BadRequest("Failed to get stops");
        }
    }
}
