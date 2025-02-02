using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonControlsValidator : CommonItemValidator<CommonControls, Control>
{
    public override string Filename => "common-controls.yaml";

    public override string Description => "Controls";

    internal override string GetId(Control item) => item.Id;

    internal override IEnumerable<Control> GetItems(CommonControls commonItem) => commonItem.Controls;
}
