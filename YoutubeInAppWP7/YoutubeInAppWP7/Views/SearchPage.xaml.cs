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
using YoutubeInAppWP7.ViewModels;
using YoutubeInAppWP7.Models;

namespace YoutubeInAppWP7
{
    public partial class SearchPage : PhoneApplicationPage
    {
        //SearchPageViewModel vm = new SearchPageViewModel();

        // Constructor
        public SearchPage()
        {
            //DataContext = vm;
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Instructions.Visibility = Visibility.Collapsed;
            ((SearchPageViewModel)DataContext).QueryYoutube();
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// If selected index is -1 (no selection) do nothing
			if (((ListBox)sender).SelectedIndex == -1)
				return;

            string VideoUrl = ((YoutubeFeedItem)((ListBox)sender).Items[((ListBox)sender).SelectedIndex]).VideoUrl;

			//Do Something
            NavigationService.Navigate(new Uri("/Views/PlayerPage.xaml?VideoUrl=" + VideoUrl, UriKind.Relative));
			
			// Reset selected index to -1 (no selection)
			((ListBox)sender).SelectedIndex = -1;
		}
    }
}