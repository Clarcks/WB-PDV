using ASArquiteruraData;
using ASArquiteruraData.Repository;
using ASArquiteruraData.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace ClassGlobals
{
  public  class Global
    {

       
        public static Itb_produto_barraRepository barraResp = new tb_produto_barraRepository();
        public static Itb_vendaRepository vendaResp = new tb_vendaRepository();
       // public static Itb_usuarioRepository UsuarioResp = new tb_usuarioRepository();
        public static List<tb_produto_barra> lstBarra = new List<tb_produto_barra>(barraResp.GetAll());
        public static List<tb_venda> lstVenda = new List<tb_venda>();
        public static List<string> lst = new List<string>();
        #region GOBAL INICIALIZADORES
        public static tb_terminal Term;
        public static tb_usuario _Usuario;
        public static tb_usuario_funcao _UsuarioFuncao;
        public static Itb_usuarioRepository _Autorizador = new tb_usuarioRepository();
        public static tb_usuario _UserAutorizador = new tb_usuario();
        public static tb_venda Venda;
        public static tb_venda_pagamento VendaPagamento;
        public static Dest _dest = new Dest();
        public static string SerieNFCe;
        public static string TefCliente;
        public static string TefEstabelecimento;
        public static bool VendaTef;
        public static string SubPasta;
        public static string PastaXmlEnvio;
        public static string PastaXmlRetorno;
        public static string PastaXmlEnviado;
        public static string PastaXmlErro;
        public static string PastaValidar;
        public static List<string> lstCST_PIS_COFINS = new List<string>();
        public static string UnidadeFederativaCodigo;
        public static string AmbienteCodigo;
        public static string tpEmis;
        public static string Certificado;
        public static string NomeImpressora;
        public static string Papel;
        public static string Resumida;
        public static string CNPJ;
        public static string Serie;
        public static string Nota;
        public static string caminhoEnt;
        public static string caminhoSai;
        public static string inscMunicipal;
        public static string cUF;
        public static string cMunFG;
        public static string xFant;
        public static string xNome;
        public static string xLgr;
        public static string Nro;
        public static string cMun;
        public static string xMun;
        public static string xBairro;
        public static string UF;
        public static string CEP;
        public static string IE;
        public static string CRT;
        public static string telefone;
        public static string aliquotas;
        public static string caminhoLog;
        public static string CopiaCtgs;
        public static string Debug;
        public static string nImprime;
        public static decimal _VlrTotalNFce;
        public static decimal _VlrTrocolNFce;
        public static decimal _VlrDescNFce = 0;
        public static bool _VlrDescNFceBool;
        public static decimal _VlrNnF;
        public static decimal _VlrOutros;
        public static decimal _VlrOutrosItens;
        public static string Auditto;
        public static int Finalizadora;
        public static string DriveRede;
        public static tb_abertura_caixa _aCaixa = new tb_abertura_caixa();
        public static int CopiasNFE;
        public static object configImpressora = new PrinterSettings();
        #endregion
    }
  public class NFCe
  {
      public static List<objNota.Produto> lstProd = new List<objNota.Produto>();
  
  
  }
    public class Prod
    {
       public string xNome { get; set; }
       public string cCodigo { get; set;}
       public decimal Valor { get; set; }
    
    }
    public class Dest
    {
        public string xNome { get; set; }
        public string CPF { get; set; }
       // public string Email { get; set; }

    }

   
    
}
