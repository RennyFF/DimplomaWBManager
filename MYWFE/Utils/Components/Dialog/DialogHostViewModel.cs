namespace MYWFE.Utils.Components.Dialog
{
    public enum DialogActionResult
    {
        Confirm,
        Dismiss
    }

    public interface IDialogContentInput
    {
    }

    public interface IDialogContentOutput
    {
    }
    public interface IDialogContentViewModel<TInput, TOutput> where TInput : IDialogContentInput where TOutput : IDialogContentOutput
    {
        void Initialize(TInput parameters, TaskCompletionSource<TOutput> taskCompletionSource);
    }
    public class DialogHostViewModel : Core.ViewModel
    {
        #region Values
        private object? _contentViewModel;
        public object? ContentViewModel
        {
            get { return _contentViewModel; }
            set { _contentViewModel = value; onPropertyChanged(nameof(ContentViewModel)); }
        }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; onPropertyChanged(nameof(IsVisible)); }
        }
        #endregion
        #region Methods
        public async Task<TOutput> ShowAsync<TInput, TOutput>(IDialogContentViewModel<TInput, TOutput> contentViewModel, TInput input) where TInput : IDialogContentInput where TOutput : IDialogContentOutput
        {
            IsVisible = true;

            ContentViewModel = contentViewModel;

            var taskCompletionSource = new TaskCompletionSource<TOutput>();

            contentViewModel.Initialize(input, taskCompletionSource);

            var result = await taskCompletionSource.Task.WaitAsync(CancellationToken.None);

            IsVisible = false;

            return result;
        }
        #endregion
    }
}
