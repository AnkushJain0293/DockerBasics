namespace TrainingService.Models
{
    public class TraineeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TrainingDto> Trainings { get; set; }
    }
}
