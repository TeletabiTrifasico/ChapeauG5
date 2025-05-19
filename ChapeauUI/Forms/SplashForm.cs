using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapeauG5.ChapeauUI.Forms
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer fadeTimer;
        private double opacityIncrement = 0.02;
        private double fadeInEnd = 1.0;
        private bool isClosing = false;

        public SplashForm()
        {
            InitializeComponent();
            this.Opacity = 0;
            InitializeFadeIn();
        }

        private void InitializeFadeIn()
        {
            fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 10;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            // Check if the form is closing or already closed
            if (isClosing || this.IsDisposed)
            {
                fadeTimer.Stop();
                return;
            }

            if (this.Opacity < fadeInEnd)
            {
                this.Opacity += opacityIncrement;
            }
            else
            {
                fadeTimer.Stop();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Mark as closing to prevent further opacity updates
            isClosing = true;

            // Stop and dispose the timer
            if (fadeTimer != null)
            {
                fadeTimer.Stop();
                fadeTimer.Tick -= FadeTimer_Tick;
                fadeTimer.Dispose();
                fadeTimer = null;
            }

            base.OnFormClosing(e);
        }

        private void SplashForm_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }
    }
}