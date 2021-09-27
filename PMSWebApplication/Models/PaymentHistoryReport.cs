using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class PaymentHistoryReport
    {
        public int Id { get; set; }

        [Display(Name = "Pay Date")]
        public DateTime? PayDate { get; set; }

        public int InvoiceNo { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Pay Method")]
        public string PayMethod { get; set; }

        [Display(Name = "Pay Description")]
        public string Description { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal? PaymentAmount { get; set; }

    }
}