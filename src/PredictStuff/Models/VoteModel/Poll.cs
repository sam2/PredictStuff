using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PredictStuff.Models.VoteModel
{
    public class Poll
    {
        public int Id { get; set; }
        public string name { get; set; }

        public List<VoteOption> options { get; set; }
    }
}

