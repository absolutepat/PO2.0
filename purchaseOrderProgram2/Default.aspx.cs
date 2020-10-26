using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using purchaseOrderProgram2.Models;
using Microsoft.AspNet.Identity;



namespace purchaseOrderProgram2
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Enable this once you have account confirmation enabled for password reset functionality
            ForgotPasswordHyperLink.NavigateUrl = "~/Account/Forgot.aspx";
            //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (Request.IsAuthenticated)
            {
                Response.Redirect("~/Home.aspx");
            }
        }

        //Only want people with KOD emails using this.
        protected bool CheckEmailDomain(string email)
        {
            string[] allowedDomains = { "@kodfp.org", "@patnet.net", "@oit.edu" };
            bool allowed = false;

            foreach (string domain in allowedDomains)
            {
                if (email.EndsWith(domain) == true)
                    allowed = true;
            }

            if (!allowed)
                Response.Write("<script>alert(\"Only email addresses ending @kodfp.org (and for now also @patnet.net, @oit.edu) are allowed.\")</script>");

            return allowed;
        }
        protected void Login_Click(object sender, EventArgs e)
        {

            if (IsValid )//&& CheckEmailDomain(loginEmailTextBox.Text)
            {
                // Validate the user password
                //var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = signinManager.PasswordSignIn(loginEmailTextBox.Text, loginPasswordTextBox.Text, isPersistent: true, shouldLockout: true);

                switch (result)
                {
                    case SignInStatus.Success:
                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                        break;
                    case SignInStatus.LockedOut:
                        Response.Redirect("/Account/Lockout");
                        break;
                    //case SignInStatus.RequiresVerification:
                    //    Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}", Request.QueryString["ReturnUrl"], "true"), true);
                    //    break;
                    case SignInStatus.Failure:
                    default:
                        loginErrorMessageLabel.Text = "Login problem. Are you sure your email and password are correct?";
                        loginErrorMessagePlaceHolder.Visible = true;
                        break;
                }
            }
        }

        protected void RegisterUser_Click(object sender, EventArgs e)
        {
            if (IsValid && CheckEmailDomain(registerEmailTextBox.Text))
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
                var user = new ApplicationUser() { UserName = registerEmailTextBox.Text, Email = registerEmailTextBox.Text };
                IdentityResult result = manager.Create(user, registerPasswordTextBox.Text);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    string code = manager.GenerateEmailConfirmationToken(user.Id);
                    string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                    manager.SendEmail(user.Id, "Confirm your po2.0 account", callbackUrl);

                    signInManager.SignIn(user, isPersistent: true, rememberBrowser: true);
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    registerErrorMessageLabel.Text = result.Errors.FirstOrDefault();
                    registerErrorMessagePlaceHolder.Visible = true;
                }
            }
        }
    }
}
