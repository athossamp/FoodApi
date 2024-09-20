using System.Text.Json;
using System.Text.Json.Serialization;

namespace log_food_api.Models
{
    public class Vendedor
    {
        public int vencodigo { get; set; }
        public string vennome { get; set; }
        public string venapelido { get; set; }
        public string venlogin { get; set; }
        public string vensenha { get; set; }
        public string venemail { get; set; }
        public string vencpf { get; set; }
        public Vendedor()
        {
            vencodigo = 0;
            vennome = "";
            venlogin = "";
            vensenha = "";
            venemail = "";
            vencpf = "";
        }
    }
}
