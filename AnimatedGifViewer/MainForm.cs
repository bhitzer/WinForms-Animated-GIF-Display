using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AnimatedGifViewer
{
    public partial class MainForm : Form
    {
        private Image _gifImage;
        private bool _isAnimating;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "GIF files (*.gif)|*.gif|All files (*.*)|*.*";
                dlg.Title = "Select a GIF file";

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    if (_gifImage != null)
                    {
                        ImageAnimator.StopAnimate(_gifImage, OnFrameChanged);
                        _gifImage.Dispose();
                        _gifImage = null;
                    }

                    _gifImage = Image.FromFile(dlg.FileName);
                    pictureBox.Image = _gifImage;

                    statusLabel.Text = Path.GetFileName(dlg.FileName);
                    playPauseButton.Enabled = true;
                    playPauseButton.Text = "Play";
                    _isAnimating = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Unable to load GIF: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            if (_gifImage == null)
            {
                MessageBox.Show(this, "Please load a GIF first.", "No GIF",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isAnimating)
            {
                ImageAnimator.StopAnimate(_gifImage, OnFrameChanged);
                _isAnimating = false;
                playPauseButton.Text = "Play";
            }
            else
            {
                ImageAnimator.Animate(_gifImage, OnFrameChanged);
                _isAnimating = true;
                playPauseButton.Text = "Pause";
            }
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (pictureBox != null && pictureBox.IsHandleCreated)
            {
                pictureBox.Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_gifImage != null)
                {
                    ImageAnimator.StopAnimate(_gifImage, OnFrameChanged);
                    _gifImage.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
