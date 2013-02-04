using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;

namespace Here
{
    public partial class Photo : PhoneApplicationPage
    {
        StringConst Strcons = new StringConst();

        public Photo()
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
                FhotoView.Navigate(new Uri(NavigationContext.QueryString["link"].ToString(), UriKind.RelativeOrAbsolute));
                FhotoView.LoadCompleted += new LoadCompletedEventHandler(FhotoView_LoadCompleted);
            }
        }
        void FhotoView_LoadCompleted(object sender, NavigationEventArgs e)
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
            pread.Visibility = Visibility.Collapsed;
            pread.IsIndeterminate = false;
        }

        private void SaveFoto(object sender, EventArgs e)
        {
            prbarstart();
            string DatePicker = NavigationContext.QueryString["BigImage"].ToString();
            WebClient client = new WebClient();
            client.OpenReadCompleted += WebClientOpenReadCompleted;
            client.OpenReadAsync(new Uri(DatePicker, UriKind.Absolute));
        }

        void WebClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            const string tempJpeg = "TempJPEG";
            var streamResourceInfo = new StreamResourceInfo(e.Result, null);
            var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
            if (userStoreForApplication.FileExists(tempJpeg))
            {
                userStoreForApplication.DeleteFile(tempJpeg);
            }
            var isolatedStorageFileStream = userStoreForApplication.CreateFile(tempJpeg);
            var bitmapImage = new BitmapImage { CreateOptions = BitmapCreateOptions.None };
            bitmapImage.SetSource(streamResourceInfo.Stream);
            var writeableBitmap = new WriteableBitmap(bitmapImage);
            writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
            isolatedStorageFileStream.Close();
            isolatedStorageFileStream = userStoreForApplication.OpenFile(tempJpeg, FileMode.Open, FileAccess.Read);
            var mediaLibarary = new MediaLibrary();
            mediaLibarary.SavePicture(string.Format("Here_Pic{0}.jpg", DateTime.Now), isolatedStorageFileStream);
            isolatedStorageFileStream.Close();
            prbarstop();
            MessageBox.Show(Strcons.photo_message);
        }

    }
}
