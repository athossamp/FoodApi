using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class EstoqueController : Controller
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Estoque> GetEstoque()
        {
            try
            {
                return Ok(new EstoqueRepositorio().GetEstoque());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Buscar item do estoque por procodigo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///			{
        ///			  "procodigo": 18
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Estoque> GetEstoquePorCodigo([FromBody] JsonElement json)
        {
            try
            {
                Estoque estoque = new Estoque();
                var procodigo = json.GetProperty("procodigo");
                var procodigoValue = procodigo.GetInt32();

                return Ok(new EstoqueRepositorio().GetEstoqueByCodigo(procodigoValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Buscar item do estoque por procodigo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///			{
        ///			  {
	    ///             "cProdescricao": "Arroz" /*Pode passar tanto procodigo, quanto codigo de barra nessa mesma string*/
        ///            }
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public List<ProdutoP> GetListaProduto([FromBody] JsonElement json)
        {
            EstoqueRepositorio oProduto = new EstoqueRepositorio();
            var cProdescricao = json.GetProperty("cProdescricao").ToString().ToUpper();
            //var cCodBarra = json.GetProperty("cCodBarra").ToString().ToUpper();
            //var nProcodigo = json.GetProperty("nProcodigo").GetInt32();

            return oProduto.GetListaProduto(cProdescricao);
        }
        /// <summary>
        /// Buscar item do estoque por procodigo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///			{
        ///			  "cmdcodigo": 131
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public List<ProdutoEstoqueComanda> GetProdutosComanda([FromBody] JsonElement json)
        {
            EstoqueRepositorio oProduto = new EstoqueRepositorio();
            int cmdcodigo = json.GetProperty("cmdcodigo").GetInt32();
            
            return oProduto.GetItensComandas(cmdcodigo);
        }
    }
}
