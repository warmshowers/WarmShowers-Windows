using Microsoft.Phone.Controls;

namespace WSApp
{
    public partial class FilterPage : PhoneApplicationPage
    {
        public FilterPage()
        {
            InitializeComponent();

            DataContext = App.ViewModelFilter;
        }
    }
}