using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OnlineTicketingSystemCore.Data;
using OnlineTicketingSystemCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Google.DataTable.Net.Wrapper;
using Google.DataTable.Net.Wrapper.Extension;

namespace OnlineTicketingSystemCore.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ILogger<TicketsController> _logger;
        private readonly OnlineTicketingSystemCoreContext _context;
        private IStringLocalizer<Language> _sharedLocalizer;
        
        public TicketsController(ILogger<TicketsController> logger, IStringLocalizer<Language> sharedLocalizer, OnlineTicketingSystemCoreContext context)
        {
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
        }

        public IActionResult Index(string sortOrder)
        {
            //var tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);
            IQueryable<Ticket> tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);

            ViewBag.DeptSortParm = String.IsNullOrEmpty(sortOrder) ? "dept_desc" : "";
            ViewBag.EmpSortParm = sortOrder == "Emp" ? "emp_desc" : "Emp";
            ViewBag.ProjSortParm = sortOrder == "Proj" ? "proj_desc" : "Proj";
            ViewBag.DesSortParm = sortOrder == "Des" ? "dec_desc" : "Des";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";

            //var students = from s in _context.Students select s;

            switch (sortOrder)
            {
                case "dept_desc":
                    tickets = tickets.OrderByDescending(t => t.Department.DeptName);
                    break;
                case "Emp":
                    tickets = tickets.OrderBy(t => t.Employee.EmpName);
                    break;
                case "emp_desc":
                    tickets = tickets.OrderByDescending(t => t.Employee.EmpName);
                    break;
                case "Proj":
                    tickets = tickets.OrderBy(t => t.Project.ProjectTitle);
                    break;
                case "proj_desc":
                    tickets = tickets.OrderByDescending(t => t.Project.ProjectTitle);
                    break;
                case "Des":
                    tickets = tickets.OrderBy(t => t.Description);
                    break;
                case "des_desc":
                    tickets = tickets.OrderByDescending(t => t.Description);
                    break;
                case "Date":
                    tickets = tickets.OrderBy(t => t.SubmitDate);
                    break;
                case "date_desc":
                    tickets = tickets.OrderByDescending(t => t.SubmitDate);
                    break;
                case "Status":
                    tickets = tickets.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    tickets = tickets.OrderByDescending(t => t.Status);
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Department.DeptName);
                    break;
            }

            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            //Ticket ticket = await _context.Tickets.FindAsync(id);

            var ticket = _context.Tickets.Where(t => t.TicketId == id).Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);

            if (ticket == null)
            {
                return NotFound();
            }
            return View(await ticket.FirstOrDefaultAsync());
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewBag.DeptRefId = new SelectList(_context.Departments, "DeptId", "DeptName");
            ViewBag.EmpRefId = new List<SelectListItem> { }; //new SelectList(_context.Employees, "EmpId", "EmpName");
            ViewBag.ProjectRefId = new SelectList(_context.Projects, "ProjectId", "ProjectTitle");
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection) 
        //[Bind(Include = "TicketId,ProjectRefId,DeptRefId,EmpRefId,Description,SubmitDate,Status")] Ticket ticket
        {
            try
            {
                // TODO: Add insert logic here
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Ticket ticket = new Ticket();

                ticket = new Ticket
                {
                    ProjectRefId = Convert.ToInt32(collection["ProjectRefId"]) ,
                    DeptRefId = Convert.ToInt32(collection["DeptRefId"]),
                    EmpRefId = Convert.ToInt32(collection["EmpRefId"]),
                    Description = collection["Description"],
                    SubmitDate = System.DateTime.Now 
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult getEmployee(string DeptRefId)
        {
            if (!string.IsNullOrEmpty(DeptRefId))
            {
                int _deptId = Convert.ToInt32(DeptRefId); //.ToInt32(DeptRefId);
                var empList = _context.Employees.Include(t => t.Department).Where(t => t.DeptRefId == _deptId);

                var classesData = empList.Select(m => new SelectListItem()
                {
                    Text = m.EmpName,
                    Value = m.EmpId.ToString(),
                });
                return Json(classesData);
            }
            else
            {
                return Json(null);
            }
        }
        
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            int _deptId = ticket.DeptRefId;
            var empList = _context.Employees.Include(t => t.Department).Where(t => t.DeptRefId == _deptId);

            List<SelectListItem> status = new List<SelectListItem>() {
                                                new SelectListItem {    Text = "Open", Value = "O"  },
                                                new SelectListItem {    Text = "Close", Value = "C" }
                                            };

            ViewBag.DeptRefId = new SelectList(_context.Departments, "DeptId", "DeptName", ticket.DeptRefId);
            ViewBag.EmpRefId = new SelectList(empList, "EmpId", "EmpName", ticket.EmpRefId); //new SelectList(_context.Employees, "EmpId", "EmpName", ticket.EmpRefId);
            ViewBag.ProjectRefId = new SelectList(_context.Projects, "ProjectId", "ProjectTitle", ticket.ProjectRefId);
            ViewBag.Status = new SelectList(status, "Value", "Text", ticket.Status);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // TODO: Add update logic here
                Ticket ticket = new Ticket();
                
                ticket = new Ticket
                {
                    TicketId = Convert.ToInt32(collection["TicketId"]),
                    ProjectRefId = Convert.ToInt32(collection["ProjectRefId"]),
                    DeptRefId = Convert.ToInt32(collection["DeptRefId"]),
                    EmpRefId = Convert.ToInt32(collection["EmpRefId"]),
                    Description = collection["Description"],
                    SubmitDate = Convert.ToDateTime(collection["SubmitDate"]),
                    Status = collection["Status"]
                };

                _context.Entry(ticket).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Ticket ticket = await db.Tickets.FindAsync(id);
            var ticket = _context.Tickets.Where(t => t.TicketId == id).Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);

            if (ticket == null)
            {
                return NotFound();
            }
            return View(await ticket.FirstOrDefaultAsync());
        }

        // POST: Tickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //var ticket = _context.Tickets.Where(t => t.TicketId == id).Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);
                Ticket ticket = await _context.Tickets.FindAsync(id);

                if (ticket == null)
                {
                    return NotFound();
                }
                
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Chart()
        {
            return View();
        }       

        [HttpGet]
        public JsonResult ChartData(string ShowType)
        {
            try
            { 
                var tickets = _context.Tickets.Include(t => t.Project)
                                  .GroupBy(t => t.Project.ProjectTitle)
                                  .Select(g => new Charts { ProjectTitle = g.Key, TotTickets = g.Count() });
                //var coloum = Language.Project;
                var jsonData = tickets.ToGoogleDataTable()
                        .NewColumn(new Column(ColumnType.String, "Project"), x => x.ProjectTitle)
                        .NewColumn(new Column(ColumnType.Number, "Total Tickets"), x => x.TotTickets)
                        .Build()
                        .GetJson();

                switch (ShowType)
                {
                    case "DeptName":
                        tickets = _context.Tickets.Include(t => t.Department)
                                  .GroupBy(t => t.Department.DeptName)
                                  .Select(g => new Charts { DeptName = g.Key, TotTickets = g.Count() });
                        //coloum = Language.Department;
                        jsonData = tickets.ToGoogleDataTable()
                                    .NewColumn(new Column(ColumnType.String, "Department"), x => x.DeptName)
                                    .NewColumn(new Column(ColumnType.Number, "Total Tickets"), x => x.TotTickets)
                                    .Build()
                                    .GetJson();
                                    break;
                    case "EmpName":
                        tickets = _context.Tickets.Include(t => t.Employee)
                                  .GroupBy(t => t.Employee.EmpName)
                                  .Select(g => new Charts { EmpName = g.Key, TotTickets = g.Count() });
                        //coloum = Language.Employee;
                        jsonData = tickets.ToGoogleDataTable()
                                    .NewColumn(new Column(ColumnType.String, "Employee"), x => x.EmpName)
                                    .NewColumn(new Column(ColumnType.Number, "Total Tickets"), x => x.TotTickets)
                                    .Build()
                                    .GetJson();
                        break;
                    case "Year":
                        tickets = _context.Tickets
                                  //.GroupBy(t=>t.SubmitDate.Year)
                                  .GroupBy(t => new
                                  {
                                      Year = t.SubmitDate.Year
                                  })
                                  .Select(g => new Charts { Year = g.Key.Year, TotTickets = g.Count() });
                        //coloum = Language.Year;
                        jsonData = tickets.ToGoogleDataTable()
                                    .NewColumn(new Column(ColumnType.String, "Year"), x => x.Year)
                                    .NewColumn(new Column(ColumnType.Number, "Total Tickets"), x => x.TotTickets)
                                    .Build()
                                    .GetJson();
                        break;
                    case "Status":
                        tickets = _context.Tickets
                                  .GroupBy(t => t.Status)
                                  .Select(g => new Charts { Status = (g.Key == "O" ? "Open" : "Close"), TotTickets = g.Count() });
                        //coloum = Language.Status;
                        jsonData = tickets.ToGoogleDataTable()
                                    .NewColumn(new Column(ColumnType.String, "Status"), x => x.Status)
                                    .NewColumn(new Column(ColumnType.Number, "Total Tickets"), x => x.TotTickets)
                                    .Build()
                                    .GetJson();
                        break;
                }
                return Json(jsonData);
            }
            catch
            {
                return Json(null);
            }
        }        

        public IActionResult Search()
        {
            var tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);
            //return View(await tickets.ToListAsync());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string Search, string SearchString)
        {
            ViewBag.Method = "POST";
            //var tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);
            IQueryable<Ticket> tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);

            if (!string.IsNullOrEmpty(SearchString))
            {
                switch (Search)
                {
                    case "DeptName":
                        tickets = tickets.Where(s => s.Department.DeptName.Contains(SearchString));
                        break;
                    case "EmpName":
                        tickets = tickets.Where(s => s.Employee.EmpName.Contains(SearchString));
                        break;
                    case "Date":
                        var SearchText = Convert.ToDateTime(SearchString);
                        tickets = tickets.Where(s => s.SubmitDate.Year == SearchText.Year &&
                                                    s.SubmitDate.Month == SearchText.Month &&
                                                    s.SubmitDate.Day == SearchText.Day);
                        break;
                    case "ProjectTitle":
                        tickets = tickets.Where(s => s.Project.ProjectTitle.Contains(SearchString));
                        break;
                    case "Desc":
                        tickets = tickets.Where(s => s.Description.Contains(SearchString));
                        break;
                }
            }

            return View(await tickets.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            //ViewBag.Message = "What is Online Ticketing System?";

            return View();
        }

        public IActionResult Contact()
        {
            //ViewBag.Message = "Contact Address:";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}