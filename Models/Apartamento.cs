using System.Text.Json.Serialization;

namespace log_food_api.Models
{    public class Apartamento
    {
        public int aptficha { get; set; }
        public string aptnumero { get; set; }

        /// <summary>
        /// Define se a conta do apartamento e única ou separada (por hóspede). J-Junta  S-Separada
        /// </summary>
        public string rapjuntosep { get; set; }
        /// <summary>
        /// Quando a conta for conjunta (rapjuntosep = 'J'), utilizar o menor regficha (primeiro hospede regsitrado). Para hospedagem com contas separadas (rapjuntosep = 'S'), utilizar o regficha do hospede que solicitou o servico/item
        /// </summary>
        [JsonPropertyName("hospedes")]
        public List<Hospede> olistHospedes { get; set; }

        public Apartamento()
        {
            olistHospedes = new List<Hospede>();
        }
    }
}