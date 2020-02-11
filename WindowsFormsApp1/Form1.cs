using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool isConnected = false;
        SerialPort port;
        bool led = false;
        Thread read;
        string resul;

        void task()
        {
            while (isConnected) 
            {
                try
                {
                    resul = port.ReadLine();
                    textBox2.AppendText("Arduino ==>  " + resul + System.Environment.NewLine);
                }
                catch
                {

                }
            };
        }

        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                if (comboBox2.Text != null && comboBox1.SelectedItem != null)
                {
                    ConnectToArduino();
                    
                }
                else
                    MessageBox.Show("выберите порт и частоту");
            }
            else
            {
                disconnectFromArduino();
            }
        }
        public void ConnectToArduino()
        {

            int Hz = Convert.ToInt32(comboBox2.SelectedItem);
            isConnected = true;
            string selectedPort = comboBox1.GetItemText(comboBox1.SelectedItem);
            port = new SerialPort(selectedPort, Hz, Parity.None, 8, StopBits.One);
            port.Open();
            button1.Text = "Отключится";
            CheckForIllegalCrossThreadCalls = false;
            read = new Thread(() => task());
            read.IsBackground = false;
            read.Start();

        }
        public void disconnectFromArduino()
        {
            isConnected = false;
            port.Close();
            read.Join();
            button1.Text = "Подключится";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!isConnected)
                MessageBox.Show("Устройство не обнаружено");
            else
            {
                if (!led)
                {
                    port.Write("#w|");
                    button2.Text = "Включить LED";
                    led = true;
                }
                else
                {
                    port.Write("#x|");
                    button2.Text = "Выключить LED";
                    led = false;
                }
            }

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (!isConnected)
                MessageBox.Show("Устройство не обнаруженj");
            else
            {
                int Shag = Convert.ToInt32(trackBar2.Value);
                String U_Shaga = "#A";
                U_Shaga = U_Shaga + Convert.ToString(Shag, 16) + "|";
                port.Write(U_Shaga);
                label2.Text = String.Format("Скорость шага: {0}", Shag);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (!isConnected)
                MessageBox.Show("Устройство не обнаружено");
            else
            {
                int servo = Convert.ToInt32(trackBar1.Value);
                String UgolServo = "#s";
                UgolServo = UgolServo + Convert.ToString(servo, 16) + "|";
                port.Write(UgolServo);
                label1.Text = String.Format("Угол сервопривода: {0}", servo);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] portnames = SerialPort.GetPortNames();
            if (portnames.Length == 0)
            {
                MessageBox.Show("Устройство не обнаружено");
            }
            foreach (string s in portnames)
            {
                comboBox1.Items.Add(s);
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isConnected)
                MessageBox.Show("Устройство не обнаружено");
            else
            if (e.KeyCode == Keys.Enter)
            {
                textBox2.AppendText("USER ==>  " + textBox1.Text + System.Environment.NewLine);
                port.Write(textBox1.Text);
                textBox1.Clear();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            read.Abort();
            port.Close();
        }
    }
}
