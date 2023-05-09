using StudentEMS.ViewModels;

using System.Windows.Controls;

namespace StudentEMS.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            this.DataContext = new HomeViewModel();
        }
    }
}
