using System;
using System.Windows.Forms;
using System.IO;
using WMPLib;
using System.Linq;

namespace ToxinPlayer
{
    public partial class Form1 : Form
    {
        private KeyHandler ghk;

        private readonly WindowsMediaPlayer player;

        const string config = "cfg.txt";

        const string hotkey = "Hotkey=";

        public Form1()
        {
            InitializeComponent();
            player = new WindowsMediaPlayer();
            ghk = new KeyHandler(Keys.Oemtilde, this);
            ghk.Register();

            player.URL = "\\Music\\Toxin.mp3";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;

            player.controls.stop();

            if (File.Exists(config))
            {
                string[] lines = File.ReadAllLines(config);

                foreach(string line in lines)
                {
                    if(line.Contains(hotkey))
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;

                        string tempLine = line.Remove(0, hotkey.Length);
                        int temp = (int)Keys.Oemtilde;

                        int.TryParse(tempLine, out temp);

                        ghk.Unregiser();
                        ghk = new KeyHandler((Keys)temp, this);
                        ghk.Register();
                    }
                }
            }
            //////////////////////////////////////

            string path = Path.GetDirectoryName(Application.StartupPath);

            string[] files = Directory.GetFiles(path + "\\Music\\", "*.mp3");

            for(int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Remove(0, path.Length + 7);
            }

            comboBox1.Items.AddRange(files);

            comboBox1.SelectedIndex = 0;

            textBox1.Text = ((Keys)ghk.key).ToString();
        }

        private void HandleHotkey()
        {
            if (player.playState == WMPPlayState.wmppsPlaying)
                player.controls.stop();
            else
                player.controls.play();
            // Do stuff...
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
                HandleHotkey();
            base.WndProc(ref m);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.URL = "Music\\" + comboBox1.Items[comboBox1.SelectedIndex].ToString(); 
            player.controls.stop();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            ghk.Unregiser();
            ghk = new KeyHandler(e.KeyCode, this);
            ghk.Register();
            timer1.Stop();
            timer1.Interval = 1;
            timer1.Start();

            WriteToConfig();
            label1.Focus();
        }

        private void WriteToConfig()
        {
            if (!File.Exists(config))
            {
                File.Create(config).Close();

                StreamWriter sw = File.AppendText(config);
                sw.WriteLine(hotkey + ghk.key);
                sw.Close();
            }
            else
            {
                File.WriteAllText(config, string.Empty);

                StreamWriter sw = File.AppendText(config);
                sw.WriteLine(hotkey + ghk.key);
                sw.Close();
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            WriteToConfig();

            ghk.Unregiser();
            ghk = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
            textBox1.Text = ((Keys)ghk.key).ToString();
        }
    }
}
