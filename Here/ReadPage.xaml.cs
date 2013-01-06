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

namespace Here
{
    public partial class ReadPage : PhoneApplicationPage
    {
        public ReadPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
          string tit;
             string desc;
            NavigationContext.QueryString.TryGetValue("tit", out tit);
            NavigationContext.QueryString.TryGetValue("desc", out desc);


          
                        if (!String.IsNullOrEmpty(tit) && !String.IsNullOrEmpty(desc))
                      {
                        //  MessageBox.Show("Good!");
            PageTitle.Text = tit;
            TxtBlog.Text = desc;
            
                    }
        }
    }
}