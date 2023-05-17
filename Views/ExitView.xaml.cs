using StudentEMS.ViewModels;

using System.Windows;
using System.Windows.Controls;

namespace StudentEMS.Views
{
    public partial class ExitView : UserControl
    {
        public ExitView()
        {
            InitializeComponent();
            Application.Current.Shutdown();
        }
    }
}
