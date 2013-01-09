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
        StringConst Strcons = new StringConst();

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
                Read_View.Navigate(new Uri(NavigationContext.QueryString["link"].ToString(), UriKind.RelativeOrAbsolute));
                Read_View.LoadCompleted += new LoadCompletedEventHandler(Read_View_LoadCompleted);
            }
        }
        void Read_View_LoadCompleted(object sender, NavigationEventArgs e)
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
            StandardTileData newTile = new StandardTileData
            {
                Title = NavigationContext.QueryString["title"].ToString(),
                BackContent = Strcons.Read_Tile_title
            };
            try
            {
                ShellTile.Create(new Uri("/Read.xaml?link=" + NavigationContext.QueryString["link"].ToString(), UriKind.Relative), newTile);
            }
            catch (Exception)
            {
                MessageBox.Show(Strcons.Tile_error);
            }
        }

        //Шаринг
        private void Shared(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.Title = Strcons.Read_Tile_title;
            shareLinkTask.LinkUri = new Uri(NavigationContext.QueryString["link"].ToString());
            string messageshare = "@stas_kulesh " + NavigationContext.QueryString["title"].ToString();
            shareLinkTask.Message = messageshare;
            shareLinkTask.Show();
        }
    }
}