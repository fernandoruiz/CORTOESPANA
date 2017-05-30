using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace cortoespana
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
		}

		private async Task ShowFestivales()
		{
			using (new Busy(this))
			{
				var festival = new Festivals { 
					Name= "Festival Test 1",
					Description = "Description Festival Test 1"
				};
				await AzureMobileService.AddFestival(festival);
			}
		}

		#region Commands

		Command showFestivalesCommand;
		public Command ShowFestivalesCommand
		{
			get
			{
				return showFestivalesCommand ??
					(showFestivalesCommand = new Command(async () => await ShowFestivales(), () => CanReload()));
			}
		}

		#endregion
	}
}
