using FirebirdSql.Data.FirebirdClient;
using Logicom.Infraestrutura;

namespace log_food_api
{
    public static class Db
    {
        //public static TLogDatabase odbFood { get; set; }
        //public static TLogDatabase odbHotel { get; set; }
        //public static TLogDatabase odbSCEF { get; set; }

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

                //Db.odbHotel.oConn.ConnectionString = "Server=192.168.0.35;Database=sagres;User Id=sa;Password=contabil*1;TrustServerCertificate=true";
                //Db.odbHotel.oConn.ConnectionString = builder.Configuration["ConnectionStrings:SCH"];
                //Db.odbHotel.Port = 1433;
                //return new TLogDatabase("Npgsql", connStrSCH);

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
                // var db = new TLogDatabase("FirebirdSql.Data.FirebirdClient", "192.168.0.35", "H:\\Bancos_Teste\\ESTOQUE_PBAIXO_BR.IB", "SYSDBA", "contabil");
                var db = new TLogDatabase("FirebirdSql.Data.FirebirdClient", connStrSCEF);
                //db.oConn = new FbConnection(connStrSCEF);
                //db.oConn.ConnectionString = connStrSCEF;

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