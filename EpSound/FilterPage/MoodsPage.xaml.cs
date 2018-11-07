using EpSound.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EpSound.FilterPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoodsPage : Page
    {
        public FilterParamMgrViewModel FilterManager { get; private set; }

        public MoodsPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is FilterParamMgrViewModel mgr)
            {
                FilterManager = mgr;
                DataContext = FilterManager;
            }
        }
    }
}
