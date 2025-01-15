namespace TrainingService.Services
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key);
        Task<bool> SetData<T>(string key, T value, TimeSpan expirationTime);
    }
}
