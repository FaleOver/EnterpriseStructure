using Presentation.Abstract;
using System.Windows;

namespace Presentation
{
    public partial class EmployeeDirectoryWindow : Window, ICloseable
    {
        public EmployeeDirectoryWindow()
        {
            InitializeComponent();
        }

        public void CloseWithResult(bool result)
        {
            this.DialogResult = result;
        }
    }
}
