using CSharpFunctionalExtensions;
using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

public interface ICommonItemValidator<TCommonItem, TItem>
{
    Task<IdResult> Validate(string targetDir, IDictionary<string, BaseItem> relatedCommonItems);
}
internal abstract class CommonItemValidator<TCommonItem, TItem> : FileParser, ICommonItemValidator<TCommonItem, TItem> where TItem : BaseItem
{
    public abstract string Filename { get; }
    public abstract string Description { get; }

    internal abstract IEnumerable<TItem> GetItems(TCommonItem commonItem);

    internal abstract BoolResult ValidateRelatedCommonItems(IList<TItem> itemsToValidate, IDictionary<string, BaseItem> relatedCommonItems);
    public async Task<IdResult> Validate(string targetDir, IDictionary<string, BaseItem> relatedCommonItems)
    {
        Console.WriteLine($"Validation of Common {Description} Started");

        var isValid = true;
        var errorCount = 0;

        var commonItem = await ParseYamlFile<TCommonItem>(Path.Combine(targetDir, Filename));
        var commonItems = GetItems(commonItem).ToList();

        var grouped = commonItems.GroupBy(x => x.Id).Where(x => x.Count() > 1);

        if (grouped.Any())
        {
            isValid = false;

            foreach (var feature in grouped)
            {
                ConsoleWriter.WriteError($"Error validating Common {Description}. Duplicate Ids identified. {feature.Key} occurs {feature.Count()} times.");
                errorCount++;
            }
        }

        var additionalValidations = ValidateRelatedCommonItems(commonItems, relatedCommonItems);
        isValid &= additionalValidations.Valid;
        errorCount += additionalValidations.ErrorCount;

        var fileResult = ValidateFile(targetDir, relatedCommonItems);
        isValid &= fileResult.Valid;
        errorCount += fileResult.ErrorCount;

        if (isValid)
        {
            Console.WriteLine($"Validation of Common {Description} Complete. Status: {isValid.ToPassOrFail()}");
        }
        else
        {
            ConsoleWriter.WriteError($"Validation of Common {Description} Complete. Status: {isValid.ToPassOrFail()}");
        }

        var baseItems = GetBaseItems(commonItems);

        return new IdResult { Ids = baseItems, ErrorCount = errorCount, Valid = isValid };
    }

    private Dictionary<string, BaseItem> GetBaseItems(List<TItem> commonItems)
    {
        return commonItems.Select(x => x as BaseItem).ToDictionary(x => x.Id);
    }

    internal BoolResult ValidateFile(string targetDir, IDictionary<string, BaseItem> relatedCommonItems)
    {
        var isValid = true;
        var errorCount = 0;

        var ids = relatedCommonItems.Keys;

        foreach (var line in File.ReadLines(Path.Combine(targetDir, Filename)))
        {
            foreach (var id in ids)
            {
                if (line.Contains(id))
                {
                    var index = line.IndexOf(id);
                    var rest = line.Substring(index + id.Length).Trim([' ', '#']);
                    if (rest.ToLower() != relatedCommonItems[id].Title.ToLower())
                    {
                        errorCount++;
                        isValid = false;
                        ConsoleWriter.WriteError($"Invalid comment following Id: {id} has comment '{rest}' but should be '{relatedCommonItems[id].Title}'");
                    }
                }
            }
        }

        return new BoolResult { Valid = isValid, ErrorCount = errorCount };
    }
}
