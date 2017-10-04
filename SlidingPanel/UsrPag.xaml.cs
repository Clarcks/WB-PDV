using ClassGlobals;
using NFe.AppTeste;
using NFe.Classes;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Classes.Informacoes.Pagamento;
using NFe.Classes.Servicos.Tipos;
using NFe.Servicos;
using NFe.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApplication1;
using NFe.Utils.NFe;
using NFe.Impressao.NFCe;
using NFe.Classes.Informacoes;
using NFe.Classes.Informacoes.Cobranca;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Identificacao;
using NFe.Classes.Informacoes.Observacoes;
using ASArquiteruraData;
using ASAsysFwSiTef;
using System.Text;
using ASArquiteruraData.RepositoryInterfaces;
using ASArquiteruraData.Repository;

namespace SlidingPanel
{
    /// <summary>
    /// Interaction logic for UsrPag.xaml
    /// </summary>
    public partial class UsrPag : UserControl
    {
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
        MainWindow form;
        List<pag> lstPag = new List<pag>();
        decimal  valorPagto = Valor.Arredondar(NFCe.lstProd.Sum(s=>s.vlrUnit) / 1, 2);
        private const string ArquivoConfiguracao = @"\configuracao.xml";
        private const string TituloErro = "Erro";
        public bool TefUser = false;
        private ConfiguracaoApp _configuracoes;
        public static NFe.Classes.NFe _nfe;
        public string Resp = string.Empty;
        public string MensagemTela = string.Empty;
        public string Troco = "0";
        public bool imprime = false;
        public static string _path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public FormaPagamento formaPag;
        public Itb_nfeRepository _nfeResp = new tb_nfeRepository();
        public Itb_produto_barraRepository proT = new tb_produto_barraRepository();
        public Itb_nfe_itemRepository _nfeItemResp = new tb_nfe_itemRepository();
        Itb_cliente_enderecoRepository end = new tb_cliente_enderecoRepository();
        public tb_cliente_endereco endereco;
        public UsrPag(MainWindow frm)
        {
            form = frm;
            InitializeComponent();
            this.Width = form.BordaDetNota.ActualWidth;
            CarregarConfiguracao();
         
            DataContext = _configuracoes;
            this.DataContext = this;
            lbVlrTotal.Content = String.Format("{0:C}",valorPagto - Global._VlrDescNFce);
            btDinheiro.Focus();
            this.Height = form.pnlPag.ActualHeight;
            form.BorPag.Height = form.pnlPag.ActualHeight;
            form.FramePag.Height = form.pnlPag.ActualHeight;
            form.Fechavenda = true;
            if (Global.Term.tb_unid_negocio.uneg_tef_IP != null)
            {
                form.lbStatus.Content = "FORMA: F1-Dinheiro  F2-Débito  F3-Crédito F4-TEF";
                TefUser = true;
            }
            if (Global.Term.tb_unid_negocio.uneg_tef_IP == null)
            {
                form.lbStatus.Content = "FORMA: F1-Dinheiro  F2-Débito  F3-Crédito";
                TefUser = false;
            }
             lstPag.Clear();
            this.Loaded += OnLoaded;
            valorPagto = (valorPagto - Global._VlrDescNFce);
        }
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this.txtDinheiro);

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
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Global.Finalizadora = 1;
            FormPag("Dinheiro");
            formaPag = FormaPagamento.fpDinheiro;
        }
        public void ObsLabel(string msg)
        {
            form.lbStatus.Content = msg;
        
        }
        private void txtDinheiro_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        public void LimpaTela()
        {
            if (form.AtendeMEsa)
            {
                Itb_venda_prevendaRepository vv = new tb_venda_prevendaRepository();
                Itb_venda_prevenda_itemRepository vvIt = new tb_venda_prevenda_itemRepository();
          
                tb_venda_prevenda mes = new tb_venda_prevenda();
                mes = vv.First(x => x.vendaPv_num_comanda == form.numMesa.ToString());
                mes.vendaPv_situacao = "IM";
                List<tb_venda_prevenda_item> lstIt = new List<tb_venda_prevenda_item>(vvIt.Find(x=>x.venda_id.Equals(mes.vendaPv_id)));
                foreach (var item in lstIt)
                {
                    item.vendaPv_item_status = "IM";
                }
                List<tb_venda_prevenda> ll = new List<tb_venda_prevenda>();
                ll.Add(mes);
                vv.AddAllList(ll, false);
                vvIt.AddAllList(lstIt, false);
                form.AtendeMEsa = false;
            }
            Global.lst.Clear();
            NFCe.lstProd.Clear();
            NFCe.lstProd = new List<objNota.Produto>();
            this.lstPag.Clear();
            form.ListNota.ItemsSource = NFCe.lstProd.GroupBy(s => s.codigo + "  " + s.descricao + Environment.NewLine + String.Format("{0:C}", s.vlrUnit) + "  ").Select(g => new { g.Key, X = g.Count() + " = " + (String.Format("{0:C}", g.Sum(s => s.vlrUnit))) }).ToList();
            form.tbVlrProd.Content = "0,00";
            form.tbVlrTotal.Content = "0,00";
            form.ValorTot = 0;
            form.lbStatus.Content = "CAIXA ABERTO";
            form.pnlPag.Visibility = Visibility.Hidden;
            form.ListNota.Visibility = Visibility.Visible;
            form.lbMesa.Content = "F8 - Para Excluir Item.";
            form.RodapeVenda(Global.Term.te_numero_nfce.ToString());
            form.LimparodaPe();
            form.Fechavenda = false; 

            if (Global._VlrDescNFceBool)
            {
                Global._VlrDescNFceBool = false;
            }
            Keyboard.Focus(form.tbCodPro);
        }

        #region VENDA

        public void FechaVenda()
        {
            try
            {
                 var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                 var retornoEnvio = servicoNFe.NfeStatusServico();
                 _configuracoes.CfgServico.tpEmis = TipoEmissao.teNormal;
            }
            catch 
            {
                _configuracoes.CfgServico.tpEmis = TipoEmissao.teOffLine;
               
            }

            try
            {
                #region Cria e Envia NFe


               // _configuracoes.CfgServico.tpEmis = TipoEmissao.teOffLine;
                //var numero = Funcoes.InpuBox("Criar e Enviar NFe", "Número da Nota:");
                //if (string.IsNullOrEmpty(numero)) throw new Exception("O Número deve ser informado!");

                //var lote = Funcoes.InpuBox("Criar e Enviar NFe", "Id do Lote:");
                //if (string.IsNullOrEmpty(lote)) throw new Exception("A Id do lote deve ser informada!");

                _nfe = GetNf(Convert.ToInt32(ClassGlobals.Global.Term.te_numero_nfce), _configuracoes.CfgServico.ModeloDocumento,
                    _configuracoes.CfgServico.VersaoNFeAutorizacao);
                _nfe.Assina(); //não precisa validar aqui, pois o lote será validado em ServicosNFe.NFeAutorizacao
                //A URL do QR-Code deve ser gerada em um objeto nfe já assinado, pois na URL vai o DigestValue que é gerado por ocasião da assinatura
                //Descomente a linha abaixo se a SEFAZ de sua UF já habilitou a NT2015.002

                _configuracoes.ConfiguracaoDanfeNfce.cIdToken = Global.Term.te_id_token_sefaz;
                _configuracoes.ConfiguracaoDanfeNfce.CSC = Global.Term.te_token_sefaz;

                
                _nfe.infNFeSupl = new infNFeSupl() { qrCode = EnderecadorDanfeNfce.ObterUrlQrCode(_nfe, _configuracoes.ConfiguracaoDanfeNfce) }; //Define a URL do QR-Code.

               // _nfe.SalvarArquivoXml(@"C:/teste/Novo.xml");

                //IMPRESSAO CONTIGENCIA_
                ClassGlobals.Global._VlrTotalNFce = (Valor.Arredondar(lstPag.Sum(i => i.vPag), 2) + Convert.ToDecimal(Troco));
                ClassGlobals.Global._VlrTrocolNFce = (Convert.ToDecimal(Troco));
                if (_configuracoes.CfgServico.tpEmis == TipoEmissao.teOffLine)
                {
                    _nfe.SalvarArquivoXml( _path+@"/Ctgs"+_nfe.infNFe.Id+".xml");
                    if (Global._dest.CPF == string.Empty)
                    {
                        Impressora.ImprimirDanferE(_path + @"/Ctgs" + _nfe.infNFe.Id + ".xml");
                    }
                    else
                    {
                        Impressora.ImprimirDanferCom(_path + @"/Ctgs" + _nfe.infNFe.Id + ".xml");
                        Global._dest.CPF = null;
                        Global._dest.xNome = null;
                    }
                        #region NFE | NFE ITEM

                    tb_nfe tbnfe = new tb_nfe();

                    tbnfe.uneg_id = Global.Term.tb_unid_negocio.uneg_id;
                    tbnfe.nfe_pdv = Global.Term.te_id_terminal;
                    tbnfe.nfe_id = (_nfeResp.GetAll().Count() + 1);
                    tbnfe.nfe_data = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

                    tbnfe.nfe_cfop = _nfe.infNFe.det[0].prod.CFOP;
                    tbnfe.nfe_chave_acesso = _nfe.infNFe.Id.ToString().Substring(3, _nfe.infNFe.Id.ToString().Length - 3);
                    tbnfe.nfe_cod_municipio = _nfe.infNFe.ide.cMunFG;
                    tbnfe.nfe_data_saida = _nfe.infNFe.ide.dhSaiEnt;
                    tbnfe.nfe_icmstot_vbc = _nfe.infNFe.total.ICMSTot.vBC;
                    tbnfe.nfe_icmstot_vcofins = _nfe.infNFe.total.ICMSTot.vCOFINS;
                    tbnfe.nfe_icmstot_vdesc = _nfe.infNFe.total.ICMSTot.vDesc;
                    tbnfe.nfe_icmstot_vfrete = _nfe.infNFe.total.ICMSTot.vFrete;
                    tbnfe.nfe_icmstot_vicms = _nfe.infNFe.total.ICMSTot.vICMS;
                    tbnfe.nfe_icmstot_vii = _nfe.infNFe.total.ICMSTot.vII;
                    tbnfe.nfe_icmstot_vipi = _nfe.infNFe.total.ICMSTot.vIPI;
                    tbnfe.nfe_icmstot_vnf = _nfe.infNFe.total.ICMSTot.vNF;
                    tbnfe.nfe_icmstot_voutros = _nfe.infNFe.total.ICMSTot.vOutro;
                    tbnfe.nfe_icmstot_vpis = _nfe.infNFe.total.ICMSTot.vPIS;
                    tbnfe.nfe_icmstot_vprod = _nfe.infNFe.total.ICMSTot.vProd;
                    tbnfe.nfe_icmstot_vseg = _nfe.infNFe.total.ICMSTot.vSeg;
                    tbnfe.nfe_icmstot_vst = _nfe.infNFe.total.ICMSTot.vST;

                    tbnfe.nfe_id_vendedor = Global._aCaixa.aberturaCx_usr_id_operador;
                    if (_nfe.infNFe.total.ISSQNtot != null)
                    {
                        tbnfe.nfe_issqntot_vbc = _nfe.infNFe.total.ISSQNtot.vBC;
                        tbnfe.nfe_issqntot_vcofins = _nfe.infNFe.total.ISSQNtot.vCOFINS;
                        tbnfe.nfe_issqntot_viss = _nfe.infNFe.total.ISSQNtot.vISS;
                        tbnfe.nfe_issqntot_vpis = _nfe.infNFe.total.ISSQNtot.vPIS;
                        tbnfe.nfe_issqntot_vserv = _nfe.infNFe.total.ISSQNtot.vServ;
                        tbnfe.nfe_itens_produto = _nfe.infNFe.det.Count();
                    }
                    tbnfe.nfe_modelo = tbnfe.nfe_modelo = "NFCe";
                    tbnfe.nfe_nat_operacao = Convert.ToDecimal(_nfe.infNFe.ide.finNFe);
                    tbnfe.nfe_numero = _nfe.infNFe.ide.nNF;

                    tbnfe.nfe_qtd_produto = _nfe.infNFe.det.Count();

                    tbnfe.nfe_serie = _nfe.infNFe.ide.serie;
                    tbnfe.nfe_situacao = "AT";
                    tbnfe.nfe_tip_frete = "1";
                    tbnfe.nfe_tipo = "1";
                    tbnfe.nfe_total_venda = _nfe.infNFe.total.ICMSTot.vNF;
                    tbnfe.nfe_uf_destino = _nfe.infNFe.emit.enderEmit.UF;
                    tbnfe.nfe_uf_origem = _nfe.infNFe.emit.enderEmit.UF;


                    InsereNFe(tbnfe);

                    List<tb_nfe_item> lstItem = new List<tb_nfe_item>();
                    foreach (var pro in _nfe.infNFe.det)
                    {

                        var PIS = (PISOutr)pro.imposto.PIS.TipoPIS;

                        var COFINS = (COFINSOutr)pro.imposto.COFINS.TipoCOFINS;

                        ICMSSN102 ICMS = new ICMSSN102();
                        if (pro.imposto.ICMS.TipoICMS.ToString().Contains("102"))
                        {
                            ICMS = (ICMSSN102)pro.imposto.ICMS.TipoICMS;

                        }

                        lstItem.Add(new tb_nfe_item
                        {
                            nfe_data = tbnfe.nfe_data,
                            cfop_id = pro.prod.CFOP,
                            infe_cofins_cst = Convert.ToInt32(COFINS.CST.ToString().Replace("cofins", "")),
                            infe_cofins_pcofins = COFINS.pCOFINS,
                            infe_cofins_vbccofins = COFINS.vBC,
                            infe_cofins_vcofiins = COFINS.vCOFINS,
                            infe_custo_medio = (pro.prod.vProd * pro.prod.qCom),
                            infe_custo_unit_prod = pro.prod.vProd,
                            //infe_icms_cst = Convert.ToInt32(ICMS.CST),
                            //infe_icms_modbc = Convert.ToInt32(ICMS.modBC),
                            //infe_icms_picms = ICMS.pICMS,
                            //infe_icms_vbc = ICMS.vBC,
                            //infe_icms_vicms = ICMS.vICMS,
                            infe_id = pro.nItem,
                            infe_pis_cst = Convert.ToInt32(PIS.CST.ToString().Replace("pis", "")),
                            infe_pis_ppis = PIS.pPIS,
                            infe_pis_vbcpis = PIS.vBC,
                            infe_pis_vpis = PIS.vPIS,
                            infe_preco_padrao = pro.prod.vProd,
                            infe_qcom = pro.prod.qCom,
                            infe_vdesc = pro.prod.vDesc,
                            infe_vlr_desconto = pro.prod.vDesc,
                            infe_vlr_frete = pro.prod.vFrete,
                            infe_vprod = pro.prod.vProd,
                            nfe_id = tbnfe.nfe_id,
                            nfe_pdv = Global.Term.te_id_terminal,
                            // pro_id = proT.First(s=>s.barra_codigo.Equals(pro.prod.cEAN)).pro_id,
                            uneg_id = 1


                        });
                    }
                    _nfeItemResp.AddAllList(lstItem, false);
                    #endregion
                    if (Global.VendaTef)
                    {
                        Impressora.ImprimiTef(Global.TefCliente);
                        Impressora.ImprimiTef(Global.TefEstabelecimento);
                        Global.VendaTef = false;
                        Global.TefCliente = string.Empty;
                        Global.TefEstabelecimento = string.Empty;
                    }
                    VendaInserir("N");
                    ClassGlobals.CarregaGlobal.SalvaNumero();
                }

                if (_configuracoes.CfgServico.tpEmis == TipoEmissao.teNormal)
                {
                    var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                    var retornoEnvio = servicoNFe.NFeAutorizacao(Convert.ToInt32("112"), IndicadorSincronizacao.Assincrono, new List<NFe.Classes.NFe> { _nfe }, true/*Envia a mensagem compactada para a SEFAZ*/);
               
                    //var ret = retornoEnvio;
                    //TrataRetorno(retornoEnvio);
                    if (retornoEnvio.Retorno.infRec.nRec != string.Empty)
                    {
                        try
                        {
                            #region Consulta Recibo de lote


                            servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                            var retornoRecibo = servicoNFe.NFeRetAutorizacao(retornoEnvio.Retorno.infRec.nRec);

                            //TrataRetorno(retornoRecibo);
                            //var dlg = new SaveFileDialog
                            //{
                            //    FileName = _nfe.infNFe.Id.Substring(3),
                            //    DefaultExt = ".xml",
                            //    Filter = "Arquivo XML (.xml)|*.xml"
                            //};
                            //var result = dlg.ShowDialog();
                            //if (result != true) return;
                            //var arquivoXml = dlg.FileName;

                            string ch = _nfe.infNFe.Id.Substring(3);
                            _nfe.SalvarArquivoXml(_path + "\\NFCe\\" + ch + ".xml");
                            

                            Prot(_path + "\\NFCe\\" + ch + ".xml");
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            if (!string.IsNullOrEmpty(ex.Message))
                                Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
                        }
                    }
                }
                #endregion
            }
                //AQUI
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        
        }
        public void InsereNFe(tb_nfe nfe)
        {

            _nfeResp.Add(nfe);

        }
        public void Prot(string caminho)
        {
            try
            {
                var arquivoXml = caminho;
                var nfe = new NFe.Classes.NFe().CarregarDeArquivoXml(arquivoXml);
                var chave = nfe.infNFe.Id.Substring(3);

                if (string.IsNullOrEmpty(chave)) throw new Exception("A Chave da NFe não foi encontrada no arquivo!");
                if (chave.Length != 44) throw new Exception("Chave deve conter 44 caracteres!");

                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoConsulta = servicoNFe.NfeConsultaProtocolo(chave);
                //TrataRetorno(retornoConsulta);

                var nfeproc = new nfeProc
                {
                    NFe = nfe,
                    protNFe = retornoConsulta.Retorno.protNFe,
                    versao = retornoConsulta.Retorno.versao
                };
                var novoArquivo = _path + "\\Autorizados\\" + @"\" + nfeproc.protNFe.infProt.chNFe +
                                  "-procNfe.xml";
                FuncoesXml.ClasseParaArquivoXml(nfeproc, novoArquivo);
                //Funcoes.Mensagem("Arquivo salvo em " + novoArquivo, "Atenção", MessageBoxButton.OK);
                if (Global._dest.CPF == null)
                {
                    Impressora.ImprimirDanferE(novoArquivo);
                   
                }
                else
                {
                    Impressora.ImprimirDanferCom(novoArquivo);
                  
                    Global._dest.CPF = null;
                    Global._dest.xNome = null;
                }
                if (Global.VendaTef)
                {
                    Impressora.ImprimiTef(Global.TefCliente);
                    Impressora.ImprimiTef(Global.TefEstabelecimento);
                    Global.VendaTef = false;
                    Global.TefCliente = string.Empty;
                    Global.TefEstabelecimento = string.Empty;
                }
                    #region NFE | NFE ITEM

               tb_nfe tbnfe = new tb_nfe();

               tbnfe.uneg_id = Global.Term.tb_unid_negocio.uneg_id;
               tbnfe.nfe_pdv = Global.Term.te_id_terminal;
               tbnfe.nfe_id = (_nfeResp.GetAll().Count() + 1);
               tbnfe.nfe_data = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

               tbnfe.nfe_cfop = _nfe.infNFe.det[0].prod.CFOP;
               tbnfe.nfe_chave_acesso = _nfe.infNFe.Id.ToString().Substring(3, _nfe.infNFe.Id.ToString().Length - 3);
               tbnfe.nfe_cod_municipio = _nfe.infNFe.ide.cMunFG;
               tbnfe.nfe_data_saida = _nfe.infNFe.ide.dhSaiEnt;
               tbnfe.nfe_icmstot_vbc = _nfe.infNFe.total.ICMSTot.vBC;
               tbnfe.nfe_icmstot_vcofins = _nfe.infNFe.total.ICMSTot.vCOFINS;
               tbnfe.nfe_icmstot_vdesc = _nfe.infNFe.total.ICMSTot.vDesc;
               tbnfe.nfe_icmstot_vfrete = _nfe.infNFe.total.ICMSTot.vFrete;
               tbnfe.nfe_icmstot_vicms = _nfe.infNFe.total.ICMSTot.vICMS;
               tbnfe.nfe_icmstot_vii = _nfe.infNFe.total.ICMSTot.vII;
               tbnfe.nfe_icmstot_vipi = _nfe.infNFe.total.ICMSTot.vIPI;
               tbnfe.nfe_icmstot_vnf = _nfe.infNFe.total.ICMSTot.vNF;
               tbnfe.nfe_icmstot_voutros = _nfe.infNFe.total.ICMSTot.vOutro;
               tbnfe.nfe_icmstot_vpis = _nfe.infNFe.total.ICMSTot.vPIS;
               tbnfe.nfe_icmstot_vprod = _nfe.infNFe.total.ICMSTot.vProd;
               tbnfe.nfe_icmstot_vseg = _nfe.infNFe.total.ICMSTot.vSeg;
               tbnfe.nfe_icmstot_vst = _nfe.infNFe.total.ICMSTot.vST;

               tbnfe.nfe_id_vendedor = Global._aCaixa.aberturaCx_usr_id_operador;
               if (_nfe.infNFe.total.ISSQNtot != null)
               {
                   tbnfe.nfe_issqntot_vbc = _nfe.infNFe.total.ISSQNtot.vBC;
                   tbnfe.nfe_issqntot_vcofins = _nfe.infNFe.total.ISSQNtot.vCOFINS;
                   tbnfe.nfe_issqntot_viss = _nfe.infNFe.total.ISSQNtot.vISS;
                   tbnfe.nfe_issqntot_vpis = _nfe.infNFe.total.ISSQNtot.vPIS;
                   tbnfe.nfe_issqntot_vserv = _nfe.infNFe.total.ISSQNtot.vServ;
                   tbnfe.nfe_itens_produto = _nfe.infNFe.det.Count();
               }
               tbnfe.nfe_modelo = "NFCe";
               tbnfe.nfe_nat_operacao = Convert.ToDecimal(_nfe.infNFe.ide.finNFe);
               tbnfe.nfe_numero = _nfe.infNFe.ide.nNF;

               tbnfe.nfe_qtd_produto = _nfe.infNFe.det.Count();

               tbnfe.nfe_serie = _nfe.infNFe.ide.serie;
               tbnfe.nfe_situacao = "AT";
               tbnfe.nfe_tip_frete = "1";
               tbnfe.nfe_tipo = "1";
               tbnfe.nfe_total_venda = _nfe.infNFe.total.ICMSTot.vNF;
               tbnfe.nfe_uf_destino = _nfe.infNFe.emit.enderEmit.UF;
               tbnfe.nfe_uf_origem = _nfe.infNFe.emit.enderEmit.UF;


               InsereNFe(tbnfe);

               List<tb_nfe_item> lstItem = new List<tb_nfe_item>();
               foreach (var pro in _nfe.infNFe.det)
               {

                   var PIS = (PISOutr)pro.imposto.PIS.TipoPIS;

                   var COFINS = (COFINSOutr)pro.imposto.COFINS.TipoCOFINS;

                   ICMSSN102 ICMS = new ICMSSN102();
                   if (pro.imposto.ICMS.TipoICMS.ToString().Contains("102"))
                   {
                       ICMS = (ICMSSN102)pro.imposto.ICMS.TipoICMS;

                   }

                   lstItem.Add(new tb_nfe_item
                   {
                       nfe_data = tbnfe.nfe_data,
                       cfop_id = pro.prod.CFOP,
                       infe_cofins_cst = Convert.ToInt32(COFINS.CST.ToString().Replace("cofins", "")),
                       infe_cofins_pcofins = COFINS.pCOFINS,
                       infe_cofins_vbccofins = COFINS.vBC,
                       infe_cofins_vcofiins = COFINS.vCOFINS,
                       infe_custo_medio = (pro.prod.vProd * pro.prod.qCom),
                       infe_custo_unit_prod = pro.prod.vProd,
                       //infe_icms_cst = Convert.ToInt32(ICMS.CST),
                       //infe_icms_modbc = Convert.ToInt32(ICMS.modBC),
                       //infe_icms_picms = ICMS.pICMS,
                       //infe_icms_vbc = ICMS.vBC,
                       //infe_icms_vicms = ICMS.vICMS,
                       infe_id = pro.nItem,
                       infe_pis_cst = Convert.ToInt32(PIS.CST.ToString().Replace("pis", "")),
                       infe_pis_ppis = PIS.pPIS,
                       infe_pis_vbcpis = PIS.vBC,
                       infe_pis_vpis = PIS.vPIS,
                       infe_preco_padrao = pro.prod.vProd,
                       infe_qcom = pro.prod.qCom,
                       infe_vdesc = pro.prod.vDesc,
                       infe_vlr_desconto = pro.prod.vDesc,
                       infe_vlr_frete = pro.prod.vFrete,
                       infe_vprod = pro.prod.vProd,
                       nfe_id = tbnfe.nfe_id,
                       nfe_pdv = Global.Term.te_id_terminal,
                       // pro_id = proT.First(s=>s.barra_codigo.Equals(pro.prod.cEAN)).pro_id,
                       uneg_id = 1


                   });
               }
               _nfeItemResp.AddAllList(lstItem, false);
               #endregion
                if(Global.VendaTef)
                {
                    Impressora.ImprimiTef(Global.TefCliente);
                    Impressora.ImprimiTef(Global.TefEstabelecimento);
                    Global.VendaTef = false;
                    Global.TefCliente = string.Empty;
                    Global.TefEstabelecimento = string.Empty;
                }
               VendaInserir(retornoConsulta.Retorno.protNFe.infProt.nProt);
               ClassGlobals.CarregaGlobal.SalvaNumero();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }

        }

        public bool VendaInserir(string nProt)
        {
            #region INSERE VENDA
              ClassGlobals.Global.Venda = new tb_venda();
              ClassGlobals.Global.Venda.venda_id = Convert.ToInt32(ClassGlobals.Global.Term.te_numero_nfce);
              ClassGlobals.Global.Venda.uneg_id =  (int)ClassGlobals.Global.Term.uneg_id;
              ClassGlobals.Global.Venda.venda_pdv = ClassGlobals.Global.Term.te_id_terminal;
              ClassGlobals.Global.Venda.venda_data = DateTime.Now;
              ClassGlobals.Global.Venda.venda_operadorcx_id = Global._aCaixa.aberturaCx_usr_id_operador;
              ClassGlobals.Global.Venda.venda_vendedor_id = Global._aCaixa.aberturaCx_usr_id_operador;
              ClassGlobals.Global.Venda.ecf_numero_serie = "1";
              ClassGlobals.Global.Venda.venda_tot_itens = NFCe.lstProd.Count();
              ClassGlobals.Global.Venda.venda_tot_valor = valorPagto;
              ClassGlobals.Global.Venda.venda_status = "FN";
              ClassGlobals.Global.Venda.venda_nfce_chave = _nfe.infNFe.Id.Substring(3,44) ;
              if (_configuracoes.CfgServico.tpEmis == TipoEmissao.teOffLine)
              {
                  ClassGlobals.Global.Venda.venda_nfce_protocolo = "CONTIGENCIA";
              }
              if (_configuracoes.CfgServico.tpEmis == TipoEmissao.teNormal)
              {
                  ClassGlobals.Global.Venda.venda_nfce_protocolo = nProt;
              }
            //ClassGlobals.Global.Venda.venda_nfce_protocolo = retornoConsulta.Retorno.protNFe.infProt.nProt;
              ClassGlobals.Global.Venda.venda_num_cupom = ClassGlobals.Global.Term.te_numero_nfce;
              Venda.Gravavenda(ClassGlobals.Global.Venda);
              #endregion

            #region INSERE VENDA PAGAMENTO
              ClassGlobals.Global.VendaPagamento = new tb_venda_pagamento();

              ClassGlobals.Global.VendaPagamento.venda_id = Convert.ToInt32(ClassGlobals.Global.Term.te_numero_nfce);
              ClassGlobals.Global.VendaPagamento.uneg_id = (int)ClassGlobals.Global.Term.uneg_id;
              ClassGlobals.Global.VendaPagamento.venda_pdv = ClassGlobals.Global.Term.te_id_terminal;
              ClassGlobals.Global.VendaPagamento.venda_data = DateTime.Now;
              ClassGlobals.Global.VendaPagamento.vpag_valor = valorPagto;
              ClassGlobals.Global.VendaPagamento.vpag_troco = (lstPag.Sum(s => s.vPag) - valorPagto);
              ClassGlobals.Global.VendaPagamento.vpag_status = "FN";
              ClassGlobals.Global.VendaPagamento.final_id = Global.Finalizadora;
              ClassGlobals.Global.VendaPagamento.finsub_id = 9;
              ClassGlobals.Global.VendaPagamento.vpag_valor_compra_saque = valorPagto;
              Venda.GravavendaPagamento(ClassGlobals.Global.VendaPagamento);
              #endregion
           
            #region INSERE VENDA ITEM
              List<tb_venda_item> lstItem = new List<tb_venda_item>();
              int count = 1;  
            foreach (var item in NFCe.lstProd)
              {
                  lstItem.Add(new tb_venda_item { uneg_id = (int)ClassGlobals.Global.Term.uneg_id, venda_id = Convert.ToInt32(ClassGlobals.Global.Term.te_numero_nfce), venda_pdv = ClassGlobals.Global.Term.te_id_terminal, venda_data = DateTime.Now, vitem_id = count, vitem_qtde = 1, vitem_preco = item.vlrUnit, vitem_descricao = item.descricao,pro_id = Convert.ToInt32(item.codigoInterno)});
                  count++;
            }
            count = 1;
              Venda.GravavendaItem(lstItem);
              #endregion
              
            return true;
        }

        #endregion
        #region Criar NFe

        protected virtual NFe.Classes.NFe GetNf(int numero, ModeloDocumento modelo, VersaoServico versao)
        {
            var nf = new NFe.Classes.NFe { infNFe = GetInf(numero, modelo, versao) };
            return nf;
        }

        protected virtual infNFe GetInf(int numero, ModeloDocumento modelo, VersaoServico versao)
        {
            var infNFe = new infNFe
            {
                versao = Auxiliar.VersaoServicoParaString(versao),
                ide = GetIdentificacao(numero, modelo, versao),
                emit = GetEmitente(),

                dest = GetDestinatario(versao, modelo),
                
                transp = GetTransporte()
            };

            for (var i = 0; i < NFCe.lstProd.Count; i++)
            {
                infNFe.det.Add(GetDetalhe(i, infNFe.emit.CRT, modelo));
            }

            infNFe.total = GetTotal(versao, infNFe.det);

            if (infNFe.ide.mod == ModeloDocumento.NFe & versao == VersaoServico.ve310)
                infNFe.cobr = GetCobranca(infNFe.total.ICMSTot); //V3.00 Somente
            if (infNFe.ide.mod == ModeloDocumento.NFCe)
                infNFe.pag = GetPagamento(infNFe.total.ICMSTot); //NFCe Somente  

            if (infNFe.ide.mod == ModeloDocumento.NFCe)
                infNFe.infAdic = new infAdic() { infCpl = "Troco: " + Troco }; //Susgestão para impressão do troco em NFCe

            return infNFe;
        }

        protected virtual ide GetIdentificacao(int numero, ModeloDocumento modelo, VersaoServico versao)
        {
            var ide = new ide
            {
                cUF = _configuracoes.CfgServico.cUF,
                natOp = "VENDA",
                indPag = IndicadorPagamento.ipVista,
                mod = modelo,
                serie = Convert.ToInt32(ClassGlobals.Global.Term.te_serie_nfce),
                nNF = numero,
                tpNF = TipoNFe.tnSaida,
                cMunFG = Convert.ToInt64(Global.cMun),
                tpEmis = _configuracoes.CfgServico.tpEmis,
                tpImp = TipoImpressao.tiRetrato,
                cNF = "1234",
                tpAmb = _configuracoes.CfgServico.tpAmb,
                finNFe = FinalidadeNFe.fnNormal,
                verProc = "3.000"
            };
            //CONTIGENCIA
            if (ide.tpEmis != TipoEmissao.teNormal)
            {
                ide.dhCont =
                    DateTime.Now.ToString(versao == VersaoServico.ve310
                        ? "yyyy-MM-ddTHH:mm:sszzz"
                        : "yyyy-MM-ddTHH:mm:ss");
                ide.xJust = "TESTE DE CONTIGÊNCIA PARA NFe/NFCe";
            }

            #region V2.00

            if (versao == VersaoServico.ve200)
            {
                ide.dEmi = DateTime.Today.ToString("yyyy-MM-dd"); //Mude aqui para enviar a nfe vinculada ao EPEC, V2.00
                ide.dSaiEnt = DateTime.Today.ToString("yyyy-MM-dd");
            }

            #endregion

            #region V3.00

            if (versao != VersaoServico.ve310) return ide;
            ide.idDest = DestinoOperacao.doInterna;
            ide.dhEmi = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            //Mude aqui para enviar a nfe vinculada ao EPEC, V3.10
            if (ide.mod == ModeloDocumento.NFe)
                ide.dhSaiEnt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            else
                ide.tpImp = TipoImpressao.tiNFCe;
            ide.procEmi = ProcessoEmissao.peAplicativoContribuinte;
            ide.indFinal = ConsumidorFinal.cfConsumidorFinal; //NFCe: Tem que ser consumidor Final
            ide.indPres = PresencaComprador.pcPresencial; //NFCe: deve ser 1 ou 4

            #endregion

            return ide;
        }

        protected virtual emit GetEmitente()
        {
            var emit = _configuracoes.Emitente; // new emit

            emit.enderEmit = GetEnderecoEmitente();
            return emit;
        }

        protected virtual enderEmit GetEnderecoEmitente()
        {
            var enderEmit = _configuracoes.EnderecoEmitente; // new enderEmit

            enderEmit.cPais = 1058;
            enderEmit.xPais = "BRASIL";
            return enderEmit;
        }

        protected virtual dest GetDestinatario(VersaoServico versao, ModeloDocumento modelo)
        {
             var dest = new dest(versao);
             //if (_configuracoes.CfgServico.tpEmis == TipoEmissao.teOffLine)
             //{
             //    //if (Global._dest.xNome != null)
                 //{
                 //    dest.CPF = Global._dest.CPF;
                 //    dest.xNome = Global._dest.xNome;
                 //    endereco = end.First(g => g.cli_id.Equals(Convert.ToInt64(Global._dest.CPF)));
                 //    if (endereco != null)
                 //    {
                 //        dest.enderDest = GetEnderecoDestinatario();
                 //    }
                 //}
                // if (Global._dest.xNome == null && _configuracoes.CfgServico.tpAmb == TipoAmbiente.taHomologacao)
                // {
                     dest.CNPJ = "99999999000191";
                     dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL"; //Obrigatório para NFe e opcional para NFCe
                 //}
            // }
           
             //if (Global._dest.xNome != null)
             //{
             //    if (_configuracoes.CfgServico.tpEmis == TipoEmissao.teNormal)
             //    {
             //        dest.CPF = Global._dest.CPF;
             //        dest.xNome = Global._dest.xNome;
             //        endereco = end.First(g => g.cli_id.Equals(Convert.ToInt64(Global._dest.CPF)));
             //        if (endereco != null)
             //        {
             //            dest.enderDest = GetEnderecoDestinatario();
             //        }
             //    }

             //}
               
                if (versao != VersaoServico.ve310) return dest;
                dest.indIEDest = indIEDest.NaoContribuinte; //NFCe: Tem que ser não contribuinte V3.00 Somente
      
            
            return dest;
        }

        protected virtual enderDest GetEnderecoDestinatario()
        {
            
            var enderDest = new enderDest
            {
                xLgr = endereco.end_nm_logradouro,
                nro = endereco.end_numero.ToString(),
                xBairro = endereco.end_bairro,
                cMun = _configuracoes.EnderecoEmitente.cMun,
                xMun = endereco.end_bairro,
                UF = _configuracoes.EnderecoEmitente.UF,
                CEP = endereco.end_cep,
                cPais = 1058,
                xPais = "BRASIL"
            };
            return enderDest;
        }

        protected virtual det GetDetalhe(int i, CRT crt, ModeloDocumento modelo)
        {
            var det = new det
            {
                nItem = i + 1,
                prod = GetProduto(i + 1),
                imposto = new imposto
                {
                    vTotTrib = 0.17m,
                    ICMS = new ICMS
                    {
                        TipoICMS =
                            crt == CRT.SimplesNacional
                                ? InformarCSOSN(Csosnicms.Csosn102, i)
                                : InformarICMS(Csticms.Cst00, VersaoServico.ve310)
                    },

                    COFINS =
                        new COFINS
                        {
                            TipoCOFINS = new COFINSOutr { CST = CSTCOFINS.cofins99, pCOFINS = 0, vBC = 0, vCOFINS = 0 }
                        },
                    PIS = new PIS { TipoPIS = new PISOutr { CST = CSTPIS.pis99, pPIS = 0, vBC = 0, vPIS = 0 } }
                }
            };

            if (modelo == ModeloDocumento.NFe) //NFCe não aceita grupo "IPI"
                det.imposto.IPI = new IPI()
                {
                    cEnq = 999,
                    TipoIPI = new IPITrib() { CST = CSTIPI.ipi00, pIPI = 5, vBC = 1, vIPI = 0.05m }
                };
            //det.impostoDevol = new impostoDevol() { IPI = new IPIDevolvido() { vIPIDevol = 10 }, pDevol = 100 };

            return det;
        }

        protected virtual prod GetProduto(int i)
        {
            var p = new prod
            {
                cProd = NFCe.lstProd[i - 1].codigoInterno.PadLeft(5, '0'),
                cEAN = NFCe.lstProd[i - 1].codigo,
                xProd = i == 1 ? "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : NFCe.lstProd[i - 1].descricao + i,
                NCM = NFCe.lstProd[i - 1].NCM,
                CFOP = 5102,
                uCom = NFCe.lstProd[i - 1].unidade,
                qCom = NFCe.lstProd[i - 1].qtd,
                vUnCom =  NFCe.lstProd[i - 1].unidade == "KG" ? NFCe.lstProd[i - 1].vKG :  NFCe.lstProd[i - 1].vlrUnit,
                vProd = NFCe.lstProd[i - 1].vlrUnit,
                vDesc = NFCe.lstProd[i - 1].vDesc,
                cEANTrib = NFCe.lstProd[i - 1].codigo,
                uTrib = NFCe.lstProd[i - 1].unidade,
                qTrib = NFCe.lstProd[i - 1].qtd,
                vUnTrib = NFCe.lstProd[i - 1].unidade == "KG" ? NFCe.lstProd[i - 1].vKG : NFCe.lstProd[i - 1].vlrUnit,
                indTot = IndicadorTotal.ValorDoItemCompoeTotalNF,
                //CEST = ?

                //ProdutoEspecifico = new arma
                //{
                //    tpArma = TipoArma.UsoPermitido,
                //    nSerie = "123456",
                //    nCano = "123456",
                //    descr = "TESTE DE ARMA"
                //}
            };
            return p;
        }

        protected virtual ICMSBasico InformarICMS(Csticms CST, VersaoServico versao)
        {
            var icms20 = new ICMS20
            {
                orig = OrigemMercadoria.OmNacional,
                CST = Csticms.Cst20,
                modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                vBC = 1,
                pICMS = 17,
                vICMS = 0.17m,
                motDesICMS = MotivoDesoneracaoIcms.MdiTaxi
            };
            if (versao == VersaoServico.ve310)
                icms20.vICMSDeson = 0.10m; //V3.00 ou maior Somente

            switch (CST)
            {
                case Csticms.Cst00:
                    return new ICMS00
                    {
                        CST = Csticms.Cst00,
                        modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                        orig = OrigemMercadoria.OmNacional,
                        pICMS = 17,
                        vBC = 1,
                        vICMS = 0.17m
                    };
                case Csticms.Cst20:
                    return icms20;
                //Outros casos aqui
            }

            return new ICMS10();
        }

        protected virtual ICMSBasico InformarCSOSN(Csosnicms CST,int numero)
        {

            string teste = NFCe.lstProd[numero].CST;


            switch (CST)
            {
                case Csosnicms.Csosn101:
                    return new ICMSSN101
                    {
                        CSOSN = Csosnicms.Csosn101,
                        orig = OrigemMercadoria.OmNacional
                    };
                case Csosnicms.Csosn102:
                    return new ICMSSN102
                    {
                        CSOSN = Csosnicms.Csosn300,
                        orig = OrigemMercadoria.OmNacional
                    };
                //Outros casos aqui
                default:
                    return new ICMSSN201();
            }
        }

        protected virtual total GetTotal(VersaoServico versao, List<det> produtos)
        {
            var icmsTot = new ICMSTot
            {
                vProd = produtos.Sum(p => p.prod.vProd),
                vNF = produtos.Sum(p => p.prod.vProd) - produtos.Sum(p => p.prod.vDesc ?? 0),
                vDesc = produtos.Sum(p => p.prod.vDesc ?? 0),
                vTotTrib = produtos.Sum(p => p.imposto.vTotTrib ?? 0),
            };
            if (versao == VersaoServico.ve310)
                icmsTot.vICMSDeson = 0;

            foreach (var produto in produtos)
            {
                if (produto.imposto.IPI != null && produto.imposto.IPI.TipoIPI.GetType() == typeof(IPITrib))
                    icmsTot.vIPI = icmsTot.vIPI + ((IPITrib)produto.imposto.IPI.TipoIPI).vIPI ?? 0;
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS00))
                {
                    icmsTot.vBC = icmsTot.vBC + ((ICMS00)produto.imposto.ICMS.TipoICMS).vBC;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS00)produto.imposto.ICMS.TipoICMS).vICMS;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS20))
                {
                    icmsTot.vBC = icmsTot.vBC + ((ICMS20)produto.imposto.ICMS.TipoICMS).vBC;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS20)produto.imposto.ICMS.TipoICMS).vICMS;
                }
                //Outros Ifs aqui, caso vá usar as classes ICMS00, ICMS10 para totalizar
            }

            var t = new total { ICMSTot = icmsTot };
            return t;
        }



        protected virtual transp GetTransporte()
        {
            //var volumes = new List<vol> {GetVolume(), GetVolume()};

            var t = new transp
            {
                modFrete = ModalidadeFrete.mfSemFrete //NFCe: Não pode ter frete
                //vol = volumes 
            };

            return t;
        }

        protected virtual vol GetVolume()
        {
            var v = new vol
            {
                esp = "teste de especia",
                lacres = new List<lacres> { new lacres { nLacre = "123456" } }
            };

            return v;
        }

        protected virtual cobr GetCobranca(ICMSTot icmsTot)
        {
            var valorParcela = Valor.Arredondar(icmsTot.vProd / 2, 2);
            var c = new cobr
            {
                fat = new fat { nFat = "12345678910", vLiq = icmsTot.vProd },
                dup = new List<dup>
                {
                    new dup {nDup = "12345678", vDup = valorParcela},
                    new dup {nDup = "987654321", vDup = icmsTot.vProd - valorParcela}
                }
            };

            return c;
        }

        protected virtual List<pag> GetPagamento(ICMSTot icmsTot)
        {
           decimal valorPagto =  Valor.Arredondar(icmsTot.vProd / 1, 2);
          
            
            var p = new List<pag>
            {
               new pag {tPag = FormaPagamento.fpDinheiro, vPag = valorPagto},
            //    //new pag {tPag = FormaPagamento.fpCartaoDebito, vPag = icmsTot.vProd - valorPagto},
            //    //new pag {tPag = FormaPagamento.fpCartaoDebito, vPag = icmsTot.vProd - valorPagto}
            };
            return lstPag;
        }

        #endregion

        private void UserControl_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                
                Button_Click_1(sender, e);
            }
            if (e.Key == Key.F2)
            {
               
                btDeb_Click(sender, e);
            }
            if (e.Key == Key.F3)
            {

                btCred_Click(sender, e);
            }

            if (e.Key == Key.F4)
            {
                if (TefUser)
                {
                    btTef_Click(sender, e);
                }
            }

            if (e.Key == Key.Enter)
            {

                form.tbCodPro.Focus();
                if (lstPag == null)
                {
                    form.lbStatus.Content = "Não ha valor de pagamento";
                }
                if (lstPag != null &&  Valor.Arredondar(lstPag.Sum(i => i.vPag), 2) >= valorPagto)
                {
                    try
                    {
                        #region Status do serviço
                        bool fecha = false;
                        form.lbStatus.Content = "Aguarde..";
                        //var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                        //var retornoStatus = servicoNFe.NfeStatusServico();
                        _configuracoes.CfgServico.tpEmis = TipoEmissao.teNormal;
                        Troco = (Valor.Arredondar(lstPag.Sum(i => i.vPag), 2) - valorPagto).ToString();
                       
                          foreach (var item in lstPag)
                         {
                            if (item.tPag == FormaPagamento.fpDinheiro)
                            {
                                item.vPag = (item.vPag - Convert.ToDecimal(Troco));
                                if (item.vPag <= 0)
                                {
                                    form.lbStatus.Content = "Somente é permitido dar Troco em Dinheiro";
                                    fecha = false;
                                    break;
                                }
                                if (item.vPag > 0)
                                {
                                    fecha = true;
                                }
                            }

                            fecha = true;
                        }
                          if (fecha)
                          {
                              FechaVenda();
                              LimpaTela();
                              Troco = string.Empty;
                          }
                        #endregion
                    }

                    catch
                    {
                        _configuracoes.CfgServico.tpEmis = TipoEmissao.teOffLine;

                        FechaVenda();
                        LimpaTela();
                        ClassGlobals.CarregaGlobal.SalvaNumero();
                    }
                }
                else
                {
                    form.lbStatus.Content = "Resta: " + (valorPagto - Valor.Arredondar(lstPag.Sum(i => i.vPag) / 1, 2));
                    lstPag.Clear();
                    Keyboard.Focus(this.txtDinheiro);
                    
                }

            }
        }

        private void btDeb_Click(object sender, RoutedEventArgs e)
        {
            Global.Finalizadora = 5;
            FormPag("C. Débito");
            formaPag = FormaPagamento.fpCartaoDebito;
        }

        public bool FormPag(string Forma)
        {
            if (lbDinheiro.Visibility != System.Windows.Visibility.Visible)
            {
                lbDinheiro.Visibility = System.Windows.Visibility.Visible;
                lbDinheiro.Content = Forma;
                txtDinheiro.Visibility = System.Windows.Visibility.Visible;
                txtDinheiro.Focus();
                txtDinheiro.Text = Convert.ToDouble(valorPagto - lstPag.Sum(s => s.vPag)).ToString();
               
                return true;
            }
            
            if (lbPag1.Visibility != System.Windows.Visibility.Visible)
            {
                if (Convert.ToDecimal(txtDinheiro.Text.Replace(",","")) == valorPagto)
                {
                    form.lbStatus.Content = "Redundancia na Forma de Pagamento";
                    return false;
                }
                else
                {
                    lbPag1.Visibility = System.Windows.Visibility.Visible;
                    lbPag1.Content = Forma;
                    txtPag1.Visibility = System.Windows.Visibility.Visible;
                    txtPag1.Focus();
                    txtPag1.Text = Convert.ToDouble(valorPagto - lstPag.Sum(s => s.vPag)).ToString();
                    return true;
                }
            }
            if (lbPag2.Visibility != System.Windows.Visibility.Visible)
            {
                if (Convert.ToDecimal(txtPag1.Text.Replace(",", "")) == valorPagto)
                {
                    form.lbStatus.Content = "Redundancia na Forma de Pagamento";
                    return false;
                }
                else
                {
                    decimal diferenca = ((Convert.ToDecimal(txtDinheiro.Text.Replace(",","")) /100) + (Convert.ToDecimal(txtPag1.Text.Replace(",","")) /100));
                    lbPag2.Visibility = System.Windows.Visibility.Visible;
                    lbPag2.Content = Forma;
                    txtPag2.Visibility = System.Windows.Visibility.Visible;
                    txtPag2.Focus();
                    txtPag2.Text = Convert.ToDouble(valorPagto - lstPag.Sum(s => s.vPag)).ToString();
                    return true;
                }
                    
            }
            return false;
        }

        private void btCred_Click(object sender, RoutedEventArgs e)
        {
            Global.Finalizadora = 3;
            FormPag("C. Crédito");
            formaPag = FormaPagamento.fpCartaoCredito;
        }

        private void txtPag1_KeyDown(object sender, KeyEventArgs e)
        {

           
        }

        private void txtPag1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (txtPag1.Text.Length > 1)
            //{
            //    decimal valor = Convert.ToDecimal(txtPag1.Text);
            //    var valorFormatado = string.Format("{0:N}", valor);
            //    txtPag1.Text = valorFormatado.ToString();
            //}
        }

        private void txtDinheiro_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDecimal(txtDinheiro.Text.Replace(",","").Trim()) >0)
            {
                lstPag.Add(new pag { tPag = formaPag, vPag = Convert.ToDecimal(txtDinheiro.Text.Replace(",", "").Trim()) /100 });
                lbVlrPag.Content = String.Format("{0:N2}",lstPag.Sum(i => i.vPag));
            }
          
        }

        private void txtPag1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDecimal(txtDinheiro.Text.Replace(",", "").Trim()) / 100 > 0)
            {
                lstPag.Add(new pag { tPag = formaPag, vPag = Convert.ToDecimal(txtPag1.Text.Replace(",", "").Trim()) / 100 });
                lbVlrPag.Content = String.Format("{0:N2}", Valor.Arredondar(lstPag.Sum(i => i.vPag) / 1, 2));
            }
        }

        private void txtPag2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Convert.ToDecimal(txtPag2.Text.Replace(",", "").Trim()) > 0)
            {
                lstPag.Add(new pag { tPag = formaPag, vPag = Convert.ToDecimal(txtPag2.Text.Replace(",", "").Trim()) });
                lbVlrPag.Content = String.Format("{0:C}", Valor.Arredondar(lstPag.Sum(i => i.vPag) / 1, 2));
            }
        }

        private void btTef_Click(object sender, RoutedEventArgs e)
        {
            //form.myImage.Visibility = System.Windows.Visibility.Hidden;
            //form.myImage2.Visibility = System.Windows.Visibility.Hidden;
            Global.Finalizadora = 4;
            FormPag("TEF Dedicado");
            form.lbStatus.Content = "Transação TEF";
            SITEF();
           
        }

        public void SITEF()
        {
            ASAsysFwSiTef.Sitef si = new Sitef(Global.Term.tb_unid_negocio.uneg_tef_IP, Global.Term.tb_unid_negocio.uneg_tef_idLoja, "SW" + Global.Term.te_id_terminal.ToString().PadLeft(6, '0'));


            int tre = si.ConfiguraIntSiTefInterativo(Global.Term.tb_unid_negocio.uneg_tef_IP, Global.Term.tb_unid_negocio.uneg_tef_idLoja, "SW" + Global.Term.te_id_terminal.ToString().PadLeft(6, '0'), 0);
            DateTime data = DateTime.Now;
            int tre2 = si.IniciaFuncaoSiTefInterativo(0, valorPagto.ToString(), Global.Term.te_numero_nfce.ToString(), data, Global._Usuario.usr_id.ToString(), "");
            List<string> str = new List<string>();
             imprime = false;
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
               
                if (proximoComando == 1)
                {
                    form.lbStatus.Content = mensagem.Replace('\0', ' ');
                }
                if (proximoComando == 2)
                {
                    form.lbStatus.Content = mensagem.Replace('\0', ' ');
                }
                if (proximoComando == 21)
                {

                    MensagemTela = mensagem;
                    frmDialog testDialog = new frmDialog(this);


                    testDialog.ShowDialog();

                    _buf = Encoding.ASCII.GetBytes(Resp);

                    tre3 = si.ContinuaFuncaoSiTefInterativo(ref proximoComando, ref tipoCampo, ref tamanhoMinimo, ref tamanhoMaximo, _buf, tamanhoBuf, continuaNavegacao);
                    _buf = new byte[tamanhoBuf];
                }
                if (proximoComando == 30)
                {
                    MensagemTela = mensagem;
                    frmDialog testDialog = new frmDialog(this);

                  
                    testDialog.ShowDialog();

                    _buf = Encoding.ASCII.GetBytes(Resp);
                    tre3 = si.ContinuaFuncaoSiTefInterativo(ref proximoComando, ref tipoCampo, ref tamanhoMinimo, ref tamanhoMaximo, _buf, tamanhoBuf, continuaNavegacao);
                    _buf = new byte[tamanhoBuf];

                }
                if (proximoComando == 3 && mensagem.Contains("Transacao OK"))
                {
                    form.lbStatus.Content = "Transacao OK";
                 
                    imprime = true;

                }
                if (imprime && proximoComando == 0)
                {


                    if (mensagem.Contains("VIA-CLIENTE") && CLIENTE == 0)
                    {

                        CLIENTE = 1;
                        Global.TefCliente = mensagem;
                    
                    }
                    if (mensagem.Contains("VIA-ESTABELECIMENTO") && ESTA == 0)
                    {
                        ESTA = 1;
                        Global.TefEstabelecimento = mensagem;

                    }
                    
                }
            }
            if (tre3 < 0)
            {
                form.lbStatus.Content = mensagem.Replace('\0', ' ');
                lstPag.Clear();

                si.FinalizaTransacaoSiTefInterativo(0, Global.Term.te_numero_nfce.ToString(), data);
            }
            if (tre3 >= 0)
            {
                
                si.FinalizaTransacaoSiTefInterativo(1, Global.Term.te_numero_nfce.ToString(), data);
                Global.VendaTef = true;
                lstPag.Add(new pag { tPag = FormaPagamento.fpOutro, vPag = valorPagto });
                lbVlrPag.Content = String.Format("{0:C}", Valor.Arredondar(valorPagto / 1, 2));
                form.lbStatus.Content = "Transacao OK";
                FechaVenda();
                LimpaTela();
            }
           
        }
    }
}
