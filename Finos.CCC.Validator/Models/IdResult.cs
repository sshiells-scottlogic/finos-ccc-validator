namespace Finos.CCC.Validator.Models;

public record IdResult
{
    public bool Valid { get; set; }
    public List<string> Ids { get; set; }
    public int ErrorCount { get; set; }
}