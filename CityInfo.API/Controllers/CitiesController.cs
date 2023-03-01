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

        /// <summary>
        /// Get a list of cities
        /// </summary>
        /// <param name="name">The name of the city to filter</param>
        /// <param name="searchQuery">The text to search for</param>
        /// <param name="pageNumber">The page number to request for paging</param>
        /// <param name="pageSize">The number of records per page maximum is 20</param>
        /// <returns>An ActionResult including an IEnumerable of CityDtoWithoutPOI</returns>

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

        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id">The id of the city</param>
        /// <param name="includePointsOfInterest">whether or not to include the city's points of interests</param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
