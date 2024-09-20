using log_food_api.Models;
using Logicom.Infraestrutura;

namespace log_food_api.Repositories
{
    public class TituloRepositorio
    {
        public List<Titulo> GetTituloDevedor(int pescodigo)
        {
            try
            {
                TLogDatabase odb = Db.GetDbSCEF();
                odb.Open();
                odb.StartTransaction();
                List<Titulo> otitulo = new List<Titulo>();
                using (var cmd = odb.NewCommand(@"
                         SELECT T.EMPCODIGO
                                   ,E.EMPFANTASIA
                                   ,T.TITCODIGO
                                   ,T.TITDOCUMENTO
                                   ,T.TITVALOR 
                                   ,T.TITEMISSAO 
                                   ,T.TITRECEBIMENTO 
                                   ,T.TITOBSERVACAO 
                                   ,T.PESCODIGO 
                                   ,PS.PESAPELIDO AS PESNOME 
                                   ,PL.PLCPLANO 
                                   ,PL.PLCDESCRICAO 
                                   ,(P.PARPARCELA ||'/'||T.TITNUMPARCELAS)AS PARCELA
                                   ,P.PARVALORSEMDESCONTO 
                                   ,P.PARCODIGO 
                                   ,P.PARVENCSEMDESCONTO 
                                   ,P.PARVENCCOMDESCONTO
                                   ,P.PARSTATUS
                                   ,P.PAROBSERVACAO
                                   ,P.PARULTIMATRANSACAO 
                                   ,P.PARDESCONTO AS DESCONTO
                                   ,P.PARACRESCIMO  AS ACRESCIMO 
                                   ,P.PARTOTALPAGO  AS VALOR_PAGO
                                   ,(PARVALORSEMDESCONTO - P.PARDESCONTO + P.PARACRESCIMO - P.PARTOTALPAGO) AS SALDO_PARCELA
                                   ,T.TITTIPOORIGEM
                                   ,T.TITNUMEROORIGEM
                                 FROM TITULO T  
                                INNER JOIN PESSOA PS 
                                ON (T.PESCODIGO = PS.PESCODIGO) 
                                INNER JOIN PLANODECONTA PL 
                                ON (T.PLCCODIGO  = PL.PLCCODIGO) 
                                INNER JOIN EMPRESA E  
                                ON (T.EMPCODIGO  = E.EMPCODIGO) 
                                INNER JOIN PARCELA P  
                                ON (T.TITCODIGO  = P.TITCODIGO) 
                                WHERE T.TITSTATUS NOT IN ('C','S')
                                  AND T.PESCODIGO = @PESCODIGO
                                  AND P.PARSTATUS NOT IN ('R','S','P') 
                              AND (P.PARVALORSEMDESCONTO - P.PARDESCONTO + P.PARACRESCIMO - P.PARTOTALPAGO) >= 0.1
                              AND P.PARVENCCOMDESCONTO < CURRENT_TIMESTAMP
                              AND T.TITRECPAG = 'R'"))
                {
                    cmd.Transaction = odb.oTxn;
                    odb.AddIntParameter(cmd, "@PESCODIGO").Value = pescodigo;
                    otitulo = cmd.FillList<Titulo>();
                }

                return otitulo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
