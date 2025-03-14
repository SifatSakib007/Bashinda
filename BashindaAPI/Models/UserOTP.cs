using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BashindaAPI.Models
{
    public class UserOTP
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Code { get; set; }

        public DateTime Expiry { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
} 