using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Face.Contract;
using WebEye.Controls.Wpf;
using System.Threading;
using System.IO.Ports;
using System.Windows.Media.Animation;
using System.Drawing;
using System.Windows.Navigation;

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
