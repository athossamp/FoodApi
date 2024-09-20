using log_food_api.Models;
using System.Runtime.InteropServices;

namespace log_food_api.Repositories
{
    public class ConexaoRepositorio
    {
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(ref NETSOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        public bool ConnectToNetworkPath (ConexaoPC credentials)
        {
            NETSOURCE netResource = new NETSOURCE
            {
                dwType = 1,
                lpRemoteName = credentials.NetworkPath,
                lpProvider = null
            };
            int result = WNetAddConnection2(ref netResource, credentials.Password, credentials.Username, 0);
            return result == 0;
        }
        public void DisconnectFromNetworkPath(string networkPath)
        {
            WNetCancelConnection2(networkPath, 0, true);
        }
    }
}
