using StudentEMS.ViewModels;

using System.Windows;

namespace StudentEMS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
