using log_food_api.Models;

namespace log_food_api.Repositories
{
    public class EmpresaRepositorio
    {
        public List<Empresa> GetEmpresas()
        {
            List<Empresa> olist = new List<Empresa>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from empresa").FillList<Empresa>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
