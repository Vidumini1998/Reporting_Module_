using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class DueAmountReport
    {
        public int Id { get; set; }

        [Display(Name = "Pay Date")]
        public DateTime? PayDate { get; set; }

        [Display(Name = "Invoice No.")]
        public int InvoiceNo { get; set; }

        //public string ClientName { get; set; }     

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Task Name")]
        public string TaskName { get; set; }

        [Display(Name = "Paid Amount")]
        public decimal? PaidAmount { get; set; }
    }
}