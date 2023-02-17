using CityInfo.API.Models;

namespace CityInfo.API.Repositories.Interfaces
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityByIdAsync(int id, bool includePointsOfInterest = false);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest> GetPointOfInterestForCityByIdAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExistsAsync(int cityId);
        Task<bool> SaveChangesAsync();
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
    }
}
