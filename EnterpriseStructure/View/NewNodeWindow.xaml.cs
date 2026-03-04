using Presentation.Abstract;
using System.Windows;

namespace Presentation.View
{
    public partial class NewNodeWindow : Window, ICloseable
    {
        public NewNodeWindow()
        {
            InitializeComponent();
        }

        public void CloseWithResult(bool result)
        {
            this.DialogResult = result;
        }
    }
}
