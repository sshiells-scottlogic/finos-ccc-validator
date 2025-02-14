namespace Finos.CCC.Validator.Models;

public record BoolResult
{
    public bool Valid { get; set; }
    public int ErrorCount { get; set; }
}
