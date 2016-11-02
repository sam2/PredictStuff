namespace NHLStatsModel.NHLEvents
{
    public class BlockedShotEvent : ShotEvent
    {
        public NHLPlayer Blocker { get; set; }
    }
}