namespace MYWFE.Utils.Components.SplashScreen
{
    internal class SplashScreenViewModel : Core.ViewModel
    {
        public async Task InitializeAsync()
        {
            await Task.Delay(3000);
        }
    }
}
