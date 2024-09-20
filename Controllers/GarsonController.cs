using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class GarsonController : Controller
    {
            [HttpGet]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public ActionResult<Garson> GetGarson()
            {
                try
                {
                    return Ok(new GarsonRepositorio().GetGarsons());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
}
