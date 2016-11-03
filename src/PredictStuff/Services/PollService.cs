using NhlStatScraper;
using NHLStatsModel;
using PredictStuff.Models.VoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PredictStuff.Services
{
    public class PollService
    {
        List<Poll> scoreViews;

        public async Task<List<Poll>> GetPolls()
        {
            if (scoreViews == null)
            {
                List<NHLBoxScore> scores = await Scraper.GetBoxScores(DateTime.Now);
                scoreViews = new List<Poll>();
                int i = 0;
                foreach (NHLBoxScore score in scores)
                {
                    Poll p = new Poll();
                    p.Id = i++;
                    p.name = score.GameId.ToString();
                    p.options = new List<VoteOption>();
                    p.options.Add(new VoteOption()
                    {
                        name = score.HomeTeamName,
                        count = 0
                    });
                    p.options.Add(new VoteOption()
                    {
                        name = score.AwayTeamName,
                        count = 0
                    });
                    scoreViews.Add(p);
                }
            }
            return scoreViews;

        }
    }
}
