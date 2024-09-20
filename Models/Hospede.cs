namespace log_food_api.Models
{
    public class Hospede
    {
        /// <summary>
        /// Código de registro do hóspede no SCH Logicom.
        /// </summary>
        public int regficha { get; set; }
        public int hosficha { get; set; }
        public string hosnome { get; set; }
        public int hosidade { get; set; }
        public string hoscpf {  get; set; }
        public string fircgc { get; set; }
        public DateTime rapdtentrada { get; set; }

        /// <summary>
        /// Código da "Firma"/Empresa cadastrada no SCH Logicom. Quando preenchido, o hóspede está por conta da Empresa
        /// </summary>
        public int fircodigo { get; set; }

        /// <summary>
        /// Nome da "Firma"/Empresa cadastrada no SCH Logicom.
        /// </summary>
        public string firnome { get; set; }
    }
}