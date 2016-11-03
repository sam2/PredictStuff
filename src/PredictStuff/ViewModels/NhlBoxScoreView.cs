using NHLStatsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PredictStuff.ViewModels
{
    public class NhlBoxScoreView
    {
        public string AwayTeamName { get; set; } //todo change to NHLTeam
        public string HomeTeamName { get; set; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public int HomeShots { get; set; }
        public int AwayShots { get; set; }

        public NhlBoxScoreView(NHLBoxScore data)
        {
            AwayTeamName = data.AwayTeamCity + " " + data.AwayTeamName;
            HomeTeamName = data.HomeTeamCity + " " + data.HomeTeamName;

            HomeScore = data.HomeScore;
            HomeShots = data.HomeShots;

            AwayScore = data.AwayScore;
            AwayShots = data.AwayShots;
        }


    }
}
