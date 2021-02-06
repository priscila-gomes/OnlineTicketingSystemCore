using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketingSystemCore.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Display(Name = "Project")]
        [StringLength(100)]
        [Required]
        public string ProjectTitle { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
