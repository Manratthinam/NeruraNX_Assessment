using System.ComponentModel.DataAnnotations;

namespace neuranx.Domain.RequestModel
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
