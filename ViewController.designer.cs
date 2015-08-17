// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SecureMyApp
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnLogin { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnSignUp { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnTouchId { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField tbxPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField tbxUsername { get; set; }

		[Action ("btnLogin_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnLogin_TouchUpInside (UIButton sender);

		[Action ("btnSignUp_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnSignUp_TouchUpInside (UIButton sender);

		[Action ("btnTouchId_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnTouchId_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}
			if (btnSignUp != null) {
				btnSignUp.Dispose ();
				btnSignUp = null;
			}
			if (btnTouchId != null) {
				btnTouchId.Dispose ();
				btnTouchId = null;
			}
			if (tbxPassword != null) {
				tbxPassword.Dispose ();
				tbxPassword = null;
			}
			if (tbxUsername != null) {
				tbxUsername.Dispose ();
				tbxUsername = null;
			}
		}
	}
}
