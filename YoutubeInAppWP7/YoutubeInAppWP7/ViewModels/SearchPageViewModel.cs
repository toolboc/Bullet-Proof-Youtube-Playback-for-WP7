using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using YoutubeInAppWP7.Models;
using YoutubeInAppWP7.Parsers;
using System.Collections.ObjectModel;

namespace YoutubeInAppWP7.ViewModels
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {

        public SearchPageViewModel()
        {
            Results = new ObservableCollection<YoutubeFeedItem>();

            if (DesignerProperties.IsInDesignTool)
            {
                Results = new ObservableCollection<YoutubeFeedItem>()
                {
                    new YoutubeFeedItem()
                            {
                                Title = "Title",
                                Description = "Description",
                                PostedDate = DateTime.Now,
								ThumbnailUrl = "/SampleData/YoutubeImage.jpg"
                            }
                };
            }
        }

        //Properties
        private string searchText = "Youtube WP7";
        public string SearchText
        {
            get { return searchText; }
            set { 
                    if (searchText!= value)
                        searchText = value;
                
                    OnPropertyChanged("SearchText");
                
                }
        }

        private ObservableCollection<YoutubeFeedItem> results;
        public ObservableCollection<YoutubeFeedItem> Results
        {
            get { return results; }
            set
            {
                    if(results != value)
                    results = value;
                    OnPropertyChanged("Results");
            }
        }

        //Note that directly querying youtube's api does not require a developer key or token
        const string youTubeApiUri = @"http://gdata.youtube.com/feeds/api/videos?q={0}";
        public void QueryYoutube()
        {
            var webClient = new WebClient();

            webClient.DownloadStringCompleted += (s, t) => // delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    var entries = YoutubeFeedParser.Parse(t.Result);

                    Results.Clear();

                    foreach (var entry in entries)
                        Results.Add(entry);
                }
                catch(Exception e)
                {
                    //ignore errors
                    throw new Exception(e.Message);
                }
            };

            webClient.DownloadStringAsync(new Uri(string.Format(youTubeApiUri,searchText)));

            return;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
