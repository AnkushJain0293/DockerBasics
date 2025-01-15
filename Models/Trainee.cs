namespace TrainingService.Models
{
    public class Trainee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Training> Trainings { get; set; } = new List<Training>();
    }
}
