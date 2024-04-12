using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIHarmony.Models
{
    public class ConfidentialWords
    {
        [Key]
        public Guid ConfidentialWordId { get; set; }

        [Required]
        public String Word { get; set; }

        [ForeignKey("Users")]
        public Guid UserId { get; set; }


        public Users Users { get; set; }
    }
}
