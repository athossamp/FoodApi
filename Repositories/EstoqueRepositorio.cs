using log_food_api.Models;
using Logicom.Infraestrutura;
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;

namespace log_food_api.Repositories
{
    public class EstoqueRepositorio
    {
        public List<Estoque> GetEstoque()
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                odb.Open();
                odb.StartTransaction();
                List<Estoque> oitem = new List<Estoque>();
                using (var cmd = odb.NewCommand(@" SELECT e.LOCCODIGO, e.PROCODIGO, p.PRODESCRICAO, e.ESTATUAL
                 FROM estoque e
                 inner join local l on l.loccodigo = e.loccodigo
                 inner join produto p on p.procodigo = e.procodigo
                 WHERE l.LOCVAREJOATACADO <> 'T' AND e.ESTATUAL > 0
                 ORDER BY p.PRODESCRICAO"))
                {
                    cmd.Transaction = odb.oTxn;
                    oitem = cmd.FillList<Estoque>();
                }

                return oitem;
            }
            catch (Exception ex)
            {
                LogUtil.InserirEvento("", "Get Estoque: " + Environment.NewLine + ex.Message);
                throw ex;
               
            }
        }
        public List<Estoque> GetEstoqueByCodigo(int procodigo)
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                odb.Open();
                odb.StartTransaction();
                List<Estoque> oitem = new List<Estoque>();
                using (var cmd = odb.NewCommand(@" SELECT e.LOCCODIGO, e.PROCODIGO, p.PRODESCRICAO
                 FROM estoque e
                 inner join local l on l.loccodigo = e.loccodigo
                 inner join produto p on p.procodigo = e.procodigo
                 WHERE l.LOCVAREJOATACADO <> 'T' and PROCODIGO = @procodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@procodigo").Value = procodigo;
                    oitem = cmd.FillList<Estoque>();
                }
                
                return oitem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ComandaItem> InserirItemEstoque(TLogDatabase db, List<ComandaItem> olist)
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

                using (var cmd = db.NewCommand("INSERT INTO comanda_item (cmdicodigo, cmdcodigo, usucodigo_atendimento, procodigo, precodigo, cmdivalor, cmdiobservacao, cmdistatus, cmdinumero_origem, cmdipreparo, usucodigo, ultatualizacao, cmdiquantidade) VALUES(@cmdicodigo, @cmdcodigo, @usucodigo_atendimento, @procodigo, @precodigo, @cmdivalor, @cmdiobservacao, @cmdistatus, @cmdinumero_origem, (select propreparo from produto where procodigo = @procodigo), @usucodigo, now(), @cmdiquantidade);"))
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
                    //db.AddStringParameter(cmd, "@cmdipreparo");
                    db.AddIntParameter(cmd, "@usucodigo");
                    db.AddFloatParameter(cmd, "@cmdiquantidade");

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
        public int retorno;
        public List<ProdutoP> GetListaProduto(string cProdescricao)
        {
            TLogDatabase dbDados = Db.GetDbSCEF();
            DbDataReader read = null;

            List<ProdutoP> lProduto = new List<ProdutoP>();
            String Sql = "";

            string[] parts = cProdescricao.Split('%');
            string descricao = parts.Length > 0 ? parts[0].Trim() : "";
            string descricao2 = parts.Length > 1 ? parts[1].Trim() : "";

            string cCODBARRA = cProdescricao;
            string valorTipo = cProdescricao;
            string cTEMP = LogUtil.RetNumero(cProdescricao);
            int nPROCODIGO = 0;
            if (cTEMP.Length < 7)
            {
                nPROCODIGO = LogUtil.AsInteger(cTEMP);
            }

            if (cCODBARRA.Length > 20)
            {
                cCODBARRA = cCODBARRA.Substring(0, 20);
            }

            try
            {
                Sql = @"SELECT P.PROCODIGO,
                     P.PRODESCRICAO,
                     P.propreco_venda,
                     P.provende_fracionado,
                     P.probalanca,
                     PB.embcodigo,
                     pb.precodigo,
                     pb.prequantidade,
                     PC.codbarra,
                     (select sum(estatual) from estoque e where e.procodigo = p.procodigo) AS NESTOQUE_ATUAL 
                FROM PRODUTO P
                INNER JOIN PROEMBALA PB ON P.PROCODIGO = PB.PROCODIGO
                LEFT JOIN PROCODBARRA PC ON PB.PRECODIGO = PC.PRECODIGO
                WHERE P.STATUS = 'A' 
                  AND PB.PREEMBALAGEMVENDA = 'S' 
                  AND PB.PRESTATUS = 'A'
                  AND P.PROVENDA_INTERNO = 'V'  
                  AND (P.PRODESCRICAO CONTAINING @PRODESCRICAO 
                       AND P.PRODESCRICAO CONTAINING @PRODESCRICAO2 
                       OR @PRODESCRICAO = CAST('*' AS CHAR(50))
                       OR PC.CODBARRA STARTING(@CODBARRA) 
                       OR @CODBARRA = CAST('*' AS CHAR(20))
                       OR P.PROCODIGO = @PROCODIGO) 
                GROUP BY P.PROCODIGO, P.PRODESCRICAO, pb.precodigo, PB.EMBCODIGO, P.PROPRECO_VENDA, pb.prequantidade, PC.CODBARRA, P.probalanca, P.provende_fracionado
                ORDER BY P.PRODESCRICAO";

                dbDados.Open();

                if (dbDados == null)
                {
                    return lProduto; // Caso o banco de dados não abra, retorna a lista vazia
                }

                DbCommand cmd = dbDados.NewCommand(Sql);

                // Preenche os parâmetros da query com os valores separados
                dbDados.AddStringParameter(cmd, "@PRODESCRICAO").Value = string.IsNullOrEmpty(descricao) ? DBNull.Value : descricao;
                dbDados.AddStringParameter(cmd, "@PRODESCRICAO2").Value = string.IsNullOrEmpty(descricao2) ? "" : descricao2;
                dbDados.AddStringParameter(cmd, "@CODBARRA").Value = string.IsNullOrEmpty(cCODBARRA) ? DBNull.Value : cCODBARRA;
                dbDados.AddIntParameter(cmd, "@PROCODIGO").Value = nPROCODIGO == 0 ? DBNull.Value : nPROCODIGO;

                read = dbDados.NewDataReader(cmd);

                while (read.Read())
                {
                    // Lógica de leitura e criação de objetos ProdutoP
                    int nprocodigo = LogUtil.GenericConvert<int>(read["PROCODIGO"]);

                    ProdutoP prod = lProduto.FirstOrDefault(p => p.NPROCODIGO == nprocodigo);
                    if (prod == null)
                    {
                        prod = new ProdutoP
                        {
                            NESTOQUE_ATUAL = LogUtil.GenericConvert<decimal>(read["NESTOQUE_ATUAL"]),
                            NPRECOVENDA = LogUtil.GenericConvert<double>(read["propreco_venda"]),
                            PROBALANCA = read["probalanca"].ToString().Trim(),
                            PROVENDE_FRACIONADO = read["provende_fracionado"].ToString().Trim(),
                            NPROCODIGO = nprocodigo,
                            SPRODESCRICAO = read["PRODESCRICAO"].ToString().Trim(),
                            proembala = new List<PROEMBALA>()
                        };

                        lProduto.Add(prod);
                    }

                    PROEMBALA proembala = new PROEMBALA
                    {
                        procodigo = nprocodigo,
                        precodigo = LogUtil.GenericConvert<int>(read["PRECODIGO"]),
                        prequantidade = LogUtil.GenericConvert<decimal>(read["PREQUANTIDADE"]),
                        embcodigo = read["EMBCODIGO"].ToString().Trim(),
                        codbar = new List<CODBAR>()
                    };

                    CODBAR codbar = new CODBAR
                    {
                        procodbarra = read["codbarra"].ToString().Trim(),
                        precodigo = LogUtil.GenericConvert<int>(read["PRECODIGO"])
                    };

                    proembala.codbar.Add(codbar);
                    prod.proembala.Add(proembala);

                    string prodCodBar = "";
                    foreach (var produto in lProduto)
                    {
                        decimal prequantidade = produto.proembala[0].prequantidade;

                        foreach (var menorProd in produto.proembala)
                        {
                            if (prequantidade > menorProd.prequantidade)
                            {
                                prequantidade = menorProd.prequantidade;
                            }
                        }

                        var menorEmb = produto.proembala.Find(p => p.prequantidade == prequantidade);
                        prodCodBar = menorEmb.codbar[0].procodbarra;
                        produto.CODBARRAPRODUTO = prodCodBar;
                    }
                }

                dbDados.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                read?.Close();
                read?.Dispose();
                dbDados.oConn.Dispose();
            }

            return lProduto;
        }

        public List<ProdutoEstoqueComanda> GetItensComandas(int cmdcodigo)
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                TLogDatabase db = Db.GetDbFood();
                db.Open();
                odb.Open();
                db.StartTransaction();
                odb.StartTransaction();
                List<ComandaItem> oprocodigos = new List<ComandaItem>();
                List<ProdutoEstoqueComanda> oitem = new List<ProdutoEstoqueComanda>();
                using (var cmdProcodigo = db.NewCommand(@"SELECT ci.cmdicodigo, ci.procodigo, ci.cmdistatus, ci.cmdiquantidade, ci.cmdiobservacao, ci.cmdivalor, ci.prequantidade, ci.embcodigo, ci.procodbarra, ci.precodigo, ci.provende_fracionado, ci.probalanca FROM comanda_item ci where ci.cmdcodigo = @cmdcodigo AND ci.cmdistatus <> 'X'"))
                {
                    cmdProcodigo.Transaction = db.oTxn;
                    db.AddIntParameter(cmdProcodigo, @"cmdcodigo").Value = cmdcodigo;
                    oprocodigos = cmdProcodigo.FillList<ComandaItem>();

                    foreach (ComandaItem procodigo in oprocodigos)
                    {
                        using (var cmd = odb.NewCommand(@"SELECT e.LOCCODIGO, e.PROCODIGO, p.PRODESCRICAO, e.ESTATUAL, p.PROPRECO_VENDA, p.probalanca, p.provende_fracionado
                         FROM estoque e
                         inner join local l on l.loccodigo = e.loccodigo
                         inner join produto p on p.procodigo = e.procodigo
                         WHERE l.LOCVAREJOATACADO <> 'T' AND e.ESTATUAL <> 0
                         AND p.PROCODIGO = @PROCODIGO
                         ORDER BY p.PRODESCRICAO"))
                        {
                            cmd.Transaction = odb.oTxn;
                            odb.AddIntParameter(cmd, @"PROCODIGO").Value = procodigo.procodigo;
                            var produtos = cmd.FillList<ProdutoEstoqueComanda>();

                            foreach (var produto in produtos)
                            {
                                produto.cmdicodigo = procodigo.cmdicodigo;
                                produto.cmdiquantidade = procodigo.cmdiquantidade;
                                produto.cmdistatus = procodigo.cmdistatus;
                                produto.cmdiobservacao = procodigo.cmdiobservacao;
                                produto.cmdivalor = procodigo.cmdivalor;
                                produto.prequantidade = procodigo.prequantidade;
                                produto.embcodigo = procodigo.embcodigo;
                                produto.procodbarra = procodigo.procodbarra;
                                produto.precodigo = procodigo.precodigo;
                                produto.PROBALANCA = procodigo.probalanca;
                                produto.PROVENDE_FRACIONADO = procodigo.provende_fracionado;
                                oitem.Add(produto);
                            }
                        }

                    }
                }
                return oitem;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
   
}
