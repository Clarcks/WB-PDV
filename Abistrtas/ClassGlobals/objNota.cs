using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassGlobals
{
  public  class objNota
    {
      public class Produto
      {
          public string codigoInterno { get; set; }
          public string codigo { get; set; }
          public string descricao { get; set; }
          public string aliquota { get; set; }
          public decimal qtd { get; set; }
          public decimal vlrUnit { get; set; }
          public decimal descontoPorc { get; set; }
          public string unidade { get; set; }
          public string tipoDescontoAcrescimo { get; set; }
          public string descontoAcrescimo { get; set; }
          public string departamento { get; set; }
          public string NCM { get; set; }
          public decimal vTotTrib { get; set; }
          public string CSTPIS { get; set; }
          public decimal vBCPIS { get; set; }
          public string pPIS { get; set; }
          public decimal vPIS { get; set; }
          public string CSTCOFINS { get; set; }
          public decimal vBCCOFINS { get; set; }
          public string pCOFINS { get; set; }
          public decimal vCOFINS { get; set; }
          public decimal vDesc { get; set; }
          public string CEST { get; set; }
          public string CFOP { get; set; }
          public string CST { get; set; }
          public decimal _vICMS { get; set; }
          public string _pICMS { get; set; }
      }

      public class Xdest
      {

          public string CNPJ { get; set; }
          public string xNome { get; set; }
          public string xLgr { get; set; }
          public string nro { get; set; }
          public string xBairro { get; set; }
          public string cMun { get; set; }
          public string xMun { get; set; }
          public string UF { get; set; }
          public string CEP { get; set; }


      }

      public class xPag
      {
          public int tpPag { get; set; }
          public decimal VlrPag { get; set; }
          public string xObs { get; set; }

      }
    }
}
