using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OnlineTicketingSystemCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineTicketingSystemCore.Data;

namespace OnlineTicketingSystemCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IStringLocalizer<Language> _sharedLocalizer;
        private readonly OnlineTicketingSystemCoreContext _context;

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<Language> sharedLocalizer, OnlineTicketingSystemCoreContext context)
        {
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _context = context;
        }

        public IActionResult Index(string sortOrder)//, string language)
        {

            /*if (!String.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }
            else
            {
                language = "";
                //HttpCookie cookie1 = Request.Cookies["Languages"];
                var cookie1 = HttpContext.Request.Cookies["Languages"];

                //if (cookie1 != null && cookie1.Value != null)
                if (cookie1 != null)// && cookie1.Value != null)
                {
                    //language = cookie1.Value;
                    language = cookie1;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
                }
            }

            CookieOptions cookieOptions = new CookieOptions();
            HttpContext.Response.Cookies.Append("Languages", language, cookieOptions);*/

            //string a = _sharedLocalizer[Language.Home];

            /*HttpCookie cookie = new HttpCookie("Languages");
            cookie.Value = language;
            Response.Cookies.Add(cookie);*/

            //var tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);
            IQueryable<Ticket> tickets = _context.Tickets.Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);

            ViewBag.DeptSortParm = String.IsNullOrEmpty(sortOrder) ? "dept_desc" : "";
            ViewBag.EmpSortParm = sortOrder == "Emp" ? "name_desc" : "Emp";
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
                case "Emp_desc":
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

            //return View(await tickets.ToListAsync());
            return View(tickets.ToList());
        }


        // GET: Tickets
        /*public ActionResult Index()
        {
            return View();
        }*/

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }*/
            //Ticket ticket = await _context.Tickets.FindAsync(id);

            var ticket = _context.Tickets.Where(t => t.TicketId == id).Include(t => t.Department).Include(t => t.Employee).Include(t => t.Project);

            if (ticket == null)
            {
                //return HttpNotFound();
                return NotFound();
            }
            return View(await ticket.FirstOrDefaultAsync());
            //return View();
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewBag.DeptRefId = new SelectList(_context.Departments, "DeptId", "DeptName");
            ViewBag.EmpRefId = new List<SelectListItem> { }; //new SelectList(_context.Employees, "EmpId", "EmpName");
            ViewBag.ProjectRefId = new SelectList(_context.Projects, "ProjectId", "ProjectTitle");
            return View();
            //return View();
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

                Ticket ticket = new Ticket();

                if (ModelState.IsValid)
                {
                    ticket = new Ticket
                    {
                        ProjectRefId = Convert.ToInt32(collection["ProjectRefId"]),
                        DeptRefId = Convert.ToInt32(collection["DeptRefId"].ToString()),
                        EmpRefId = Convert.ToInt32(collection["EmpRefId"].ToString()),
                        Description = collection["Description"],
                        SubmitDate = Convert.ToDateTime(collection["SubmitDate"]),
                        Status = collection["Status"]
                    };

                    //ticket.Status = ticket.Status.DefaultIfEmpty<Ticket>;
                    _context.Tickets.Add(ticket);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Index));
                }

                //Ticket ticket = new Ticket();
                ViewBag.DeptRefId = new SelectList(_context.Departments, "DeptId", "DeptName", ticket.DeptRefId);

                int _deptId = ticket.DeptRefId;
                var empList = _context.Employees.Include(t => t.Department).Where(t => t.DeptRefId == _deptId);

                ViewBag.EmpRefId = new SelectList(empList, "EmpId", "EmpName", ticket.EmpRefId); // new SelectList(_context.Employees, "EmpId", "EmpName", ticket.EmpRefId);
                ViewBag.ProjectRefId = new SelectList(_context.Projects, "ProjectId", "ProjectTitle", ticket.ProjectRefId);
                return View(ticket);
                //return RedirectToAction(nameof(Index));*/
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult getEmployee(string DeptRefId)
        {
            //int val = Ticket.
            //IEnumerable<SelectListItem> empList = new List<SelectListItem>();
            //List<SelectListItem> empList = new List<SelectListItem>();


            //var empList = _context.Employees.Include(t => t.Department);
            if (!string.IsNullOrEmpty(DeptRefId))
            {
                int _deptId = Convert.ToInt32(DeptRefId); //.ToInt32(DeptRefId);
                var empList = _context.Employees.Include(t => t.Department).Where(t => t.DeptRefId == _deptId);

                var classesData = empList.Select(m => new SelectListItem()
                {
                    Text = m.EmpName,
                    Value = m.EmpId.ToString(),
                });
                //return Json(classesData, JsonRequestBehavior.AllowGet);
                return Json(classesData);

                /*empList = (from e in _context.Employees where e.DeptRefId == _deptId select e)
                           .AsEnumerable()
                           .Select(m => new SelectListItem() { Text = m.EmpName, Value = m.EmpId.ToString() });*/
            }
            else
            {
                //return Json(null, JsonRequestBehavior.AllowGet);
                return Json(null);
            }
            //return new SelectList(empList, "Value", "Text", "EmpRefId");
            //return View((new SelectList(empList, "Value", "Text", "EmpRefId")).ToList());
            //ViewBag.EmpRefId = new SelectList(empList, "EmpId", "EmpName");
            //return View(new SelectList(empList, "EmpId", "EmpName", "EmpRefId"));

            //return View(new SelectList(empList, "EmpId", "EmpName"));
        }
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            /*if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }*/

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                //return HttpNotFound();
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
            //return View();
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                Ticket ticket = new Ticket();

                if (ModelState.IsValid)
                {
                    ticket = new Ticket
                    {
                        TicketId = Convert.ToInt32(collection["TicketId"]),
                        ProjectRefId = Convert.ToInt32(collection["ProjectRefId"]),
                        DeptRefId = Convert.ToInt32(collection["DeptRefId"].ToString()),
                        EmpRefId = Convert.ToInt32(collection["EmpRefId"].ToString()),
                        Description = collection["Description"],
                        SubmitDate = Convert.ToDateTime(collection["SubmitDate"]),
                        Status = collection["Status"]
                    };

                    _context.Entry(ticket).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.DeptRefId = new SelectList(_context.Departments, "DeptId", "DeptName", ticket.DeptRefId);

                int _deptId = ticket.DeptRefId;
                var empList = _context.Employees.Include(t => t.Department).Where(t => t.DeptRefId == _deptId);

                ViewBag.EmpRefId = new SelectList(empList, "EmpId", "EmpName", ticket.EmpRefId); //new SelectList(_context.Employees, "EmpId", "EmpName", ticket.EmpRefId);
                ViewBag.ProjectRefId = new SelectList(_context.Projects, "ProjectId", "ProjectTitle", ticket.ProjectRefId);

                List<SelectListItem> status = new List<SelectListItem>() {
                                                new SelectListItem {    Text = "Open", Value = "O"  },
                                                new SelectListItem {    Text = "Close", Value = "C" }
                                            };
                ViewBag.Status = new SelectList(status, "Value", "Text", ticket.Status);
                return View(ticket);


                //return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                /*if (id == null)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);
                }*/

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
                //return View(await ticket.FirstOrDefaultAsync());

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Chart()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Chart(string ShowType)
        {
            var tickets = _context.Tickets.Include(t => t.Project)
                              .GroupBy(t => t.Project.ProjectTitle)
                              .Select(g => new Charts { ProjectTitle = g.Key, TotTickets = g.Count() });

            switch (ShowType)
            {
                case "DeptName":
                    tickets = _context.Tickets.Include(t => t.Department)
                              .GroupBy(t => t.Department.DeptName)
                              .Select(g => new Charts { DeptName = g.Key, TotTickets = g.Count() });
                    break;
                case "EmpName":
                    tickets = _context.Tickets.Include(t => t.Employee)
                              .GroupBy(t => t.Employee.EmpName)
                              .Select(g => new Charts { EmpName = g.Key, TotTickets = g.Count() });
                    break;
                case "Year":
                    tickets = _context.Tickets
                              //.GroupBy(t=>t.SubmitDate.Year)
                              .GroupBy(t => new
                              {
                                  Year = t.SubmitDate.Year
                              })
                              .Select(g => new Charts { Year = g.Key.Year, TotTickets = g.Count() });
                    break;
                case "Status":
                    tickets = _context.Tickets
                              .GroupBy(t => t.Status)
                              .Select(g => new Charts { Status = (g.Key == "O" ? "Open" : "Close"), TotTickets = g.Count() });
                    break;
            }

            return View(await tickets.ToListAsync());
        }

        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string Search, string SearchString)
        {
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
        //private readonly ILogger<HomeController> _logger;
        //private IStringLocalizer<Language> _sharedLocalizer;

        /*public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        /*public HomeController(ILogger<HomeController> logger, IStringLocalizer<Language> sharedLocalizer)
        {
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
        }


        public IActionResult Index()
        {     
            return View();
        }*/

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
