using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ParentServer
{
    public partial class ParentForm : Form
    {
        #region DATA
        private const string PythonEngine =  @"C:\Python27\python.exe";
        private const string PythonStartFile = @"Python\PythonServer.py";
        private PythonHandler pythonListener;
        private Process pythonEngineProcess;
        #endregion

        public ParentForm()
        {
            InitializeComponent();
            comboBoxActions.SelectedIndex = 0;
            DataBaseHandler.sqlQuery = "Select * From Words";
            dataGridView1.DataSource = DataBaseHandler.ExecuteDataTable();
            StartPythonEngine();
        }

        #region  INIT
       
        /// <summary>
        /// 
        /// </summary>
        private void StartPythonEngine()
        {
            #region ----------   TRY-CATCH   ----------
            try
            {
                
                // Start python engine
                pythonEngineProcess = new Process();
                pythonEngineProcess.StartInfo.FileName = PythonEngine;
                pythonEngineProcess.StartInfo.Arguments = PythonStartFile + " " + Properties.Settings.Default.ClientPort;
                pythonEngineProcess.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
           //     pythonEngineProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pythonEngineProcess.Start();
                pythonListener = new PythonHandler(this);                
            }
            catch (Win32Exception e)
            {
                EndProcess(e);
            }
            catch (ObjectDisposedException e)
            {
                EndProcess(e);
            }
            catch (InvalidOperationException e)
            {
                EndProcess(e);
            }
            catch (Exception e)
            {
                EndProcess(e);
            }
            #endregion
        }

        private void EndProcess(Exception e)
        {
            PrintToLog(e.Message, Color.Red);
            if (pythonEngineProcess != null)
            {
                pythonEngineProcess.Dispose();
                pythonEngineProcess = null;
            }
            StopPythonEngine();
        }

        public void StopPythonEngine()
        {
            #region ----------   TRY-CATCH   ----------
            try
            {
                if (pythonEngineProcess != null)
                {
                    pythonEngineProcess.Kill();
                    pythonEngineProcess.Dispose();
                    pythonEngineProcess = null;
                }
                if (pythonListener != null)
                {
                    pythonListener.Close();
                    pythonListener = null;
                }
            }
            catch (Win32Exception e)
            {
                PrintToLog(e.Message, Color.Red);
                pythonEngineProcess = null;
                pythonListener = null;
            }
            catch (InvalidOperationException e)
            {
                PrintToLog(e.Message, Color.Red);
                pythonEngineProcess = null;
                pythonListener = null;
            }
            catch (Exception e)
            {
                PrintToLog(e.Message, Color.Red);
                pythonEngineProcess = null;
                pythonListener = null;
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// This method prints log to GUI
        /// </summary>
        /// <param name="msg">message for print</param>
        /// <param name="color">color in view</param>
        public void PrintToLog(string msg, Color color)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    richTextBoxLog.SelectionColor = color;
                    this.richTextBoxLog.AppendText(msg + Environment.NewLine);
                    this.richTextBoxLog.ScrollToCaret();
                });
            }
            else
            {
                richTextBoxLog.SelectionColor = color;
                this.richTextBoxLog.AppendText(msg + Environment.NewLine);
                this.richTextBoxLog.ScrollToCaret();
            }
        }

        public void AddNewChild(string childname, string ip)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string [] fields = { (listViewChilds.Items.Count+1).ToString(), childname, ip };
                    listViewChilds.Items.Add( new ListViewItem(fields));
                });
            }
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopPythonEngine();
        }

      
        private void buttonSend_Click(object sender, EventArgs e)
        {
            pythonListener.Send("127.0.0.1"  + "#" + comboBoxActions.SelectedItem);
        }
    }
}
