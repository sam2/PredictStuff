namespace NHLStatsModel.NHLEvents
{
    public class PenaltyEvent : NHLEvent
    {
        public NHLPlayer To { get; set; }
        public NHLPlayer On { get; set; }
        public int Duration { get; set; }
        public string Infraction { get; set; }
    }
}