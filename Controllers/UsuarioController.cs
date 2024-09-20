using Azure;
using log_food_api.Models;
using log_food_api.Services;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using System.IO;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    public class UsuarioController : Controller
    {
        /// <summary>
        /// Gerar token de acesso (deve ser utilizado na chamada aos outros métodos da api - Bearer Token).
        /// </summary>
        /// <param name="login">Envio no header</param>
        /// <param name="senha">Envio no header</param>
        /// <returns>token</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName("GerarTokenUsuario")]
        public ActionResult<Usuario> Login([FromHeader] string login, [FromHeader] string senha)
        {
            try
            {
                if (LogUtil.Empty(login) || LogUtil.Empty(senha))
                {
                    return NotFound("Acesso Negado.");
                }

                var ousuarioRep = new UsuarioRepositorio();

                var ousuario = ousuarioRep.GetUsuario(login, senha);

                if (ousuario == null || ousuario.usucodigo == 0)
                {
                    return NotFound(new { message = "Usuário ou senha inválidos" });
                }

                var token = TokenService.GenerateToken(ousuario.usulogin, ousuario.usucpf);

                if (!String.IsNullOrEmpty(token.token))
                {
                    ousuarioRep.SetToken(ousuario.usucodigo, token.token, token.Expires);
                }

                ousuario.usutoken = token.token;
                ousuario.usutoken_expira = token.Expires;
                ousuario.usulogin = "";
                ousuario.ususenha = "";
                ousuario.usucpf = "";
                //ousuario.usutipo = 
                //ousuario.usunivel = 0;

                return Ok(ousuario);
            }
            catch (Exception ex)
            {
                return NotFound("Acesso Negado.");
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName("CriarUsuario")]
        public ActionResult<Usuario> CriarUsuario([FromBody] Usuario json)
        {
            try
            {
                return Ok(new UsuarioRepositorio().InserirUsuario(json));
            }
            catch (Exception ex)
            {
                return NotFound("Não foi possível criar usuário." + ex.Message);
            }
        }
        /*public ActionResult<dynamic> Login([FromHeader] string login, [FromHeader] string senha)
        {
            try
            {
                if (LogUtil.Empty(login) || LogUtil.Empty(senha))
                {
                    return NotFound("Acesso Negado.");
                }
                
                var ousuarioRep = new UsuarioRepositorio();

                var ousuario = ousuarioRep.GetUsuario(login, senha);

                if (ousuario == null || ousuario.usucodigo == 0)
                {
                    return NotFound(new { message = "Usuário ou senha inválidos" });
                }

                var token = TokenService.GenerateToken(ousuario.usulogin, ousuario.usucpf);

                if (!String.IsNullOrEmpty(token.token))
                {
                    ousuarioRep.SetToken(ousuario.usucodigo, token.token, token.Expires);
                }

                ousuario.ususenha = "";

                return new
                {
                    Usuario = ousuario,
                    token.token,
                    expires = token.Expires
                };
            }
            catch (Exception ex)
            {
                return NotFound("Acesso Negado.");
            }
        }*/

        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ActionName("AtualizarSenha")]
        //public void AtualizarSenha([FromBody] string usulogin)
        //{
        //    try
        //    {
        //        new UsuarioRepositorio().UpdateSenha(usulogin);
        //    }
        //    catch (Exception ex)
        //    {
        //        NotFound("Não foi possível encontrar o usuário." + ex.Message);
        //    }
        //}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UsuarioVendedor> GetVendedor()
        {
            try
            {
                return Ok(new UsuarioRepositorio().GetUsuarioVendedor());
            }
            catch (Exception ex)
            {
                return NotFound("Não foi possível encontrar o usuário." + ex.Message);
            }
        }
    }
}
