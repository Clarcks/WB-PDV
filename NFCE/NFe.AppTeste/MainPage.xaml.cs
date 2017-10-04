using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NFe.AppTeste
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void _buttonOpenSidebar_Checked(object sender, RoutedEventArgs e)
        {
            Nav.Navigate(new System.Uri("UserControl1.xaml", UriKind.RelativeOrAbsolute)); 

        }

        private void Nav_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // ... Test for F5 key.
            //if (e.Key == Key.F5)
            //{
            //    this.Title = "You pressed F5";
            //}
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {

            //if (e.Key == Key.F5)
            //{

            //    _buttonOpenSidebar.Checked += _buttonOpenSidebar_Checked;

            //}
        }

    }
}
