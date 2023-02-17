
using AutoMapper;
using CityInfo.API.Dtos;
using CityInfo.API.Models;
using CityInfo.API.Repositories.Interfaces;
using CityInfo.API.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {

        #region Fields

        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region Public Methods

        // GET All PointOfInterests for a city
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointsOfInterests = _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterests));
        }

        // GET one PointOfInteret for a city
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        // CREATE PointOfInterest for a city
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
            PointOfInterestDto pointOfInterestDto)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var newPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterestDto);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, newPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestDto = _mapper.Map<PointOfInterestDto>(newPointOfInterest);
            return CreatedAtRoute("GetPointOfInterest", new { cityId, pointOfInterestId = createdPointOfInterestDto.Id }, createdPointOfInterestDto);
        }

        // FULL UPDATE PointOfInterest for a city
        [HttpPut("{pointOfinterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestDto pointOfInterest)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                return NotFound();
            }

            var updateTarget = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (updateTarget == null)
            {
                return NotFound();
            }

            updateTarget.Name = pointOfInterest.Name;
            updateTarget.Description = pointOfInterest.Description;
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }


        // PARTIAL UPDATE PointOfInterest for a city
        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestDto> patchDocument)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                return NotFound();
            }

            var updateTarget = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (updateTarget == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestDto()
            {
                Name = updateTarget.Name,
                Description = updateTarget.Description,
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            updateTarget.Name = pointOfInterestToPatch.Name;
            updateTarget.Description = pointOfInterestToPatch.Description;
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        // DELETE Point of Interest
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                return NotFound();
            }

            var deleteTarget = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (deleteTarget == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(deleteTarget);
            _mailService.Send("Point of Interest deleted", $"Point of Interest {deleteTarget} with id {deleteTarget.Id} was deleted.");
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        #endregion

        #region Private Methods

        

        #endregion
    }
}
