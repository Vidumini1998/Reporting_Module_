using CrystalDecisions.CrystalReports.Engine;
using PMSWebApplication.Models;
using PMSWebApplication.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PMSWebApplication.Controllers
{
    public class ReportingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public async Task<ActionResult> Index(int ProjectId)
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");
            var payments = db.Payments.Include(p => p.Project).Include(p => p.Task).Where(p => p.ProjectId == ProjectId);
            return View(await payments.ToListAsync());
        }

        //Get: Payment History Report
        public async Task<ActionResult> PaymentHistoryReport(int? id)
        {
            List<PaymentHistoryReport> ProjectId = new List<PaymentHistoryReport>();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");

            var payments = await db.Payments.ToListAsync();
            List<PaymentHistoryReport> paymentHistoryReport = new List<PaymentHistoryReport>();

            foreach (var item in payments)
            {
                PaymentHistoryReport paid = new PaymentHistoryReport();
            
                if (id == null || id == item.Project.Id)
                {
                    paid.Id = item.Id;
                    paid.PayDate = item.PayDate;
                    paid.ProjectName = item.Project.ProjectName;
                    paid.TaskName = item.Task.TaskName;
                    paid.PayMethod = item.PayMethod;
                    paid.Description = item.PayDiscription;
                    paid.PaymentAmount = item.PaymentAmount;
                }
                else continue;
               

                

                paymentHistoryReport.Add(paid);
            }
            return View(paymentHistoryReport);

        }

        //Export Payment History Report
        public async Task<ActionResult> ExportPaymentHistoryReport(int? id)
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("//Reports//PaymentHistoryReport.rpt")));

            if (id == null)
            {
                rd.SetDataSource(db.Payments.Select(c => new
                {
                    ProjectId = c.Project.ProjectName.ToString(),
                    TaskId = c.Task.TaskName.ToString(),
                    PayDiscription = c.PayDiscription,
                    TaskStages = c.PayDate.ToString(),
                    InvoiceNo = c.PayMethod.ToString(),
                    PayMethod = c.PaymentAmount.ToString()

                }).ToList());
            } else
            {
                rd.SetDataSource(db.Payments.Where(x => x.ProjectId == id).Select(c => new
                {
                    ProjectId = c.Project.ProjectName.ToString(),
                    TaskId = c.Task.TaskName.ToString(),
                    PayDiscription = c.PayDiscription,
                    TaskStages = c.PayDate.ToString(),
                    InvoiceNo = c.PayMethod.ToString(),
                    PayMethod = c.PaymentAmount.ToString()

                }).ToList());
            }

           

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Payment_History_Report.pdf");
        }

        //Get: DueAmountReport
        public async Task<ActionResult> DueAmountReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<DueAmountReport> duePaymentReport = new List<DueAmountReport>();

            foreach (var task in tasks)
            {
                DueAmountReport paid = new DueAmountReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                var payment = await db.Payments.FindAsync(task.ProjectId);
                var total = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();

                    paid.Id = payment.Id;
                    paid.PayDate = payment.PayDate;              
                    paid.ProjectName = project.ProjectName;
                    paid.TaskName = task.TaskName;
                    paid.PaidAmount = payment.PaymentAmount;

                duePaymentReport.Add(paid);
            }
            return View(duePaymentReport);
        }

        //Export Due Amount Report
        public async Task<ActionResult> ExportDueAmountReport(int? id)
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("//Reports//DueAmountReport.rpt")));

            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<DueAmountReport> duePaymentReport = new List<DueAmountReport>();

            if (id == null)
            {
                rd.SetDataSource(db.Payments.Select(c => new
                {
                    ProjectName = c.Project.ProjectName.ToString(),
                    TaskName = c.PayDate.ToString(),
                    InvoiceNo = c.PaymentAmount.ToString()

                }).ToList());
            }
            else
            {
                rd.SetDataSource(db.Payments.Select(c => new
                {
                    ProjectName = c.Project.ProjectName.ToString(),
                    TaskName = c.PayDate.ToString(),
                    InvoiceNo = c.PaymentAmount.ToString()

                }).ToList());
            }

                Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Due_Amount_Report.pdf");
        }

        //Get: Monthly Report
        public async Task<ActionResult> MonthlyReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<MonthlyReport> monthlyReport = new List<MonthlyReport>();

            foreach (var task in tasks)
            {
                MonthlyReport month = new MonthlyReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                var payment = await db.Payments.FindAsync(task.ProjectId);
                var contact = await db.Contacts.FindAsync(task.ProjectId);
                var count = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();

                month.Id = task.Id;
                month.Deadline = task.Deadline;
                month.ProjectName = project.ProjectName;
                month.TaskName = task.TaskName;
                month.TaskStatus = task.TaskStatus;
                month.TotalPayment = task.TaskWisePayment;

                monthlyReport.Add(month);
            }
            return View(monthlyReport);
        }

        [HttpPost]
        public async Task<ActionResult> MonthlyReport(DateTime? Sdate)
        {
            ViewBag.Sdate = new SelectList(db.Tasks, "Id", "Sdate");
            var tasks = db.Tasks.Include(p => p.Project).Include(p => p.Payments);
            return View(await tasks.ToListAsync());
        }

        //[HttpPost]
        public async Task<ActionResult> ExportMonthlyReport(int? id )
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("//Reports//MonthlyReport.rpt")));

            DateTime startDate;
            //string ss = s.ToString();

            startDate = DateTime.Parse("2021-05-01");
            //startDate = DateTime.Parse(ss);
            DateTime endDate = DateTime.Parse("2021-05-08");


            rd.SetDataSource(db.Payments.Where(x => x.PayDate < endDate && x.PayDate > startDate).Select(c => new
            {
                InvoiceNo = id ,
                ProjectId = c.Project.ProjectName,
                TaskId = c.Task.TaskName,
                TaskStages = c.Task.TaskStages,
                PayMethod = c.PayMethod,
                PayDiscription = c.PayDiscription,

            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Monthly_Report.pdf");
        }

        // GET: Upcomming Payements
        public async Task<ActionResult> UpcomingPaymentReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<UpcomingPaymentReport> upcomingPayments = new List<UpcomingPaymentReport>();

            foreach (var task in tasks)
            {
                UpcomingPaymentReport paid = new UpcomingPaymentReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                var payment = await db.Payments.FindAsync(task.ProjectId);
                //var contact = await db.Contacts.FindAsync(task.ProjectId);
                var count = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id && x.TaskStages == task.TaskStages).Select(x => x.PaymentAmount).Sum();
                //stages nathuwa ganna code eka //var count = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();

                //System.Diagnostics.Debug.WriteLine(count);
                //System.Diagnostics.Debug.WriteLine(task.TaskWisePayment);

                paid.Id = task.Id;
                paid.Deadline = task.Deadline;
                paid.ProjectName = project.ProjectName;
                paid.TaskName = task.TaskName;
                paid.TotalPayment = task.TaskWisePayment;
                paid.PaidAmount = payment.PaymentAmount;
                paid.Balance = (task.TaskWisePayment - count);
                paid.TaskStages = task.TaskStages;

                upcomingPayments.Add(paid);
            }

            return View(upcomingPayments);
        }

        //Export Upcoming Payment Report
        public async Task<ActionResult> ExportUpcomingPaymentReport()
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("//Reports//UpcomingPaymentReport+.rpt")));

            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<UpcomingPaymentReport> upcomingPaymentReport = new List<UpcomingPaymentReport>();

            rd.SetDataSource(db.Payments/*.Where(x => x.ProjectId == task.Id)*/.Select(c => new
            {
                ProjectId = c.Project.ProjectName.ToString(),
                TaskId = c.Task.TaskName.ToString(),
                PayDiscription = c.PayDiscription,
                TaskStages = c.PayDate.ToString(),
                InvoiceNo = c.InvoiceNo.ToString(),
                PayMethod = c.PaymentAmount.ToString()

            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Upcoming_Payment_Report.pdf");
        }

        //Get: Project Status Report
        public async Task<ActionResult> ProjectStatusReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<ProjectStatusReport> projectStatusReport = new List<ProjectStatusReport>();

            foreach (var task in tasks)
            {
                ProjectStatusReport projectStatus = new ProjectStatusReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                //var total = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();

                projectStatus.Id = task.Id;
                projectStatus.Deadline = task.Deadline;
                projectStatus.EmployeeName = task.AssignedEmployee;
                projectStatus.ProjectName = project.ProjectName;
                projectStatus.ProjectStatus = task.TaskStages;

                projectStatusReport.Add(projectStatus);
            }
            return View(projectStatusReport);

        }

        //Get: TaskStatusReport
        public async Task<ActionResult> TaskStatusReport(int? id)
        {
            List<PaymentHistoryReport> ProjectId = new List<PaymentHistoryReport>();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<TaskStatusReport> taskStatusReport = new List<TaskStatusReport>();

            foreach (var task in tasks)
            {
                TaskStatusReport taskStatus = new TaskStatusReport();
                var project = await db.Projects.FindAsync(task.ProjectId);

                taskStatus.Id = task.Id;
                taskStatus.Deadline = task.Deadline;
                taskStatus.EmployeeName = task.AssignedEmployee;
                taskStatus.ProjectName = project.ProjectName;
                taskStatus.TaskName = task.TaskName;
                taskStatus.TaskStatus = task.TaskStatus;

                taskStatusReport.Add(taskStatus);
            }
            return View(taskStatusReport);

        }

        //ExportTaskStatusReport
        public async Task<ActionResult> ExportTaskStatusReport()
        {

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("//Reports//TaskStatusReport.rpt")));

            //var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<TaskStatusReport> taskStatusReport = new List<TaskStatusReport>();

            rd.SetDataSource(db.Tasks.Select(c => new
            {
                ProjectName = c.Project.ProjectName.ToString(),
                TaskName = c.TaskName.ToString(),
                ClientName = c.Deadline.ToString(),
                TaskStatus = c.TaskStatus.ToString(),
                EmployeeName = c.AssignedEmployee.ToString()
               
            }).ToList());

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
            rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
            rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "Task_Status_Report.pdf");
        }

        //public ActionResult ExportTaskStatusReport()
        //{

        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(Path.Combine(Server.MapPath("\\Reports\\TaskStatusReport.rpt")));

        //    rd.SetDataSource(db.Tasks/*.Where(x => x.ProjectId == Task.CurrentId)*/.Select(c => new
        //    {
        //        ProjectName = c.ProjectId.ToString(),
        //        TaskName = c.TaskName.ToString(),
        //        TaskStatus = c.TaskStatus.ToString(),
        //        EmployeeName = c.AssignedEmployee.ToString(),

        //    }).ToList());

        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();


        //    rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
        //    rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
        //    rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

        //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    stream.Seek(0, SeekOrigin.Begin);

        //    return File(stream, "application/pdf", "CustomerList.pdf");
        //}

        //public async Task<ActionResult> ExportTaskStatusReport1()
        //{
        //    ReportDocument rd = new ReportDocument();
        //    rd.Load(Path.Combine(Server.MapPath("//Reports//TaskStatusReport.rpt")));
        //    var project = await db.Projects.ToListAsync();

        //    foreach (var task in project)
        //    {
        //        rd.SetDataSource(db.Tasks.Where(x => x.ProjectId == task.Id).Select(c => new
        //        {
        //            ProjectName = c.ProjectId.ToString(),
        //            TaskName = c.TaskName.ToString(),
        //            TaskStatus = c.TaskStatus.ToString(),
        //            EmployeeName = c.AssignedEmployee.ToString(),

        //        }).ToList());

        //    }

        //    Response.Buffer = false;
        //    Response.ClearContent();
        //    Response.ClearHeaders();

        //    rd.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
        //    rd.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
        //    rd.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

        //    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //    stream.Seek(0, SeekOrigin.Begin);

        //    return File(stream, "application/pdf", "TaskstatusReport.pdf");
        //}


        //Get: Revenue Report
        public async Task<ActionResult> RevenueReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<RevenueReport> revenueReport = new List<RevenueReport>();

            foreach (var task in tasks)
            {
                RevenueReport paid = new RevenueReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                var payment = await db.Payments.FindAsync(task.ProjectId);
                var contact = await db.Contacts.FindAsync(task.ProjectId);
                var count = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();

                paid.Id = task.Id;
                paid.PayDate = payment.PayDate;
                paid.InvoiceNo = payment.InvoiceNo;
                paid.ProjectName = project.ProjectName;
                paid.PaymentMethod = payment.PayMethod;
                paid.Description = payment.PayDiscription;
                paid.PaymentAmount = payment.PaymentAmount;


                revenueReport.Add(paid);
            }
            return View(revenueReport);

        }
        
        //Get: Income Report
        public async Task<ActionResult> IncomeReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<IncomeReport> incomeReport = new List<IncomeReport>();

            foreach (var task in tasks)
            {
                IncomeReport income = new IncomeReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                var payment = await db.Payments.FindAsync(task.ProjectId);
                var contact = await db.Contacts.FindAsync(task.ProjectId);
                var count = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();
            }
            return View(incomeReport);
        }

        //Get: DeadlineListReport
        public async Task<ActionResult> DeadlineListReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<DeadlineListReport> deadlineListReport = new List<DeadlineListReport>();

            foreach (var task in tasks)
            {
                DeadlineListReport deadlines = new DeadlineListReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
                //var total = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id).Select(x => x.PaymentAmount).Sum();

                deadlines.Id = task.Id;
                deadlines.ProjectName = project.ProjectName;
                deadlines.TaskName = task.TaskName;
                deadlines.Deadline = task.Deadline;

                deadlineListReport.Add(deadlines);
            }
            return View(deadlineListReport);

        }

        //Get: Priority Report
        public async Task<ActionResult> PriorityReport()
        {
            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<PriorityReport> priorityrep = new List<PriorityReport>();

            foreach (var task in tasks)
            {
                PriorityReport priority = new PriorityReport();
                var project = await db.Projects.FindAsync(task.ProjectId);
               
                priority.Id = task.Id;
                priority.ProjectName = project.ProjectName;             
                priority.Deadline = task.Deadline;

                priorityrep.Add(priority);
            }
            return View(priorityrep);

        }
    }
}


