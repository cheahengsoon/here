using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Here
{
    public partial class MainPage : PhoneApplicationPage
    {
        StringConst Strcons = new StringConst();
        BackgroundWorker backroundWorker;
        bool isPageNew = true;
        bool isSwitch = true;
        Popup myPopup;
        private BackgroundWorker rss = new BackgroundWorker();

        public MainPage()
        {
            InitializeComponent();
            myPopup = new Popup() { IsOpen = true, Child = new ASplashScreen() };
            this.Loaded += MainPage_Loaded;
        }

        private void rss_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RSSDWN(Strcons.RSS);
            RSSDWN(Strcons.FLICKR);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            rss.RunWorkerCompleted += new RunWorkerCompletedEventHandler(rss_RunWorkerCompleted);
            rss.RunWorkerAsync();
            TileUpdate(Strcons.Tile_title);
            backroundWorker = new BackgroundWorker();
            PeriodicTask periodicTask = new PeriodicTask(Strcons.Task_description)
        {
            Description = Strcons.Task_description
        };
            try
            {
                ScheduledActionService.Add(periodicTask);
            }
            catch
            { }
            RunBackroundWorker();
        }

        void RSSDWN(string RSSName)
        {
            if (isPageNew)
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(RSSName));
            }
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ParseRSSAndBindData(e.Result);
            }
            else
            {
                MessageBox.Show(Strcons.error_network);
                NavigationService.GoBack();
            }
        }

        void ParseRSSAndBindData(string RSSText)
        {
            if (isSwitch == true)
            {
                XElement rssnz = XElement.Parse(RSSText);
                var nzpost = (from post in rssnz.Descendants("item")
                              select new PostMessage
                              {
                                  title = post.Element("title").Value,
                                  pubDate = DateTime.Parse(post.Element("pubDate").Value),
                                  link = post.Element("link").Value
                              });
                RssAll.ItemsSource = nzpost;
                isSwitch = false;
            }
            else
            {
                XElement rssnz = XElement.Parse(RSSText);
                XNamespace ns = "http://search.yahoo.com/mrss/";
                var nzpost = (from post in rssnz.Descendants("item")
                              select new PostMessage
                              {
                                  title = post.Element("title").Value,
                                  pubDate = DateTime.Parse(post.Element("pubDate").Value),
                                  link = post.Element("link").Value,
                                  BigImage = post.Element(ns + "content").Attribute("url").Value
                              });
                RssFLK.ItemsSource = nzpost;
                isSwitch = true;
                isPageNew = false;
            }
        }
        //открытие статьи. + передача параметров в read.xaml
        private void RssAll_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Read.xaml?link=" + ((PostMessage)(RssAll.SelectedItem)).link + "&title=" + ((PostMessage)(RssAll.SelectedItem)).title + "&date=" + ((PostMessage)(RssAll.SelectedItem)).pubDate, UriKind.Relative));
        }
        //обновление главного tile 
        public void TileUpdate(string TitleTile)
        {
            var apptile = ShellTile.ActiveTiles.First();
            var appTileData = new StandardTileData();
            appTileData.Title = TitleTile;
            appTileData.Count = 0;
            appTileData.BackgroundImage = new Uri("/TilePic.png", UriKind.RelativeOrAbsolute);
            apptile.Update(appTileData);
        }
        //обработка тапов по иконкам соцсетей
        private void SocTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            if (sender == google)
                cvtap.Uri = new Uri(Strcons.google_str, UriKind.Absolute);
            else if (sender == youtube)
                cvtap.Uri = new Uri(Strcons.youtube_str, UriKind.Absolute);
            else if (sender == facebook)
                cvtap.Uri = new Uri(Strcons.facebook_str, UriKind.Absolute);
            else if (sender == twitter)
                cvtap.Uri = new Uri(Strcons.twitter_str, UriKind.Absolute);
            else if (sender == linkedin)
                cvtap.Uri = new Uri(Strcons.linkedin_str, UriKind.Absolute);
            else if (sender == staskulesh)
                cvtap.Uri = new Uri(Strcons.sklink, UriKind.Absolute);
            else if (sender == Rate)
            {
                cvtap.Uri = new Uri(Strcons.ratestr, UriKind.Absolute);
            }
            cvtap.Show();
        }

        private void RssFLK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Photo.xaml?link=" + ((PostMessage)(RssFLK.SelectedItem)).link + "&BigImage=" + ((PostMessage)(RssFLK.SelectedItem)).BigImage, UriKind.Relative));
        }

        private void RunBackroundWorker()
        {
            backroundWorker.DoWork += ((s, args) =>
                {
                    Thread.Sleep(5000);
                });

            backroundWorker.RunWorkerCompleted += ((s, args) =>
                {
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            this.myPopup.IsOpen = false;
                        }
                    );
                });
            backroundWorker.RunWorkerAsync();
        }
        //Message мне
        private void MailSend(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask ectask = new EmailComposeTask();
            ectask.Subject = Strcons.Email_subject;
            ectask.To = Strcons.Email_to;
            ectask.Show();
        }
    }
}