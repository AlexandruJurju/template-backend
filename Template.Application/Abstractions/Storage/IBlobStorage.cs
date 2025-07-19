namespace Template.Application.Abstractions.Storage;

public interface IBlobStorage
{
    Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<Guid> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}
