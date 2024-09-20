using log_food_api.Models;
using Logicom.Infraestrutura;
using System.Data.Common;

namespace log_food_api
{
    public class ProdutoRepositorio
    {
        public Produto GetByCodigo(TLogDatabase db, int nprocodigo)
        {
            Produto oitem = new Produto();

            try
            {
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
                           and p.procodigo = @nprocodigo"))
                {
                    db.AddIntParameter(cmd, "@nprocodigo").Value = nprocodigo;
                    cmd.Transaction = db.oTxn;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
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
                            oitem.prostatus = reader["prostatus"].ToString();
                            oitem.ultatualizacao = reader["ultatualizacao"].GenericConvert<DateTime>();

                            /*if (reader["proimagem"] != DBNull.Value)
                            {
                                oitem.proimagem = (byte[])reader["proimagem"];
                            }*/

                            oitem.cdptitulo = reader["cdptitulo"].ToString();
                            oitem.cdpdescricao = reader["cdpdescricao"].ToString();
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
                    cmd.Transaction = db.oTxn;

                    Produto_Composicao procomp = new Produto_Composicao();
                    Produto_Composicao_Substituto proSub = new Produto_Composicao_Substituto();

                    cmd.Parameters[0].Value = oitem.procodigo;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (oitem.olistComposicao.Find(p => p.procodigo_default == reader["procodigo_default"].GenericConvert<int>()) == null)
                            {
                                procomp = new Produto_Composicao();
                                procomp.pcpcodigo = reader["pcpcodigo"].GenericConvert<int>();
                                procomp.procodigo_pai_default = reader["procodigo_pai_default"].GenericConvert<int>();
                                procomp.procodigo_default = reader["procodigo_default"].GenericConvert<int>();
                                procomp.prodescricao_default = reader["prodescricao_default"].ToString().Trim();
                                procomp.pcpadicional = reader["pcpadicional"].ToString().Trim();
                                procomp.pcpisento_cobranca = reader["pcpisento_cobranca"].ToString().Trim();
                                oitem.olistComposicao.Add(procomp);
                            }

                            procomp = oitem.olistComposicao.Find(p => p.procodigo_default == reader["procodigo_default"].GenericConvert<int>());

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
            catch (Exception ex)
            {
                throw ex;
            }

            return oitem;
        }
        public ProdutoSCH InserirProdutoSCH(ProdutoSCH produto)
        {
            var db = Db.GetDbSCH();
            db.Open();
            try
            {
                using (DbCommand cmd = db.NewCommand(@"INSERT INTO Sagres.dbo.PRODUTO
                (PROCODIGO, PRODESCRICAO, PROPRECOUNITA, PROUNIDADE, PROCODIMPFIS, PROTIPO,
                PROSTATUS, FFSCODIGO, PRONCM, PROCODBARRA, CESTCODIGO)
                VALUES(@procodigo, @prodescricao, @proprecounita, @prounidade, @procodimpfis,
                @protipo, @prostatus, @ffscodigo, @proncm, @procodbarra, @cestcodigo);"))
                {
                    db.AddIntParameter(cmd, "@procodigo").Value = produto.procodigo;
                    db.AddStringParameter(cmd, "@prodescricao").Value = produto.prodescricao;
                    db.AddFloatParameter(cmd, "@proprecounita").Value = produto.proprecounita;
                    db.AddStringParameter(cmd, "@prounidade").Value = produto.prounidade;
                    db.AddStringParameter(cmd, "@procodimpfis").Value = produto.procodimpfis;
                    db.AddStringParameter(cmd, "@protipo").Value = produto.protipo;
                    db.AddStringParameter(cmd, "@prostatus").Value = produto.prostatus;
                    db.AddIntParameter(cmd, "@ffscodigo").Value = produto.ffscodigo;
                    db.AddStringParameter(cmd, "@proncm").Value = produto.proncm;
                    db.AddStringParameter(cmd, "@procodbarra").Value = produto.procodbarra;
                    db.AddIntParameter(cmd, "@cestcodigo").Value = produto.cestcodigo;
                    cmd.ExecuteNonQuery();
                    db.Close();
                    InserirProduto(produto);
                }
                return produto;
                
            }
            catch (Exception ex) 
            {
                db.Close();
                throw new Exception(ex.Message);
            }
        }
        public ProdutoSCH InserirProduto(ProdutoSCH produto)
        {
            var db = Db.GetDbFood();
            db.Open();
            try
            {
                using (DbCommand cmd = db.NewCommand(@"INSERT INTO PRODUTO(procodigo, embcodigo, prequantidade,
            prodescricao, propreco_venda, propreco_custo,
            procodbarra, latcodigo_preparo, prostatus,
            pronumero_origem, usucodigo, ultatualizacao) VALUES
            (@procodigo, @embcodigo, 1,
            @prodescricao, @propreco_venda, @propreco_custo,
            '', 9, 'A',
            @procodigo, 0, NOW())"))
                {
                    db.AddIntParameter(cmd, "@procodigo").Value = produto.procodigo;
                    db.AddStringParameter(cmd, "@embcodigo").Value = produto.prounidade;
                    db.AddStringParameter(cmd, "@prodescricao").Value = produto.prodescricao;
                    db.AddFloatParameter(cmd, "@propreco_venda").Value = produto.proprecounita;
                    db.AddFloatParameter(cmd, "@propreco_custo").Value = produto.proprecounita;
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
                return produto;
            }
            catch (Exception ex)
            {
                db.Close();
                throw new Exception(ex.Message);
            }
        }
        public Produto UpdateProduto(Produto produto)
        {
            var db = Db.GetDbFood();
            db.Open();
             try
            {
                using (DbCommand cmd = db.NewCommand(@"UPDATE produto
                SET cdpcodigo=@cdpcodigo,
                proadicional=@proadicional, propreparo=@propreparo,
                latcodigo_preparo=@latcodigo_preparo,
                WHERE procodigo=@procodigo;"))
                {
                    db.AddIntParameter(cmd, "@procodigo").Value = produto.procodigo;
                    db.AddIntParameter(cmd, "@cdpcodigo").Value = produto.cdpcodigo;
                    db.AddStringParameter(cmd, "@embcodigo").Value = produto.embcodigo;
                    db.AddStringParameter(cmd, "@proadicional").Value = produto.proadicional;
                    db.AddIntParameter(cmd, "@latcodigo_preparo").Value = produto.latcodigo_preparo;
                    cmd.ExecuteNonQuery();
                    db.Close();
                }
                return produto;
            }
            catch (Exception ex)
            {
                db.Close();
                throw new Exception(ex.Message);
            }
         }
    }

}
