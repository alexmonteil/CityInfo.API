
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

        #endregion

        #region Constructors
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }

        #endregion

        #region Public Methods

        // GET All PointOfInterests for a city
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterest>>> GetPointsOfInterest(int cityId)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        // GET one PointOfInteret for a city
        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterest>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        // CREATE PointOfInterest for a city
        [HttpPost]
        public async Task<ActionResult<PointOfInterest>> CreatePointOfInterest(int cityId,
            PointOfInterestDto pointOfInterest)
        {
            var city = await _cityInfoRepository.GetCityByIdAsync(cityId);
            if (city == null)
            {
                return NotFound();
            }

            var newPointOfInterest = new PointOfInterest()
            {
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(newPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();
            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = newPointOfInterest.Id }, newPointOfInterest);
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
