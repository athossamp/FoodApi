using System.Text.Json.Serialization;

namespace log_food_api.Models
{
    public class Error
    {
        public int stat { get; set; }
        public string message { get; set; }

        public Error()
        { 
            stat = 0;
            message = string.Empty;
        }
    }
}
