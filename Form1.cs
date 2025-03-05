using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Security.Principal;

namespace RastreadorDeProcesos
{
    public partial class Form1 : Form
    {
        private string selectedExePath; // Ruta del ejecutable seleccionado
        private string processName;     // Nombre del proceso sin extensión
        private DateTime startTime;     // Hora de inicio del proceso
        private Timer timer;            // Temporizador para actualizar la interfaz
        private bool isProcessRunning = false; // Estado del proceso
        private readonly string configFilePath; // Ruta del archivo de configuración
        private readonly string logFilePath;    // Ruta del archivo de log
        private NotifyIcon notifyIcon;          // Icono en la bandeja del sistema

        public Form1()
        {
            // Verificar si se ejecuta como administrador
            if (!IsRunningAsAdmin())
            {
                RestartAsAdmin();
                return;
            }

            InitializeComponent();

            // Configurar el formulario
            this.ShowInTaskbar = false; // No aparecer en la barra de tareas
            this.WindowState = FormWindowState.Minimized;

            // Configurar bandeja del sistema
            SetupNotifyIcon();

            // Definir rutas de archivos una vez
            configFilePath = Path.Combine(Application.StartupPath, "config.txt");
            logFilePath = Path.Combine(Application.StartupPath, "log.txt");

            // Cargar configuración
            LoadConfig();

            // Configurar temporizador
            timer = new Timer { Interval = 1000 }; // 1 segundo
            timer.Tick += Timer_Tick;
            timer.Start();

            SetStartup();
        }

        private void SetupNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application, // Icono por defecto, puedes personalizarlo
                Text = "ProcessTracker",
                Visible = true
            };
            notifyIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Abrir", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            menu.Items.Add("Salir", null, (s, e) => Application.Exit());
            notifyIcon.ContextMenuStrip = menu;

            this.FormClosing += (s, e) =>
            {
                notifyIcon.Visible = false; // Limpiar al cerrar
                notifyIcon.Dispose();
            };
        }

        private bool IsRunningAsAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                Verb = "runas", // Solicitar elevación
                UseShellExecute = true
            };
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                MessageBox.Show("Se requieren privilegios de administrador para ejecutar esta aplicación.");
            }
            Application.Exit();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    selectedExePath = File.ReadAllText(configFilePath)?.Trim();
                    if (!string.IsNullOrEmpty(selectedExePath))
                    {
                        processName = Path.GetFileNameWithoutExtension(selectedExePath);
                        UpdateProcessInfo();
                        CheckProcessInitialState();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo de configuración: {ex.Message}");
            }
        }

        private void UpdateProcessInfo()
        {
            this.Text = $"ProcessTracker - {Path.GetFileName(selectedExePath)}";
            lblProcessName.Text = $"Proceso: {processName}";
            notifyIcon.Text = $"ProcessTracker - {processName}";
        }

        private void CheckProcessInitialState()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    startTime = processes[0].StartTime; // Usar hora de inicio real del proceso
                    lblStartTime.Text = $"Hora de Inicio: {startTime:HH:mm:ss}";
                    isProcessRunning = true;
                }
                else
                {
                    lblStartTime.Text = "Hora de Inicio: No iniciada";
                    isProcessRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el proceso inicial: {ex.Message}");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lblLocalTime.Text = $"Hora Local: {DateTime.Now:HH:mm:ss}";

            if (isProcessRunning)
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                lblElapsedTime.Text = $"Tiempo Transcurrido: {(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
            }
            else
            {
                lblElapsedTime.Text = "Tiempo Transcurrido: 00:00:00";
            }

            CheckProcess();
        }

        private void CheckProcess()
        {
            if (string.IsNullOrEmpty(processName)) return;

            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0 && !isProcessRunning)
                {
                    // Proceso comenzó
                    isProcessRunning = true;
                    startTime = processes[0].StartTime; // Usar hora de inicio real del proceso
                    lblStartTime.Text = $"Hora de Inicio: {startTime:HH:mm:ss}";
                }
                else if (processes.Length == 0 && isProcessRunning)
                {
                    // Proceso terminó
                    isProcessRunning = false;
                    DateTime endTime = DateTime.Now;
                    lblStartTime.Text = "Hora de Inicio: No iniciada";

                    TimeSpan elapsed = endTime - startTime;
                    string totalTimeFormatted = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}";
                    string logMessage = $"Proceso: {processName} | Inicio: {startTime:HH:mm} | Fin: {endTime:HH:mm} | Duración: {totalTimeFormatted}";

                    try
                    {
                        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al escribir en el log: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el proceso: {ex.Message}");
            }
        }

        private void SetStartup()
        {
            try
            {
                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    rk?.SetValue("ProcessTracker", Application.ExecutablePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar el inicio automático: {ex.Message}");
            }
        }

        private void btnSelectExe_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos ejecutables (*.exe)|*.exe";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    selectedExePath = openFileDialog1.FileName;
                    processName = Path.GetFileNameWithoutExtension(selectedExePath);
                    File.WriteAllText(configFilePath, selectedExePath);
                    UpdateProcessInfo();
                    CheckProcessInitialState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al seleccionar el ejecutable: {ex.Message}");
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide(); // Ocultar formulario al minimizar
            }
        }
    }
}