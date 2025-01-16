using CommandLine;
using Finos.CCC.Validator.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;
using Finos.CCC.Validator.Validators;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IFeaturesValidator, FeaturesValidator>();

using IHost host = builder.Build();

ParserResult<ActionInputs> parser = Default.ParseArguments<ActionInputs>(() => new(), args);
parser.WithNotParsed(
    errors =>
    {
        host.Services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DotNet.GitHubAction.Program")
            .LogError("{Errors}", string.Join(
                Environment.NewLine, errors.Select(error => error.ToString())));

        Environment.Exit(2);
    });

await parser.WithParsedAsync(
    async options => await StartAnalysisAsync(options, host));

await host.RunAsync();

static async ValueTask StartAnalysisAsync(ActionInputs inputs, IHost host)
{
    var featureValidator = host.Services.GetRequiredService<IFeaturesValidator>();
    await featureValidator.Validate(inputs.TargetDir);

    Environment.Exit(0);
}