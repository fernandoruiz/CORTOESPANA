using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using cortoespana;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Xamarin.Forms;

[assembly: Dependency(typeof(AzureMobileService))]
namespace cortoespana
{
	public class AzureMobileService
	{
		public MobileServiceClient Client { get; private set; }
		private IMobileServiceSyncTable<Festivals> festvialsTable;

		private async Task Initialize()
		{
			Client = new MobileServiceClient("http://newferfreeapp.azurewebsites.net");

			var path = Path.Combine(MobileServiceClient.DefaultDatabasePath, "debtsync.db");

			var store = new MobileServiceSQLiteStore(path);

			store.DefineTable<Festivals>();

			await Client.SyncContext.InitializeAsync(store);

			festvialsTable = Client.GetSyncTable<Festivals>();
		}

		private async Task SyncFestival()
		{
			await festvialsTable.PullAsync("allFestivals", festvialsTable.CreateQuery());
			await Client.SyncContext.PushAsync();
		}

		public async Task<List<Festivals>> GetAllFestivals()
		{
			await SyncFestival();
			return await festvialsTable.ToListAsync();
		}

		public async Task<bool> AddFestival(Festivals festival)
		{
			try
			{
				await festvialsTable.InsertAsync(festival);
				await SyncFestival();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
