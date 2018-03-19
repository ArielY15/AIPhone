﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIPhone
{
    /// <summary>
    /// Interaction logic for SpeechPage.xaml
    /// </summary>
    public partial class SpeechPage : Page
    {
        private SpeechRecManager speechManager;

        private List<String> wordsToDetect;

        public SpeechPage()
        {
            InitializeComponent();

            speechManager = new SpeechRecManager();
            speechManager.PartialResponseReceived += TextArrived;
            speechManager.ResponseReceived += TextArrived;
            wordsToDetect = new List<string>();
            wordsToDetect.Add("pizza");
            wordsToDetect.Add("vodka");
            wordsToDetect.Add("rain");
            wordsToDetect.Add("electronics");
            wordsToDetect.Add("sunny");
        }

        private void TextArrived(string str)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (string word in wordsToDetect)
                {
                    if (str.Contains(word))
                    {
                        wordsToDetect.Remove(word);
                        if (word.Equals("pizza", StringComparison.InvariantCultureIgnoreCase))
                            paris.Visibility = Visibility.Hidden;
                        if (word.Equals("rain", StringComparison.InvariantCultureIgnoreCase))
                            seattle.Visibility = Visibility.Hidden;
                        if (word.Equals("sunny", StringComparison.InvariantCultureIgnoreCase))
                            telaviv.Visibility = Visibility.Hidden;
                        if (word.Equals("electronics", StringComparison.InvariantCultureIgnoreCase))
                            hongkong.Visibility = Visibility.Hidden;
                        if (word.Equals("vodka", StringComparison.InvariantCultureIgnoreCase))
                            moscow.Visibility = Visibility.Hidden;
                        break;
                    }
                }
                if (!wordsToDetect.Any())
                {
                    done();
                }
                SpeechResult.Content = str;
            }));
        }

        private void video_MediaEnded(object sender, RoutedEventArgs e)
        {
            video.Visibility = Visibility.Hidden;
            imageGrid.Visibility = Visibility.Visible;
            speechManager.Start();
        }

        private async void done()
        {
            InstructionLabel.Content = "Congratulations all missiles are disabled!";
            await Task.Delay(10000);

            NavigationService navService = NavigationService.GetNavigationService(this);

            OcrRec nextPage = new OcrRec();

            navService.Navigate(nextPage);
        }
    }
}
