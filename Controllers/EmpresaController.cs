using log_food_api.Models;
using log_food_api.Repositories;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class EmpresaController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Empresa> GetEmpresas()
        {
            try
            {
                return Ok(new EmpresaRepositorio().GetEmpresas());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }
    }
}
