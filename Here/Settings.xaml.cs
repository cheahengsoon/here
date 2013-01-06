using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace Here
{
    public partial class Settings : PhoneApplicationPage
    {

        App thisApp = Application.Current as App;

        public Settings()
        {
            InitializeComponent();
        }

        private void facebook_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/FB.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("name"))
            {
              //  namefb.Text = thisApp.UserName;
                

            }
        }
    }
}