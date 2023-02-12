using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Models.City, Dtos.CityDtoWithoutPOI>();
            CreateMap<Models.City, Dtos.CityDto>();
        }
    }
}
