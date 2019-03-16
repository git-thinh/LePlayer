using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Customer
    {
        public string Surname { set; get; }
        public string Forename { set; get; }
        public bool HasDiscount { set; get; }
        public int Discount { set; get; }
        public string Address { set; get; }
        public string Postcode { set; get; }
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Surname).NotEmpty();
            RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
            RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
            RuleFor(x => x.Address).Length(20, 250);
            RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
        }

        private bool BeAValidPostcode(string postcode)
        {
            // custom postcode validating logic goes here
            return true;
        }
    }

    public class TestCustomerValidator
    {
        public static void Run()
        {
            var customer = new Customer();
            var validator = new CustomerValidator();
            ValidationResult results = validator.Validate(customer);

            bool success = results.IsValid;
            IList<ValidationFailure> failures = results.Errors;
        }
    }

}
