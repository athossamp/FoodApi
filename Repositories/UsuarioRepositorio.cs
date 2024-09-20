using Azure.Core;
using log_food_api;
using log_food_api.Models;
using Logicom.Infraestrutura;
using Microsoft.Net.Http.Headers;
using System.Data.Common;

namespace log_food_api
{
    public class UsuarioRepositorio
    {
        public UsuarioRepositorio()
        {
        }
        public Usuario GetUsuario(string cuser, string cpwd)
        {
            Usuario oitem = null;
            var db = Db.GetDbFood();

            try
            {
                db.Open();

                using (DbCommand cmd = db.NewCommand("select * from usuario where usulogin = @login and ususenha = @senha"))
                {
                    db.AddStringParameter(cmd, "@login").Value = cuser;
                    db.AddStringParameter(cmd, "@senha").Value = cpwd;

                    oitem = Utils.FillList<Usuario>(cmd).FirstOrDefault();

                    /*using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            oitem = new Vendedor();
                            oitem.vencodigo = reader["vencodigo"].GenericConvert<int>();
                            oitem.vennome = reader["vennome"].ToString();
                            oitem.venapelido = reader["venapelido"].ToString();
                            oitem.venlogin = reader["venlogin"].ToString();
                            oitem.vensenha = reader["vensenha"].ToString();
                            oitem.venemail = reader["venemail"].ToString();
                            oitem.vencpf = reader["vencpf"].ToString();
                        }
                    }*/
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

            return oitem;
        }

        public Usuario GetByCodigo(short nusucodigo, TLogDatabase db)
        {
            Usuario oitem = null;

            try
            {
                using (DbCommand cmd = db.NewCommand("select * from usuario where usucodigo = @nusucodigo"))
                {
                    db.AddInt16Parameter(cmd, "@nusucodigo").Value = nusucodigo;
                    cmd.Transaction = db.oTxn;

                    oitem = Utils.FillList<Usuario>(cmd).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oitem;
        }
        public string GetTipo(short nusucodigo, TLogDatabase db)
        {
            try
            {
                
                using (DbCommand cmd = db.NewCommand("select usutipo from usuario where usucodigo = @nusucodigo"))
                {
                    db.AddInt16Parameter(cmd, "@nusucodigo").Value = nusucodigo;
                    cmd.Transaction = db.oTxn;

                    return Utils.FillList<Usuario>(cmd).FirstOrDefault().usutipo;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetToken(short usucodigo, string ctoken, DateTime expiracao)
        {
            var db = Db.GetDbFood();

            try
            {
                var transa = db.StartTransaction();

                using (DbCommand cmd = db.NewCommand("update usuario set usutoken = @token, usutoken_expira=@usutoken_expira, ultatualizacao=now() where usucodigo = @usucodigo"))
                {
                    cmd.Transaction = transa;
                    db.AddStringParameter(cmd, "@token").Value = ctoken;
                    db.AddInt16Parameter(cmd, "@usucodigo").Value = usucodigo;
                    db.AddDateParameter(cmd, "@usutoken_expira").Value = expiracao;
                    cmd.ExecuteNonQuery();
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.RollBack();
                throw ex;
            }
        }

        public short GetUsuario(string ctoken)
        {
            var db = Db.GetDbFood();

            try
            {
                db.Open();

                using (DbCommand cmd = db.NewCommand("select usucodigo where usutoken = @token and usustatus = 'A'"))
                {
                    db.AddStringParameter(cmd, "@token").Value = ctoken;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        return reader[0].GenericConvert<short>();
                    }
                }
            }
            catch (Exception ex)
            {
                db.Close();
                throw ex;
            }

            return 0;
        }

        public short GetUsuarioByToken(HttpRequest req)
        {
            var db = Db.GetDbFood();

            try
            {
                db.Open();

                using (DbCommand cmd = db.NewCommand("select usucodigo from usuario where usutoken = @token and usustatus = 'A'"))
                {
                    var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer", "").Trim();

                    if (token == null || token.Length == 0)
                    {
                        throw new Exception("Usuário Inválido!");
                    }

                    db.AddStringParameter(cmd, "@token").Value = token;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader[0].GenericConvert<short>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                db.Close();
                throw ex;
            }

            throw new Exception("Usuário Inválido!");

        }
        public Usuario InserirUsuario(Usuario usuario)
        {
            var db = Db.GetDbFood();
            try
            {
                db.Open();
                using (DbCommand cmd = db.NewCommand("INSERT INTO usuario (usucodigo, usunome," +
                    " usuapelido, usumeta, usunivel," +
                    " usustatus, usutipo, usuemail," +
                    " usutelefone_ddd, usutelefone, " +
                    "usucpf, usucomissao, usulogin, " +
                    "ususenha, usunumero_origem, usutoken," +
                    "transa," +
                    " usucodigo_insercao, " +
                    "ultatualizacao)VALUES(nextval('usucodigo'), @usunome" +
                    ", @usuapelido, 0, @usunivel," + //se usutipo for G, usunivel = 9, usutipo for C usunivel = 6, etc
                    " 'A', @usutipo, @usuemail, @usutelefone_ddd," +
                    "@usutelefone, @usucpf, 0, @usulogin," +
                    " @ususenha, @garcodigo, '', 0, @usucodigo_insercao, now());"))
                {
                    db.AddStringParameter(cmd, "@usunome").Value = usuario.usunome;
                    db.AddStringParameter(cmd, "@usuapelido").Value = usuario.usuapelido;
                    db.AddStringParameter(cmd, "@usutipo").Value = usuario.usutipo;
                    db.AddIntParameter(cmd, "@usunivel").Value = usuario.usunivel;
                    db.AddStringParameter(cmd, "@usuemail").Value = usuario.usuemail == null ? DBNull.Value : usuario.usuemail;
                    db.AddStringParameter(cmd, "@usutelefone_ddd").Value = usuario.usutelefone_ddd;
                    db.AddStringParameter(cmd, "@usutelefone").Value = usuario.usutelefone == null ? DBNull.Value : usuario.usutelefone;
                    db.AddStringParameter(cmd, "@usucpf").Value = usuario.usucpf;
                    db.AddStringParameter(cmd, "@usulogin").Value = usuario.usulogin;
                    db.AddStringParameter(cmd, "@ususenha").Value = usuario.ususenha;
                    db.AddIntParameter(cmd, "@usucodigo_insercao").Value = usuario.usucodigo;
                    db.AddIntParameter(cmd, "@garcodigo").Value = usuario.usunumero_origem == null ? DBNull.Value : usuario.usunumero_origem;
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
                return usuario;
            } catch (Exception ex) {
                db.Close();
                throw new Exception(ex.Message);
            }
        }
        //public void UpdateSenha(Usuario usuario)
        //{
        //    TLogDatabase db = Db.GetDbFood();
        //    try
        //    {
        //        var user = new Usuario();
        //        using(DbCommand cmdUsulogin = db.NewCommand("SELECT * FROM usuario WHERE usulogin = @usulogin"))
        //        {
        //            db.AddStringParameter(cmdUsulogin, "@usulogin").Value = usuario.usulogin;
        //            user = cmdUsulogin.FillList<Usuario>().FirstOrDefault();
        //        }
        //        if(user.usulogin == null || user.usulogin == "")
        //        {
        //            throw new Exception("Usuário não encontrado!");
        //        }

        //        using(DbCommand cmd = db.NewCommand("UPDATE usuario SET ususenha = @ususenha WHERE usulogin = @usulogin"))
        //        {
        //            db.AddStringParameter(cmd, "@ususenha").Value = usuario.ususenha;
        //            db.AddStringParameter(cmd, "@usulogin").Value = usuario.usulogin;
        //        }
        //    } catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public List<UsuarioVendedor> GetUsuarioVendedor()
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                odb.Open();
                odb.StartTransaction();
                List<UsuarioVendedor> ousuario = new List<UsuarioVendedor>();
                using (var cmd = odb.NewCommand(@"SELECT V.VENCODIGO, V.VENNOME FROM VENDEDOR V"))
                {
                    cmd.Transaction = odb.oTxn;
                    ousuario = cmd.FillList<UsuarioVendedor>();
                }

                return ousuario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
