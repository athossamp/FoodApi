namespace log_food_api.Models
{

    public class NFCESerie
    {
        public int NFSRCODIGO { get; set; }
        public int CHKCODIGO { get; set; }
        public string NFSRSERIE { get; set; }
        public int NFSRNF { get; set; }
        public string NFSRSTATUS { get; set; }
        public int USUCODIGO { get; set; }
        public int EMPCODIGO { get; set; }
    }

    public class Movimento
    {
        public int aptficha { get; set; }
        public int cmdnumero_origem { get; set; }
        public int depcodigo { get; set; }
        public int NFSRNF { get; set; }
        public int cmdvalor_total { get; set; }
        public int usucodigo { get; set; }

    }

    public class CabCupomNFCE
    {
        public int cabficha { get; set; }
        public int NFSRSERIE { get; set; }
        public int cbncupomfiscal { get; set; }
        public int cbntipoambiente { get; set; }
        public int cbntipoemissao { get; set; }
        public string cbnstatustransm { get; set; }
        public int cbncodstatus { get; set; }
        public string cbnchaveacesso { get; set; }
        public string qrcode { get; set; }
        public string cbnxml { get; set; }
        public string cbncpf_cnpj { get; set; }
        public string cbnstatus { get; set; }
        public int usucodigo { get; set; }
        public int chkcodigo { get; set; }
        public int empcodigo { get; set; }

    }
    public class CabCupom
    {
        public int cabficha { get; set; }
        public string cabcupomfiscal { get; set; }
        public int garcodigo { get; set; }
        public int regficha { get; set; }
        public string cabimpresso { get; set; }
        public string cabnmesa { get; set; }
        public int depcodigo { get; set; }
        public int usucodigo { get; set; }
        public decimal cabvalor { get; set; }
        public string cabaptnumero { get; set; }
        public int cabpgficha { get; set; }
        public int cabusupago { get; set; }
        public int cabcheckout { get; set; }
        public string cabtipo { get; set; }
        public string cabstatus { get; set; }
        public int loccodigo { get; set; }
        public int ecfcodigo { get; set; }
        public int cabcnpj_cpf { get; set; }
        public int empcodigo { get; set; }
        public string cabprevisaopagto { get; set; }
        public string cabobservacao { get; set; }

    }
    public class ProcessarNFCERequest
    {
        public int Empcodigo { get; set; }
    }

    public class PisCofinsCest
    {
        public string piscofins { get; set; }
        public string cest { get; set; }
    }
    public class CupomNFCERet
    {
        public string nfsrSerie { get; set; }
        public int nFSRNF { get; set; }
        public int nfsrcodigo { get; set; }
        public int CHKCODIGO { get; set; }
    }
    public class SefaRet
    {
        public string tipoemissao { get; set; }
        public string tipoambiente { get; set; }
        public string chaveDoc { get; set; }
        public string digestValue { get; set; }
        public string status { get; set; }
        public string motivo { get; set; }
        public string sefaXML { get; set; }
    }

}