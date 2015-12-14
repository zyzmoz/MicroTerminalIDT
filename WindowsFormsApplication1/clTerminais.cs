using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace WindowsFormsApplication1
{
    class clTerminais
    {
        private String ip;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        public FbDataReader Open(FbConnection _conn)
        {
            FbCommand fbCmd = new FbCommand();
            fbCmd.Connection = _conn;
            fbCmd.CommandText = "SELECT * FROM TERMINAISIDT";
            return fbCmd.ExecuteReader();
        }
    }
}
