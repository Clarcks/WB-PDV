using ASArquiteruraData;
using ASArquiteruraData.Repository;
using ASArquiteruraData.RepositoryInterfaces;
using ClassGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ClassGlobals
{
    public class CarregaGlobal
    {
       public static bool ComponenteConfig(string arquivo)
       {
           try
           {
             
               XmlDocument xmlDoc = new XmlDocument();
               xmlDoc.Load(arquivo); //Carregando o arquivo
               string PastaSub = DateTime.Now.ToString("yyyyMM");
               Global.SubPasta = PastaSub;

                 //Global.Auditto = xmlDoc.GetElementsByTagName("GravarEventosNaPastaEnviadosNFe")[0].InnerText.ToString();
               //Global.PastaXmlEnvio = xmlDoc.GetElementsByTagName("PastaXmlEnvio")[0].InnerText.ToString();
               //Global.PastaXmlRetorno = xmlDoc.GetElementsByTagName("PastaXmlRetorno")[0].InnerText.ToString();
               //Global.PastaXmlEnviado = xmlDoc.GetElementsByTagName("PastaXmlEnviado")[0].InnerText.ToString();
               //Global.PastaXmlErro = xmlDoc.GetElementsByTagName("PastaXmlErro")[0].InnerText.ToString();
               //Global.PastaValidar = xmlDoc.GetElementsByTagName("PastaValidar")[0].InnerText.ToString();
               Global.CNPJ = xmlDoc.GetElementsByTagName("CNPJ")[0].InnerText.ToString();
               Global.UnidadeFederativaCodigo = xmlDoc.GetElementsByTagName("cUF")[0].InnerText.ToString();
               Global.AmbienteCodigo = xmlDoc.GetElementsByTagName("tpAmb")[0].InnerText.ToString();
               Global.tpEmis = xmlDoc.GetElementsByTagName("tpEmis")[0].InnerText.ToString();
               //Global.Certificado = xmlDoc.GetElementsByTagName("Certificado")[0].InnerText.ToString();
               //Global.Serie = xmlDoc.GetElementsByTagName("Serie")[0].InnerText.ToString();
               //Global.Nota = xmlDoc.GetElementsByTagName("Nota")[0].InnerText.ToString();
               //Global.Certificado = xmlDoc.GetElementsByTagName("Certificado")[0].InnerText.ToString();
               //Global.caminhoEnt = xmlDoc.GetElementsByTagName("caminhoEnt")[0].InnerText.ToString();
               //Global.caminhoSai = xmlDoc.GetElementsByTagName("caminhoSai")[0].InnerText.ToString();
               //Global.inscMunicipal = xmlDoc.GetElementsByTagName("inscMunicipal")[0].InnerText.ToString();
               //Global.cUF = xmlDoc.GetElementsByTagName("cUF")[0].InnerText.ToString();
               Global.cMunFG = xmlDoc.GetElementsByTagName("cMun")[0].InnerText.ToString();
               //Global.xFant = xmlDoc.GetElementsByTagName("xFant")[0].InnerText.ToString();
               //Global.xNome = xmlDoc.GetElementsByTagName("Nome")[0].InnerText.ToString();
               //Global.xLgr = xmlDoc.GetElementsByTagName("xLgr")[0].InnerText.ToString();
               //Global.Nro = Convert.ToInt32(xmlDoc.GetElementsByTagName("Nro")[0].InnerText).ToString();
               Global.cMun = xmlDoc.GetElementsByTagName("cMun")[0].InnerText.ToString();
               Global.xMun = xmlDoc.GetElementsByTagName("xMun")[0].InnerText.ToString();
               //Global.xBairro = xmlDoc.GetElementsByTagName("xBairro")[0].InnerText.ToString();
               //Global.UF = xmlDoc.GetElementsByTagName("UF")[0].InnerText.ToString();
               Global.CEP = xmlDoc.GetElementsByTagName("CEP")[0].InnerText.ToString();
               //Global.IE = xmlDoc.GetElementsByTagName("IE")[0].InnerText.ToString();
               //Global.CRT = xmlDoc.GetElementsByTagName("CRT")[0].InnerText.ToString();
               Global.telefone = xmlDoc.GetElementsByTagName("fone")[0].InnerText.ToString();
               //Global.aliquotas = xmlDoc.GetElementsByTagName("aliquotas")[0].InnerText.ToString();
               //Global.caminhoLog = xmlDoc.GetElementsByTagName("caminhoLog")[0].InnerText.ToString();
               //Global.CopiaCtgs = xmlDoc.GetElementsByTagName("CopiaCtgs")[0].InnerText.ToString();
               //Global.DriveRede = xmlDoc.GetElementsByTagName("Debug")[0].InnerText.ToString();
               Itb_terminalRepository term = new tb_terminalRepository();
               List<tb_terminal> lstTerm = new List<tb_terminal>(term.GetAll());
               Global.Term = lstTerm[0];
              

               return true;
           }
           catch (Exception erro)
           {

               return false;

           }

       }
       public static bool SalvaNumero()
       {
           try
           {
               Itb_terminalRepository termSalva = new tb_terminalRepository();

               Global.Term.te_numero_nfce = Convert.ToInt32(Global.Term.te_numero_nfce +1);
               List<tb_terminal> lstTerm = new List<tb_terminal>();
               lstTerm.Add(Global.Term);
               termSalva.AddAllList(lstTerm,false);
               return true;
           }
           catch 
           {

               return false;
           }
       
       }
       
    }
}
