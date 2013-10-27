using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nasga.me.Helpers;

namespace nasga.me.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "First name required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Class required.")]
        [Display(Name = "Class")]
        public string AthleteClass { get; set; }
        public List<SelectListItem> Classes { get; set; }

        public ProfileViewModel(Dictionary<string, string> athleteInfo)
        {
        //http://stackoverflow.com/questions/2829873/how-can-i-detect-if-this-dictionary-key-exists-in-c
            string firstName;
            string lastName;
            string athleteClass;
            athleteInfo.TryGetValue(AppSettingsGet.FirstNameCookie, out firstName);
            athleteInfo.TryGetValue(AppSettingsGet.LastNameCookie, out lastName);
            athleteInfo.TryGetValue(AppSettingsGet.AthleteClassCookie, out athleteClass);
            FirstName = firstName;
            LastName = lastName;
            AthleteClass = athleteClass;
            var classes = new List<string> {string.Empty};
            classes.AddRange(AppSettingsGet.AthleteClasses);
            Classes = classes.Select(p => new SelectListItem
            {
                Text = p,
                Value = p,
                Selected = p == AthleteClass
            }).ToList();
        }

        public ProfileViewModel()
        {
            var classes = new List<string> { string.Empty };
            classes.AddRange(AppSettingsGet.AthleteClasses);
            Classes = classes.Select(p => new SelectListItem
            {
                Text = p,
                Value = p,
                Selected = p == string.Empty
            }).ToList();
        }
    }
}