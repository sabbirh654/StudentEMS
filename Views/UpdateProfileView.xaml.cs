using StudentEMS.ViewModels;

using System.Windows.Controls;

namespace StudentEMS.Views
{
    public partial class UpdateProfileView : UserControl
    {
        public UpdateProfileView()
        {
            InitializeComponent();
            this.DataContext = new UpdateProfileViewModel();
        }
    }
}
