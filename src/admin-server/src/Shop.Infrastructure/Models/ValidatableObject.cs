using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Infrastructure.Models
{
    public abstract class ValidatableObject
    {
        public virtual bool IsValid()
        {
            return Validate().Count == 0;
        }

        public virtual IList<ValidationResult> Validate()
        {
            IList<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, new ValidationContext(this, null, null), validationResults, true);
            return validationResults;
        }
    }
}
