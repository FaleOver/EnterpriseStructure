using Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace Presentation.Abstract
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected readonly IErrorMediator ErrorMediator;

        protected BaseViewModel(IErrorMediator errorMediator)
        {
            ErrorMediator = errorMediator;
        }

        protected async Task SafeExecuteAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (BusinessValidationException ex)
            {
                ErrorMediator.ShowError(ex.Message, "Внимание");
            }
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            if (window != null)
                window.DialogResult = false;
        }
    }
}
