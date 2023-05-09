using StudentEMS.ViewModels;

using System.Windows.Controls;

namespace StudentEMS.Views
{
    public partial class SubjectView : UserControl
    {
        public SubjectView()
        {
            InitializeComponent();
            this.DataContext = new SubjectViewModel();
        }
    }
}
