namespace detekcezmen.Models;

public class FilesDictionary
{
    public Dictionary<string, FileMetadata> Files { get; set; } = [];
}

public class FileMetadata
{
    public string Hash { get; set; } = string.Empty;
    public int Version { get; set; }
}