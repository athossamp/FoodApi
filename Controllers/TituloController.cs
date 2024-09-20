using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    //[Authorize]
    public class TituloController : Controller
    {
        /// <summary>
        /// Buscar item do estoque por procodigo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///			{
        ///			  "pescodigo": 2470,
        ///			 
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Titulo> GetTituloDevedor([FromBody] JsonElement json)
        {
            try
            {
                int pescodigo = 0;

                if (json.ValueKind != JsonValueKind.Undefined)
                {
                    pescodigo = json.GetProperty("pescodigo").GetInt32();
                }
                return Ok(new TituloRepositorio().GetTituloDevedor(pescodigo));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Pessoa> GetClientes([FromBody] JsonElement json)
        {
            try
            {
                var pesnome = json.GetProperty("PESNOME").ToString().Trim();
                var pescodigo = json.GetProperty("PESCODIGO").GetInt32();
                return Ok(new PessoaRepositorio().GetClientes(pesnome, pescodigo));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
