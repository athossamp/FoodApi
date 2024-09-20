using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace log_food_api.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <example>teste de exemplo</example>
    public class Comanda
    {
        public int cmdcodigo { get; set; }

        /// <summary>
        /// Código do Local de Atedimento registrado na Comanda.
        /// </summary>
        public short latcodigo { get; set; }
        public int? cmdapartamento { get; set; }
        public short usucodigo_abertura { get; set; }
        public string cmdobservacao { get; set; }
        public decimal cmdvalor_total { get; set; }
        public decimal cmdvalor_taxa_servico { get; set; }
        public decimal cmdvalor_pago { get; set; }

        /// <summary>
        /// Status da comanda: [A]berta  [F]echada  [C]ancelada  [U]nificada
        /// </summary>
        [Required]
        public string cmdstatus { get; set; } = null!;
        public DateTime? cmdabertura { get; set; }
        public DateTime? cmdfechamento { get; set; }
        public int? cmdnumero_origem { get; set; }
        public int? transa { get; set; }
        public int usucodigo { get; set; }
        public DateTime? ultatualizacao { get; set; }

        /// <summary>
        /// Nome do Local de Atedimento registrado na Comanda.
        /// </summary>
        public string? latnome { get; set; }
        public string? latchave { get; set; }
        public int loccodigo { get; set; }
        public int depcodigo { get; set; }
        public int cpgcodigo { get; set; }
        public int fpgcodigo { get; set; }
        public string cmdtipo { get; set; }

        [JsonPropertyName("itens")]
        public List<ComandaItem> olistItens { get; set; }
        public Comanda()
        {
            latcodigo = 0;
            usucodigo_abertura = 0;
            cmdvalor_total = 0;
            cmdvalor_taxa_servico = 0;
            cmdvalor_pago = 0;
            cmdstatus = "A";
        }
    }

    public class ComandaPagamento
    {
        public int latcodigo { get; set; }
        public int usucodigo { get; set; }
        public int cmdcodigo { get; set; }
        public string cabimpresso { get; set; }
        public string latchave { get; set; }
        public int depcodigo { get; set; }
        public int cmdnumero_origem {  get; set; }
        public int usunumero_origem { get; set; }
        public decimal cmdtaxa {  get; set; }
        public decimal cmdvalor_total { get; set; }
        public decimal cmdvalor_taxa { get; set; }
        public string cmdapartamento {  get; set; }
        public int aptficha { get; set; }
        public decimal cmdvalor_pago { get; set; }
        public int loccodigo { get; set; }
        public int empcodigo {  get; set; }
        public string cpfcnpj { get; set; }
        public decimal valor_total_faturado { get; set; }
        public string? tipo_pagamento {  get; set; }

        [JsonPropertyName("itens")]
        public List<ComandaItens> olist { get; set; }
    }

    public class ComandaPagamentoRetorno
    {
        public int cmdcodigo {  get; set; }
        public decimal valor_total_sem_taxa { get; set; }
        public decimal valor_total_taxa_servico { get; set; }
        public decimal valor_total_pago { get; set; }
        public decimal valor_total_faturado { get; set; }
        public decimal valor_pendente { get; set; }
        public string cmdstatus { get; set; }
        public DateTime cmdfechamento { get; set; }
        public string comprovante { get; set; }
    }
    public class CabCupomRetorno
    {
        public int cabficha;

    }
    public class ComandaItens
    {
        public int procodigo { get; set; }
        public decimal proquantidade { get; set; }
        public decimal propreco_venda { get; set; }
        public int usucodigo { get; set; }

    }
}
