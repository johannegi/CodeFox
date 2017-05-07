using CodeFox.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace CodeFox.Models.ViewModels
{
    public class AddFilesViewModel
    {
        public List<string> TypeList;

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }
        
        public Project TheProject { get; set; }
    }
}

