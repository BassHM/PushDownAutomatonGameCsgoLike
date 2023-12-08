using DesactivadorDeBombas.Clases;
using DesactivadorDeBombas.Properties;
using System.Diagnostics;
using System.Resources;
using System.Windows.Forms;

namespace DesactivadorDeBombas
{
    public partial class Form1 : Form
    {
        private AutomataPila automataPila;
        private bool bombaActivada = false;
        private int intentos = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            try
            {
                string ruta = Application.StartupPath;
                //Unir ruta con public y el nombre del archivo
                ruta = ruta + @"\Public\pointer.png";
                Bitmap bmp = new Bitmap(new Bitmap(ruta), 150, 150);
                this.Cursor = new Cursor(bmp.GetHicon());
            }
            catch (Exception ex)
            {

            }



        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {


        }
        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }
        //Crea una seccion de la bomba
        #region "Botones numericos de la bomba"
        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text += "3";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text += "2";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text += "5";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text += "6";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label1.Text += "7";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text += "1";
        }
        private void button8_Click(object sender, EventArgs e)
        {
            label1.Text += "8";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            label1.Text += "9";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text += "4";
        }
        private void button0_Click(object sender, EventArgs e)
        {
            label1.Text += "0";
        }
        #endregion

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonContinuar_Click(object sender, EventArgs e)
        {
            if (bombaActivada)
            {
                if (label1.Text.Length <= AutomataPila.MaximoDeCaracteresContrasenia)
                {
                    if (automataPila.CadenaAceptada(label1.Text + " "))
                    {
                        MessageBox.Show("La bomba se ha desactivado");
                        bombaActivada = false;
                        intentos = 0;
                        label1.Text = "";
                    }
                    else
                    {
                        intentos++;
                        if (intentos == 3)
                        {
                            MessageBox.Show("La bomba ha explotado");
                            bombaActivada = false;
                            intentos = 0;
                            label1.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("La bomba no se ha desactivado");
                            label1.Text = "";
                            automataPila.ReinicarAutomata();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("La contraseña es demasiado larga");
                    label1.Text = "";
                }
            }
            else
            {
                if (label1.Text.Length > AutomataPila.MaximoDeCaracteresContrasenia)
                {
                    MessageBox.Show("La contraseña es demasiado larga");
                }
                else
                {
                    bombaActivada = true;
                    List<int> semilla = new List<int> { };
                    string semillaString = label1.Text.ToString();
                    semillaString.Reverse();
                    foreach (char c in semillaString)
                    {
                        semilla.Add(int.Parse(c.ToString()));
                    }
                    automataPila = new AutomataPila(semilla);
                    automataPila.ImprimirTransiciones();
                    MessageBox.Show("La bomba se ha activado");
                    label1.Text = "";
                    //Obtener las transiciones del automata
                    label4.Text = automataPila.ObtenerTransicionesAntiterrorista();
                    label5.Text = automataPila.ObtenerPila();
                }
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if(label1.Text.Length > 0)
            {
                label1.Text = label1.Text.Substring(0, label1.Text.Length - 1);
            }
        }
    }
}
