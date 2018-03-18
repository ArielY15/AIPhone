using System;
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

        public SpeechPage()
        {
            InitializeComponent();

            speechManager = new SpeechRecManager();
            speechManager.PartialResponseReceived += TextArrived;
            speechManager.ResponseReceived += TextArrived;
        }

        private void TextArrived(string str)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SpeechResult.Content = str;
            }));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            speechManager.Start();
        }
    }
}
