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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1;

namespace SlidingPanel
{
    /// <summary>
    /// Interaction logic for Suprimento.xaml
    /// </summary>
    public partial class Suprimento : UserControl
    {
        int _supSan;
        MainWindow Form;
        private decimal _number = 0M;
        public decimal Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
        public Suprimento(MainWindow form, int SupSan)
        {
            _supSan =  0;
            Form = form;
            _supSan = SupSan;
            InitializeComponent();
            lbCaixa.Content = ClassGlobals.Global._Usuario.usr_id;
            lbDinheiro.Visibility = Visibility.Hidden;
            txtDinheiro.Visibility = Visibility.Hidden;
            this.Loaded += OnLoaded;
            btDinheiro.Focus();
            if (_supSan == 1)
            {
                lbSuprimento.Content = "Suprimento";
            }
            if (_supSan == 2)
            {
                lbSuprimento.Content = "Sangria";
            }
            Form.lbStatus.Content = "1 - Dinheiro 2- Cartão 3 - Cheque";
        }
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this.bt);

        }

        private void UserControl_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad1)
            {
                lbDinheiro.Content = "Dinheiro";
                lbDinheiro.Visibility = Visibility.Visible;
                txtDinheiro.Visibility = Visibility.Visible;
                txtDinheiro.Clear();
                txtDinheiro.Focus();
            }
            if (e.Key == Key.NumPad2)
            {
                lbDinheiro.Content = "Cartão";
                lbDinheiro.Visibility = Visibility.Visible;
                txtDinheiro.Visibility = Visibility.Visible;
                txtDinheiro.Clear();
                txtDinheiro.Focus();
            }
            if (e.Key == Key.NumPad3)
            {
                lbDinheiro.Content = "Cheque";
                lbDinheiro.Visibility = Visibility.Visible;
                txtDinheiro.Visibility = Visibility.Visible;
                txtDinheiro.Clear();
                txtDinheiro.Focus();
            }
            if (e.Key == Key.Enter)
            {
                if (txtDinheiro.Visibility == Visibility.Hidden || txtDinheiro.Text == string.Empty)
                {
                    Form.lbStatus.Content = "Digite um Valor.";

                }
                else
                {
                   

                    if (_supSan == 1)
                    {
                        Itb_suprimentosRepository supresp = new tb_suprimentosRepository();
                        int cont = supresp.GetAll().Count();
                        tb_suprimentos sup = new tb_suprimentos();
                        sup.uneg_id = Convert.ToInt32(ClassGlobals.Global.Term.uneg_id);
                        sup.suprimentof_pdv = ClassGlobals.Global.Term.te_id_terminal;
                        sup.suprimentof_data = DateTime.Now;
                        sup.suprimentof_id = (cont +1);
                        sup.suprimentof_operadorCx = ClassGlobals.Global._Usuario.usr_id;
                        sup.suprimentof_responsavel = Convert.ToInt32(lbCaixa.Content.ToString());
                        sup.suprimentof_status = "FN";
                        sup.suprimentof_valor = (Convert.ToDecimal(txtDinheiro.Text.Replace(",", "")) /100);
                        sup.final_id = 1;
                       
                        sup.suprimentof_observacao = "SUPRIMENTO";
                        supresp.Add(sup);
                        ClassGlobals.Impressora.SuprimentoSangria(lbSuprimento.Content.ToString(), lbCaixa.Content.ToString(), txtDinheiro.Text, lbDinheiro.Content.ToString());
                        Form.lbStatus.Content = "Suprimento cadastrado ";
                        Form.pnlPag.Visibility = Visibility.Hidden;
                        Form.tbCodPro.Focus();
                    }
                    if (_supSan == 2)
                    {
                        Itb_sangriasRepository sanresp = new tb_sangriasRepository();
                        int contsan = sanresp.GetAll().Count();
                        tb_sangrias san = new tb_sangrias();
                        san.uneg_id = Convert.ToInt32(ClassGlobals.Global.Term.uneg_id);
                        san.sangriafl_pdv = ClassGlobals.Global.Term.te_id_terminal;
                        san.sangriaf_data = DateTime.Now;
                        san.sangriaf_id = (contsan +1);
                        san.sangriaf_operadorCx = ClassGlobals.Global._Usuario.usr_id;
                        san.sangriaf_responsavel = Convert.ToInt32(lbCaixa.Content.ToString());
                        san.sangriaf_status = "FN";
                        san.sangriaf_valor = (Convert.ToDecimal(txtDinheiro.Text.Replace(",", "")) / 100);
                        san.final_id = 1;
                        san.sangriaf_observacao = "SANGRIA";
                        
                        sanresp.Add(san);
                        ClassGlobals.Impressora.SuprimentoSangria(lbSuprimento.Content.ToString(), lbCaixa.Content.ToString(), txtDinheiro.Text, lbDinheiro.Content.ToString());
                        Form.lbStatus.Content = "Sangria cadastrado ";
                        Form.pnlPag.Visibility = Visibility.Hidden;
                        Form.tbCodPro.Focus();
                    }

                }
            }
        }

        private void btDinheiro_Click(object sender, RoutedEventArgs e)
        {
            lbDinheiro.Content = "Dinheiro";
            lbDinheiro.Visibility = Visibility.Visible;
            txtDinheiro.Visibility = Visibility.Visible;
            txtDinheiro.Clear();
            txtDinheiro.Focus();
        }

        private void btCartao_Click(object sender, RoutedEventArgs e)
        {
            lbDinheiro.Content = "Cartão";
            lbDinheiro.Visibility = Visibility.Visible;
            txtDinheiro.Visibility = Visibility.Visible;
            txtDinheiro.Clear();
            txtDinheiro.Focus();
        }

        private void btCheque_Click(object sender, RoutedEventArgs e)
        {
            lbDinheiro.Content = "Cheque";
            lbDinheiro.Visibility = Visibility.Visible;
            txtDinheiro.Visibility = Visibility.Visible;
            txtDinheiro.Clear();
            txtDinheiro.Focus();
        }
        
    }
}
