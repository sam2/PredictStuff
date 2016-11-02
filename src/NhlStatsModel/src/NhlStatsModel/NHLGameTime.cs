using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLStatsModel
{
    public class NHLGameTime
    {
        public string TimeLeft { get; set; }
        public int Period { get; set; }

        public NHLGameTime(string timeLeft, int period)
        {
            TimeLeft = timeLeft;
            Period = period;
        }
    }
}
