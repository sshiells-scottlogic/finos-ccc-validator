using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonControlsValidator : CommonItemValidator<CommonControls, Control>
{
    public override string Filename => "controls.yaml";

    public override string Description => "Controls";

    internal override IEnumerable<Control> GetItems(CommonControls commonItem) => commonItem.ControlFamilies.SelectMany(x => x.Controls);

    internal override BoolResult ValidateRelatedCommonItems(IList<Control> itemsToValidate, IDictionary<string, BaseItem> relatedCommonItems)
    {
        var valid = true;
        var errorCount = 0;

        var threadIds = relatedCommonItems.Select(x => x.Key).ToList();

        foreach (var control in itemsToValidate)
        {
            var cccThreatMapping = control.ThreatMappings.FirstOrDefault(x => x.ReferenceId == "CCC");

            if (cccThreatMapping != null)
            {
                foreach (var threat in cccThreatMapping.Identifiers)
                {
                    if (!threadIds.Contains(threat))
                    {
                        valid = false;
                        errorCount++;
                        ConsoleWriter.WriteError($"ERROR: {control.Id} contains an invalid common threat: {threat}.");
                    }
                }
            }
        }

        return new BoolResult { Valid = valid, ErrorCount = errorCount };
    }
}
