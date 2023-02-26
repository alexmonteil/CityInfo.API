using CityInfo.API.Data;
using CityInfo.API.Models;
using CityInfo.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {

        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(searchQuery))
            {
                return await GetCitiesAsync();
            }

            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery) || (a.Description != null && a.Description.Contains(searchQuery)));
            }

            return await collection.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityByIdAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).Where(c => c.Id == cityId)
                    .FirstOrDefaultAsync();
            }

            return await _context.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterests.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterests.Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityByIdAsync(cityId, false);
            city?.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterests.Remove(pointOfInterest);
        }
    }
}
