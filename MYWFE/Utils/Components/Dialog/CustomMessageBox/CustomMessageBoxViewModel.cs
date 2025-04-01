using MYWFE.Core;

namespace MYWFE.Utils.Components.Dialog.CustomMessageBox
{
    #region DialogSettings
    public class CustomMessageBoxInput : IDialogContentInput
    {
        public string MainText { get; set; }
        public CustomMessageBoxInput( string mainText)
        {
            MainText = mainText;
        }
    }
    public class CustomMessageBoxOutput(DialogActionResult dialogActionResult) : IDialogContentOutput
    {
        public DialogActionResult DialogActionResult { get; } = dialogActionResult;
    }
    #endregion

    class CustomMessageBoxViewModel : Core.ViewModel, IDialogContentViewModel<CustomMessageBoxInput, CustomMessageBoxOutput>
    {
        #region Values
        private TaskCompletionSource<CustomMessageBoxOutput>? _tcs;

        private string _mainText;
        public string MainText
        {
            get { return _mainText; }
            set { _mainText = value; onPropertyChanged(nameof(MainText)); }
        }
        #endregion
        #region Commands
        private RelayCommand _confirmCommand;
        public RelayCommand ConfirmCommand
        {
            get
            {
                return _confirmCommand ??= new RelayCommand(async obj =>
                {
                    await Task.Run(() => _tcs?.SetResult(new(DialogActionResult.Confirm)));
                }, obj => true);
            }
        }

        private RelayCommand _dismissCommand { get; set; }
        public RelayCommand DismissCommand
        {
            get
            {
                return _dismissCommand ??= new RelayCommand(async obj =>
                {
                    await Task.Run(() => _tcs?.SetResult(new(DialogActionResult.Dismiss)));
                }, obj => true);
            }
        }
        #endregion
        public void Initialize(CustomMessageBoxInput parameters, TaskCompletionSource<CustomMessageBoxOutput> tcs)
        {
            MainText = parameters.MainText;
            _tcs = tcs;
        }
    }
}
