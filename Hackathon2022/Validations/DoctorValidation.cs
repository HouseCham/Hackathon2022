using FluentValidation;
using Hackathon2022.Models;

namespace Hackathon2022.Validations
{
    public class DoctorValidation : AbstractValidator<Doctor>
    {
        public DoctorValidation()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(50);
            RuleFor(x => x.Surname).NotEmpty().NotNull().MaximumLength(100);
            RuleFor(x => x.Token).NotEmpty().NotNull().MaximumLength(50);
        }
    }
}
