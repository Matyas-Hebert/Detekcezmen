namespace detekcezmen.Models ;

public class AnalyzeResult
{
    public List<(string File, int Version)> ChangedFiles { get; set; } = new List<(string File, int Version)>();
    public List<(string File, int Version)> NewFiles { get; set; } = new List<(string File, int Version)>();
    public List<(string File, int Version)> DeletedFiles { get; set; } = new List<(string File, int Version)>();
}