using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace HereTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        public const string RSS = "http://feeds.feedburner.com/kulesh?format=xml";
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            try
            {
                WebClient nzRSS = new WebClient();
                //nzRSS.DownloadStringCompleted += new DownloadStringCompletedEventHandler(nzRSS_DSC);
                nzRSS.DownloadStringAsync(new Uri(RSS));
                nzRSS.DownloadStringCompleted += new DownloadStringCompletedEventHandler(nzRSS_DSC);
            }
            catch
            {
                var tile = ShellTile.ActiveTiles.First();
                var apptile = new StandardTileData();
                apptile.Title = "Здесь в...";
                apptile.BackgroundImage = new Uri("/stas-kulesh-app-icon.png", UriKind.RelativeOrAbsolute);
                apptile.BackContent = " ";
                tile.Update(apptile);
            }
        }

        void nzRSS_DSC(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var rssData = (from rss in XElement.Parse(e.Result).Descendants("item")
                               select new PostMessage
                               {
                                   title = rss.Element("title").Value
                               }).ToArray().Reverse();
                foreach (var item in rssData)
                {
                    String nameData = item.title;
                    var tile = ShellTile.ActiveTiles.First();
                    var apptile = new StandardTileData();
                    apptile.Title = "Здесь в...";
                    apptile.BackgroundImage = new Uri("/stas-kulesh-app-icon.png", UriKind.RelativeOrAbsolute);
                    apptile.BackContent = nameData;
                    tile.Update(apptile);
                }
            }
            catch
            {
            }
            finally
            {
                //      ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(10));
                NotifyComplete();
            }
        }

    }
}