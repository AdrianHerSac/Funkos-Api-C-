namespace FunkosApi.config;

public class StorageSettings
{
    public string RootPath { get; set; } = "wwwroot/uploads";
    
    public bool DeleteOnStartup { get; set; }
    
    public long MaxFileSize { get; set; } = 5 * 1024 * 1024;
    
    public string[] AllowedExtensions { get; set; } = 
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    
    public string[] AllowedImageTypes { get; set; } = 
        { "image/jpeg", "image/png", "image/gif", "image/webp" };
    
    public string ImagesFolder { get; set; } = "images";
    
    public string DocumentsFolder { get; set; } = "documents";
    
    public string GetFolderPath(string folderName)
    {
        return Path.Combine(RootPath, folderName);
    }
}