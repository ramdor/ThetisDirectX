// based on code from https://github.com/TAPR/OpenHPSDR-Thetis
// same license conditions
// https://github.com/TAPR/OpenHPSDR-Thetis/blob/master/Project%20Files/GNU_GENERAL_PUBLIC_LICENSE.rtf
// MW0LGE - 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace ThetisDirectX
{
    public partial class frmMain : Form
    {
        private bool m_bDisplayLoopRunning = false;
        private Thread draw_display_thread;
        private ThreadPriority m_tpDisplayThreadPriority = ThreadPriority.Normal;
        public bool m_bPause_DisplayThread = true;
        private bool m_bResizeDX2Display = false;
        public const int MAX_FPS = 1000;

        public frmMain()
        {
            InitializeComponent();
            
            this.Text += this.is64Bit ? " - x64" : " - x86";

            Win32.TimeBeginPeriod(1);

            m_bPause_DisplayThread = true;

            if (draw_display_thread == null || !draw_display_thread.IsAlive)
            {
                draw_display_thread = new Thread(new ThreadStart(RunDisplay))
                {
                    Name = "Draw Display Thread",
                    Priority = m_tpDisplayThreadPriority,
                    IsBackground = false
                };
                draw_display_thread.Start();
            }

            updateRes();
            radioButton1.Select();

            DisplayFPS = 60;
            UseAccurateFramingTiming = false;

            Display.Target = picDisplay;

            Display.VerticalBlanks = 0;
            Display.ShowFPS = true;
            Display.AntiAlias = false;
            Display.ShowFrameRateIssue = true;

            string[] adaptors = Display.DX2Adaptors();

            if (adaptors.Length > 0)
            {
                cmbAdaptors.Items.AddRange(adaptors);
                if (cmbAdaptors.Items.Count > 0)
                    cmbAdaptors.SelectedIndex = 0;
            }
            else
            {
                cmbAdaptors.Hide();
            }

            chkRun.Checked = false;
        }

        private delegate void addTextDelgate(string t);
        private void addText(string t)
        {
            if (!this.InvokeRequired)
            {
                updateText(t);
            }
            else
            {
                addTextDelgate del = new addTextDelgate(updateText);
                Invoke(del, new object[] { t });
            }
        }

        private delegate void shutdownDelgate();
        private void doShutDown()
        {
            if (!this.InvokeRequired)
            {
                shutdown();
            }
            else
            {
                shutdownDelgate del = new shutdownDelgate(shutdown);
                Invoke(del);
            }
        }
        private void shutdown()
        {
            Display.ShutdownDX2D();
            Win32.TimeEndPeriod(1);
            this.Close();
        }

        private void updateText(string t)
        {
            string[] lines = txtResults.Lines;

            string[] newLines;
            if (lines.Length + 1 > 24)
            {
                newLines = new string[24];
                for (int i = 0; i < newLines.Length - 1; i++)
                {
                    newLines[i] = lines[i + 1];
                }
            }
            else
            {
                newLines = new string[lines.Length + 1];
                for (int i = 0; i < newLines.Length - 1; i++)
                {
                    newLines[i] = lines[i];
                }
            }

            newLines[newLines.Length - 1] = t;

            txtResults.Lines = newLines;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        HiPerfTimer m_objLines = new HiPerfTimer();
        unsafe private void RunDisplay()
        {
            m_bDisplayLoopRunning = true;

            m_objLines.Reset();

            HiPerfTimer objStopWatch = new HiPerfTimer();
            double fFractionOfMs = 0;
            double fThreadSleepLate = 0;

            while (m_bDisplayLoopRunning)
            {
                objStopWatch.Reset();

                if (m_bResizeDX2Display)
                {
                    Display.Target = picDisplay;
                    Display.ResizeDX2D();
                    m_bResizeDX2Display = false;
                }

                if (!m_bPause_DisplayThread)
                {

                    int nLines = Display.RenderDX2D();

                    m_nLineCount += nLines;
                    if (m_nLineCount >= m_nLineTotal)
                    {
                        // made the grade
                        m_nLineCount = 0;

                        addText(m_nLineTotal.ToString() + " lines in : " + m_objLines.Elapsed.ToString());

                        m_objLines.Reset();
                    }
                }

                //MW0LGE consider how long all the above took (reset at start of loop), and remove any inaccuarcy from Thread.Sleep below
                double dly = display_delay - objStopWatch.ElapsedMsec - fThreadSleepLate;

                if (dly < 0)
                {
                    if (dly <= -1) Display.FrameRateIssue = true;
                    dly = 0;
                    fFractionOfMs = 0;
                }
                else
                {
                    Display.FrameRateIssue = false;
                }

                if (m_bUseAccurateFrameTiming)
                {
                    // wait for the calculated delay
                    objStopWatch.Reset();
                    while (objStopWatch.ElapsedMsec <= dly)
                    {
                        //Thread.Sleep(0);  // hmmm
                    }
                    fThreadSleepLate = objStopWatch.ElapsedMsec - dly;
                }
                else
                {
                    // accumulate the fractional delay
                    fFractionOfMs += dly - (int)dly;
                    int nIntegerPart = (int)fFractionOfMs;
                    fFractionOfMs -= nIntegerPart;

                    int nWantToWait = (int)dly + nIntegerPart;
                    fThreadSleepLate = 0;

                    if (nWantToWait > 0)
                    {
                        // time how long we actually sleep for, and use this difference to lower dly time next time around
                        objStopWatch.Reset();
                        Thread.Sleep(nWantToWait); // not guaranteed to be the delay we want, but it will be AT LEAST what we want
                        fThreadSleepLate = objStopWatch.ElapsedMsec - nWantToWait;
                    }
                    else if (fFractionOfMs > 0)
                    {
                        objStopWatch.Reset();
                        while (objStopWatch.ElapsedMsec <= fFractionOfMs)
                        {
                            //Thread.Sleep(0);  // hmmm
                        }
                        fFractionOfMs = objStopWatch.ElapsedMsec - fFractionOfMs;
                    }
                }
            }
            doShutDown();
        }

        private int display_fps = 60;
        private float display_delay = 1000 / 60f;
        private int DisplayFPS
        {
            get { return display_fps; }
            set
            {
                display_fps = value;
                if (display_fps > MAX_FPS) display_fps = MAX_FPS;
                if (display_fps < 1) display_fps = 1;
                display_delay = 1000 / (float)display_fps;

                Display.CurrentFPS = display_fps;
            }
        }
        private bool m_bUseAccurateFrameTiming = false;
        private bool UseAccurateFramingTiming
        {
            get { return m_bUseAccurateFrameTiming; }
            set { m_bUseAccurateFrameTiming = value; }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (m_bDisplayLoopRunning)
            {
                this.Hide();
                m_bDisplayLoopRunning = false;
                e.Cancel = true;
            }
        }

        private void udLinesPerFrame_ValueChanged(object sender, EventArgs e)
        {
            Display.LinesPerFrame = (int)udLinesPerFrame.Value;
        }

        private void udFPS_ValueChanged(object sender, EventArgs e)
        {
            if(udFPS.Value != DisplayFPS)
            {
                m_bPause_DisplayThread = true;
                DisplayFPS = (int)udFPS.Value;
                cmbAdaptors_SelectedIndexChanged(this, EventArgs.Empty);
                m_bPause_DisplayThread = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numberOfLinesToTime(sender);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            numberOfLinesToTime(sender);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numberOfLinesToTime(sender);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            numberOfLinesToTime(sender);
        }

        private int m_nLineTotal = 250000;
        private int m_nLineCount = 0;
        private void numberOfLinesToTime(object sender)
        {
            if(sender is RadioButton)
            {
                bool oldPause = m_bPause_DisplayThread;
                m_bPause_DisplayThread = false;

                RadioButton rb = (RadioButton)sender;

                bool bOk = int.TryParse(rb.Name.Substring(11), out int num);

                if (bOk)
                {
                    switch (num)
                    {
                        case 1:
                            m_nLineTotal = 250000;
                            break;
                        case 2:
                            m_nLineTotal = 500000;
                            break;
                        case 3:
                            m_nLineTotal = 1000000;
                            break;
                        case 4:
                            m_nLineTotal = 2000000;
                            break;
                    }
                }

                m_nLineCount = 0;
                m_objLines.Reset();

                m_bPause_DisplayThread = oldPause;
            }
        }

        private void chkRun_CheckedChanged(object sender, EventArgs e)
        {
            m_bPause_DisplayThread = !chkRun.Checked;
            m_nLineCount = 0;
            m_objLines.Reset();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            m_bResizeDX2Display = true;

            updateRes();
        }

        private void updateRes()
        {
            lblResolution.Text = picDisplay.Width.ToString() + " x " + picDisplay.Height.ToString();
        }

        private void chkVSync_CheckedChanged(object sender, EventArgs e)
        {
            Display.VerticalBlanks = chkVSync.Checked ? 1 : 0;
        }

        private void chkAA_CheckedChanged(object sender, EventArgs e)
        {
            Display.AntiAlias = chkAA.Checked;
        }

        private void udLineWidth_ValueChanged(object sender, EventArgs e)
        {
            Display.LineWidth = (float)udLineWidth.Value;
        }

        private void cmbAdaptors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbAdaptors.SelectedIndex >= 0)
            {
                SharpDX.DXGI.Adapter a = Display.DX2Adaptor(cmbAdaptors.Items[cmbAdaptors.SelectedIndex].ToString());

                if (a != null)
                {
                    Display.ShutdownDX2D();
                    Display.InitDX2D(a);
                }
            }
        }
        private bool is64Bit
        {
            get
            {
                return System.IntPtr.Size == 8 ? true : false;
            }
        }
    }
}