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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using NFe.Classes;
using NFe.Classes.Informacoes;
using NFe.Classes.Informacoes.Cobranca;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Classes.Informacoes.Observacoes;
using NFe.Classes.Informacoes.Pagamento;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;
using NFe.Classes.Servicos.Tipos;
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
using NFe.Servicos;
using NFe.Servicos.Retorno;
using NFe.Utils;
using NFe.Utils.Assinatura;
using NFe.Utils.NFe;
using RichTextBox = System.Windows.Controls.RichTextBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using WebBrowser = System.Windows.Controls.WebBrowser;
using System.Runtime.InteropServices;
using System.Xml;
using ASAsysFwSiTef;
using ASArquiteruraData.Repository;
using ASArquiteruraData.RepositoryInterfaces;
using ASArquiteruraData;
using ClassGlobals;
using System.Threading;
using System.Drawing.Printing;

namespace NFe.AppTeste
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : System.Windows.Controls.UserControl
    {
        private const string ArquivoConfiguracao = @"\configuracao.xml";
        private const string TituloErro = "Erro";
        private ConfiguracaoApp _configuracoes;
        private Classes.NFe _nfe;
        private readonly string _path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Itb_terminalRepository term = new tb_terminalRepository();
        Itb_unid_negocioRepository unidade = new tb_unid_negocioRepository();
        public class TANCA
        {
            #region TANCA SD-1000

            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string ConsultarSAT(int SESSAO);
          
            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string ConsultarStatusOperacional(int SESSAO, string CODIGO_ATIVACAO);


            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string ExtrairLogs(int SESSAO, string CODIGO_ATIVACAO);

            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string ConfigurarInterfaceDeRede(int SESSAO, string CODIGO_ATIVACAO, string DADOS_CONF);

            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string TrocarCodigoDeAtivacao(int SESSAO, string CODIGO_ATIVACAO, int OPCAO, string NOVO_CODIGO, string CONFIRMA_NOVO_CODIGO);

            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string CancelarUltimaVenda(int SESSAO, string CODIGO_ATIVACAO, string CHAVE, string DADOS_CANCELAMENTO);

            [return: MarshalAsAttribute(UnmanagedType.AnsiBStr)]
            [DllImport("SAT/Tanca/SAT.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
            public static extern string EnviarDadosVenda(int SESSAO, string CODIGO_ATIVACAO, string DADOS_VENDA);

            #endregion
        }
        public UserControl1()
        {
            InitializeComponent();
            CarregarConfiguracao();
            DataContext = _configuracoes;
            foreach (string impressora in PrinterSettings.InstalledPrinters)
            {
                cbImpPdv.Items.Add(impressora);
                cbImpCoz.Items.Add(impressora);
            }
            List<string> listSat = new System.Collections.Generic.List<string>();
            listSat.Add("COM1");
            listSat.Add("COM2");
            listSat.Add("COM3");
            listSat.Add("COM4");
            listSat.Add("COM5");
            listSat.Add("COM6");
            listSat.Add("COM7");
            listSat.Add("COM8");
            listSat.Add("COM9");
            listSat.Add("COM10");
            cbSat1.ItemsSource = listSat;
            cbSat.ItemsSource = Enum.GetNames(typeof(ACBrFramework.BAL.ModeloBal));
            List<tb_unid_negocio> lstU = new List<tb_unid_negocio>(unidade.GetAll());
             if(lstU[0].uneg_tef_IP != null)
             {
                 TxtIpTef.Text = lstU[0].uneg_tef_IP;
                 TxtIdLoja.Text = lstU[0].uneg_tef_idLoja;
             }
            
            List<tb_terminal> lst = new List<tb_terminal>(term.GetAll());
            if (lst[0].te_acbr_bal_marca != null)
            {
                cbSat.Text = lst[0].te_acbr_bal_marca;
                cbSat1.Text = lst[0].te_acbr_bal_porta;
            }
            if (lst[0].te_acbr_caminho_log != null)
            {
                cbImpCoz.Text = lst[0].te_acbr_caminho_log;
                
            }
            if (lst[0].te_caminho_servidor != null)
            {
                cbImpPdv.Text = lst[0].te_caminho_servidor;

            }
            TxtIpTef.Text = lst[0].tb_unid_negocio.uneg_tef_IP;
            TxtNumNFce.Text = lst[0].te_numero_nfce.ToString();
            TxtSerNFce.Text = lst[0].te_serie_nfce.ToString();
        }

        #region METODOS
      
        public static string Consulta()
        {
            int sessao = GerarCodigoNumerico(int.Parse(DateTime.Now.ToString("HHmmss")));
            string retorno = string.Empty;

            string resp = TANCA.ConsultarSAT(sessao);

            string[] _ret = resp.Split('|');

            if (_ret[1] == "08000")
            {
                retorno = "SAT EM OPERACAO.";
            }
            if (_ret[1] == "08098")
            {
                retorno = "SAT em processamento. Tente novamente.";
            }
            if (_ret[1] == "08099")
            {
                retorno = "Erro desconhecido.";
            }
            return retorno;
        }
        public static Int32 GerarCodigoNumerico(Int32 numeroNF)
        {
            string s;
            Int32 i, j, k;

            // Essa função gera um código numerico atravéz de calculos realizados sobre o parametro numero
            s = numeroNF.ToString("000000");
            for (i = 0; i < 6; ++i)
            {
                k = 0;
                for (j = 0; j < 6; ++j)
                    k += Convert.ToInt32(s[j]) * (j + 1);
                s = (k % 11).ToString().Trim() + s;
            }
            return Convert.ToInt32(s.Substring(0, 6));
        }
        private void SalvarConfiguracao()
        {
            try
            {
                _configuracoes.SalvarParaAqruivo(_path + ArquivoConfiguracao);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(string.Format("{0} \n\nDetalhes: {1}", ex.Message, ex.InnerException), "Erro",
                        MessageBoxButton.OK);


            }
        }

        private void CarregarConfiguracao()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            try
            {
                _configuracoes = !File.Exists(path + ArquivoConfiguracao)
                    ? new ConfiguracaoApp()
                    : FuncoesXml.ArquivoXmlParaClasse<ConfiguracaoApp>(path + ArquivoConfiguracao);
                if (_configuracoes.CfgServico.TimeOut == 0)
                    _configuracoes.CfgServico.TimeOut = 3000; //mínimo

                #region Carrega a logo no controle logoEmitente

             

                #endregion



            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }

        private void CarregaDadosCertificado()
        {
            try
            {
                var cert = CertificadoDigital.ObterDoRepositorio();
                TxtCertificado.Text = cert.SerialNumber;
                //TxtValidade.Text = "Validade: " + cert.GetExpirationDateString();
            }
            catch (Exception ex)
            {
                Funcoes.Mensagem(ex.Message, TituloErro, MessageBoxButton.OK);
            }
        }

        #endregion






        public void SITEF(string ip,string loja)
        {
            ASAsysFwSiTef.Sitef si = new Sitef(ip, loja.PadLeft(8,'0'), "SW" + Global.Term.te_id_terminal.ToString().PadLeft(6, '0'));


            int tre = si.ConfiguraIntSiTefInterativo(ip, loja.PadLeft(8, '0'), "SW" + Global.Term.te_id_terminal.ToString().PadLeft(6, '0'), 0);
            DateTime data = DateTime.Now;
            int tre2 = si.IniciaFuncaoSiTefInterativo(0, "10,00", Global.Term.te_numero_nfce.ToString(), data, Global._Usuario.usr_id.ToString(), "");
            List<string> str = new List<string>();
            bool imprime = false;
            int proximoComando = 0;
            int tipoCampo = 0;
            int tamanhoMinimo = 0;
            int tamanhoMaximo = 0;
            int CLIENTE = 0;
            int ESTA = 0;
            int tamanhoBuf = 1024;
            byte[] _buf = new byte[tamanhoBuf];
            int continuaNavegacao = 0;
            string mensagem = string.Empty;

            int tre3 = si.ContinuaFuncaoSiTefInterativo(ref proximoComando, ref tipoCampo, ref tamanhoMinimo, ref tamanhoMaximo, _buf, tamanhoBuf, continuaNavegacao);

            while (tre3 == 10000)
            {

                tre3 = si.ContinuaFuncaoSiTefInterativo(ref proximoComando, ref tipoCampo, ref tamanhoMinimo, ref tamanhoMaximo, _buf, tamanhoBuf, continuaNavegacao);

                mensagem = System.Text.Encoding.ASCII.GetString(_buf);
                string[] tress = mensagem.Replace('\0', ' ').Split(';');
                if (mensagem.Contains("onectando SiTef"))
                {
                    lbResult.Content = tress[0];
                }
                if(mensagem.Contains("Conectado"))
                {
                    lbResult.Content = tress[0];

                    List<tb_unid_negocio> lstTef = new List<tb_unid_negocio>(unidade.GetAll());
                    lstTef[0].uneg_tef_idLoja = loja.PadLeft(8, '0').Trim();
                    lstTef[0].uneg_tef_IP = ip.Trim();
                    unidade.AddAllList(lstTef, false);
                }

            }

        }




        private void BtnArquivoCertificado_Click(object sender, RoutedEventArgs e)
        {
            TxtArquivoCertificado.Text = Funcoes.BuscarArquivoCertificado();
        }

        private void btnCertificado_Click(object sender, RoutedEventArgs e)
        {
            CarregaDadosCertificado();
        }

        private void BtnDiretorioSchema_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new FolderBrowserDialog();
                dlg.ShowDialog();
                TxtDiretorioSchema.Text = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }

        private void BtnDiretorioXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new FolderBrowserDialog();
                dlg.ShowDialog();
                TxtDiretorioXml.Text = dlg.SelectedPath;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }

        private void btnRemoveLogo_Click(object sender, RoutedEventArgs e)
        {
            LogoEmitente.Source = null;
            
        }

        private void btnLogo_Click(object sender, RoutedEventArgs e)
        {
            var arquivo = Funcoes.BuscarImagem();
            if (string.IsNullOrEmpty(arquivo)) return;
            var imagem = System.Drawing.Image.FromFile(arquivo);
            LogoEmitente.Source = new BitmapImage(new Uri(arquivo));

          
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               SalvarConfiguracao();
               System.Windows.Forms.MessageBox.Show("Arquivo Salvo com sucesso.");
            }
            catch (System.Exception msg)
            {

                System.Windows.Forms.MessageBox.Show(msg.Message);
            }
            
        }

        private void CbxSalvarXml_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStatusServico_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                #region Status do serviço

                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoStatus = servicoNFe.NfeStatusServico();

                TrataRetorno(retornoStatus);

                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }
        internal void RetornoStr(RichTextBox richTextBox, string retornoXmlString)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText(retornoXmlString);
        }
        private void TrataRetorno(RetornoBasico retornoBasico)
        {
          //  EnvioStr(RtbEnvioStr, retornoBasico.EnvioStr);
           // RetornoStr(RtbEnvioStr, retornoBasico.RetornoStr);
            //RetornoXml(WebXmlRetorno, retornoBasico.RetornoStr);
            //RetornoCompletoStr(RtbRetornoCompletoStr, retornoBasico.RetornoCompletoStr);
            //RetornoDados(retornoBasico.Retorno, RtbDadosRetorno);
        }
        internal void EnvioStr(RichTextBox richTextBox, string envioStr)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText(envioStr);
        }
        private void BtnInutiliza_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnImportarXml_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnCriareEnviar2_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnConsultarReciboLote2_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnGerarNfe2_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnCriareEnviar3_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnConsultarReciboLote3_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnGerarNfe3_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnConsultaXml_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnConsultaChave_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnEnviaEpec_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnCartaCorrecao_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnCancelarNFe_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnConsultaEpec_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnAssina_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnValida_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnAdicionaNfeproc_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnCarregaXmlEnvia_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnConsultaCadastro_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnDownlodNfe_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnNfceDanfe_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnNfceDanfeOff_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void BtnAdminCsc_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            ACBrFramework.BAL.ACBrBAL balan = new ACBrFramework.BAL.ACBrBAL();
            balan.Modelo = (ACBrFramework.BAL.ModeloBal)Enum.Parse(typeof(ACBrFramework.BAL.ModeloBal), cbSat.Text);
            balan.Porta = cbSat1.Text;
            try
            {
                balan.Ativar();

                txtSat.AppendText(balan.LePeso().ToString());

                int pes = Convert.ToInt32(balan.LePeso());
                if (pes > 0)
                {
                    List<tb_terminal> ls = new List<tb_terminal>();
                    ls = term.GetAll().ToList();
                    ls[0].te_balanca = true;
                    ls[0].te_acbr_bal_marca = cbSat.Text;
                    ls[0].te_acbr_bal_porta = cbSat1.Text;
                    term.AddAllList(ls, false);
                }
                if (pes < 0)
                {
                    txtSat.AppendText("Balança "+ balan.Modelo.ToString()+" não Connectada ou com Problemas.");
                }
            }
            catch (System.Exception ex)
            {
                
                txtSat.AppendText(ex.Message);
            }
          
        }

        private void BtnTestaTef_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            lbResult.Content = "Conectando SiTef Aguarde...";
            Thread.Sleep(300);
            SITEF(TxtIpTef.Text, TxtIdLoja.Text);
        }

        private void BtnGravar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(cbImpCoz.Text != string.Empty)
            {
                List<tb_terminal> lstTerm = new List<tb_terminal>(term.GetAll());
                lstTerm[0].te_acbr_caminho_log = cbImpCoz.Text;
                lstTerm[0].te_caminho_servidor = cbImpPdv.Text;
                term.AddAllList(lstTerm, false);
                lbImpressora.Content = "Salvo com sucesso.";
            }
        }
    }
}
