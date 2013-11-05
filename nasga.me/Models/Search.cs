using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace nasga.me.Models
{
    public class Search
    {
        [Required(ErrorMessage = "Class required.")]
        [Display(Name = "Class")]
        public string AthleteClass { get; set; }
        [Required(ErrorMessage = "Year required.")]
        public string Year { get; set; }
        [Required(ErrorMessage = "Name required.")]
        public string Name { get; set; }
    }
}