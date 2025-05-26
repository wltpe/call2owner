namespace Oversight.DTO
{
    public class SocietyFlatDTO
    {
        public int Id { get; set; }
        public int SocietyId { get; set; }
        public int? SocietyBuildingId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FlatImage { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsActive { get; set; }
    }
}
