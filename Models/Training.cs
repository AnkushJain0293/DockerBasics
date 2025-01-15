namespace TrainingService.Models
{
    public class Training
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Trainee> Trainees { get; set; } = new List<Trainee>();
    }
}
