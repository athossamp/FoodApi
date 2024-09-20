using log_food_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class CardapioController : Controller
    {
        /// <summary>
        /// Lista com produtos do Cardápio.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Cardapio>> ConsultarCardapio()
        {
            try
            {
                return Ok(Db.GetDbFood().NewCommand(@"select * from cardapio").FillList<Cardapio>());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
