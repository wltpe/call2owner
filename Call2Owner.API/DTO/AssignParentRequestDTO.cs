namespace Call2Owner.DTO
{
    public class AssignParentRequestDTO
    {
        public List<Guid>? UserIds { get; set; }
        public Guid ParentId { get; set; }
    }
}
