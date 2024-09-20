using log_food_api.Services;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    public class LoginController : Controller
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName("GerarToken")]
        private ActionResult<dynamic> Login([FromHeader] string login, [FromHeader] string senha)
        {
            try
            {
                if (LogUtil.Empty(login) || LogUtil.Empty(senha))
                {
                    return NotFound("Acesso Negado.");
                }
                var ovendedor = new VendedorRepositorio().GetVendedor(login, senha);

                if (ovendedor == null)
                {
                    return NotFound(new { message = "Usuário ou senha inválidos" });
                }

                var token = TokenService.GenerateToken(ovendedor.venlogin, ovendedor.vencpf);

                ovendedor.vensenha = "";

                return new
                {
                    Vendedor = ovendedor,
                    token.token,
                    expires = token.Expires
                };
            }
            catch (Exception ex)
            {
                return NotFound("Acesso Negado.");
            }
        }
    }
}
