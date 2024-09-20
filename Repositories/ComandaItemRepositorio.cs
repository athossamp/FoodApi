using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace log_food_api.Repositories
{
    public class ComandaItemRepositorio
    {
        /*public ComandaItem SetStatus(TLogDatabase odb, ComandaItem oitem)
        {
            var db = Db.GetDbFood();

            try
            {
                var transa = db.StartTransaction();

                var comanda = new ComandaRepositorio().GetComanda(odb, oitem.cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                var logRep = new ComandaItemLogRepositorio();

                using (var cmd = db.NewCommand(@"update comanda_item set cmdistatus = @cmdistatus where cmdicodigo = @cmdicodigo"))
                {

                    var ofind = comanda.olistItens.Find(p => p.cmdicodigo ==  oitem.cmdcodigo);

                    if (ofind != null)
                    {
                        if (ofind.cmdistatus != "E" && ofind.cmdistatus != "A")
                        {
                            throw new Exception("Cancelamento do Item Não Permitido!");
                        }
                    }

                    cmd.Transaction = transa;
                    db.AddIntParameter(cmd, "@cmdicodigo").Value = oitem.cmdicodigo;
                    db.AddStringParameter(cmd, "@cmdistatus").Value = oitem.cmdistatus;
                    cmd.ExecuteNonQuery();

                    switch (oitem.cmdistatus)
                    {
                        case "A":
                            if (oitem.cmdipreparo == "S")
                            {
                                logRep.InserirLog(db, oitem.cmdicodigo, "PREPLIB");
                            }
                            break;

                        case "P":
                            logRep.InserirLog(db, oitem.cmdicodigo, "PRELANC");
                            break;

                        case "X":
                            logRep.InserirLog(db, oitem.cmdicodigo, "CANCLOJA");
                            break;

                        case "B":
                            logRep.InserirLog(db, oitem.cmdicodigo, "BLOQLOJA");
                            break;
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                throw ex;
            }

            return oitem;
        }*/

        public List<ComandaItem> SetStatus(TLogDatabase db, List<ComandaItem> oitens)
        {
            var comandaRep = new ComandaRepositorio();

            try
            {
                var comanda = comandaRep.GetComanda(db, oitens[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                var logRep = new ComandaItemLogRepositorio();
                var usuRep = new UsuarioRepositorio();

                using (var cmd = db.NewCommand(@"update comanda_item set cmdistatus = @cmdistatus, usucodigo=@usucodigo, ultatualizacao=now() where cmdicodigo = @cmdicodigo"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddIntParameter(cmd, "@cmdicodigo");
                    db.AddInt16Parameter(cmd, "@usucodigo");
                    db.AddStringParameter(cmd, "@cmdistatus");

                    //db.AddIntParameter(cmd, "@cmdicodigo").Value = oitem.cmdicodigo;
                    //db.AddStringParameter(cmd, "@cmdistatus").Value = oitem.cmdistatus;

                    foreach (var item in oitens)
                    {
                        var ofind = comanda.olistItens.Find(p => p.cmdicodigo == item.cmdicodigo);

                        if (ofind != null)
                        {
                            if (ofind.cmdistatus.IndexOfAny(new char[] { 'P', 'F' }) > -1)
                            {
                                logRep.InserirLog(item.cmdicodigo, "CANCLJNEG");

                                if (ofind.cmdistatus == "P")
                                {
                                    throw new Exception("Item Já Foi Pago. Cancelamento Não Autorizado!");
                                }
                                if (ofind.cmdistatus == "F")
                                {
                                    throw new Exception("Item Já Foi Faturado. Cancelamento Não Autorizado!");
                                }
                            }

                            if (ofind.cmdistatus.IndexOfAny(new char[] { 'W', 'E', 'Z' }) > -1)
                            {
                                if (usuRep.GetTipo(item.usucodigo, db) != "G")
                                {
                                    logRep.InserirLog(item.cmdicodigo, "CANCLJNEG");
                                    throw new Exception("Cancelamento do Item Não Autorizado!");
                                }
                            }
                        }

                        cmd.Parameters["@cmdicodigo"].Value = item.cmdicodigo;
                        cmd.Parameters["@usucodigo"].Value = item.usucodigo;
                        cmd.Parameters["@cmdistatus"].Value = item.cmdistatus;
                        cmd.ExecuteNonQuery();

                        switch (item.cmdistatus)
                        {
                            case "A":
                                if (item.cmdipreparo == "S")
                                {
                                    logRep.InserirLog(db, item.cmdicodigo, "PREPLIB");
                                }
                                break;

                            case "P":
                                logRep.InserirLog(db, item.cmdicodigo, "PREPINI");
                                break;

                            case "E":
                                logRep.InserirLog(db, item.cmdicodigo, "ENTFIM");
                                break;

                            case "F":
                                logRep.InserirLog(db, item.cmdicodigo, "PREPFIN");
                                break;

                            case "X":
                                logRep.InserirLog(db, item.cmdicodigo, "CANCLOJA");
                                break;

                            case "B":
                                logRep.InserirLog(db, item.cmdicodigo, "BLOQLOJA");
                                break;
                        }
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                LogUtil.InserirEvento("", "Log_Food_Api: SetStatus " + ex.Message);
                throw ex;
            }

            return comandaRep.GetComanda(db, oitens[0].cmdcodigo).olistItens;
        }

        public List<ComandaItem> PedidoVendaSetStatus(TLogDatabase db, List<ComandaItem> oitens)
        {
            var comandaRep = new ComandaRepositorio();

            try
            {
                var comanda = comandaRep.GetComandaPedidoVenda(db, oitens[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                var logRep = new ComandaItemLogRepositorio();
                var usuRep = new UsuarioRepositorio();

                using (var cmd = db.NewCommand(@"update comanda_item set cmdistatus = @cmdistatus, usucodigo=@usucodigo, ultatualizacao=now() where cmdicodigo = @cmdicodigo"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddIntParameter(cmd, "@cmdicodigo");
                    db.AddInt16Parameter(cmd, "@usucodigo");
                    db.AddStringParameter(cmd, "@cmdistatus");

                    //db.AddIntParameter(cmd, "@cmdicodigo").Value = oitem.cmdicodigo;
                    //db.AddStringParameter(cmd, "@cmdistatus").Value = oitem.cmdistatus;

                    foreach (var item in oitens)
                    {
                        var ofind = comanda.olistItens.Find(p => p.cmdicodigo == item.cmdicodigo);

                        if (ofind != null)
                        {
                            if (ofind.cmdistatus.IndexOfAny(new char[] { 'P', 'F' }) > -1)
                            {
                                logRep.InserirLog(item.cmdicodigo, "CANCLJNEG");

                                if (ofind.cmdistatus == "P")
                                {
                                    throw new Exception("Item Já Foi Pago. Cancelamento Não Autorizado!");
                                }
                                if (ofind.cmdistatus == "F")
                                {
                                    throw new Exception("Item Já Foi Faturado. Cancelamento Não Autorizado!");
                                }
                            }

                            if (ofind.cmdistatus.IndexOfAny(new char[] { 'W', 'E', 'Z' }) > -1)
                            {
                                if (usuRep.GetTipo(item.usucodigo, db) != "G")
                                {
                                    logRep.InserirLog(item.cmdicodigo, "CANCLJNEG");
                                    throw new Exception("Cancelamento do Item Não Autorizado!");
                                }
                            }
                        }

                        cmd.Parameters["@cmdicodigo"].Value = item.cmdicodigo;
                        cmd.Parameters["@usucodigo"].Value = item.usucodigo;
                        cmd.Parameters["@cmdistatus"].Value = item.cmdistatus;
                        cmd.ExecuteNonQuery();

                        switch (item.cmdistatus)
                        {
                            case "A":
                                if (item.cmdipreparo == "S")
                                {
                                    logRep.InserirLog(db, item.cmdicodigo, "PREPLIB");
                                }
                                break;

                            case "P":
                                logRep.InserirLog(db, item.cmdicodigo, "PREPINI");
                                break;

                            case "E":
                                logRep.InserirLog(db, item.cmdicodigo, "ENTFIM");
                                break;

                            case "F":
                                logRep.InserirLog(db, item.cmdicodigo, "PREPFIN");
                                break;

                            case "X":
                                logRep.InserirLog(db, item.cmdicodigo, "CANCLOJA");
                                break;

                            case "B":
                                logRep.InserirLog(db, item.cmdicodigo, "BLOQLOJA");
                                break;
                        }
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                LogUtil.InserirEvento("", "Log_Food_Api: SetStatus " + ex.Message);
                throw ex;
            }

            return comandaRep.GetComandaPedidoVenda(db, oitens[0].cmdcodigo).olistItens;
        }

        public List<ComandaItem> SetObservacao(TLogDatabase db, List<ComandaItem> oitens)
        {
            var comandaRep = new ComandaRepositorio();

            try
            {
                var comanda = comandaRep.GetComanda(db, oitens[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                var logRep = new ComandaItemLogRepositorio();
                var usuRep = new UsuarioRepositorio();

                using (var cmd = db.NewCommand(@"update comanda_item set cmdiobservacao = @cmdiobservacao, usucodigo=@usucodigo, ultatualizacao=now() where cmdicodigo = @cmdicodigo"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddIntParameter(cmd, "@cmdicodigo");
                    db.AddInt16Parameter(cmd, "@usucodigo");
                    db.AddStringParameter(cmd, "@cmdiobservacao");

                    foreach (var item in oitens)
                    {
                        var ofind = comanda.olistItens.Find(p => p.cmdicodigo == item.cmdicodigo);

                        if (ofind != null)
                        {
                            if (ofind.cmdistatus.IndexOfAny(new char[] { 'P', 'F' }) > -1)
                            {
                                logRep.InserirLog(item.cmdicodigo, "CANCLJNEG");

                                if (ofind.cmdistatus == "P")
                                {
                                    throw new Exception("Item Já Foi Pago. Mudança Não Autorizada!");
                                }
                                if (ofind.cmdistatus == "F")
                                {
                                    throw new Exception("Item Já Foi Faturado. Mudança Não Autorizada!");
                                }
                            }

                            //if (ofind.cmdistatus.IndexOfAny(['W', 'E', 'Z']) > -1)
                            if (ofind.cmdistatus.IndexOfAny(new char[] { 'W', 'E', 'Z' }) > -1)

                            {
                                if (usuRep.GetTipo(item.usucodigo, db) != "G")
                                {
                                    logRep.InserirLog(item.cmdicodigo, "CANCLJNEG");
                                    throw new Exception("Mudança do Item Não Autorizada!");
                                }
                            }
                        }

                        cmd.Parameters["@cmdicodigo"].Value = item.cmdicodigo;
                        cmd.Parameters["@usucodigo"].Value = item.usucodigo;
                        cmd.Parameters["@cmdiobservacao"].Value = item.cmdiobservacao;
                        cmd.ExecuteNonQuery();

                        /*
                        switch (item.cmdistatus)
                        {
                            case "A":
                                if (item.cmdipreparo == "S")
                                {
                                    logRep.InserirLog(db, item.cmdicodigo, "PREPLIB");
                                }
                                break;

                            case "P":
                                logRep.InserirLog(db, item.cmdicodigo, "PREPINI");
                                break;

                            case "E":
                                logRep.InserirLog(db, item.cmdicodigo, "ENTFIM");
                                break;

                            case "F":
                                logRep.InserirLog(db, item.cmdicodigo, "PREPFIN");
                                break;

                            case "X":
                                logRep.InserirLog(db, item.cmdicodigo, "CANCLOJA");
                                break;

                            case "B":
                                logRep.InserirLog(db, item.cmdicodigo, "BLOQLOJA");
                                break;
                        }*/
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                LogUtil.InserirEvento("", "Log_Food_Api: SetStatus " + ex.Message);
                throw ex;
            }

            return comandaRep.GetComanda(db, oitens[0].cmdcodigo).olistItens;
        }

        public List<ComandaItem> InserirItensPedidoVenda(TLogDatabase db, List<ComandaItem> olist)
        {
            if (olist == null || olist.Count == 0)
            {
                return new List<ComandaItem>();
            }
            var comandaRep = new ComandaRepositorio();

            try
            {
                var comanda = comandaRep.GetComanda(db, olist[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                StringBuilder stb = new StringBuilder();

                foreach (var item in olist)
                {
                    if (item.cmdivalor < 0.0001m)
                    {
                        stb.AppendLine("Item Cód: " + item.procodigo + " Sem Valor Definido!");
                    }
                }

                if (stb.Length > 0)
                {
                    throw new Exception(stb.ToString());
                }

                using (var cmd = db.NewCommand("CALL atualizar_produto(@p_cmdicodigo, @p_cmdcodigo, @p_usucodigo, @p_procodigo, @p_precodigo, @p_cmdivalor, @p_cmdivalor_pago, @p_cmdiobservacao, @p_cmdistatus, @p_cmdinumero_origem, @p_cmdipreparo, @p_cmdiquantidade, @p_prequantidade, @p_procodbarra, @p_embcodigo, @p_edit, @p_probalanca, @p_provende_fracionado);"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddIntParameter(cmd, "@p_cmdicodigo");
                    db.AddIntParameter(cmd, "@p_cmdcodigo");
                    db.AddIntParameter(cmd, "@p_usucodigo");
                    db.AddIntParameter(cmd, "@p_procodigo");
                    db.AddIntParameter(cmd, "@p_precodigo");
                    db.AddFloatParameter(cmd, "@p_cmdivalor");
                    db.AddFloatParameter(cmd, "@p_cmdivalor_pago");
                    db.AddStringParameter(cmd, "@p_cmdiobservacao");
                    db.AddStringParameter(cmd, "@p_cmdistatus");
                    db.AddIntParameter(cmd, "@p_cmdinumero_origem");
                    db.AddStringParameter(cmd, "@p_cmdipreparo");
                    db.AddFloatParameter(cmd, "@p_cmdiquantidade");
                    db.AddFloatParameter(cmd, "@p_prequantidade");
                    db.AddStringParameter(cmd, "@p_procodbarra");
                    db.AddStringParameter(cmd, "@p_embcodigo");
                    db.AddBooleanParameter(cmd, "@p_edit");
                    db.AddStringParameter(cmd, "@p_provende_fracionado");
                    db.AddStringParameter(cmd, "@p_probalanca");

                    var logRep = new ComandaItemLogRepositorio();
                    var itemComposicaoRep = new ComandaItemComposicaoRepositorio(db);
                    //var prodRep = new ProdutoRepositorio();

                    // valicadao da composicao
                    /*foreach (var item in olist)
                    {
                        if (item.olistComposicao == null && item.olistComposicao.Count == 0)
                        {
                            var prod = prodRep.GetByCodigo(db, item.procodigo);

                            if (prod.olistComposicao != null && prod.olistComposicao.Count > 0)
                            {
                                item.olistComposicao = new List<ComandaItemComposicao>();

                                foreach (var itemProdComp in prod.olistComposicao)
                                {
                                    ComandaItemComposicao itemcomp = new ComandaItemComposicao();
                                    itemcomp.procodigo = itemcomp.procodigo;
                                    itemcomp.cmdicodigo = item.cmdicodigo;
                                    itemcomp.procodigo_composicao = itemProdComp.procodigo_default;

                                    item.olistComposicao.Add(itemcomp);
                                }
                            }
                        }
                    }*/

                    foreach (var item in olist)
                    {
                        item.cmdcodigo = comanda.cmdcodigo;
                        item.cmdicodigo = db.GetGenerator("cmdicodigo");

                        cmd.Parameters["@p_cmdicodigo"].Value = item.cmdicodigo;
                        cmd.Parameters["@p_cmdcodigo"].Value = item.cmdcodigo;
                        cmd.Parameters["@p_usucodigo"].Value = item.usucodigo_atendimento;
                        cmd.Parameters["@p_procodigo"].Value = item.procodigo;
                        cmd.Parameters["@p_precodigo"].Value = item.precodigo;
                        cmd.Parameters["@p_cmdivalor"].Value = item.cmdivalor;
                        cmd.Parameters["@p_cmdivalor_pago"].Value = item.cmdivalor;
                        cmd.Parameters["@p_cmdiobservacao"].Value = item.cmdiobservacao;
                        cmd.Parameters["@p_cmdistatus"].Value = item.cmdistatus;
                        cmd.Parameters["@p_cmdinumero_origem"].Value = item.cmdinumero_origem;
                        cmd.Parameters["@p_cmdipreparo"].Value = item.cmdipreparo;
                        cmd.Parameters["@p_cmdiquantidade"].Value = item.cmdiquantidade;
                        cmd.Parameters["@p_prequantidade"].Value = item.prequantidade;
                        cmd.Parameters["@p_procodbarra"].Value = item.procodbarra;
                        cmd.Parameters["@p_embcodigo"].Value = item.embcodigo;
                        cmd.Parameters["@p_edit"].Value = item.edit;
                        cmd.Parameters["@p_provende_fracionado"].Value = item.provende_fracionado;
                        cmd.Parameters["@p_probalanca"].Value = item.probalanca;
                        cmd.ExecuteNonQuery();

                        if (item.olistComposicao != null && item.olistComposicao.Count > 0)
                        {
                            foreach (var itemComp in item.olistComposicao)
                            {
                                itemComp.cmdicodigo = item.cmdicodigo;
                                itemComp.usucodigo = item.usucodigo;
                            }

                            itemComposicaoRep.InserirItens(item.olistComposicao);
                        }

                        switch (item.cmdistatus)
                        {
                            case "A":
                                if (item.cmdipreparo == "S")
                                {
                                    logRep.InserirLog(db, item.cmdicodigo, "PREPLIB");
                                }
                                break;

                            case "B":
                                logRep.InserirLog(db, item.cmdicodigo, "PRELANC");
                                break;

                            case "E":
                                logRep.InserirLog(db, item.cmdicodigo, "ENTFIM");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return comandaRep.GetComanda(db, olist[0].cmdcodigo).olistItens;
        }
        public List<ComandaItem> InserirItens(TLogDatabase db, List<ComandaItem> olist)
        {
            if (olist == null || olist.Count == 0)
            {
                return new List<ComandaItem>();
            }
            var comandaRep = new ComandaRepositorio();

            try
            {
                var comanda = comandaRep.GetComanda(db, olist[0].cmdcodigo);

                if (comanda.cmdstatus != "A")
                {
                    throw new Exception("Comanda Não Está Aberta!");
                }

                StringBuilder stb = new StringBuilder();

                foreach (var item in olist)
                {
                    if (item.cmdivalor < 0.0001m)
                    {
                        stb.AppendLine("Item Cód: " + item.procodigo + " Sem Valor Definido!");
                    }
                }

                if (stb.Length > 0)
                {
                    throw new Exception(stb.ToString());
                }

                using (var cmd = db.NewCommand("INSERT INTO comanda_item (cmdicodigo, cmdcodigo, usucodigo_atendimento, procodigo, precodigo, cmdivalor, cmdiobservacao, cmdistatus, cmdinumero_origem, cmdipreparo, usucodigo, ultatualizacao, cmdiquantidade, prequantidade, procodbarra, embcodigo) VALUES(@cmdicodigo, @cmdcodigo, @usucodigo_atendimento, @procodigo, @precodigo, @cmdivalor, @cmdiobservacao, @cmdistatus, @cmdinumero_origem, (select propreparo from produto where procodigo = @procodigo), @usucodigo, now(), @cmdiquantidade, @prequantidade, @procodbarra, @embcodigo);"))
                {
                    cmd.Transaction = db.oTxn;
                    db.AddIntParameter(cmd, "@cmdicodigo");
                    db.AddIntParameter(cmd, "@cmdcodigo");
                    db.AddInt16Parameter(cmd, "@usucodigo_atendimento");
                    db.AddIntParameter(cmd, "@procodigo");
                    db.AddIntParameter(cmd, "@precodigo");
                    db.AddFloatParameter(cmd, "@cmdivalor");
                    db.AddStringParameter(cmd, "@cmdistatus");
                    db.AddStringParameter(cmd, "@cmdiobservacao");
                    db.AddIntParameter(cmd, "@cmdinumero_origem");
                    db.AddFloatParameter(cmd, "@cmdiquantidade");
                    db.AddFloatParameter(cmd, "@prequantidade");
                    db.AddStringParameter(cmd, "@procodbarra");
                    db.AddIntParameter(cmd, "@usucodigo");
                    db.AddStringParameter(cmd, "@embcodigo");

                    var logRep = new ComandaItemLogRepositorio();
                    var itemComposicaoRep = new ComandaItemComposicaoRepositorio(db);
                    //var prodRep = new ProdutoRepositorio();

                    // valicadao da composicao
                    /*foreach (var item in olist)
                    {
                        if (item.olistComposicao == null && item.olistComposicao.Count == 0)
                        {
                            var prod = prodRep.GetByCodigo(db, item.procodigo);

                            if (prod.olistComposicao != null && prod.olistComposicao.Count > 0)
                            {
                                item.olistComposicao = new List<ComandaItemComposicao>();

                                foreach (var itemProdComp in prod.olistComposicao)
                                {
                                    ComandaItemComposicao itemcomp = new ComandaItemComposicao();
                                    itemcomp.procodigo = itemcomp.procodigo;
                                    itemcomp.cmdicodigo = item.cmdicodigo;
                                    itemcomp.procodigo_composicao = itemProdComp.procodigo_default;

                                    item.olistComposicao.Add(itemcomp);
                                }
                            }
                        }
                    }*/

                    foreach (var item in olist)
                    {
                        item.cmdcodigo = comanda.cmdcodigo;
                        item.cmdicodigo = db.GetGenerator("cmdicodigo");

                        cmd.Parameters["@cmdicodigo"].Value = item.cmdicodigo;
                        cmd.Parameters["@cmdcodigo"].Value = item.cmdcodigo;
                        cmd.Parameters["@usucodigo_atendimento"].Value = item.usucodigo_atendimento;
                        cmd.Parameters["@procodigo"].Value = item.procodigo;
                        cmd.Parameters["@precodigo"].Value = item.precodigo;
                        cmd.Parameters["@cmdivalor"].Value = item.cmdivalor;
                        cmd.Parameters["@cmdistatus"].Value = item.cmdistatus;
                        cmd.Parameters["@cmdiobservacao"].Value = item.cmdiobservacao;
                        cmd.Parameters["@cmdinumero_origem"].Value = item.cmdinumero_origem;
                        //cmd.Parameters["@cmdipreparo"].Value = item.cmdipreparo;
                        cmd.Parameters["@usucodigo"].Value = item.usucodigo;
                        cmd.Parameters["@cmdiquantidade"].Value = item.cmdiquantidade;
                        cmd.Parameters["@prequantidade"].Value = item.prequantidade;
                        cmd.Parameters["@procodbarra"].Value = item.procodbarra;
                        cmd.Parameters["@embcodigo"].Value = item.embcodigo;
                        cmd.ExecuteNonQuery();

                        if (item.olistComposicao != null && item.olistComposicao.Count > 0)
                        {
                            foreach (var itemComp in item.olistComposicao)
                            {
                                itemComp.cmdicodigo = item.cmdicodigo;
                                itemComp.usucodigo = item.usucodigo;
                            }

                            itemComposicaoRep.InserirItens(item.olistComposicao);
                        }

                        switch (item.cmdistatus)
                        {
                            case "A":
                                if (item.cmdipreparo == "S")
                                {
                                    logRep.InserirLog(db, item.cmdicodigo, "PREPLIB");
                                }
                                break;

                            case "B":
                                logRep.InserirLog(db, item.cmdicodigo, "PRELANC");
                                break;

                            case "E":
                                logRep.InserirLog(db, item.cmdicodigo, "ENTFIM");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return comandaRep.GetComanda(db, olist[0].cmdcodigo).olistItens;
        }
    }
       
}
