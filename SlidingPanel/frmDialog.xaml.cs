using ASArquiteruraData;
using ASArquiteruraData.Repository;
using ASArquiteruraData.RepositoryInterfaces;
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
    /// Interaction logic for frmDialog.xaml
    /// </summary>
    public partial class frmDialog : Window
    {
        ACBrFramework.BAL.ACBrBAL tre = new ACBrFramework.BAL.ACBrBAL();
        ACBrFramework.ECF.ACBrECF tres = new ACBrFramework.ECF.ACBrECF();
        Itb_terminalRepository _term = new tb_terminalRepository();
        UsrPag form;
        MainWindow frmWin;
        bool balEspera = false;
        public frmDialog(MainWindow frmW)
        {
            List<tb_terminal> ls = new List<tb_terminal>(_term.GetAll());
           
            frmWin = frmW;
            InitializeComponent();
            if (ls[0].te_acbr_bal_marca != null)
            { 
               TexBlockMsg.Text = "Aguardando Peso. "+Environment.NewLine+"posicione o produto na balança e tecle ENTER";
               balEspera = true;
               tre.Modelo = (ACBrFramework.BAL.ModeloBal)Enum.Parse(typeof(ACBrFramework.BAL.ModeloBal), ls[0].te_acbr_bal_marca);
               tre.Porta = ls[0].te_acbr_bal_porta;
               tre.Ativar();
               //decimal str = tre.LePeso();
            }
            if (ls[0].te_acbr_bal_marca == null)
            {
                this.Loaded += OnLoaded;
                TexBlockMsg.Text = "Digite o Peso.";
            }
        }
       
        public frmDialog(UsrPag frm)
        {
            

            InitializeComponent();
            form = frm;
            string[] tre = form.MensagemTela.Replace('\0',' ').Split(';');
            foreach (var item in tre)
	        {
		         TexBlockMsg.Text = TexBlockMsg.Text + item+Environment.NewLine;
	       }
            if (TexBlockMsg.Text.Contains("Transacao OK"))
            {
                TexBlockMsg.Text = "Transacao OK : Aperte ENTER";
                tbResp.Visibility = System.Windows.Visibility.Hidden;
            }
            this.Loaded += OnLoaded;
        }
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this.tbResp);

        }
        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            if (e.Key == Key.Enter)
            {
                if (form == null)
                {
                    frmWin.Peso = tbResp.Text;
                }
                if (frmWin == null)
                {
                    form.Resp = tbResp.Text;
                }
                if (balEspera && form == null)
                {
                    frmWin.Peso = tre.LePeso().ToString();
                }
                this.Close();
            }
        }


    }
}
