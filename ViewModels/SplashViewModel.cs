using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;

namespace cortoespana
{
	public class SplashViewModel : BaseViewModel
	{
		public SplashViewModel()
		{

		}

		private int SPLASH_DISPLAY_LENGTH = 3000;

		private async Task ExecuteLoadInfoCommand()
		{
			using (new Busy(this))
			{
				await Task.Delay(SPLASH_DISPLAY_LENGTH);
				Application.Current.MainPage = new MainPage();
			}
		}


		#region Commands

		Command loadInfoCommand;
		public Command LoadInfoCommand
		{
			get
			{
				return loadInfoCommand ??
					(loadInfoCommand = new Command(async () => await ExecuteLoadInfoCommand(), () => CanReload()));
			}
		}

		#endregion
	}
}