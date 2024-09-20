using log_food_api.Models;
using log_food_api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace log_food_api.Controllers
{
    [ApiController]
    [Authorize]
    public class ComandaitemController : Controller
    {
        /// <summary>
        /// Atualizar os status dos itens na comanda.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        /// 
        ///     [
        ///       {
        ///         "cmdcodigo": 254, /* Código da Comanda */
        ///         "cmdicodigo": 1450, /* Código do Item na Comanda */
        ///         "cmdistatus": "E" /* B-Bloqueado A-Aberto W-EmPreparoCozinha Z-FimPreparoCozinha E-Entregue X-Excluído  */
        ///       },
        ///       {
        ///         "cmdcodigo": 254,
        ///         "cmdicodigo": 1455,
        ///         "cmdistatus": "E"
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AtribuirStatusItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ComandaItem>> SetStatusItem([FromBody] List<ComandaItemStatus> json)
        {
            if (json == null || json.Count == 0)
            {
                return NotFound("Obrigatório informar itens!");
            }

            List<ComandaItem> olist = new List<ComandaItem>();

            var db = Db.GetDbFood();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);
                
                var comandaRep = new ComandaRepositorio();
                
                var comanda = comandaRep.GetComanda(db, json[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                foreach (var item in json)
                {
                    var proc = comanda.olistItens.Find(p => p.cmdicodigo == item.cmdicodigo);

                    if (proc != null)
                    {
                        if (
                            proc.usucodigo_atendimento == null || proc.usucodigo_atendimento == 0
                            )
                        {
                            proc.usucodigo_atendimento = nusucodigo;
                        }

                        proc.cmdistatus = item.cmdistatus;
                        proc.usucodigo = nusucodigo;

                        olist.Add(proc);
                    }
                }

                var transa = db.StartTransaction();

                var itemRep = new ComandaItemRepositorio();

                olist = itemRep.SetStatus(db, olist);
                
                comandaRep.TotalizarComanda(db, json[0].cmdcodigo, nusucodigo);
                
                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok(olist);
        }
        [HttpPost]
        [Route("PedidoVendaAtribuirStatusItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ComandaItem>> PedidoVendaSetStatusItem([FromBody] List<ComandaItemStatus> json)
        {
            if (json == null || json.Count == 0)
            {
                return NotFound("Obrigatório informar itens!");
            }

            List<ComandaItem> olist = new List<ComandaItem>();

            var db = Db.GetDbFood();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);

                var comandaRep = new ComandaRepositorio();

                var comanda = comandaRep.GetComandaPedidoVenda(db, json[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                foreach (var item in json)
                {
                    var proc = comanda.olistItens.Find(p => p.cmdicodigo == item.cmdicodigo);

                    if (proc != null)
                    {
                        if (
                            proc.usucodigo_atendimento == null || proc.usucodigo_atendimento == 0
                            )
                        {
                            proc.usucodigo_atendimento = nusucodigo;
                        }

                        proc.cmdistatus = item.cmdistatus;
                        proc.usucodigo = nusucodigo;

                        olist.Add(proc);
                    }
                }

                var transa = db.StartTransaction();

                var itemRep = new ComandaItemRepositorio();

                olist = itemRep.PedidoVendaSetStatus(db, olist);

                comandaRep.TotalizarComanda(db, json[0].cmdcodigo, nusucodigo);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok(olist);
        }

        /// <summary>
        /// Atualizar a observação do item na comanda.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        /// 
        ///     [
        ///       {
        ///         "cmdcodigo": 254, /* Código da Comanda */
        ///         "cmdicodigo": 1450, /* Código do Item na Comanda */
        ///         "cmdiobservacao": "Sem sal"
        ///       },
        ///       {
        ///         "cmdcodigo": 254,
        ///         "cmdicodigo": 1455,
        ///         "cmdiobservacao": "Sem tucupi"
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AtribuirObsItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ComandaItem>> SetObservacaoItem([FromBody] List<ComandaItemStatus> json)
        {
            if (json == null || json.Count == 0)
            {
                return NotFound("Obrigatório informar itens!");
            }

            List<ComandaItem> olist = new List<ComandaItem>();

            var db = Db.GetDbFood();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);

                var comanda = new ComandaRepositorio().GetComanda(db, json[0].cmdcodigo);
                
                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                foreach (var item in json)
                {
                    var proc = comanda.olistItens.Find(p => p.cmdicodigo == item.cmdicodigo);

                    if (proc != null)
                    {
                        if (proc.usucodigo_atendimento == null || proc.usucodigo_atendimento == 0)
                        {
                            proc.usucodigo_atendimento = nusucodigo;
                        }

                        proc.cmdiobservacao = item.cmdiobservacao;
                        proc.usucodigo = nusucodigo;

                        olist.Add(proc);
                    }
                }

                var transa = db.StartTransaction();

                var itemRep = new ComandaItemRepositorio();

                olist = itemRep.SetObservacao(db, olist);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok(olist);
        }



        /// <summary>
        /// Inserir itens na comanda.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        /// 
        ///     [
        ///       {
        ///         "cmdicodigo": 0,
        ///         "cmdcodigo": 365,
        ///         "usucodigo_atendimento": 5,
        ///         "procodigo": 20,
        ///         "precodigo": 0,
        ///         "cmdivalor": 30,
        ///         "cmdinumero_origem": 0,
        ///         "cmdiobservacao": "Sem cebola",
        ///         "cmdistatus": "B", /*  B-Bloqueado A-Aberto E-Entregue  */
        ///         "cmdipreparo": "S"
        ///       },
        ///       {
        ///         "cmdicodigo": 0,
        ///         "cmdcodigo": 365,
        ///         "usucodigo_atendimento": 5,
        ///         "procodigo": 25,
        ///         "precodigo": 0,
        ///         "cmdivalor": 57,
        ///         "cmdinumero_origem": 0,
        ///         "cmdistatus": "B", /*  B-Bloqueado A-Aberto E-Entregue  */
        ///         "cmdipreparo": "N"
        ///       }
        ///     ]
        /// </remarks>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("InserirItensComanda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ComandaItem>> InserirItensComanda([FromBody] List<ComandaItem> json)
        {
            if (json == null || json.Count == 0)
            {
                return NotFound("É Obrigatório informar ao menos um item!");
            }

            List<ComandaItem> olist = new List<ComandaItem>();

            var db = Db.GetDbFood();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);
                var comandaRep = new ComandaRepositorio();

                var transa = db.StartTransaction();

                var comanda = comandaRep.GetComanda(db, json[0].cmdcodigo);

                if (comanda.cmdcodigo == 0 || comanda.cmdstatus != "A")
                {
                    throw new Exception("A Comanda não está Aberta!");
                }

                var local = new LocalAtendimentoRepositorio().GetLocalAtendimento(db, comanda.latcodigo);

                if (local.latstatus != "O")
                {
                    throw new Exception("O Local de Atendimento não está disponível!");
                }

                foreach (var item in json)
                {
                    if (item.usucodigo_atendimento == null || item.usucodigo_atendimento == 0)
                    {
                        item.usucodigo_atendimento = nusucodigo;
                    }

                    item.usucodigo = nusucodigo;
                }

                new ComandaItemRepositorio().InserirItens(db, json);

                var itemRep = new ComandaItemRepositorio();

                /*foreach (var item in json)
                {
                    itemRep.SetStatus(db, item);
                }*/

                //olist = new ComandaRepositorio().GetComanda(db, json[0].cmdcodigo).olistItens;

                olist = itemRep.SetStatus(db, json);

                comandaRep.TotalizarComanda(db, comanda.cmdcodigo, nusucodigo);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok(olist);
        }
        [HttpPost]
        [Route("InserirItensPedidoVenda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<ComandaItem>> InserirItensPedidoVenda([FromBody] List<ComandaItem> json)
        {
            if (json == null || json.Count == 0)
            {
                return NotFound("É Obrigatório informar ao menos um item!");
            }

            List<ComandaItem> olist = new List<ComandaItem>();

            var db = Db.GetDbFood();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);
                var comandaRep = new ComandaRepositorio();

                var transa = db.StartTransaction();

                var comanda = comandaRep.GetComanda(db, json[0].cmdcodigo);

                if (comanda.cmdcodigo == 0 || comanda.cmdstatus != "A")
                {
                    throw new Exception("A Comanda não está Aberta!");
                }

                var local = new LocalAtendimentoRepositorio().GetLocalAtendimento(db, comanda.latcodigo);

                if (local.latstatus != "O")
                {
                    throw new Exception("O Local de Atendimento não está disponível!");
                }

                foreach (var item in json)
                {
                    if (item.usucodigo_atendimento == null || item.usucodigo_atendimento == 0)
                    {
                        item.usucodigo_atendimento = nusucodigo;
                    }

                    item.usucodigo = nusucodigo;
                }

                new ComandaItemRepositorio().InserirItensPedidoVenda(db, json);

                var itemRep = new ComandaItemRepositorio();

                /*foreach (var item in json)
                {
                    itemRep.SetStatus(db, item);
                }*/

                //olist = new ComandaRepositorio().GetComanda(db, json[0].cmdcodigo).olistItens;

                olist = itemRep.SetStatus(db, json);

                comandaRep.TotalizarComandaPedidoVenda(db, comanda.cmdcodigo, nusucodigo);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok(olist);
        }

    }
}