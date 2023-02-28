using AutoMapper;
using CityInfo.API.Dtos;
using CityInfo.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    public class CitiesController : GenericController
    {

        #region Fields
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        private const int maxCitiesPageSize = 20;
        #endregion

        #region Constructors
        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        #region Public Methods

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDtoWithoutPOI>>> GetCities([FromQuery] string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {

            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }
            var (cities, paginationMetaData) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
            return Ok(_mapper.Map<IEnumerable<CityDtoWithoutPOI>>(cities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if(includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityDtoWithoutPOI>(city));
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
