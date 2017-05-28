using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace cortoespana
{
	public class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
	{
		~BaseContentPage()
		{
			_viewModel = null;
		}

		public BaseContentPage()
		{
			BindingContext = ViewModel;

			NavigationPage.SetBackButtonTitle(this, null);
			NavigationPage.SetHasBackButton(this, false);
			NavigationPage.SetHasNavigationBar(this, false);

			Title = this.GetType().Name;
		}

		#region Properties

		protected T _viewModel;

		public T ViewModel
		{
			get
			{
				if (_viewModel == null)
				{
					_viewModel = new T();
					_viewModel.Page = this;
				}

				return _viewModel;
			}
		}

		bool _hasSubscribed;

		double width;
		double height;
		Thickness padding;

		#endregion

		#region Functions

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		protected override bool OnBackButtonPressed()
		{
			if (Navigation.ModalStack.Count > 0)
			{
				this.Navigation.PopModalAsync(true);
				return true;
			}
			else if (Navigation.NavigationStack.Count > 0)
			{
				this.Navigation.PopAsync(true);
				return true;
			}
			return base.OnBackButtonPressed();
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			if (width != this.width || height != this.height)
			{
				this.width = width;
				this.height = height;
				if (width > height)
				{
					ViewModel.Landscape = true;
				}
				else
				{
					ViewModel.Landscape = false;
				}
			}
		}

		#endregion

	}
}