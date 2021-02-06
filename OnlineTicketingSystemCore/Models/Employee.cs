using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketingSystemCore.Models
{
    public class Employee
    {
        [Key]
        public int EmpId { get; set; }

        [Display(Name = "Employee")]
        [StringLength(100)]
        [Required]
        public string EmpName { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

        [ForeignKey("Department")]
        public int DeptRefId { get; set; }
        public Department Department { get; set; }
    }
}
