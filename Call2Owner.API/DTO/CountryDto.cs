namespace Oversight.DTO
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? CountryImage { get; set; }
        public bool IsFavourite { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

}
