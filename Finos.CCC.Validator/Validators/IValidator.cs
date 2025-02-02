using Finos.CCC.Validator.Models;

namespace Finos.CCC.Validator.Validators;

internal interface IValidator
{
    Task<bool> Validate(CommonData commonData);
}
