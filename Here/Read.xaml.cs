using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Here
{
    public partial class FRead : PhoneApplicationPage
    {

        string Tile_title = "Пост из \"Здесь в...\"";
        string Tile_error = "Такой тайл уже существует!";

        public FRead()
        {
            InitializeComponent();
        }
        //Переопределяем OnNavigatedTo для получения параметров и вызова WebBrowser
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("link"))
            {
                prbarstart();
                webBrowser1.Navigate(new Uri(NavigationContext.QueryString["link"].ToString(), UriKind.RelativeOrAbsolute));
                webBrowser1.LoadCompleted += new LoadCompletedEventHandler(webBrowser1_LoadCompleted);
            }
        }
        void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            prbarstop();
        }
        public void prbarstart()
        {
            pread.Visibility = Visibility;
            pread.IsIndeterminate = true;

        }
        public void prbarstop()
        {
            pread.Visibility = System.Windows.Visibility.Collapsed;
            pread.IsIndeterminate = false;
        }

        //Делаем тайл с сылкой на пост
        private void CreateTile(object sender, EventArgs e)
        {
            tiles();
        }
        //Метод делающий тайлы
        public void tiles()
        {
            StandardTileData newTile = new StandardTileData
             {
                 Title = NavigationContext.QueryString["title"].ToString(),
                 BackContent = Tile_title
             };
            try
            {
                ShellTile.Create(new Uri("/Read.xaml?link=" + NavigationContext.QueryString["link"].ToString(), UriKind.Relative), newTile);
            }
            catch (Exception)
            {
                MessageBox.Show(Tile_error);
            }
        }

        //Шаринг
        private void Shared(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = Tile_title;
            shareLinkTask.LinkUri = new Uri(NavigationContext.QueryString["link"].ToString());
            string messageshare = "@stas_kulesh " + NavigationContext.QueryString["title"].ToString();
            shareLinkTask.Message = messageshare;
            shareLinkTask.Show();
        }
    }
}