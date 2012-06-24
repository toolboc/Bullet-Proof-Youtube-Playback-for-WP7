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
using Microsoft.SilverlightMediaFramework.Core;
using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
using Microsoft.SilverlightMediaFramework.Core.Media;
using System.Windows.Threading;
using System.IO;
using System.Windows.Navigation;

//Why and how?

//What we are doing is setting up an invisible browser control which can extract the mp4 cache url by interpretting
//javascript in YoutubeMp4Extractor.html.  Assuming the extractor is hosted and obtained remotely, we can 
//repair in app youtube video playback functionality through changes on the server side by updating YoutubeMP4Extractor.html
//should the means to parse a youtube mp4 cache url change!  Thus saving us time and frustration to recertify an app in the marketplace!
//
//This happens often enough that it's basically a timebomb if you bake a youtube parser into a mobile application.  I created
//this to give myself peace of mind that my apps could be bulletproofed against changes initiated by Youtube.
//
//Examples of youtube breaking parsers include:
//http://trac.videolan.org/vlc/ticket/4608,
//http://answers.yahoo.com/question/index?qid=20100403204443AAOk2EG,
//http://yazsoft.com/f/viewtopic.php?f=5&t=1230

//More how

//For playback we are using the Microsoft Media Platform from: http://smf.codeplex.com/
//For whatever reason, I had to recompile SMF from source with shorter assembly names, therefore, this method of playback
//is not guaranteed with the official SMF Player build.  You can read more info on this @ http://smf.codeplex.com/discussions/280487


namespace YoutubeInAppWP7.Views
{
    public partial class PlayerPage : PhoneApplicationPage
    {
        String VideoUrl;
        bool readyToPlay = false;
        Dispatcher Dispatcher = Deployment.Current.Dispatcher;

        public PlayerPage()
        {
            InitializeComponent();
        }

        protected override void  OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VideoUrl = NavigationContext.QueryString["VideoUrl"];
        } 

        //You could also name the player in xaml, it depends on the purpose of your application and reuse of
        //views / viewmodels for different types of data.  I.e. you may have a collection of mixed audio and
        //video sources, but you wish to play them in the same player view.  Video items would may be templated 
        //with the SMF player and to distinguish two videos, you would need to know which one's player loaded
        SMFPlayer currentPlayer;

        private void SMFPlayer_Loaded(object sender, RoutedEventArgs e)
        {
                currentPlayer = (Microsoft.SilverlightMediaFramework.Core.SMFPlayer)sender;

                if (!readyToPlay)
                    InitWebBrowser();
        }

        private void Browser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            // The script in our page has processed our request and has returned 
            // the result to us using window.external.Notify. 
            //
            // Take the result and add it to the SMFPlayer playlist as a PlaylistItem.
            try
            {
                
                //e.Value will contain the extracted mp4 cache url, if there is one
                if (e.Value != null)
                {
                    string sdUrl = e.Value;

                    var item = new PlaylistItem
                    {
                        //you can also do things like stretch etc.
                        MediaSource = new Uri(sdUrl, UriKind.Absolute),
                        DeliveryMethod = DeliveryMethods.ProgressiveDownload
                    };

                    //Assuming SMFPlayer is your SMFPlayer from the MMP pack

                    if (currentPlayer.Playlist.Count > 0)
                        Dispatcher.BeginInvoke(() => currentPlayer.Playlist.Clear());

                    Dispatcher.BeginInvoke(() => currentPlayer.Visibility = Visibility.Visible);

                    Dispatcher.BeginInvoke(() => currentPlayer.Playlist.Add(item));

                    //to make sure the damn thing fires up
                    Dispatcher.BeginInvoke(() => currentPlayer.GoToPlaylistItem(0));

                    Dispatcher.BeginInvoke(() => currentPlayer.Play());
                    Dispatcher.BeginInvoke(() => currentPlayer.IsControlStripVisible = false);

                }
                else
                {
                    MessageBox.Show("Error parsing video for mp4 cache url");
                }
            }
            catch
            {
                MessageBox.Show("Error adding / beginning playback of video in SMFPlayer ");
            }

        }

        private void Browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // The page content has been fully loaded and our JavaScript function 
            // is now available to be called. I'm passing in the VideoUrl 
            // obtained from the previously clicked listbox item
            //
            // Take a look in YoutubeMp4Extractor.html. You'll see that it takes the raw html of the Video Url,
            // applies a regex to extract the cached mp4 url, then calls window.external.Notify and passes 
            // back the cached mp4 url.
            //
            // window.external.Notify results in the Browser_ScriptNotify handler 
            // getting called below.


            readyToPlay = true;
    
            try{
                
                if (readyToPlay)
                {
                    var webClient = new WebClient();

                    webClient.DownloadStringCompleted += (s, t) => // delegate(object sender, OpenReadCompletedEventArgs e)
                    {
                        try
                        {
                            Browser.InvokeScript("GetYoutubeUrl", t.Result);
                        }
                        catch
                        {
                            MessageBox.Show("Error invoking mp4 extractor from hidden browser control");
                        }
                    };

                    webClient.DownloadStringAsync(new Uri(VideoUrl));
                }
            }
            catch { }
        }


        public void Browser_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                //The beauty of supplying the extractor this way, is that it that you could obtain the 
                //extractor from a remote location, thus inhibiting the ability for your app to suddenly
                //being unable to play content should a change arise in youtube's current format
                var rs = Application.GetResourceStream(new Uri("YoutubeMp4Extractor.html", UriKind.Relative));
                    using (StreamReader reader = new StreamReader(rs.Stream))
                    {    //Visualize the text data in a TextBlock text
                        Browser.NavigateToString(reader.ReadToEnd());
                    }
                
            }
            catch { }

        }

        WebBrowser Browser;   // this invisible browser will execute our javascript in YoutubeMp4Extractor.html
        public void InitWebBrowser()
        {
            try
            {

                Browser = new WebBrowser();   // init
                Browser.Visibility = Visibility.Collapsed;
                Browser.IsScriptEnabled = true;
                Browser.Loaded += new RoutedEventHandler(Browser_Loaded);    // add event handler
                Browser.LoadCompleted += new LoadCompletedEventHandler(Browser_LoadCompleted);
                Browser.ScriptNotify += new EventHandler<NotifyEventArgs>(Browser_ScriptNotify);
                LayoutRoot.Children.Add(Browser);
            }
            catch { }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {

            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                VisualStateManager.GoToState(this, "ReturnNormal", true);
            }


            else
            {
                VisualStateManager.GoToState(this, "PlayFullScreen", true);
            }

        }
    }
}