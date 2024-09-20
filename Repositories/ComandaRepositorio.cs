using log_food_api.Models;
using Logicom.Infraestrutura;
using LogicomInfraestrutura;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace log_food_api.Repositories
{
    public class ComandaRepositorio
    {

        public Int16 CBNTIPOAMBIENTE = 2; //PRODUÇÃO: 1, HOMOLOGAÇÃO: 2
        public Comanda GetComanda(TLogDatabase odb, int cmdcodigo)
        {
            Comanda oitem = new Comanda();

            try
            {
                using (var cmd = odb.NewCommand(@"select * from comanda where cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo;
                    oitem = cmd.FillList<Comanda>().FirstOrDefault();
                }
                using (var cmdItens = odb.NewCommand(@"select 
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
                    odb.AddIntParameter(cmdItens, "@cmdcodigo").Value = oitem.cmdcodigo;
                    cmdItens.Transaction = odb.oTxn;

                    oitem.olistItens = cmdItens.FillList<ComandaItem>();
                }

                using (var cmdItens = odb.NewCommand(@"
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
                    odb.AddIntParameter(cmdItens, "@cmdicodigo");
                    cmdItens.Transaction = odb.oTxn;

                    foreach (var itemComanda in oitem.olistItens)
                    {
                        cmdItens.Parameters[0].Value = itemComanda.cmdicodigo;
                        itemComanda.olistComposicao = cmdItens.FillList<ComandaItemComposicao>();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oitem;
        }
        public Comanda GetComandaPedidoVenda(TLogDatabase odb, int cmdcodigo)
        {
            Comanda oitem = new Comanda();

            try
            {
                using (var cmd = odb.NewCommand(@"select * from comanda where cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo;
                    oitem = cmd.FillList<Comanda>().FirstOrDefault();
                }
                using (var cmdItens = odb.NewCommand(@"select 
                         ci.*
                        from comanda_item ci
                        where ci.cmdcodigo = @cmdcodigo 
                        and ci.cmdistatus <> 'X'"))
                {
                    odb.AddIntParameter(cmdItens, "@cmdcodigo").Value = oitem.cmdcodigo;
                    cmdItens.Transaction = odb.oTxn;

                    oitem.olistItens = cmdItens.FillList<ComandaItem>();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oitem;
        }
        public List<Comanda> GetComandasPorStatus()
        {
            List<Comanda> oitem = new List<Comanda>();
            TLogDatabase odb = Db.GetDbFood();
            try
            {
                using (var cmd = odb.NewCommand(@"select * from comanda where cmdstatus = 'C' OR cmdstatus = 'F'"))
                {
                    cmd.Transaction = odb.oTxn;
                    //odb.AddStringParameter(cmd, "@cmdstatus").Value = cmdstatus;
                    oitem = cmd.FillList<Comanda>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //Comanda oitem2 = new Comanda();


            //    oitem2.latcodigo = 2;
            //    oitem2.cmdapartamento = 0;
            //    oitem2.usucodigo_abertura = 2;
            //    oitem2.cmdobservacao = "";
            //    oitem2.cmdvalor_total = 0;
            //    oitem2.cmdvalor_taxa_servico = 0;
            //oitem.Add(oitem2);
            return oitem;
            
        }
        public List<Comanda> GetComandaPorMesa(TLogDatabase odb, int latcodigo)
        {
            List<Comanda> oitem = new List<Comanda>();

            try
            {
                using (var cmd = odb.NewCommand(@"select * from comanda where latcodigo = @latcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@latcodigo").Value = latcodigo;
                    oitem = cmd.FillList<Comanda>().ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oitem;
        }

        public void TotalizarComanda(TLogDatabase odb, int cmdcodigo, short nusucodigo)
        {
            try
            {
                var comanda = GetComanda(odb, cmdcodigo);

                if (comanda == null || comanda.cmdstatus != "A")
                {
                    return;
                }

                using (var cmd = odb.NewCommand(@"update comanda set 
                    cmdvalor_total = (select sum(cmdivalor) from comanda_item where cmdcodigo = @cmdcodigo and cmdistatus not in ('B', 'X')),
                    cmdvalor_pago = (select sum(cmdivalor_pago) from comanda_item where cmdcodigo = @cmdcodigo and cmdistatus not in ('B', 'X')),
                    ultatualizacao = now(),
                    usucodigo = @usucodigo
                    where cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo;
                    odb.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void TotalizarComandaPedidoVenda(TLogDatabase odb, int cmdcodigo, short nusucodigo)
        {
            try
            {
                var comanda = GetComanda(odb, cmdcodigo);

                if (comanda == null || comanda.cmdstatus != "A")
                {
                    return;
                }

                using (var cmd = odb.NewCommand(@"update comanda set 
                    cmdvalor_total = (select sum(cmdivalor*cmdiquantidade) from comanda_item where cmdcodigo = @cmdcodigo and cmdistatus not in ('B', 'X')),
                    cmdvalor_pago = (select sum(cmdivalor_pago*cmdiquantidade) from comanda_item where cmdcodigo = @cmdcodigo and cmdistatus not in ('B', 'X')),
                    ultatualizacao = now(),
                    usucodigo = @usucodigo
                    where cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo;
                    odb.AddInt16Parameter(cmd, "@usucodigo").Value = nusucodigo;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void EfetivarPagamentoComanda([FromBody] ComandaPagamento comandaPag)
        { //Se for P (pago à vista, no caso para clientes externos) não inserir no movimento, caso F (faturado, pode inserir movimento)
            TLogDatabase odb = Db.GetDbSCH();
            Int16 nseq = 1;
            odb.Open();
            var filename = "";
            SefaRet sefaRet = new SefaRet();
            //chamar função que vai processar o arquivo

            var oCabCupomRetorno = InserirCabCupom(odb, comandaPag);
            foreach (ComandaItens item in comandaPag.olist)
            {
                InserirIteCupom(odb, comandaPag.usunumero_origem, oCabCupomRetorno, item, nseq);
                nseq++;

            }
            //SE FOR CLIENTE EXTERNO APTFICHA 0
            NFCESerie cupomNFCERet = new NFCESerie();
            cupomNFCERet = InserirCabCupomNFCE(odb, oCabCupomRetorno, comandaPag);
            if(comandaPag.aptficha == 0 && comandaPag.cmdnumero_origem == 0)
            {
                UpdateCabCupom(odb, comandaPag, cupomNFCERet.NFSRNF, oCabCupomRetorno);
                filename = GetFileName(odb, comandaPag.empcodigo, cupomNFCERet.NFSRNF, cupomNFCERet.NFSRSERIE);
                sefaRet = ProcessarNFCE(oCabCupomRetorno, comandaPag.empcodigo, filename); //passar o: oCabCupomRetorno.cabficha,
                UpdateCabCupomNFCE(odb, sefaRet, oCabCupomRetorno, comandaPag.usunumero_origem);
                UpdateCabCupomNFCEXML(odb, oCabCupomRetorno, sefaRet, comandaPag.usunumero_origem);
            } else
            {
                InserirMovimento(odb, comandaPag, cupomNFCERet.NFSRNF);
                UpdateCabCupom(odb, comandaPag, cupomNFCERet.NFSRNF, oCabCupomRetorno);
                filename = GetFileName(odb, comandaPag.empcodigo, cupomNFCERet.NFSRNF, cupomNFCERet.NFSRSERIE);
                sefaRet = ProcessarNFCE(oCabCupomRetorno, comandaPag.empcodigo, filename); //passar o: oCabCupomRetorno.cabficha,
                UpdateCabCupomNFCE(odb, sefaRet, oCabCupomRetorno, comandaPag.usunumero_origem);
                UpdateCabCupomNFCEXML(odb, oCabCupomRetorno, sefaRet, comandaPag.usunumero_origem);
            }
           
        }

        public static string GetFileName(TLogDatabase odb, int empcodigo, int nfsrnf, string nfsrSerie)
        {
            string fileName = ""; //cnpjdaempresa_sequencialnfsrnf_001.L000 001 = nfsrserie
            try
            {
                using (DbCommand cmd = odb.NewCommand(@"
                SELECT EMPCODIGO, EMPNOME, EMPFANTASIA, EMPCNPJ
                FROM Sagres.dbo.EMPRESA WHERE EMPCODIGO = @EMPCODIGO;
                 "))
                {
                    odb.Open();
                    odb.AddIntParameter(cmd, "@EMPCODIGO").Value = empcodigo;
                    using (DbDataReader reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            fileName = (reader["EMPCNPJ"].ToString().Trim() + "_" + nfsrnf + "_" + nfsrSerie);
                        }
                    odb.Close();

                }

            }
            catch (Exception ex)
            {
                odb.Close();
                throw;
            }
            return fileName;
        }

        public static void ProcessarNFCE000(int empcodigo, int cabficha, string fileName, string ip)
        {


            string cSep = "|";
            string cfilename = fileName;
            StringBuilder sb = new StringBuilder();
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version.ToString();
            TLogDatabase odb = Db.GetDbSCH();
            odb.Open();
            var empuf = "";
            var empibge = "";
            var chkcodigo = "";
            var departamento = "";
            var garcon = "";
            var hospede = "";
            var apto = "";
            var cest = new ComandaRepositorio().PisCofinsCest(odb).cest;
            var piscofins = new ComandaRepositorio().PisCofinsCest(odb).piscofins;
            decimal totalVpag = 0;
            decimal total = 0;
            //string pathRequisicao = @"C:\logicom\apps\nfce\req\"; //cnpjdaempresa_sequencialnfsrnf_001.L000
            string pathRequisicao = @"\\"+ ip +@"\logicom\apps\nfce\req\"; //cnpjdaempresa_sequencialnfsrnf_001.L000
            if (!Directory.Exists(pathRequisicao))
            {
                Directory.CreateDirectory(pathRequisicao);
            }
            string parametro = new ParametroRepositorio().GetInformacaoMaquina("offlineNFCE");
            try
            {
                
                // Primeira linha a ser concatenada
                sb.Append("C").Append(cSep)
                  .Append(parametro) // Se você tiver lógica para `IfThen(param.lofflineNFCe, 'N', 'S')`, adicione aqui
                  .Append(cSep)
                  .Append(pathRequisicao) //meter aqui ip da maquina/rota compartilhamento
                  .Append(cSep)
                  .Append("S") 
                  .AppendLine();

                using (DbCommand cmd = odb.NewCommand(@"SELECT EMP.*,
                       CID.CIDCODIGO_IBGE AS CIDCODIGOIBGE,
                       CID.CIDNOME,
                       1058 AS PAICODIGO,
                       'BRASIL' AS PAINOME,
                       EST.UFCODIGO_IBGE AS UFCODIGOIBGE
                  FROM EMPRESA EMP
                  INNER JOIN CIDADE CID ON CID.CIDCODIGO = 2
                  LEFT JOIN UF EST ON EST.UFCODIGO = EMP.UFCODIGO
                 WHERE EMP.EMPCODIGO = @EMPCODIGO"))
                {
                    odb.AddIntParameter(cmd, "@EMPCODIGO").Value = empcodigo;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            empuf = reader["UFCODIGOIBGE"].ToString();
                            empibge = reader["CIDCODIGOIBGE"].ToString();
                            // Segunda linha a ser concatenada
                            sb.Append("E").Append(cSep)
                              .Append(reader["EMPCNPJ"].ToString()).Append(cSep)
                              .Append(reader["EMPNOME"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPFANTASIA"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPENDERECO"].ToString().Trim()).Append(cSep)
                              .Append("").Append(cSep)
                              .Append(reader["EMPBAIRRO"].ToString().Trim()).Append(cSep)
                              .Append(reader["CIDCODIGOIBGE"].ToString().Trim()).Append(cSep)
                              .Append(reader["CIDNOME"].ToString().Trim()).Append(cSep)
                              .Append(reader["UFCODIGOIBGE"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPCEP"].ToString().Trim()).Append(cSep)
                              .Append(reader["PAICODIGO"].ToString().Trim()).Append(cSep)
                              .Append(reader["PAINOME"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPFONE"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPIE"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPREGIME_TRIBUTARIO"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPCERTIFICADODIGITAL"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPCERT_PIN"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPCSCSEQ"].ToString().Trim()).Append(cSep)
                              .Append(reader["EMPCSC"].ToString().Trim())
                              .AppendLine(); // Adicionar quebra de linha após a linha de dados
                        }
                    }
                }
                using (DbCommand cmd4 = odb.NewCommand(@" SELECT H.HOSNOME, 
                       R.HOSFICHA, 
                       C.CABFICHA, 
                       C.CABCUPOMFISCAL, 
                       C.CABDTEMISSAO, 
                       C.CABIMPRESSO,
                       C.DEPCODIGO, 
                       DEP.DEPDESCRICAO, 
                       C.USUCODIGO, 
                       C.CABVALOR, 
                       C.CABAPTNUMERO, 
                       C.CABSTATUS,
                       C.CABNMESA,
                       C.GARCODIGO,
                       G.GARNOME,
                       C.EMPCODIGO,
                       CABPGFICHA,
                       CABPREVISAOPAGTO
                  FROM CABCUPOM C 
                  LEFT OUTER JOIN REGISTRO R
                               ON C.REGFICHA = R.REGFICHA
                  LEFT OUTER JOIN HOSPEDES H
                               ON R.HOSFICHA = H.HOSFICHA
                  LEFT OUTER JOIN GARSON G
                               ON (C.GARCODIGO = G.GARCODIGO)
                  LEFT JOIN DEPARTAMENTO DEP
                               ON C.DEPCODIGO = DEP.DEPCODIGO
                 WHERE C.CABFICHA = @CABFICHA
                "))
                {
                    odb.AddIntParameter(cmd4, "@CABFICHA").Value = cabficha;

                    using (DbDataReader reader4 = cmd4.ExecuteReader())
                    {
                        if (reader4.Read())
                        {
                            departamento = reader4["DEPDESCRICAO"].ToString().Trim();
                            garcon = reader4["GARNOME"].ToString().Trim();
                            hospede = reader4["HOSNOME"].ToString().Trim();
                            apto = reader4["CABAPTNUMERO"].ToString().Trim();
                        }
                    }
                }
                using (DbCommand cmd3 = odb.NewCommand(@"
                SELECT N.*,
                       CAM.CAMCODIGO,
                       CAM.CAMNOME
                  FROM CABCUPOM_NFCE N
                  LEFT JOIN COMANDA_CAB CCB
                         ON CCB.CABFICHA = N.CABFICHA
                  LEFT JOIN CAMAREIRA CAM
                          ON CAM.CAMCODIGO = CCB.CAMCODIGO
                WHERE N.CABFICHA = @NCABFICHA"))
                {
                    odb.AddIntParameter(cmd3, "@NCABFICHA").Value = cabficha;

                    using (DbDataReader reader3 = cmd3.ExecuteReader())
                    {
                        if (reader3.Read())
                        {
                            chkcodigo = reader3["CHKCODIGO"].ToString().Trim();
                            //Convert.ToDateTime(reader3["CBNEMISSAO"]);
                            // Linha com os dados da segunda query
                            sb.Append("N").Append(cSep)
                                .Append(empuf).Append(cSep)
                                .Append("VENDA").Append(cSep)
                                .Append("2").Append(cSep)
                                .Append("65").Append(cSep)
                                .Append(reader3["NFSRSERIE"].ToString().Trim()).Append(cSep)
                                .Append(reader3["CBNCUPOMFISCAL"].ToString().Trim()).Append(cSep)
                                .Append(Convert.ToDateTime(reader3["CBNEMISSAO"]).ToString("yyyy-MM-dd").Trim()).Append("T")
                                .Append(Convert.ToDateTime(reader3["CBNEMISSAO"]).ToString("hh:mm:ss").Trim()).Append("-03:00").Append(cSep)
                                .Append(Convert.ToDateTime(reader3["CBNEMISSAO"]).ToString("yyyy-MM-dd").Trim()).Append("T")
                                .Append(Convert.ToDateTime(reader3["CBNEMISSAO"]).ToString("hh:mm:ss").Trim()).Append("-03:00").Append(cSep)
                                .Append("1").Append(cSep)
                                .Append(empibge.Trim()).Append(cSep)
                                .Append("4").Append(cSep)
                                .Append(reader3["CBNTIPOEMISSAO"].ToString().Trim()).Append(cSep)
                                .Append("1").Append(cSep)
                                .Append("1").Append(cSep)
                                .Append("1").Append(cSep)
                                .Append(reader3["CBNTIPOAMBIENTE"].ToString().Trim()).Append(cSep)
                                .Append("1").Append(cSep)
                                .Append("0").Append(cSep)
                                .Append("9").Append(cSep)
                                .Append(reader3["CHKCODIGO"].ToString().Trim()).Append(cSep)
                                .Append(reader3["CABFICHA"].ToString().Trim()).Append(cSep)
                                .Append("0").Append(cSep)
                                .Append(reader3["USUCODIGO"].ToString().Trim()).Append(cSep)
                                .Append(piscofins).Append(cSep) //   SELECT * FROM PARAMETRO p where p.PARNOME  = 'NFCE_PIS_COFINS' DEFAULT N (se retornar nulo é N)
                                .Append("R").Append(cSep)
                                .Append(version).Append(cSep)

                              .AppendLine(); // Adicionar quebra de linha após cada linha de dados
                        }
                    }
                }

                // Executar a segunda query
                using (DbCommand cmd2 = odb.NewCommand(@"SELECT I.ITESEQUENCIA,
               P.PRODESCRICAO, 
               I.PROCODIGO, 
               I.ITEQUANT,
               I.ITEVALORUNITA,
               (ITEQUANT*ITEVALORUNITA) TOTAL,
               P.PROCODIMPFIS,
               P.PRONCM,
               PROUNIDADE EMBCODIGO,  
               FFS.FFSCODIGO,
               FFS.FFSCSTPIS PROCSTPIS,
               FFS.FFSCSTCOFINS PROCSTCOFINS,
               FFS.FFSCSTICMS PROCSTICMS,
               FFS.FFSALIQPIS PROALIQPIS,
               FFS.FFSALIQCOFINS PROALIQCOFINS,
               FFS.FFSALIQICMS PROALIQICMS,
               NCM.NCMALIQ_EST,
               NCM.NCMALIQ_IMP,
               NCM.NCMALIQ_NAC,
               A.CTAORIGEM,
               P.PROCODBARRA,
               CFOCODIGO_IF,
               CESTCHAVE
          FROM PRODUTO P
         INNER JOIN ITECUPOM I
                 ON P.PROCODIGO = I.PROCODIGO
          LEFT JOIN FIGURA_FISCAL_SAIDA FFS
                 ON P.FFSCODIGO = FFS.FFSCODIGO
          LEFT JOIN SITUACAO_TRIBUTARIA_NFISCAL SFN
                 ON FFS.FFSCSTICMS = SFN.PRO_SIT_TRIB_NF
          LEFT JOIN NCM
                 ON P.PRONCM = NCM.PRONCM
          LEFT JOIN CST_TABELA_A A
                 ON FFS.CTACODIGO = A.CTACODIGO
          LEFT JOIN CEST
                 ON CEST.CESTCODIGO = P.CESTCODIGO
        WHERE I.CABFICHA = @CABFICHA"))
                {
                    odb.AddIntParameter(cmd2, "@CABFICHA").Value = cabficha;

                    using (DbDataReader readerItem = cmd2.ExecuteReader())
                    {
                        while (readerItem.Read())
                        {
                            total = Convert.ToDecimal(readerItem["TOTAL"]);
                            totalVpag += total; // Soma os valores de TOTAL
                            decimal impostoEst = 0;
                            decimal impostoFed = 0;
                            decimal impostoTotal = 0;

                            var oValidaCestString = "";
                            bool oValidaCest = true;
                            /*query para verificar se o cest ta valido ou nao*/


                            if (readerItem["NCMALIQ_NAC"].ToString() == "")
                            {
                                impostoEst = Convert.ToDecimal(readerItem["PROALIQICMS"]);
                                impostoFed = Convert.ToDecimal(readerItem["PROALIQPIS"]) + Convert.ToDecimal(readerItem["PROALIQFINS"]);
                            }
                            else
                            {
                                impostoEst = Convert.ToDecimal(readerItem["NCMALIQ_EST"]);
                                if (readerItem["CTAORIGEM"].ToString() == "N")
                                {
                                    impostoFed = Convert.ToDecimal(readerItem["NCMALIQ_NAC"]);
                                }
                                else if (readerItem["CTAORIGEM"].ToString() == "E")
                                {
                                    impostoFed = Convert.ToDecimal(readerItem["NCMALIQ_IMP"]);
                                }
                                else
                                {
                                    impostoFed = Convert.ToDecimal(readerItem["PROALIQPIS"]) + Convert.ToDecimal(readerItem["PROALIQFINS"]);
                                }

                            }
                            impostoTotal = impostoFed + impostoEst;
                            // Linha com os dados da segunda query
                            sb.Append("I").Append(cSep)
                              .Append(readerItem["PROCODIGO"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["ITESEQUENCIA"].ToString()).Append(cSep)
                              .Append(readerItem["PROCODBARRA"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["PRODESCRICAO"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["PRONCM"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["CFOCODIGO_IF"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["EMBCODIGO"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["ITEQUANT"].ToString().Replace(",", ".").Trim()).Append(cSep)
                              .Append(readerItem["ITEVALORUNITA"].ToString().Replace(",", ".").Trim()).Append(cSep)
                              .Append(total.ToString("F2")).Replace(",", ".").Append(cSep) // Usa a variável total
                              .Append(readerItem["PROCODBARRA"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["EMBCODIGO"].ToString().Trim()).Append(cSep)
                              .Append("").Append(cSep)
                              .Append(readerItem["ITEVALORUNITA"].ToString().Replace(",", ".").Trim()).Append(cSep)
                              .Append(0).Append(cSep)
                              .Append(0).Append(cSep)
                              .Append(0).Append(cSep)
                              .Append(0).Append(cSep)
                              .Append(1).Append(cSep)
                              .Append(readerItem["PROCSTICMS"].ToString().Substring(0, 1).Trim()).Append(cSep)
                              .Append(readerItem["PROCSTICMS"].ToString().Substring(1, 2).Trim()).Append(cSep)
                              .Append(3).Append(cSep)
                              .Append("").Append(cSep)
                              .Append(readerItem["PROALIQICMS"].ToString().Trim()).Append(cSep)
                              .Append("").Append(cSep)
                              .Append(readerItem["PROCSTPIS"].ToString().Trim()).Append(cSep)
                              .Append("").Append(cSep)
                              .Append(readerItem["PROALIQPIS"].ToString().Trim()).Append(cSep)
                              .Append(readerItem["PROCSTCOFINS"].ToString().Trim()).Append(cSep)
                              .Append("").Append(cSep)
                              .Append(readerItem["PROALIQCOFINS"].ToString().Trim()).Append(cSep)
                              .Append((total * impostoTotal).ToString("F2")).Append(cSep)
                              .Append("0.00").Append(cSep)
                              .Append("0.00").Append(cSep)
                              .Append("0.00").Append(cSep)
                              .Append(cest).Append(cSep)
                              .Append("")
                              .AppendLine(); // Adicionar quebra de linha após cada linha de dados
                        }
                    }
                }

                // Adicionar a linha final com o total calculado
                sb.Append("P").Append(cSep)
                  .Append("05").Append(cSep) // tpag
                  .Append(totalVpag.ToString("F2").Replace(",", ".")).Append(cSep) // vpag
                  .Append("").Append(cSep) // CNPJ credenciadora
                  .Append("").Append(cSep) // tBand
                  .Append("").Append(cSep) // cAut
                  .Append(""); // vpagtot
                sb.AppendLine();
                sb.Append('O').Append(cSep).Append("Comanda V: ").Append(version);

                File.WriteAllText(pathRequisicao + cfilename + ".L000", sb.ToString());
                odb.Close();
            } catch (Exception)
            {
                odb.Close();
                throw;
            }
        }
        public static void ProcessarNFCE003(int cabficha, string fileName, string ip)
        {
            TLogDatabase odb = Db.GetDbSCH();
            // string pathRequisicao = @"C:\logicom\apps\nfce\req\"; //cnpjdaempresa_sequencialnfsrnf_001.L000
            string pathResposta = @"\\" + ip + @"\logicom\apps\nfce\resp\";
            odb.Open();
            string cSep = "|";
            string cfilename = fileName; //nome do arquivo
            StringBuilder sb = new StringBuilder();
            StringBuilder sb003 = new StringBuilder();
            var chkcodigo = "";
            var departamento = "";
            var garcon = "";
            var hospede = "";
            var apto = "";
            decimal total = 0;
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version.ToString();


            try
            {
                using (DbCommand cmd4 = odb.NewCommand(@" SELECT H.HOSNOME, 
                       R.HOSFICHA, 
                       C.CABFICHA, 
                       C.CABCUPOMFISCAL, 
                       C.CABDTEMISSAO, 
                       C.CABIMPRESSO,
                       C.DEPCODIGO, 
                       DEP.DEPDESCRICAO, 
                       C.USUCODIGO, 
                       C.CABVALOR, 
                       C.CABAPTNUMERO, 
                       C.CABSTATUS,
                       C.CABNMESA,
                       C.GARCODIGO,
                       G.GARNOME,
                       C.EMPCODIGO,
                       CABPGFICHA,
                       CABPREVISAOPAGTO
                  FROM CABCUPOM C 
                  LEFT OUTER JOIN REGISTRO R
                               ON C.REGFICHA = R.REGFICHA
                  LEFT OUTER JOIN HOSPEDES H
                               ON R.HOSFICHA = H.HOSFICHA
                  LEFT OUTER JOIN GARSON G
                               ON (C.GARCODIGO = G.GARCODIGO)
                  LEFT JOIN DEPARTAMENTO DEP
                               ON C.DEPCODIGO = DEP.DEPCODIGO
                 WHERE C.CABFICHA = @CABFICHA
                "))
                {
                    odb.AddIntParameter(cmd4, "@CABFICHA").Value = cabficha;

                    using (DbDataReader reader4 = cmd4.ExecuteReader())
                    {
                        if (reader4.Read())
                        {
                            departamento = reader4["DEPDESCRICAO"].ToString().Trim();
                            garcon = reader4["GARNOME"].ToString().Trim();
                            hospede = reader4["HOSNOME"].ToString().Trim();
                            apto = reader4["CABAPTNUMERO"].ToString().Trim();
                        }
                    }
                }
                //                // Executar a segunda query
                using (DbCommand cmd2 = odb.NewCommand(@"SELECT I.ITESEQUENCIA,
               P.PRODESCRICAO, 
               I.PROCODIGO, 
               I.ITEQUANT,
               I.ITEVALORUNITA,
               (ITEQUANT*ITEVALORUNITA) TOTAL,
               P.PROCODIMPFIS,
               P.PRONCM,
               PROUNIDADE EMBCODIGO,  
               FFS.FFSCODIGO,
               FFS.FFSCSTPIS PROCSTPIS,
               FFS.FFSCSTCOFINS PROCSTCOFINS,
               FFS.FFSCSTICMS PROCSTICMS,
               FFS.FFSALIQPIS PROALIQPIS,
               FFS.FFSALIQCOFINS PROALIQCOFINS,
               FFS.FFSALIQICMS PROALIQICMS,
               NCM.NCMALIQ_EST,
               NCM.NCMALIQ_IMP,
               NCM.NCMALIQ_NAC,
               A.CTAORIGEM,
               P.PROCODBARRA,
               CFOCODIGO_IF,
               CESTCHAVE
          FROM PRODUTO P
         INNER JOIN ITECUPOM I
                 ON P.PROCODIGO = I.PROCODIGO
          LEFT JOIN FIGURA_FISCAL_SAIDA FFS
                 ON P.FFSCODIGO = FFS.FFSCODIGO
          LEFT JOIN SITUACAO_TRIBUTARIA_NFISCAL SFN
                 ON FFS.FFSCSTICMS = SFN.PRO_SIT_TRIB_NF
          LEFT JOIN NCM
                 ON P.PRONCM = NCM.PRONCM
          LEFT JOIN CST_TABELA_A A
                 ON FFS.CTACODIGO = A.CTACODIGO
          LEFT JOIN CEST
                 ON CEST.CESTCODIGO = P.CESTCODIGO
        WHERE I.CABFICHA = @CABFICHA"))
                {
                    odb.AddIntParameter(cmd2, "@CABFICHA").Value = cabficha;

                    using (DbDataReader readerItem = cmd2.ExecuteReader())
                    {
                        while (readerItem.Read())
                        {
                            total = Convert.ToDecimal(readerItem["TOTAL"]);
                        }
                    }
                }


                int NCABFICHA = cabficha; // Exemplo de valor para NCABFICHA
                string chk = chkcodigo; // Exemplo de valor para cCKT
                DateTime dAtual = DateTime.Now; // Exemplo de valor para dAtual
                string cSep2 = "#"; // Separador

                //odb.NewCommand(@"SELECT * FROM ")
                string cMsg = new StringBuilder()
                .Append("Id: ").Append(NCABFICHA)
                .Append(" Ckt: " + chk)
                .Append(" Impresso: ").Append(dAtual.ToString("dd/MM/yyyy HH:mm:ss"))
                .Append(" App: Comanda " + version).Append(cSep2)
                .Append(" Departamento: " + departamento).Append(cSep2)
                .Append(" Garcon: " + garcon).Append(cSep2)
                .Append("Apto: " + apto + " - " + hospede).Append(cSep2)
                .Append(cSep).ToString();

                sb003.Append("T").Append(cSep)
                     .Append("").Append(cSep)
                     .Append("").Append(cSep)
                     .Append("").Append(cSep)
                     .Append("").Append(cSep)
                     .Append(cMsg).AppendLine();

                sb003.Append("P").Append(cSep)
                     .Append("05").Append(cSep)
                     .Append(total.ToString("F2")).Replace(",", ".").Append(cSep)
                     .Append("").Append(cSep)
                     .Append("").Append(cSep)
                     .Append("").Append(cSep)
                     .Append(total.ToString("F2")).Replace(",", ".").Append(cSep);

                File.WriteAllText(pathResposta + cfilename + ".L003", sb003.ToString());
                
            } catch (Exception ex)
            {
                File.WriteAllText("" + cfilename + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", ex.Message);
            }
        }
        public SefaRet ProcessarRetornoNFCE(string fileName, string ip)
        {
            string cfilename = fileName;
            string pathResposta = @"\\" + ip + @"\logicom\apps\nfce\resp\";
            SefaRet sefaRet = new SefaRet();
            using(StreamReader sr = new StreamReader(pathResposta + cfilename + ".L001")) //mudarpra vriavel de cfilename
            {
                while (sr.Peek() >= 0)
                {
                    string str;
                    string[] strArray;
                    str = sr.ReadLine();
                    strArray = str.Split('|');

                    sefaRet.tipoemissao = strArray[1];
                    sefaRet.tipoambiente = strArray[2];
                    sefaRet.chaveDoc = strArray[3];
                    sefaRet.digestValue = strArray[6];
                    sefaRet.status = strArray[7];
                    sefaRet.motivo = strArray[8];
                    sefaRet.sefaXML = strArray[10];
                }
            }
            return sefaRet;
        }


        public SefaRet ProcessarNFCE(int cabficha, int empcodigo, string fileName)
        {
            SefaRet sefaRet = new SefaRet();
            ConexaoRepositorio repository = new ConexaoRepositorio();

            string parametroUsuario = new ParametroRepositorio().GetInformacaoMaquina("conexao_usuario");
            string parametroPassword = new ParametroRepositorio().GetInformacaoMaquina("conexao_senha");
            string parametroIp = new ParametroRepositorio().GetInformacaoMaquina("conexao_ip");

            try
            {
                // Conectar à pasta para ProcessarNFCE003
                ConexaoPC credentials003 = new ConexaoPC
                {
                    NetworkPath = @"\\" + parametroIp + @"\logicom\apps\nfce\resp",
                    Username = parametroUsuario,  // PARAMETRO DO BANCO
                    Password = parametroPassword     // PARAMETRO DO BANCO
                };

                if (!repository.ConnectToNetworkPath(credentials003))
                {
                    throw new Exception("Erro ao conectar à pasta de resposta para ProcessarNFCE003.");
                }

                ProcessarNFCE003(cabficha, fileName, parametroIp); // ip ser parametro do banco
                Thread.Sleep(3000);

                // Desconectar após ProcessarNFCE003
                repository.DisconnectFromNetworkPath(credentials003.NetworkPath);

                // Conectar à pasta para ProcessarNFCE000
                ConexaoPC credentials000 = new ConexaoPC
                {
                    NetworkPath = @"\\" + parametroIp + @"\logicom\apps\nfce\req",
                    Username = parametroUsuario,  // Substitua com o nome de usuário
                    Password = parametroPassword     // Substitua com a senha
                };

                if (!repository.ConnectToNetworkPath(credentials000))
                {
                    throw new Exception("Erro ao conectar à pasta de requisição para ProcessarNFCE000.");
                }

                ProcessarNFCE000(empcodigo, cabficha, fileName, parametroIp);

                // Desconectar após ProcessarNFCE000
                repository.DisconnectFromNetworkPath(credentials000.NetworkPath);

                // Conectar à pasta para ProcessarRetornoNFCE
                //Conexao credentialsRetorno = new Conexao
                //{
                //    NetworkPath = @"\\" + parametroIp + @"\logicom\apps\nfce\resp",
                //    Username = parametroUsuario,  // Substitua com o nome de usuário
                //    Password = parametroPassword     // Substitua com a senha
                //};

                //if (!Directory.Exists(credentialsRetorno.NetworkPath))
                //{
                //    Directory.CreateDirectory(credentialsRetorno.NetworkPath);
                //}

               // sefaRet = ProcessarRetornoNFCE(fileName, parametroIp);

                /* Criar função que irá esperar o resultado do processamento de
                   nfce da sefa e repetir a verificação até aprovado
                   caso demore mais que x segundos gerar o cupom em contingência */
            }
            catch (Exception ex)
            {
                throw new Exception("Não conseguiu processar o NFCE: " + ex.Message);
            }

            return sefaRet;
        }

        public PisCofinsCest PisCofinsCest(TLogDatabase odb)
        {
            PisCofinsCest pisCofinsCest = new PisCofinsCest();
        
            try
            {
                var oValidaCestString = "";
                bool oValidaCest = false;
                var pisCofins = "";
                //SELECT* FROM PARAMETRO p where p.PARNOME = 'NFCE_CEST_ATIVO'
                using (DbCommand cmd = odb.NewCommand(@"SELECT * FROM PARAMETRO p where p.PARNOME = 'NFCE_CEST_ATIVO'"))
                {
                    using (DbDataReader readerCest = cmd.ExecuteReader())
                        if (readerCest.Read())
                        {
                            oValidaCestString = readerCest["PARVALOR"].ToString().Trim();
                            if (oValidaCestString != "")
                            {
                                oValidaCest = true;
                                pisCofinsCest.cest = oValidaCestString;
                            }
                            else
                            {
                                oValidaCest = false;
                                pisCofinsCest.cest = "";
                            }
                        } 

                }
                using (DbCommand cmd2 = odb.NewCommand(@"SELECT * FROM PARAMETRO p where p.PARNOME = 'NFCE_PIS_COFINS'"))
                {
                    using(DbDataReader readerPisCofins = cmd2.ExecuteReader())
                    
                        if(readerPisCofins.Read())
                        {
                            pisCofins = readerPisCofins["PARVALOR"].ToString();
                            if (pisCofins != "")
                            {
                                pisCofinsCest.piscofins = "S";
                            }
                            else
                            {
                                pisCofinsCest.piscofins = "N";
                            }
                        } else
                        {
                            pisCofinsCest.piscofins = "N";
                        }                    
                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return pisCofinsCest;
            }
        public int InserirCabCupom(TLogDatabase odb, ComandaPagamento comandaPag)
        { //verificar se o regficha/apt for 0, inserir com o regficha null
            var cabCupomRet = new CabCupomRetorno();
            try
            {
                int cabFicha = GetGenId(odb, "CABFICHA");
                cabCupomRet.cabficha = cabFicha;
                TLogDatabase db = Db.GetDbFood();
                Comanda comanda = GetComanda(db, comandaPag.cmdcodigo);
                //talvez o empcodigo tenha que vir de um GetDepartamento usando o depcodigo
                using (var cmd = odb.NewCommand(@"INSERT INTO Sagres.dbo.CABCUPOM
                (CABFICHA, CABCUPOMFISCAL,
                CABDTEMISSAO, GARCODIGO, REGFICHA,
                CABIMPRESSO, CABNMESA,
                DEPCODIGO,
                USUCODIGO, CABVALOR, CABAPTNUMERO,
                CABPGFICHA, CABUSUPAGO, CABCHECKOUT,
                CABTIPO, CABSTATUS, LOCCODIGO,
                ECFCODIGO, CABCNPJ_CPF, EMPCODIGO,
                CABPREVISAOPAGTO, CABOBSERVACAO)
                VALUES(@CABFICHA, null, GETDATE(), @usucodigo, @cmdnumero_origem, 'S', @latchave,
                @DEPCODIGO, @usunumero_origem,
                @cmdvalor_total, @cmdapartamento, null, @cmdvalor_pago, 1, 'F',
                'A', 1, @LOCCODIGO, 
                null, @empcodigo, '', '');")) 
                {
                    odb.Open();
                    odb.StartTransaction();
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@CABFICHA").Value = cabFicha;
                    odb.AddInt16Parameter(cmd, "@usucodigo").Value = comandaPag.usucodigo;
                    if(comandaPag.cmdnumero_origem == 0)
                    {
                        odb.AddIntParameter(cmd, "@cmdnumero_origem").Value = DBNull.Value;
                    } else
                    {
                        odb.AddIntParameter(cmd, "@cmdnumero_origem").Value = comandaPag.cmdnumero_origem;
                    }
                   
                  //  odb.AddStringParameter(cmd, "@CABIMPRESSO").Value = comandaPag.cabimpresso; //TODO: S OU N
                    odb.AddStringParameter(cmd, "@latchave").Value = comandaPag.latchave;
                    odb.AddIntParameter(cmd, "@DEPCODIGO").Value = comandaPag.depcodigo;
                    odb.AddIntParameter(cmd, "@usunumero_origem").Value = comandaPag.usunumero_origem;
                    odb.AddFloatParameter(cmd, "@cmdvalor_total").Value = comandaPag.cmdvalor_total;
                    odb.AddIntParameter(cmd, "@cmdapartamento").Value = comandaPag.cmdapartamento;
                    odb.AddFloatParameter(cmd, "@cmdvalor_pago").Value = comandaPag.cmdvalor_pago;
                    odb.AddIntParameter(cmd, "@LOCCODIGO").Value = comandaPag.loccodigo;
                    odb.AddIntParameter(cmd, "@empcodigo").Value = comandaPag.empcodigo;
                    cmd.ExecuteNonQuery();
                    odb.Commit();
                    odb.Close();
                }
            }
            catch (Exception ex)
            {
                odb.RollBack();
                odb.Close();
                throw ex;
            }
            
            return cabCupomRet.cabficha;
        }

        public int GetGenId(TLogDatabase odb, string ger)
        {
            int genId = 0;
            try
            {
                using (var cmd = odb.NewCommand(@"               
                USE [Sagres]
                DECLARE @GER CHAR(20), @INCREM INT;

                set @GER = @GER_PARAM; set @INCREM = 1; 

                EXEC [dbo].[GEN_ID] @GER, @INCREM, @SAIDA OUTPUT;"
                ))

                //SELECT @SAIDA"))
                {
                    // Adiciona o parâmetro para @GER
                    var gerParam = new SqlParameter("@GER_PARAM", ger);
                    cmd.Parameters.Add(gerParam);

                    // Adiciona o parâmetro de saída @SAIDA
                    var outputParam = new SqlParameter("@SAIDA", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();

                    // Captura o valor do parâmetro de saída
                    genId = (int)cmd.Parameters["@SAIDA"].Value;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return genId;
        }


        public NFCESerie GetNFCESerie(int empcodigo)
        { 

            var db = Db.GetDbSCH();
            NFCESerie nFCESerie = new NFCESerie();
            try
            {
                using (var cmd = db.NewCommand(@"SELECT * 
                 FROM NFCE_SERIE NFSR
                 WHERE NFSR.NFSRSTATUS = 'A'
                 AND EMPCODIGO = @EMPCODIGO"))
                {
                    db.Open();
                    db.AddIntParameter(cmd, "@EMPCODIGO").Value = empcodigo;
                    cmd.Transaction = db.oTxn;
                    using (DbDataReader reader = cmd.ExecuteReader())
                        if (reader.Read())
                        {
                            nFCESerie.CHKCODIGO = reader["CHKCODIGO"].GenericConvert<int>();
                            nFCESerie.EMPCODIGO = reader["EMPCODIGO"].GenericConvert<int>();
                            nFCESerie.NFSRCODIGO = reader["NFSRCODIGO"].GenericConvert<int>();
                            nFCESerie.USUCODIGO = reader["USUCODIGO"].GenericConvert<int>();
                            nFCESerie.NFSRNF = reader["NFSRNF"].GenericConvert<int>();
                            nFCESerie.NFSRSERIE = reader["NFSRSERIE"].ToString().Trim();
                            nFCESerie.NFSRSTATUS = reader["NFSRSTATUS"].ToString().Trim();
                        }
                    db.Close();
                }
                    
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
           return nFCESerie;
        }

        //public int GetCabpgficha(TLogDatabase odb, int loccodigo)
        //{
        //    int genCabPgFicha = 0;
        //    try
        //    {
        //        using (var cmd = odb.NewCommand(@"
        //        SELECT MAX(CABPGFICHA) AS CABPGFICHA
        //        FROM CABCUPOM
        //        WHERE
        //           LOCCODIGO = @LOCCODIGO"
        //        ))

                
        //        {
        //            // Adiciona o parâmetro para @GER
        //            var gerParam = new SqlParameter("@GER_PARAM", ger);
        //            cmd.Parameters.Add(gerParam);

        //            // Adiciona o parâmetro de saída @SAIDA
        //            var outputParam = new SqlParameter("@SAIDA", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //            cmd.Parameters.Add(outputParam);

        //            cmd.ExecuteNonQuery();

        //            // Captura o valor do parâmetro de saída
        //            genCabPgFicha = (int)cmd.Parameters["@SAIDA"].Value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    return genCabPgFicha;
        //}
    
    //RECEBER NA InserirIteCupom OS DADOS DE COMANDAITENS DA COMANDAPAGAMENTO DA FUNÇÃO INSERIRCABCUPOM
        public void InserirIteCupom(TLogDatabase odb, int nusucodigo, int cabCupomRet, ComandaItens comandaItens, Int16 nseq )
            {
                try
                {

                    using (var cmd = odb.NewCommand(@"INSERT INTO Sagres.dbo.ITECUPOM
                    (CABFICHA, ITESEQUENCIA, PROCODIGO, ITEQUANT, ITEVALORUNITA, FFSCODIGO, ULTATUALIZACAO, USUCODIGO)
                    VALUES(@CABFICHA, @ITESEQUENCIA, @procodigo, @proquantidade, @propreco_venda, null,
                    GETDATE(), @usucodigo);"))
                    {
                        odb.Open();
                        odb.StartTransaction();
                        cmd.Transaction = odb.oTxn;
                        odb.AddIntParameter(cmd, "@ITESEQUENCIA").Value = nseq;
                        odb.AddIntParameter(cmd, "@CABFICHA").Value = cabCupomRet;
                        odb.AddIntParameter(cmd, "@procodigo").Value = comandaItens.procodigo;
                       odb.AddInt16Parameter(cmd, "@proquantidade").Value = comandaItens.proquantidade;
                        odb.AddIntParameter(cmd, "@propreco_venda").Value = comandaItens.propreco_venda;
                        odb.AddIntParameter(cmd, "@usucodigo").Value = nusucodigo;                   

                        cmd.ExecuteNonQuery();
                        odb.Commit();
                        odb.Close();
                    }

                } catch (Exception ex)
                {
                    odb.RollBack();
                    odb.Close();
                    throw new Exception(ex.Message);
                }
            }
            //RECEBER NA InserirCabCupomNFCE OS DADOS DE COMANDAITENS DA COMANDAPAGAMENTO DA FUNÇÃO INSERIRCABCUPOM
        public NFCESerie InserirCabCupomNFCE(TLogDatabase odb, int cabCupomRet, ComandaPagamento comandaPagamento)
        {
           // CupomNFCERet cupomNFCERet = new CupomNFCERet();
            NFCESerie cupomNFCERet = GetNFCESerie(comandaPagamento.empcodigo);
            UpdateNFCESerie(odb, cupomNFCERet.NFSRCODIGO, (cupomNFCERet.NFSRNF + 1), comandaPagamento);

            try
            {
                using (var cmd = odb.NewCommand(@"INSERT INTO Sagres.dbo.CABCUPOM_NFCE
                (CABFICHA, NFSRSERIE, CBNCUPOMFISCAL,
                CBNTIPOAMBIENTE, CBNTIPOEMISSAO,
                CBNSTATUSTRANSM, CBNCODSTATUS,
                CBNCHAVEACESSO, CBNQRCODE, CBNXML,
                CBNCPF_CNPJ, CBNSTATUS, ULTATUALIZACAO,
                USUCODIGO, CHKCODIGO, CBNEMISSAO, EMPCODIGO)
                VALUES(@CABFICHA, @NFSRSERIE, @NFSRNF,
                @CBNTIPOAMBIENTE, 1, 'P', null, null, null, 
                null, @cpfcnpj, 'A', GETDATE(), @usucodigo,
                @CHKCODIGO, GETDATE(), @empcodigo);")) /*P(CBNSTATUSTRANSM) vira S caso o status seja 1, vira N se for 9
                                                        O A(cbnstatus) vira F caso seja 1 ou 9, se for diferente fica A até processar
                                                        */
                {
                    odb.Open();
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@CBNTIPOAMBIENTE").Value = CBNTIPOAMBIENTE;
                    odb.AddIntParameter(cmd, "@CABFICHA").Value = cabCupomRet;
                    odb.AddStringParameter(cmd, "@NFSRSERIE").Value = cupomNFCERet.NFSRSERIE;
                    odb.AddIntParameter(cmd, "@NFSRNF").Value = (cupomNFCERet.NFSRNF +1);
                    odb.AddStringParameter(cmd, "@cpfcnpj").Value = comandaPagamento.cpfcnpj;
                    odb.AddIntParameter(cmd, "@usucodigo").Value = comandaPagamento.usunumero_origem;
                    odb.AddIntParameter(cmd, "@CHKCODIGO").Value = cupomNFCERet.CHKCODIGO;
                    odb.AddIntParameter(cmd, "@empcodigo").Value = comandaPagamento.empcodigo;
                    cmd.ExecuteNonQuery();
                    odb.Close();
                }

                //RETORNAR NFSRNF e inserir na tabela movimento (MovNFiscal)
            }
            catch (Exception ex)
            {
                odb.Close();
                throw new Exception(ex.Message);
            }
            return cupomNFCERet;
        }
        //RECEBER NA InserirMovimento OS DADOS DE COMANDAPAGAMENTO INSERIRCABCUPOM E CRIAR GET DE NFSRNF
        public void InserirMovimento(TLogDatabase odb, ComandaPagamento comandaPagamento, int NFSRNF)
        { //se aptficha/regficha for 0, enviar null
            try
            {
                using (var cmd = odb.NewCommand(@"insert into movimento
                    (AptFicha,regFicha,DepCodigo,MovNFiscal,MovData,MovValor
					,UsuCodigo,MovDTUltAlt,MovStatus)
					values (@aptficha, @cmdnumero_origem,
                    @depcodigo, @NFSRNF,
                    GETDATE(), @cmdvalor_total, @usucodigo, GETDATE(),'A');")) 
                {
                    odb.Open();
                    cmd.Transaction = odb.oTxn;
                    if(comandaPagamento.cmdnumero_origem == 0 && comandaPagamento.aptficha == 0)
                    {
                        odb.AddIntParameter(cmd, "@aptficha").Value = DBNull.Value;
                        odb.AddIntParameter(cmd, "@cmdnumero_origem").Value = DBNull.Value;
                    }
                    else
                    {
                        odb.AddIntParameter(cmd, "@aptficha").Value = comandaPagamento.aptficha;
                        odb.AddIntParameter(cmd, "@cmdnumero_origem").Value = comandaPagamento.cmdnumero_origem;
                    }
                    odb.AddIntParameter(cmd, "@depcodigo").Value = comandaPagamento.depcodigo;
                    odb.AddIntParameter(cmd, "@NFSRNF").Value = NFSRNF;
                    odb.AddFloatParameter(cmd, "@cmdvalor_total").Value = comandaPagamento.cmdvalor_total;
                    odb.AddIntParameter(cmd, "@usucodigo").Value = comandaPagamento.usucodigo;
                   
                    cmd.ExecuteNonQuery();
                    odb.Close();
                }

            }
            catch (Exception ex)
            {
                odb.Close();
                throw new Exception(ex.Message);
            }
        }
        public void UpdateCabCupom(TLogDatabase odb, ComandaPagamento comandaPagamento, int cabCupomFiscal, int cabficha)
        {
            try
            {
                using (var cmd = odb.NewCommand(@"UPDATE CABCUPOM SET 
                CABCUPOMFISCAL = @CABCUPOMFISCAL,
                CABIMPRESSO = @CABIMPRESSO,
                CABSTATUS = @CABSTATUS
                WHERE
                CABFICHA = @CABFICHA"))
                {
                    odb.Open();
                    odb.StartTransaction();
                    cmd.Transaction = odb.oTxn;
                    odb.AddStringParameter(cmd, "@CABCUPOMFISCAL").Value = cabCupomFiscal;
                    odb.AddStringParameter(cmd, "@CABIMPRESSO").Value = comandaPagamento.cabimpresso;
                    odb.AddIntParameter(cmd, "@CABFICHA").Value = cabficha;
                    odb.AddStringParameter(cmd, "@CABSTATUS").Value = comandaPagamento.tipo_pagamento;
                    cmd.ExecuteNonQuery();
                    odb.Close();
                }
            }
            catch (Exception ex)
            {
                odb.Close();
                throw new Exception(ex.Message);
            }
        }
        public void UpdateNFCESerie(TLogDatabase odb, int nfsrcodigo, int nfsrnf,  ComandaPagamento comandaPagamento)
        {
            try
            {
                using (var cmd = odb.NewCommand(@"UPDATE NFCE_SERIE
                   SET NFSRNF = @NFSRNF,
                       ULTATUALIZACAO = GETDATE(),
                       USUCODIGO = @USUCODIGO
                 WHERE NFSRCODIGO = @NFSRCODIGO"))
                {
                    odb.Open();
                    odb.StartTransaction();
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@NFSRCODIGO").Value = nfsrcodigo;
                    odb.AddInt16Parameter(cmd, "@USUCODIGO").Value = comandaPagamento.usunumero_origem;
                    odb.AddIntParameter(cmd, "@NFSRNF").Value = nfsrnf;
                    cmd.ExecuteNonQuery();
                    odb.Close();
                }
            }
            catch (Exception ex)
            {
                odb.Close();
                throw new Exception(ex.Message);
            }
        }
        public void UpdateCabCupomNFCE(TLogDatabase odb, SefaRet sefaRet, int cabficha, int usucodigo)
        {
            
            try
            {
                using (var cmd = odb.NewCommand(@"UPDATE dbo.CABCUPOM_NFCE
		        SET CBNSTATUSTRANSM = @CBNSTATUSTRANSM
		            ,CBNSTATUS = @CBNSTATUS
		            ,ULTATUALIZACAO = GETDATE()
		            ,USUCODIGO = @USUCODIGO
		        WHERE CABFICHA = @CABFICHA;"))
                {
                    odb.Open();
                    /*P(CBNSTATUSTRANSM) vira S caso o status seja 1, vira N se for 9
                     O A(cbnstatus) vira F caso seja 1 ou 9, se for diferente fica A até processar*/
                    if(sefaRet.tipoemissao == "1")
                    {
                        cmd.Transaction = odb.oTxn;
                        odb.AddStringParameter(cmd, "@CBNSTATUSTRANSM").Value = "S";
                        odb.AddStringParameter(cmd, "@CBNSTATUS").Value = "F";
                        odb.AddIntParameter(cmd, "@USUCODIGO").Value = usucodigo;
                        odb.AddIntParameter(cmd, "@CABFICHA").Value = cabficha;
                        cmd.ExecuteNonQuery();
                    } 
                    else if(sefaRet.tipoemissao == "9")
                    {
                        cmd.Transaction = odb.oTxn;
                        odb.AddStringParameter(cmd, "@CBNSTATUSTRANSM").Value = "N";
                        odb.AddStringParameter(cmd, "@CBNSTATUS").Value = "F";
                        odb.AddIntParameter(cmd, "@USUCODIGO").Value = usucodigo;
                        odb.AddIntParameter(cmd, "@CABFICHA").Value = cabficha;
                        cmd.ExecuteNonQuery();
                    }
                    odb.Close();
                }
            }
            catch (Exception ex)
            {
                odb.Close();
                throw new Exception(ex.Message);
            }
        }
        public void UpdateCabCupomNFCEXML(TLogDatabase odb, int cabficha, SefaRet sefaRet, int usucodigo)
        {
            try
            {
                using (var cmd = odb.NewCommand(@"UPDATE dbo.CABCUPOM_NFCE
		        SET CBNTIPOEMISSAO = @CBNTIPOEMISSAO
		              ,CBNSTATUSTRANSM = @CBNSTATUSTRANSM
		              ,CBNCODSTATUS = @CBNCODSTATUS
		              ,CBNCHAVEACESSO = @CBNCHAVEACESSO
		              ,CBNXML = @CBNXML
		              ,CBNSTATUS = @CBNSTATUS
		              ,ULTATUALIZACAO = GETDATE()
		              ,USUCODIGO = @USUCODIGO
		         WHERE CABFICHA = @CABFICHA;"))
                {
                    /*P(CBNSTATUSTRANSM) vira S caso o status seja 1, vira N se for 9
                     O A(cbnstatus) vira F caso seja 1 ou 9, se for diferente fica A até processar*/
                    odb.Open();
                    if (sefaRet.tipoemissao == "1") {
                        cmd.Transaction = odb.oTxn;
                        odb.AddIntParameter(cmd, "@CBNTIPOEMISSAO").Value = sefaRet.tipoemissao;
                        odb.AddStringParameter(cmd, "@CBNSTATUSTRANSM").Value = "S";
                        odb.AddIntParameter(cmd, "@CBNCODSTATUS").Value = sefaRet.status;
                        odb.AddStringParameter(cmd, "@CBNCHAVEACESSO").Value = sefaRet.chaveDoc;
                        //odb.AddStringParameter(cmd, "@CBNQRCODE").Value = cabCupomNFCE.qrcode;
                        byte[] sefaXMLBytes = System.Text.Encoding.UTF8.GetBytes(sefaRet.sefaXML);
                        odb.AddBlobParameter(cmd, "@CBNXML").Value = sefaXMLBytes; // Adicionar como BLOB
                        odb.AddStringParameter(cmd, "@CBNSTATUS").Value = "F";
                        odb.AddIntParameter(cmd, "@USUCODIGO").Value = usucodigo;
                        odb.AddIntParameter(cmd, "@CABFICHA").Value = cabficha;
                        cmd.ExecuteNonQuery();
                        odb.Close();
                    } else if (sefaRet.tipoemissao == "9")
                    {
                        cmd.Transaction = odb.oTxn;
                        odb.AddIntParameter(cmd, "@CBNTIPOEMISSAO").Value = sefaRet.tipoemissao;
                        odb.AddStringParameter(cmd, "@CBNSTATUSTRANSM").Value = "S";
                        odb.AddIntParameter(cmd, "@CBNCODSTATUS").Value = sefaRet.status;
                        odb.AddStringParameter(cmd, "@CBNCHAVEACESSO").Value = sefaRet.chaveDoc;
                        byte[] sefaXMLBytes = System.Text.Encoding.UTF8.GetBytes(sefaRet.sefaXML);
                        odb.AddBlobParameter(cmd, "@CBNXML").Value = sefaXMLBytes; // Adicionar como BLOB
                        odb.AddStringParameter(cmd, "@CBNSTATUS").Value = "F";
                        odb.AddIntParameter(cmd, "@USUCODIGO").Value = usucodigo;
                        odb.AddIntParameter(cmd, "@CABFICHA").Value = cabficha;
                        cmd.ExecuteNonQuery();
                        odb.Close();
                    }


                }
            }
            catch (Exception ex)
            {
                odb.Close();
                throw new Exception(ex.Message);
            }
        }

        public static decimal SumValorPago(Comanda comanda)
        {
            decimal sum = 0;
            sum += comanda.cmdvalor_total;
            return sum;
        }
        /*public Comanda GetComandasParaPreparo()
        {
            Comanda oitem = new Comanda();

            try
            {
                using (var cmd = odb.NewCommand(@"select * from comanda where cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo;
                    oitem = cmd.FillList<Comanda>().FirstOrDefault();
                }

                using (var cmd = odb.NewCommand(@"select * from comanda_item where cmdcodigo = @cmdcodigo"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@cmdcodigo").Value = cmdcodigo;
                    oitem.olistItens = cmd.FillList<ComandaItem>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oitem;
        }*/
    }
}
