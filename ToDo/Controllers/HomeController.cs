using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToDo.Models;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Task = ToDo.Models.Task;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        TaskContext db = new TaskContext();

        [HttpGet]
        public ActionResult Index()
        {
           var tasks = db.Tasks.Include(t => t.Project); // Select from 2 tables  //  LEFT OUTER JOIN

           return View(tasks);
        }
        
        [HttpGet]
        public ActionResult Create()
        {
            SelectList projects = new SelectList(db.Projects, "Id", "Name");
            ViewBag.Projects = projects;

            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Name,ProjectId")]Task task)
        {
            if (string.IsNullOrEmpty(task.Name) || task.Name.Length > 49)
            {
                ModelState.AddModelError("Name", "Error");
            }
            else if (task.ProjectId == null)
            {
                ModelState.AddModelError("ProjectId", "Error");
            }

            if (ModelState.IsValid)
            {
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(task);
        }
        [HttpGet]
        public ActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateProject([Bind(Include = "Id,Name")]Project project)
        {
            if (string.IsNullOrEmpty(project.Name) || project.Name.Length > 49)
            {
                ModelState.AddModelError("Name", "Error");
            }

            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task tasks = await db.Tasks.FindAsync(id);
            if (tasks != null)
            {
                SelectList projects = new SelectList(db.Projects, "Id", "Name", tasks.ProjectId);
                ViewBag.Projects = projects;
                return View(tasks);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ProjectId")]Task task)
        {
            if (string.IsNullOrEmpty(task.Name) || task.Name.Length > 49)
            {
                ModelState.AddModelError("Name", "Error");
            }
            else if (task.ProjectId == null)
            {
                ModelState.AddModelError("ProjectId", "Error");
            }

            if (ModelState.IsValid)
            {
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task tasks = await db.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return HttpNotFound();
            }

            return View(tasks);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Task task = await db.Tasks.FindAsync(id);
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult TasksSearch(string name)
        {
            var alltasks = db.Tasks.Include(t => t.Project).Where(a => a.Name.Contains(name)).ToList();
            if (alltasks.Count <= 0)
            {
                return PartialView("ErrorFound");
            }
            return PartialView(alltasks);
        }

        [HttpPost]
        public ActionResult Reset()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult JsonParse(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult JsonParse()
        {
            string json = "", path = "";

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName); 
                    path = Path.Combine(Server.MapPath("~/"), fileName);
                    file.SaveAs(path);
                }
                try
                {
                    json = System.IO.File.ReadAllText(path);
                }
                catch (Exception)
                {
                    return View();
                }

                List<Task> TaskList;
                try
                {
                    var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                    TaskList = ser.Deserialize<List<Task>>(json);
                }
                catch (Exception)
                {
                    return RedirectToAction("Index");
                }

                db.Tasks.AddRange(TaskList);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult XmlParse(int? id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult XmlParse()
        {
            string xml = "", path = "";

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/"), fileName);
                    file.SaveAs(path);
                }
                try
                {
                    xml = System.IO.File.ReadAllText(path);
                }
                catch (Exception)
                {
                    return View();
                }

                IEnumerable<Task> TaskList;

                try
                {
                    XDocument doc = XDocument.Parse(xml);

                    TaskList = from c in doc.Descendants("Component")
                                    select new Task()
                                    {
                                        Id = (int)c.Attribute("id"),
                                        Name = (string)c.Attribute("name"),
                                        ProjectId = (int)c.Attribute("projectId")
                                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return RedirectToAction("Index");
                }

                db.Tasks.AddRange(TaskList);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}