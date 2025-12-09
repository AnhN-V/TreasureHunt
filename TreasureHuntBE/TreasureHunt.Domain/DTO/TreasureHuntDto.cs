using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreasureHunt.Domain.DTO
{
    public class TreasureHuntInput
    {
        [Required]
        [Range(1, 500)]
        public int N { get; set; }

        [Required]
        [Range(1, 500)]
        public int M { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int P { get; set; }

        [Required]
        public int[][] Matrix { get; set; }
    }

    public class TreasureHuntResponse
    {
        public double MinFuel { get; set; }
        public List<Position> Path { get; set; }
    }

    public class Position
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int ChestNumber { get; set; }
    }
}
