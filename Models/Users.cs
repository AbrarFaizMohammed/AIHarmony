using System.ComponentModel.DataAnnotations;

namespace AIHarmony.Models
{
    public class Users
    {
        [Key]
        public Guid usedId { get; set; }
        [Required]
        public String firstName { get; set; }
        [Required]
        public String lastName { get; set; }
        [Required]
        public String emailId { get; set; }
        [Required]
        public String password { get; set; }
    }
}
