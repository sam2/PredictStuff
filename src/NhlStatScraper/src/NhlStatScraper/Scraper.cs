using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using NHLStatsModel;
using NHLStatsModel.NHLEvents;
using System.Threading.Tasks;

namespace NhlStatScraper
{
    public static class Scraper
    {
        public async static Task<List<NHLGame>> GetNHLGames(DateTime date)
        {
            Console.WriteLine("\nParsing BoxScores on " + date.ToString());
            List<NHLGame> games = new List<NHLGame>();
            List<NHLBoxScore> scores = await GetBoxScores(date);
            foreach (NHLBoxScore score in scores)
            {
                Console.WriteLine(score.HomeTeamCity + " vs " + score.AwayTeamCity);
                string pbpURL = GetPlayByPlayURL(score.GameId);
                NHLGame game = new NHLGame(score, await ParseEvents(pbpURL));
                games.Add(game);
            }
            return games;
        }

        public async static Task<List<NHLBoxScore>> GetBoxScores(DateTime time)
        {
            string path = "http://live.nhle.com/GameData/GCScoreboard/" + time.ToString("yyyy-MM-dd") + ".jsonp";
            string text = await GetPageString(path);
            text = FormatScoreboardString(text);
            List<NHLBoxScore> games = new List<NHLBoxScore>();
            var objects = JObject.Parse(text);
            foreach (var stat in objects["loadScoreBoard"]["games"])
            {
                NHLBoxScore game = new NHLBoxScore();
                game.AwayTeamName = (string)stat["atcommon"];
                game.AwayTeamCity = (string)stat["atn"];
                game.AwayScore = (string)stat["ats"] == "" ? 0 : (int)stat["ats"];
                if (stat["atsog"] != null)
                    game.AwayShots = (string)stat["atsog"] == "" ? 0 : (int)stat["atsog"];

                game.HomeTeamName = (string)stat["htcommon"];
                game.HomeTeamCity = (string)stat["htn"];
                game.HomeScore = (string)stat["hts"] == "" ? 0 : (int)stat["hts"];
                if (stat["htsog"] != null)
                    game.HomeShots = (string)stat["htsog"] == "" ? 0 : (int)stat["htsog"];

                game.GameId = (int)stat["id"];
                game.StartTime = DateTime.Now; //hack, replace with accurate data

                games.Add(game);
            }
            return games;
        }

        #region Events

        private async static Task<List<NHLEvent>> ParseEvents(string url) //http://www.nhl.com/scores/htmlreports/20152016/PL021201.HTM
        {
            List<NHLEvent> nhlEvents = new List<NHLEvent>();


            HttpWebRequest oReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse resp = await oReq.GetResponseAsync();
            var doc = new HtmlAgilityPack.HtmlDocument();

            doc.Load(resp.GetResponseStream());
            if (doc.DocumentNode.Descendants("title").SingleOrDefault().InnerText.Contains("404"))
            {
                Console.WriteLine("No play by play found for url:\n " + url + "\n has the game been played yet?");
                return null;
            }

            //get all rows in page
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//tr[@class='evenColor']"))
            {
                List<string> elementsInRow = new List<string>();

                //remove extra whitespace etc
                foreach (string s in node.InnerText.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)))
                {
                    elementsInRow.Add(s.Replace("\r", ""));
                }

                NHLEvent nhlEvent = ParseEvent(elementsInRow);

                if (nhlEvent != null)
                {
                    nhlEvents.Add(nhlEvent);
                }
            }
            return nhlEvents;
        }

        private static NHLEvent ParseEvent(List<String> row)
        {
            NHLEvent nhlevent = null;
            switch (row[4])
            {
                case "SHOT":
                    nhlevent = ParseShotOnGoal(row);
                    break;
                case "MISS":
                    nhlevent = ParseMissedShot(row);
                    break;
                case "GOAL":
                    nhlevent = ParseGoal(row);
                    break;
                case "BLOCK":
                    nhlevent = ParseBlockedShot(row);
                    break;
                case "PENL":
                    nhlevent = new PenaltyEvent();
                    break;
                case "FAC":
                    nhlevent = new FaceoffEvent();
                    break;
                default:
                    return null;
            }

            int period = int.Parse(row[1]);

            nhlevent.GameTime = new NHLGameTime(row[3], period);
            nhlevent.Team = row[5].Split(' ')[0];
            nhlevent.Strength = row[2];
            //TODO: set also on ice

            return nhlevent;
        }

        private static ShotOnGoalEvent ParseShotOnGoal(List<string> row)
        {
            ShotOnGoalEvent sog = new ShotOnGoalEvent();
            string[] desc = row[5].Split(','); //COL ONGOAL - #29 MACKINNON, Wrist, Off. Zone, 9 ft.           

            string distance = desc[desc.Length - 1].Trim().Split(' ')[0];
            sog.Distance = int.Parse(distance);
            sog.Type = desc[1].Trim();
            sog.Shooter = ParsePlayer(desc[0].Split('-')[1].Trim(), desc[0].Split(' ')[0].Trim());

            return sog;
        }

        private static MissedShotEvent ParseMissedShot(List<string> row)
        {
            MissedShotEvent ms = new MissedShotEvent();
            string[] desc = row[5].Split(','); //MTL #17 BOURQUE, Wrist, Wide of Net, Off. Zone, 10 ft.

            //split the description and get player number and name
            string[] player = desc[0].Split(' ');
            ms.Shooter = ParsePlayer(player[1] + " " + player[2], player[0]);

            //populate the shot object
            ms.Type = desc[1].Trim();
            ms.Distance = int.Parse(desc[4].Split(' ')[1]);
            ms.Description = desc[2].Trim();

            return ms;
        }

        private static GoalEvent ParseGoal(List<string> row)
        {
            GoalEvent goal = new GoalEvent();
            string[] desc = row[5].Split(','); //STL #91 TARASENKO(9), Wrist, Off. Zone, 15 ft. Assists: #17 SCHWARTZ(5); #12 LEHTERA(7)

            string[] player = desc[0].Split(' ');
            goal.Shooter = ParsePlayer(player[1] + " " + player[2], player[0]);
            goal.Type = desc[1].Trim();

            string[] distanceAndAssists = desc[desc.Length - 1].Split(':', ';'); //15 ft.Assists: #17 SCHWARTZ(5); #12 LEHTERA(7)

            goal.Distance = int.Parse(distanceAndAssists[0].Split(' ')[1]);

            if (distanceAndAssists.Count() > 1)
                goal.Assist1 = ParsePlayer(distanceAndAssists[1], player[0]);

            //secondary assist
            if (distanceAndAssists.Count() > 2)
                goal.Assist2 = ParsePlayer(distanceAndAssists[2], player[0]);

            return goal;
        }

        private static BlockedShotEvent ParseBlockedShot(List<string> row)
        {
            BlockedShotEvent block = new BlockedShotEvent();
            string[] desc = row[5].Split(','); //COL #22 REDMOND BLOCKED BY TOR #46 POLAK, Wrist, Def. Zone
            string[] players = desc[0].Split(new string[] { "BLOCKED BY" }, StringSplitOptions.None);

            string shooterName = players[0].Trim().Substring(4);
            string blockerName = players[1].Trim().Substring(4);

            block.Shooter = ParsePlayer(shooterName, players[0].Trim().Split(' ')[0]);
            block.Blocker = ParsePlayer(blockerName, players[1].Trim().Split(' ')[0]);
            block.Type = desc[1].Trim();


            return block;
        }

        #endregion

        #region Utilities

        private static NHLPlayer ParsePlayer(string player, string team) //format: #17 BOURQUE
        {
            player = player.Trim();
            string[] playerInfo = player.Split(' ');
            int playerNumber = int.Parse(playerInfo[0].Substring(1));

            string playerName = string.Empty;
            for (int i = 1; i < playerInfo.Count(); i++)
            {
                playerName += playerInfo[i];
            }
            playerName = TrimPlayerName(playerName);

            return new NHLPlayer(playerNumber, playerName, team);

        }


        private static string TrimPlayerName(string name)
        {
            string newName = string.Empty;
            for (int i = 0; i < name.Count(); i++)
            {
                if (Char.IsLetter(name[i]))
                {
                    newName += name[i];
                }
            }
            return newName;
        }

        private static async Task<string> GetPageString(string url)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            string result = null;
            bool pageNotFound = false;
            WebResponse response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                try
                {
                    response = await request.GetResponseAsync();
                }
                catch (System.Net.WebException)
                {
                    pageNotFound = true;
                }

                if (!pageNotFound)
                {

                    // we will read data via the response stream
                    System.IO.Stream resStream = response.GetResponseStream();

                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // fill the buffer with data
                        try
                        {
                            count = resStream.Read(buf, 0, buf.Length);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine(ex.StackTrace);
                            break;
                        }

                        // make sure we read some data
                        if (count != 0)
                        {
                            // translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // any more data to read?

                    result = sb.ToString();
                }

                if (result == string.Empty)
                    result = null;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.StackTrace);
            }

            return result;
        }

        private static string GetPlayByPlayURL(int gameid)
        {
            string url = "http://www.nhl.com/scores/htmlreports/";

            int firstYear = gameid / 1000000;
            int secondYear = firstYear + 1;
            url += firstYear.ToString() + secondYear.ToString() + "/";

            string id = gameid.ToString().Substring(4);
            url += "PL" + id + ".HTM";

            return url;
        }

        private static string FormatScoreboardString(string text)
        {
            if (text != null)
            {
                text = text.Substring("loadScoreboard(".Length);
                text = text.TrimEnd('\r');
                text = text.TrimEnd('\n');
                text = text.TrimEnd(')');
                text = "{\"loadScoreBoard\":" + text + "}";
            }
            return text;
        }

        #endregion      

    }
}
