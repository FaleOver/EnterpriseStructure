using Presentation.Abstract;
using Presentation.View;
using Presentation.ViewModel;
using System.Windows;

namespace Presentation.DialogService
{
    public class DialogService : IDialogService
    {
        public bool ShowDialog(object viewModel)
        {
            Window window = viewModel switch
            {
                NewNodeViewModel => new NewNodeWindow(),
                RenameNodeViewModel => new RenameNodeWindow(),
                EmplDirectoryViewModel => new EmployeeDirectoryWindow(),
                EmployeeFormViewModel => new EmployeeFormWindow(),
                _ => throw new Exception($"Нет окна для {viewModel.GetType()}")
            };

            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;

            return window.ShowDialog() == true;
        }
    }
}