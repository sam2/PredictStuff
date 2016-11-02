namespace NHLStatsModel.NHLEvents
{
    public class ShotEvent : NHLEvent
    {
        public NHLPlayer Shooter { get; set; }
        public string Type { get; set; }
        public int Distance { get; set; }

        
    }
}