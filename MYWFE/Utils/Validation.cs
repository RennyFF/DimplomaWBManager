using FluentValidation;
using MYWFE.Utils.Components.Dialog.CustomModal;

namespace MYWFE.Utils
{
    class ModalValidation : AbstractValidator<CustomModalViewModel>
    {
        public ModalValidation()
        {
            RuleFor(x => x.FeedbackToken)
                .NotEmpty().WithMessage("Поле не может быть пустым.");

            RuleFor(x => x.StatiscticsToken)
                .NotEmpty().WithMessage("Поле не может быть пустым.");
        }
    }
}
