using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using purchaseOrderProgram2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchaseOrderProgram2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Bootstratp_Click(object sender, EventArgs e)
        {

            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));           
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            IdentityRole newRoleBucket = null;

            if (!roleManager.RoleExists("_Admin"))
            {
                newRoleBucket = new IdentityRole();
                newRoleBucket.Name = "_Admin";
                roleManager.Create(newRoleBucket);
            }

            if (!roleManager.RoleExists("_Purchasing Agent"))
            {
                newRoleBucket = new IdentityRole();
                newRoleBucket.Name = "_Purchasing Agent";
                roleManager.Create(newRoleBucket);
            }

            userManager.AddToRole(User.Identity.GetUserId(), "_Admin");
            Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }
    }
}