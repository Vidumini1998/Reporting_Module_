using System;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApplication.Models
{
    public class PriorityReport
    {
        public int Id { get; set; }

        public DateTime? Deadline { get; set; }

        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Priority No.")]
        public int? PriorityNo { get; set; }
    }
}