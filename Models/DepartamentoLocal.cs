namespace log_food_api.Models
{
    public class Departamento
    {
        public int depcodigo { get; set; }
        public string depdescricao { get; set; }
        public string depcd { get; set; }
        public decimal depiss { get; set; }
        public int depnivel { get; set; }
        public decimal depcomissao { get; set; }
        public string depaltera { get; set; }
        public string depdiaria { get; set; }
        public string depdeducao { get; set; }
        public int depaglutinacao { get; set; }
        public string depagrupa { get; set; }
        public int empcodigo { get; set; }
        public string depnfce { get; set; }

    }
    public class Localizacao
    {
        public int loccodigo { get; set; }
        public string locdescricao { get; set; }
        public string locexigeapto { get; set; }
        public string locmesa { get; set; }
        public string locgarcon { get; set; }
        public string locfaturamentoopcional { get; set; }
    }

    public class DepartLocal
    {
        public int loccodigo { get; set; }
        public int depcodigo { get; set; }
        public string dplfaturamento { get; set; }
    }
}
