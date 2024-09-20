using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class PagamentoController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FormaPagamento> GetFormasPagamento()
        {
            try
            {
                return Ok(new PagamentoRepositorio().GetFormaPagamentos());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CondicaoPagamento> GetCondicoesPagamento()
        {
            try
            {
                return Ok(new PagamentoRepositorio().GetCondicaoPagamentos());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
