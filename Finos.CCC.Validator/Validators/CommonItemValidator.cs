using CSharpFunctionalExtensions;
using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

public interface ICommonItemValidator<TCommonItem, TItem>
{
    Task<IdResult> Validate(string targetDir, IList<string> relatedCommonItems);
}
internal abstract class CommonItemValidator<TCommonItem, TItem> : FileParser, ICommonItemValidator<TCommonItem, TItem>
{
    public abstract string Filename { get; }
    public abstract string Description { get; }

    internal abstract IEnumerable<TItem> GetItems(TCommonItem commonItem);

    internal abstract string GetId(TItem item);

    internal abstract BoolResult ValidateRelatedCommonItems(IList<TItem> itemsToValidate, IList<string> relatedCommonItems);
    public async Task<IdResult> Validate(string targetDir, IList<string> relastedCommonItems)
    {
        Console.WriteLine($"Validation of Common {Description} Started");

        var isValid = true;
        var errorCount = 0;

        var commonItem = await ParseYamlFile<TCommonItem>(Path.Combine(targetDir, Filename));
        var commonItems = GetItems(commonItem).ToList();
        var ids = commonItems.Select(GetId).ToList();

        var grouped = ids.GroupBy(x => x).Where(x => x.Count() > 1);

        if (grouped.Any())
        {
            isValid = false;

            foreach (var feature in grouped)
            {
                ConsoleWriter.WriteError($"Error validating Common {Description}. Duplicate Ids identified. {feature.Key} occurs {feature.Count()} times.");
                errorCount++;
            }
        }

        var additionalValidations = ValidateRelatedCommonItems(commonItems, relastedCommonItems);
        isValid &= additionalValidations.Valid;
        errorCount += additionalValidations.ErrorCount;


        if (isValid)
        {
            Console.WriteLine($"Validation of Common {Description} Complete. Status: {isValid.ToPassOrFail()}");
        }
        else
        {
            ConsoleWriter.WriteError($"Validation of Common {Description} Complete. Status: {isValid.ToPassOrFail()}");
        }

        return new IdResult { Ids = ids, ErrorCount = errorCount, Valid = isValid };
    }
}
