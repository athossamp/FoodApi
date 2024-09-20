using log_food_api.Models;

namespace log_food_api.Repositories
{
    public class GarsonRepositorio
    {
        public List<Garson> GetGarsons()
        {

            List<Garson> olist = new List<Garson>();

            try
            {
                return Db.GetDbSCH().NewCommand(@"SELECT GARCODIGO, GARNOME
                FROM Sagres.dbo.GARSON;").FillList<Garson>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
