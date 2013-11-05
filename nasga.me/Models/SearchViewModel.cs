using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nasga.me.Interfaces;
using ServiceStack.Common;

namespace nasga.me.Models
{
    public class SearchViewModel : Search
    {
        public List<SelectListItem> Classes { get; set; }
        public List<SelectListItem> Years { get; set; }

        public SearchViewModel(IConfigManager configManager)
        {
            var classes = new List<string> { string.Empty };
            var years = new List<string> { string.Empty };
            classes.AddRange(configManager.AthleteClasses);
            years.AddRange(configManager.Years);
            Classes = classes.Select(p => new SelectListItem
            {
                Text = p,
                Value = p
            }).ToList();
            Years = years.Select(y => new SelectListItem
            {
                Text = y,
                Value = y
            }).ToList();
        }
    }
}