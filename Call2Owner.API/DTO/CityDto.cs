namespace Call2Owner.DTO
{
    public class CityDto
    {
        public int Id { get; set; }
        public int StateId { get; set; }
        public string Name { get; set; } 
        public string? Description { get; set; }
        public string? CityImage { get; set; }
        public bool IsFavourite { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

}
