using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class DepartamentoLocalController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Departamento> GetDepartamento()
        {
            try
            {
                return Ok(new DepartamentoLocalRepositorio().GetDepartamentos());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Localizacao> GetLocalizacao()
        {
            try
            {
                return Ok(new DepartamentoLocalRepositorio().GetLocalizacao());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<DepartLocal> GetDepartLocal()
        {
            try
            {
                return Ok(new DepartamentoLocalRepositorio().GetDepartLocals());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
