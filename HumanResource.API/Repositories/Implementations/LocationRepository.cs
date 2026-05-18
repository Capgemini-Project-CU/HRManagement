using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class LocationRepository : ILocationRepository
    {
        private readonly HRDbContext _context;
        public LocationRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                 .AsNoTracking()
                 .Include(l => l.Country)
                 .ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(decimal id)
        {
            return await _context.Locations
               .Include(l => l.Country)
               .AsNoTracking()
               .FirstOrDefaultAsync(l => l.LocationId == id);
        }

        public async Task<IEnumerable<Location>> GetByCountryAsync(string countryId)
        {
            return await _context.Locations
                .Where(l => l.CountryId == countryId)
                .AsNoTracking()
                .Include(l => l.Country)
                .ToListAsync();
        }


        public async Task<Location> AddAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<Location?> UpdateAsync(Location location)
        {
            var existing = await _context.Locations
                .FirstOrDefaultAsync(l => l.LocationId == location.LocationId);

            if (existing == null)
                return null;

            existing.StreetAddress = location.StreetAddress;
            existing.PostalCode = location.PostalCode;
            existing.City = location.City;
            existing.StateProvince = location.StateProvince;
            existing.CountryId = location.CountryId;

            await _context.SaveChangesAsync();
            return await _context.Locations
                .Include(l => l.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LocationId == location.LocationId);
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == id);

            if (location == null) return false;

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CountryExistsAsync(string countryId)
        {
            return await _context.Countries
               .AnyAsync(c => c.CountryId == countryId);
        }
    }
}
