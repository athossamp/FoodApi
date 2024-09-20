using Azure.Core;
using log_food_api.Models;
using log_food_api.Repositories;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Text.Json;

namespace log_food_api.Controllers
{
    [ApiController]
    [Authorize]
    public class ComandaController : Controller
    {
        /// <summary>
        /// Criar uma comanda.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///			{
        ///			   "cmdcodigo":0,
        ///			   "latcodigo":5,
        ///			   "vencodigo_abertura":1,
        ///			   "cmdobservacao":"Teste de observação no cabeçalho",
        ///			   "cmdstatus":"A",
        ///			   "usucodigo":1,
        ///			   "itens":[
        ///			      {
        ///			         "cmdicodigo":0,
        ///			         "cmdcodigo":0,
        ///			         "usucodigo_atendimento":1,
        ///			         "procodigo":15,
        ///			         "precodigo":0,
        ///			         "cmdivalor":68,
        ///			         "cmdiobservacao":"Sem cebola",
        ///			         "cmdistatus":"A",
        ///			         "cmdinumero_origem":0,
        ///			         "cmdipreparo":"S",
        ///			         "usucodigo":1,
        ///			         "composicao":[
        ///			            {
        ///			               "procodigo":3,
        ///			               "procodigo_composicao":4
        ///			            },
        ///			            {
        ///			               "procodigo":14,
        ///			               "procodigo_composicao":14
        ///			            }
        ///			         ]
        ///			      },
        ///			      {
        ///			         "cmdicodigo":0,
        ///			         "cmdcodigo":0,
        ///			         "usucodigo_atendimento":1,
        ///			         "procodigo":21,
        ///			         "precodigo":0,
        ///			         "cmdivalor":11,
        ///			         "cmdiobservacao":"Sem açúcar",
        ///			         "cmdistatus":"A",
        ///			         "cmdinumero_origem":0,
        ///			         "cmdipreparo":"S",
        ///			         "usucodigo":1,
        ///							"composicao":[]
        ///			      },
        ///			      {
        ///			         "cmdicodigo":0,
        ///			         "cmdcodigo":0,
        ///			         "usucodigo_atendimento":1,
        ///			         "procodigo":16,
        ///			         "precodigo":0,
        ///			         "cmdivalor":6,
        ///			         "cmdiobservacao":"",
        ///			         "cmdistatus":"A",
        ///			         "cmdinumero_origem":0,
        ///			         "usucodigo":1,
        ///							"composicao":[]
        ///			      }
        ///			   ]
        ///			}
        /// </remarks>
        /// <param name="json"></param>
        /// <response code="200">Comanda criada.</response>
        /// <response code="404">Comanda não criada e/ou erro</response>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Route("InserirComanda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Comanda> InserirComanda([FromBody] Comanda comanda)
        {
            TLogDatabase db = Db.GetDbFood();

            try
            {
                var comandaRep = new ComandaRepositorio();
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);

                var transa = db.StartTransaction();

                if (comanda.cmdcodigo == 0)
                {
                    comanda.cmdstatus = "A";
                    comanda.cmdcodigo = db.GetGenerator("cmdcodigo");
                }

                var localAtend = new LocalAtendimento();

                //valida se local está disponível
                using (DbCommand cmd = db.NewCommand("select * from local_atendimento where latcodigo = @latcodigo and lattipo <> 'I'"))
                {
                    db.AddInt16Parameter(cmd, "@latcodigo").Value = comanda.latcodigo;
                    localAtend = cmd.FillList<LocalAtendimento>().FirstOrDefault();
                }

                if (localAtend.latcodigo == 0)
                {
                    throw new Exception("Local de Atendimento inválido!");
                }
                
                
                // Retirado método para conseguir inserir mais de uma comanda na mesma mesa.
                
                //if (localAtend.latstatus != "D")
                //{
                //    throw new Exception("Local de Atendimento Não Está Disponível!");
                //}

                using (DbCommand cmd = db.NewCommand("update local_atendimento set latstatus = 'O', usucodigo = @usucodigo, ultatualizacao=now() where latcodigo = @latcodigo"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddInt16Parameter(cmd, "@latcodigo").Value = comanda.latcodigo;
                    db.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                    cmd.ExecuteNonQuery();
                }

                //adicionar validacao no regficha

                using (DbCommand cmd = db.NewCommand("INSERT INTO comanda (cmdcodigo, latcodigo, cmdapartamento, usucodigo_abertura, cmdobservacao, cmdvalor_total, cmdvalor_taxa_servico, cmdvalor_pago, cmdstatus, cmdabertura, cmdfechamento, cmdnumero_origem, transa, usucodigo, ultatualizacao, loccodigo, depcodigo, cpgcodigo, fpgcodigo, cmdtipo) VALUES(@cmdcodigo, @latcodigo, @cmdapartamento, @usucodigo_abertura, @cmdobservacao, 0.00, 0.00, 0.00, @cmdstatus, now(), null, @cmdnumero_origem, null, @usucodigo, now(), @loccodigo, @depcodigo, @cpgcodigo, @fpgcodigo, @cmdtipo);"))
                {
                    cmd.Transaction = transa;

                    if (comanda.usucodigo_abertura == 0)
                    {
                        comanda.usucodigo_abertura = nusucodigo;
                    }

                    db.AddIntParameter(cmd, "@cmdcodigo").Value = comanda.cmdcodigo;
                    db.AddInt16Parameter(cmd, "@latcodigo").Value = comanda.latcodigo;
                    db.AddIntParameter(cmd, "@cmdapartamento").Value = comanda.cmdapartamento == null ? DBNull.Value : comanda.cmdapartamento;
                    db.AddInt16Parameter(cmd, "@usucodigo_abertura").Value = comanda.usucodigo_abertura;
                    db.AddStringParameter(cmd, "@cmdobservacao").Value = comanda.cmdobservacao == null ? DBNull.Value : comanda.cmdobservacao;
                    db.AddStringParameter(cmd, "@cmdstatus").Value = comanda.cmdstatus;
                    db.AddIntParameter(cmd, "@cmdnumero_origem").Value = comanda.cmdnumero_origem == null ? DBNull.Value : comanda.cmdnumero_origem;
                    db.AddIntParameter(cmd, "@transa").Value = comanda.transa == null ? DBNull.Value : comanda.transa;
                    db.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                    db.AddInt16Parameter(cmd, "@loccodigo").Value = comanda.loccodigo;
                    db.AddInt16Parameter(cmd, "@depcodigo").Value = comanda.depcodigo;
                    db.AddIntParameter(cmd, "@cpgcodigo").Value = comanda.cpgcodigo;
                    db.AddIntParameter(cmd, "@fpgcodigo").Value = comanda.fpgcodigo;
                    db.AddStringParameter(cmd, "@cmdtipo").Value = comanda.cmdtipo;
                    cmd.ExecuteNonQuery();
                }

                foreach (var item in comanda.olistItens)
                {
                    if (item.usucodigo_atendimento == null || item.usucodigo_atendimento == 0)
                    {
                        item.usucodigo_atendimento = nusucodigo;
                    }

                    item.usucodigo = nusucodigo;
                }

                if (comanda.cmdcodigo < 1)
                {
                    return NotFound("Erro ao inserir Comanda!");
                }

                foreach (var item in comanda.olistItens)
                {
                    item.cmdcodigo = comanda.cmdcodigo;
                }

                //insere itens
                new ComandaItemRepositorio().InserirItens(db, comanda.olistItens);

                // atualiza status do local de atendimento
                new LocalAtendimentoRepositorio().UpdateStatus(db, comanda.latcodigo, "O");

                comandaRep.TotalizarComanda(db, comanda.cmdcodigo, nusucodigo);

                comanda = comandaRep.GetComanda(db, comanda.cmdcodigo);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return comanda;

        }

        /// <summary>
        /// Fechar uma comanda após pagamento/faturamento.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        /// 
        ///{
        ///		"latcodigo": 4,
        ///		"cmdtaxa": true, /*Enviar true para pagar 10%, false para não pagar*/
        ///    "cmdcodigo": 30,
        ///    "valor_total_sem_servico": 11,
        ///    "valor_taxa_servico": 1.1 /* Valor dos 10% */
        ///}
        /// </remarks>
        /// <param name="json"></param>
        /// <response code="200">Comanda para com sucesso.</response>
        /// <response code="404">Pagamento Não Efetuado e/ou erro.</response>
        /// <returns></returns>
        /// 


    //[HttpPost]
    //    [Produces("application/json")]
    //    [Route("ProcessarNFCE")]
    //    [ProducesResponseType(StatusCodes.Status200OK)]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    public void ProcessaNFCE([FromBody] ComandaPagamento comandaPag)
    //    {
    //        //TLogDatabase db = Db.GetDbSCH();
    //        try
    //        {
    //            var comandaRepositorio = new ComandaRepositorio();
    //            comandaRepositorio.EfetivarPagamentoComanda(comandaPag);

    //        }
    //        catch (Exception e)
    //        {
    //            throw;
    //        }
    //        //ComandaRepositorio.ProcessarNFCE(empcodigo.Empcodigo, 1036137);

    //    }

        [HttpPost]
        [Produces("application/json")]
        [Route("FecharComanda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult<Comanda> FinalizarComanda([FromBody] Comanda comanda)
        //public ActionResult<Comanda> FinalizarComanda([FromBody] ComandaPagamento json)
        public ActionResult<ComandaPagamentoRetorno> FinalizarComanda([FromBody] ComandaPagamento json)
        {
            TLogDatabase db = Db.GetDbFood();
            var comanda = new Comanda();
            var comandaPagRet = new ComandaPagamentoRetorno();

            try
            {
                comanda = new ComandaRepositorio().GetComanda(db, json.cmdcodigo);
                
                var transa = db.StartTransaction();

                if (comanda.cmdcodigo < 1)
                {
                    throw new Exception("Comanda Inexistente!");
                }

                if (comanda.cmdstatus == "F")
                {
                    throw new Exception("Comanda Já Foi Fechada!");
                }

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                var localAtend = new LocalAtendimento();

                //valida se local está disponível
                using (DbCommand cmd = db.NewCommand("select latcodigo, latstatus from local_atendimento where latcodigo = @latcodigo and lattipo <> 'I'"))
                {
                    db.AddInt16Parameter(cmd, "@latcodigo").Value = comanda.latcodigo;
                    localAtend = cmd.FillList<LocalAtendimento>().FirstOrDefault();
                }

                if (localAtend.latcodigo == 0)
                {
                    throw new Exception("Local de atendimento inválido!");
                }
                if (localAtend.latstatus != "O")
                {
                    throw new Exception("Local de atendimento não está ocupado!");
                }


                //validar valor total com valor pago
                //validar valor faturado com valor pago/faturado

                /*COLOCAR VERIFICAÇÃO PARA A QUESTÃO DE CONTA CONJUNSTA OU SEPARADA
                 VERIFICAR NA HORA DE ENVIAR PARA O BANCO DO SAGRES
                CASO CONJUNTA FAZER A QUERY:   select c.cmdapartamento, sum(c.cmdvalor_total) from comanda c group by c.cmdapartamento 
                e enviar para o banco do sagres a conta após faturamento
                CASO SEPARADA FAZER A QUERY SEM O SUM.*/
               
                    var hospedeCheck = new ApartamentoRepositorio().GetApartamentoByApto(comanda.cmdapartamento);
                    decimal totalValorPago = 0;
                    /*Verifica se é hóspede (possui um apartamento)*/
                    if (comanda.cmdapartamento != 0)
                    {
                        /*
                         * se a conta não for de apartamento, só gerar o cmdvalor_pago e emitir o cupom
                        * Verificar se é conta conjunta
                         
                        se for separada pega o valor do comanda.cmdvalor_pago e envia para o banco do sagres e altera status pra F
                        
                        adicionar verificação na hora de criar comanda para criar só uma comanda por apartamento se for conta conjunta.

                        se for conjunta iterar por todas as comandas da mesa com status A (antes de fechar a comanda)
                        
                        somar todos os valores e AÍ ENTÃO alterar para o status F.*/
                        if (hospedeCheck.rapjuntosep == "J")
                        {
                            using (DbCommand cmd = db.NewCommand("select c.cmdapartamento, c.cmdnumero_origem, c.cmdcodigo, c.cmdvalor_total, c.latcodigo  from comanda c where c.cmdapartamento = @cmdapartamento and c.cmdstatus = 'A' and c.cmdcodigo = @cmdcodigo"))
                            {
                                cmd.Transaction = transa;
                                db.AddIntParameter(cmd, "@cmdapartamento").Value = comanda.cmdapartamento;
                                db.AddIntParameter(cmd, "@cmdcodigo").Value = comanda.cmdcodigo;

                            using (DbDataReader reader = cmd.ExecuteReader())
                            {
                                Comanda comandas = new Comanda();
                                //TODO: Colocar cmdtaxa como valor parametrizado para taxa de serviço
                                //Criar um get de parametros e pegar o valor que tem lá para fazer as contas
                                var cabCupomRet = new CabCupomRetorno();
                            }
                                var comandaRepositorio = new ComandaRepositorio();
                                comandaRepositorio.EfetivarPagamentoComanda(json);
                            }
                        }
                        else
                        {
                        /*Adicionar função que irá mandar os dados da comanda para o banco do sagres sem somar os valores
                         tendo em vista que é uma conta separada*/
                        using (DbCommand cmd = db.NewCommand("select c.cmdapartamento, c.cmdnumero_origem, c.cmdcodigo, c.cmdvalor_total, c.latcodigo  from comanda c where c.cmdapartamento = @cmdapartamento and c.cmdstatus = 'A' and c.cmdcodigo = @cmdcodigo"))
                        {
                            cmd.Transaction = transa;
                            db.AddIntParameter(cmd, "@cmdapartamento").Value = comanda.cmdapartamento;
                            db.AddIntParameter(cmd, "@cmdcodigo").Value = comanda.cmdcodigo;
                            using (DbDataReader reader = cmd.ExecuteReader())
                            {
                                Comanda comandas = new Comanda();
                            }

                        }
                        var comandaRepositorio = new ComandaRepositorio();
                        comandaRepositorio.EfetivarPagamentoComanda(json);
                    }

                    }
                else
                {
                    using (DbCommand cmd = db.NewCommand("select c.cmdapartamento, c.cmdcodigo, c.cmdvalor_total, c.latcodigo  from comanda c where c.cmdcodigo = @cmdcodigo and c.cmdstatus = 'A'"))
                    {
                        cmd.Transaction = transa;
                       
                        db.AddIntParameter(cmd, "@cmdcodigo").Value = comanda.cmdcodigo;
                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            Comanda comandas = new Comanda();
                            var comandaRepositorio = new ComandaRepositorio();
                            comandaRepositorio.EfetivarPagamentoComanda(json);
                        }

                    }
                }
                /* Fazer verificação do tipo de pagamento,
                 * se for dinheiro o garçom n pode fechar (tem que ir pra gerencia),
                 * se for cartão ou faturado pode
                 * TODO: pedir no json pra vir além do tipo de pagamento (à vista ou faturado) o tipo de pagamento de fato
                 dinheiro, cartão, etc*/
                var garcomCheck = new UsuarioRepositorio().GetByCodigo((short)comanda.usucodigo, db);
                if(json.tipo_pagamento == "DINHEIRO" && garcomCheck.usutipo != "G")
                {
                    return NotFound("Apenas a gerencia pode receber em dinheiro!");
                } else
                {
                    using (DbCommand cmd = db.NewCommand("update comanda set cmdstatus = 'F', cmdfechamento = NOW(), cmdvalor_pago = @cmdvalor_pago, cmdvalor_taxa_servico = @cmdvalor_taxa_servico where cmdcodigo = @cmdcodigo"))
                    {
                        cmd.Transaction = transa;
                        db.AddIntParameter(cmd, "@cmdcodigo").Value = comanda.cmdcodigo;
                        db.AddFloatParameter(cmd, "@cmdvalor_pago").Value = json.cmdvalor_pago;
                        db.AddFloatParameter(cmd, "@cmdvalor_taxa_servico").Value = (comanda.cmdvalor_total / 10);
                        cmd.ExecuteNonQuery();
                    }

                    if (comanda.cmdcodigo < 1)
                    {
                        return NotFound("Erro ao finalizar Comanda!");
                    }
                }

                comanda = new ComandaRepositorio().GetComanda(db, comanda.cmdcodigo);

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            comandaPagRet.cmdstatus = comanda.cmdstatus;
            comandaPagRet.valor_total_pago = comanda.cmdvalor_pago;
            var comandaCheck = new ComandaRepositorio().GetComandaPorMesa(db, json.latcodigo);
            bool comandaSemStatusA = comandaCheck.All(c => c.cmdstatus != "A");
            if (comandaSemStatusA)
            {
                using (DbCommand cmd = db.NewCommand("update local_atendimento set latstatus = 'D', ultatualizacao = NOW() where latcodigo = @latcodigo"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddInt16Parameter(cmd, "@latcodigo").Value = comanda.latcodigo;
                    cmd.ExecuteNonQuery();
                }
            }
            return comandaPagRet;

        }

        /// <summary>
        /// Excluir uma comanda.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     {        
        ///       "cmdcodigo": 123
        ///     }
        /// </remarks>
        /// /// <param name="json"></param> 
        /// <response code="200">Confirmação da Comanda Excluída</response>
        /// <response code="404">Erro ao excluir a Comanda.</response>
        [HttpDelete]
        [Route("ExcluirComanda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult ExcluirComanda([FromBody] JsonElement json)
        {
            var cmdcodigo = json.GetProperty("cmdcodigo");

            if (cmdcodigo.ValueKind == JsonValueKind.Null || cmdcodigo.ValueKind != JsonValueKind.Number)
            {
                return NotFound("Obrigatório informar o Id da Comanda.");
            }

            var db = Db.GetDbFood();
            var comandaItemRep = new ComandaItemRepositorio();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);

                var transa = db.StartTransaction();

                using (DbCommand cmd = db.NewCommand(@"select
                      c.cmdstatus,
                      la.latnome
                      from comanda c
                      inner join local_atendimento la 
                      on (c.latcodigo = la.latcodigo)
                      where c.cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = transa;
                    db.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo.GetInt32();

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader[0].ToString() != "A")
                            {
                                return NotFound(string.Concat("A comanda não está aberta!",
                                    Environment.NewLine,
                                    "Número: " + cmdcodigo.GetInt32().ToString("N0"),
                                    Environment.NewLine,
                                    reader["latnome"]));
                            }
                        }
                        else
                        {
                            throw new Exception(string.Concat("Comanda inexistente!",
                                    Environment.NewLine,
                                    "Número: " + cmdcodigo.GetInt32().ToString("N0")));
                        }
                    }
                }

                var comanda = new ComandaRepositorio().GetComanda(db, cmdcodigo.GetInt32());

                if (comanda.olistItens.Exists(p => p.cmdistatus == "E"))
                {
                    throw new Exception("Comanda possui Itens Entregues!" +
                        Environment.NewLine +
                        "Cancelamento inválido.");
                }

                List<ComandaItem> olistUpd = new List<ComandaItem>();

                olistUpd.AddRange(comanda.olistItens);

                foreach (var item in olistUpd)
                {
                    item.usucodigo = nusucodigo;
                    item.cmdistatus = "X";
                }

                // atualiza status dos itens da comanda para excluído
                comandaItemRep.SetStatus(db, olistUpd);

                // atualiza status da comanda para excluído
                using (DbCommand cmd = db.NewCommand("update comanda set cmdstatus = 'X', usucodigo=@usucodigo, ultatualizacao = now() where cmdcodigo = @cmdcodigo and cmdstatus <> 'F'"))
                {
                    cmd.Transaction = transa;
                    db.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo.GetInt32();
                    db.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                    cmd.ExecuteNonQuery();
                }

                //disponibiliza local de atendimento
                new LocalAtendimentoRepositorio().UpdateStatus(db, comanda.latcodigo, "D");

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok("Comanda excluída com sucesso!");
        }

        /// <summary>
        /// Lista com comandas abertas/ativas.
        /// </summary>
        /// <remarks>
        /// Exemplo para retornar comanda específica:
        ///
        ///     {        
        ///       "cmdcodigo": 123
        ///     }
        ///     
        /// 
        /// Exemplo para retornar todas as comandas:
        ///
        ///     {        
        ///       "cmdcodigo": 0
        ///     }
        /// </remarks>
        /// /// <param name="json"></param> 
        /// <response code="200">Confirmação da Comanda Excluída</response>
        /// <response code="404">Erro ao excluir a Comanda.</response>
        [HttpPost]
        [Route("ConsultarComandasAbertas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Comanda>> GetComandasAbertas([FromBody] JsonElement json)
        {
            List<Comanda> olist = new List<Comanda>();

            var db = Db.GetDbFood();
            DbCommand cmdList = db.NewCommand(@"select 
                 c.*
                ,la.latnome 
                from comanda c 
                inner join local_atendimento la
                on (c.latcodigo = la.latcodigo)
                where cmdstatus = 'A' order by cmdabertura");

            try
            {
                if (json.ValueKind != JsonValueKind.Undefined)
                {
                    var cmdcodigo = json.GetProperty("cmdcodigo");

                    if (cmdcodigo.ValueKind != JsonValueKind.Null && cmdcodigo.ValueKind == JsonValueKind.Number && cmdcodigo.GetInt32() > 0)
                    {
                        cmdList = db.NewCommand(@"select 
                             c.*
                            ,la.latnome 
                            from comanda c 
                            inner join local_atendimento la
                            on (c.latcodigo = la.latcodigo)
                            where cmdstatus = 'A' and c.cmdcodigo = @cmdcodigo order by cmdabertura");
                        db.AddIntParameter(cmdList, "@cmdcodigo").Value = cmdcodigo.GetInt32();
                    }
                }

                olist = cmdList.FillList<Comanda>();

                using (var cmdItens = db.NewCommand(@"select 
                         ci.*
                        ,p.prodescricao
                        ,p.latcodigo_preparo
                        ,la.latnome as latnome_preparo
                        from comanda_item ci
                        inner join produto p
                        on (ci.procodigo = p.procodigo)
                        left join local_atendimento la 
                        on (p.latcodigo_preparo = la.latcodigo)
                        where ci.cmdcodigo = @cmdcodigo 
                        and ci.cmdistatus <> 'X'"))
                {
                    db.AddIntParameter(cmdItens, "@cmdcodigo");

                    foreach (var item in olist)
                    {
                        cmdItens.Parameters[0].Value = item.cmdcodigo;
                        item.olistItens = cmdItens.FillList<ComandaItem>();
                    }
                }

                using (var cmdItens = db.NewCommand(@"
                        select
                       cic.*
                      ,p.prodescricao
                      from comanda_item ci 
                      inner join comanda_item_composicao cic
                      on (ci.cmdicodigo = cic.cmdicodigo)
                      inner join produto p
                      on (cic.procodigo_composicao = p.procodigo)
                      where ci.cmdicodigo = @cmdicodigo
                      and ci.cmdistatus <> 'X'"))
                {
                    db.AddIntParameter(cmdItens, "@cmdicodigo");

                    foreach (var item in olist)
                    {
                        foreach (var itemComanda in item.olistItens)
                        {
                            cmdItens.Parameters[0].Value = itemComanda.cmdicodigo;
                            itemComanda.olistComposicao = cmdItens.FillList<ComandaItemComposicao>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotFound("Nao autorizado" + ex);
            }
            finally
            {
                if (cmdList != null)
                {
                    cmdList.Dispose();
                }
            }

            return Ok(olist);
        }

        /// <summary>
        /// Lista com comandas abertas/ativas.
        /// </summary>
        /// <remarks>
        /// Exemplo para retornar comandas de Local de Atendimento específico:
        ///
        ///     {        
        ///       "id": "COZINHA" /* COZINHA/COPA */
        ///     }
        ///     
        /// 
        /// Exemplo para retornar todas as comandas:
        ///
        ///     {        
        ///       "id": ""
        ///     }
        /// </remarks>
        /// /// <param name="json"></param> 
        /// <response code="200">Confirmação da Comanda Excluída</response>
        /// <response code="404">Erro ao excluir a Comanda.</response> 
        [HttpPost]
        [Route("ConsultarComandasLocalAtend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Comanda>> GetComandasLocalAtend([FromBody] JsonElement json)
        {
            List<Comanda> olist = new List<Comanda>();

            var db = Db.GetDbFood();
            DbCommand cmdList = db.NewCommand(@"
                select distinct
                  c.*
                 ,la.latnome 
                  from comanda c
                  inner join comanda_item ci 
                  on (c.cmdcodigo = ci.cmdcodigo)
                  inner join produto p 
                  on (ci.procodigo = p.procodigo)
                  inner join local_atendimento la 
                  on (c.latcodigo = la.latcodigo)
                  where cmdstatus = 'A'
                  and p.latcodigo_preparo > 0
                  order by cmdabertura");

            try
            {
                var id = new JsonElement();

                if (json.ValueKind != JsonValueKind.Undefined)
                {
                    id = json.GetProperty("id");

                    if (id.ValueKind != JsonValueKind.Null && id.ValueKind == JsonValueKind.String && id.GetString().Length > 0)
                    {
                        cmdList = db.NewCommand(@"
                            select distinct
                              c.*
                             ,la.latnome 
                              from comanda c
                              inner join comanda_item ci 
                              on (c.cmdcodigo = ci.cmdcodigo)
                              inner join produto p 
                              on (ci.procodigo = p.procodigo)
                              inner join local_atendimento la 
                              on (c.latcodigo = la.latcodigo)
                              inner join local_atendimento la2 
                              on (p.latcodigo_preparo  = la2.latcodigo)
                              where cmdstatus = 'A'
                              and p.latcodigo_preparo > 0
                              and la2.latchave = @id
                              order by cmdabertura");
                        db.AddStringParameter(cmdList, "@id").Value = id.GetString();
                    }
                }

                olist = cmdList.FillList<Comanda>();

                using (var cmdItens = db.NewCommand(@"select 
                         ci.*
                        ,p.prodescricao
                        ,p.latcodigo_preparo
                        ,la.latnome as latnome_preparo
                        from comanda_item ci
                        inner join produto p
                        on (ci.procodigo = p.procodigo)
                        inner join local_atendimento la 
                        on (p.latcodigo_preparo = la.latcodigo)
                        where ci.cmdcodigo = @cmdcodigo 
                        and ci.cmdistatus <> 'X'
                        and p.latcodigo_preparo > 0"))
                {
                    if (id.ValueKind != JsonValueKind.Null && id.GetString().Length > 0)
                    {
                        cmdItens.CommandText += " and la.latchave = @id";
                        db.AddStringParameter(cmdItens, "@id").Value = id.GetString();
                    }

                    db.AddIntParameter(cmdItens, "@cmdcodigo");

                    foreach (var item in olist)
                    {
                        cmdItens.Parameters["cmdcodigo"].Value = item.cmdcodigo;
                        item.olistItens = cmdItens.FillList<ComandaItem>();
                    }
                }

                using (var cmdItens = db.NewCommand(@"
                        select
                         cic.*
                        ,p.prodescricao
                        from comanda_item ci 
                        inner join comanda_item_composicao cic
                        on (ci.cmdicodigo = cic.cmdicodigo)
                        inner join produto p
                        on (cic.procodigo_composicao = p.procodigo)
                        inner join local_atendimento la 
                        on (p.latcodigo_preparo = la.latcodigo)
                        where ci.cmdicodigo = @cmdicodigo
                        and p.latcodigo_preparo > 0
                        and ci.cmdistatus <> 'X'"))
                {
                    if (id.ValueKind != JsonValueKind.Null && id.GetString().Length > 0)
                    {
                        cmdItens.CommandText += " and la.latchave = @id";
                        db.AddStringParameter(cmdItens, "@id").Value = id.GetString();
                    }

                    db.AddIntParameter(cmdItens, "@cmdicodigo");

                    foreach (var item in olist)
                    {
                        foreach (var itemComanda in item.olistItens)
                        {
                            cmdItens.Parameters["@cmdicodigo"].Value = itemComanda.cmdicodigo;
                            itemComanda.olistComposicao = cmdItens.FillList<ComandaItemComposicao>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotFound("Nao autorizado");
            }
            finally
            {
                if (cmdList != null)
                {
                    cmdList.Dispose();
                }
            }

            return Ok(olist);
        }
        /// <summary>
        /// Cancelar uma comanda.
        /// </summary>
        /// <remarks>
        /// Exemplo:
        ///
        ///     {        
        ///       "cmdcodigo": 123
        ///     }
        /// </remarks>
        /// /// <param name="json"></param> 
        /// <response code="200">Confirmação da Comanda Cancelada</response>
        /// <response code="404">Erro ao cancelar a Comanda.</response>
        [HttpDelete]
        [Route("CancelarComanda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult CancelarComanda([FromBody] JsonElement json)
        {
            var cmdcodigo = json.GetProperty("cmdcodigo");

            if (cmdcodigo.ValueKind == JsonValueKind.Null || cmdcodigo.ValueKind != JsonValueKind.Number)
            {
                return NotFound("Obrigatório informar o Id da Comanda.");
            }

            var db = Db.GetDbFood();
            var comandaItemRep = new ComandaItemRepositorio();

            try
            {
                var nusucodigo = new UsuarioRepositorio().GetUsuarioByToken(Request);

                var transa = db.StartTransaction();

                using (DbCommand cmd = db.NewCommand(@"select
                      c.cmdstatus,
                      la.latnome
                      from comanda c
                      inner join local_atendimento la 
                      on (c.latcodigo = la.latcodigo)
                      where c.cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = transa;
                    db.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo.GetInt32();

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader[0].ToString() != "A")
                            {
                                return NotFound(string.Concat("A comanda não está aberta!",
                                    Environment.NewLine,
                                    "Número: " + cmdcodigo.GetInt32().ToString("N0"),
                                    Environment.NewLine,
                                    reader["latnome"]));
                            }
                        }
                        else
                        {
                            throw new Exception(string.Concat("Comanda inexistente!",
                                    Environment.NewLine,
                                    "Número: " + cmdcodigo.GetInt32().ToString("N0")));
                        }
                    }
                }

                var comanda = new ComandaRepositorio().GetComanda(db, cmdcodigo.GetInt32());

                if (comanda == null || comanda.olistItens == null || !comanda.olistItens.Any())
                {
                    // atualiza status da comanda para excluído
                    using (DbCommand cmd = db.NewCommand("update comanda set cmdstatus = 'C', usucodigo=@usucodigo, ultatualizacao = now() where cmdcodigo = @cmdcodigo and cmdstatus <> 'F'"))
                    {
                        cmd.Transaction = transa;
                        db.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo.GetInt32();
                        db.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                        cmd.ExecuteNonQuery();
                    }

                    //disponibiliza local de atendimento
                    new LocalAtendimentoRepositorio().UpdateStatus(db, comanda.latcodigo, "D");

                    db.Commit();
                } else
                {
                    if (comanda.olistItens.Exists(p => p.cmdistatus == "E"))
                    {
                        throw new Exception("Comanda possui Itens Entregues!" +
                            Environment.NewLine +
                            "Cancelamento inválido.");
                    }

                    List<ComandaItem> olistUpd = new List<ComandaItem>();

                    olistUpd.AddRange(comanda.olistItens);

                    foreach (var item in olistUpd)
                    {
                        item.usucodigo = nusucodigo;
                        item.cmdistatus = "X";
                    }

                    // atualiza status dos itens da comanda para excluído
                    comandaItemRep.SetStatus(db, olistUpd);

                    // atualiza status da comanda para excluído
                    using (DbCommand cmd = db.NewCommand("update comanda set cmdstatus = 'C', usucodigo=@usucodigo, ultatualizacao = now() where cmdcodigo = @cmdcodigo and cmdstatus <> 'F'"))
                    {
                        cmd.Transaction = transa;
                        db.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo.GetInt32();
                        db.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                        cmd.ExecuteNonQuery();
                    }

                    //disponibiliza local de atendimento
                    //adicionar aqui uma verificaçao se ainda há comandas, se houver comandas na mesa ele não pode atualizar para D
                    // Verifica se ainda há comandas abertas na mesa
                    using (DbCommand cmd = db.NewCommand(@"select count(1) from comanda 
                                                    where latcodigo = @latcodigo and cmdstatus = 'A'"))
                    {
                        db.AddIntParameter(cmd, "@latcodigo").Value = comanda.latcodigo;
                        var count = (int)cmd.ExecuteScalar();

                        if (count == 0)
                        {
                            // Se não houver comandas abertas, disponibiliza o local de atendimento
                            new LocalAtendimentoRepositorio().UpdateStatus(db, comanda.latcodigo, "D");
                        }
                    }

                    db.Commit();
                }

               
            }
            catch (Exception ex)
            {
                db.RollBack();
                return NotFound(ex.Message);
            }

            return Ok("Comanda cancelada com sucesso!");
        }
        [HttpGet]
        [Route("GetComandasPorStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Comanda>> GetComandasPorStatus()
        {
            TLogDatabase odb = Db.GetDbFood();
            try
            {
                return Ok(new ComandaRepositorio().GetComandasPorStatus());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
