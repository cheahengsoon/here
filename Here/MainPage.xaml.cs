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

        public const string RSS = "http://feeds.feedburner.com/kulesh?format=xml";
        public const string FLICKR = "http://api.flickr.com/services/feeds/geo/?id=65129513@N00&lang=en-us&format=rss_200";

        string error_network = "Произошла ошибка! Проверьте доступность интернета и попробуйте ещё раз";
        string Task_description = "Here Task";
        string Tile_title = "Здесь в...";
        string Email_subject = "Отзыв о \"Здесь в...\" для WP";
        string Email_to = "sunrizz@outlook.com";

        BackgroundWorker backroundWorker;
        bool isPageNew;
        Popup myPopup;

        public MainPage()
        {
            InitializeComponent();
            myPopup = new Popup() { IsOpen = true, Child = new ASplashScreen() };
            backroundWorker = new BackgroundWorker();
            RunBackroundWorker();
            isPageNew = true;
            Loaded += new RoutedEventHandler(MainPage_Loaded);
            TileUpdate();
        }

        //загрузка xml и парсинг
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PeriodicTask periodicTask = new PeriodicTask(Task_description)
        {
            Description = Task_description
        };
            try
            {
                ScheduledActionService.Add(periodicTask);
            }
            catch
            {
                // MessageBox.Show("Error during registration of Periodic task");
            }

            if (isPageNew)
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += new
                DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(new Uri(RSS));
                isPageNew = false;
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
                MessageBox.Show(error_network);
                NavigationService.GoBack();
            }
        }

        void ParseRSSAndBindData(string RSSText)
        {
            XElement rssnz = XElement.Parse(RSSText);
            XNamespace ns = "http://search.yahoo.com/mrss/";

            var nzpost =
                (from post in rssnz.Descendants("item")
                 select new PostMessage
                 {
                     title = post.Element("title").Value,
                     pubDate = DateTime.Parse(post.Element("pubDate").Value),
                     link = post.Element("link").Value
                 });

            RssAll.ItemsSource = nzpost;
            FlickrLoad();

        }



        public void FlickrLoad()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new
            DownloadStringCompletedEventHandler(flickr_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(FLICKR));
        }

        void flickr_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ParseflickrAndBindData(e.Result);
            }
        }

        void ParseflickrAndBindData(string RSSText)
        {
            XElement rssnz = XElement.Parse(RSSText);
            XNamespace ns = "http://search.yahoo.com/mrss/";

            RssFLK.ItemsSource = from post in rssnz.Descendants("item")
                                 select new PostMessageFL
                                 {
                                     ImageSource = post.Element(ns + "thumbnail").Attribute("url").Value,
                                     title = post.Element("title").Value,
                                     link = post.Element("link").Value,
                                     BigImage = post.Element(ns + "content").Attribute("url").Value
                                 };
        }
        //УРА!!!! Обращение к конкретному элементу RSSALL!!!!!  :)
        //void LoadRss()
        //{
        //    string nzrss = ((PostMessage)(RssAll.Items[0])).title;
        //    MessageBox.Show(nzrss);
        //}

        //открытие статьи. + передача параметров в read.xaml
        private void RssAll_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Read.xaml?link=" + ((PostMessage)(RssAll.SelectedItem)).link + "&title=" + ((PostMessage)(RssAll.SelectedItem)).title + "&date=" + ((PostMessage)(RssAll.SelectedItem)).pubDate, UriKind.Relative));
        }

        //обновление главного tile 
        public void TileUpdate()
        {
            var apptile = ShellTile.ActiveTiles.First();
            var appTileData = new StandardTileData();
            appTileData.Title = Tile_title;
            appTileData.BackgroundImage = new Uri("/stas-kulesh-app-icon.png", UriKind.RelativeOrAbsolute);
            apptile.Update(appTileData);
        }



        //обработка тапов по иконкам соцсетей

        private void google_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("https://plus.google.com/u/0/118010450346942959031/", UriKind.Absolute);
            cvtap.Show();
        }
        private void youtube_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("http://www.youtube.com/user/kulyesh", UriKind.Absolute);
            cvtap.Show();
        }
        private void facebook_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("http://facebook.com/kulesh", UriKind.Absolute);
            cvtap.Show();
        }
        private void twitter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("http://twitter.com/stas_kulesh", UriKind.Absolute);
            cvtap.Show();
        }
        private void linkedin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("http://nz.linkedin.com/in/staskulesh", UriKind.Absolute);
            cvtap.Show();
        }

        //Нажатие на staskulesh.com
        private void textBox6_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("http://staskulesh.com", UriKind.Absolute);
            cvtap.Show();
        }

        private void RssFLK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Photo.xaml?link=" + ((PostMessageFL)(RssFLK.SelectedItem)).link + "&BigImage=" + ((PostMessageFL)(RssFLK.SelectedItem)).BigImage, UriKind.Relative));
        }

        private void RunBackroundWorker()
        {
            backroundWorker.DoWork += ((s, args) =>
                {
                    Thread.Sleep(15000);
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
        private void textBox20_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask ectask = new EmailComposeTask();
            ectask.Subject = Email_subject;
            ectask.To = Email_to;
            ectask.Show();
        }
        //Кнопка - "Оценить"
        private void Rate_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask cvtap = new WebBrowserTask();
            cvtap.Uri = new Uri("http://www.windowsphone.com/ru-ru/store/app/here/6eb87263-c07e-407a-b8f8-f0b19e241ada", UriKind.Absolute);
            cvtap.Show();
        }




    }
}