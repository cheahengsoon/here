using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
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
        bool isPageNew, isSwitch = true;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(Strcons.RSS));

            TileUpdate(Strcons.Tile_title);

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

        }

        void RSSDWN(string RSSName)
        {
            if (isPageNew)
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(RSSName));
                MessageBox.Show("Text");
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
            if (isSwitch)
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

        private void RssFLK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Photo.xaml?link=" + ((PostMessage)(RssFLK.SelectedItem)).link + "&BigImage=" + ((PostMessage)(RssFLK.SelectedItem)).BigImage, UriKind.Relative));
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