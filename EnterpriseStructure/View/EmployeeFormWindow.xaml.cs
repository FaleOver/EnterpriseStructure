using Presentation.Abstract;
using System.Windows;

namespace Presentation.View
{
    public partial class EmployeeFormWindow : Window, ICloseable
    {
        public EmployeeFormWindow()
        {
            InitializeComponent();
        }

        public void CloseWithResult(bool result)
        {
            this.DialogResult = result;
        }
    }
}
