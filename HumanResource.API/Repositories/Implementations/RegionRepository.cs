using HumanResource.API.Data;
using HumanResource.API.Models;
using HumanResource.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.API.Repositories.Implementations
{
    public class RegionRepository : IRegionRepository
    {
        private readonly HRDbContext _context;

        public RegionRepository(HRDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            var regions = await _context.Regions
                .ToListAsync();

            foreach (var region in regions)
            {
                await _context.Entry(region)
                    .Collection(r => r.Countries)
                    .LoadAsync();
            }

            return regions;
        }

        public async Task<Region?> GetByIdAsync(decimal id)
        {
            var region = await _context.Regions
                .FirstOrDefaultAsync(r => r.RegionId == id);

            if (region != null)
            {
                await _context.Entry(region)
                    .Collection(r => r.Countries)
                    .LoadAsync();
            }

            return region;
        }

        public async Task<Region> AddAsync(Region region)
        {
            await _context.Regions.AddAsync(region);

            await _context.SaveChangesAsync();

            return region;
        }

        public async Task<Region?> UpdateAsync(Region region)
        {
            var existing = await _context.Regions
                .FirstOrDefaultAsync(r => r.RegionId == region.RegionId);

            if (existing == null)
                return null;

            existing.RegionName = region.RegionName;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            var region = await _context.Regions
                .FirstOrDefaultAsync(r => r.RegionId == id);

            if (region == null)
                return false;

            _context.Regions.Remove(region);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}