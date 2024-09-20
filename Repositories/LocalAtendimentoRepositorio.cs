using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Security.Cryptography;

namespace log_food_api.Repositories
{
    public class LocalAtendimentoRepositorio
    {
        public void UpdateStatus(TLogDatabase odb, short latcodigo, string latstatus)
        {
            LocalAtendimento local = new LocalAtendimento();

            try
            {
                using (var cmd = odb.NewCommand(@"update local_atendimento set latstatus = @latstatus where latcodigo = @latcodigo and lattipo <> 'I'"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@latcodigo").Value = latcodigo;
                    odb.AddStringParameter(cmd, "@latstatus").Value = latstatus;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LocalAtendimento> GetLocaisAtendimento()
        {
            List<LocalAtendimento> olist = new List<LocalAtendimento>();

            try
            {
                return Db.GetDbFood().NewCommand(@"select * from local_atendimento").FillList<LocalAtendimento>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LocalAtendimento GetLocalAtendimento(TLogDatabase db, short latcodigo)
        {
            try
            {
                using (var cmd = db.NewCommand(@"select * from local_atendimento where latcodigo = @latcodigo"))
                {
                    db.AddInt16Parameter(cmd, "@latcodigo").Value = latcodigo;

                    return cmd.FillList<LocalAtendimento>().FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     
        public List<AtendimentoInfo> GetLocalAtendAtendente()
        {
            List<AtendimentoInfo> groupedAtendimentoInfos = new List<AtendimentoInfo>();

            try
            {
                using (var cmd = Db.GetDbFood().NewCommand(@"select u.usucodigo, u.usunome,
        c.cmdcodigo, la.latnome, la.latcodigo, la.lattipo, la.latchave, la.depcodigo, la.loccodigo, c.cmdstatus  
        from usuario u
        inner join
        comanda c on u.usucodigo = c.usucodigo_abertura
        inner join
        local_atendimento la on c.latcodigo = la.latcodigo 
        where la.latstatus = 'O' and c.cmdstatus = 'A' order by la.latcodigo"))
                {
                    var atendimentoInfos = cmd.FillList<AtendimentoInfoIntermediate>();
                    var atendimentoInfosGrouped = atendimentoInfos.GroupBy(a => new { a.latcodigo, a.latnome, a.latchave, a.lattipo, a.usunome, a.depcodigo, a.loccodigo })
                                                                  .Select(g => new AtendimentoInfo
                                                                  {
                                                                      latcodigo = g.Key.latcodigo,
                                                                      latnome = g.Key.latnome,
                                                                      latchave = g.Key.latchave,
                                                                      lattipo = g.Key.lattipo,
                                                                      usunome = g.Key.usunome,
                                                                      depcodigo = g.Key.depcodigo,
                                                                      loccodigo = g.Key.loccodigo,
                                                                      olistcomandas = g.Select(a => new AtendimentoInfoComanda
                                                                      {
                                                                          cmdcodigo = a.cmdcodigo,
                                                                          usucodigo = a.usucodigo,
                                                                          
                                                                      }).ToList()
                                                                  }).ToList();

                    groupedAtendimentoInfos.AddRange(atendimentoInfosGrouped);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return groupedAtendimentoInfos;
        }
       public void checaComanda()
        {
            TLogDatabase odb = Db.GetDbSCEF();
            TLogDatabase db = Db.GetDbFood();
            int maxInterbase = 0;
            int maxPostgre = 0;
            db.Open();
            try
            {
                
               
                using (DbCommand cmdIb = odb.NewCommand(@"SELECT MAX(CMDCODIGO) FROM COMANDA"))
                {
                    odb.Open();
                    using (DbDataReader reader = cmdIb.ExecuteReader())
                        if (reader.Read())
                        {
                            maxInterbase = reader["MAX"].GenericConvert<int>();
                        }
                        
                }
                using (DbCommand cmdPgsql = db.NewCommand(@"SELECT MAX(latcodigo) FROM local_atendimento"))
                {
                    db.Open();
                    using (DbDataReader readerPgsql = cmdPgsql.ExecuteReader())
                        if(readerPgsql.Read())
                    {
                        maxPostgre = readerPgsql["max"].GenericConvert<int>();
                    }
                }
                   
                if(maxInterbase > maxPostgre)
                {
                    using (DbCommand cmdIb = odb.NewCommand(@"SELECT CMDCODIGO FROM COMANDA WHERE CMDSTATUS = 'A' AND CMDCODIGO > " + maxPostgre ))
                    {
                        odb.Open();
                        using (DbDataReader reader = cmdIb.ExecuteReader())
                            while (reader.Read())
                            {
                              int cmdcodigo = reader["CMDCODIGO"].GenericConvert<int>();
                                using (var cmdInsert = db.NewCommand(@"INSERT INTO local_atendimento
                                (latcodigo, lattipo, latnome, latstatus, usucodigo, ultatualizacao, cmdcodigo_origem)
                                VALUES(@latcodigo, 'C','Comanda "+ cmdcodigo +"', 'D', 1, NOW(), @cmdcodigo);"))
                                {
                                    db.AddIntParameter(cmdInsert, "@latcodigo").Value = cmdcodigo;
                                    db.AddIntParameter(cmdInsert, "@cmdcodigo").Value = cmdcodigo;
                                    cmdInsert.ExecuteNonQuery();

                                }
                            }
                    }

                    }

                    db.Commit();
                    odb.Close();
                    db.Close();
            } catch(Exception ex)
            {
                db.RollBack();
                odb.Close();
                db.Close();
                throw ex;
            }
        }
    }
   
}

