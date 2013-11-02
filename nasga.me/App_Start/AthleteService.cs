﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using nasga.me.Interfaces;
using ServiceStack.Common;
using ServiceStack.Common.Extensions;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace nasga.me.App_Start
{
    public class AthleteService : Service
    {
        public object Any(Athlete request) //this returns cached json/raw bytes
        {
            //TODO: switch based on webconfig entry
            string cacheKey = UrnId.Create<AthleteResponse>(request.LastName + request.FirstName + request.Class);
            return RequestContext.ToOptimizedResultUsingCache(base.Cache, cacheKey, new TimeSpan(1, 0, 0), () =>
                    {
                        //var athleteResults = NasgaClient.AgilityScreenScrape(request);
                        var athleteResults = NasgaClient.MockScreenScrape(request);
                        return athleteResults;
                    });
        }

        public AthleteResponse Get(Athlete request)
        {
            var cacheKey = UrnId.Create<AthleteResponse>(request.LastName + request.FirstName + request.Class);
            var athleteResponse = base.Cache.Get<AthleteResponse>(cacheKey);
            if (athleteResponse != null) return athleteResponse;
            athleteResponse = NasgaClient.AgilityScreenScrape(request);
            //athleteResponse = NasgaClient.MockScreenScrape(request);
            base.Cache.Set<AthleteResponse>(cacheKey, athleteResponse, new TimeSpan(0, 5, 0));
            return athleteResponse;
        }
    }

    //TODO Move these to Models
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


    public static class NasgaClient
    {
        public static AthleteResponse AgilityScreenScrape(Athlete request)
        {
            //"class=Pro&rankyear=2013&x=26&y=10"
            //classes: Pro, All+Women, All+Amateurs, All+Masters, All+Lightweight
            string athleteClass;

            switch (request.Class) //TODO need to handle Master (no longer All+Masters) and Pro+Master
            {
                case "Master":
                case "Pro":
                    athleteClass = request.Class;
                    break;
                case "Amateur":
                    athleteClass = "All+" + request.Class + "s";
                    break;
                default:
                    athleteClass = "All+" + request.Class;
                    break;
            }

            var athleteResponse = new AthleteResponse
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Class = request.Class,
                Records = new List<AthleteResponse.Record>()
            };


            var maxYear = DateTime.Now.Year;

            #region WebClientToNasgaWeb
            using (var client = new WebClient())
            {
                for (int i = 2009; i <= maxYear; i++) //for some reason, records older than 2009 crash the scrape?
                {
                    var formValues = new NameValueCollection
                        {
                            {"class", athleteClass},
                            {"rankyear", i.ToString()},
                            {"x", "26"},
                            {"y", "10"}
                        };
                    byte[] byteArray = client.UploadValues("http://nasgaweb.com/dbase/rank_overall.asp", formValues);
                    var document = new HtmlDocument();
                    document.LoadHtml(Encoding.ASCII.GetString(byteArray));
                    HtmlNode body = document.DocumentNode.Descendants().FirstOrDefault(n => n.Name == "body");
                    if (body == null) continue;

                    //this is the only identifier for the athlete table: <table cellspacing="2" cellpadding="1">
                    HtmlNode athleteTable =
                        body.Descendants("table")
                            .FirstOrDefault(
                                t => t.Attributes.Contains("cellpadding") && t.Attributes["cellpadding"].Value == "1");
                    if (athleteTable == null) continue;

                    //skip first two rows of this table, after that, each row is an athlete, first two rows have bgcolor: #99ccff
                    List<HtmlNode> athleteRows =
                        athleteTable.Descendants("tr")
                                    .Where(
                                        r =>
                                        r.Attributes.Contains("bgcolor") && r.Attributes["bgcolor"].Value != "#99ccff")
                                    .ToList();
                    if (!athleteRows.Any()) continue;

                    string nasgaName = request.FirstName + "&nbsp;" + request.LastName;
                    HtmlNode athleteRow = athleteRows.FirstOrDefault(a => a.InnerText.Contains(nasgaName));
                    if (athleteRow == null) continue;
                    string[] athleteData = athleteRow.Descendants("td").Select(d => d.InnerText).ToArray();
                    var record = ParseAthleteData(athleteData,i,athleteRows.Count);
                    athleteResponse.Records.Add(record);
                }
            }
            #endregion
            AthleteResponse.Record prRecords = GetPRs(athleteResponse.Records);
            athleteResponse.Records.Add(prRecords);
            return athleteResponse;
        }

        public static AthleteResponse MockScreenScrape(Athlete request)
        {
            var athleteResponse = new AthleteResponse
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Class = request.Class,
                Records = new List<AthleteResponse.Record>()
            };

            //mock records are based off a single female athlete with two years of records
            var record1 = new AthleteResponse.Record
            {
                Year = "2012",
                Rank = "2/250",
                TotalPoints = 2965,
                Braemar = new AthleteResponse.Record.BraemarEvent
                {
                    Throw = ThrowConverter("15'-7.00&quot;"),
                    Points = 336
                },
                Open = new AthleteResponse.Record.OpenEvent
                {
                    Throw = ThrowConverter("18'-9.00&quot;"),
                    Points = 297
                },
                Heavy = new AthleteResponse.Record.HeavyEvent
                {
                    Throw = ThrowConverter("11'-1.00&quot;"),
                    Points = 222
                },
                Light = new AthleteResponse.Record.LightEvent
                {
                    Throw = ThrowConverter("25'-9.00&quot;"),
                    Points = 275
                },
                HeavyHammer = new AthleteResponse.Record.HeavyHammerEvent
                {
                    Throw = ThrowConverter("31'-10.00&quot;"),
                    Points = 255
                },
                LightHammer = new AthleteResponse.Record.LightHammerEvent
                {
                    Throw = ThrowConverter("0'-0.00&quot;"),
                    Points = 0
                },
                Caber = new AthleteResponse.Record.CaberEvent
                {
                    Throw = "877",
                    Points = 877
                },
                Sheaf = new AthleteResponse.Record.SheafEvent
                {
                    Throw = ThrowConverter("11'-0.00&quot;"),
                    Points = 305
                },
                Wfh = new AthleteResponse.Record.WfhEvent
                {
                    Throw = ThrowConverter("8'-0.00&quot;"),
                    Points = 398
                }
            };

            var record2 = new AthleteResponse.Record
            {
                Year = "2013",
                Rank = "1/250",
                TotalPoints = 3337,
                Braemar = new AthleteResponse.Record.BraemarEvent
                {
                    Throw = ThrowConverter("17'-3.00&quot;"),
                    Points = 372
                },
                Open = new AthleteResponse.Record.OpenEvent
                {
                    Throw = ThrowConverter("21'-6.60&quot;"),
                    Points = 341
                },
                Heavy = new AthleteResponse.Record.HeavyEvent
                {
                    Throw = ThrowConverter("16'-2.50&quot;"),
                    Points = 325
                },
                Light = new AthleteResponse.Record.LightEvent
                {
                    Throw = ThrowConverter("32'-6.50&quot;"),
                    Points = 347
                },
                HeavyHammer = new AthleteResponse.Record.HeavyHammerEvent
                {
                    Throw = ThrowConverter("39'-1.00&quot;"),
                    Points = 314
                },
                LightHammer = new AthleteResponse.Record.LightHammerEvent
                {
                    Throw = ThrowConverter("52'-7.50&quot;"),
                    Points = 349
                },
                Caber = new AthleteResponse.Record.CaberEvent
                {
                    Throw = "292",
                    Points = 292
                },
                Sheaf = new AthleteResponse.Record.SheafEvent
                {
                    Throw = ThrowConverter("18'-0.00&quot;"),
                    Points = 499
                },
                Wfh = new AthleteResponse.Record.WfhEvent
                {
                    Throw = ThrowConverter("10'-0.00&quot;"),
                    Points = 498
                }
            };

            athleteResponse.Records.Add(record1);
            athleteResponse.Records.Add(record2);
            AthleteResponse.Record prRecords = GetPRs(athleteResponse.Records);
            athleteResponse.Records.Add(prRecords);
            return athleteResponse;
        }

        public static string ThrowConverter(string nasgaThrow)
        {
            //17'-3.00" - what shows on site
            //"17'-3.00&quot;" - actual value from screen scrape
            return nasgaThrow.Replace("&quot;", "\"");
        }

        private static AthleteResponse.Record GetPRs(List<AthleteResponse.Record> records)
        {
            var bestBraemar = records.OrderByDescending(b => b.Braemar.Points).First().Braemar;
            var bestOpen = records.OrderByDescending(b => b.Open.Points).First().Open;
            var bestHeavy = records.OrderByDescending(b => b.Heavy.Points).First().Heavy;
            var bestLight = records.OrderByDescending(b => b.Light.Points).First().Light;
            var bestHeavyHammer = records.OrderByDescending(b => b.HeavyHammer.Points).First().HeavyHammer;
            var bestLightHammer = records.OrderByDescending(b => b.LightHammer.Points).First().LightHammer;
            var bestCaber = records.OrderByDescending(b => b.Caber.Points).First().Caber;
            var bestSheaf = records.OrderByDescending(b => b.Sheaf.Points).First().Sheaf;
            var bestWfh = records.OrderByDescending(b => b.Wfh.Points).First().Wfh;
            var bestThrowsRecord = new AthleteResponse.Record
            {
                Year = "All Time",
                Rank = "-",
                Braemar = bestBraemar,
                Open = bestOpen,
                Heavy = bestHeavy,
                Light = bestLight,
                HeavyHammer = bestHeavyHammer,
                LightHammer = bestLightHammer,
                Caber = bestCaber,
                Sheaf = bestSheaf,
                Wfh = bestWfh
            };
            return bestThrowsRecord;
        }

        private static AthleteResponse.Record ParseAthleteData(string[] athleteData, int year, int totalAthletesInClass)
        {
            int totalPoints;
            int braemarPoints;
            int openPoints;
            int heavyPoints;
            int lightPoints;
            int heavyHammerPoints;
            int lightHammerPoints;
            int caberPoints;
            int sheafPoints;
            int wfhPoints;
            int.TryParse(athleteData[2], out totalPoints);
            int.TryParse(athleteData[4], out braemarPoints);
            int.TryParse(athleteData[6], out openPoints);
            int.TryParse(athleteData[8], out heavyPoints);
            int.TryParse(athleteData[10], out lightPoints);
            int.TryParse(athleteData[12], out heavyHammerPoints);
            int.TryParse(athleteData[14], out lightHammerPoints);
            int.TryParse(athleteData[15], out caberPoints);
            int.TryParse(athleteData[17], out sheafPoints);
            int.TryParse(athleteData[19], out wfhPoints);

            #region recordCreation
            var record = new AthleteResponse.Record
            {
                Year = year.ToString(),
                Rank = athleteData[0] + "/" + totalAthletesInClass,
                TotalPoints = totalPoints,
                Braemar = new AthleteResponse.Record.BraemarEvent
                {
                    Throw = ThrowConverter(athleteData[3]),
                    Points = braemarPoints
                },
                Open = new AthleteResponse.Record.OpenEvent
                {
                    Throw = ThrowConverter(athleteData[5]),
                    Points = openPoints
                },
                Heavy = new AthleteResponse.Record.HeavyEvent
                {
                    Throw = ThrowConverter(athleteData[7]),
                    Points = heavyPoints
                },
                Light = new AthleteResponse.Record.LightEvent
                {
                    Throw = ThrowConverter(athleteData[9]),
                    Points = lightPoints
                },
                HeavyHammer = new AthleteResponse.Record.HeavyHammerEvent
                {
                    Throw = ThrowConverter(athleteData[11]),
                    Points = heavyHammerPoints
                },
                LightHammer = new AthleteResponse.Record.LightHammerEvent
                {
                    Throw = ThrowConverter(athleteData[13]),
                    Points = lightHammerPoints
                },
                Caber = new AthleteResponse.Record.CaberEvent
                {
                    Throw = caberPoints.ToString(),
                    Points = caberPoints
                },
                Sheaf = new AthleteResponse.Record.SheafEvent
                {
                    Throw = ThrowConverter(athleteData[16]),
                    Points = sheafPoints
                },
                Wfh = new AthleteResponse.Record.WfhEvent
                {
                    Throw = ThrowConverter(athleteData[18]),
                    Points = wfhPoints
                }
            };
            #endregion
            return record;
        }
    }
}