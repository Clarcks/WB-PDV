using ClassGlobals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Xml;

namespace SlidingPanel.Classes
{
  public  class processaXml
    {

        public static decimal _vlrTotVicms = 0;
        public static decimal _vlrTotBCVicms = 0;
        public static decimal valorTotal = 0;
        public static string caminhoInstall = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static bool PingContigencia()
        {
            try
            {
                Ping pingSender = new Ping();
                IPAddress address = IPAddress.Loopback;
                PingReply reply = pingSender.Send("google.com.br");

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {

                return false;
            }

            return false;

        }

        public static bool GerarArquivoXml(List<objNota.xPag> lstPag, string rodape, List<objNota.Produto> lst, List<objNota.Xdest> lstDest)
        {
            string caminhoNFCe = string.Empty;
            valorTotal = 0;
            int ano = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(2, 2));
            int mes = Convert.ToInt32(DateTime.Now.Month.ToString());
            if (!PingContigencia())
            {
                Global.tpEmis = "9";

            }
            if (PingContigencia())
            {
                Global.tpEmis = "1";
            }
            if (Global.tpEmis == "9")
                caminhoNFCe = Global.PastaValidar;

            if (Global.tpEmis != "9")
            {
                caminhoNFCe = @"C:\\ASAsys\\ASAsysNFCE";
            }
            string Chave = montaChaveAcessoNFe(Global.UnidadeFederativaCodigo, ano, mes, Global.CNPJ, Global.Serie, Global.Nota, Global.tpEmis);
            try
            {
                UTF8Encoding _utf8 = new UTF8Encoding();
                XmlTextWriter _xml = new XmlTextWriter(caminhoNFCe + "\\" + Chave + "-nfe.xml", _utf8);
                _xml.WriteStartDocument();
                #region NFe
                _xml.WriteStartElement("NFe", "http://www.portalfiscal.inf.br/nfe");// TAG raiz da NF-e, Nó com tipo de

                #region infNFe
                _xml.WriteStartElement("infNFe");//Grupo que contém as informações da NF-e;

                _xml.WriteStartAttribute("Id");// informar a chave de acesso da

                _xml.WriteString("NFe" + Chave);
                _xml.WriteEndAttribute();

                _xml.WriteStartAttribute("versao");////Versão do leiaute (v2.0),Atributos do Nó
                _xml.WriteString("3.10");//vesao da nfe.
                _xml.WriteEndAttribute();

                #region ide
                _xml.WriteStartElement("ide");//ide (inserida como filha na tag infNFe)
                _xml.WriteStartElement("cUF");//Código da UF do emitente do Documento Fiscal. Utilizar a Tabela do IBGE de código de unidades da federação (Anexo IV - Tabela de UF, Município e País).
                _xml.WriteString(Global.UnidadeFederativaCodigo);// Código da Uf, tam. 2
                _xml.WriteEndElement();

                _xml.WriteStartElement("cNF");//Código numérico que compõe a Chave de Acesso. Número aleatório gerado pelo emitente para cada NF-e para evitar acessos indevidos da NF-e.(v2.0)
                _xml.WriteString(Chave.Substring(35, 8));// Código da Nf, tam. 8
                _xml.WriteEndElement();

                _xml.WriteStartElement("natOp");//Descrição da Natureza da Operação
                _xml.WriteString("VENDAS DE PRODUCAO DO ESTABELECIMENTO");// Tam. 1-60
                _xml.WriteEndElement();

                _xml.WriteStartElement("indPag");//Indicador forma de pagamento; 0 – pagamento à vista;1 – pagamento à prazo;2 - outros.
                _xml.WriteString("0");// tam. 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("mod");//Código do Modelo do Documento Fiscal
                _xml.WriteString("65");// tam. 2; Utilizar o código 55 para identificação da NF-e, emitida em substituição ao modelo 1 ou 1A.
                _xml.WriteEndElement();

                _xml.WriteStartElement("serie");//Série do Documento Fiscal
                _xml.WriteString(Global.Serie);// Tam 1-3
                _xml.WriteEndElement();

                _xml.WriteStartElement("nNF");//Número do Documento Fiscal
                _xml.WriteString(Global.Nota);// Tam. 1 - 9
                _xml.WriteEndElement();

                _xml.WriteStartElement("dhEmi");//Data de emissão do Documento Fiscal
                _xml.WriteString(String.Format("{0:yyyy-MM-ddTHH:mm:ss}", DateTime.Now) + "-03:00");// Formato “AAAA-MM-DD”
                _xml.WriteEndElement();


                _xml.WriteStartElement("tpNF");//Tipo de Operação
                _xml.WriteString("1");// 0-entrada / 1-saída, tam. 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("idDest");//Código do Município de Ocorrência do Fato Gerador
                _xml.WriteString("1");// tam. 7
                _xml.WriteEndElement();

                _xml.WriteStartElement("cMunFG");//Código do Município de Ocorrência do Fato Gerador
                _xml.WriteString(Global.cMunFG);// tam. 7
                _xml.WriteEndElement();


                _xml.WriteStartElement("tpImp");//Formato de Impressão do DANFE
                _xml.WriteString("4");//1-Retrato/ 2-Paisagem
                _xml.WriteEndElement();

                _xml.WriteStartElement("tpEmis");//Tipo de Emissão da NF-e (normal, contingencia, ver manual)
                _xml.WriteString(Global.tpEmis);// tam. 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("cDV");//Dígito Verificador da Chave de Acesso da NF-e
                _xml.WriteString(Chave.Substring(43, 1));// tam. 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("tpAmb");//Tipo de ambiente,1-Produção/ 2-Homologação
                _xml.WriteString(Global.AmbienteCodigo);// Tam 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("finNFe");//Finalidade de emissão da NFe,1- NF-e normal/ 2-NF-e complementar / 3 – NF-e de ajuste
                _xml.WriteString("1");// Finalidade da Nfe
                _xml.WriteEndElement();

                _xml.WriteStartElement("indFinal");//Finalidade de emissão da NFe,1- NF-e normal/ 2-NF-e complementar / 3 – NF-e de ajuste
                _xml.WriteString("1");// Finalidade da Nfe
                _xml.WriteEndElement();

                _xml.WriteStartElement("indPres");//Finalidade de emissão da NFe,1- NF-e normal/ 2-NF-e complementar / 3 – NF-e de ajuste
                _xml.WriteString("1");// Finalidade da Nfe
                _xml.WriteEndElement();

                _xml.WriteStartElement("procEmi");//Processo de emissão da NF-e
                _xml.WriteString("0");// Tam 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("verProc");//Versão do Processo de emissão da NF-e
                _xml.WriteString("ASAsysNFCe");// tam. 1-20
                _xml.WriteEndElement();

                if (Global.tpEmis == "9")
                {
                    _xml.WriteStartElement("dhCont");//Processo de emissão da NF-e
                    _xml.WriteString(String.Format("{0:yyyy-MM-ddTHH:mm:ss}", DateTime.Now) + "-03:00");// Tam 1
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("xJust");//Versão do Processo de emissão da NF-e
                    _xml.WriteString("Entrada em Contigencia por falta de comunicação com a SEFAZ");// tam. 1-20
                    _xml.WriteEndElement();
                }



                _xml.WriteEndElement();//ide
                //fim ide
                #endregion

                #region emit
                _xml.WriteStartElement("emit");//ide

                _xml.WriteStartElement("CNPJ");// CNPJ Emitente
                _xml.WriteString(Global.CNPJ);// tam 14
                _xml.WriteEndElement();

                _xml.WriteStartElement("xNome");//Razão Social ou Nome do emitente
                _xml.WriteString(Global.xNome);// tam. 2-60
                _xml.WriteEndElement();

                _xml.WriteStartElement("xFant");//Nome fantasia
                _xml.WriteString(Global.xFant);// tam. 1-60
                _xml.WriteEndElement();
                #region enderEmit
                _xml.WriteStartElement("enderEmit");//Tag codigo Uf

                _xml.WriteStartElement("xLgr");//Logradouro
                _xml.WriteString(Global.xLgr);//tam. 2-60
                _xml.WriteEndElement();

                _xml.WriteStartElement("nro");//Número
                _xml.WriteString(Global.Nro);// 1-60
                _xml.WriteEndElement();
                //ocorrencia 0-1


                _xml.WriteStartElement("xBairro");//Bairro
                _xml.WriteString(Global.xBairro);// tam. 2-60
                _xml.WriteEndElement();

                _xml.WriteStartElement("cMun");//Código do município
                _xml.WriteString(Global.cMun);// 1
                _xml.WriteEndElement();

                _xml.WriteStartElement("xMun");//Nome do município
                _xml.WriteString(Global.xMun);// tam. 2-60
                _xml.WriteEndElement();

                _xml.WriteStartElement("UF");//Sigla da UF
                _xml.WriteString(Global.UF);// tam. 2
                _xml.WriteEndElement();

                _xml.WriteStartElement("CEP");//Código do CEP, Informar os zeros não significativos.
                _xml.WriteString(Global.CEP);// tam. 8
                _xml.WriteEndElement();

                _xml.WriteStartElement("cPais");//Código do País
                _xml.WriteString("1058");// tam. 4 1058 - Brasil
                _xml.WriteEndElement();

                _xml.WriteStartElement("xPais");//Nome do País
                _xml.WriteString("Brasil");// tam. 1-60; Brasil ou BRASIL
                _xml.WriteEndElement();

                _xml.WriteStartElement("fone");//Telefone
                _xml.WriteString(Global.telefone);// tam. 6-14 - Preencher com o Código DDD + número do telefone.
                //Nas operações com exterior é permitido informar o código do país + código da localidade + número do telefone (v.2.0)
                _xml.WriteEndElement();
                _xml.WriteEndElement();//enderEmit
                #endregion

                _xml.WriteStartElement("IE");//IE
                _xml.WriteString(Global.IE);// tam. 0-14
                _xml.WriteEndElement();
                //Ocorrencia 0-1
                _xml.WriteStartElement("CRT");//Código de Regime Tributário
                //Este campo será obrigatoriamente preenchido com:
                //1 – Simples Nacional;
                //2 – Simples Nacional – excesso de sublimite de receita bruta;
                //3 – Regime Normal. (v2.0).
                _xml.WriteString(Global.CRT);// Tam. 1
                _xml.WriteEndElement();
                _xml.WriteEndElement();//emit
                #endregion

                #region dest

                if (lstDest.Count != 0)
                {
                    #region dest
                    _xml.WriteStartElement("dest");////INICIO DEST

                    foreach (var item in lstDest)
                    {
                        if (item.CNPJ.Length == 14)
                        {
                            _xml.WriteStartElement("CNPJ");
                            _xml.WriteString(item.CNPJ.Trim());
                            _xml.WriteEndElement();
                        }
                        if (item.CNPJ.Length == 11)
                        {
                            _xml.WriteStartElement("CPF");
                            _xml.WriteString(item.CNPJ.Trim());
                            _xml.WriteEndElement();

                        }
                        _xml.WriteStartElement("xNome");
                        _xml.WriteString(item.xNome.Trim());
                        _xml.WriteEndElement();

                    #endregion

                        if (item.xLgr.Replace(" ", "").Length > 2)
                        {
                            #region EndDest

                            _xml.WriteStartElement("enderDest");

                            _xml.WriteStartElement("xLgr");
                            _xml.WriteString(item.xLgr.Trim());
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("nro");
                            _xml.WriteString(item.nro.Trim());
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("xBairro");
                            _xml.WriteString(item.xBairro.Trim());
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("cMun");
                            _xml.WriteString(item.cMun.Trim());
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("xMun");
                            _xml.WriteString(item.xMun.Trim());
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("UF");
                            _xml.WriteString(item.UF.Trim());
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("CEP");
                            _xml.WriteString(item.CEP.Trim());
                            _xml.WriteEndElement();


                            _xml.WriteEndElement();



                            #endregion
                        }
                    }


                    _xml.WriteStartElement("indIEDest");
                    _xml.WriteString("9");
                    _xml.WriteEndElement();

                    _xml.WriteEndElement();//FIM DEST


                #endregion
                }
                #region det
                int incrementoProd = 1;

                foreach (var item in lst)
                {

                    _xml.WriteStartElement("det");//Grupo do detalhamento de Produtos e Serviços da NF-e
                    _xml.WriteStartAttribute("nItem");//Número do item (1-990)
                    _xml.WriteString(incrementoProd.ToString().Trim());
                    _xml.WriteEndAttribute();// finalizando o atributo
                    #region prod
                    _xml.WriteStartElement("prod");//TAG de grupo do detalhamento de Produtos e Serviços da NF-e

                    _xml.WriteStartElement("cProd");//Código do produto ou serviço
                    _xml.WriteString(item.codigo.ToString().Trim());// tam 1-60
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("cEAN");//GTIN (Global Trade Item Number) do produto, antigo código EAN ou código de barras
                    // _xml.WriteString("2");// tam 0,8,12,13,14
                    _xml.WriteEndElement();

                    if (incrementoProd == 1 && Global.AmbienteCodigo == "2")
                    {
                        _xml.WriteStartElement("xProd");//Descrição do produto ou serviço
                        _xml.WriteString("NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL".Trim());// tam 1-120
                        _xml.WriteEndElement();
                    }
                    else
                    {
                        _xml.WriteStartElement("xProd");//Descrição do produto ou serviço
                        _xml.WriteString(item.descricao.Trim());// tam 1-120
                        _xml.WriteEndElement();
                    }


                    //omitida tag EX_TIPI

                    _xml.WriteStartElement("NCM");//Código NCM com 8 dígitos ou 2 dígitos (gênero)
                    _xml.WriteString(item.NCM.ToString().Trim());// tam 2,8
                    _xml.WriteEndElement();
                    if (item.CEST == string.Empty)
                    {

                    }
                    else
                    {
                        _xml.WriteStartElement("CEST");//Código NCM com 8 dígitos ou 2 dígitos (gênero)
                        _xml.WriteString("1300101");// tam 2,8
                        _xml.WriteEndElement();

                    }

                    //omitida tag EX_TIPI
                    _xml.WriteStartElement("CFOP");//Código Fiscal de Operações e Prestações
                    _xml.WriteString(item.CFOP);// tam 4
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("uCom");//Unidade Comercial
                    _xml.WriteString(item.unidade.Trim().Replace('"', ' ').Replace(" ", ""));// tam 1-6
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("qCom");//Quantidade Comercial
                    _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.qtd)));//tam 15
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("vUnCom");//Valor Unitário de Comercialização
                    _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.vlrUnit)));//tam 21
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("vProd");//Valor Total Bruto dos Produtos ou Serviços
                    _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.vlrUnit) * Convert.ToDecimal(item.qtd.ToString().Replace(".", ","))));// tam 15
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("cEANTrib");//GTIN (Global Trade Item Number) da unidade tributável, antigo código EAN ou código de barras
                    //  _xml.WriteString("");// tam 0,8,12,13,14
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("uTrib");//Unidade Tributável
                    _xml.WriteString(item.unidade.Trim().Replace('"', ' ').Replace(" ", ""));// tam 1-6
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("qTrib");//Quantidade Tributável
                    _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.qtd)));//tam 15
                    _xml.WriteEndElement();

                    _xml.WriteStartElement("vUnTrib");//Valor Unitário de tributação
                    _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.vlrUnit)));// tam 21
                    _xml.WriteEndElement();
                    if (Global._VlrOutros > 0 && incrementoProd == 1)
                    {
                        _xml.WriteStartElement("vOutro");//Outras despesas acessórias
                        _xml.WriteString(FormatValueForXML(Global._VlrOutros));// tam 21
                        _xml.WriteEndElement();
                    }
                    if (item.descontoPorc > 0)
                    {
                        _xml.WriteStartElement("vDesc");//Valor Unitário de tributação
                        _xml.WriteString(FormatValueForXML(item.descontoPorc));// tam 21
                        _xml.WriteEndElement();
                    }
                    else
                    {

                    }
                    //ocorrencia 0-1
                    _xml.WriteStartElement("indTot");//Indica se valor do Item (vProd) entra no valor total da NF-e (vProd)
                    _xml.WriteString("1");// Este campo deverá ser preenchido com:
                    //0 – o valor do item (vProd) não compõe o valor total da NF-e(vProd)
                    //1 – o valor do item (vProd)compõe o valor total da NF-e(vProd) (v2.0)
                    _xml.WriteEndElement();

                    _xml.WriteEndElement();//fim prod

                    #endregion

                    #region imposto
                    _xml.WriteStartElement("imposto");//Grupo de Tributos incidentes no Produto ou Serviço


                    _xml.WriteStartElement("vTotTrib");//Indica se valor do Item (vProd) entra no valor total da NF-e (vProd)
                    _xml.WriteString(FormatValueForXML(item.vTotTrib));// Este campo deverá ser preenchido com:

                    _xml.WriteEndElement();

                    #region ICMS
                    _xml.WriteStartElement("ICMS");//Grupo do ICMS da Operação própria e ST
                    if (item.CST == ("101"))
                    {
                        #region ICMS 101
                        _xml.WriteStartElement("ICMSSN101");//Grupo do ICMS da Operação própria e ST

                        _xml.WriteStartElement("orig");//Origem da mercadoria
                        _xml.WriteString("0");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("CSOSN");//Origem da mercadoria
                        _xml.WriteString("101");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("pCredSN");//Origem da mercadoria
                        _xml.WriteString("0.00");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("vCredICMSSN");//Origem da mercadoria
                        _xml.WriteString("0.00");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteEndElement();

                        #endregion
                    }
                    if (item.CST == ("300"))
                    {
                        #region ICMS 500
                        _xml.WriteStartElement("ICMSSN102");//Grupo do ICMS da Operação própria e ST

                        _xml.WriteStartElement("orig");//Origem da mercadoria
                        _xml.WriteString("0");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("CSOSN");//Origem da mercadoria
                        _xml.WriteString("300");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteEndElement();

                        #endregion
                    }
                    if (item.CST == ("500"))
                    {
                        #region ICMS 500
                        _xml.WriteStartElement("ICMSSN500");//Grupo do ICMS da Operação própria e ST

                        _xml.WriteStartElement("orig");//Origem da mercadoria
                        _xml.WriteString("0");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("CSOSN");//Origem da mercadoria
                        _xml.WriteString("500");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("vBCSTRet");//Origem da mercadoria
                        _xml.WriteString("0.00");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("vICMSSTRet");//Origem da mercadoria
                        _xml.WriteString("0.00");// tam 1
                        _xml.WriteEndElement();

                        _xml.WriteEndElement();

                        #endregion
                    }
                    if (item.CST != ("500") && item.CST != ("300") && item.CST != ("101"))
                    {
                        #region ICMS_ValComum
                        _xml.WriteStartElement("ICMS" + item.CST);//Grupo do ICMS da Operação própria e ST

                        //para o caso ICMS00,10,20,30,40,51,90,Part,ST,101,102,201,202,500,900
                        _xml.WriteStartElement("orig");//Origem da mercadoria
                        _xml.WriteString("0");// tam 1
                        _xml.WriteEndElement();

                        //para o caso ICMS00,10,20,51,70,90,Part,900
                        _xml.WriteStartElement("CST");//Valor da BC do ICMS
                        _xml.WriteString(item.CST);// tam 15
                        _xml.WriteEndElement();
                        if (item.CST == "00")
                        {
                            _xml.WriteStartElement("modBC");
                            _xml.WriteString("1");
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("vBC");
                            _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.vlrUnit) * Convert.ToInt32(item.qtd)));
                            _xml.WriteEndElement();

                            _xml.WriteStartElement("pICMS");
                            _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item._pICMS)));
                            _xml.WriteEndElement();



                            _xml.WriteStartElement("vICMS");

                            _xml.WriteString(FormatValueForXML(item._vICMS).Replace(",", "."));
                            _xml.WriteEndElement();

                            if (item._vICMS > 0)
                            {
                                _vlrTotVicms = _vlrTotVicms + item._vICMS;
                                _vlrTotBCVicms = _vlrTotBCVicms + Convert.ToDecimal(item.vlrUnit) * Convert.ToInt32(item.qtd);
                            }
                        }

                        _xml.WriteEndElement();

                        #endregion
                    }
                    _xml.WriteEndElement();
                    #endregion


                    if (Global.lstCST_PIS_COFINS.Contains(item.CSTPIS))
                    {
                        #region region PIS 04,05,06,07,08,09

                        _xml.WriteStartElement("PIS");//Grupo do PIS

                        _xml.WriteStartElement("PISNT");//Grupo de PIS tributado pela alíquota


                        _xml.WriteStartElement("CST");//Código de Situação Tributária do PIS
                        _xml.WriteString(item.CSTPIS);// tam 2
                        _xml.WriteEndElement();

                        _xml.WriteEndElement();
                        _xml.WriteEndElement();

                        #endregion
                    }
                    if (!Global.lstCST_PIS_COFINS.Contains(item.CSTPIS))
                    {
                        #region PIS
                        _xml.WriteStartElement("PIS");//Grupo do PIS
                        #region PISComum

                        _xml.WriteStartElement("PISOutr");//Grupo de PIS tributado pela alíquota


                        _xml.WriteStartElement("CST");//Código de Situação Tributária do PIS
                        _xml.WriteString(item.CSTPIS);// tam 2
                        _xml.WriteEndElement();

                        #endregion

                        #region PISAliq_PISOutr
                        _xml.WriteStartElement("vBC");//Valor da Base de Cálculo do PIS
                        _xml.WriteString(item.vBCPIS.ToString().Replace(",", "."));// tam 15
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("pPIS");//Alíquota do PIS (em percentual)
                        _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.pPIS.Replace(",", "."))));// tam 5
                        _xml.WriteEndElement();

                        #endregion



                        //usado em PISAliq,PISQtde,PISOutr
                        _xml.WriteStartElement("vPIS");//Valor do PIS
                        _xml.WriteString(item.vPIS.ToString().Replace(",", "."));// tam 15
                        _xml.WriteEndElement();
                        _xml.WriteEndElement();//end tipo do pis..
                        _xml.WriteEndElement();//end PIS...;
                        #endregion
                    }
                    if (Global.lstCST_PIS_COFINS.Contains(item.CSTCOFINS))
                    {
                        #region region COFINS 04,05,06,07,08,09

                        _xml.WriteStartElement("COFINS");//Grupo do PIS

                        _xml.WriteStartElement("COFINSNT");//Grupo de PIS tributado pela alíquota


                        _xml.WriteStartElement("CST");//Código de Situação Tributária do PIS
                        _xml.WriteString(item.CSTPIS);// tam 2
                        _xml.WriteEndElement();

                        _xml.WriteEndElement();
                        _xml.WriteEndElement();

                        #endregion
                    }
                    if (!Global.lstCST_PIS_COFINS.Contains(item.CSTPIS))
                    {
                        #region COFINS


                        _xml.WriteStartElement("COFINS");//Grupo do COFINS

                        _xml.WriteStartElement("COFINSOutr");//Grupo de COFINS tributado pela alíquota

                        _xml.WriteStartElement("CST");//Código de Situação Tributária do COFINS
                        _xml.WriteString(item.CSTCOFINS);// tam 2
                        _xml.WriteEndElement();




                        _xml.WriteStartElement("vBC");//Valor da Base de Cálculo do COFINS
                        _xml.WriteString(item.vBCCOFINS.ToString().Replace(",", "."));// tam 15
                        _xml.WriteEndElement();

                        _xml.WriteStartElement("pCOFINS");//Alíquota da COFINS (em percentual)
                        _xml.WriteString(FormatValueForXML(Convert.ToDecimal(item.pCOFINS.Replace(",", "."))));// tam 5
                        _xml.WriteEndElement();



                        //usado em COFINSAliq,COFINSQtde,COFINSOutr
                        _xml.WriteStartElement("vCOFINS");//Valor do COFINS
                        _xml.WriteString(item.vCOFINS.ToString().Replace(",", "."));// tam 15
                        _xml.WriteEndElement();

                        _xml.WriteEndElement();//end COFINS...;

                        _xml.WriteEndElement();//end COFINS;
                        #endregion
                    }

                    //ou este


                    _xml.WriteEndElement();//end element Imposto
                    #endregion
                #endregion

                    _xml.WriteEndElement();
                    //omitida tag DI e filhos
                    //omitida tag veicProd e filhos
                    //omitida tag med e filhos
                    //omitida tag arma
                    //omitida tag comb
                    incrementoProd++;

                    //fim det
                #endregion

                }

                #region Total
                _xml.WriteStartElement("total");//Grupo de Valores Totais da NF-e

                #region ICMSTot

                _xml.WriteStartElement("ICMSTot");//Grupo de Valores Totais referentes ao ICMS

                _xml.WriteStartElement("vBC");//Valor da BC do ICMS
                _xml.WriteString(FormatValueForXML(_vlrTotBCVicms).Replace(",", "."));// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vICMS");//Valor do ICMS
                _xml.WriteString(FormatValueForXML(_vlrTotVicms).Replace(",", "."));// tam 5
                _xml.WriteEndElement();

                _xml.WriteStartElement("vICMSDeson");//Valor da BC do ICMS ST
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vBCST");//Valor da BC do ICMS ST
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vST");//Valor da BC do ICMS ST
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();
                // decimal validar = Convert.ToDecimal(Global._lstVendaItem.Sum(s => s.valorTotal));
                _xml.WriteStartElement("vProd");//Valor Total Bruto dos Produtos ou Serviços
                decimal valor = 0;

                foreach (var item in lst)
                {
                    valor = Convert.ToDecimal(item.vlrUnit) * Convert.ToDecimal(item.qtd.ToString().Replace(".", ","));
                    valorTotal = valorTotal + valor;
                }


                _xml.WriteString(FormatValueForXML(valorTotal));// tam 15
                _xml.WriteEndElement();
                valorTotal = valorTotal + Global._VlrOutros;
                _xml.WriteStartElement("vFrete");//Valor Total do Frete
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vSeg");//Valor Total do Seguro
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vDesc");//Valor do Desconto
                _xml.WriteString(FormatValueForXML(Global._VlrDescNFce));// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vII");//Valor Total do II
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vIPI");//Valor do IPI
                _xml.WriteString("0.00");// tam 3
                _xml.WriteEndElement();

                _xml.WriteStartElement("vPIS");//Valor do PIS
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vCOFINS");//Valor do COFINS
                _xml.WriteString("0.00");// tam 15
                _xml.WriteEndElement();

                _xml.WriteStartElement("vOutro");//Outras despesas acessórias
                if (Global._VlrOutros > 0)
                {
                    _xml.WriteString(FormatValueForXML(Global._VlrOutros));// tam 15
                    _xml.WriteEndElement();
                }
                if (Global._VlrOutros == 0)
                {
                    _xml.WriteString("0.00");// tam 15
                    _xml.WriteEndElement();
                }

                _xml.WriteStartElement("vNF");//Valor Total da NF-e

                _xml.WriteString(FormatValueForXML((valorTotal - Global._VlrDescNFce)));// tam 15
                _xml.WriteEndElement();


                _xml.WriteStartElement("vTotTrib");//Indica se valor do Item (vProd) entra no valor total da NF-e (vProd)
                _xml.WriteString(FormatValueForXML(lst.Sum(e => e.vTotTrib)));// Este campo deverá ser preenchido com:

                _xml.WriteEndElement();


                _xml.WriteEndElement();

                #endregion
                //ocorrencia 0-1

                //ocorrencia 0-1


                _xml.WriteEndElement();
                #endregion

                #region transp

                _xml.WriteStartElement("transp");//Grupo de Informações do Transporte da NF-e

                _xml.WriteStartElement("modFrete");//Modalidade do frete
                _xml.WriteString("9");// tam 15
                _xml.WriteEndElement();





                _xml.WriteEndElement();
                #endregion

                #region pag

                decimal resta = valorTotal - Global._VlrDescNFce;
                Global._VlrTrocolNFce = Math.Abs((lstPag.Sum(s => s.VlrPag) - resta));
                int count = 1;
                foreach (var item in lstPag)
                {
                    Thread.Sleep(150);

                    if (lstPag.Count() > 1)
                    {
                        resta = resta - item.VlrPag;
                    }
                    _xml.WriteStartElement("pag");//Grupo de Informações do Transporte da NF-e
                    _xml.WriteStartElement("tPag");//Modalidade do frete
                    if (item.tpPag.ToString().Length == 1)
                    {
                        _xml.WriteString("0" + item.tpPag.ToString());// tam 15
                        _xml.WriteEndElement();
                    }
                    else
                    {
                        _xml.WriteString(item.tpPag.ToString());// tam 15
                        _xml.WriteEndElement();
                    }
                    _xml.WriteStartElement("vPag");//Modalidade do frete

                    if (lstPag.Count() > 1)
                    {
                        #region MAIS DE UMA FORMA PAG
                        decimal diferencial = item.VlrPag - valorTotal;

                        if (item.VlrPag < valorTotal && lstPag.Count() > 1 && resta == Math.Abs(diferencial))
                        {



                            _xml.WriteString(FormatValueForXML(item.VlrPag));// tam 15
                            _xml.WriteEndElement();
                        }
                        if (item.VlrPag > valorTotal && lstPag.Count() > 1)
                        {


                            string pAGtSTa = FormatValueForXML(Math.Abs(item.VlrPag - Math.Abs(resta)));
                            _xml.WriteString(pAGtSTa);// tam 15
                            _xml.WriteEndElement();
                        }
                        if (item.VlrPag < valorTotal && lstPag.Count() > 1 && resta != Math.Abs(diferencial) && count < lstPag.Count())
                        {

                            string pAGtST = FormatValueForXML(item.VlrPag);

                            _xml.WriteString(pAGtST);// tam 15
                            _xml.WriteEndElement();
                        }
                        if (item.VlrPag < valorTotal && lstPag.Count() > 1 && resta != Math.Abs(diferencial) && count == lstPag.Count())
                        {

                            string pAGtST = FormatValueForXML(Math.Abs(item.VlrPag - Math.Abs(resta)));

                            _xml.WriteString(pAGtST);// tam 15
                            _xml.WriteEndElement();
                        }

                        _xml.WriteEndElement();

                        #endregion
                    } if (lstPag.Count() == 1)
                    {

                        #region MAIS DE UMA FORMA PAG
                        if (item.VlrPag > valorTotal && lstPag.Count() == 1)
                        {


                            _xml.WriteString(FormatValueForXML(valorTotal - Global._VlrDescNFce));// tam 15
                            _xml.WriteEndElement();
                        }
                        else
                        {
                            _xml.WriteString(FormatValueForXML(valorTotal - Global._VlrDescNFce));// tam 15
                            _xml.WriteEndElement();

                        }

                        _xml.WriteEndElement();

                        #endregion

                    }

                    count++;
                }

                Global._VlrTotalNFce = lstPag.Sum(s => s.VlrPag);
                if (Global._VlrTotalNFce < valorTotal)
                {
                    Global._VlrTotalNFce = valorTotal - Global._VlrDescNFce;
                }

                #endregion

                #region
                _xml.WriteStartElement("infAdic");//Grupo de Informações do Transporte da NF-e
                _xml.WriteStartElement("infCpl");//Grupo de Informações do Transporte da NF-e
                string troco = string.Empty;
                if (lstPag.Sum(s => s.VlrPag) > lstPag.Sum(s => s.VlrPag))
                {
                    troco = "Troco: " + FormatValueForXML(lstPag.Sum(s => s.VlrPag) + lstPag.Sum(s => s.VlrPag));
                }

                _xml.WriteString(troco + rodape);


                _xml.WriteEndElement();
                _xml.WriteEndElement();
                #endregion



                _xml.WriteEndElement();//InfNFE
                #endregion

                //end envnfe
                _xml.WriteEndElement();//envNFE

                _xml.Close();




                if (Global._VlrTrocolNFce < 0)
                {
                    Global._VlrTrocolNFce = 0;
                }
                _vlrTotVicms = 0;
                _vlrTotBCVicms = 0;
                valorTotal = 0;
                resta = 0;
                count = 0;

                if (Global.tpEmis != "9")
                {
                    File.Move(caminhoNFCe + "\\" + Chave + "-nfe.xml", Global.PastaXmlEnvio + "\\" + Chave + "-nfe.xml");
                }

                return true;
            }
            catch
            {

                return false;

            }


        }

        #region MONTAGEM DA CHAVE E FORMATACOES MOEDAS
        public static void salvaNumeroNota(int numeroNota)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(@"C:/ASAsys/ASAsysNFCE/" + Global.CNPJ.ToString() + "/UniNfeConfig.xml");
            XmlNode node;
            node = myXmlDocument.DocumentElement;

            foreach (XmlNode node1 in node.ChildNodes)

                if (node1.Name == "Nota")
                {

                    node1.InnerText = numeroNota.ToString();
                }

            myXmlDocument.Save(@"C:/ASAsys/ASAsysNFCE/" + Global.CNPJ.ToString() + "/UniNfeConfig.xml");
        }


        public static String montaChaveAcessoNFe(String UF, Int32 Ano, Int32 Mes, String CNPJ, String Serie, String Numero, String FormaEmissao)
        {
            //Forma Emissao = 1-Normal, 2-Contigencia FS, 3-SCAN, 4-DPEC, 5-FS-DA

            String sAno = (Ano - (Ano / 100 * 100)).ToString();
            String Chave = UF + sAno + Mes.ToString("D2") + CNPJ + "65" + Serie.PadLeft(3, '0') + Numero.PadLeft(9, '0') + FormaEmissao + Numero.PadLeft(8, '0');
            Chave += calculaDVNFe(Chave);
            return Chave;

        }
        public static String calculaDVNFe(String ChaveAcesso)
        {
            Int32 Peso = 2, Soma = 0, Contador, Digito;
            for (Contador = (ChaveAcesso.Length - 1); Contador >= 0; Contador--)
            {
                Soma = Soma + (Convert.ToInt32(ChaveAcesso[Contador].ToString()) * Peso);
                if (Peso < 9) Peso++;
                else Peso = 2;
            }
            Digito = 11 - (Soma % 11);
            if (Digito > 9) Digito = 0;
            return Digito.ToString();
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
        #endregion
    }
}
