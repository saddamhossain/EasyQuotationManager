using EasyQuotationManager.Shared.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Validation
{
    public class ApplicationUserEditViewModelValidator : AbstractValidator<ApplicationUserEditViewModel>
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationUserEditViewModelValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(s => s.FirstName).Cascade(CascadeMode.Stop).NotNull().WithMessage("First Name is required!").NotEmpty().WithMessage("First Name is required!"); ;

            RuleFor(s => s.LastName).Cascade(CascadeMode.Stop).NotNull().WithMessage("Last Name is required!").NotEmpty().WithMessage("Last Name is required!");

            RuleFor(s => s.LicenseExpirationDate).Cascade(CascadeMode.Stop).NotNull().WithMessage("License Expiration Date is required!").NotEmpty().WithMessage("License Expiration Date is required!");

            RuleFor(s => s.Email).Cascade(CascadeMode.Stop).NotNull().WithMessage("Email is required!").NotEmpty().WithMessage("Email is required!");

            RuleFor(s => s.Role).Cascade(CascadeMode.Stop).NotNull().WithMessage("Please select a role!").NotEmpty().WithMessage("Please select a role!");
        }
    }
}
