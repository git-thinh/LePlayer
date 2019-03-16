using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Validator(typeof(PersonCreateRequestModelValidator))]
    public class PersonCreateRequestModel
    {
        public Guid PersonId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }


    public class PersonCreateRequestModelValidator : AbstractValidator<PersonCreateRequestModel>
    {
        //Simple validator that checks for values in Firstname and Lastname
        public PersonCreateRequestModelValidator()
        {
            RuleFor(r => r.Firstname).NotEmpty();
            RuleFor(r => r.Lastname).NotEmpty();
        }
    }
}
