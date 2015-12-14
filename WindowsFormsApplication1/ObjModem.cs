using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public class ObjModem
    {
        #region Members
        public Int32 nrPorta = 6550;//PORTA DE COMUNICAÇÃO COM O MICROTERMINAL.
        public String AceptP = "192.168.1.100";//IP DO MICROTERMINAL.
        public ObjTerminal[] Terminais;//ARRAY DE OBJETOS MICROTERMINAIS.

        public event EventHandler OnConnect;//ENVENTO A SER DIPARADO QUANDO O OUVINTE ESCUTA UMA CONEXÃO.
        private Thread Server;//THREAD SERVER PARA ESCUTAR UMA CONEXÃO.
        #endregion

        /// <summary>
        /// CONSTRUTOR DO OBJETO MODEM (OUVINTE).
        /// </summary>
        public ObjModem()
        {
        }

        /// <summary>
        /// METHOD PARA INICIAR A THREAD OUVINTE.
        /// </summary>
        /// <param name="_nrPorta">PORTA DE CONEXÃO COM O MICROTERMINAL.</param>
        /// <param name="_AceptIP">ENDEREÇO IP DO MICROTERMINAL.</param>
        public void Start(Int32 _nrPorta, String _AceptIP)
        {
            nrPorta = _nrPorta;//ALTERA A PORTA PADRÃO PARA A PORTA DO ARGUMENTO.
            AceptP = _AceptIP;//ALTERA O IP PADRÃO PARA O IP DO ARGUMENTO.
            Server = new Thread(new ThreadStart(runServerModem));//INSTANCIA A THREAD SERVER(OUVINTE).
            Server.Start();//INICIA O OUVINTE.
        }

        /// <summary>
        /// METHODO OUVINTE (REFERENTE A THREAD SERVER).
        /// </summary>
        private void runServerModem()
        {
            //Tenta receber conexões na porta especificada
            try
            {
                TcpListener MyListener = new TcpListener(new IPAddress(0), nrPorta);//Cria um listener para escutar uma determinada porta.
                MyListener.Start();//Inicia o listener(ouvinte)
                
                //Mantem o objeto executando em um loop infinito (Até o fim da aplicação)
                while (true)
                {
                    try
                    {                        
                        Socket MySocket = MyListener.AcceptSocket();//Socket aguardando uma conexão...
                        
                        String stIp = MySocket.RemoteEndPoint.ToString().Split(":".ToCharArray())[0];//Recupera o IP e a Porta do socket.

                        if (stIp.Length == 0)
                        {
                            MySocket = null;//Destroi o objeto socket.
                            GC.Collect();//Força o usuo do Garbage Collect (Coletor de Lixo) do Windows.
                        }

                        //Verifica se a coleção de terminais existe.
                        if(Terminais == null)Terminais = new ObjTerminal[1];
                        
                        //Verifica se o objeto terminal já existe na coleção.
                        //Caso exista significa que o terminal esta desconectado, 
                        //e devemos reconecta-lo.
                        if (Terminais[0] != null && Terminais[0].stIpTerminal == stIp)
                        {                            
                            Terminais[0].ReConectar(MySocket);//RECONECTA O TERMINAL.
                            if (OnConnect != null) OnConnect(this, EventArgs.Empty);//Dispara o o evento de Conexão para o form
                            continue;
                        }

                        //Verifica se o terminal que está solicitando uma conexão esta devidamente autorizado.
                        ObjTerminal tmpTerminal = new ObjTerminal(stIp, MySocket);
                        if(stIp != AceptP)//Verifica se o terminal 
                        {
                            tmpTerminal.Display("TERMINAL SEM AUTORIZACAO");
                            tmpTerminal = null;
                            MySocket = null;
                            GC.Collect();
                            continue;
                        }

                        Terminais[0] = tmpTerminal;
                        if (OnConnect != null) OnConnect(this, EventArgs.Empty);//Dispara o o evento de Conexão para o form
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Falha na conexão TCP-IP:\n" + ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                //O SISTEMA APENAS EXECUTARÁ O ENCERRAMENTO DO MICRO TERMINAL NO CASO
                //DO SISTEMA JA ESTAR SENDO EXECUTADO.... NÃO MUDAR ISSO
                //20/04/2007
                MessageBox.Show("Falha na conexão TCP-IP:\nSistema em uso:\n" + ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
