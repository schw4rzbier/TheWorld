using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            _logger.LogInformation("Getting all trips from the database");
            return _context.Trips.ToList();
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public void AddStop(string tripName, Stop newStop, string identityName)
        {  
            var trip = GetUserTripByName(tripName, identityName);
            if (trip != null)
            {
                //this is where ef core is different and needs extra attention
                trip.Stops.Add(newStop);
                _context.Stops.Add(newStop);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
              return (await _context.SaveChangesAsync()) > 0;
        }

        public Trip GetTripByName(string tripName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name == tripName);
        }

        public IEnumerable<Trip> GetTripsByUsername(string userName)
        {
            return _context.Trips
                .Where(t => t.UserName == userName)
                .Include(t => t.Stops)
                .ToList();
        }

        public Trip GetUserTripByName(string tripName, string identityName)
        {
            return _context.Trips
                .Include(t => t.Stops)
                .FirstOrDefault(t => t.Name == tripName && t.UserName == identityName);
        }
    }
}
