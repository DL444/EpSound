using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EpSound
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen : Page
    {
        public SplashScreen()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await LoadFilters();
        }

        private async System.Threading.Tasks.Task LoadFilters()
        {
            ProgRing.Visibility = Visibility.Visible;
            FailureBox.Visibility = Visibility.Collapsed;
            for (int i = 0; ; i++)
            {
                try
                {
                    ViewModel.FilterParamMgrViewModel filterMgr = await ViewModel.ModelVmAdapter.GetFilterParameters();
                    (Window.Current.Content as Frame).Navigate(typeof(MainPage), filterMgr);
                    return;
                }
                catch (Exception)
                {
                    if (i > 4)
                    {
                        ProgRing.Visibility = Visibility.Collapsed;
                        FailureBox.Visibility = Visibility.Visible;
                        return;
                    }
                }
            }
        }

        private async void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            await LoadFilters();
        }
    }
}
