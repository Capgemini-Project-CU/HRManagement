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
            return await _context.Regions
                .Include(r => r.Countries)
                .ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(decimal id)
        {
            return await _context.Regions
                .Include(r => r.Countries)
                .FirstOrDefaultAsync(r => r.RegionId == id);
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
