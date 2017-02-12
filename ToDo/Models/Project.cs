using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ToDo.Models
{
    [Serializable]
    public class Project
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [StringLength(49, MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public Project()
        {
            Tasks = new List<Task>();
        }
    }
}