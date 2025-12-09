using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasureHunt.Domain.Entities
{
    public class TreasureHuntResult
    {
        public int Id { get; set; }
        public int N { get; set; }
        public int M { get; set; }
        public int P { get; set; }
        public string MatrixJson { get; set; }
        public double Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
