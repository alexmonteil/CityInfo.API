﻿
using AutoMapper;
using CityInfo.API.Dtos;
using CityInfo.API.Models;
using CityInfo.API.Repositories.Interfaces;
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
        private readonly IMapper _mapper;

        #endregion

        #region Constructors
        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var pointsOfInterests = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

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

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId);
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
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestDto pointOfInterestDto)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var updateTarget = await _cityInfoRepository.GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId);
            if (updateTarget == null) 
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterestDto, updateTarget);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }


        // PARTIAL UPDATE PointOfInterest for a city
        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestDto> patchDocument)
        {
            
            if (await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var updateTarget = await _cityInfoRepository.GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId);
            if (updateTarget == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestDto>(updateTarget);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, updateTarget);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        // DELETE Point of Interest
        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var deleteTarget = await _cityInfoRepository.GetPointOfInterestForCityByIdAsync(cityId, pointOfInterestId);
            if (deleteTarget == null)
            {
                return NotFound();
            }


            _cityInfoRepository.DeletePointOfInterest(deleteTarget);
            await _cityInfoRepository.SaveChangesAsync();
            return NoContent();
        }

        #endregion

        #region Private Methods

        

        #endregion
    }
}
