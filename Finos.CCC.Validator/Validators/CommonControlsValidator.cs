using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonControlsValidator : CommonItemValidator<CommonControls, Control>
{
    public override string Filename => "common-controls.yaml";

    public override string Description => "Controls";

    internal override IEnumerable<Control> GetItems(CommonControls commonItem) => commonItem.Controls;

    internal override BoolResult ValidateRelatedCommonItems(IList<Control> itemsToValidate, IList<string> relatedCommonItems)
    {
        var valid = true;
        var errorCount = 0;

        foreach (var control in itemsToValidate)
        {
            foreach (var threat in control.Threats)
            {
                if (!relatedCommonItems.Contains(threat))
                {
                    valid = false;
                    errorCount++;
                    ConsoleWriter.WriteError($"ERROR: {control.Id} contains an invalid common threat: {threat}.");
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }
}
