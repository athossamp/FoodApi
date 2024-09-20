using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace log_food_api.Models
{
    public class ComandaItem
    {
        public int cmdicodigo { get; set; }
        public int cmdcodigo { get; set; }
        public string probalanca { get; set; }
        public string provende_fracionado { get; set; }

        /// <summary>
        /// Usuário que está atendendo o cliente/hóspede.
        /// </summary>
        public short? usucodigo_atendimento { get; set; }
        public int procodigo { get; set; }

        /// <summary>
        /// Código do Produto.
        /// </summary>
        [Required]
        public int precodigo { get; set; }
        public decimal cmdivalor { get; set; }
        public string? cmdiobservacao { get; set; }
        public decimal cmdiquantidade { get; set; }
        public decimal prequantidade { get; set; }
        public string procodbarra { get; set; }
        /// <summary>
        /// Status do item: B-Bloqueado  A-Aberto  W-EmPreparoCozinha  Z-FimPreparoCozinha  E-Entregue  X-Excluído  P-Pago/F-Faturado
        /// </summary>
        [Required]
        public string cmdistatus { get; set; }

        /// <summary>
        /// /* Quando for Hotelaria regficha do hóspede ou quando SCEF pescodigo */
        /// </summary>
        public int? cmdinumero_origem { get; set; }
        public string cmdipreparo { get; set; }

        /// <summary>
        /// Usuário "logado" no aplicativo que está lançando o item na comanda.
        /// </summary>
        public short usucodigo { get; set; }
        public DateTime ultatualizacao { get; set; }
        public string? prodescricao { get; set; }

        /// <summary>
        /// Código do Local de Preparo do Item.
        /// </summary>
        public short  latcodigo_preparo { get; set; }

        /// <summary>
        /// Nome do Local de Preparo do Item: COPA / COZINHA
        /// </summary>
        public string? latnome_preparo { get; set; }
        public string embcodigo {  get; set; }

        public Boolean edit {  get; set; }

        [JsonPropertyName("composicao")]
        public List<ComandaItemComposicao>? olistComposicao { get; set; }

        public ComandaItem()
        {
            cmdipreparo = "N";
        }
    }

    public class ComandaItemStatus
    {
        /// <summary>
        /// Código da comanda.
        /// </summary>
        public int cmdcodigo { get; set; }

        /// <summary>
        /// Código do item da comanda.
        /// </summary>
        public int cmdicodigo { get; set; }

        /// <summary>
        /// Status do item a ser atualizado: B-Bloqueado  A-Aberto  W-EmPreparoCozinha  Z-FimPreparoCozinha  E-Entregue  X-Excluído  P-Pago/F-Faturado
        /// </summary>
        //[Required]
        public string? cmdistatus { get; set; }
        public string? cmdiobservacao { get; set; }
    }

    public class ComandaItemPagamento
    {
        public int cmdicodigo { get; set; }
        public decimal valor_sem_taxa { get; set; }
        public decimal valor_taxa_servico { get; set; }
        public decimal valor_pago { get; set; }
        public decimal valor_faturado { get; set; }
    }
}