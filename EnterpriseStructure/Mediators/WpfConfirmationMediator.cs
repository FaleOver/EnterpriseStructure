using Presentation.Abstract;
using System.Windows;

namespace Presentation.Mediators
{
    class WpfConfirmationMediator : IConfirmationMediator
    {
        public bool Confirm(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, 
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
