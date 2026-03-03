using Presentation.ViewModel;
using System.Windows;

namespace EnterpriseStructure
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void TreeView_SelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedNode = e.NewValue as StructureNodeViewModel;
            }
        }
    } 
}