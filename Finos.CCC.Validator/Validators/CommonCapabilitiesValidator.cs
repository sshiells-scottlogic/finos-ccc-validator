using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonCapabilitiesValidator : CommonItemValidator<CommonCapabilities, Capability>
{
    public override string Filename => "capabilities.yaml";

    public override string Description => "Capabilities";

    internal override IEnumerable<Capability> GetItems(CommonCapabilities commonItem) => commonItem.Capabilities;

    internal override BoolResult ValidateRelatedCommonItems(IList<Capability> itemsToValidate, IDictionary<string, BaseItem> relatedCommonItems)
    {
        return new BoolResult { Valid = true, ErrorCount = 0 };
    }
}
