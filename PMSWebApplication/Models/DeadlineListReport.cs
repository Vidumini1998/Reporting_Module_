using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class DeadlineListReport
    {

        public int Id { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        public DateTime? Deadline { get; set; }

    }
}