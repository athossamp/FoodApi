using log_food_api.Models;

namespace log_food_api.Repositories
{
    public class MoedaRepositorio
    {
        public List<Moeda> GetMoedas()
        {

            List<Moeda> olist = new List<Moeda>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from tmoeda").FillList<Moeda>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
