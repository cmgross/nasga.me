using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;

namespace nasga.me.Models
{
    public class ProfileViewModel : Profile
    {
        public string ProfileError { get; set; }
        public List<SelectListItem> Classes { get; set; }

        public ProfileViewModel(IConfigManager configManager, IReadOnlyDictionary<string, string> athleteInfo) : base(configManager,athleteInfo)
        {
            var classes = new List<string> {string.Empty};
            classes.AddRange(configManager.AthleteClasses);
            Classes = classes.Select(p => new SelectListItem
            {
                Text = p,
                Value = p,
                Selected = p == AthleteClass
            }).ToList();
        }

        public ProfileViewModel()
        {
            
        }
    }
}