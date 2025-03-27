using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonThreatsValidator : CommonItemValidator<CommonThreats, Threat>
{
    public override string Filename => "common-threats.yaml";

    public override string Description => "Threats";

    internal override IEnumerable<Threat> GetItems(CommonThreats commonItem) => commonItem.Threats;

    internal override BoolResult ValidateRelatedCommonItems(IList<Threat> itemsToValidate, IDictionary<string, BaseItem> relatedCommonItems)
    {
        var valid = true;
        var errorCount = 0;

        var featureIds = relatedCommonItems.Select(x => x.Key).ToList();

        foreach (var threat in itemsToValidate)
        {
            foreach (var feature in threat.Features)
            {
                if (!featureIds.Contains(feature))
                {
                    valid = false;
                    errorCount++;
                    ConsoleWriter.WriteError($"ERROR: {threat.Id} contains an invalid common feature: {feature}.");
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }
}
