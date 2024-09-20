using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class ApartamentoController : Controller
    {
        /// <summary>
        /// Consulta de hóspedes no apartamento.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     {        
        ///       "apto": 428
        ///     }
        /// </remarks>
        /// /// <param name="json"></param> 
        /// <response code="200">Confirmação da Comanda Excluída</response>
        /// <response code="404">Erro ao excluir a Comanda.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName("ConsultarApartamento")]
        public ActionResult<Apartamento> GetApartamento([FromBody] JsonElement json)
        {
            Apartamento a = new Apartamento();

            var apto = json.GetProperty("apto");

            if (apto.ValueKind == JsonValueKind.Null || apto.ValueKind != JsonValueKind.String || LogUtil.Empty(apto.GetString()))
            {
                return NotFound("Obrigatório informar o Número do Apto.");
            }

            var db = Db.GetDbSCH();

            try
            {
                using (DbCommand cmd = db.NewCommand(@"select
                          a.APTFICHA 
                         ,a.APTNUMERO 
                         ,r.RAPDTENTRADA 
                         ,r.RAPJUNTOSEP
                         ,reg.REGFICHA
                         ,h.HOSFICHA 
                         ,h.HOSNOME 
                         ,h.HOSIDADE
                         ,h.HOSCPF 
                         ,F.FIRCODIGO 
                         ,F.FIRNOME
                         ,F.FIRCGC
                        from apto a
                        inner join REGAPTO r 
                        on (a.APTFICHA = r.APTFICHA)
                        inner join REGISTRO reg
                        on (r.RAPFICHA = reg.RAPFICHA)
                        inner join HOSPEDES h 
                        on (reg.HOSFICHA = h.HOSFICHA)
                        LEFT JOIN FIRMA f 
                        ON (REG.FIRCODIGO = F.FIRCODIGO)
                        where APTSTATUS = 'O'
                        and r.RAPSTATUS = 'A'
                        and reg.REGSTATUS = 'A'
                        AND a.APTNUMERO  = @APTNUMERO
                        order by r.APTFICHA, h.HOSNOME"))
                {
                    db.AddStringParameter(cmd, "@APTNUMERO").Value = apto.GetString();
                    db.Open();

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (a.aptficha == 0)
                            {
                                a.aptficha = reader["APTFICHA"].GenericConvert<int>();
                                a.aptnumero = reader["APTNUMERO"].ToString().Trim();
                                a.rapjuntosep = reader["RAPJUNTOSEP"].ToString().Trim();
                            }

                            Hospede h = new Hospede();
                            h.rapdtentrada = reader["RAPDTENTRADA"].GenericConvert<DateTime>();
                            h.regficha = reader["REGFICHA"].GenericConvert<int>();
                            h.hosficha = reader["HOSFICHA"].GenericConvert<int>();
                            h.hosnome = reader["HOSNOME"].ToString().Trim();
                            h.hoscpf = reader["HOSCPF"].ToString().Trim();
                            h.fircgc = reader["FIRCGC"].ToString().Trim();
                            h.hosidade = reader["HOSIDADE"].GenericConvert<int>();
                            h.fircodigo = reader["FIRCODIGO"].GenericConvert<int>();
                            h.firnome = reader["FIRNOME"].ToString().Trim();
                            a.olistHospedes.Add(h);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Nao autorizado - " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            return a;
        }
    }
}
