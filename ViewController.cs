using System;

using UIKit;
using Foundation;
using LocalAuthentication;
using AgileBits;

namespace SecureMyApp
{
    public partial class ViewController : UIViewController
    {

        const string serviceId = "MySecureApp";
        NSError error;
        LAContext context = new LAContext();

        public ViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var hasLogin = NSUserDefaults.StandardUserDefaults.BoolForKey("hasLogin");

            if (hasLogin)
            {
                btnSignUp.Hidden = true;
                btnOnePassword.Enabled = true;
            }
            else
            {
                btnLogin.Hidden = true;
                btnTouchId.Hidden = true;
                btnOnePassword.Enabled = false;

            }

            var storedUsername = NSUserDefaults.StandardUserDefaults.StringForKey("username");
            if (!string.IsNullOrEmpty(storedUsername))
                tbxUsername.Text = storedUsername;

            //Setup 1Pasword button
            btnOnePassword.Hidden &= !OnePasswordExtension.SharedExtension.IsAppExtensionAvailable;        

            //Setup Touch ID button
            btnTouchId.Hidden = true;
            btnTouchId.Hidden &= !context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        async partial void btnLogin_TouchUpInside(UIButton sender)
        {
            if (string.IsNullOrEmpty(tbxUsername.Text) || string.IsNullOrEmpty(tbxPassword.Text))
            {
                var alert = new UIAlertView("Oops!", "You must enter both a username and password", null, "Oops", null);
                alert.Show();
                return;
            }

            if (CheckLogin(tbxUsername.Text, tbxPassword.Text))
            {
                var newVC = new UIViewController();
                await PresentViewControllerAsync(newVC, true);
            }
            else
            {
                var alert = new UIAlertView("Login problem", "wrong username", null, "Oops...again", null);
                alert.Show();
            }
           
        }

        partial void btnSignUp_TouchUpInside(UIButton sender)
        {
            if (string.IsNullOrEmpty(tbxUsername.Text) || string.IsNullOrEmpty(tbxPassword.Text))
            {
                var alert = new UIAlertView("Oops!", "You must enter both a username and password", null, "Oops", null);
                alert.Show();
                return;
            }

            tbxUsername.ResignFirstResponder();
            tbxPassword.ResignFirstResponder();

            var hasLoginKey = NSUserDefaults.StandardUserDefaults.BoolForKey("hasLogin");
            if (!hasLoginKey)
                NSUserDefaults.StandardUserDefaults.SetValueForKey(new NSString(tbxUsername.Text), new NSString("username"));

            Helpers.KeychainHelpers.SetPasswordForUsername(tbxUsername.Text, tbxPassword.Text, serviceId, Security.SecAccessible.Always, true);
            NSUserDefaults.StandardUserDefaults.SetBool(true, "hasLogin");
            NSUserDefaults.StandardUserDefaults.Synchronize();

            var newVC = new UIViewController();
            PresentViewController(newVC, true, null);

            if (OnePasswordExtension.SharedExtension.IsAppExtensionAvailable)
                SaveLoginTo1Password(sender);
        }

        bool CheckLogin(string username, string password)
        {
            if (password == Helpers.KeychainHelpers.GetPasswordForUsername(username, serviceId, true) && username == NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("username")).ToString())
                return true;
            else
            {
                return false;
            }
                
        }

        partial void btnTouchId_TouchUpInside(UIButton sender)
        {
            //Lets double check the device supports Touch ID
            if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out error))
            {
                var replyHandler = new LAContextReplyHandler((success, error) =>
                    {
                        InvokeOnMainThread(() =>
                            {
                                if (success)
                                {
                                    var newVC = new UIViewController();
                                    PresentViewController(newVC, true, null);
                                }
                                else
                                {
                                    var alert = new UIAlertView("OOPS!", "Something went wrong.", null, "Oops", null);
                                    alert.Show();
                                }
                            });
                    });
                context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, "Logging in with Touch ID", replyHandler);
            }
            else
            {
                var alert = new UIAlertView("Error", "TouchID not available", null, "BOOO!", null);
                alert.Show();
            }
                    
        }

        partial void btnOnePassword_TouchUpInside(UIButton sender)
        {
            OnePasswordExtension.SharedExtension.FindLoginForURLString("SecureMyApp.Login", this, (NSObject)sender, (loginDetails, error) =>
                {
                    if (loginDetails == null)
                    {
                        if (error != null && error.Domain == AppExtension.ErrorDomain && error.Code == AppExtension.ErrorCodeCancelledByUser)
                        {
                            return;
                        }

                        Console.WriteLine("Couldn't fill login: {0}", error != null ? error.Description : string.Empty);

                        return;
                    }

                    if (loginDetails.ContainsKey(AppExtension.UsernameKey))
                    {
                        tbxUsername.Text = (NSString)loginDetails[AppExtension.UsernameKey];
                    }

                    if (loginDetails.ContainsKey(AppExtension.PasswordKey))
                    {
                        tbxPassword.Text = (NSString)loginDetails[AppExtension.PasswordKey];
                    }
                });
        }

        void SaveLoginTo1Password(NSObject sender)
        {
            var newLoginDetails = new NSMutableDictionary
            {
                { AppExtension.TitleKey, (NSString)"SecureMyApp" },
                { AppExtension.UsernameKey, (NSString)(string.IsNullOrWhiteSpace(tbxUsername.Text) ? string.Empty : tbxUsername.Text) },
                { AppExtension.PasswordKey, (NSString)(string.IsNullOrWhiteSpace(tbxPassword.Text) ? string.Empty : tbxPassword.Text) },
                { AppExtension.NotesKey, (NSString)"Saved with SecureMyApp sample app" },
                { AppExtension.SectionTitleKey, (NSString)"SecureMyApp App" },
            };

            var passwordGenerationOptions = new NSMutableDictionary
            {
                { AppExtension.GeneratedPasswordMinLengthKey, new NSNumber(6) },
                { AppExtension.GeneratedPasswordMinLengthKey, new NSNumber(10) }
            };

            OnePasswordExtension.SharedExtension.StoreLoginForURLString("SecureMyApp.Login", newLoginDetails, passwordGenerationOptions, this, sender, (loginDetails, error) =>
                {
                    if (loginDetails == null)
                    {
                        if (error != null && error.Domain == AppExtension.ErrorDomain && error.Code == AppExtension.ErrorCodeCancelledByUser)
                        {
                            return;
                        }

                        Console.WriteLine("Couldn't generate login: {0}", error != null ? error.Description : string.Empty);

                        return;
                    }

                    if (loginDetails.ContainsKey(AppExtension.UsernameKey))
                    {
                        tbxUsername.Text = (NSString)loginDetails[AppExtension.UsernameKey];
                    }

                    if (loginDetails.ContainsKey(AppExtension.PasswordKey))
                    {
                        tbxPassword.Text = (NSString)loginDetails[AppExtension.PasswordKey];
                    }
                });


        }
    }
}

