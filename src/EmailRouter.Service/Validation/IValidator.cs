using System.Collections.Generic;

namespace EmailRouter.Service.Validation
{
    public interface IValidator<T>
    {
         IEnumerable<ValidationResult> Validate(T message);
    }
}