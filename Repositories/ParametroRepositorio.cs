using log_food_api.Models;
using Logicom.Infraestrutura;
using System.Data.Common;

namespace log_food_api.Repositories
{
    public class ParametroRepositorio
    {
        public List<Parametro> GetParametros()
        {
            List<Parametro> olist = new List<Parametro>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from parametro").FillList<Parametro>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ParametroSCEF> GetParametrosSCEF()
        {
            List<ParametroSCEF> olist = new List<ParametroSCEF>();

            try
            {
                return Db.GetDbSCEF().NewCommand(@"SELECT * FROM PARAMETRO").FillList<ParametroSCEF>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetInformacaoMaquina(string parnome)
        {
            Parametro parametro = new Parametro();
            TLogDatabase db = Db.GetDbFood();
            try 
            { 
                using  (var cmd = db.NewCommand(@"SELECT * FROM PARAMETRO WHERE PARNOME = @parnome"))
                {
                    db.AddStringParameter(cmd, "@parnome").Value = parnome;
                    db.Open();
                    cmd.Transaction = db.oTxn;
                    
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            parametro.parvalor = reader["parvalor"].ToString();
                        }
                    }
                    db.Close();
                }
            } catch(Exception ex)
            {
                db.Close();
                throw ex;
            }
            return parametro.parvalor;
        }

    }
}
