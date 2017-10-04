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
using System.Windows.Media.Animation;
using ASArquiteruraData;
using ASArquiteruraData.DataContextInterface;
using ASArquiteruraData.DataContext;
using ASArquiteruraData.RepositoryInterfaces;
using ASArquiteruraData.Repository;
using SlidingPanel.Classes;
using System.Reflection;
using ClassGlobals;
using SlidingPanel;
using System.Data;
using System.Threading;
using NFe.AppTeste;
using System.Windows.Threading;
using NFe.Utils;
using System.IO;
using NFe.Servicos;
using System.Configuration;
using NFe.Classes;
using System.Timers;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data.Common;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SenhaUser;
        private const string ArquivoConfiguracao = @"\configuracao.xml";
        public static string _path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string CaminhoArq = _path+ArquivoConfiguracao;
        public int Pan = 0;
        public decimal ValorTot = 0;
        public bool itemCancela = false;
        public bool inseredest = false;
        public bool Fechavenda = false;
        public bool AtendeMEsa = false;
        public string Peso = string.Empty;
        public DateTime _dtAbertura;
        private ConfiguracaoApp _configuracoes;
        private DispatcherTimer timerImageChange;
        private Image[] ImageControls;
        private List<ImageSource> Images = new List<ImageSource>();
        private static string[] ValidImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        private static string[] TransitionEffects = new[] { "Fade" };
        private string TransitionType, strImagePath = "";
        private int CurrentSourceIndex, CurrentCtrlIndex, EffectIndex = 0, IntervalTimer = 1;
        public bool BuscaProd ;
        public string BuscaProdCod = string.Empty;
        public int numMesa;
      
        public MainWindow()
        {
            InitializeComponent();

            System.Timers.Timer bTimer = new System.Timers.Timer();
            bTimer.Elapsed += new ElapsedEventHandler(BOnTimedEvent);
            bTimer.Interval = 300000;
            bTimer.Enabled = true;

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            //Itb_produtoRepository proResp = new tb_produtoRepository();
            //List<tb_produto> TestItems = proResp.GetAll().ToList();
           // MetroMessageBox.Show(this, "Your message here.", "Title Here", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
            Itb_promocaoRepository prom = new tb_promocaoRepository();
            List<tb_promocao> lstProm = new List<tb_promocao>(prom.GetAll());
            foreach (var item in lstProm)
            {
                if (item.prm_dt_fim <= DateTime.Now)
                {
                    item.prm_situacao = "I";
                }
            }
            prom.AddAllList(lstProm, false);
            if (Global._Usuario == null)
            {
                lbStatus.Content = "CAIXA FECHADO";
                lbInfo.Content = "Digite Seu Código para Abrir o caixa.";

            }
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;
            lbversao.Content = "Versão: " + version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString() + "." + version.Revision.ToString();
            if (File.Exists(CaminhoArq))
            {
                CarregaGlobal.ComponenteConfig(CaminhoArq);
            }
            if (!File.Exists(CaminhoArq))
            {
                MessageBox.Show("O PDV não esta Devidamente Configurado. você pode Configrar na opção 'F5' Configuração NFCe");
            }
                this.Loaded += OnLoaded;
                //Initialize Image control, Image directory path and Image timer.
                IntervalTimer = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalTime"]);
                strImagePath = ConfigurationManager.AppSettings["ImagePath"];
                ImageControls = new[] { myImage, myImage2 };

                LoadImageFolder(strImagePath);

                timerImageChange = new DispatcherTimer();
                timerImageChange.Interval = new TimeSpan(0, 0, IntervalTimer);
                timerImageChange.Tick += new EventHandler(timerImageChange_Tick);
                
                PlaySlideShow();
                timerImageChange.IsEnabled = true;
           
        }

        private void BOnTimedEvent(object sender, ElapsedEventArgs e)
        {
           
            System.Diagnostics.Process.Start(_path+"\\Screen\\RPS4.exe");

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(
              DispatcherPriority.Normal,
              (Action)(() => { lbData.Content = DateTime.Now.ToString(); }));
           
        }

        #region SLIDER
        private void LoadImageFolder(string folder)
        {
           
            var sw = System.Diagnostics.Stopwatch.StartNew();
            if (!System.IO.Path.IsPathRooted(folder))
                folder = System.IO.Path.Combine(Environment.CurrentDirectory, folder);
            if (!System.IO.Directory.Exists(folder))
            {
             
                return;
            }
            Random r = new Random();
            var sources = from file in new System.IO.DirectoryInfo(folder).GetFiles().AsParallel()
                          where ValidImageExtensions.Contains(file.Extension, StringComparer.InvariantCultureIgnoreCase)
                          orderby r.Next()
                          select CreateImageSource(file.FullName, true);
            Images.Clear();
            Images.AddRange(sources);
            sw.Stop();
            Console.WriteLine("Total time to load {0} images: {1}ms", Images.Count, sw.ElapsedMilliseconds);
        }

        private ImageSource CreateImageSource(string file, bool forcePreLoad)
        {
            if (forcePreLoad)
            {
                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(file, UriKind.Absolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                src.Freeze();
                return src;
            }
            else
            {
                var src = new BitmapImage(new Uri(file, UriKind.Absolute));
                src.Freeze();
                return src;
            }
        }

        private void timerImageChange_Tick(object sender, EventArgs e)
        {
            PlaySlideShow();
        }

        private void PlaySlideShow()
        {
            try
            {
                if (Images.Count == 0)
                    return;
                var oldCtrlIndex = CurrentCtrlIndex;
                CurrentCtrlIndex = (CurrentCtrlIndex + 1) % 2;
                CurrentSourceIndex = (CurrentSourceIndex + 1) % Images.Count;

                Image imgFadeOut = ImageControls[oldCtrlIndex];
                Image imgFadeIn = ImageControls[CurrentCtrlIndex];
                ImageSource newSource = Images[CurrentSourceIndex];
                imgFadeIn.Source = newSource;

                TransitionType = TransitionEffects[EffectIndex].ToString();

                Storyboard StboardFadeOut = (Resources[string.Format("{0}Out", TransitionType.ToString())] as Storyboard).Clone();
                StboardFadeOut.Begin(imgFadeOut);
                Storyboard StboardFadeIn = Resources[string.Format("{0}In", TransitionType.ToString())] as Storyboard;
                StboardFadeIn.Begin(imgFadeIn);
            }
            catch (Exception ex) { }
        }


        #endregion
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
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this.tbCodPro);
            
        }
        private void btnLeftMenuHide_Click(object sender, RoutedEventArgs e)
        {

            ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }

        private void btnLeftMenuShow_Click(object sender, RoutedEventArgs e)
        {
            lbStatus.Content = "Menu NFC-e";
            NFe.AppTeste.testeNFCe teste = new NFe.AppTeste.testeNFCe();
            FrameLeft.Navigate(teste);
            ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }


       




        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public bool verificaUser()
        {
            if (Global._Usuario == null)
            {
                lbStatus.Content = "CAIXA FECHADO";
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool VerificaString(string str)
        {
            char[] c = str.ToCharArray();
            char le = ' ';
            for (int cont = 0; cont < c.Length; cont++)
            {
                le = c[cont];
                if (char.IsLetter(le) || char.IsPunctuation(le))
                    return true;
            }
            return false;
        } 
        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {

            //if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    MessageBox.Show("CTRL + A Pressed!");
            //}

           
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) | (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) | (e.Key == Key.Enter | e.Key == Key.F1 | e.Key == Key.F2 | e.Key == Key.F3 | e.Key == Key.F4 | e.Key == Key.F5 | e.Key == Key.F6 | e.Key == Key.F7 | e.Key == Key.F8 | e.Key == Key.F9 | e.Key == Key.F10 | e.Key == Key.F11 | e.Key == Key.F12 | e.Key == Key.Escape | e.Key == Key.LeftCtrl))
            {

                //KeyConverter kv = new KeyConverter();
                //if ((char.IsNumber((string)kv.ConvertTo(e.Key, typeof(string)), 0) == false))
                //{
                //    e.Handled = true;
                //}

                if (e.Key == Key.Enter && ver(tbCodPro.Text))
                {
                    if (Global._VlrDescNFceBool)
                    {
                        if (tbCodPro.Text == "")
                        {

                        }
                        else
                        {
                            decimal desc = Convert.ToDecimal(tbCodPro.Text);


                            Global._VlrDescNFce = Convert.ToDecimal(tbCodPro.Text);
                            //NFCe.lstProd
                            decimal Vdesc = Valor.Arredondar(Global._VlrDescNFce / NFCe.lstProd.Count(), 2);
                            foreach (var item in NFCe.lstProd)
                            {
                                item.vDesc = Vdesc;
                            }

                            lbStatus.Content = "Desconto de " + String.Format("{0:C}", Global._VlrDescNFce) + " aplicado";
                            //tbVlrTotal.Content = 
                            tbVlrTotal.Content = String.Format("{0:C}", ValorTot - Global._VlrDescNFce);
                            tbCodPro.Clear();
                            RodapeDesconto(String.Format("{0:C}", Global._VlrDescNFce));
                        }
                    }
                    if (Global._Usuario == null)
                    {
                        Itb_usuarioRepository UsuarioResp = new tb_usuarioRepository();

                        Int32 teste = Convert.ToInt32(tbCodPro.Text.Trim());
                        tb_usuario user = new tb_usuario();
                        try
                        {
                            user = UsuarioResp.First(s => s.usr_id.Equals(teste));
                        }
                        catch
                        {
                            tbCodPro.Clear();
                            lbStatus.Content = "Úsuario nao encontrado.";
                            e.Handled = true;
                        }

                        if (user == null)
                        {
                            lbStatus.Content = "Úsuario não encontrado!";
                            tbCodPro.Clear();
                        }
                        if (user.usr_nome != null)
                        {
                            senha testDialog = new senha(this);

                            // Show testDialog as a modal dialog and determine if DialogResult = OK.
                            testDialog.ShowDialog();



                            if (SenhaUser != user.usr_senha_local)
                            {
                                lbStatus.Content = "Senha não Confere!";
                                tbCodPro.Clear();
                            }
                            if (SenhaUser == user.usr_senha_local && !File.Exists(CaminhoArq))
                            {
                                #region MENU NFCE
                                Pan = 1;
                                this.Title = "Menu NFC-e";
                                NFe.AppTeste.UserControl1 teste2 = new NFe.AppTeste.UserControl1();
                                FrameLeft.Navigate(teste2);
                                ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
                                #endregion

                            }
                            if (SenhaUser == user.usr_senha_local && File.Exists(CaminhoArq))
                            {


                                Global._Usuario = new tb_usuario();
                                Global._Usuario = user;
                                Global._UsuarioFuncao = user.tb_usuario_funcao;
                                tbCodPro.Clear();
                                lbStatus.Content = "CAIXA ABERTO";
                                lbUser.Content = "Úsuario: " + Global._Usuario.usr_nome.ToUpper() + " - " + Global.Term.te_nome;
                                lbInfo.Content = "Código do Produto";
                                SenhaUser = string.Empty;
                                RodapeVenda(Global.Term.te_numero_nfce.ToString());
                                aberturaCaixa(false);
                            }
                        }


                    }
                }
           
                if (e.Key == Key.F5)
                {
                    Global._UserAutorizador = null;
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else
                    {
                        if (Global._UsuarioFuncao.funcao_menu_gerencial == true)
                        {

                            #region MENU NFCE
                            Pan = 1;
                            this.Title = "Configuração NFC-e";
                            NFe.AppTeste.UserControl1 rs = new NFe.AppTeste.UserControl1();
                            FrameLeft.Navigate(rs);
                            ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
                            #endregion
                        }
                        if (Global._UsuarioFuncao.funcao_menu_gerencial == false)
                        {

                            senha frmSenha = new senha(this);
                            frmSenha.ShowDialog();
                            try
                            {
                                Global._UserAutorizador = Global._Autorizador.First(s => s.usr_senha_local.Equals(SenhaUser));
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_menu_gerencial == true)
                                {
                                    #region MENU NFCE
                                    Pan = 1;
                                    this.Title = "Configuração NFC-e";
                                    NFe.AppTeste.UserControl1 rs = new NFe.AppTeste.UserControl1();
                                    FrameLeft.Navigate(rs);
                                    ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
                                    #endregion
                                }
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_menu_gerencial == false)
                                {
                                    lbStatus.Content = "Úsuario " + Global._UserAutorizador.usr_id.ToString() + " Sem Permissão";
                                }
                            }
                            catch
                            {

                                lbStatus.Content = "Úsuario não Identificado";
                            }





                        }

                    }
                }
                if (e.Key == Key.F3)
                {
                    if (!Fechavenda)
                    {
                        #region SANGRIA
                        if (Global._Usuario == null)
                        {
                            lbStatus.Content = "CAIXA FECHADO";

                        }
                        else
                        {
                            if (Global._UsuarioFuncao.funcao_sangria == true)
                            {

                                #region SANGRIA SUPRIMENTO
                                Pan = 2;
                                this.Title = "Sangria | Suprimento";
                                pnlPag.Visibility = Visibility.Visible;
                                Suprimento frmPg = new Suprimento(this, 1);
                                FramePag.Navigate(frmPg);
                                #endregion
                            }
                            if (Global._UsuarioFuncao.funcao_sangria == false)
                            {

                                senha frmSenha = new senha(this);
                                frmSenha.ShowDialog();
                                try
                                {
                                    Global._UserAutorizador = Global._Autorizador.First(s => s.usr_senha_local.Equals(SenhaUser));
                                    if (Global._UserAutorizador.tb_usuario_funcao.funcao_sangria == true)
                                    {
                                        #region SANGRIA SUPRIMENTO
                                        Pan = 2;
                                        this.Title = "Sangria | Suprimento";
                                        pnlPag.Visibility = Visibility.Visible;
                                        Suprimento frmPg = new Suprimento(this, 1);
                                        FramePag.Navigate(frmPg);
                                        #endregion
                                    }
                                    if (Global._UserAutorizador.tb_usuario_funcao.funcao_sangria == false)
                                    {
                                        lbStatus.Content = "Úsuario " + Global._UserAutorizador.usr_id.ToString() + " Sem Permissão";
                                    }
                                }
                                catch
                                {

                                    lbStatus.Content = "Úsuario não Identificado";
                                }





                            }

                        }
                        #endregion
                    }
                }
                if (e.Key == Key.F12)
                {
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else
                    {
                        if (Global._UsuarioFuncao.funcao_sangria == true)
                        {

                            #region SANGRIA SUPRIMENTO
                            Pan = 2;
                            this.Title = "Sangria | Suprimento";
                            pnlPag.Visibility = Visibility.Visible;
                            Suprimento frmPg = new Suprimento(this, 2);
                            FramePag.Navigate(frmPg);
                            #endregion
                        }
                        if (Global._UsuarioFuncao.funcao_sangria == false)
                        {

                            senha frmSenha = new senha(this);
                            frmSenha.ShowDialog();
                            try
                            {
                                Global._UserAutorizador = Global._Autorizador.First(s => s.usr_senha_local.Equals(SenhaUser));
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_sangria == true)
                                {
                                    #region SANGRIA SUPRIMENTO
                                    Pan = 2;
                                    this.Title = "Sangria | Suprimento";
                                    pnlPag.Visibility = Visibility.Visible;
                                    Suprimento frmPg = new Suprimento(this, 2);
                                    FramePag.Navigate(frmPg);
                                    #endregion
                                }
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_sangria == false)
                                {
                                    lbStatus.Content = "Úsuario " + Global._UserAutorizador.usr_id.ToString() + " Sem Permissão";
                                }
                            }
                            catch
                            {

                                lbStatus.Content = "Úsuario não Identificado";
                            }





                        }

                    }
                }
                if (e.Key == Key.F2)
                {
                    if (!Fechavenda)
                    {
                        #region FECHAMENTO DE VENDA
                        if (NFCe.lstProd.Count == 0)
                        {
                            lbStatus.Content = "Não existem Vendas.";
                        }
                        if (NFCe.lstProd.Count > 0)
                        {
                            Pan = 2;
                            ListNota.Visibility = Visibility.Hidden;
                            this.Title = "Pagamento";
                            pnlPag.Visibility = Visibility.Visible;
                            UsrPag frmPg = new UsrPag(this);
                            FramePag.Navigate(frmPg);
                            // ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
                        }
                        #endregion
                    }
                }
                if (e.Key == Key.F6)
                {
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else
                    {
                        #region CONFIGRACAO NFCE
                        Global._VlrDescNFceBool = true;
                        //Pan = 1;
                        //this.Title = "Configuração NFC-e";
                        //NFe.AppTeste.UserControl1 rs = new NFe.AppTeste.UserControl1();
                        //FrameLeft.Navigate(rs);
                        //ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);

                        lbStatus.Content = "Digite o valor do desconto";


                        #endregion
                    }
                }
                if (e.Key == Key.Escape)
                {
                   
                    pnlCliente.Visibility = Visibility.Visible;
                    DataGri.Visibility = Visibility.Hidden;
                    BuscaProd = false;
                    tbCodPro.Clear();
                    Keyboard.Focus(tbCodPro);
                    #region VOLTA TELA
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    if (Global._Usuario == null && !File.Exists(CaminhoArq))
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    if (Global._Usuario == null && File.Exists(CaminhoArq) && Pan == 1)
                    {
                        CarregaGlobal.ComponenteConfig(CaminhoArq);
                        lbStatus.Content = "CAIXA FECHADO";
                        ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
                        Pan = 0;

                    }
                    else
                    {
                        #region TELA INICAL
                        this.Title = "Pagina Principal";
                        if (AtendeMEsa)
                        {
                            NFCe.lstProd.Clear();
                            ListNota.Visibility = Visibility.Hidden;
                            lbMesa.Content = "F8 - Para Excluir Item.";
                            tbVlrProd.Content = "0,00";
                            tbVlrTotal.Content = "0,00";
                            ValorTot = 0;
                            lbStatus.Content = "CAIXA ABERTO";
                            AtendeMEsa = false;
                            LimparodaPe();
                        }
                        if (Pan == 1)
                        {
                            ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
                            Pan = 0;
                        }
                        if (Pan == 2)
                        {
                            pnlPag.Visibility = Visibility.Hidden;
                            if (NFCe.lstProd.Count() > 0)
                            {
                                ListNota.Visibility = Visibility.Visible;
                            }
                            lbStatus.Content = "CAIXA ABERTO";
                            Pan = 0;
                            Fechavenda = false;
                        }
                      

                        #endregion
                    }
                    #endregion
                }
                if (e.Key == Key.F1)
                {
                    if (!Fechavenda)
                    {
                        #region FEICHA CAIXA
                        if (Global._Usuario == null)
                        {
                            lbStatus.Content = "CAIXA FECHADO";
                            if (MessageBox.Show("Deseja Fechar o Programa", "Sair do Sistema", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                            {
                                Application.Current.Shutdown();
                            }
                        }
                        else
                        {
                            if (Pan == 0)
                            {
                                lbStatus.Content = "SAIR DO CAIXA";
                                // var senha = Funcoes.InpuBox("Sair do caixa","Digite sua senha para sair do Caixa");

                                senha testDialog = new senha(this);

                                // Show testDialog as a modal dialog and determine if DialogResult = OK.
                                testDialog.ShowDialog();
                                if (SenhaUser != Global._Usuario.usr_senha_local)
                                {
                                    lbStatus.Content = "Senha não Confere!";
                                }
                                else
                                {
                                    Impressora.imprimeFechamento();

                                    aberturaCaixa(true);
                                    Global._Usuario = null;

                                    lbStatus.Content = "CAIXA FECHADO";
                                    lbInfo.Content = "Digite Seu Código para Abrir o caixa.";
                                    lbUser.Content = "Úsuario: úsario não logado.";
                                    SenhaUser = string.Empty;

                                }
                            }
                        }
                        #endregion
                    }
                }
                if (e.Key == Key.F8)
                {
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else
                    {
                        if (Global._UsuarioFuncao.funcao_cancela_vendaItem == true)
                        {

                            #region CANCELAMENTO DE PRODUTO
                            if (NFCe.lstProd.Count == 0)
                            {
                                lbStatus.Content = "Não existe produtos para excluir.";
                            }
                            if (NFCe.lstProd.Count > 1)
                            {
                                itemCancela = true;
                                lbStatus.Content = "Código a ser Cancelado";
                            }
                            #endregion
                        }
                        if (Global._UsuarioFuncao.funcao_cancela_vendaItem == false)
                        {

                            senha frmSenha = new senha(this);
                            frmSenha.ShowDialog();
                            try
                            {
                                Global._UserAutorizador = Global._Autorizador.First(s => s.usr_senha_local.Equals(SenhaUser));
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_cancela_vendaItem == true)
                                {
                                    #region CANCELAMENTO DE PRODUTO
                                    if (NFCe.lstProd.Count == 0)
                                    {
                                        lbStatus.Content = "Não existe produtos para excluir.";
                                    }
                                    if (NFCe.lstProd.Count > 1)
                                    {
                                        itemCancela = true;
                                        lbStatus.Content = "Código a ser Cancelado";
                                    }
                                    #endregion
                                }
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_cancela_vendaItem == false)
                                {
                                    lbStatus.Content = "Úsuario " + Global._UserAutorizador.usr_id.ToString() + " Sem Permissão";
                                }
                            }
                            catch
                            {

                                lbStatus.Content = "Úsuario não Identificado";
                            }





                        }

                    }
                }
                if (e.Key == Key.F11)
                {
                    if (AtendeMEsa)
                    {
                        string msg = string.Empty;
                        Itb_venda_prevendaRepository Abert = new tb_venda_prevendaRepository();
                        if (Abert.Find(x => x.vendaPv_situacao == "RC").Count() > 0)
                        {
                            foreach (var item in Abert.Find(x => x.vendaPv_situacao == "RC"))
                            {
                                if (item.vendaPv_situacao == "RC")
                                {
                                    msg = msg + " -- {" + item.vendaPv_num_comanda + "} ";
                                }
                            }
                        }
                        else
                        {
                            msg = "Não Existem Mesas Abertas Neste PDV.";
                        }
                        lbStatus.Content = msg;
                    }
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else if(!AtendeMEsa)
                    {
                        #region INSERIR USUARIO
                        var cpf = Funcoes.InpuBox("CPF do Destinatario", "Digite o CPF:");
                        if (cpf == string.Empty)
                        {
                            lbStatus.Content = "o CPF deve ser Informado!";
                        }
                        if (cpf.Length != 11)
                        {
                            lbStatus.Content = "o CPF precisa ter 11 digitos!";
                        }
                        else
                        {
                            ClassGlobals.Global._dest.CPF = cpf;
                        }

                        var nome = Funcoes.InpuBox("Nome do Destinatario", "Digite o Nome:");

                        ClassGlobals.Global._dest.xNome = nome;
                        RodapeClienete(cpf);
                        #endregion
                    }
                }
                if (e.Key == Key.F7)
                {
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else
                    {
                        if (AtendeMEsa)
                        {
                            lbStatus.Content = "CANCELAMENTO DE MESA {"+numMesa.ToString()+"}";
                            if (MessageBox.Show("Deseja Cancelar a Mesa {" + numMesa.ToString() + "} ?", "Cancelamento Mesa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            {
                                lbStatus.Content = "Aguardando Produto.";
                            }
                            else
                            {
                                Itb_venda_prevendaRepository vv = new tb_venda_prevendaRepository();
                                Itb_venda_prevenda_itemRepository vvIt = new tb_venda_prevenda_itemRepository();

                                tb_venda_prevenda mes = new tb_venda_prevenda();
                                mes = vv.First(x => x.vendaPv_num_comanda == numMesa.ToString());
                                mes.vendaPv_situacao = "IM";
                                List<tb_venda_prevenda_item> lstIt = new List<tb_venda_prevenda_item>(vvIt.Find(x => x.venda_id.Equals(mes.vendaPv_id)));
                                foreach (var item in lstIt)
                                {
                                    item.vendaPv_item_status = "IM";
                                }
                                List<tb_venda_prevenda> ll = new List<tb_venda_prevenda>();
                                ll.Add(mes);
                                vv.AddAllList(ll, false);
                                vvIt.AddAllList(lstIt, false);
                                AtendeMEsa = false;

                            }
                        }
                        if (Global._UsuarioFuncao.funcao_cancela_venda == true)
                        {

                            #region CANLA VENDA
                           
                            if (NFCe.lstProd.Count == 0)
                            {
                                lbStatus.Content = "Não existe venda aberta.";
                            }
                            if (NFCe.lstProd.Count >= 1)
                            {
                                lbStatus.Content = "CANCELAMENTO DE NOTA";
                                if (MessageBox.Show("Deseja Cancelar a venda", "Cancelamento", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                                {
                                    lbStatus.Content = "Aguardando Produto.";
                                }
                                else
                                {
                                    NFCe.lstProd.Clear();
                                    ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) });
                                    lbStatus.Content = "Venda Cancelada.";
                                    ValorTot = 0;
                                    tbVlrTotal.Content = String.Format("{0:C}", ValorTot - Global._VlrDescNFce);
                                    tbVlrProd.Content = String.Format("{0:C}", "0,00");
                                    Thread.Sleep(400);
                                    lbStatus.Content = "Caixa Aberto.";
                                    LimparodaPe();
                                }

                            }
                            #endregion
                        }
                        if (Global._UsuarioFuncao.funcao_cancela_venda == false)
                        {

                            senha frmSenha = new senha(this);
                            frmSenha.ShowDialog();
                            try
                            {
                                Global._UserAutorizador = Global._Autorizador.First(s => s.usr_senha_local.Equals(SenhaUser));
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_cancela_venda == true)
                                {
                                    #region CANLA VENDA
                                    if (NFCe.lstProd.Count == 0)
                                    {
                                        lbStatus.Content = "Não existe venda aberta.";
                                    }
                                    if (NFCe.lstProd.Count > 1)
                                    {
                                        lbStatus.Content = "CANCELAMENTO DE NOTA";
                                        if (MessageBox.Show("Deseja Cancelar a venda", "Cancelamento", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                                        {
                                            lbStatus.Content = "Aguardando Produto.";
                                        }
                                        else
                                        {
                                            NFCe.lstProd.Clear();
                                            ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) });
                                            lbStatus.Content = "Venda Cancelada.";
                                            ValorTot = 0;
                                            tbVlrTotal.Content = String.Format("{0:C}", ValorTot - Global._VlrDescNFce);
                                            tbVlrProd.Content = String.Format("{0:C}", "0,00");
                                            Thread.Sleep(400);
                                            lbStatus.Content = "Caixa Aberto.";
                                            LimparodaPe();
                                        }

                                    }
                                    #endregion
                                }
                                if (Global._UserAutorizador.tb_usuario_funcao.funcao_cancela_venda == false)
                                {
                                    lbStatus.Content = "Úsuario " + Global._UserAutorizador.usr_id.ToString() + " Sem Permissão";
                                }
                            }
                            catch
                            {

                                lbStatus.Content = "Úsuario não Identificado";
                            }





                        }

                    }

                }
                if (e.Key == Key.F9)
                {
                    if (Global._Usuario == null)
                    {
                        lbStatus.Content = "CAIXA FECHADO";

                    }
                    else
                    {
                        //if (File.Exists(ArquivoConfiguracao))
                        //{
                        //    CarregarConfiguracao();
                        //}
                        CarregarConfiguracao();
                        var nome = Funcoes.InpuBox("Número da Venda", "Digite o Número:");
                        var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                        tb_venda venda = new tb_venda();
                        venda = Global.vendaResp.First(s => s.venda_num_cupom == Convert.ToInt32(nome));
                        if (venda.venda_nfce_protocolo.Contains("CONTIGENCIA"))
                        {
                            lbStatus.Content = "NFCe Ainda em Contigencia";
                            venda.venda_status = "CN";
                            List<tb_venda> lstV = new List<tb_venda>();
                            lstV.Add(venda);
                            Global.vendaResp.AddAllList(lstV, false);
                            Impressora.ImprimiCancelamento(venda.venda_nfce_chave, venda.venda_tot_valor.ToString(), venda.venda_num_cupom.ToString(), Global._Usuario.usr_id.ToString());

                        }
                        else
                        {
                            var retornoEnvio = servicoNFe.RecepcaoEventoCancelamento(112, 1, venda.venda_nfce_protocolo, venda.venda_nfce_chave, "CANCELAMENTO DE NFCE", _configuracoes.Emitente.CNPJ);
                            if (retornoEnvio.Retorno.retEvento[0].infEvento.cStat == 135)
                            {
                                lbStatus.Content = "Venda Nº " + venda.venda_num_cupom.ToString() + " Cancelada.";
                                venda.venda_status = "CN";
                                List<tb_venda> lstV = new List<tb_venda>();
                                lstV.Add(venda);
                                Global.vendaResp.AddAllList(lstV, false);
                                Impressora.ImprimiCancelamento(venda.venda_nfce_chave, venda.venda_tot_valor.ToString(), venda.venda_num_cupom.ToString(), Global._aCaixa.aberturaCx_usr_id_operador.ToString());
                            }
                            else
                            {
                                lbStatus.Content = retornoEnvio.Retorno.retEvento[0].infEvento.xMotivo;
                            }

                        }
                    }
                }
                if (e.Key == Key.F4)
                {
                    if (!Fechavenda)
                    {
                        #region IMPRIMI DANFE
                        if (Global._Usuario == null)
                        {
                            lbStatus.Content = "CAIXA FECHADO";

                        }
                        else if(!AtendeMEsa)
                        {
                            if (NFCe.lstProd.Count > 1)
                            {
                                lbStatus.Content = "Existe venda em Aberto neste PDV.";
                            }
                            if (NFCe.lstProd.Count == 0)
                            {
                                var nome = Funcoes.InpuBox("Número do Orçamento", "Digite o Número do Orçamento:");

                                //tb_venda venda = new tb_venda();
                                //venda = Global.vendaResp.First(s => s.venda_num_cupom == Convert.ToInt32(nome));
                                //if (venda.venda_nfce_protocolo.Contains("CONTIGENCIA"))
                                //{
                                //    var novoArquivo = @"C:\\teste\\CtgsNFe" + venda.venda_nfce_chave +
                                //                 ".xml";
                                //    Impressora.ImprimirDanferE(novoArquivo);
                                //}
                                //else
                                //{
                                //    var novoArquivo = _path + "\\Autorizados\\" + @"\" + venda.venda_nfce_chave +
                                //                  "-procNfe.xml";
                                //    Impressora.ImprimirDanferE(novoArquivo);
                                //}

                                preVenda(nome);
                            }
                        }
                        else if (AtendeMEsa)
                        { 
                            string impcoz = string.Empty;
                            foreach (var item in NFCe.lstProd.GroupBy(x=>x.descricao))
	                        {
                               impcoz = impcoz + Environment.NewLine +"Desc--"+ item.First().descricao +" --Qtd:"+ item.Count();
	                        }

                            Impressora.ImprimiCozinha(impcoz,lbCliente.Content.ToString());
                            impcoz = string.Empty;
                        }
                    }
                        #endregion
                }
            }
            if (e.Key == Key.Enter && BuscaProd == false && Global._Usuario != null)
            {
                //e.Handled = true;
                if (tbCodPro.Text != string.Empty)
                {
                    Itb_produtoRepository pro = new tb_produtoRepository();

                    DataGri.Items.Clear();
                    foreach (var item in pro.Find(s => s.pro_descricao.StartsWith(tbCodPro.Text)))
                    {
                        DataGri.Items.Add(item.pro_descricao);
                    }
                    DataGri.Visibility = System.Windows.Visibility.Visible;
                    DataGri.SelectedItem = 0;
                    BuscaProd = true;
                    Keyboard.Focus(DataGri);
                    pnlCliente.Visibility = Visibility.Hidden;
                }
            }
            
        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        private void tbCodPro_TextInput(object sender, TextCompositionEventArgs e)
        {
           
        }
        public void preVenda(string numero)
        {
            Itb_venda_prevendaRepository vendaResp = new tb_venda_prevendaRepository();
            Itb_venda_prevenda_itemRepository vendaItemResp = new tb_venda_prevenda_itemRepository();

          
            var SAG =
    from s in vendaResp.GetAll()
    from g in vendaItemResp.GetAll()

    where g.venda_id == Convert.ToInt32(numero)
    where s.venda_id == Convert.ToInt32(numero)
  
    where s.vendaPv_situacao == "ET"
    select new
    {
        s.venda_data,
        s.venda_id,

        s.cli_id,

        g.vendaPv_item_descricao,
        g.vendaPv_item_ean13,

        g.tb_produto.tbr_produto_uneg.First(t => t.pro_id.Equals(g.pro_id)).proun_preco_venda,
        g.vendaPv_item_fpop_qtde_Unitaria,
        g.vendaPv_item_totalValor,
        s.tb_cliente
       

    };
            if (SAG.Count() > 0)
            {
                #region PRE VENDA
                foreach (var item in SAG)
                {
                    lbStatus.FontSize = 25;
                    var Barra = Global.lstBarra.Find(y => y.barra_codigo == Convert.ToInt64(item.vendaPv_item_ean13));
                    lbStatus.Content = Barra.tb_produto.pro_descricao;
                    List<tbr_produto_uneg> valor = new List<tbr_produto_uneg>(Barra.tb_produto.tbr_produto_uneg);
                    tbVlrProd.Content = String.Format("{0:C}", valor[0].proun_preco_venda * Convert.ToInt32(item.vendaPv_item_fpop_qtde_Unitaria));
                    AddList(Barra, Convert.ToInt32(item.vendaPv_item_fpop_qtde_Unitaria));
                    ClassGlobals.Global._dest.CPF = item.cli_id.ToString();
                    ClassGlobals.Global._dest.xNome = item.tb_cliente.cli_nome;
                    RodapeClienete(item.tb_cliente.cli_cpf);
                }
                #endregion
            }
            else
            {
                lbStatus.Content = "Orçamento não localizado.";
            }
        }
        public bool AddList(tb_produto_barra barra , int qtd)
        {
            try
            {
                decimal PromoVlr = 0;
                var prod = barra.tb_produto;
                List<tbr_produto_uneg> Vlr = new List<tbr_produto_uneg>(prod.tbr_produto_uneg);
                Itbr_promocao_produtoRepository promo = new tbr_promocao_produtoRepository();
                var respPromo = promo.First(x => x.pro_id.Equals(barra.pro_id) && x.prom_situacao == "A");

                if (respPromo != null)
                {
                    for (int e = 0; e < qtd; e++)
                    {
                        NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao, unidade = prod.pro_unidade_venda, codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = 1, vlrUnit = ((decimal)respPromo.prom_preco), codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });
                    }
                }
                for (int e = 0; e < qtd; e++)
                {
                    NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao, unidade = prod.pro_unidade_venda, codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = 1, vlrUnit = ((decimal)Vlr[0].proun_preco_venda), codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });
                }
                ListNota.Visibility = Visibility.Visible;


                ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) }).ToList();

               // ValorTot = ValorTot + (decimal)Vlr[0].proun_preco_venda;
                ListNota.SelectedIndex = (ListNota.Items.Count - 1);
                tbVlrTotal.Content = String.Format("{0:C}", NFCe.lstProd.Sum(d=>d.vlrUnit));
                VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, ListNota, null);
                
                double scrollHeight = vsp.ScrollOwner.ScrollableHeight;

                double offset = scrollHeight * ListNota.SelectedIndex / ListNota.Items.Count;

                vsp.SetVerticalOffset(offset);
                RodapeValor(String.Format("{0:C}", NFCe.lstProd.Sum(d => d.vlrUnit)));
                return true;
            }
            catch
            {

                return false;
            }

        }
        public bool AddList(tb_produto_barra barra)
        {
            try
            {
                

                var prod = barra.tb_produto;
                if (prod.pro_tb_balanca == "S" && AtendeMEsa == false)
                {
                    frmDialog fr = new frmDialog(this);
                    fr.ShowDialog();

                    List<tbr_produto_uneg> Vlr = new List<tbr_produto_uneg>(prod.tbr_produto_uneg);
                    Itbr_promocao_produtoRepository promo = new tbr_promocao_produtoRepository();
                    try
                    {
                        var respPromo = promo.First(x => x.pro_id.Equals(barra.pro_id) && x.prom_situacao == "A");


                        NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao + " -KG " + Peso, unidade = "KG", codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = Convert.ToDecimal(Peso), vlrUnit = (Convert.ToDecimal(Peso) * (decimal)respPromo.prom_preco), vKG = (decimal)respPromo.prom_preco, codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });
                    }
                    catch
                    {
                        NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao + " -KG " + Peso, unidade = "KG", codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = Convert.ToDecimal(Peso), vlrUnit = (Convert.ToDecimal(Peso) * (decimal)Vlr[0].proun_preco_venda), vKG = (decimal)Vlr[0].proun_preco_venda, codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });
                       
                    }
                  
                 
                    
                 

                    ListNota.Visibility = Visibility.Visible;
                    ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) }).ToList();
                    tbVlrProd.Content = String.Format("{0:C}", (Convert.ToDecimal(Peso) * (decimal)Vlr[0].proun_preco_venda));
                    ValorTot = NFCe.lstProd.Sum(f => f.vlrUnit);
                    ListNota.SelectedIndex = (ListNota.Items.Count - 1);
                    tbVlrTotal.Content = String.Format("{0:C}", ValorTot);
                    VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, ListNota, null);

                    double scrollHeight = vsp.ScrollOwner.ScrollableHeight;

                    double offset = scrollHeight * ListNota.SelectedIndex / ListNota.Items.Count;

                    vsp.SetVerticalOffset(offset);
                    RodapeValor(String.Format("{0:C}", ValorTot));
                    Peso = string.Empty;
                    return true;
                }
                if (prod.pro_tb_balanca != "S" && AtendeMEsa == false)
                {
                    List<tbr_produto_uneg> Vlr = new List<tbr_produto_uneg>(prod.tbr_produto_uneg);
                    Itbr_promocao_produtoRepository promo = new tbr_promocao_produtoRepository();

                    try
                    {
                           var respPromo = promo.First(x => x.pro_id.Equals(barra.pro_id) && x.tb_promocao.prm_situacao == "A");

                   
                        NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao, unidade = prod.pro_unidade_venda, codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = 1, vlrUnit = (decimal)respPromo.prom_preco, codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });

                   
                    }
                    catch 
                    {
                        
                    
                        NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao, unidade = prod.pro_unidade_venda, codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = 1, vlrUnit = (decimal)Vlr[0].proun_preco_venda, codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });
                    
                    }
                 
                  

                    ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) }).ToList();
                    ListNota.Visibility = Visibility.Visible;
                    ValorTot = NFCe.lstProd.Sum(f => f.vlrUnit);
                    ListNota.SelectedIndex = (ListNota.Items.Count - 1);
                    tbVlrTotal.Content = String.Format("{0:C}", ValorTot);
                    VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, ListNota, null);

                    double scrollHeight = vsp.ScrollOwner.ScrollableHeight;

                    double offset = scrollHeight * ListNota.SelectedIndex / ListNota.Items.Count;

                    vsp.SetVerticalOffset(offset);
                    RodapeValor(String.Format("{0:C}", ValorTot));
                    return true;
                }
                if(prod.pro_tb_balanca != "S" && AtendeMEsa == true)
                {
                    List<tbr_produto_uneg> Vlr = new List<tbr_produto_uneg>(prod.tbr_produto_uneg);
                    NFCe.lstProd.Add(new objNota.Produto { descricao = prod.pro_descricao, unidade = prod.pro_unidade_venda, codigo = barra.barra_codigo.ToString(), NCM = prod.pro_ncm.ToString(), qtd = 1, vlrUnit = (decimal)Vlr[0].proun_preco_venda, codigoInterno = prod.pro_id.ToString(), CST = prod.pro_cod_csosn_icms.ToString(), Orig = prod.pro_cod_origem.ToString() });
                     Itb_venda_prevendaRepository preMesa = new tb_venda_prevendaRepository();
                     var Varmesa = preMesa.First(x => x.vendaPv_num_comanda == numMesa.ToString());
                     ListNota.Visibility = Visibility.Visible;
                    ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) }).ToList();
                    InsereCompraItem(NFCe.lstProd, DateTime.Now, Varmesa.vendaPv_id, NFCe.lstProd.Count());
                    ValorTot = NFCe.lstProd.Sum(f => f.vlrUnit);
                    ListNota.SelectedIndex = (ListNota.Items.Count - 1);
                    tbVlrTotal.Content = String.Format("{0:C}", ValorTot);
                    VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, ListNota, null);

                    double scrollHeight = vsp.ScrollOwner.ScrollableHeight;

                    double offset = scrollHeight * ListNota.SelectedIndex / ListNota.Items.Count;

                    vsp.SetVerticalOffset(offset);
                    RodapeValor(String.Format("{0:C}", ValorTot));
                    return true;
                }
                return true;
            }
            catch
            {

                return false;
            }

        }
        public static void InsereCompraItem(List<objNota.Produto> lst, DateTime data, int venda_id, int iddd)
        {
            int idd = iddd +1;
            Itb_venda_prevenda_itemRepository preIVenda = new tb_venda_prevenda_itemRepository();
            List<tb_venda_prevenda_item> lstItem = new List<tb_venda_prevenda_item>();
            tb_venda_prevenda_item pre = new tb_venda_prevenda_item();
            foreach (var item in lst)
            {
                lstItem.Add(new tb_venda_prevenda_item { vendaPv_item_id = idd, vendaPv_data_preVenda = data, vendaPv_id = venda_id, uneg_id = 1, venda_pdv = 10, venda_id = venda_id, vendaPv_item_codigo = Convert.ToInt32(item.codigoInterno), vendaPv_item_descricao = item.descricao, pro_id = Convert.ToInt32(item.codigoInterno), vendaPv_item_totalValor = (item.qtd * item.vlrUnit), vendaPv_item_fpop_qtde_Unitaria = Convert.ToInt32(item.qtd), vendaPv_item_ean13 = item.codigo.ToString(),vendaPv_item_status = "RC" });
                idd++;
            }
            preIVenda.AddAllList(lstItem, false);

        }
        public void ResgataMesa()
        {
            Itb_venda_prevendaRepository preMesa = new tb_venda_prevendaRepository();
            var Varmesa = preMesa.First(x => x.vendaPv_num_comanda == numMesa.ToString());
            Itb_venda_prevenda_itemRepository PreIt = new tb_venda_prevenda_itemRepository();
            try
            {
                List<tb_venda_prevenda_item> lstMesaIt = new List<tb_venda_prevenda_item>(PreIt.Find(x => x.venda_id.Equals(Varmesa.vendaPv_id) && x.vendaPv_item_status == "RC"));

                if (lstMesaIt.Count() >= 1)
                {
                    foreach (var item in lstMesaIt)
                    {
                        List<tbr_produto_uneg> Vlr = new List<tbr_produto_uneg>(item.tb_produto.tbr_produto_uneg);
                        NFCe.lstProd.Add(new objNota.Produto { descricao = item.tb_produto.pro_descricao, unidade = item.tb_produto.pro_unidade_venda, codigo = item.tb_produto.tb_produto_barra.First(x => x.pro_id.Equals(item.pro_id)).barra_codigo.ToString(), NCM = item.tb_produto.pro_ncm.ToString(), qtd = 1, vlrUnit = (decimal)Vlr[0].proun_preco_venda, codigoInterno = item.tb_produto.pro_id.ToString(), CST = item.tb_produto.pro_cod_csosn_icms.ToString(), Orig = item.tb_produto.pro_cod_origem.ToString() });
                    }

                    ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) }).ToList();
                }
                ListNota.Visibility = System.Windows.Visibility.Visible;
                ValorTot = NFCe.lstProd.Sum(f => f.vlrUnit);
                ListNota.SelectedIndex = (ListNota.Items.Count - 1);
                tbVlrTotal.Content = String.Format("{0:C}", ValorTot);
                VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null, ListNota, null);

                double scrollHeight = vsp.ScrollOwner.ScrollableHeight;

                double offset = scrollHeight * ListNota.SelectedIndex / ListNota.Items.Count;

                vsp.SetVerticalOffset(offset);
                RodapeValor(String.Format("{0:C}", ValorTot));
            }
            catch
            {
                
               
            }
           
        }
        public void removerItem(string cod)
        {
            try
            {
              
                foreach (var item in NFCe.lstProd)
                {

                    if (item.codigo.Equals(cod))
                    {
                        ValorTot = (ValorTot - item.vlrUnit);
                        NFCe.lstProd.Remove(item);
                        ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) });
                        tbVlrTotal.Content = String.Format("{0:C}", NFCe.lstProd.Sum(f => f.vlrUnit));
                        tbCodPro.Clear();
                        lbStatus.Content = "Produto "+item.codigo+" Cancelado";
                        RodapeValor(String.Format("{0:C}", NFCe.lstProd.Sum(f=>f.vlrUnit)));
                        break;
                    }
                      
                }

            }
            catch
            {
                
               
            }
        
        }
        public bool ver(string ver)
        {
            try
            {
                Convert.ToInt64(ver);
                return true;
            }
            catch
            {

                return false;
            }
        }
        private void tbCodPro_TextChanged(object sender, TextChangedEventArgs e)
        {

           
             #region INSERE PRODUTOS
            if (tbCodPro.Text.Length == 13 && ver(tbCodPro.Text))
            {
                if (Global._Usuario == null)
                {
                    lbStatus.Content = "CAIXA FECHADO";
                    tbCodPro.Clear();
                }
                else
                {
                    try
                    {
                        if (!itemCancela)
                        {
                            lbStatus.FontSize = 25;
                            var Barra = Global.lstBarra.Find(y => y.barra_codigo == Convert.ToInt64(tbCodPro.Text));
                            lbStatus.Content = Barra.tb_produto.pro_descricao;
                            List<tbr_produto_uneg> valor = new List<tbr_produto_uneg>(Barra.tb_produto.tbr_produto_uneg);
                            tbVlrProd.Content = String.Format("{0:C}", valor[0].proun_preco_venda);
                            AddList(Barra);
                            tbCodPro.Text = string.Empty;
                        }
                        if (itemCancela)
                        {
                            removerItem(tbCodPro.Text);

                            itemCancela = false;
                        }
                    }
                    catch
                    {

                        lbStatus.Content = "Produto não encontrado";
                    }



                }
            }
                #endregion
            
        }

        private void tbPass_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length == 4)
            {

            }

        }

        public void aberturaCaixa(bool fecha)
        {
            Itb_abertura_caixaRepository abrecaixa = new tb_abertura_caixaRepository();
          
            if (!fecha)
            {

                Global._aCaixa.aberturaCx_dt_abertura = DateTime.Now;
                Global._aCaixa.uneg_id = Convert.ToInt32(Global.Term.uneg_id);
                Global._aCaixa.aberturaCx_usr_id_operador = Global._Usuario.usr_id;
                abrecaixa.Add(Global._aCaixa);
              
            } if (fecha)
            {
               
                Global._aCaixa.aberturaCx_dt_fechamento = DateTime.Now;
                List<tb_abertura_caixa> lst = new List<tb_abertura_caixa>(abrecaixa.GetAll());
                lst.RemoveAt(lst.Count() -1);
                lst.Add(Global._aCaixa);

                abrecaixa.AddAllList(lst, true);
              
            }

           
        }

        public void RodapeVenda(string venda)
        {
            lbVenda.Content = "Venda Nº: " + venda;
        }
        public void RodapeClienete(string cliente)
        {
            lbCliente.Content = "Cliente: " + cliente;
        }
        public void RodapeValor(string valor)
        {
            lbValor.Content = "Valor: " + valor;
        }
       
        public void RodapeDesconto(string desconto)
        {
            lbDesc.Content = "Desconto: " + desconto;
        }
        public void LimparodaPe()
        {
            lbDesc.Content = "Desconto: ";
            lbValor.Content = "Valor: ";
            lbCliente.Content = "Cliente: ";
        
        }

        private void DataGri_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string teste = e.AddedItems[0].ToString();
                Itb_produto_barraRepository bg = new tb_produto_barraRepository();
                lbStatus.FontSize = 25;
                BuscaProdCod = bg.First(u => u.tb_produto.pro_descricao.Equals(teste)).barra_codigo.ToString();
                lbStatus.Content = bg.First(u => u.tb_produto.pro_descricao.Equals(teste)).tb_produto.pro_descricao;
                tbVlrProd.Content = String.Format("{0:C}",bg.First(u => u.tb_produto.pro_descricao.Equals(teste)).tb_produto.tbr_produto_uneg.First().proun_preco_venda);
            }
            catch 
            {
                
              
            }
          
        }

        private void DataGri_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && BuscaProd == true)
            {
                tbCodPro.Text = BuscaProdCod;
                BuscaProd = false;
                BuscaProdCod = string.Empty;
                pnlCliente.Visibility = Visibility.Visible;
                DataGri.Visibility = Visibility.Hidden;
                tbCodPro.Clear();
                Keyboard.Focus(tbCodPro);
            }
        }

        public static void InsereCompra(DateTime data, int vendaPv_id, int venda_id, string vendaPv_num_preVenda, decimal vendaPv_valorTotal, string status, Int64 cliId, bool addCli,string mesa)
        {
            Itb_venda_prevendaRepository preVenda = new tb_venda_prevendaRepository();
            tb_venda_prevenda pre = new tb_venda_prevenda();
            pre.vendaPv_id = vendaPv_id;
            pre.venda_data = data;
            pre.uneg_id = 1;
            pre.venda_pdv = 10;
            pre.vendaPv_num_comanda = mesa;
            if (addCli)
            {
                pre.cli_id = cliId;
            }
            pre.venda_id = venda_id;
            pre.vendaPv_data_preVenda = data;
            pre.vendaPv_num_preVenda = vendaPv_num_preVenda;
            pre.vendaPv_valorTotal = vendaPv_valorTotal;
            pre.vendaPv_situacao = status;
            preVenda.Add(pre);


        }

        private void Window_KeyUp_1(object sender, KeyEventArgs e)
        {
    


        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (Global._Usuario == null)
            {
                lbStatus.Content = "CAIXA FECHADO";
                return;
            }

            if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control && AtendeMEsa == false)
            {
                var mesa = Funcoes.InpuBox("Número da Mesa", "Digite o Número da Mesa:");
                Itb_venda_prevendaRepository VendaMesa = new tb_venda_prevendaRepository();
                int numPreV = VendaMesa.GetAll().Count() + 1;
                try
                {
                    numMesa = Convert.ToInt32(mesa);
                    AtendeMEsa = true;

                    if (VendaMesa.Find(x => x.vendaPv_num_comanda == mesa && x.vendaPv_situacao == "RC").Count() == 1)
                    {
                        ResgataMesa();
                        //InsereCompra(DateTime.Now, numPreV, numPreV, mesa, Convert.ToDecimal("0"), "RC", Convert.ToInt64(1), false,mesa);
                        lbMesa.Content = "Mesa {" + mesa + "} Status {Aberto}";
                        RodapeClienete("Mesa {" + mesa + "}");
                    }
                    else
                    {
                        InsereCompra(DateTime.Now, numPreV, numPreV, mesa, Convert.ToDecimal("0"), "RC", Convert.ToInt64(1), false, mesa);
                        lbMesa.Content = "Mesa {" + mesa + "} Status {Aberto}";
                        RodapeClienete("Mesa {" + mesa + "}");

                    }

                    tbCodPro.Text = string.Empty;
                }
                catch
                {

                }

            }
        }
       
    }
}
