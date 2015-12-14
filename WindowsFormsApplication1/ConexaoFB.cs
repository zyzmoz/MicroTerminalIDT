using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using System.Windows.Forms;
using System.Data;

namespace WindowsFormsApplication1
{
    class ConexaoFB
    {
        static FbConnection fbCnn;
        static FbCommandBuilder fbCmm = new FbCommandBuilder();
        //Getter and setters
        public static FbConnection FbCnn
        {
               get{return fbCnn;}
        }
        public static FbCommandBuilder FbCmm
        {
            get { return fbCmm; }
            
        }

        public static Boolean Active(Boolean bActive)
        {
            if (bActive == true)
            {
                string _conn;
                _conn = "User=SYSDBA;Password=masterkey;";
                _conn += "Database=C:\\SGSUPER\\DBCOMP.FDB;";
                _conn += "Port=3050;Dialect=3;Charset=NONE;Connection lifetime=0;";
                _conn += "Connection timeout=7;Pooling=True;Packet Size=8192;Server Type=0";

                fbCnn = new FbConnection(_conn);
                fbCnn.Open();                

                return true;
            }
            else
            {
                return false;
            }

        }

        public static void AddItemComanda(String cartao, String codigo, String Qtde, String Operador)
        {
            try
            {
                FbTransaction transaction = FbCnn.BeginTransaction();
                string SQL = "insert into VENDATERMINAL(CONTROLE, CARTAO, CODIGO, QTD, OPERADOR) values (null, " + cartao + ",'" + codigo + "', " + Qtde.Replace(",",".") + ", "+ Operador + " );";
                FbCommand cmd = new FbCommand(SQL, FbCnn, transaction);
                cmd.ExecuteNonQuery();
                transaction.Commit();

            }catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static void AcertaBase()
        {
          /*  try
                {
                FbTransaction transaction = fbCnn.BeginTransaction();
                FbCommand cmd = new FbCommand("create table terminaisIDT(IP varchar(14) primary key not null);", fbCnn, transaction);
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
                catch
                {
                //nono
            }*/
        }

        public static void Add(String Ip)
        {
            try
            {
                string _conn;
                _conn = "User=SYSDBA;Password=masterkey;";
                _conn += "Database=C:\\SGSUPER\\DBCOMP.FDB;";
                _conn += "Port=3050;Dialect=3;Charset=NONE;Connection lifetime=0;";
                _conn += "Connection timeout=7;Pooling=True;Packet Size=8192;Server Type=0";

                fbCnn = new FbConnection(_conn);
                fbCnn.Open();
                FbCommand readCommand = new FbCommand();
                readCommand.CommandText = "SELECT count(IP) FROM TERMINAISIDT where IP = '" + Ip + "'";
                readCommand.Connection = fbCnn;
                //FbDataReader reader = readCommand.ExecuteReader();
                int RecordCount  = Convert.ToInt32(readCommand.ExecuteScalar());

                if (RecordCount == 0)
                {
                    FbTransaction transaction = fbCnn.BeginTransaction();
                    string SQL = "insert into terminaisIDT values ('" + Ip + "');";

                    FbCommand cmd = new FbCommand(SQL, fbCnn, transaction);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                else
                {
                    MessageBox.Show("Terminal já cadastrado!");
                }
            }
            catch (Exception E)
            {
                MessageBox.Show("Erro ao inserir dados\n" + E.Message);
            }

        }

        public static void Remove(String Ip)
        {
            try
            {
                string _conn;
                _conn = "User=SYSDBA;Password=masterkey;";
                _conn += "Database=C:\\SGSUPER\\DBCOMP.FDB;";
                _conn += "Port=3050;Dialect=3;Charset=NONE;Connection lifetime=0;";
                _conn += "Connection timeout=7;Pooling=True;Packet Size=8192;Server Type=0";

                fbCnn = new FbConnection(_conn);
                fbCnn.Open();

                FbTransaction transaction = fbCnn.BeginTransaction();
                string SQL = "delete from terminaisIDT where ip = '" + Ip + "';";

                FbCommand cmd = new FbCommand(SQL, fbCnn, transaction);
                cmd.ExecuteNonQuery();
                transaction.Commit();
                MessageBox.Show("Terminal excluido com sucesso!");
            }
            catch (Exception E)
            {
                MessageBox.Show("Erro ao exclir dados\n" + E.Message);
            }

        }

        public static DataTable Load()
        {
            try
            {
                clTerminais tblTerminais = new clTerminais();
                DataTable dtaTable = new DataTable();
                FbDataReader fbReader;
                fbReader = tblTerminais.Open(ConexaoFB.FbCnn);
                dtaTable.Load(fbReader);
                return dtaTable;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return null;
            }
        }

       
    
        
    }

   

}
