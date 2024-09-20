using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class MoedasController : Controller

    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Moeda> GetMoedas()
        {
            try
            {
                return Ok(new MoedaRepositorio().GetMoedas());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
