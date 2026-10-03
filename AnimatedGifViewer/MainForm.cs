using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AnimatedGifViewer
{
    public partial class MainForm : Form
    {
        private readonly Button _openButton;
        private readonly Button _playPauseButton;
        private readonly CheckBox _loopCheckBox;
        private readonly Label _statusLabel;
        private readonly PictureBox _pictureBox;

        private Image _gifImage;
        private bool _isAnimating;

        public MainForm()
        {
            InitializeComponent();

            this.Text = "Animated GIF Viewer";
            this.Width = 760;
            this.Height = 620;
            this.StartPosition = FormStartPosition.CenterScreen;

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10)
            };

            _openButton = new Button
            {
                Text = "Open GIF",
                Width = 110,
                Height = 30
            };
            _openButton.Click += OpenButton_Click;

            _playPauseButton = new Button
            {
                Text = "Play",
                Width = 110,
                Height = 30,
                Enabled = false
            };
            _playPauseButton.Click += PlayPauseButton_Click;

            _loopCheckBox = new CheckBox
            {
                Text = "Loop Animation",
                Checked = true,
                AutoSize = true,
                Margin = new Padding(20, 8, 0, 0)
            };

            _statusLabel = new Label
            {
                Text = "No GIF loaded",
                AutoSize = true,
                Margin = new Padding(20, 8, 0, 0)
            };

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = Color.Black
            };

            topPanel.Controls.Add(_openButton);
            topPanel.Controls.Add(_playPauseButton);
            topPanel.Controls.Add(_loopCheckBox);
            topPanel.Controls.Add(_statusLabel);

            this.Controls.Add(_pictureBox);
            this.Controls.Add(topPanel);
            this.Controls.SetChildIndex(_pictureBox, 0);
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
                    _pictureBox.Image = _gifImage;

                    _statusLabel.Text = Path.GetFileName(dlg.FileName);
                    _playPauseButton.Enabled = true;
                    _playPauseButton.Text = "Play";
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
                _playPauseButton.Text = "Play";
            }
            else
            {
                ImageAnimator.Animate(_gifImage, OnFrameChanged);
                _isAnimating = true;
                _playPauseButton.Text = "Pause";
            }
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (_pictureBox != null && _pictureBox.IsHandleCreated)
            {
                _pictureBox.Invalidate();
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
