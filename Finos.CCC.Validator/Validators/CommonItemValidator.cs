using CSharpFunctionalExtensions;

namespace Finos.CCC.Validator.Validators;

public interface ICommonItemValidator<TCommonItem, TItem>
{
    Task<Result<List<string>>> Validate(string targetDir);
}
internal abstract class CommonItemValidator<TCommonItem, TItem> : Validator, ICommonItemValidator<TCommonItem, TItem>
{
    public abstract string Filename { get; }
    public abstract string Description { get; }

    internal abstract IEnumerable<TItem> GetItems(TCommonItem commonItem);

    internal abstract string GetId(TItem item);
    public async Task<Result<List<string>>> Validate(string targetDir)
    {
        Console.WriteLine($"Validation of Common {Description} Started");

        List<string> ids = [];

        var isValid = true;

        var commonItem = await ParseYamlFile<TCommonItem>(Path.Combine(targetDir, Filename));

        var grouped = GetItems(commonItem).GroupBy(GetId).Where(x => x.Count() > 1);

        if (grouped.Any())
        {
            // TODO - validate ids against metadata 

            isValid = false;

            foreach (var feature in grouped)
            {
                Console.WriteLine($"Error validating Common {Description}. Duplicate Ids identified. {feature.Key} occurs {feature.Count()} times.");
            }
        }

        var status = isValid ? "SUCCESS" : "FAILED";
        Console.WriteLine($"Validation of Common {Description} Complete. Status: {status}");

        return isValid ? Result.Success(ids) : Result.Failure<List<string>>($"Error validating Common {Description}");
    }
}
