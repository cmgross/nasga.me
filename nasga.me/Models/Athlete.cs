using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nasga.me.Interfaces;
using ServiceStack.ServiceInterface.ServiceModel;

namespace nasga.me.Models
{
    //Request DTO
    public class Athlete
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
    }

    //Response DTO
    public class AthleteResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
        public List<Record> Records { get; set; }
        public ResponseStatus ResponseStatus { get; set; } //Where Exceptions get auto-serialized

        public class Record
        {
            public string Year { get; set; }
            public string Rank { get; set; }
            public int TotalPoints { get; set; }
            public BraemarEvent Braemar { get; set; }
            public OpenEvent Open { get; set; }
            public HeavyEvent Heavy { get; set; }
            public LightEvent Light { get; set; }
            public HeavyHammerEvent HeavyHammer { get; set; }
            public LightHammerEvent LightHammer { get; set; }
            public CaberEvent Caber { get; set; }
            public SheafEvent Sheaf { get; set; }
            public WfhEvent Wfh { get; set; }

            public class BraemarEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class OpenEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class HeavyEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class LightEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class HeavyHammerEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class LightHammerEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class CaberEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class SheafEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
            public class WfhEvent : IThrowEvent
            {
                public string Throw { get; set; }
                public int Points { get; set; }
            }
        }
    }
}