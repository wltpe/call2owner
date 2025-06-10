namespace Call2Owner.DTO
{
    public class SocietyFlatDTO
    {
        public Guid Id { get; set; }
        public Guid SocietyId { get; set; }
        public Guid? SocietyBuildingId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? FlatImage { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsActive { get; set; }
    }
}
