namespace OnvifCamera
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DiscoverButton = new Button();
            comboBox1 = new ComboBox();
            UsernameLabel = new Label();
            UserTextBox = new TextBox();
            PasswordTextBox = new TextBox();
            PasswordLabel = new Label();
            IpTextBox = new TextBox();
            IpLabel = new Label();
            DisplayButton = new Button();
            UrlTextBox = new TextBox();
            videoView = new LibVLCSharp.WinForms.VideoView();
            StatusTextBox = new TextBox();
            ConnectEventsButton = new Button();
            ((System.ComponentModel.ISupportInitialize)videoView).BeginInit();
            SuspendLayout();
            // 
            // DiscoverButton
            // 
            DiscoverButton.Location = new Point(440, 18);
            DiscoverButton.Margin = new Padding(4);
            DiscoverButton.Name = "DiscoverButton";
            DiscoverButton.Size = new Size(118, 36);
            DiscoverButton.TabIndex = 0;
            DiscoverButton.Text = "Discover";
            DiscoverButton.UseVisualStyleBackColor = true;
            DiscoverButton.Click += DiscoverButton_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(565, 19);
            comboBox1.Margin = new Padding(4);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(292, 33);
            comboBox1.TabIndex = 1;
            // 
            // UsernameLabel
            // 
            UsernameLabel.AutoSize = true;
            UsernameLabel.Location = new Point(28, 19);
            UsernameLabel.Margin = new Padding(4, 0, 4, 0);
            UsernameLabel.Name = "UsernameLabel";
            UsernameLabel.Size = new Size(51, 25);
            UsernameLabel.TabIndex = 2;
            UsernameLabel.Text = "User:";
            // 
            // UserTextBox
            // 
            UserTextBox.Location = new Point(140, 15);
            UserTextBox.Margin = new Padding(4);
            UserTextBox.Name = "UserTextBox";
            UserTextBox.Size = new Size(292, 31);
            UserTextBox.TabIndex = 3;
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Location = new Point(140, 56);
            PasswordTextBox.Margin = new Padding(4);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.Size = new Size(292, 31);
            PasswordTextBox.TabIndex = 5;
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.Location = new Point(28, 60);
            PasswordLabel.Margin = new Padding(4, 0, 4, 0);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(91, 25);
            PasswordLabel.TabIndex = 4;
            PasswordLabel.Text = "Password:";
            // 
            // IpTextBox
            // 
            IpTextBox.Location = new Point(140, 98);
            IpTextBox.Margin = new Padding(4);
            IpTextBox.Name = "IpTextBox";
            IpTextBox.Size = new Size(292, 31);
            IpTextBox.TabIndex = 6;
            // 
            // IpLabel
            // 
            IpLabel.AutoSize = true;
            IpLabel.Location = new Point(28, 101);
            IpLabel.Margin = new Padding(4, 0, 4, 0);
            IpLabel.Name = "IpLabel";
            IpLabel.Size = new Size(32, 25);
            IpLabel.TabIndex = 7;
            IpLabel.Text = "Ip:";
            // 
            // DisplayButton
            // 
            DisplayButton.Location = new Point(15, 139);
            DisplayButton.Margin = new Padding(4);
            DisplayButton.Name = "DisplayButton";
            DisplayButton.Size = new Size(161, 36);
            DisplayButton.TabIndex = 8;
            DisplayButton.Text = "Connect Single";
            DisplayButton.UseVisualStyleBackColor = true;
            DisplayButton.Click += DisplayButton_Click;
            // 
            // UrlTextBox
            // 
            UrlTextBox.Location = new Point(352, 139);
            UrlTextBox.Margin = new Padding(4);
            UrlTextBox.Name = "UrlTextBox";
            UrlTextBox.Size = new Size(505, 31);
            UrlTextBox.TabIndex = 10;
            // 
            // videoView
            // 
            videoView.BackColor = Color.Black;
            videoView.Location = new Point(15, 182);
            videoView.Margin = new Padding(4);
            videoView.MediaPlayer = null;
            videoView.Name = "videoView";
            videoView.Size = new Size(842, 604);
            videoView.TabIndex = 11;
            videoView.Text = "videoView1";
            // 
            // StatusTextBox
            // 
            StatusTextBox.Location = new Point(15, 794);
            StatusTextBox.Margin = new Padding(4);
            StatusTextBox.Multiline = true;
            StatusTextBox.Name = "StatusTextBox";
            StatusTextBox.ScrollBars = ScrollBars.Vertical;
            StatusTextBox.Size = new Size(842, 226);
            StatusTextBox.TabIndex = 12;
            // 
            // ConnectEventsButton
            // 
            ConnectEventsButton.Location = new Point(184, 139);
            ConnectEventsButton.Margin = new Padding(4);
            ConnectEventsButton.Name = "ConnectEventsButton";
            ConnectEventsButton.Size = new Size(161, 36);
            ConnectEventsButton.TabIndex = 13;
            ConnectEventsButton.Text = "Connect Events";
            ConnectEventsButton.UseVisualStyleBackColor = true;
            ConnectEventsButton.Click += ConnectEventsButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(877, 1033);
            Controls.Add(ConnectEventsButton);
            Controls.Add(StatusTextBox);
            Controls.Add(videoView);
            Controls.Add(UrlTextBox);
            Controls.Add(DisplayButton);
            Controls.Add(IpLabel);
            Controls.Add(IpTextBox);
            Controls.Add(PasswordTextBox);
            Controls.Add(PasswordLabel);
            Controls.Add(UserTextBox);
            Controls.Add(UsernameLabel);
            Controls.Add(comboBox1);
            Controls.Add(DiscoverButton);
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)videoView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button DiscoverButton;
        private ComboBox comboBox1;
        private Label UsernameLabel;
        private TextBox UserTextBox;
        private TextBox PasswordTextBox;
        private Label PasswordLabel;
        private TextBox IpTextBox;
        private Label IpLabel;
        private Button DisplayButton;        
        private TextBox UrlTextBox;
        private LibVLCSharp.WinForms.VideoView videoView;
        private TextBox StatusTextBox;
        private Button ConnectEventsButton;
    }
}
