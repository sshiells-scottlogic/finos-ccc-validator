using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonThreatsValidator : CommonItemValidator<CommonThreats, Threat>
{
    public override string Filename => "threats.yaml";

    public override string Description => "Threats";

    internal override IEnumerable<Threat> GetItems(CommonThreats commonItem) => commonItem.Threats;

    internal override BoolResult ValidateRelatedCommonItems(IList<Threat> itemsToValidate, IDictionary<string, BaseItem> relatedCommonItems)
    {
        var valid = true;
        var errorCount = 0;

        var capabilityIds = relatedCommonItems.Select(x => x.Key).ToList();

        foreach (var threat in itemsToValidate)
        {
            var cccSharedCapabilities = threat.Capabilities.FirstOrDefault(x => x.ReferenceId == "CCC");

            if (cccSharedCapabilities != null)
            {
                foreach (var capability in cccSharedCapabilities.Identifiers)
                {
                    if (!capabilityIds.Contains(capability))
                    {
                        valid = false;
                        errorCount++;
                        ConsoleWriter.WriteError($"ERROR: {threat.Id} contains an invalid common capability: {capability}.");
                    }
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }
}
