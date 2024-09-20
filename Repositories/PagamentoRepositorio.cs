using log_food_api.Models;
using Logicom.Infraestrutura;

namespace log_food_api.Repositories
{
    public class PagamentoRepositorio
    {
        public List<FormaPagamento> GetFormaPagamentos()
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                odb.Open();
                odb.StartTransaction();
                List<FormaPagamento> oFormaPagamento = new List<FormaPagamento>();
                using (var cmd = odb.NewCommand(@"SELECT * FROM FORMAPAGAMENTO"))
                {
                    cmd.Transaction = odb.oTxn;
                    oFormaPagamento = cmd.FillList<FormaPagamento>();
                }

                return oFormaPagamento;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CondicaoPagamento> GetCondicaoPagamentos()
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                odb.Open();
                odb.StartTransaction();
                List<CondicaoPagamento> oCondPagamento = new List<CondicaoPagamento>();
                using (var cmd = odb.NewCommand(@"SELECT * FROM CONDPAGAMENTO"))
                {
                    cmd.Transaction = odb.oTxn;
                    oCondPagamento = cmd.FillList<CondicaoPagamento>();
                }

                return oCondPagamento;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
