using log_food_api.Models;
using log_food_api.Repositories;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class LocalAtendimentoController : Controller
    {
        /// <summary>
        /// Lista com locais de atendimento.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LocalAtendimento> ConsultarLocaisAtendimento()
        {
            try
            {

                return Ok(new LocalAtendimentoRepositorio().GetLocaisAtendimento());
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AtendimentoInfo> ConsultarLocalAtendComAtendente()
        {
            try
            {
                return Ok(new LocalAtendimentoRepositorio().GetLocalAtendAtendente());
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public void ChecaComanda()
        {
            try
            {
               new LocalAtendimentoRepositorio().checaComanda();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }

}