using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using nasga.me.Interfaces;

namespace nasga.me.Models
{
    public class Profile
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

        public Profile(IConfigManager configManager, IReadOnlyDictionary<string, string> athleteInfo)
        {
            string firstName;
            string lastName;
            string athleteClass;
            athleteInfo.TryGetValue(configManager.AthleteFirstNameKey, out firstName);
            athleteInfo.TryGetValue(configManager.AthleteLastNameKey, out lastName);
            athleteInfo.TryGetValue(configManager.AthleteClassKey, out athleteClass);
            FirstName = firstName;
            LastName = lastName;
            AthleteClass = athleteClass;
        }

        public Profile()
        {
            
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(AthleteClass);
        }
    }
}