using System;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using Plugin.Connectivity;
using System.Threading;
using System.Globalization;

namespace cortoespana
{
	public class BaseViewModel : BaseNotify
	{

		public BaseViewModel()
		{
			Initialize();
			Page = null;
		}

		public BaseViewModel(ContentPage page)
		{
			Initialize();
			Page = page;
		}

		private void Initialize()
		{
			CrossConnectivity.Current.ConnectivityChanged += OnConnectivityChanged;
			AzureMobileService = DependencyService.Get<AzureMobileService>();
		}

		#region Properties

		public ContentPage Page
		{
			get;
			set;
		}

		public AzureMobileService AzureMobileService
		{
			get;
			set;
		}

		private bool _online = false;
		public bool Online
		{
			get
			{
				return _online;
			}
			private set
			{
				_online = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged("Offline");
			}
		}

		public bool Offline
		{
			get
			{
				return !Online;
			}
		}

		private bool _isBusy = false;
		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				_isBusy = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged("IsNotBusy");
			}
		}

		public bool IsNotBusy
		{
			get { return !IsBusy; }
		}

		private bool _landscape;
		public bool Landscape
		{
			get { return _landscape; }

			set
			{
				_landscape = value;
				NotifyPropertyChanged();
				NotifyPropertyChanged("Portrait");
			}
		}

		public bool Portrait
		{
			get { return !Landscape; }
		}

		private bool _configEnabled = true;
		public bool ConfigEnabled
		{
			get { return _configEnabled; }
			set
			{
				_configEnabled = value;
				NotifyPropertyChanged();
			}
		}


		#endregion

		#region Functions

		void OnConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
		{
			Online = e.IsConnected;
		}

		public virtual async Task ShowAbout()
		{
			using (new Busy(this))
			{
				//await Page.Navigation.PushModalAsync(new AboutPage());
			}
		}

		async Task CloseAboutPage()
		{
			await Page.Navigation.PopModalAsync();
		}

		async Task CloseLicensesPage()
		{
			await Page.Navigation.PopModalAsync();
		}

		public string GetContentTextFile(string filename)
		{
			string text = "";
			var assembly = typeof(App).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream(filename);
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
			return text;
		}

		public virtual async Task ShowConfig()
		{
			using (new Busy(this))
			{
				//await Page.Navigation.PushModalAsync(new ConfigPage());
			}
		}

		#endregion

		#region Commands

		Command navigateBackCommand;
		public Command NavigateBackCommand
		{
			get
			{
				return navigateBackCommand ?? (navigateBackCommand = new Command(() =>
				{
					Page.SendBackButtonPressed();
				}));
			}
		}

		public bool CanReload()
		{
			return IsNotBusy;
		}

		Command showAboutCommand;
		public Command ShowAboutCommand
		{
			get
			{
				return showAboutCommand ?? (showAboutCommand = new Command(
					async () => await ShowAbout(), () => CanReload()));
			}
		}

		Command closeAboutPageCommand;
		public Command CloseAboutPageCommand
		{
			get
			{
				return closeAboutPageCommand ?? (closeAboutPageCommand = new Command(
					async () => await CloseAboutPage()));
			}
		}

		Command closeLicensesPageCommand;
		public Command CloseLicensesPageCommand
		{
			get
			{
				return closeLicensesPageCommand ?? (closeLicensesPageCommand = new Command(
					async () => await CloseLicensesPage()));
			}
		}

		Command showConfigCommand;
		public Command ShowConfigCommand
		{
			get
			{
				return showConfigCommand ?? (showConfigCommand = new Command(async () =>
				{
					await ShowConfig();
				}, () => CanReload()));
			}
		}

		#endregion

		#region Helpers

		CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		public CancellationToken CancellationToken
		{
			get
			{
				return _cancellationTokenSource.Token;
			}
		}

		public virtual void CancelTasks()
		{
			if (!_cancellationTokenSource.IsCancellationRequested && CancellationToken.CanBeCanceled)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource = new CancellationTokenSource();
			}
		}

		public async Task RunSafe(Func<Task> execute, bool notifyOnError = false)
		{
			Exception networkException = null;
			Exception exception = null;

			if (!CrossConnectivity.Current.IsConnected)
			{
				Online = false;
			}

			try
			{
				if (!CancellationToken.IsCancellationRequested)
				{
					await execute();
				}
			}
			catch (TaskCanceledException ex)
			{
				exception = ex;
				Debug.WriteLine("Task Cancelled");
				throw ex;
			}
			catch (HttpRequestException ex)
			{
				exception = ex;
				networkException = ex;
			}
			catch (Exception ex)
			{
				exception = ex;
				Debug.WriteLine(@"ERROR {0}", ex.Message);
				await Page.DisplayAlert("ERROR", ex.Message, "OK");
			}
			finally
			{
				if (networkException != null)
				{
					Online = false;
					Debug.WriteLine(networkException);
				}

				if (exception == null)
				{
					Online = true;
				}
			}
		}

		#endregion
	}
}