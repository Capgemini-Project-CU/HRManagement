using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly HRDbContext _context;

        public CountryRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var countries = await _context.Countries
                .ToListAsync();

            foreach (var country in countries)
            {
                await _context.Entry(country)
                    .Reference(c => c.Region)
                    .LoadAsync();
            }

            return countries;
        }

        public async Task<Country?> GetByIdAsync(string id)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.CountryId == id);

            if (country != null)
            {
                await _context.Entry(country)
                    .Reference(c => c.Region)
                    .LoadAsync();
            }

            return country;
        }

        public async Task<IEnumerable<Country>> GetByRegionIdAsync(decimal regionId)
        {
            var countries = await _context.Countries
                .Where(c => c.RegionId == regionId)
                .ToListAsync();

            foreach (var country in countries)
            {
                await _context.Entry(country)
                    .Reference(c => c.Region)
                    .LoadAsync();
            }

            return countries;
        }

        public async Task<Country> AddAsync(Country country)
        {
            await _context.Countries.AddAsync(country);

            await _context.SaveChangesAsync();

            return country;
        }

        public async Task<Country?> UpdateAsync(Country country)
        {
            var existing = await _context.Countries
                .FirstOrDefaultAsync(c => c.CountryId == country.CountryId);

            if (existing == null)
                return null;

            existing.CountryName = country.CountryName;

            existing.RegionId = country.RegionId;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var country = await _context.Countries
                .FirstOrDefaultAsync(c => c.CountryId == id);

            if (country == null)
                return false;

            _context.Countries.Remove(country);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}