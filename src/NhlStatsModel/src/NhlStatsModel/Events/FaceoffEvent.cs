namespace NHLStatsModel.NHLEvents
{
    public class FaceoffEvent : NHLEvent
    {
        public NHLPlayer Winner { get; set; }
        public NHLPlayer Loser { get; set; }

        /*
        public FaceoffEvent(NHLPlayer winner, NHLPlayer loser)
        {
            Winner = winner;
            Loser = loser;
        }
         * */
    }
}