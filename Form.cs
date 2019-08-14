using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AculixInject
{
    public partial class Form1 : Form
    {
        private string dllToInject = "";
        private const string DllFilePath = "aculix-inject.dll";

        [DllImport(DllFilePath, CallingConvention = CallingConvention.Cdecl)]
        private extern static int inject(int procID, string dllpath);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.Cdecl)]
        private extern static void ShowMe();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetRunningApps();
            if (!File.Exists(DllFilePath))
            {
                MessageBox.Show("aculix-inject.dll is missing.", "ERROR");
                Environment.Exit(0);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if(dllToInject != "")
            {
                if(procListBox.SelectedItem != null)
                {
                    if (File.Exists("aculix-inject.dll"))
                    {
                        int id = Convert.ToInt32((procListBox.SelectedItem as ComboboxItem).Value);
                        if (id != 0)
                        {
                            new Thread(() =>
                            {
                                Thread.CurrentThread.IsBackground = true;
                                /* run your code here */
                                if (inject(id, dllToInject) != 1)
                                {
                                    MessageBox.Show("Injection failed!", "ERROR");
                                }
                            }).Start();
                        }
                    }
                    else
                    {
                        MessageBox.Show("aculix-inject.dll is missing.", "ERROR");
                    }
                }
                else
                {
                    MessageBox.Show("No Process selected!", "ERROR");
                }
                
            }
            else
            {
                MessageBox.Show("No DLL to inject is selected!", "ERROR");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open DLL File";
            theDialog.Filter = "DLL files|*.dll";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                dllToInject = theDialog.FileName;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ShowMe();
        }

        void GetRunningApps()
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        ComboboxItem combobox = new ComboboxItem();
                        combobox.Text = p.MainWindowTitle;
                        combobox.Value = p.Id;
                        procListBox.Items.Add(combobox);
                    }
                }
                catch { }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            procListBox.Items.Clear();
            GetRunningApps();
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
