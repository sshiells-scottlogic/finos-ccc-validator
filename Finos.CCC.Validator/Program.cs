﻿using CommandLine;
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
    var isValid = true;

    // TODO Parse metadata

    var commonFeatureValidator = host.Services.GetRequiredService<CommonFeaturesValidator>();
    var commonFeaturesResult = await commonFeatureValidator.Validate(inputs.TargetDir);
    var commonThreatsValidator = host.Services.GetRequiredService<CommonThreatsValidator>();
    var commonThreatsResult = await commonThreatsValidator.Validate(inputs.TargetDir);
    var commonControlsValidator = host.Services.GetRequiredService<CommonControlsValidator>();
    var commonControlsResult = await commonControlsValidator.Validate(inputs.TargetDir);

    // populate id fields

    // validate features

    // valdiate threats

    // validate controls

    Environment.Exit(isValid ? 0 : 1);
}