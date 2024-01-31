using EasyQuotationManager.Data;
using EasyQuotationManager.Model;
using EasyQuotationManager.Shared.Constant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyQuotationManager.SeedData
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            try
            {
                // If any migration pending, then it will migrate that.
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

            #region If the particular role does not exist in database, then the following code execute and create the follwoing Roles
            if (!_context.Roles.Any(s => s.Name == SD.AdminUser))
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminUser)).GetAwaiter().GetResult();
            }

            if (!_context.Roles.Any(s => s.Name == SD.RegisteredUser))
            {
                _roleManager.CreateAsync(new IdentityRole(SD.RegisteredUser)).GetAwaiter().GetResult();
            }
            #endregion


            // By Default one Admin user will be created if this user not exist in database. It's happening when the project run first time. You can update using your information.
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "saddamhossain953@gmail.com",
                Email = "saddamhossain953@gmail.com",
                FirstName = "Saddam",
                LastName = "Hossain",
                LicenseExpirationDate = DateTime.UtcNow.AddMonths(1),
                EmailConfirmed = true,
                PhoneNumber = "8801721927824",
                CreatedBy = "saddamhossain953@gmail.com",
                CreatedDate = DateTime.Now
            }, "Admin@12345").GetAwaiter().GetResult();

            // Here which admin user already created using the above code that's user role added in our database
            IdentityUser user = await _context.Users.FirstOrDefaultAsync(s => s.Email == "saddamhossain953@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.AdminUser);
        }
    }
}
