using log_food_api;
using log_food_api.Models;
using Logicom.Infraestrutura;
using System.Data.Common;

namespace log_food_api
{
    public class VendedorRepositorio
    {
        public Vendedor GetVendedor(string cuser, string cpwd)
        {
            Vendedor oitem = null;
            var db = Db.GetDbFood();

            try
            {
                db.Open();

                using (DbCommand cmd = db.NewCommand("select * from vendedor where venlogin = @login and vensenha = @senha"))
                {
                    db.AddStringParameter(cmd, "@login").Value = cuser;
                    db.AddStringParameter(cmd, "@senha").Value = cpwd;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            oitem = new Vendedor();
                            oitem.vencodigo = reader["vencodigo"].GenericConvert<int>();
                            oitem.vennome = reader["vennome"].ToString();
                            oitem.venapelido = reader["venapelido"].ToString();
                            oitem.venlogin = reader["venlogin"].ToString();
                            oitem.vensenha = reader["vensenha"].ToString();
                            oitem.venemail = reader["venemail"].ToString();
                            oitem.vencpf = reader["vencpf"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.Close();
            }

            return oitem;
        }
    }
}
