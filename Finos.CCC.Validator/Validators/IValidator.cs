using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IValidator
{
    Task<BoolResult> Validate(CommonData commonData);
}
