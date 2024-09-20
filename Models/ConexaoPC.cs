namespace log_food_api.Models
{
    public class ConexaoPC
    {
        public string NetworkPath { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public struct NETSOURCE
    {
        public int dwScope { get; set; }
        public int dwType { get; set; }
        public int dwDisplayType { get; set; }
        public int dwUsage { get; set; }
        public string lpLocalName { get; set; }
        public string lpRemoteName { get; set; }
        public string lpComment { get; set; }
        public string lpProvider { get; set; }
    }
}
