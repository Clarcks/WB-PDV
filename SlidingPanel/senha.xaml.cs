using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApplication1;

namespace SlidingPanel
{
    /// <summary>
    /// Interaction logic for senha.xaml
    /// </summary>
    public partial class senha : Window
    {
        MainWindow Form;
        public senha(MainWindow form)
        {
            Form = form;
            InitializeComponent();


            this.Loaded += OnLoaded;

        }
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this.tbSenha);

        }

        private void btLogar_Click(object sender, RoutedEventArgs e)
        {
            Form.SenhaUser = tbSenha.Password;
            this.Close();
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Form.SenhaUser = tbSenha.Password;
                Form.tbCodPro.Focus();
                this.Close();
            }
            if (e.Key == Key.Escape)
            {
                Form.tbCodPro.Focus();
                this.Close();
            }
        }
    }
}
