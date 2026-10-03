using System;
using System.Drawing;
using System.Windows.Forms;

namespace AnimatedGifDisplay
{
    public partial class MainForm : Form
    {
        private Image currentGif;
        private bool isAnimating;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "Animated GIF Viewer";
            this.Width = 700;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize controls
            setupControls();
            isAnimating = false;
        }

        private void setupControls()
        {
            // Panel for buttons
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = SystemColors.Control
            };

            // Open File Button
            Button openButton = new Button
            {
                Text = "Open GIF",
                Location = new Point(10, 10),
                Width = 100,
                Height = 30
            };
            openButton.Click += OpenButton_Click;

            // Play/Pause Button
            Button playPauseButton = new Button
            {
                Text = "Play",
                Location = new Point(120, 10),
                Width = 100,
                Height = 30,
                Enabled = false
            };
            playPauseButton.Click += (s, e) => PlayPauseButton_Click(s, e, playPauseButton);
            playPauseButton.Tag = "playPauseButton";
            this.Tag = playPauseButton; // Store reference for later access

            // Loop Checkbox
            CheckBox loopCheckBox = new CheckBox
            {
                Text = "Loop Animation",
                Location = new Point(230, 15),
                Width = 120,
                Height = 20,
                Checked = true
            };
            loopCheckBox.Tag = "loopCheckBox";

            // Status Label
            Label statusLabel = new Label
            {
                Text = "No GIF loaded",
                Location = new Point(360, 15),
                Width = 300,
                Height = 20,
                AutoSize = false
            };
            statusLabel.Tag = "statusLabel";

            buttonPanel.Controls.Add(openButton);
            buttonPanel.Controls.Add(playPauseButton);
            buttonPanel.Controls.Add(loopCheckBox);
            buttonPanel.Controls.Add(statusLabel);

            // PictureBox for GIF display
            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = Color.Black
            };
            pictureBox.Tag = "pictureBox";

            this.Controls.Add(pictureBox);
            this.Controls.Add(buttonPanel);
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
                openFileDialog.Title = "Select an Animated GIF";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Dispose previous image if exists
                        if (currentGif != null)
                        {
                            currentGif.Dispose();
                        }

                        // Load new GIF
                        currentGif = Image.FromFile(openFileDialog.FileName);
                        
                        // Get picture box and display
                        PictureBox pictureBox = FindControl("pictureBox") as PictureBox;
                        if (pictureBox != null)
                        {
                            pictureBox.Image = currentGif;
                        }

                        // Update UI
                        Label statusLabel = FindControl("statusLabel") as Label;
                        if (statusLabel != null)
                        {
                            statusLabel.Text = $"Loaded: {System.IO.Path.GetFileName(openFileDialog.FileName)}";
                        }

                        Button playPauseButton = FindControl("playPauseButton") as Button;
                        if (playPauseButton != null)
                        {
                            playPauseButton.Enabled = true;
                            playPauseButton.Text = "Play";
                        }

                        isAnimating = false;

                        MessageBox.Show("GIF loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading GIF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PlayPauseButton_Click(object sender, EventArgs e, Button playPauseButton)
        {
            if (currentGif == null)
            {
                MessageBox.Show("Please load a GIF first.", "No GIF Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isAnimating = !isAnimating;

            if (isAnimating)
            {
                playPauseButton.Text = "Pause";

                // Start animation
                PictureBox pictureBox = FindControl("pictureBox") as PictureBox;
                if (pictureBox != null)
                {
                    ImageAnimator.Animate(currentGif, OnFrameChanged);
                    OnFrameChanged(null, null);
                }
            }
            else
            {
                playPauseButton.Text = "Play";

                // Stop animation
                if (currentGif != null)
                {
                    ImageAnimator.StopAnimate(currentGif, OnFrameChanged);
                }
            }
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            if (isAnimating && currentGif != null)
            {
                PictureBox pictureBox = FindControl("pictureBox") as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.Invalidate();
                }
            }
        }

        private Control FindControl(string tag)
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel)
                {
                    foreach (Control childControl in panel.Controls)
                    {
                        if (childControl.Tag != null && childControl.Tag.ToString() == tag)
                        {
                            return childControl;
                        }
                    }
                }
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (currentGif != null)
                {
                    ImageAnimator.StopAnimate(currentGif, OnFrameChanged);
                    currentGif.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
