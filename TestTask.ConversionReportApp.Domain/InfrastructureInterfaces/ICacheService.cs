namespace TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;

public interface ICacheService<T>
{
    public Task<T?> GetAsync(string key, CancellationToken token);

    public Task SetAsync(string key, T item, CancellationToken token);
}