using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Idle
{
    public partial class FormMain : Form
    {

        // Imports
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        private bool isClosing = false;

        public FormMain()
        {
            InitializeComponent();

            // Start UpdateTime thread
            var IdleThread = new Thread(new ThreadStart(UpdateTime));
            IdleThread.IsBackground = true;
            IdleThread.Start();
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        static uint InputTime()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;
            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;
                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? idleTime : 0);
        }

        private void UpdateTime()
        {
            while(!isClosing)
            {
                this.InvokeEx(f => f.label1.Text = InputTime() + " ms");
                Thread.Sleep(10);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;
        }
    }
}