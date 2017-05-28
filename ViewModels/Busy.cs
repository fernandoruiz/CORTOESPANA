using System;

namespace cortoespana
{
	public class Busy : IDisposable
	{
		readonly object _sync = new Object();
		readonly BaseViewModel _viewModel;

		public Busy(BaseViewModel viewModel)
		{
			_viewModel = viewModel;
			lock (_sync)
			{
				_viewModel.IsBusy = true;
			}
		}

		public void Dispose()
		{
			lock (_sync)
			{
				_viewModel.IsBusy = false;
			}
		}
	}
}