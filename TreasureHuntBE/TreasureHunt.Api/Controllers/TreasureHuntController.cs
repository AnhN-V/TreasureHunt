using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TreasureHunt.Domain.DTO;
using TreasureHunt.Domain.Entities;
using TreasureHunt.Infrastructure.Data;
using TreasureHuntAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace TreasureHunt.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreasureHuntController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITreasureHuntService _treasureHuntService;

        public TreasureHuntController(
            AppDbContext context,
            ITreasureHuntService treasureHuntService)
        {
            _context = context;
            _treasureHuntService = treasureHuntService;
        }

        [HttpPost("solve")]
        public async Task<ActionResult<TreasureHuntResponse>> Solve([FromBody] TreasureHuntInput input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (input.P > input.N * input.M)
            {
                return BadRequest(new { message = $"P must be <= N * M ({input.N * input.M})" });
            }

            if (input.Matrix.Length != input.N)
            {
                return BadRequest(new { message = "Matrix row count doesn't match N" });
            }

            foreach (var row in input.Matrix)
            {
                if (row.Length != input.M)
                {
                    return BadRequest(new { message = "Matrix column count doesn't match M" });
                }

                foreach (var val in row)
                {
                    if (val < 1 || val > input.P)
                    {
                        return BadRequest(new { message = $"Matrix values must be between 1 and {input.P}" });
                    }
                }
            }

            try
            {
                var result = _treasureHuntService.Solve(input);

                var dbResult = new TreasureHuntResult
                {
                    N = input.N,
                    M = input.M,
                    P = input.P,
                    MatrixJson = JsonSerializer.Serialize(input.Matrix),
                    Result = result.MinFuel,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TreasureHuntResult.Add(dbResult);
                await _context.SaveChangesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<object>>> GetHistory()
        {
            var history = await _context.TreasureHuntResult
                .OrderByDescending(r => r.CreatedAt)
                .Take(20)
                .ToListAsync();

            var result = history.Select(r => new
            {
                r.Id,
                r.N,
                r.M,
                r.P,
                Matrix = JsonSerializer.Deserialize<int[][]>(r.MatrixJson),
                r.Result,
                r.CreatedAt
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var result = await _context.TreasureHuntResult.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                result.Id,
                result.N,
                result.M,
                result.P,
                Matrix = JsonSerializer.Deserialize<int[][]>(result.MatrixJson),
                result.Result,
                result.CreatedAt
            });
        }
    }
}