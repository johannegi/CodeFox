using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeFox.Models.ViewModels
{
    public class AddFolderViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int ProjectID { get; set; }
    }
}