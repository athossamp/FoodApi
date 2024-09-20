namespace log_food_api.Models
{
    public class CondicaoPagamento
    {
        public int CPGCODIGO { get; set; }
        public string CPGDESCRICAO { get; set; }
        public int CPGPARCELAS { get; set; }
        public decimal CPGPERCENTUALENTRADA { get; set; }
        public int CPGPRAZOMEDIO { get; set; }
        public decimal CPGPERCPRECO_VENDA { get; set; }
        public int USUCODIGO { get; set; }
        public decimal CPGVALORADICIONAL { get; set; }
        public string CPG_AVISTA { get; set; }
        public int PLPCODIGO { get; set; }
        public int PLPCODIGO_MOB {  get; set; }
        public string CPGDESCONTO { get; set; }

    }
    public class FormaPagamento
    {
        public int FPGCODIGO { get; set; }
        public string FPGDESCRICAO { get; set; }
        public int USUCODIGO { get; set; }
        public string FPGPROMISSORIA { get; set; }
    }
}
