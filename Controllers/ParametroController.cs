using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class ParametroController : Controller
    {
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Parametro> GetParametros()
        {
            try
            {
                return Ok(new ParametroRepositorio().GetParametros());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ParametroSCEF> GetParametrosSCEF()
        {
            try
            {
                return Ok(new ParametroRepositorio().GetParametrosSCEF());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
