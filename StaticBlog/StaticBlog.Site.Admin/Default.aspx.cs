using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity.Owin;

namespace StaticBlog.Site.Admin
{
    // ReSharper disable once InconsistentNaming
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OpenAuthLogin.ReturnUrl = "/Admin";
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            // Validate the user password
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, false);

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (result)
            {
                case SignInStatus.Success:
                    IdentityHelper.RedirectToReturnUrl(Response);
                    break;

                case SignInStatus.LockedOut:
                    Response.Redirect("/Account/Lockout");
                    break;

                case SignInStatus.RequiresVerification:
                    Response.Redirect($"/Account/TwoFactorAuthenticationSignIn?ReturnUrl=/Admin&RememberMe={RememberMe.Checked}",
                        true);
                    break;

                default:
                    FailureText.Text = "Invalid login attempt";
                    ErrorMessage.Visible = true;
                    break;
            }
        }
    }
}