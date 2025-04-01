using System.Collections.ObjectModel;
using MYWFE.Core;
using MYWFE.Utils.Components.Dialog.CustomMessageBox;
using MYWFE.Utils.Components.Dialog;
using MYWFE.Utils.Services;
using MYWFE.Utils.Types;

namespace MYWFE.MVVM.ViewModel
{
    class TemplatesViewModel : Core.ViewModel
    {
        #region PassedValues
        private IAnswerService _answerService;
        public IAnswerService AnswerService
        {
            get { return _answerService; }
            set { _answerService = value; onPropertyChanged(nameof(AnswerService)); }
        }

        private IConfigurationService _configurationService;
        public IConfigurationService ConfigurationService
        {
            get { return _configurationService; }
            set { _configurationService = value; }
        }

        private CustomMessageBoxViewModel CustomMessageBoxViewModel { get; }

        private DialogHostViewModel DialogHost { get; }
        #endregion
        #region Values
        private ObservableCollection<sbyte> _priorityNumbers = new ObservableCollection<sbyte>()
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9
        };
        public ObservableCollection<sbyte> PriorityNumbers
        {
            set => _priorityNumbers = value;
            get => _priorityNumbers;
        }

        private Answer? _selectedAnswer;
        public Answer? SelectedAnswer
        {
            set
            {
                _selectedAnswer = value;
                onPropertyChanged(nameof(SelectedAnswer));
            }
            get => _selectedAnswer;
        }
        #endregion
        #region Commands
        private RelayCommand _addAnswer;
        public RelayCommand AddAnswer
        {
            get
            {
                return _addAnswer ??= new RelayCommand(obj =>
                {
                    AnswerService.AddAnswer(new Answer());
                }, obj => true);
            }
        }
        private RelayCommand _removeAnswer;
        public RelayCommand RemoveAnswer
        {
            get
            {
                return _removeAnswer ??= new RelayCommand(async obj =>
                {
                    if (!ConfigurationService.Configuration.DisableNotifications)
                    {
                        var dialogOutput = await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Вы уверены что хотите удалить шаблон ответа?"));
                        if (dialogOutput.DialogActionResult == DialogActionResult.Dismiss)
                        {
                            return;
                        }
                    }
                    await Task.Run(() => AnswerService.RemoveAnswer((obj as Answer).Id));
                }, obj => SelectedAnswer != null);
            }
        }
        private RelayCommand _saveAnswer;
        public RelayCommand SaveAnswers
        {
            get
            {
                return _saveAnswer ??= new RelayCommand(async obj =>
                {
                    await Task.Run(() => AnswerService.UpdateAllAnswers());
                    if (!ConfigurationService.Configuration.DisableNotifications) {
                        await DialogHost.ShowAsync(CustomMessageBoxViewModel, new CustomMessageBoxInput("Шаблоны успешно сохранены!"));
                    }
                }, obj => true);
            }
        }
        #endregion
        public TemplatesViewModel(IConfigurationService configurationService, IAnswerService answerService, DialogHostViewModel dialogHost, CustomMessageBoxViewModel customMessageBoxViewModel)
        {
            ConfigurationService = configurationService;
            AnswerService = answerService;

            DialogHost = dialogHost;
            CustomMessageBoxViewModel = customMessageBoxViewModel;
        }
    }
}
