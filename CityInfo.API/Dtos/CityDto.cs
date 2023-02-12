using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }
        public ICollection<PointOfInterest> PointsOfInterest { get; set; } = new List<PointOfInterest>();
        public int NumberOfPointsOfInterest => PointsOfInterest.Count;
    }
}
