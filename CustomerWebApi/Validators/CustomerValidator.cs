using FluentValidation;

namespace CustomerWebApi.Validators
{
    public class CustomerValidator : AbstractValidator<Customer.Core.Models.Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.id).NotEmpty().NotNull().GreaterThan(0);
            RuleFor(x => x.age).NotEmpty().NotNull().GreaterThan(18);
            RuleFor(x => x.firstName).NotEmpty().NotNull();
            RuleFor(x => x.lastName).NotEmpty().NotNull();
        }
    }
}
