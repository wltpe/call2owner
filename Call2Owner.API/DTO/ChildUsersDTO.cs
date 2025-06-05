//using Call2Owner.Model;

namespace Call2Owner.DTO
{
    public class ChildUsersDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<ChildUsersDTO> Children { get; set; } = new();
    }
}
