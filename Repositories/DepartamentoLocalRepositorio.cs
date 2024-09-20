using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;

namespace log_food_api.Repositories
{
    public class DepartamentoLocalRepositorio
    {
       
        public List<Departamento> GetDepartamentos()
        {

            List<Departamento> olist = new List<Departamento>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from departamento").FillList<Departamento>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Localizacao> GetLocalizacao()
        {
            List<Localizacao> olist = new List<Localizacao>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from localizacao").FillList<Localizacao>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DepartLocal> GetDepartLocals()
        {
            List<DepartLocal> olist = new List<DepartLocal>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from departamentolocal").FillList<DepartLocal>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
