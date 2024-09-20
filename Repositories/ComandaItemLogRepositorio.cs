using log_food_api.Models;
using Logicom.Infraestrutura;
using System.Data.Common;
using System.Windows.Forms;

namespace log_food_api.Repositories
{
    public class ComandaItemLogRepositorio
    {
        public void InserirLog(TLogDatabase db, int cmdicodigo, string tlichave)
        {
            try
            {
                using (var cmd = db.NewCommand(@"INSERT INTO comanda_item_log (cmilcodigo, cmdicodigo, tlicodigo, usucodigo, cmilinsercao) VALUES(nextval('cmilcodigo'), @cmdicodigo, (select tlicodigo from tipo_log_item where tlichave = @tlichave), 0, now());"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddIntParameter(cmd, "@cmdicodigo").Value = cmdicodigo;
                    db.AddStringParameter(cmd, "@tlichave").Value = tlichave;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void InserirLog(int cmdicodigo, string tlichave)
        {
            var db = Db.GetDbFood();

            try
            {
                using (var cmd = db.NewCommand(@"INSERT INTO comanda_item_log (cmilcodigo, cmdicodigo, tlicodigo, usucodigo, cmilinsercao) VALUES(nextval('cmilcodigo'), @cmdicodigo, (select tlicodigo from tipo_log_item where tlichave = @tlichave), 0, now());"))
                {
                    cmd.Transaction = db.StartTransaction();
                    db.AddIntParameter(cmd, "@cmdicodigo").Value = cmdicodigo;
                    db.AddStringParameter(cmd, "@tlichave").Value = tlichave;
                    cmd.ExecuteNonQuery();
                    db.Commit();
                }
            }
            catch (Exception ex)
            {
                if (db != null)
                {
                    db.RollBack();
                }

                throw ex;
            }
        }
    }
}
