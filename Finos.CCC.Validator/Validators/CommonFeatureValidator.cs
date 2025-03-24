using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonFeaturesValidator : CommonItemValidator<CommonFeatures, Feature>
{
    public override string Filename => "common-features.yaml";

    public override string Description => "Features";

    internal override IEnumerable<Feature> GetItems(CommonFeatures commonItem) => commonItem.Features;

    internal override BoolResult ValidateRelatedCommonItems(IList<Feature> itemsToValidate, IList<BaseItem> relatedCommonItems)
    {
        return new BoolResult { Valid = true, ErrorCount = 0 };
    }
}
