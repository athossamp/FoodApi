using System.Text.Json.Serialization;

namespace log_food_api.Models
{
    public class Produto
    {
        public int procodigo { get; set; }
        public int precodigo { get; set; }
        public int clmcodigo { get; set; }
        public int cdpcodigo { get; set; }
        public string embcodigo { get; set; }
        public decimal prequantidade { get; set; }
        public string prodescricao { get; set; }
        public string prodescricao_adicional { get; set; }
        public decimal propreco_venda { get; set; }
        public decimal propreco_custo { get; set; }
        public string procodbarra { get; set; }
        public string protipo { get; set; }
        public string proadicional { get; set; }
        public string propreparo { get; set; }
        public int latcodigo_preparo { get; set; }
        public string prostatus { get; set; }
        public byte[] proimagem { get; set; }
        public int? pronumero_origem { get; set; }
        public int transa { get; set; }
        public short usucodigo { get; set; }
        public DateTime ultatualizacao { get; set; }

        public string cdptitulo { get; set; }
        public string cdpdescricao { get; set; }

        /// <summary>
        /// Produto(s) que compõem o produto/prato (utilizado em casos que o item pode ser substituído por um item definido como substituto).
        /// </summary>
        [JsonPropertyName("composicao")]
        public List<Produto_Composicao> olistComposicao { get; set; }
        public Produto()
        {
            procodigo = 0;
            prodescricao = string.Empty;
            olistComposicao = new List<Produto_Composicao>();
        }
    }
    public class Produto_Composicao
    {
        public int pcpcodigo { get; set; }
        public int procodigo_pai_default { get; set; }
        public int procodigo_default { get; set; }
        public string prodescricao_default { get; set; }
        public string pcpadicional { get; set; }
        public string pcpisento_cobranca { get; set; }

        /// <summary>
        /// Produto(s) que podem substituir um item que compõe o produto/prato original.
        /// </summary>
        [JsonPropertyName("substitutos")]
        public List<Produto_Composicao_Substituto> olistSubstituto { get; set; }

        public Produto_Composicao()
        {
            olistSubstituto = new List<Produto_Composicao_Substituto>();
        }
    }
    public class Produto_Composicao_Substituto
    {
        public int procodigo_pai_composicao { get; set; }
        public int procodigo_troca { get; set; }
        public string prodescricao_troca { get; set; }
        public decimal pcsbvalor_adicional { get; set; }
    }

    public class ProdutoSCH
    {
        public int procodigo { get; set; }
        public string prodescricao { get; set; }
        public decimal proprecounita { get; set; }
        public string prounidade { get; set; }
        public string procodimpfis { get; set; }
        public string protipo { get; set; }
        public string prostatus { get; set; }
        public int ffscodigo { get; set; }
        public string proncm { get; set; }
        public string procodbarra { get; set; }
        public int cestcodigo { get; set; }
    }
    public class ProdutoP
    {
        public string CODBARRAPRODUTO { get; set; }
        public decimal NESTOQUE_ATUAL { get; set; }
        public string PROVENDE_FRACIONADO { get; set; }
        public string PROBALANCA {  get; set; }
        public double NPRECOVENDA { get; set; }

        public int NPROCODIGO { get; set; }
        public string SPRODESCRICAO { get; set; }

        public List<PROEMBALA> proembala { get; set; }
        public ProdutoP()
        {
            NESTOQUE_ATUAL = 0;
            NPRECOVENDA = 0;
            NPROCODIGO = 0;
            SPRODESCRICAO = "";
        }
    }
        public class CODBAR
        {
            public string procodbarra { get; set; }
             public int precodigo { get; set; }
        }
        public class PROEMBALA
        {
            public int procodigo { get; set; }
            public int precodigo { get; set; }
            public decimal prequantidade { get; set; }
            public string embcodigo { get; set; }
            public List<CODBAR> codbar { get; set;}
        }

       
    }
    public class ProdutoEstoqueComanda
    {
        public int LOCCODIGO { get; set; }
        public int PROCODIGO { get; set; }
        public string PROVENDE_FRACIONADO { get; set; }
        public string PROBALANCA {  get; set; }
        public string PRODESCRICAO { get; set; }
        public decimal ESTATUAL { get; set; }
        public int cmdicodigo { get; set; }
        public decimal cmdivalor { get; set; }
        public string cmdistatus { get; set; }
        public decimal cmdiquantidade { get; set; }
        public string cmdiobservacao { get; set; }
        public decimal prequantidade { get; set; }
        public string embcodigo { get; set; }
        public string procodbarra { get; set; }
    public int precodigo { get; set; }
    }
