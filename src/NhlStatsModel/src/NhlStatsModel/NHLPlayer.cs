namespace NHLStatsModel
{
    public class NHLPlayer
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }//NHLTeam team

        public NHLPlayer(int number, string name,  string team) 
        {
            Number = number;
            Name = name;
            Team = team;
        }
    }
}