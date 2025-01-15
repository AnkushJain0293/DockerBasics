using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;
using TrainingService.Models;
using TrainingService.Services;

namespace TrainingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraineesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cache;

        public TraineesController(AppDbContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainee([FromBody] Trainee trainee)
        {
            _context.Trainees.Add(trainee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTraineeWithTrainings), new { id = trainee.Id }, trainee);
        }

        /// <summary>
        /// Get a trainee with their associated trainings.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTraineeWithTrainings(int id)
        {
            var cacheKey = $"TraineeWithTrainings-{id}";
            var cachedData = await _cache.GetData<TraineeDto>(cacheKey);

            if (cachedData != null && cachedData != default(TraineeDto))
            {
                return Ok(new { Source = "Cache", Trainee = cachedData });
            }

            var traineeFromDb = await _context.Trainees.
                Include(t => t.Trainings).
                FirstOrDefaultAsync(t => t.Id == id);

            if (traineeFromDb == null)
            {
                return NotFound($"Trainee with ID {id} not found.");
            }

            var traineeDto = new TraineeDto
            {
                Id = traineeFromDb.Id,
                Name = traineeFromDb.Name,
                Trainings = traineeFromDb.Trainings.Select(t => new TrainingDto
                {
                    Id = t.Id,
                    Title = t.Title
                }).ToList()
            };

            await _cache.SetData<TraineeDto>(cacheKey, traineeDto, TimeSpan.FromMinutes(10));
            return Ok(new { Source = "Database", Trainee = traineeDto });
        }

        [HttpGet("with-trainings")]
        public async Task<IActionResult> GetTraineesWithTrainings()
        {
            var trainees = await _context.Trainees.Include(t => t.Trainings).ToListAsync();

            var traineeDtos = trainees.Select(t => new TraineeDto
            {
                Id = t.Id,
                Name = t.Name,
                Trainings = t.Trainings.Select(tr => new TrainingDto
                {
                    Id = tr.Id,
                    Title = tr.Title
                }).ToList()
            }).ToList();

            return Ok(traineeDtos);
        }

        [HttpPost("{traineeId}/associate-training/{trainingId}")]
        public async Task<IActionResult> AddTrainingAssociation(int traineeId, int trainingId)
        {
            var trainee = await _context.Trainees.Include(t => t.Trainings).FirstOrDefaultAsync(t => t.Id == traineeId);
            var training = await _context.Trainings.FirstOrDefaultAsync(t => t.Id == trainingId);

            if (trainee == null || training == null)
                return NotFound();

            trainee.Trainings.Add(training);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Trainee and training associated successfully." });
        }
    }
}
