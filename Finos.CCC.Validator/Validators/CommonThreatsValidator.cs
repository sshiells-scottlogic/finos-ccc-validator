using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal class CommonThreatsValidator : CommonItemValidator<CommonThreats, Threat>
{
    public override string Filename => "common-threats.yaml";

    public override string Description => "Threats";

    internal override string GetId(Threat item) => item.Id;

    internal override IEnumerable<Threat> GetItems(CommonThreats commonItem) => commonItem.Threats;
}
