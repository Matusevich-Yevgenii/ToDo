using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ToDo.Models
{
    public class TaskDbInit : DropCreateDatabaseAlways<TaskContext>
    {
        protected override void Seed(TaskContext db)
        {
            db.Tasks.Add(new Task { Name = "Go out to dinner", ProjectId = 1});
            db.Tasks.Add(new Task { Name = "Meeting with boss", ProjectId = 2 });
            db.Tasks.Add(new Task { Name = "Pickup kids at school", ProjectId = 1 });

            db.Projects.Add(new Project { Name = "Home" });
            db.Projects.Add(new Project { Name = "Work" });

            base.Seed(db);
        }
    }
}