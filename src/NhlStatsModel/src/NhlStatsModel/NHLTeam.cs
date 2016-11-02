using System.Collections.Generic;

namespace NHLStatsModel
{
    public class NHLTeam
    {
        public string Location { get; set; }
        public string Name { get; set; }

        public List<NHLPlayer> Roster { get; set; }
    }

    
}