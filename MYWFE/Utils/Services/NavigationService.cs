using MYWFE.Core;
using MYWFE.MVVM.ViewModel;

namespace MYWFE.Services
{
    public interface INavigationService
    {
        Core.ViewModel CurrentView { get; }
        void Navigate<T>() where T : Core.ViewModel;
    }
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly Func<Type, Core.ViewModel> _viewModelFactory;
        private Core.ViewModel _currentView;

        public Core.ViewModel CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                onPropertyChanged();
            }
        }

        public NavigationService(Func<Type, Core.ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            Core.ViewModel Base_viewModel = _viewModelFactory.Invoke(typeof(HomeViewModel));
            CurrentView = Base_viewModel;
        }

        public void Navigate<TViewModel>() where TViewModel : Core.ViewModel
        {
            Core.ViewModel ViewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = ViewModel;
        }
    }
}
