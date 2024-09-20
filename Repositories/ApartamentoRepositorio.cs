using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace log_food_api.Repositories
{
    public class ApartamentoRepositorio
    {
        public Apartamento GetApartamentoByApto(int? apto)
        {
            Apartamento a = new Apartamento();

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
                         ,F.FIRCODIGO 
                         ,F.FIRNOME
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
                    db.AddStringParameter(cmd, "@APTNUMERO").Value = apto;
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
                throw ex;
            }
            finally
            {
                db.Close();
            }

            return a;
        }
    }
}
