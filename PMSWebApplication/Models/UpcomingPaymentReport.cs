using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class UpcomingPaymentReport
    {
        public int Id { get; set; }

        public DateTime? Deadline { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Task Stageses")]
        public string TaskStages { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Total Payment")]
        public decimal? TotalPayment { get; set; }

        [Display(Name = "Paid Amount")]
        public decimal? PaidAmount { get; set; }

        public decimal? Balance { get; set; }

    }
}