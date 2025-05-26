namespace Oversight.DTO
{
    public class SocietyBuildingCTO
    {
        public int Id { get; set; } // Include for GET and PUT
        public int SocietyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BuildingImage { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsActive { get; set; }
    }
}
