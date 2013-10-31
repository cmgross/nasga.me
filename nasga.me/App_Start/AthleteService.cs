using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using ServiceStack.Common;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace nasga.me.App_Start
{
    public class AthleteService : Service
    {
        public object Any(Athlete request)
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
    }

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
            //public string TotalPoints { get; set; } //TODO include the total points to pull out the highest/farthest throw
            public string BraemarThrow { get; set; }
            //public string BraemarPoints { get; set; }
            public string OpenThrow { get; set; }
            //public string OpenPoints { get; set; }
            public string HeavyThrow { get; set; }
            //public string HeavyPoints { get; set; }
            public string LightThrow { get; set; }
            //public string LightPoints { get; set; }
            public string HeavyHammerThrow { get; set; }
            //public string HeavyHammerPoints { get; set; }
            public string LightHammerThrow { get; set; }
            //public string LightHammerPoints { get; set; }
            public string CaberPoints { get; set; }
            public string SheafThrow { get; set; }
            //public string SheafPoints { get; set; }
            public string WfhThrow { get; set; }
            //public string WfhPoints { get; set; }
        }
    }


    public static class NasgaClient
    {
        public static AthleteResponse AgilityScreenScrape(Athlete request)
        {
            //"class=Pro&rankyear=2013&x=26&y=10"
            //classes: Pro, All+Women, All+Amateurs, All+Masters, All+Lightweight
            string athleteClass;

            switch (request.Class)
            {
                case "Pro":
                    athleteClass = request.Class;
                    break;
                default:
                    athleteClass = "All+" + request.Class;
                    break;
            }

            AthleteResponse athleteResponse = new AthleteResponse
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Class = request.Class,
                Records = new List<AthleteResponse.Record>()
            };


            int maxYear = DateTime.Now.Year;

            #region WebClientToNasgaWeb
            using (WebClient client = new WebClient())
            {
                for (int i = 2009; i <= maxYear; i++)
                {
                    NameValueCollection formValues = new NameValueCollection
                        {
                            {"class", athleteClass},
                            {"rankyear", i.ToString()},
                            {"x", "26"},
                            {"y", "10"}
                        };
                    byte[] byteArray = client.UploadValues("http://nasgaweb.com/dbase/rank_overall.asp", formValues);
                    HtmlDocument document = new HtmlDocument();
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

                    AthleteResponse.Record record = new AthleteResponse.Record
                    {
                        Year = i.ToString(),
                        Rank = athleteData[0] + "/" + athleteRows.Count,
                        //TotalPoints = athleteData[2],
                        BraemarThrow = ThrowConverter(athleteData[3]),
                        //BraemarPoints = athleteData[4],
                        OpenThrow = ThrowConverter(athleteData[5]),
                        //OpenPoints = athleteData[6],
                        HeavyThrow = ThrowConverter(athleteData[7]),
                        //HeavyPoints = athleteData[8],
                        LightThrow = ThrowConverter(athleteData[9]),
                        //LightPoints = athleteData[10],
                        HeavyHammerThrow = ThrowConverter(athleteData[11]),
                        //HeavyHammerPoints = athleteData[12],
                        LightHammerThrow = ThrowConverter(athleteData[13]),
                        //LightHammerPoints = athleteData[14],
                        CaberPoints = athleteData[15],
                        SheafThrow = ThrowConverter(athleteData[16]),
                        //SheafPoints = athleteData[17],
                        WfhThrow = ThrowConverter(athleteData[18]),
                        //WfhPoints = athleteData[19]
                    };
                    athleteResponse.Records.Add(record);
                }
            }
            #endregion

            return athleteResponse;
        }
        public static AthleteResponse MockScreenScrape(Athlete request)
        {
            AthleteResponse athleteResponse = new AthleteResponse
            {
                FirstName = "Test",
                LastName = "Athlete",
                Class = "Test Class",
                Records = new List<AthleteResponse.Record>()
            };

            AthleteResponse.Record record1 = new AthleteResponse.Record
            {
                Year = "2012",
                Rank = "2/250",
                BraemarThrow = ThrowConverter("17'-3.00&quot;"),
                OpenThrow = ThrowConverter("17'-3.00&quot;"),
                HeavyThrow = ThrowConverter("17'-3.00&quot;"),
                LightThrow = ThrowConverter("17'-3.00&quot;"),
                HeavyHammerThrow = ThrowConverter("17'-3.00&quot;"),
                LightHammerThrow = ThrowConverter("17'-3.00&quot;"),
                CaberPoints = "999",
                SheafThrow = ThrowConverter("17'-3.00&quot;"),
                WfhThrow = ThrowConverter("17'-3.00&quot;")
            };

            AthleteResponse.Record record2 = new AthleteResponse.Record
            {
                Year = "2013",
                Rank = "1/250",
                BraemarThrow = ThrowConverter("17'-3.00&quot;"),
                OpenThrow = ThrowConverter("17'-3.00&quot;"),
                HeavyThrow = ThrowConverter("17'-3.00&quot;"),
                LightThrow = ThrowConverter("17'-3.00&quot;"),
                HeavyHammerThrow = ThrowConverter("17'-3.00&quot;"),
                LightHammerThrow = ThrowConverter("17'-3.00&quot;"),
                CaberPoints = "999",
                SheafThrow = ThrowConverter("17'-3.00&quot;"),
                WfhThrow = ThrowConverter("17'-3.00&quot;")
            };

            athleteResponse.Records.Add(record1);
            athleteResponse.Records.Add(record2);
            return athleteResponse;
        }
        public static string ThrowConverter(string nasgaThrow)
        {
            //17'-3.00" - what shows on site
            //"17'-3.00&quot;" - actual value from screen scrape
            return nasgaThrow.Replace("&quot;", "\"");
        }
    }
}