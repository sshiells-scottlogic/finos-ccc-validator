﻿using CSharpFunctionalExtensions;

namespace Finos.CCC.Validator.Validators;

public interface ICommonItemValidator<TCommonItem, TItem>
{
    Task<Result<List<string>>> Validate(string targetDir);
}
internal abstract class CommonItemValidator<TCommonItem, TItem> : FileParser, ICommonItemValidator<TCommonItem, TItem>
{
    public abstract string Filename { get; }
    public abstract string Description { get; }

    internal abstract IEnumerable<TItem> GetItems(TCommonItem commonItem);

    internal abstract string GetId(TItem item);
    public async Task<Result<List<string>>> Validate(string targetDir)
    {
        Console.WriteLine($"Validation of Common {Description} Started");

        var isValid = true;

        var commonItem = await ParseYamlFile<TCommonItem>(Path.Combine(targetDir, Filename));
        var ids = GetItems(commonItem).Select(GetId).ToList();

        var grouped = ids.GroupBy(x => x).Where(x => x.Count() > 1);

        if (grouped.Any())
        {
            isValid = false;

            foreach (var feature in grouped)
            {
                Console.WriteLine($"Error validating Common {Description}. Duplicate Ids identified. {feature.Key} occurs {feature.Count()} times.");
            }
        }

        Console.WriteLine($"Validation of Common {Description} Complete. Status: {isValid.ToPassOrFail()}");

        return isValid ? Result.Success(ids) : Result.Failure<List<string>>($"Error validating Common {Description}");
    }
}
