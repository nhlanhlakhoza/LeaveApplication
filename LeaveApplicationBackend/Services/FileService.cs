using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public class FileService
{
    public async Task<string> SaveFileToDesktop(IFormFile file, int leaveId, bool isPdf = false)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string baseFolder = Path.Combine(desktop, "LeaveStorage");

        // Base folder
        if (!Directory.Exists(baseFolder))
        {
            Directory.CreateDirectory(baseFolder);
        }

        // ONLY LeaveDocuments folder (always used)
        string folder = Path.Combine(baseFolder, "LeaveDocuments");

        // Create if not exists
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string originalName = Path.GetFileNameWithoutExtension(file.FileName);
        string extension = Path.GetExtension(file.FileName);

        string finalName = $"{originalName}_Leave{leaveId}{extension}";
        string filePath = Path.Combine(folder, finalName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath;
    }
}