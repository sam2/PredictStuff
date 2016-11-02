using NHLStatsModel.NHLEvents;
using System.Collections.Generic;

namespace NHLStatsModel
{
    public class NHLGame
    {
        public string id;

        public NHLBoxScore BoxScore;
        public List<NHLEvent> Events;

        public NHLGame(NHLBoxScore boxScore, List<NHLEvent> events)
        {
            BoxScore = boxScore;
            id = boxScore.GameId.ToString() ;
            Events = events;
        }
    }
}
