namespace log_food_api.Models
{
    public class Cardapio
    {
        public int cdpcodigo { get; set; }
        public string cdpplano { get; set; }
        public string cdptitulo { get; set; }
        public string cdpdescricao { get; set; }
        public int cdpnivel { get; set; }
        public string cdpsint { get; set; }
        public string cdptipo { get; set; }
        public int cdppai { get; set; }
        public string cdpstatus { get; set; }
        public int? cdpnumero_origem { get; set; }
        public int transa { get; set; }
        public int usucodigo { get; set; }
        public DateTime ultatualizacao { get; set; }

        public Cardapio()
        {

        }
    }
}
