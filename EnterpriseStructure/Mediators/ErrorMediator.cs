using Presentation.Abstract;
using System.Windows;

namespace Presentation.Mediators
{
    public class WpfErrorMediator : IErrorMediator
    {
        public void ShowError(string message, string title = "Ошибка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
