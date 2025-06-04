namespace detekcezmen.Services;
 
using detekcezmen.Models;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

using System.Security.Cryptography;
using System.Text;

public class Analyzer
{
    private readonly IWebHostEnvironment _env;
    public Analyzer(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string ComputeHash(string content)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(contentBytes);
        return Convert.ToBase64String(hashBytes);
    }

    private void AnalyzeFile(string file, FilesDictionary filesDictionary, AnalyzeResult result)    
    {
        if (!filesDictionary.Files.ContainsKey(file))
        {
            string hash = string.Empty;
            try
            {
                hash = ComputeHash(File.ReadAllText(file));
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to read file '{file}'.", ex);
            }

            filesDictionary.Files[file] = new FileMetadata
            {
                Hash = hash,
                Version = 1
            };
            result.NewFiles.Add((file, 1));
        }
        else
        {
            var fileMetadata = filesDictionary.Files[file];
            var currentHash = string.Empty;
            try
            {
                currentHash = ComputeHash(File.ReadAllText(file));
            }
            catch (IOException ex)
            {
                throw new IOException($"Failed to read file '{file}'.", ex);
            }
            if (fileMetadata.Hash != currentHash)
            {
                filesDictionary.Files[file].Hash = currentHash;
                filesDictionary.Files[file].Version++;
                result.ChangedFiles.Add((file, fileMetadata.Version));
            }
        }
    }

    private void AnalyzeDirectory(string folder, FilesDictionary filesDictionary, AnalyzeResult result)
    {
        string[] files;
        folder = Path.Combine(_env.ContentRootPath, folder);
        if (!Directory.Exists(folder))
        {
            return;
        }
        else files = Directory.GetFiles(folder);
        foreach (var file in files)
        {
            AnalyzeFile(file, filesDictionary, result);
        }

        string[] folders = Directory.GetDirectories(folder);
        foreach (var subFolder in folders)
        {
            AnalyzeDirectory(subFolder, filesDictionary, result);
        }
    }

    public AnalyzeResult Analyze(string srcFolder)
    {
        if (string.IsNullOrEmpty(srcFolder))
        {
            throw new ArgumentException("Source folder cannot be null or empty.", nameof(srcFolder));
        }

        string jsonString;

        try
        {
            jsonString = File.ReadAllText(Path.Combine(_env.ContentRootPath, "appdata", "files.json"));
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException("The file 'files.json' does not exist in the appdata directory.");
        }

        FilesDictionary filesDictionary = JsonSerializer.Deserialize<FilesDictionary>(jsonString)
            ?? new FilesDictionary();
        var result = new AnalyzeResult();

        AnalyzeDirectory(srcFolder, filesDictionary, result);
        
        foreach (var entry in filesDictionary.Files.ToList())
        {
            if (!File.Exists(entry.Key) && entry.Key.StartsWith(srcFolder))
            {
                result.DeletedFiles.Add((entry.Key, entry.Value.Version));
                filesDictionary.Files.Remove(entry.Key);
            }
        }

        string updatedJson = JsonSerializer.Serialize(filesDictionary, new JsonSerializerOptions { WriteIndented = true });
        try
        {
            File.WriteAllText(Path.Combine(_env.ContentRootPath, "appdata", "files.json"), updatedJson);
        }
        catch (Exception ex)
        {
            throw new IOException("Failed to write to 'files.json'.", ex);
        }

        return result;
    }
}
