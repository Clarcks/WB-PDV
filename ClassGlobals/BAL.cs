using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ClassGlobals
{
  public class BAL
    {
      [DllImport("P05.DLL")]
      public static extern int AbrePorta(int porta, int velocidade, int dataBits, int paridade);

      [DllImport("P05.DLL")]
      public static extern int FechaPorta();

      [DllImport("P05.DLL")]
      public static extern int PegaPeso(int tipoEscrita, StringBuilder peso, string diretorio);


      public decimal RetornaPeso(int portaCom)
      {
          int retorno = AbrePorta(portaCom, 0, 0, 2);

          if (retorno == 1)
          {
              StringBuilder pesoString = new StringBuilder();
              retorno = PegaPeso(1, pesoString, "");
              decimal peso = Convert.ToDecimal(pesoString.ToString());

              retorno = FechaPorta();

              if (retorno == 1)
              {
                  return peso;
              }
              else
              {
                  //Erro ao fechar porta
              }
          }
          else
          {
              //Erro ao abrir a porta
          }

          return 0;
      }

    }
}
