using FirebirdSql.Data.FirebirdClient;
using Logicom.Infraestrutura;

namespace log_food_api
{
    public static class Db
    {

        public static string connStrFood { get; set; }
        public static string connStrSCH { get; set; }
        public static string connStrSCEF { get; set; }


        public static TLogDatabase GetDbFood()
        {
            try
            {
                return new TLogDatabase("Npgsql", connStrFood);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TLogDatabase GetDbSCH()
        {
            try
            {
                var db = new TLogDatabase("Microsoft.Data.SqlClient", connStrSCH);
                db.oConn.ConnectionString = connStrSCH;
                db.Port = 1433;

                return db;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static TLogDatabase GetDbSCEF()
        {
            try
            {
                var db = new TLogDatabase("FirebirdSql.Data.FirebirdClient", connStrSCEF);

                return db;
            }
            catch (Exception ex)
            {
                LogUtil.InserirEvento("", "Get DBScef: " + Environment.NewLine + ex.Message);
                throw ex;
            }
        }
    }
}
