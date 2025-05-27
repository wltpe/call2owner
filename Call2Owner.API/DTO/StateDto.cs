namespace Oversight.DTO
{
    public class StateDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Statemage { get; set; }
        public bool IsFavourite { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

}
