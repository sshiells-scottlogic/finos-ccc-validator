using CommandLine;
using Finos.CCC.Validator;
using Finos.CCC.Validator.Models;
using Finos.CCC.Validator.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static CommandLine.Parser;


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<CommonFeaturesValidator>();
builder.Services.AddSingleton<CommonThreatsValidator>();
builder.Services.AddSingleton<CommonControlsValidator>();
builder.Services.AddSingleton<FeaturesValidator>();
builder.Services.AddSingleton<ThreatsValidator>();
builder.Services.AddSingleton<ControlsValidator>();
builder.Services.AddSingleton<MetadataReader>();

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
    var commonFeatureValidator = host.Services.GetRequiredService<CommonFeaturesValidator>();
    var commonFeaturesResult = await commonFeatureValidator.Validate(inputs.TargetDir, []);
    var commonThreatsValidator = host.Services.GetRequiredService<CommonThreatsValidator>();
    var commonThreatsResult = await commonThreatsValidator.Validate(inputs.TargetDir, commonFeaturesResult.Ids);
    var commonControlsValidator = host.Services.GetRequiredService<CommonControlsValidator>();
    var commonControlsResult = await commonControlsValidator.Validate(inputs.TargetDir, commonThreatsResult.Ids);

    var metadataReader = host.Services.GetRequiredService<MetadataReader>();
    var metadata = await metadataReader.LoadMetaData(inputs.TargetDir);

    var commonData = new CommonData
    {
        Controls = commonControlsResult.Ids,
        Features = commonFeaturesResult.Ids,
        Threats = commonThreatsResult.Ids,
        MetaData = metadata
    };

    var featuresValidator = host.Services.GetRequiredService<FeaturesValidator>();
    var featuresResult = await featuresValidator.Validate(commonData);
    var threatsValidator = host.Services.GetRequiredService<ThreatsValidator>();
    var threatsResult = await threatsValidator.Validate(commonData);
    var controlsValidator = host.Services.GetRequiredService<ControlsValidator>();
    var controlsResult = await controlsValidator.Validate(commonData);

    var isValid = commonFeaturesResult.Valid
        && commonThreatsResult.Valid
        && commonControlsResult.Valid
        && featuresResult.Valid
        && threatsResult.Valid
        && controlsResult.Valid;

    var errorCount = commonFeaturesResult.ErrorCount
        + commonThreatsResult.ErrorCount
        + commonControlsResult.ErrorCount
        + featuresResult.ErrorCount
        + threatsResult.ErrorCount
        + controlsResult.ErrorCount;

    if (isValid)
    {
        ConsoleWriter.WriteSuccess("Validation Completed Successfully.");
    }
    else
    {
        ConsoleWriter.WriteError($"Validation Failed with {errorCount} error(s).");
    }

    Environment.Exit(isValid ? 0 : 1);
}