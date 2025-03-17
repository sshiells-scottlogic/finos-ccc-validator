namespace Finos.CCC.Validator.Models;

public record IdResult
{
    public bool Valid { get; set; }
    public List<BaseItem> Ids { get; set; }
    public int ErrorCount { get; set; }
}