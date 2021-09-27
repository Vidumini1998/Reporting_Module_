using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class ProjectStatusReport
    {
        public int Id { get; set; }

        public DateTime? Deadline { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Project Status")]
        public string ProjectStatus { get; set; }
    }
}