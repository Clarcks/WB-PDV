using ASArquiteruraData;
using ASArquiteruraData.Repository;
using ASArquiteruraData.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassGlobals
{
 public   class Venda
 {
     #region CONTRUTORES
     public static  Itb_vendaRepository _venda = new tb_vendaRepository();
     public static Itb_venda_itemRepository _vendaItem = new tb_venda_itemRepository();
     public static Itb_venda_pagamentoRepository _vendaPag = new tb_venda_pagamentoRepository();
     #endregion

     #region METODOS
     public static bool Gravavenda(tb_venda venda)
     {
         try
         {

             _venda.Add(venda);
             return true;
         }
         catch (Exception)
         {

             return false;
         }
     }

     public static bool GravavendaItem(List<tb_venda_item> vendaItem)
     {
         try
         {

             _vendaItem.AddAllList(vendaItem,false);
             return true;
         }
         catch (Exception)
         {

             return false;
         }
     }

     public static bool GravavendaPagamento(tb_venda_pagamento vendaPagamento)
     {
         try
         {

             _vendaPag.Add(vendaPagamento);
             return true;
         }
         catch (Exception)
         {

             return false;
         }
     }
     #endregion
    }
}
