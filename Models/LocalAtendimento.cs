using System.Text.Json.Serialization;

namespace log_food_api.Models
{
    public class LocalAtendimento
    {
        public int latcodigo { get; set; }

        /// <summary>
        /// Tipo de Local de Atendimento. /* M-Mesa   C-Comanda   S-Salão   I-Interno(para COPA/COZINHA) */
        /// </summary>
        public string lattipo { get; set; }
        public string latchave { get; set; }
        public string latnome { get; set; }
        public string latdescricao { get; set; }
        public int? latnumero_origem { get; set; }

        /// <summary>
        /// Status do Local de Atendimento. /* D-Disponivel  R-Reservada  O-Ocupado  I-Inativo  */
        /// </summary>
        public string latstatus { get; set; }
        public int depcodigo { get; set; }
        public int loccodigo { get; set; }

        public LocalAtendimento()
        {
        }
    }

    /// <summary>
    /// Adicionado este model para captação de dados específicos para a tela pedidos abertos,
    /// no caso verificar que mesas estão ocupadas para que o usuário não precise ficar recadastrando.
    /// </summary>
    public class AtendimentoInfo
    {
        public int latcodigo { get; set; }
        public string latnome { get; set; }
        public string latchave { get; set; }
        public string lattipo { get; set; }
        public string usunome {  get; set; }
        public int loccodigo { get; set; }
        public int depcodigo { get; set; }

        [JsonPropertyName("comandas")]
        public List<AtendimentoInfoComanda>? olistcomandas { get; set; }

        public AtendimentoInfo() 
        {
            olistcomandas = new List<AtendimentoInfoComanda>();
        }
    }

    public class AtendimentoInfoComanda
    {
        public int cmdcodigo { get; set; }
        public int usucodigo { get; set; }
        
    }
    public class AtendimentoInfoIntermediate
    {
        public int latcodigo { get; set; }
        public string latnome { get; set; }
        public string latchave { get; set; }
        public string lattipo { get; set; }
        public int cmdcodigo { get; set; }
        public int usucodigo { get; set; }
        public string usunome { get; set; }
        public int loccodigo { get; set;}
        public int depcodigo { get; set;}
    }

}
