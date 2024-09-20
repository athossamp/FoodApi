using log_food_api.Models;
using Logicom.Infraestrutura;
using System.Data.Common;
using System.Text;
using System.Windows.Forms;

namespace log_food_api.Repositories
{
    public class PessoaRepositorio
    {
       public List<Pessoa> GetClientes(string pesnome, int pescodigo)
        {
        List<Pessoa> olist = new List<Pessoa>();
        TLogDatabase odb = Db.GetDbSCEF();
    try
    {
        if (pescodigo == 0)
        {
            using (var cmd = odb.NewCommand(@"SELECT
                  P.PESCODIGO, P.PESNOME, P.PESAPELIDO, PC.PCLSTATUS, PC.CPGCODIGO, PC.FPGCODIGO
                  FROM PESSOA P
                  inner join PESCLIENTE PC
                  ON PC.PESCODIGO = P.PESCODIGO
                  WHERE PC.PCLSTATUS = 'A'
                  AND (P.PESNOME CONTAINING(@PESNOME))"))
            {
                odb.AddStringParameter(cmd, "@PESNOME").Value = pesnome;
                odb.Open();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pessoa p = new Pessoa();
                        var pessoaNome = reader["pesnome"];

                        p.PESNOME = pessoaNome.ToString().Trim();
                        p.PESAPELIDO = reader["pesapelido"].ToString().Trim();
                        p.PESCODIGO = reader["pescodigo"].GenericConvert<int>();
                        p.CPGCODIGO = reader["cpgcodigo"].GenericConvert<int>();
                        p.FPGCODIGO = reader["fpgcodigo"].GenericConvert<int>();
                        olist.Add(p);
                    }
                }
                return olist;
            }
        }
        else
        {
            using (var cmd = odb.NewCommand(@" SELECT
                  P.PESCODIGO, P.PESNOME, P.PESAPELIDO, PC.PCLSTATUS, PC.CPGCODIGO, PC.FPGCODIGO
                  FROM PESSOA P
                  inner join PESCLIENTE PC
                  ON PC.PESCODIGO = P.PESCODIGO
                  WHERE PC.PCLSTATUS = 'A'
                  AND P.PESCODIGO = @PESCODIGO"))
            {
                odb.AddStringParameter(cmd, "@PESCODIGO").Value = pescodigo;
                odb.Open();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Pessoa p = new Pessoa();
                        var pessoaNome = reader["pesnome"];
                        p.PESNOME = pessoaNome.ToString().Trim();
                        p.PESAPELIDO = reader["pesapelido"].ToString().Trim();
                        p.PESCODIGO = reader["pescodigo"].GenericConvert<int>();
                        p.CPGCODIGO = reader["cpgcodigo"].GenericConvert<int>();
                        p.FPGCODIGO = reader["fpgcodigo"].GenericConvert<int>();
                        olist.Add(p); 
                    }
                }
                return olist;
            }
        }
    }
    catch (Exception ex)
    {
        throw ex;
    }
}


        //public Pessoa GetPessoaByPescodigo(TLogDatabase odb, int pescodigo)
        //{
        //    Pessoa pessoa = new Pessoa();
        //    try
        //    { 
        //        using(var cmd = odb.NewCommand(@"SELECT * FROM PESSOA WHERE PESCODIGO = @PESCODIGO"))
        //        {
        //            cmd.Transaction = odb.oTxn;
        //            odb.AddIntParameter(cmd, @"PESCODIGO").Value = pescodigo;
        //        }

        //    }
        //}
    }
}
