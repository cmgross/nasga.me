using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nasga.me.App_Start;

namespace nasga.me.Models
{
    public class PersonalRecordsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
        public List<string> Years { get; set; }
        public List<string> Ranks { get; set; }
        public List<AthleteResponse.Record.BraemarEvent> BraemarEvents { get; set; }
        public List<AthleteResponse.Record.OpenEvent> OpenEvents { get; set; }
        public List<AthleteResponse.Record.HeavyEvent> HeavyEvents { get; set; }
        public List<AthleteResponse.Record.LightEvent> LightEvents { get; set; }
        public List<AthleteResponse.Record.HeavyHammerEvent> HeavyHammerEvents { get; set; }
        public List<AthleteResponse.Record.LightHammerEvent> LightHammerEvents { get; set; }
        public List<AthleteResponse.Record.CaberEvent> CaberEvents { get; set; }
        public List<AthleteResponse.Record.SheafEvent> SheafEvents { get; set; }
        public List<AthleteResponse.Record.WfhEvent> WfhEvents { get; set; }
       

        public PersonalRecordsViewModel(AthleteResponse athlete)
        {
            FirstName = athlete.FirstName;
            LastName = athlete.LastName;
            Class = athlete.Class;
            List<AthleteResponse.Record> orderedRecords = athlete.Records.OrderByDescending(c => c.Year).ToList();
            Years = orderedRecords.Select(y => y.Year).ToList();
            Ranks = orderedRecords.Select(r => r.Rank).ToList();
            BraemarEvents = orderedRecords.Select(b => b.Braemar).ToList();
            OpenEvents = orderedRecords.Select(o => o.Open).ToList();
            HeavyEvents = orderedRecords.Select(h => h.Heavy).ToList();
            LightEvents = orderedRecords.Select(l => l.Light).ToList();
            HeavyHammerEvents = orderedRecords.Select(hh => hh.HeavyHammer).ToList();
            LightHammerEvents = orderedRecords.Select(lh => lh.LightHammer).ToList();
            CaberEvents = orderedRecords.Select(c => c.Caber).ToList();
            SheafEvents = orderedRecords.Select(s => s.Sheaf).ToList();
            WfhEvents = orderedRecords.Select(w => w.Wfh).ToList();
        }
    }
}