using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace log_food_api.Models
{
    public class ComandaItemComposicao
    {
        public int? ciccodigo { get; set; }
        public int? cmdicodigo { get; set; }
        public int? procodigo { get; set; }
        public int? procodigo_composicao { get; set; }
        public short? usucodigo { get; set; }
        public DateTime? ultatualizacao { get; set; }
        public string prodescricao { get; set; }

        public ComandaItemComposicao() {
            ciccodigo = 0;
            cmdicodigo = 0;
            procodigo = 0;
            procodigo_composicao = 0;
            usucodigo = 0;
            prodescricao = "";
        }
        
    }
}