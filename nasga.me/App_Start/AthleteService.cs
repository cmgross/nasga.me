using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Microsoft.Ajax.Utilities;
using nasga.me.Models;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace nasga.me.App_Start
{
    public class AthleteService : Service
    {
        public object Any(Athlete request) //this returns cached json/raw bytes
        {
            //TODO: switch between screen scrape methods and create timespan both based on webconfig
            string cacheKey = UrnId.Create<AthleteResponse>(request.LastName + request.FirstName + request.Class);
            return RequestContext.ToOptimizedResultUsingCache(base.Cache, cacheKey, new TimeSpan(1, 0, 0), () =>
                    {
                        //var athleteResults = NasgaClient.AgilityScreenScrape(request);
                        var athleteResults = MockScreenScrape(request);
                        return athleteResults;
                    });
        }

        public AthleteResponse Get(Athlete request)
        {
            var cacheKey = UrnId.Create<AthleteResponse>(request.LastName + request.FirstName + request.Class);
            var athleteResponse = base.Cache.Get<AthleteResponse>(cacheKey);
            if (athleteResponse != null) return athleteResponse;
            athleteResponse = AgilityScreenScrape(request);
            //athleteResponse = MockScreenScrape(request);
            base.Cache.Set<AthleteResponse>(cacheKey, athleteResponse, new TimeSpan(0, 5, 0));
            return athleteResponse;
        }

        public List<AthleteResponse> Get(List<Athlete> request)
        {
            return request.Select(Get).ToList();
        }

        public List<string> GetNames(string term, string year, string athleteClass)
        {
            //extract class switch in AgilityScreenScrape to its own method
            List<string> nameList = new List<string>
            {
                "Jonathan", "Lisa", "Jordan", "Tyler", "Susan", "Brandon", "Clayton", "Elizabeth", "Jennifer"
            };
            return nameList.Where(n =>n.StartsWith(term, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private AthleteResponse AgilityScreenScrape(Athlete request)
        {
            //"class=Pro&rankyear=2013&x=26&y=10"
            //classes: Pro, All+Women, All+Amateurs, All+Masters, All+Lightweight
            string athleteClass;

            switch (request.Class)
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
                for (int i = 2009; i <= maxYear; i++) //for some reason, records older than 2009 crash the site even on the site?
                {
                    var athleteRows = GetAthleteRows(client, athleteClass, i);
                    if (!athleteRows.Any()) continue;
                    string nasgaName = request.FirstName + "&nbsp;" + request.LastName;
                    HtmlNode athleteRow = athleteRows.FirstOrDefault(a => a.InnerText.Contains(nasgaName));
                    if (athleteRow == null) continue;
                    string[] athleteData = athleteRow.Descendants("td").Select(d => d.InnerText).ToArray();
                    var record = ParseAthleteData(athleteData, i, athleteRows.Count, athleteClass);
                    athleteResponse.Records.Add(record);
                }
            }
            #endregion

            if (!athleteResponse.Records.Any()) return athleteResponse;
            AthleteResponse.Record prRecords = GetPRs(athleteResponse.Records);
            athleteResponse.Records.Add(prRecords);
            return athleteResponse;
        }

        private List<HtmlNode> GetAthleteRows(WebClient client, string athleteClass, int year)
        {
            var cacheKey = UrnId.Create<List<HtmlNode>>(athleteClass + year);
            var athleteRowsInCache = base.Cache.Get<List<HtmlNode>>(cacheKey);
            if (athleteRowsInCache != null) return athleteRowsInCache;
            var athleteRows = new List<HtmlNode>();
            var formValues = new NameValueCollection
                        {
                            {"class", athleteClass},
                            {"rankyear", year.ToString()},
                            {"x", "26"},
                            {"y", "10"}
                        };
            byte[] byteArray = client.UploadValues("http://nasgaweb.com/dbase/rank_overall.asp", formValues);
            var document = new HtmlDocument();
            document.LoadHtml(Encoding.ASCII.GetString(byteArray));
            HtmlNode body = document.DocumentNode.Descendants().FirstOrDefault(n => n.Name == "body");
            if (body == null) return athleteRows;

            //this is the only identifier for the athlete table: <table cellspacing="2" cellpadding="1">
            HtmlNode athleteTable =
                body.Descendants("table")
                    .FirstOrDefault(
                        t => t.Attributes.Contains("cellpadding") && t.Attributes["cellpadding"].Value == "1");
            if (athleteTable == null) return athleteRows;

            //skip first two rows of this table, after that, each row is an athlete, first two rows have bgcolor: #99ccff
            athleteRows =
                athleteTable.Descendants("tr")
                            .Where(
                                r =>
                                r.Attributes.Contains("bgcolor") && r.Attributes["bgcolor"].Value != "#99ccff")
                            .ToList();
            base.Cache.Set<List<HtmlNode>>(cacheKey, athleteRows, new TimeSpan(0, 10, 0));
            return athleteRows;
        }

        private static AthleteResponse MockScreenScrape(Athlete request)
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

        private static string ThrowConverter(string nasgaThrow)
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

        private static AthleteResponse.Record ParseAthleteData(string[] athleteData, int year, int totalAthletesInClass, string athleteClass)
        {
            //there is an age column in masters table that shifts all data one column
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
            int.TryParse(athleteData[athleteClass == "Master" ? 3 : 2], out totalPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 5 : 4], out braemarPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 7 : 6], out openPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 9 : 8], out heavyPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 11 : 10], out lightPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 13 : 12], out heavyHammerPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 15 : 14], out lightHammerPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 16 : 15], out caberPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 18 : 17], out sheafPoints);
            int.TryParse(athleteData[athleteClass == "Master" ? 20 : 19], out wfhPoints);

            #region recordCreation
            var record = new AthleteResponse.Record
            {
                Year = year.ToString(),
                Rank = athleteData[0] + "/" + totalAthletesInClass,
                TotalPoints = totalPoints,
                Braemar = new AthleteResponse.Record.BraemarEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 4 : 3]),
                    Points = braemarPoints
                },
                Open = new AthleteResponse.Record.OpenEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 6 : 5]),
                    Points = openPoints
                },
                Heavy = new AthleteResponse.Record.HeavyEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 8 : 7]),
                    Points = heavyPoints
                },
                Light = new AthleteResponse.Record.LightEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 10 : 9]),
                    Points = lightPoints
                },
                HeavyHammer = new AthleteResponse.Record.HeavyHammerEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 12 : 11]),
                    Points = heavyHammerPoints
                },
                LightHammer = new AthleteResponse.Record.LightHammerEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 14 : 13]),
                    Points = lightHammerPoints
                },
                Caber = new AthleteResponse.Record.CaberEvent
                {
                    Throw = caberPoints.ToString(),
                    Points = caberPoints
                },
                Sheaf = new AthleteResponse.Record.SheafEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 17 : 16]),
                    Points = sheafPoints
                },
                Wfh = new AthleteResponse.Record.WfhEvent
                {
                    Throw = ThrowConverter(athleteData[athleteClass == "Master" ? 19 : 18]),
                    Points = wfhPoints
                }
            };
            #endregion
            return record;
        }
    }
}