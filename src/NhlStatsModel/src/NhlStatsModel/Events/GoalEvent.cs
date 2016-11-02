namespace NHLStatsModel.NHLEvents
{
    public class GoalEvent : ShotOnGoalEvent
    {
        public NHLPlayer Assist1 { get; set; }
        public NHLPlayer Assist2 { get; set; }
    }
}