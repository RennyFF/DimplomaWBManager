using MYWFE.Core;
using MYWFE.MVVM.Model.ApiRequests.ResponseTypes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace MYWFE.Utils.Components.Dialog.CustomImageContainerModal
{
    #region DialogSettings
    public class CustomImageContainerModalInput : IDialogContentInput
    {
        public List<ReviewElementPhotoLink> ImageList { get; set; }
    }

    public class CustomImageContainerModalOutput(DialogActionResult dialogActionResult) : IDialogContentOutput
    {
        public DialogActionResult DialogActionResult { get; } = dialogActionResult;
    }
    #endregion
    public class CustomImageContainerModalViewModel : Core.ViewModel, IDialogContentViewModel<CustomImageContainerModalInput, CustomImageContainerModalOutput>
    {
        #region Values
        private TaskCompletionSource<CustomImageContainerModalOutput>? _tcs;

        private ObservableCollection<ReviewElementPhotoLink> _imageList;
        public ObservableCollection<ReviewElementPhotoLink> ImageList
        {
            get { return _imageList; }
            set { _imageList = value; onPropertyChanged(nameof(ImageList)); }
        }
        #endregion
        #region Commands
        private RelayCommand _closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand ??= new RelayCommand(async obj =>
                {
                    await Task.Run(() => _tcs?.SetResult(new(DialogActionResult.Confirm)));
                }, obj => true);
            }
        }

        private RelayCommand _openImageCommand;
        public RelayCommand OpenImageCommand
        {
            get
            {
                return _openImageCommand ??= new RelayCommand(async obj =>
                {
                    var Photolinks = obj as ReviewElementPhotoLink;
                    string photoUrl = Photolinks.fullSize;
                    string localPath = Path.Combine(Path.GetTempPath(), $"photo.jpg");

                    using (var client = new WebClient())
                    {
                        try
                        {
                            await client.DownloadFileTaskAsync(new Uri(photoUrl), localPath);
                            Process photoProcess = new Process
                            {
                                StartInfo = new ProcessStartInfo(localPath) { UseShellExecute = true }
                            };
                            photoProcess.Start();
                            await Task.Delay(3000);
                            File.Delete(localPath);
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                    }
                }, obj => true);
            }
        }
        #endregion
        public void Initialize(CustomImageContainerModalInput parameters, TaskCompletionSource<CustomImageContainerModalOutput> tcs)
        {
            ImageList = new ObservableCollection<ReviewElementPhotoLink>(parameters.ImageList);
            _tcs = tcs;
        }
    }
}
