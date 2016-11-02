using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NHLStatsModel
{
    public class NHLBoxScore
    {
        public string AwayTeamCity { get; set; }
        public string AwayTeamName { get; set; } //todo change to NHLTeam

        public string HomeTeamName { get; set; }
        public string HomeTeamCity { get; set; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public int HomeShots { get; set; }
        public int AwayShots { get; set; }

        public int GameId { get; set; }

        public DateTime StartTime { get; set; }      
    }
}
