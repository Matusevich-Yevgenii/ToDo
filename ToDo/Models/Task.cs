using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace ToDo.Models
{
    [Serializable]
    [XmlRoot("task")]
    public class Task
    {
        public Task() { }

        [XmlAttribute("id")]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [XmlElement("name")]
        [Required]
        [StringLength(49, MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [XmlElement("projectid")]
        [Required]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [XmlElement("project")]
        public Project Project { get; set; }
    }
}