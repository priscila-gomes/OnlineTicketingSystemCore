using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketingSystemCore.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DeptId { get; set; }

        [Display(Name = "Department")]
        [StringLength(100, MinimumLength = 2)]
        [Required]
        public string DeptName { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
