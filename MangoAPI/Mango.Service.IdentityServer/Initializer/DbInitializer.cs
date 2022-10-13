using IdentityModel;
using Mango.Service.IdentityServer.DbContexts;
using Mango.Service.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Service.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(Settings.ADMIN).Result != null) return;
            
            _roleManager.CreateAsync(new IdentityRole(Settings.ADMIN)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Settings.CUSTOMER)).GetAwaiter().GetResult();

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "0123456788",
                FirstName = "Void",
                LastName = "Admin"
            };
        
            _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, Settings.ADMIN).GetAwaiter().GetResult();
            var tempAdmin = _userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{adminUser.FirstName} {adminUser.LastName}"),
                new Claim(JwtClaimTypes.GivenName, $"{adminUser.FirstName}"),
                new Claim(JwtClaimTypes.FamilyName, $"{adminUser.LastName}"),
                new Claim(JwtClaimTypes.Role, Settings.ADMIN)
            }).Result;

            ApplicationUser customerUser = new ApplicationUser()
            {
                UserName = "customer",
                Email = "customer@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "0123456789",
                FirstName = "Potato",
                LastName = "Customer"
            };

            _userManager.CreateAsync(customerUser, "Customer123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, Settings.CUSTOMER).GetAwaiter().GetResult();
            var tempUser = _userManager.AddClaimsAsync(customerUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{customerUser.FirstName} {customerUser.LastName}"),
                new Claim(JwtClaimTypes.GivenName, $"{customerUser.FirstName}"),
                new Claim(JwtClaimTypes.FamilyName, $"{customerUser.LastName}"),
                new Claim(JwtClaimTypes.Role, Settings.CUSTOMER)
            }).Result;
        }
    }
}
