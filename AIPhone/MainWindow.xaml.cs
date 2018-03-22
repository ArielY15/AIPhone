using System.Windows;

namespace AIPhone
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _mainFrame.Navigate(new OcrRec());
        }
       
    }
}
