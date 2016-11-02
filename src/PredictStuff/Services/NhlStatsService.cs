using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NhlStatScraper;
using NHLStatsModel;

namespace PredictStuff.Services
{
    public class NhlStatsService
    {
        public async Task<List<NHLBoxScore>> GetBoxScores()
        {
            return await Scraper.GetBoxScores(DateTime.Now);
        }

    }
}
