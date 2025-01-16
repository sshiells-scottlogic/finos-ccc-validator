using CommandLine;

namespace Finos.CCC.Validator.Models;

public class ActionInputs
{
    [Option('t', "targetDir", Required = true, HelpText = "The path to target directory")]
    public string TargetDir { get; set; } = null;
}
