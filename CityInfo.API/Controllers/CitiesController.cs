

using CityInfo.API.Models;
using CityInfo.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    public class CitiesController : GenericController
    {

        #region Fields
        private readonly ICityInfoRepository _cityInfoRepository;
        #endregion

        #region Constructors
        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }
        #endregion

        #region Public Methods



        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            var cities = await _cityInfoRepository.GetCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
