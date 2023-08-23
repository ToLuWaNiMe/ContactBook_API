using System.ComponentModel.DataAnnotations;

namespace ContactBookApi.Models
{
    public class ContactDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;



        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
            ErrorMessage = "Invalid email format. Sample format: name@example.com")]
        public string EmailAddress { get; set; } = string.Empty;



        public string PhoneNumber { get; set; } = string.Empty;



        public string PhotoURL { get; set; } = string.Empty;
    }
}
