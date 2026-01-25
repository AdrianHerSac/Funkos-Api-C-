using FunkosApi.config;
using FunkosApi.Error;
using FunkosApi.Service;
using Microsoft.Extensions.Options;

namespace FunkosApi.Services;

public class FileSystemStorageService : IStorageService
{
    private readonly StorageSettings _settings;
    private readonly ILogger<FileSystemStorageService> _logger;
    private readonly string _rootPath;

    public FileSystemStorageService(
        IOptions<StorageSettings> settings,
        ILogger<FileSystemStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        _rootPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, 
            "..", "..", "..", 
            _settings.RootPath));
        
        _rootPath = Path.GetFullPath(_rootPath);
    }

    public Task InitAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var directories = new[]
            {
                _rootPath,
                Path.Combine(_rootPath, _settings.ImagesFolder),
                Path.Combine(_rootPath, _settings.DocumentsFolder),
                Path.Combine(_rootPath, "temp")
            };

            foreach (var dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    _logger.LogInformation("Directorio creado: {Path}", dir);
                }
            }

            if (_settings.DeleteOnStartup)
            {
                CleanDirectory(_rootPath);
                _logger.LogInformation("Directorio de almacenamiento limpio");
            }
        }, cancellationToken);
    }

    public async Task<string> StoreAsync(
        IFormFile file, 
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("El archivo es nulo o vacío", nameof(file));

        if (file.Length > _settings.MaxFileSize)
            throw new FileSizeExceededException(
                $"El archivo excede el tamaño máximo de {_settings.MaxFileSize / 1024 / 1024}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(extension))
            throw new InvalidFileTypeException(
                $"Tipo de archivo no permitido: {extension}");

        var fileName = GenerateFileName(file.FileName);
        var folderPath = GetFolderPath(folder);
        var filePath = Path.Combine(folderPath, fileName);

        Directory.CreateDirectory(folderPath);
        
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        _logger.LogInformation("Archivo guardado: {FileName} ({Size} bytes)", 
            fileName, file.Length);

        return fileName;
    }

    public async Task<string> StoreAsync(
        Stream stream,
        string fileName,
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(extension))
            throw new InvalidFileTypeException(
                $"Tipo de archivo no permitido: {extension}");

        var fileNameGenerated = GenerateFileName(fileName);
        var folderPath = GetFolderPath(folder);
        var filePath = Path.Combine(folderPath, fileNameGenerated);

        Directory.CreateDirectory(folderPath);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return fileNameGenerated;
    }

    public Task<Stream> LoadAsStreamAsync(
        string fileName, 
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileName, folder);
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Archivo no encontrado: {fileName}");

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream>(stream);
    }

    public string GetFilePath(string fileName, string? folder = null)
    {
        return Path.Combine(GetFolderPath(folder), fileName);
    }

    public string GetUrl(string fileName, string? folder = null)
    {
        var path = folder != null ? $"/uploads/{folder}/{fileName}" : $"/uploads/{fileName}";
        return path.Replace("\\", "/"); // Normalizar para Windows
    }

    public bool Exists(string fileName, string? folder = null)
    {
        var filePath = GetFilePath(fileName, folder);
        return File.Exists(filePath);
    }

    public Task<bool> DeleteAsync(
        string fileName, 
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(fileName, folder);
        
        if (!File.Exists(filePath))
            return Task.FromResult(false);

        File.Delete(filePath);
        _logger.LogInformation("Archivo eliminado: {FileName}", fileName);
        
        return Task.FromResult(true);
    }

    public Task DeleteAllAsync(
        string? folder = null,
        CancellationToken cancellationToken = default)
    {
        var folderPath = GetFolderPath(folder);
        
        if (Directory.Exists(folderPath))
        {
            CleanDirectory(folderPath);
            _logger.LogInformation("Todos los archivos eliminados de: {Folder}", folderPath);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> ListFilesAsync(string? folder = null)
    {
        var folderPath = GetFolderPath(folder);
        
        if (!Directory.Exists(folderPath))
            return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());

        var files = Directory.GetFiles(folderPath)
            .Select(Path.GetFileName);
        
        return Task.FromResult(files)!;
    }

    #region Métodos Privados

    private string GetFolderPath(string? folder)
    {
        if (string.IsNullOrEmpty(folder))
            return _rootPath;

        var folderPath = Path.Combine(_rootPath, folder);
        return Path.GetFullPath(folderPath);
    }

    private static string GenerateFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var uniqueName = Guid.NewGuid().ToString("N")[..16]; // 16 caracteres
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"); // año/mes/dia/hora/min/seg
        return $"{timestamp}_{uniqueName}{extension}";
    }

    private static void CleanDirectory(string path)
    {
        if (!Directory.Exists(path)) return;

        foreach (var file in Directory.GetFiles(path))
        {
            try { File.Delete(file); }
            catch { /* Ignorar errores al borrar */ }
        }
    }

    #endregion
}