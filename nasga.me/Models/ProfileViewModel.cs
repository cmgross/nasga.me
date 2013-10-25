using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace nasga.me.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "First name required.")]
        [StringLength(5, ErrorMessage = "First name cannot exceed 5 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Class required.")]
        public string AthleteClass { get; set; }
        public List<SelectListItem> Classes { get; set; }

        public ProfileViewModel(string firstName = "", string lastName = "", string className = "")
        {
            FirstName = firstName;
            LastName = lastName;
            //put this in webconfig
            var classes = new List<string> {"", "Pro", "Masters", "Pro+Masters", "Amateur", "Womens"};
            Classes = classes.Select(p => new SelectListItem
            {
                Text = p,
                Value = p,
                Selected = p == className
            }).ToList();
        }

        public ProfileViewModel()
        {
            var classes = new List<string> { "", "Pro", "Masters", "Pro+Masters", "Amateur", "Womens" };
            Classes = classes.Select(p => new SelectListItem
            {
                Text = p,
                Value = p,
                Selected = p == ""
            }).ToList();
        }
    }
}