using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cortoespana
{
	public partial class SplashPage : SplashPageXaml
	{
		public SplashPage()
		{
			InitializeComponent();
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.LoadInfoCommand.Execute(null);
		}
	}

	public partial class SplashPageXaml : BaseContentPage<SplashViewModel>
	{
		
	}
}
