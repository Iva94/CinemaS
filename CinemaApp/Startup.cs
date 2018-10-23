using CinemaApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CinemaApp.Startup))]
namespace CinemaApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        //private void CreateRolesAndUsers()
        //{
        //   cinemaDatabaseEntities dbContext = new cinemaDatabaseEntities();

        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));

        //    if (!roleManager.RoleExists("Admin"))
        //    {
        //        var role = new IdentityRole();
        //        role.Name = "Admin";
        //        roleManager.Create(role);

        //        var user = new ApplicationUser();
        //        user.UserName = "admin";
        //        user.Email = "admin@hotmail.com";
        //        string userPWD = "A@Y200722";
        //        var chkUser = userManager.Create(user, userPWD);

        //        if (chkUser.Succeeded)
        //        {
        //            var result1 = userManager.AddToRole(user.Id, "Admin");
        //        }
        //    }

        //    if (!roleManager.RoleExists("Customer"))
        //    {
        //        var role = new IdentityRole("Customer");
        //        role.Name = "Customer";
        //        roleManager.Create(role);
        //    }
        //}
    }
}
