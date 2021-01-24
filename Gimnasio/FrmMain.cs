﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO.Ports;
using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets;
using Gimnasio.Socios;
using Gimnasio.Utilidades;

namespace Gimnasio
{
    public partial class FrmMain : Form
    {
        Registro.frmRegistro frmMNFC;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public FrmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            //mensaje inicio, puedes quitar esta linea, pero este software lo hizo Héctor de León Guevara | www.hdeleon.net
            try
            {
                if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/hdeleon.txt"))
                {
                    FrmMensajeInicial frmMensaje = new FrmMensajeInicial();
                    frmMensaje.ShowDialog();

                    System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "/hdeleon.txt");
                }
            }
            catch { }

            sinSesion();

        }
        /// <summary>
        /// metodo que abre la ventana de login
        /// </summary>
        private void sinSesion()
        {
            panel1.Enabled = false;
            cerrarFormuarios();

            Sesion.frmLogin frmL = new Sesion.frmLogin();
            frmL.ShowDialog();

            if (Utilidades.clsUsuario.existeSesion)
                panel1.Enabled = true;

            Thread thdUDPServer = new Thread(new ThreadStart(serverThread));
            thdUDPServer.Start();
        }

        /// <summary>
        /// UpdConnection
        /// </summary>
        private void serverThread()
        {
            //UPD Service Configuration
            int listenPort = 11000;
            UdpClient listener = new UdpClient(listenPort);

            try
            {
                while (true)
                {
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] bytes = listener.Receive(ref RemoteIpEndPoint);
                    string idUsuario = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        //Variables de comprobación
                        bool comprobarSocio = false;

                        clsSistemaApertura torno = new clsSistemaApertura();
                        comprobarSocio = torno.comprobarSocio(idUsuario);

                        if (comprobarSocio == true)
                        {
                            if (!detectarFormularioAbierto("frmRegistro"))
                            {
                                frmMNFC = new Registro.frmRegistro("RegistroNFC", idUsuario);
                                frmMNFC.ShowDialog();
                                frmMNFC.WindowState = FormWindowState.Normal;
                            }
                            torno.abrir();
                            //System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                            //timer.Tick += new EventHandler(timerEvent);
                            //timer.Interval = 5000;
                            //timer.Start();
                        }
                        else
                        {
                            if (!detectarFormularioAbierto("frmRegistro"))
                            {
                                frmMNFC = new Registro.frmRegistro("RegistroNFC", idUsuario);
                                frmMNFC.Show();
                                frmMNFC.WindowState = FormWindowState.Normal;
                            }
                            
                            //timer.Tick += new EventHandler(timerEvent);
                            //timer.Interval = 5000;
                            //timer.Start();
                        }
                       
                        //Código para escribir en el textbox los datos del usuario
                        //listBox_received.Items.Add(RemoteIpEndPoint.Address.ToString() + ": Membresia(" + membresia + "), Vencimiento(" + fechaVencimiento + ")");
                        //listBox_received.SelectedIndex = listBox_received.Items.Count - 1;
                        //listBox_received.SelectedIndex = -1;
                    }));
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmUsuarios"))
            {
                Usuarios.frmUsuarios frmU = new Usuarios.frmUsuarios();
                frmU.MdiParent = this;
                frmU.Show();
                frmU.WindowState = FormWindowState.Maximized;
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmMembresias"))
            {
                Membresias.frmMembresias frmM = new Membresias.frmMembresias();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        #region METODOS PRIVADOS

        private void timerEvent(Object myObject, EventArgs myEventArgs)
        {
            frmMNFC.Close();
            timer.Stop();
        }

        private bool detectarFormularioAbierto(string formulario)
        {
            bool abierto = false;

            if (Application.OpenForms[formulario] != null)
            {
                abierto = true;
                Application.OpenForms[formulario].Activate();
                Application.OpenForms[formulario].WindowState = FormWindowState.Maximized;
            }
            return abierto;
        }

        private void cerrarFormuarios()
        {
            if (this.MdiChildren.Length > 0)
            {
                foreach (Form childForm in this.MdiChildren)
                    childForm.Close();


            }
        }

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmProductos"))
            {
                Productos.frmProductos frmM = new Productos.frmProductos();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmSocios"))
            {
                Socios.frmSocios frmM = new Socios.frmSocios();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmRegistro"))
            {
                Registro.frmRegistro frmM = new Registro.frmRegistro();
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            //SE ABRE REGISTRO CON F12
            if (e.KeyCode == Keys.F12)
            {
                if (!detectarFormularioAbierto("frmRegistro"))
                {
                    Registro.frmRegistro frmM = new Registro.frmRegistro();
                    frmM.Show();
                    frmM.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void FrmMain_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmReportes"))
            {
                Reportes.frmReportes frmM = new Reportes.frmReportes();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Utilidades.clsUsuario.salir())
            {
                sinSesion();
            }
            else
            {
                MessageBox.Show(Utilidades.clsUsuario.error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmConfiguracion"))
            {
                Configuracion.frmConfiguracion frmM = new Configuracion.frmConfiguracion();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void iniciarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Utilidades.clsUsuario.existeSesion)
                sinSesion();
            else
                MessageBox.Show("Cierra la sesión primero");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmEntradas"))
            {
                Entradas.frmEntradas frmM = new Entradas.frmEntradas();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!detectarFormularioAbierto("frmSalidas"))
            {
                Salidas.frmSalidas frmM = new Salidas.frmSalidas();
                frmM.MdiParent = this;
                frmM.Show();
                frmM.WindowState = FormWindowState.Maximized;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                #region TCP
                //string requestUrl = "http://192.168.0.36/";
                //WebRequest request = HttpWebRequest.Create(requestUrl);
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //string postData = "myparam1=myvalue1&myparam2=myvalue2";
                //using (var writer = new StreamWriter(request.GetRequestStream()))
                //{
                //    writer.Write(postData);
                //}
                //string responseFromRemoteServer;
                //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                //{
                //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                //    {
                //        responseFromRemoteServer = reader.ReadToEnd();
                //    }
                //}
                #endregion

                #region UPD
                IPAddress ip = new IPAddress(Convert.ToByte("192.168.0.102"));
                string port = "8080";
                string mensaje = "LFT";

                UdpClient udpClient = new UdpClient();
                udpClient.Connect(ip, Convert.ToInt16(port));
                Byte[] senddata = Encoding.ASCII.GetBytes(mensaje);
                udpClient.Send(senddata, senddata.Length);

                MessageBox.Show("Puerta Abierta");
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {


            SaveFileDialog sFileDialog1 = new SaveFileDialog();

            sFileDialog1.InitialDirectory = "c:\\";
            sFileDialog1.Filter = "Archivos de sql (*.sql)|*.sql";
            sFileDialog1.FilterIndex = 1;
            sFileDialog1.RestoreDirectory = true;

            if (sFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //se guarda el respaldo
                    try
                    {
                        string constr = "server=localhost;User Id=root;Persist Security Info=True;database=gym";
                        string file = sFileDialog1.FileName;
                        MySqlBackup mb = new MySqlBackup(constr);
                        mb.ExportInfo.FileName = file;
                        mb.ExportInfo.ExportViews = false;

                        if (System.IO.File.Exists(file))
                            System.IO.File.Delete(file);
                        mb.Export();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocurrio un error " + ex.Message);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error del Sistema: " + ex.Message);
                }
            }

            MessageBox.Show("Información guardada con exito");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Estas seguro de abrir un archivo de respaldo? se remplazara toda la información en el sistema con lo que existe en el archivo de respaldo", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //se abre la ventana para seleccionar acrhivo

                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "Archivos de sql (*.sql)|*.sql";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //se abre el respaldo
                        try
                        {
                            string constr = "server=localhost;User Id=root;Persist Security Info=True;database=gym";
                            string file = openFileDialog1.FileName;
                            MySqlBackup mb = new MySqlBackup(constr);
                            mb.ImportInfo.FileName = file;
                            mb.ImportInfo.SetTargetDatabase("gym", ImportInformations.CharSet.utf8);
                            mb.Import();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ocurrio un error " + ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error del Sistema: " + ex.Message);
                    }
                }

                MessageBox.Show("Información restaurada con exito");

                //se cierran formularios
                cerrarFormuarios();
                //se cierra sesion
                if (Utilidades.clsUsuario.salir())
                {
                    sinSesion();
                }
                else
                {
                    MessageBox.Show(Utilidades.clsUsuario.error);
                }

            }
        }

        private void creadoPorHéctorDeLeónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCreador oFrm = new frmCreador();
            oFrm.ShowDialog();

        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
