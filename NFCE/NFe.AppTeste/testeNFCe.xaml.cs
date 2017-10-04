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
using NFe.Servicos;
using NFe.Servicos.Retorno;
using NFe.Utils;
using NFe.Utils.Assinatura;
using NFe.Utils.NFe;
using NFe.Utils.Assinatura;
using RichTextBox = System.Windows.Controls.RichTextBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using WebBrowser = System.Windows.Controls.WebBrowser;
using NFe.Impressao.NFCe;
using ClassGlobals;
namespace NFe.AppTeste
{
    /// <summary>
    /// Interaction logic for testeNFCe.xaml
    /// </summary>
    public partial class testeNFCe : System.Windows.Controls.UserControl 
    {
        private const string ArquivoConfiguracao = @"\configuracao.xml";
        private const string TituloErro = "Erro";
        private ConfiguracaoApp _configuracoes;
        private Classes.NFe _nfe;
        private readonly string _path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public testeNFCe()
        {
           
            InitializeComponent();
            CarregarConfiguracao();

            DataContext = _configuracoes;

            
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

        #region Tratamento de retornos dos Serviços

        internal void RetornoDados<T>(T objeto, RichTextBox richTextBox) where T : class
        {
            richTextBox.Document.Blocks.Clear();

            foreach (var atributos in Funcoes.LerPropriedades(objeto))
            {
                richTextBox.AppendText(atributos.Key + " = " + atributos.Value + "\r");
            }
        }

        internal void RetornoCompletoStr(RichTextBox richTextBox, string retornoCompletoStr)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText(retornoCompletoStr);
        }

        internal void EnvioStr(RichTextBox richTextBox, string envioStr)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText(envioStr);
        }

        internal void RetornoXml(WebBrowser webBrowser, string retornoXmlString)
        {
            var stw = new StreamWriter(_path + @"\tmp.xml");
            stw.WriteLine(retornoXmlString);
            stw.Close();
            webBrowser.Navigate(_path + @"\tmp.xml");
        }

        internal void RetornoStr(RichTextBox richTextBox, string retornoXmlString)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.AppendText(retornoXmlString);
        }

        #endregion
        private void TrataRetorno(RetornoBasico retornoBasico)
        {
            EnvioStr(RtbEnvioStr, retornoBasico.EnvioStr);
            RetornoStr(RtbEnvioStr, retornoBasico.RetornoStr);
            RetornoXml(WebXmlRetorno, retornoBasico.RetornoStr);
            RetornoCompletoStr(RtbRetornoCompletoStr, retornoBasico.RetornoCompletoStr);
            RetornoDados(retornoBasico.Retorno, RtbDadosRetorno);
        }







        private void BtnStatusServico_Click(object sender, RoutedEventArgs e)
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

        private void BtnInutiliza_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnImportarXml_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConsultaEpec_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelarNFe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCriareEnviar2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConsultarReciboLote2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnGerarNfe2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdicionaNfeproc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var arquivoXml = Funcoes.BuscarArquivoXml();
                var nfe = new Classes.NFe().CarregarDeArquivoXml(arquivoXml);
                var chave = nfe.infNFe.Id.Substring(3);

                if (string.IsNullOrEmpty(chave)) throw new Exception("A Chave da NFe não foi encontrada no arquivo!");
                if (chave.Length != 44) throw new Exception("Chave deve conter 44 caracteres!");

                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoConsulta = servicoNFe.NfeConsultaProtocolo(chave);
                TrataRetorno(retornoConsulta);

                var nfeproc = new nfeProc
                {
                    NFe = nfe,
                    protNFe = retornoConsulta.Retorno.protNFe,
                    versao = retornoConsulta.Retorno.versao
                };
                var novoArquivo = _path+"\\Autorizados\\" + @"\" + nfeproc.protNFe.infProt.chNFe +
                                  "-procNfe.xml";
                FuncoesXml.ClasseParaArquivoXml(nfeproc, novoArquivo);
                Funcoes.Mensagem("Arquivo salvo em " + novoArquivo, "Atenção", MessageBoxButton.OK);
                Impressora.ImprimirDanferE(novoArquivo);
            
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }
        public void Prot(string caminho)
        {
            try
            {
                var arquivoXml = caminho;
                var nfe = new Classes.NFe().CarregarDeArquivoXml(arquivoXml);
                var chave = nfe.infNFe.Id.Substring(3);

                if (string.IsNullOrEmpty(chave)) throw new Exception("A Chave da NFe não foi encontrada no arquivo!");
                if (chave.Length != 44) throw new Exception("Chave deve conter 44 caracteres!");

                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoConsulta = servicoNFe.NfeConsultaProtocolo(chave);
                TrataRetorno(retornoConsulta);

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
                Impressora.ImprimirDanferE(novoArquivo);

            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        
        }
        private void BtnValida_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCriareEnviar3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Cria e Envia NFe

                var numero = Funcoes.InpuBox("Criar e Enviar NFe", "Número da Nota:");
                if (string.IsNullOrEmpty(numero)) throw new Exception("O Número deve ser informado!");

                var lote = Funcoes.InpuBox("Criar e Enviar NFe", "Id do Lote:");
                if (string.IsNullOrEmpty(lote)) throw new Exception("A Id do lote deve ser informada!");

                _nfe = GetNf(Convert.ToInt32(numero), _configuracoes.CfgServico.ModeloDocumento,
                    _configuracoes.CfgServico.VersaoNFeAutorizacao);
                _nfe.Assina(); //não precisa validar aqui, pois o lote será validado em ServicosNFe.NFeAutorizacao
                //A URL do QR-Code deve ser gerada em um objeto nfe já assinado, pois na URL vai o DigestValue que é gerado por ocasião da assinatura
                //Descomente a linha abaixo se a SEFAZ de sua UF já habilitou a NT2015.002
                _nfe.infNFeSupl = new infNFeSupl() { qrCode = EnderecadorDanfeNfce.ObterUrlQrCode(_nfe, _configuracoes.ConfiguracaoDanfeNfce) }; //Define a URL do QR-Code.
                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoEnvio = servicoNFe.NFeAutorizacao(Convert.ToInt32(lote), IndicadorSincronizacao.Assincrono, new List<Classes.NFe> { _nfe }, true/*Envia a mensagem compactada para a SEFAZ*/);

                TrataRetorno(retornoEnvio);
                if (retornoEnvio.Retorno.infRec.nRec != string.Empty)
                {
                    try
                    {
                        #region Consulta Recibo de lote

                        
                        servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                        var retornoRecibo = servicoNFe.NFeRetAutorizacao(retornoEnvio.Retorno.infRec.nRec);

                        TrataRetorno(retornoRecibo);
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
                        _nfe.SalvarArquivoXml(_path+"\\NFCe\\"+ch+".xml");
                        Prot(_path + "\\NFCe\\"+ch+".xml");
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        if (!string.IsNullOrEmpty(ex.Message))
                            Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }

        private void BtnGerarNfe3_Click(object sender, RoutedEventArgs e)
        {
            GeranNfe(_configuracoes.CfgServico.VersaoNFeAutorizacao, _configuracoes.CfgServico.ModeloDocumento);
        }
        private void GeranNfe(VersaoServico versaoServico, ModeloDocumento modelo)
        {
            try
            {
                #region Gerar NFe

                var numero = Funcoes.InpuBox("Criar e Enviar NFe", "Número da Nota:");
                if (string.IsNullOrEmpty(numero)) throw new Exception("O Número deve ser informado!");

                _nfe = GetNf(Convert.ToInt32(numero), modelo, versaoServico);
                _nfe.Assina();
                //Descomente a linha abaixo se a SEFAZ de sua UF já habilitou a NT2015.002
                _nfe.infNFeSupl = new infNFeSupl() { qrCode = EnderecadorDanfeNfce.ObterUrlQrCode(_nfe, _configuracoes.ConfiguracaoDanfeNfce) };
                _nfe.Valida();

                #endregion

                ExibeNfe();

                var dlg = new SaveFileDialog
                {
                    FileName = _nfe.infNFe.Id.Substring(3),
                    DefaultExt = ".xml",
                    Filter = "Arquivo XML (.xml)|*.xml"
                };
                var result = dlg.ShowDialog();
                if (result != true) return;
                var arquivoXml = dlg.FileName;
                _nfe.SalvarArquivoXml(arquivoXml);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }
        private void ExibeNfe()
        {
            _nfe.SalvarArquivoXml(_path + @"\tmp.xml");
            WebXmlNfe.Navigate(_path + @"\tmp.xml");
            TabItemNfe.IsSelected = true;
        }
        #region Criar NFe

        protected virtual Classes.NFe GetNf(int numero, ModeloDocumento modelo, VersaoServico versao)
        {
            var nf = new Classes.NFe { infNFe = GetInf(numero, modelo, versao) };
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
                infNFe.infAdic = new infAdic() { infCpl = "Troco: 10,00" }; //Susgestão para impressão do troco em NFCe

            return infNFe;
        }

        protected virtual ide GetIdentificacao(int numero, ModeloDocumento modelo, VersaoServico versao)
        {
            var ide = new ide
            {
                cUF = Estado.RJ,
                natOp = "VENDA",
                indPag = IndicadorPagamento.ipVista,
                mod = modelo,
                serie = 1,
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
            var dest = new dest(versao)
            {
                CNPJ = "99999999000191",
                //CPF = "99999999999",
            };
            if (modelo == ModeloDocumento.NFCe)
            {
                dest.xNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL"; //Obrigatório para NFe e opcional para NFCe
                dest.enderDest = GetEnderecoDestinatario(); //Obrigatório para NFe e opcional para NFCe
            }

            //if (versao == VersaoServico.ve200)
            //    dest.IE = "ISENTO";
            if (versao != VersaoServico.ve310) return dest;
            dest.indIEDest = indIEDest.NaoContribuinte; //NFCe: Tem que ser não contribuinte V3.00 Somente
            dest.email = "teste@gmail.com"; //V3.00 Somente
            return dest;
        }

        protected virtual enderDest GetEnderecoDestinatario()
        {
            var enderDest = new enderDest
            {
                xLgr = "RUA ...",
                nro = "S/N",
                xBairro = "CENTRO",
                cMun = 2802908,
                xMun = "ITABAIANA",
                UF = "SE",
                CEP = "49500000",
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
                                ? InformarCSOSN(Csosnicms.Csosn102)
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
                cProd = NFCe.lstProd[i-1].codigoInterno.PadLeft(5, '0'),
                cEAN = NFCe.lstProd[i - 1].codigo,
                xProd = i == 1 ? "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : NFCe.lstProd[i-1].descricao + i,
                NCM = NFCe.lstProd[i-1].NCM,
                CFOP = 5102,
                uCom = NFCe.lstProd[i-1].unidade,
                qCom = NFCe.lstProd[i - 1].qtd,
                vUnCom = NFCe.lstProd[i - 1].vlrUnit,
                vProd = NFCe.lstProd[i - 1].vlrUnit,
                vDesc = 0.00m,
                cEANTrib = NFCe.lstProd[i - 1].codigo,
                uTrib = NFCe.lstProd[i - 1].unidade,
                qTrib = NFCe.lstProd[i - 1].qtd,
                vUnTrib = NFCe.lstProd[i - 1].vlrUnit,
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

        protected virtual ICMSBasico InformarCSOSN(Csosnicms CST)
        {
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
                        CSOSN = Csosnicms.Csosn102,
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
            var valorPagto = Valor.Arredondar(icmsTot.vProd / 1, 2);
            var p = new List<pag>
            {
                new pag {tPag = FormaPagamento.fpDinheiro, vPag = valorPagto},
                //new pag {tPag = FormaPagamento.fpCartaoDebito, vPag = icmsTot.vProd - valorPagto},
                //new pag {tPag = FormaPagamento.fpCartaoDebito, vPag = icmsTot.vProd - valorPagto}
            };
            return p;
        }

        #endregion
        private void BtnDownlodNfe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConsultaCadastro_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConsultaXml_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConsultaChave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEnviaEpec_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdminCsc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnNfceDanfeOff_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCartaCorrecao_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAssina_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCarregaXmlEnvia_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnNfceDanfe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConsultarReciboLote3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region Consulta Recibo de lote

                var recibo = Funcoes.InpuBox("Consultar processamento de lote de NF-e", "Número do recibo:");
                if (string.IsNullOrEmpty(recibo)) throw new Exception("O número do recibo deve ser informado!");
                var servicoNFe = new ServicosNFe(_configuracoes.CfgServico);
                var retornoRecibo = servicoNFe.NFeRetAutorizacao(recibo);

                TrataRetorno(retornoRecibo);

                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    Funcoes.Mensagem(ex.Message, "Erro", MessageBoxButton.OK);
            }
        }
    }
}
