using log_food_api.Models;
using Logicom.Infraestrutura;
using System.Data.Common;

namespace log_food_api.Repositories
{
    public class ComandaItemComposicaoRepositorio
    {
        private DbCommand cmdIns{ get; set; }
        private TLogDatabase _db;
        public ComandaItemComposicaoRepositorio(TLogDatabase odbFood)
        {
            _db = odbFood;
            cmdIns = odbFood.NewCommand(@"INSERT INTO comanda_item_composicao (ciccodigo, cmdicodigo, procodigo, procodigo_composicao, usucodigo, ultatualizacao) 
                VALUES(nextval('ciccodigo'), @cmdicodigo, @procodigo, @procodigo_composicao, @usucodigo, now());");
            odbFood.AddIntParameter(cmdIns, "@cmdicodigo");
            odbFood.AddIntParameter(cmdIns, "@procodigo");
            odbFood.AddIntParameter(cmdIns, "@procodigo_composicao");
            odbFood.AddInt16Parameter(cmdIns, "@usucodigo");
        }

        public void InserirItens(List<ComandaItemComposicao> olist)
        {
            try
            {
                cmdIns.Transaction = _db.oTxn;

                foreach (var item in olist)
                {
                    cmdIns.Parameters["@cmdicodigo"].Value = item.cmdicodigo;
                    cmdIns.Parameters["@procodigo"].Value = item.procodigo;
                    cmdIns.Parameters["@procodigo_composicao"].Value = item.procodigo_composicao;
                    cmdIns.Parameters["@usucodigo"].Value = item.usucodigo;
                    cmdIns.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ~ComandaItemComposicaoRepositorio()
        {
            try
            {
                if (cmdIns != null)
                {
                    cmdIns.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
