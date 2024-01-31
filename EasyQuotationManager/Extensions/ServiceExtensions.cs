using EasyQuotationManager.Repository.IRepository;
using EasyQuotationManager.Repository.Repository;
using EasyQuotationManager.SeedData;
using EasyQuotationManager.Services;
using EasyQuotationManager.Shared.ViewModels;
using EasyQuotationManager.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.Extensions
{
    public static class ServiceExtensions
    {
        // Configure all repository
        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<IInternalLogRepository, InternalLogRepository>();
            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IContactRepository, ContactRepository>();
            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IQuotationRepository, QuotationRepository>();
        }

        // Configure All Fluent Validation
        public static void ConfigureFluentValidation(this IServiceCollection services)
        {
            services.AddTransient<IValidator<ApplicationUserViewModel>, ApplicationUserViewModelValidator>();
            services.AddTransient<IValidator<ApplicationUserEditViewModel>, ApplicationUserEditViewModelValidator>();
        }
    }
}
