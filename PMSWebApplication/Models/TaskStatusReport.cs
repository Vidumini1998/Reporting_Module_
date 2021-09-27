using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class TaskStatusReport
    {
        public int Id { get; set; }

        public DateTime? Deadline { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Task Status")]
        public string TaskStatus { get; set; }         
    }
}