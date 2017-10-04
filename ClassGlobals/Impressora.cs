using ASArquiteruraData.Repository;
using ASArquiteruraData.RepositoryInterfaces;
using ClassGlobals;
using ClassGlobals.Report;
using CrystalDecisions.CrystalReports.Engine;
using MessagingToolkit.QRCode.Codec;
//using SlidingPanel.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ClassGlobals
{
  public  class Impressora
    {
      public static Suprimento SupriDanfeC = new Suprimento();
      public static Fechamento reportFechamento = new Fechamento();
      public static DanfeCfce reportDanfeC = new DanfeCfce();
      public static DanfeCfce reportDanfeCom = new DanfeCfce();
      public static CancelaNfce reporCancela = new CancelaNfce();
      public static RelatorioGenrencial reporTef = new RelatorioGenrencial();
      public static GerencialGrande reporG = new GerencialGrande();

      public static bool ImprimiCozinha(string texto,string mesa)
      {
          try
          {
              TextObject txtObjectForma = (TextObject)reporG.ReportDefinition.ReportObjects["Text1"];
              TextObject txtObjectCliente = (TextObject)reporG.ReportDefinition.ReportObjects["TextCliente"];
              TextObject txtObjectEmpresa = (TextObject)reporG.ReportDefinition.ReportObjects["TextEmpresa"];
              txtObjectForma.Text = texto;
              txtObjectEmpresa.Text = Global.Term.tb_unid_negocio.uneg_fantasia + "-- CNPJ:" + Global.Term.tb_unid_negocio.uneg_cnpj.ToString() + Environment.NewLine + "Rua: " + Global.Term.tb_unid_negocio.uneg_logradouro + "-- " + Global.Term.tb_unid_negocio.uneg_endereco_numero + Global.Term.tb_unid_negocio.uneg_bairro + Environment.NewLine + " CEP:" + Global.Term.tb_unid_negocio.uneg_cep + "--Tel: " + Global.Term.tb_unid_negocio.uneg_telefones ;
              txtObjectCliente.Text = Environment.NewLine + mesa + " Data: " + DateTime.Now.ToString("dd/MM/yyy HH:mm:ss");
              var configImpressora = new PrinterSettings();
              reporG.Refresh();
              reporG.PrintOptions.PrinterName = configImpressora.PrinterName;
              reporG.PrintToPrinter(1, true, 0, 0);

              reporG.Close();
              return true;
          }
          catch
          {

              return false;
          }
      }
       public static bool ImprimiTef(string texto)
       {
          try
          {
              TextObject txtObjectForma = (TextObject)reporTef.ReportDefinition.ReportObjects["Text1"];

              txtObjectForma.Text = texto;

              var configImpressora = new PrinterSettings();
              reporTef.Refresh();
              reporTef.PrintOptions.PrinterName = configImpressora.PrinterName;
              reporTef.PrintToPrinter(1, true, 0, 0);

              reporTef.Close();
              return true;
          }
          catch
          {

              return false;
          }
      }
      public static bool ImprimiCancelamento(string chave, string valor, string numero, string auto)
      {
          try
          {
              TextObject txtObjectForma = (TextObject)reporCancela.ReportDefinition.ReportObjects["textoNfce"];

              txtObjectForma.Text = "Chave da NFce: " + chave + Environment.NewLine + "Número Venda: " + numero + Environment.NewLine + "Valor Venda: " + FormatValueForXML(Convert.ToDecimal(valor)) + Environment.NewLine+"Autorizador: "+auto;

              var configImpressora = new PrinterSettings();
              reporCancela.Refresh();
              reporCancela.PrintOptions.PrinterName = configImpressora.PrinterName;
              reporCancela.PrintToPrinter(1, true, 0, 0);

              reporCancela.Close();
              return true;
          }
          catch
          {

              return false;
          }
      }
      public static bool SuprimentoSangria(string titulo,string auto,string valor,string forma)
      {
          try
          {
              TextObject txtObjectTitulo = (TextObject)SupriDanfeC.ReportDefinition.ReportObjects["txtTitulo"];
              TextObject txtObjectAut = (TextObject)SupriDanfeC.ReportDefinition.ReportObjects["txtAut"];
              TextObject txtObjectVlr = (TextObject)SupriDanfeC.ReportDefinition.ReportObjects["txtVlr"];
              TextObject txtObjectForma = (TextObject)SupriDanfeC.ReportDefinition.ReportObjects["txtForma"];
              txtObjectTitulo.Text = titulo;
              txtObjectAut.Text = auto;
              txtObjectVlr.Text = valor;
              txtObjectForma.Text = forma;

              var configImpressora = new PrinterSettings();
              SupriDanfeC.Refresh();
              SupriDanfeC.PrintOptions.PrinterName = configImpressora.PrinterName;
              SupriDanfeC.PrintToPrinter(1, true, 0, 0);

              SupriDanfeC.Close();
              return true;
          }
          catch
          {

              return false;
          }
      }
      public static bool imprimeFechamento()
      {
          Itb_sangriasRepository sanresp = new tb_sangriasRepository();
          Itb_suprimentosRepository supresp = new tb_suprimentosRepository();
          try
          {
              TextObject TextHoraAbertura = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextHoraAbertura"];
              TextObject TextVendasFeitas = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextVendasFeitas"];
              TextObject TextVendasCanceladas = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextVendasCanceladas"];
              TextObject TextSangrias = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextSangrias"];
              TextObject TextSuprimentos = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextSuprimentos"];
              TextObject TextTotalVendas = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextTotalVendas"];
              TextObject TextTotalSangrias = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextTotalSangrias"];
              TextObject TextTotalSuprimentos = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextTotalSuprimentos"];
              TextObject TextNumeroCaixa = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextNumeroCaixa"];
              TextObject TextNumeroNota = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextNumeroNota"];
              TextObject TextVendaCancelada = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextTotalVendaCancelada"];
                TextObject TextTotalemCaixa = (TextObject)reportFechamento.ReportDefinition.ReportObjects["TextTotalemCaixa"];
              
          

              TextHoraAbertura.Text = Global._aCaixa.aberturaCx_dt_abertura.ToString();
              TextVendasFeitas.Text = Global.vendaResp.Find(s=>s.venda_data >= Global._aCaixa.aberturaCx_dt_abertura).Count().ToString();
              TextVendasCanceladas.Text = Global.vendaResp.Find(s => s.venda_data >= Global._aCaixa.aberturaCx_dt_abertura && s.venda_status.Equals("CN")).Count().ToString();
              
              TextSangrias.Text = sanresp.Find(s => s.sangriaf_data >= Global._aCaixa.aberturaCx_dt_abertura).Count().ToString();
            
              TextSuprimentos.Text =   supresp.Find(s => s.suprimentof_data >= Global._aCaixa.aberturaCx_dt_abertura).Count().ToString();;
              decimal val = Convert.ToDecimal(Global.vendaResp.Find(s => s.venda_data >= Global._aCaixa.aberturaCx_dt_abertura && s.venda_status.Equals("FN")).Sum(s => s.venda_tot_valor));
              TextTotalVendas.Text = FormatValueForXML(val);
              decimal valCanc = Convert.ToDecimal(Global.vendaResp.Find(s => s.venda_data >= Global._aCaixa.aberturaCx_dt_abertura && s.venda_status.Equals("CN")).Sum(s => s.venda_tot_valor));
              TextVendaCancelada.Text = FormatValueForXML(valCanc);
              decimal valSangria = Convert.ToDecimal(sanresp.Find(s => s.sangriaf_data >= Global._aCaixa.aberturaCx_dt_abertura).Sum(s=>s.sangriaf_valor));
              TextTotalSangrias.Text = FormatValueForXML(valSangria);
              decimal valSuprimento = Convert.ToDecimal(supresp.Find(s => s.suprimentof_data >= Global._aCaixa.aberturaCx_dt_abertura).Sum(s => s.suprimentof_valor));
              TextTotalSuprimentos.Text = FormatValueForXML(valSuprimento);
              TextNumeroCaixa.Text = Global.Term.te_id_terminal.ToString();
              TextNumeroNota.Text = Global.Term.te_numero_nfce.ToString();
              decimal TotalCaixa = (val + valSuprimento)-(valSangria + valCanc);
              TextTotalemCaixa.Text = FormatValueForXML(TotalCaixa);
              var configImpressora = new PrinterSettings();
              reportFechamento.Refresh();
              reportFechamento.PrintOptions.PrinterName = configImpressora.PrinterName;
              reportFechamento.PrintToPrinter(1, true, 0, 0);

              reportDanfeC.Close();
              return true;
          }
          catch
          {

              return false;
          }
      }
      public static string _path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      public static bool ImprimirDanferCom(string caminho)
      {


          try
          {
              #region QRCODE
              XmlDocument xmlDoc = new XmlDocument();
              xmlDoc.Load(caminho); //Carregando o arquivo

              string qrcode = xmlDoc.GetElementsByTagName("qrCode")[0].InnerText.ToString();
              string tipEmiss = xmlDoc.GetElementsByTagName("tpEmis")[0].InnerText.ToString();

              QRCodeEncoder qrCodecEncoder = new QRCodeEncoder();
              qrCodecEncoder.QRCodeBackgroundColor = System.Drawing.Color.White;
              qrCodecEncoder.QRCodeForegroundColor = System.Drawing.Color.Black;

              qrCodecEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
              qrCodecEncoder.QRCodeScale = 6;
              qrCodecEncoder.QRCodeVersion = 0;
              qrCodecEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;

              Image imageQRCode;
              imageQRCode = qrCodecEncoder.Encode(qrcode);
              imageQRCode.Save("C:\\NFe\\Report\\QRcode.png");

              #endregion

              #region IMPRESSAO DANFE NFCE


              DataSet dsEmployees = new DataSet();
              dsEmployees.ReadXml(caminho);
              reportDanfeCom.SetDataSource(dsEmployees);
              TextObject txtObjectPag = (TextObject)reportDanfeCom.ReportDefinition.ReportObjects["TxtVlrTotPag"];
              TextObject txtObjectVacres = (TextObject)reportDanfeCom.ReportDefinition.ReportObjects["TxtVlrAcres"];
              TextObject txtObjectTroco = (TextObject)reportDanfeCom.ReportDefinition.ReportObjects["TxtVlrTroco"];
              TextObject txtObjectDesc = (TextObject)reportDanfeCom.ReportDefinition.ReportObjects["TxtvDesc"];
              txtObjectPag.Text = FormatValueForXML(Global._VlrTotalNFce);
              txtObjectVacres.Text = FormatValueForXML(Global._VlrOutros);
              txtObjectTroco.Text = FormatValueForXML(Global._VlrTrocolNFce);
              txtObjectDesc.Text = FormatValueForXML(Global._VlrDescNFce);

              var configImpressora = new PrinterSettings();
              reportDanfeCom.Refresh();
              reportDanfeCom.PrintOptions.PrinterName = configImpressora.PrinterName;
              reportDanfeCom.PrintToPrinter(1, true, 0, 0);

              reportDanfeCom.Close();
              dsEmployees.Dispose();


              #endregion

              Global._VlrTotalNFce = 0;
              Global._VlrTrocolNFce = 0;
              Global._VlrDescNFce = 0;
              Global._VlrNnF = 0;
              Global._VlrOutros = 0;
              Global._VlrOutrosItens = 0;

              return true;
          }
          catch (Exception es)
          {

              return false;
          }
      }
      
      public static bool ImprimirDanferE(string caminho)
      {


          try
          {
              #region QRCODE
              XmlDocument xmlDoc = new XmlDocument();
              xmlDoc.Load(caminho); //Carregando o arquivo

              string qrcode = xmlDoc.GetElementsByTagName("qrCode")[0].InnerText.ToString();
              string tipEmiss = xmlDoc.GetElementsByTagName("tpEmis")[0].InnerText.ToString();

              QRCodeEncoder qrCodecEncoder = new QRCodeEncoder();
              qrCodecEncoder.QRCodeBackgroundColor = System.Drawing.Color.White;
              qrCodecEncoder.QRCodeForegroundColor = System.Drawing.Color.Black;

              qrCodecEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
              qrCodecEncoder.QRCodeScale = 6;
              qrCodecEncoder.QRCodeVersion = 0;
              qrCodecEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;

              Image imageQRCode;
              imageQRCode = qrCodecEncoder.Encode(qrcode);
              imageQRCode.Save("C:\\NFe\\Report\\QRcode.png");

              #endregion

              #region IMPRESSAO DANFE NFCE
            

                      DataSet dsEmployees = new DataSet();
                      dsEmployees.ReadXml(caminho);
                      reportDanfeC.SetDataSource(dsEmployees);
                      TextObject txtObjectPag = (TextObject)reportDanfeC.ReportDefinition.ReportObjects["TxtVlrTotPag"];
                      TextObject txtObjectVacres = (TextObject)reportDanfeC.ReportDefinition.ReportObjects["TxtVlrAcres"];
                      TextObject txtObjectTroco = (TextObject)reportDanfeC.ReportDefinition.ReportObjects["TxtVlrTroco"];
                      TextObject txtObjectDesc = (TextObject)reportDanfeC.ReportDefinition.ReportObjects["TxtvDesc"];
                      txtObjectPag.Text = FormatValueForXML(Global._VlrTotalNFce );
                      txtObjectVacres.Text = FormatValueForXML(Global._VlrOutros);
                      txtObjectTroco.Text = FormatValueForXML(Global._VlrTrocolNFce);
                      txtObjectDesc.Text = FormatValueForXML(Global._VlrDescNFce);

                      var configImpressora = new PrinterSettings();
                      reportDanfeC.Refresh();
                      reportDanfeC.PrintOptions.PrinterName = configImpressora.PrinterName;
                      reportDanfeC.PrintToPrinter(1, true, 0, 0);

                      reportDanfeC.Close();
                      dsEmployees.Dispose();


              #endregion

              Global._VlrTotalNFce = 0;
              Global._VlrTrocolNFce = 0;
              Global._VlrDescNFce = 0;
              Global._VlrNnF = 0;
              Global._VlrOutros = 0;
              Global._VlrOutrosItens = 0;

              return true;
          }
          catch(Exception es)
          {

              return false;
          }
      }
      
      private static string FormatValueForXML(decimal value)
      {
          string valueRetorno = string.Empty;
          try
          {
              //1º RETIRAR CASAS DECIMAIS
              decimal numero = Decimal.Round(Decimal.Zero, 2);
              numero = Decimal.Floor(value);
              valueRetorno = numero.ToString();

              //2º PEGAR DIZIMA 0,95000
              decimal dizima = (numero - value) * -1;
              if (dizima > 0)
              {
                  string dizimaString = dizima.ToString();
                  dizimaString = (dizimaString.Substring(dizimaString.IndexOf(','), dizimaString.Length - dizimaString.IndexOf(','))).Replace(",", "");
                  switch (dizimaString.Length)
                  {
                      case 1:
                          dizimaString = "." + dizimaString + "0";
                          break;
                      case 2:
                          dizimaString = "." + dizimaString;
                          break;
                      case 3:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 4:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 5:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 6:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 7:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 8:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 9:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      case 10:
                          dizimaString = "." + dizimaString.Substring(0, 2);
                          break;
                      default:
                          break;
                  }
                  valueRetorno += dizimaString;
              }
              else
                  valueRetorno += ".00";
          }
          catch (Exception ex)
          {

          }
          return valueRetorno;
      }
    }
}
