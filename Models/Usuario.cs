namespace log_food_api.Models
{
    public class Usuario
    {
        public short usucodigo { get; set; }
        public string usunome { get; set; }
        public string usuapelido { get; set; }
        public decimal usumeta { get; set; }
        public short usunivel { get; set; }
        public string usustatus { get; set; }

        /// <summary>
        /// Tipo de usuário: V-Vendedor  G-Gerente
        /// </summary>
        public string usutipo { get; set; }
        public string usuemail { get; set; }
        public string usutelefone_ddd { get; set; }
        public string usutelefone { get; set; }
        public string usucpf { get; set; }
        public decimal usucomissao { get; set; }
        public string usulogin { get; set; }
        public string ususenha { get; set; }
        public int? usunumero_origem { get; set; }
        public string usutoken { get; set; }
        public DateTime usutoken_expira { get; set; }
        public int usucodigo_insercao { get; set; }
        public int transa { get; set; }
        public DateTime ultatualizacao { get; set; }
    }
    public class UsuarioVendedor
    {
        public int VENCODIGO { get; set; }
        public string VENNOME { get; set;}
    }
}
