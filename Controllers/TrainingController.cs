using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;
using TrainingService.Models;
using TrainingService.Services;

namespace TrainingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cache;
        public TrainingController(AppDbContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddTraining([FromBody] Training training)
        {
            _context.Trainings.Add(training);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTraining), new { id = training.Id }, training);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTraining(int id)
        {
            var cacheKey = $"Trainings-{id}";
            var cachedData = await _cache.GetData<Training>(cacheKey);

            if (cachedData != null && cachedData != default(Training))
            {
                return Ok(new { Source = "Cache", Training = cachedData });
            }

            var training = await _context.Trainings.
                FirstOrDefaultAsync(t => t.Id == id);

            if (training == null)
                return NotFound();

            var trainingDto = new TrainingDto
            {
                Title = training.Title,
                Id = training.Id,
            };

            await _cache.SetData<TrainingDto>(cacheKey, trainingDto, TimeSpan.FromMinutes(10));

            return Ok(new { Source = "Database", Training = trainingDto });
        }
    }
}
