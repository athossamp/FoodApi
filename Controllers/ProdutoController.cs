using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace log_food_api.Controllers
{
    [Route("[action]")]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        /// <summary>
        /// Lista todos os produtos disponíveis.
        /// </summary>
        /// <remarks>
        /// Exemplo para retornar produto específico [JsonBody]:
        ///
        ///     {        
        ///       "procodigo": 15
        ///     }
        ///        
        /// </remarks>
        /// /// <param name="json"></param> 
        /// <response code="200">Consulta realizada com sucesso.</response>
        /// <response code="404">Erro ao excluir ao consultar produtos.</response>
        [HttpPost]
        [ActionName("ConsultarProdutos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Produto>> GetProdutos([FromBody] JsonElement json)
        {
            List<Produto> olist = new List<Produto>();
            int nprocodigo = 0;

            var db = Db.GetDbFood();

            try
            {
                db.Open();

                using (var cmd = db.NewCommand(@"
                        select
                           p.*
                          ,c.cdpcodigo 
                          ,c.cdptitulo 
                          ,c.cdpdescricao   
                           from produto p
                           inner join cardapio c
                           on (p.cdpcodigo = c.cdpcodigo)
                           where p.prostatus = 'A'
                           order by c.cdpplano,p.prodescricao "))
                {

                    if (json.ValueKind != JsonValueKind.Undefined)
                    {
                        nprocodigo = json.GetProperty("procodigo")!.GetInt32();
                    }

                    if (nprocodigo > 0)
                    {
                        cmd.CommandText = @"
                          select
                           p.*
                          ,c.cdpcodigo 
                          ,c.cdptitulo 
                          ,c.cdpdescricao   
                           from produto p
                           inner join cardapio c
                           on (p.cdpcodigo = c.cdpcodigo)
                           where p.prostatus = 'A'
                           and p.procodigo = @nprocodigo
                           order by c.cdpplano,p.prodescricao ";

                        db.AddIntParameter(cmd, "@nprocodigo").Value = nprocodigo;

                    }
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Produto oitem = new Produto();
                            oitem.procodigo = reader["procodigo"].GenericConvert<int>();
                            oitem.precodigo = reader["precodigo"].GenericConvert<int>();
                            oitem.clmcodigo = reader["clmcodigo"].GenericConvert<int>();
                            oitem.cdpcodigo = reader["cdpcodigo"].GenericConvert<int>();
                            oitem.embcodigo = reader["embcodigo"].ToString().Trim();
                            oitem.prequantidade = reader["prequantidade"].GenericConvert<decimal>();
                            oitem.prodescricao = reader["prodescricao"].ToString();
                            oitem.prodescricao_adicional = reader["prodescricao_adicional"].ToString();
                            oitem.propreco_venda = reader["propreco_venda"].GenericConvert<decimal>();
                            oitem.propreco_custo = reader["propreco_custo"].GenericConvert<decimal>();
                            oitem.procodbarra = reader["procodbarra"].ToString();
                            oitem.protipo = reader["protipo"].ToString();
                            oitem.proadicional = reader["proadicional"].ToString();
                            oitem.propreparo = reader["propreparo"].ToString();
                            oitem.latcodigo_preparo = reader["latcodigo_preparo"].GenericConvert<int>();
                            oitem.prostatus = reader["prostatus"].ToString();
                            oitem.ultatualizacao = reader["ultatualizacao"].GenericConvert<DateTime>();

                            /*if (reader["proimagem"] != DBNull.Value)
                            {
                                oitem.proimagem = (byte[])reader["proimagem"];
                            }*/

                            oitem.cdptitulo = reader["cdptitulo"].ToString();
                            oitem.cdpdescricao = reader["cdpdescricao"].ToString();

                            olist.Add(oitem);
                        }
                    }
                }

                //set composicao e substitutos
                using (var cmd = db.NewCommand(@"
                        select pc.pcpcodigo,
                          pc.procodigo_pai as procodigo_pai_default,
                          pc.procodigo as procodigo_default, 
                          p.prodescricao as prodescricao_default, 
                          pc.pcpadicional, 
                          pc.pcpisento_cobranca,
                          pcS.procodigo_pai as procodigo_pai_composicao,
                          pcs.procodigo as procodigo_troca, 
                          p2.prodescricao as prodescricao_troca,
                          pcs.pcsbvalor_adicional 
                          from produto_composicao pc
                          inner join produto p
                          on (pc.procodigo = p.procodigo)
                          left join produto_composicao_substituto pcs 
                          on (pc.pcpcodigo = pcs.pcpcodigo)
                          left join produto p2
                          on (p2.procodigo = pcs.procodigo)
                          where pc.procodigo_pai = @procodigo
                          and pc.pcpstatus = 'A'
                          and pcs.pcsbstatus = 'A'
                          order by p.procodigo, pc.pcpcodigo"))
                {
                    db.AddIntParameter(cmd, "@procodigo");

                    Produto_Composicao procomp = new Produto_Composicao();
                    Produto_Composicao_Substituto proSub = new Produto_Composicao_Substituto();

                    foreach (var item in olist)
                    {
                        cmd.Parameters[0].Value = item.procodigo;

                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (item.olistComposicao.Find(p => p.procodigo_default == reader["procodigo_default"].GenericConvert<int>()) == null)
                                {
                                    procomp = new Produto_Composicao();
                                    procomp.pcpcodigo = reader["pcpcodigo"].GenericConvert<int>();
                                    procomp.procodigo_pai_default = reader["procodigo_pai_default"].GenericConvert<int>();
                                    procomp.procodigo_default = reader["procodigo_default"].GenericConvert<int>();
                                    procomp.prodescricao_default = reader["prodescricao_default"].ToString().Trim();
                                    procomp.pcpadicional = reader["pcpadicional"].ToString().Trim();
                                    procomp.pcpisento_cobranca = reader["pcpisento_cobranca"].ToString().Trim();
                                    item.olistComposicao.Add(procomp);
                                }

                                procomp = item.olistComposicao.Find(p => p.procodigo_default == reader["procodigo_default"].GenericConvert<int>());

                                if (procomp != null && procomp.olistSubstituto.Find(p => p.procodigo_troca == reader["procodigo_troca"].GenericConvert<int>() && p.procodigo_pai_composicao == reader["procodigo_pai_composicao"].GenericConvert<int>()) == null)
                                {
                                    proSub = new Produto_Composicao_Substituto();
                                    proSub.procodigo_pai_composicao = reader["procodigo_pai_composicao"].GenericConvert<int>();
                                    proSub.procodigo_troca = reader["procodigo_troca"].GenericConvert<int>();
                                    proSub.prodescricao_troca = reader["prodescricao_troca"].ToString().Trim();
                                    proSub.pcsbvalor_adicional = reader["pcsbvalor_adicional"].GenericConvert<decimal>();
                                    procomp.olistSubstituto.Add(proSub);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            finally
            {
                db.Close();
            }

            return Ok(olist);
        }
        /// <summary>
        /// Buscar item do estoque por procodigo.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///			{
        ///			  "procodigo": 18;
        ///           "prodescricao": "Camarão à paulista";
        ///           "proprecounita": 18.99;
        ///           "prounidade": UND;
        ///           "procodimpfis": "I00";
        ///           "protipo": 1;
        ///           "prostatus": A;
        ///           "ffscodigo": 2;
        ///           "proncm": 21069090 ;
        ///           "procodbarra": null;
        ///           "cestcodigo": 728;
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("InserirProduto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProdutoSCH> InserirProdutoSCH(ProdutoSCH produtoSCH)
        {
            try
            {
                return Ok(new ProdutoRepositorio().InserirProdutoSCH(produtoSCH));
            }
            catch (Exception ex)
            {
                return NotFound("Não foi possível inserir o produto." + ex.Message);
            }
        }
        /// <summary>
        /// Update do produto.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///			{
        ///			  "procodigo": 18,
        ///           "cdpcodigo": "1",
        ///           "embcodigo": "UND",
        ///           "proadicional": "",
        ///           "latcodigo_preparo": 1
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPatch]
        [ActionName("AtualizarProduto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Produto> UpdateProduto(Produto produto)
        {
            try
            {
                return Ok(new ProdutoRepositorio().UpdateProduto(produto));
            }
            catch(Exception ex)
            {
                return NotFound("Não foi possível atualizar o produto: " + ex.Message);
            }
        }
    }
}
