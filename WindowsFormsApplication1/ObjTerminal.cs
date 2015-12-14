using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace WindowsFormsApplication1

{
    public class ObjTerminal
    {
        #region Members
        public Boolean isGetBalanca = false;
        private String stBalanca = "";
        public String stIpTerminal = "";
        private Socket socket;
        private Thread cliente;
        public BinaryWriter MyWriter;
        public BinaryReader MyReader;

        public delegate void EventCharReceived(Object sender, Char ReceivedChar);
        public delegate void EventCharReceivedSerial(Object sender, String ReceivedChar);
        public event EventHandler OnStop;
        public event EventCharReceived OnReceivedChar;
        public event EventCharReceivedSerial OnReceivedCharSerial;

        private String Operador = "";
        private Boolean OpSelected = false;

        private String Cartao = "";
        private Boolean CrSelected = false;

        private String Produto = "";
        private Boolean PrSelected = false;

        private String Quantidade = "0";
        private Boolean QtSelected = false;
        #endregion

        public ObjTerminal(String _stIP, Socket _socket)
        {
            socket = _socket;
            NetworkStream nws = new NetworkStream(_socket);
            MyReader = new BinaryReader(nws);
            MyWriter = new BinaryWriter(nws);
            stIpTerminal = _stIP;
            OnStop += new EventHandler(ObjTerminal_OnStop);
            cliente = new Thread(new ThreadStart(runOutPut));
            cliente.Start();
        }

        protected void ObjTerminal_OnStop(object sender, EventArgs e)
        {
            socket = null;
            MyReader = null;
            MyWriter = null;
            cliente = null;
            GC.Collect();
        }

        #region METHODS

        #region Operador
        public bool isOpSelected
        {
            get { return OpSelected; }
            set
            {
                OpSelected = value;
                if (!OpSelected) setOperador("");
            }

        }    
               

        public void setOperador(String e)
        {
            Operador = e;
        }

        public String getOperador()
        {
            return Operador;
        }
        #endregion

        #region Cartao
        public bool isCrSelected
        {
            get { return CrSelected; }
            set
            {
                CrSelected = value;
                if (!CrSelected) setCartao("");
            }
        }

        public void setCartao(String valor)
        {
            Cartao = valor;
        }

        public String getCartao()
        {
            return Cartao;
        }

        #endregion

        #region Produto
        
        public bool isProdutoSelected
        {
            get { return PrSelected; }
            set
            {
                PrSelected = value;
                if (!PrSelected) setProduto("");
            }
        }


        public void setProduto(String valor)
        {
            Produto = valor;            
        }

        public String getProduto()
        {
            return Produto;
        }        


        #endregion

        #region Quantidade

        public bool isQtSelected
        {
            get { return QtSelected; }
            set
            {
                QtSelected = value;
                if (!QtSelected) setQuantidade("");
            }
        }

        public void setQuantidade(String valor )
        {   
            Quantidade = valor;
           
        }

        public String getQuantidade()
        {
            return Quantidade;
        }

        #endregion

        public void ReConectar(Socket _socket)
        {
            lock (this)
            {
                try
                {
                    socket = null;
                    MyWriter = null;
                    MyReader = null;
                    GC.Collect();
                    socket = _socket;
                    NetworkStream nws = new NetworkStream(socket);
                    MyWriter = new BinaryWriter(nws);
                    MyReader = new BinaryReader(nws);
                    if (cliente == null)
                    {
                        cliente = new Thread(new ThreadStart(runOutPut));
                        cliente.Start();
                    }
                }
                catch { }
            }
        }
        public void ClearDisplay()
        {
            //Verifica se o micro terminal esta conectado
            try
            {
                String _Command = Convert.ToChar(27) + "[H" + Convert.ToChar(27) + "[J";                
                MyWriter.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(_Command));
            }
            catch { }
        }
        public void Display(String args)
        {
            if (args.Length == 0) return; //Verifica se existem dados a serem impressos no micro terminal            
            MyWriter.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(args + ""));
        }
        public void Cursor(Int32 linha, Int32 coluna)
        {
            try
            {
                String send = Convert.ToChar(27) + "[" + linha.ToString("00") + ";" + coluna.ToString("00") + "H";
                MyWriter.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(send));
            }
            catch { }
        }
        public void LeSerial(Char x)
        {
            if (char.IsDigit(x))
            {
                stBalanca += x.ToString();
            }
            else if (x == (char)27 || x == (char)2)
            {
                //FIM DA COMUNICAÇÃO
                isGetBalanca = false;
                if (OnReceivedCharSerial != null) OnReceivedCharSerial(this, stBalanca);
                stBalanca = "";
            }
        }
        public void HabilitaCom(Int32 nrCom)
        {
            if(nrCom == 0)
                MyWriter.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToChar(27) + "[?24h" + Convert.ToChar(27) + "[5i"));
            else MyWriter.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToChar(27) + "[?24r" + Convert.ToChar(27) + "[5i"));
        }
        public void DesabilitaSerial()
        {
            MyWriter.Write(Convert.ToChar(27) + "[4i");
        }
        public void runOutPut()
        {
            try
            {
                Object ret = "";
                do
                {
                    //Tenta ler o pacote
                    try
                    {
                        if (MyReader != null)
                        {
                            //Recebe um char do micro terminal
                            ret = MyReader.Read();
                            //MyFunctions.setLog("RECEBIDO " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + "COMANDO RECEBIDO (runReader): " + ret.ToString());
                            try
                            {
                                //Converte o byte em um char valido
                                char x = Convert.ToChar(ret);
                                if (!isGetBalanca)
                                {
                                    if (OnReceivedChar != null) OnReceivedChar(this, x);
                                }
                                else
                                {
                                    LeSerial(x);
                                }
                            }
                            catch { }
                        }
                    }
                    catch (IOException) { }

                } while (ret.ToString() != "#" && socket.Connected);
                cliente.Abort();
            }
            catch (ThreadAbortException)
            {
                if (OnStop != null) OnStop(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                cliente.Abort();//Sai da thread;
            }
        }
        #endregion
    }
}
