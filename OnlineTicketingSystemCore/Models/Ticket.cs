using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Web;
//using OnlineTicketingSystemCore.Resources;

namespace OnlineTicketingSystemCore.Models
{
    public class Ticket
    {
        [Key]
        public int TicketId { get; set; }

        [Display(Name = "Project", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ProjectRequired")]
        [ForeignKey("Project")]
        public int ProjectRefId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Department", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "DepartmentRequired")]
        [ForeignKey("Department")]
        public int DeptRefId { get; set; }
        public Department Department { get; set; }

        [Display(Name = "Employee", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "EmployeeRequired")]
        [ForeignKey("Employee")]
        public int EmpRefId { get; set; }
        public Employee Employee { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "DescriptionRequired")]
        [StringLength(400)]
        //[Display(Name = "Problem Description")]
        //[Required]
        public string Description { get; set; }

        [Display(Name = "SubmitDate", ResourceType = typeof(Language))]
        [Required]
        //[Display(Name = "Submit Date")]
        //[DataType(DataType.Date)]
        public DateTime SubmitDate { get; set; }

        //[DefaultValue("O")]
        [Display(Name = "Status", ResourceType = typeof(Language))]
        [StringLength(1)]
        [Required]
        public string Status { get; set; } = "O";
    }
}
