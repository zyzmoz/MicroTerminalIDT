using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using FirebirdSql.Data.FirebirdClient;



namespace WindowsFormsApplication1
{
    public partial class frm_main : Form
    {
        public static ObjModem modem;
      
        public frm_main()
        {
            InitializeComponent();
            StreamReader file = new StreamReader("\\SGSUPER\\cartoes.txt");
            ed_numComandas.Text = file.ReadLine();
            file.Close();
            modem = new ObjModem();
            modem.OnConnect += new EventHandler(modem_OnConnect);
        }

        public void Terminal_OnReceivedChar(object sender, Char e)
        {
            if (InvokeRequired)
            {
                Invoke(new ObjTerminal.EventCharReceived(Terminal_OnReceivedChar), sender, e);
                return;
            }

            if (e.ToString() == "\u007f")
            {
                modem.Terminais[0].isOpSelected = false;
                modem.Terminais[0].isCrSelected = false;
                modem.Terminais[0].isProdutoSelected = false;
                modem.Terminais[0].isQtSelected = false;

                modem.Terminais[0].setCartao("");
                modem.Terminais[0].setProduto("");
                modem.Terminais[0].setQuantidade("0");
                modem.Terminais[0].setOperador("");
                modem.Terminais[0].ClearDisplay();
                modem.Terminais[0].Display("Opr.:");
                modem.Terminais[0].Cursor(2, 1);
                modem.Terminais[0].Display("Informe Operador!!!");
                modem.Terminais[0].Cursor(1, 6);
            }
            else
            {
                if (!modem.Terminais[0].isOpSelected)
                {
                    if ((e.ToString() != "\b") && (e.ToString() != "\r") && (e.ToString() != "\u007f") && (e.ToString() != ".") && (e.ToString() != ","))
                    {
                        modem.Terminais[0].Display(e.ToString());
                        modem.Terminais[0].setOperador(modem.Terminais[0].getOperador() + e.ToString());

                    }
                    if ((e.ToString() == "\b") && (modem.Terminais[0].getOperador() != ""))
                    {
                        modem.Terminais[0].setOperador(modem.Terminais[0].getOperador().Substring(0, modem.Terminais[0].getOperador().Length - 1));
                        modem.Terminais[0].Display(e.ToString());
                    }
                    if ((modem.Terminais[0].getOperador() != "") && (e.ToString() == "\r") && (modem.Terminais[0].getOperador() != "0"))
                    {
                        modem.Terminais[0].isOpSelected = true;
                        modem.Terminais[0].ClearDisplay();
                        modem.Terminais[0].Display("Cartao:");
                        modem.Terminais[0].Cursor(2, 1);
                        modem.Terminais[0].Display("Informe Cartao!!!");
                        modem.Terminais[0].Cursor(1, 8);
                    }
                }


                if ((modem.Terminais[0].isOpSelected) && (!modem.Terminais[0].isCrSelected))
                {


                    if ((e.ToString() != "\b") && (e.ToString() != "\r") && (e.ToString() != "\u007f") && (e.ToString() != ".") && (e.ToString() != ","))
                    {
                        modem.Terminais[0].Display(e.ToString());
                        modem.Terminais[0].setCartao(modem.Terminais[0].getCartao() + e.ToString());
                    }
                    if ((e.ToString() == "\b") && (modem.Terminais[0].getCartao() != ""))
                    {
                        modem.Terminais[0].setCartao(modem.Terminais[0].getCartao().Substring(0, modem.Terminais[0].getCartao().Length - 1));
                        modem.Terminais[0].Display(e.ToString());
                    }
                    if ((modem.Terminais[0].getCartao() != "") && (e.ToString() == "\r") && (modem.Terminais[0].getCartao() != "0"))
                    {
                        modem.Terminais[0].isCrSelected = true;
                        modem.Terminais[0].ClearDisplay();
                        modem.Terminais[0].Display("Cartao:" + modem.Terminais[0].getCartao() + " Op:");
                        modem.Terminais[0].Cursor(2, 1);
                        modem.Terminais[0].Display("Prod:");
                    }


                }
                else
                {
                    if (modem.Terminais[0].isQtSelected)
                    {
                        //modem.Terminais[0].isQtSelected = false;
                        if ((e.ToString() != "\b") && (e.ToString() != "\r") && (e.ToString() != "\u007f") && (e.ToString() != ".") && (e.ToString() != ","))
                        {
                            modem.Terminais[0].Display(e.ToString());
                            modem.Terminais[0].setProduto(modem.Terminais[0].getProduto() + e.ToString());
                        }
                        if ((e.ToString() == "\b") && (modem.Terminais[0].getProduto() != ""))
                        {
                            modem.Terminais[0].setProduto(modem.Terminais[0].getProduto().Substring(0, modem.Terminais[0].getProduto().Length - 1));
                            modem.Terminais[0].Display(e.ToString());
                        }
                        if ((e.ToString() == "\r") && (modem.Terminais[0].getProduto() != ""))
                            modem.Terminais[0].isQtSelected = false;
                    }
                }



                if ((modem.Terminais[0].isCrSelected) && (!modem.Terminais[0].isProdutoSelected) && (!modem.Terminais[0].isQtSelected))
                {

                    if ((e.ToString() != "\b") && (e.ToString() != "\r") && (e.ToString() != "\u007f") && (e.ToString() != ".") && (e.ToString() != ","))
                    {
                        modem.Terminais[0].Display(e.ToString());
                        modem.Terminais[0].setProduto(modem.Terminais[0].getProduto() + e.ToString());
                    }

                    if ((e.ToString() == "\b") && (modem.Terminais[0].getProduto() != ""))
                    {
                        modem.Terminais[0].setProduto(modem.Terminais[0].getProduto().Substring(0, modem.Terminais[0].getProduto().Length - 1));
                        modem.Terminais[0].Display(e.ToString());
                    }

                    if ((e.ToString() == "\r") && (modem.Terminais[0].getProduto() != ""))
                    {
                        modem.Terminais[0].isProdutoSelected = true;
                        modem.Terminais[0].ClearDisplay();
                        modem.Terminais[0].Display("Prod:" + "Produto");
                        modem.Terminais[0].Cursor(2, 1);
                        modem.Terminais[0].Display("Qtde:");
                    }
                }


                if ((modem.Terminais[0].isProdutoSelected) && (!modem.Terminais[0].isQtSelected))
                {
                    if ((e.ToString() != "\b") && (e.ToString() != "\r") && (e.ToString() != "\u007f") && (e.ToString() != "."))
                    {
                        if (!((modem.Terminais[0].getQuantidade().Contains(".")) || (modem.Terminais[0].getQuantidade().Contains(","))))
                        {
                            modem.Terminais[0].setQuantidade(modem.Terminais[0].getQuantidade() + e.ToString());
                            modem.Terminais[0].Display(e.ToString());
                        }
                        else
                        {
                            if ((e.ToString() != ",") && (e.ToString() != "."))
                            {
                                modem.Terminais[0].setQuantidade(modem.Terminais[0].getQuantidade() + e.ToString());
                                modem.Terminais[0].Display(e.ToString());
                            }
                        }

                    }

                    if ((e.ToString() == "\b") && (modem.Terminais[0].getQuantidade() != ""))
                    {
                        modem.Terminais[0].setProduto(modem.Terminais[0].getQuantidade().Substring(0, modem.Terminais[0].getQuantidade().Length - 1));
                        modem.Terminais[0].Display(e.ToString());
                    }



                    if ((e.ToString() == "\r") && (modem.Terminais[0].getQuantidade() != "") && (Convert.ToDouble(modem.Terminais[0].getQuantidade()) > 0))
                    {
                        ConexaoFB.AddItemComanda(modem.Terminais[0].getCartao(), modem.Terminais[0].getProduto(), modem.Terminais[0].getQuantidade(), modem.Terminais[0].getOperador());
                        modem.Terminais[0].isProdutoSelected = false;
                        modem.Terminais[0].isQtSelected = true;
                        modem.Terminais[0].setProduto("");
                        modem.Terminais[0].setQuantidade("0");
                        modem.Terminais[0].ClearDisplay();
                        modem.Terminais[0].Display("Cartao:" + modem.Terminais[0].getCartao() + " Op:");
                        modem.Terminais[0].Cursor(2, 1);
                        modem.Terminais[0].Display("Prod:");
                    }
                }



            }
        }

            

          

        protected void frmMain_OnReceivedCharSerial(object sender, string ReceivedChar)
        {
            MessageBox.Show(ReceivedChar);
        }



        public void modem_OnConnect(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(modem_OnConnect), sender, e);
                return;
            }
            
            modem.Terminais[0].OnReceivedChar += new ObjTerminal.EventCharReceived(Terminal_OnReceivedChar);
            modem.Terminais[0].OnReceivedCharSerial += new ObjTerminal.EventCharReceivedSerial(frmMain_OnReceivedCharSerial);
            //modem.Terminais[0].Display(DateTime.Now.ToShortDateString());
            modem.Terminais[0].setOperador("");
            modem.Terminais[0].Display("Opr.:");
            modem.Terminais[0].Cursor(2, 1);
            modem.Terminais[0].Display("Informe Operador!!!");
            modem.Terminais[0].Cursor(1, 6);
        }




        private void btn_Sair_Click(object sender, EventArgs e)
        {
            DialogResult dlg = MessageBox.Show("Deseja Realmente Fechar a Aplicação?", "Aviso!", MessageBoxButtons.YesNo);
            if (dlg == DialogResult.Yes)             
                System.Windows.Forms.Application.Exit();                  
            

        }

        private void btn_Minimizar_Click(object sender, EventArgs e)
        {
            this.Hide();
           

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                this.Hide();
                timer1.Enabled = false;                    
            }
        }

        private void frm_main_Load(object sender, EventArgs e)
        {
            ConexaoFB.AcertaBase();

            dataGridView1.DataSource = ConexaoFB.Load();
            
            modem.Start(6550, "192.168.0.199");

        }

        private void btn_Incluir_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Digite o IP do Terminal","Cadastro de Terminal","",-1,-1);
            if (input != "")
            {
                ConexaoFB.Add(input);
                dataGridView1.DataSource = ConexaoFB.Load();
            }
        }

        private void btn_Excluir_Click(object sender, EventArgs e)
        {
        
            string terminal = dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].Cells[0].Value.ToString();
            DialogResult dlg = MessageBox.Show("Deseja realmente excluir o terminal : " + terminal + " ?", "Aviso!", MessageBoxButtons.YesNo);

            if (dlg == DialogResult.Yes)
            {
                ConexaoFB.Remove(terminal);
                dataGridView1.DataSource = ConexaoFB.Load();
            }
        }

        private void btn_Reiniciar_Click(object sender, EventArgs e)
        {
            
        }
    }
}
