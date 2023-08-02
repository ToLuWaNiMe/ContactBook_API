namespace ContactBookApi.Models
{
    public class Contact 
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
