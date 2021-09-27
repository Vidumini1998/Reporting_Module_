using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class MonthlyReport
    {
        public int Id { get; set; }

        public DateTime? Deadline { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        public string BugFixes { get; set; }

        [Display(Name = "Task Status")]
        public string TaskStatus { get; set; }

        [Display(Name = "Total Payment")]
        public decimal? TotalPayment { get; set; }

    }
}