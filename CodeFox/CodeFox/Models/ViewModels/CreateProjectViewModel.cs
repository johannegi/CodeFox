using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeFox.Models.ViewModels
{
    public class CreateProjectViewModel
    {
        public List<string> TypeList;

        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string ReadMe { get; set; }
    }
}