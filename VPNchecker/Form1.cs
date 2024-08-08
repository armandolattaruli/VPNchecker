using System;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using VPNchecker.Properties;
using Microsoft.Win32; // Aggiungi questo riferimento

namespace VPNChecker
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer;
        private PictureBox pictureBox;
        private const string appName = "VPNChecker";

        public Form1()
        {
            InitializeComponent();
            this.Icon = Resources.danger_ico;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 300); // Imposta una dimensione fissa per il Form
            this.TopMost = true;
            this.ShowInTaskbar = false;

            // Imposta uno sfondo solido
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;

            pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.Image = Resources.danger; // Usa l'immagine aggiunta come risorsa
            this.Controls.Add(pictureBox);
            CenterPictureBox();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000; // Controlla ogni 5 secondi
            timer.Tick += Timer_Tick;
            timer.Start();

            // Esegui il controllo iniziale della connessione VPN
            CheckVpnStatus();

            // Aggiungi l'applicazione all'avvio del sistema
            AddToStartup();
        }

        private void AddToStartup()
        {
            string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (key.GetValue(appName) == null)
                {
                    key.SetValue(appName, Application.ExecutablePath);
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Nascondi il form all'inizio
            this.Hide();
            // Esegui il controllo iniziale della connessione VPN e mostra il form solo se la VPN non è connessa
            if (!IsVpnConnected())
            {
                this.Show();
                CenterPictureBox();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CheckVpnStatus();
        }

        private void CheckVpnStatus()
        {
            if (IsVpnConnected())
            {
                this.Hide();
            }
            else
            {
                this.Show();
                CenterPictureBox();
            }
        }

        private bool IsVpnConnected()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.Description.ToLower().Contains("vpn") || ni.Name.ToLower().Contains("vpn")))
                {
                    return true;
                }
            }
            return false;
        }

        private void CenterPictureBox()
        {
            pictureBox.Left = (this.ClientSize.Width - pictureBox.Width) / 2;
            pictureBox.Top = (this.ClientSize.Height - pictureBox.Height) / 2;
        }
    }
}
