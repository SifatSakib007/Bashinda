using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bashinda.Models
{
    public class UserOTP
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string UserId { get; set; }
        
        [Required]
        public required string Code { get; set; }
        
        [Required]
        public DateTime Expiry { get; set; }
        
        [ForeignKey("UserId")]
        public required ApplicationUser User { get; set; }
    }
}
