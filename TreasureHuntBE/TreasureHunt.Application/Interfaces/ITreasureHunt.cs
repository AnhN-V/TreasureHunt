using System;
using System.Collections.Generic;
using System.Linq;
using TreasureHunt.Domain.DTO;
using TreasureHunt.Domain;

namespace TreasureHuntAPI.Services
{
    public interface ITreasureHuntService
    {
        TreasureHuntResponse Solve(TreasureHuntInput input);
    }

    public class TreasureHuntService : ITreasureHuntService
    {
        public TreasureHuntResponse Solve(TreasureHuntInput input)
        {
            // 1. Tìm tất cả vị trí của mỗi loại rương
            var positions = new Dictionary<int, List<(int row, int col)>>();

            for (int i = 0; i < input.N; i++)
            {
                for (int j = 0; j < input.M; j++)
                {
                    int chest = input.Matrix[i][j];
                    if (!positions.ContainsKey(chest))
                        positions[chest] = new List<(int, int)>();
                    positions[chest].Add((i + 1, j + 1));
                }
            }

            // 2. DP: dp[key][position_index] = min fuel
            var dp = new Dictionary<int, Dictionary<int, double>>();

            // 3. Khởi tạo: bắt đầu tại (1,1) với key 0
            dp[0] = new Dictionary<int, double> { [0] = 0 }; // chỉ có 1 vị trí: (1,1)
            var startPos = (1, 1);

            // 4. Xử lý từng key từ 1 → p
            for (int key = 1; key <= input.P; key++)
            {
                if (!positions.ContainsKey(key))
                    throw new InvalidOperationException($"Không tìm thấy rương số {key}");

                dp[key] = new Dictionary<int, double>();
                var currentPositions = positions[key];

                // 5. Với mỗi vị trí của rương hiện tại
                for (int i = 0; i < currentPositions.Count; i++)
                {
                    var currentPos = currentPositions[i];
                    double minFuel = double.MaxValue;

                    // 6. Tìm cách đến đây từ tất cả vị trí của key trước
                    if (key == 1)
                    {
                        // Từ vị trí bắt đầu
                        minFuel = Distance(startPos, currentPos);
                    }
                    else
                    {
                        var prevPositions = positions[key - 1];
                        for (int j = 0; j < prevPositions.Count; j++)
                        {
                            if (dp[key - 1].ContainsKey(j))
                            {
                                var prevPos = prevPositions[j];
                                double fuel = dp[key - 1][j] + Distance(prevPos, currentPos);
                                minFuel = Math.Min(minFuel, fuel);
                            }
                        }
                    }

                    dp[key][i] = minFuel;
                }
            }

            // 7. Kết quả: min của tất cả vị trí rương p
            double result = double.MaxValue;
            foreach (var fuel in dp[input.P].Values)
            {
                result = Math.Min(result, fuel);
            }

            return new TreasureHuntResponse
            {
                MinFuel = result,
                Path = new List<Position>() // Có thể backtrack để tìm path
            };
        }

        private double Distance((int r1, int c1) p1, (int r2, int c2) p2)
        {
            int dx = p1.r1 - p2.r2;
            int dy = p1.c1 - p2.c2;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}