namespace VPN_Windows_App
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			pictureBox1 = new PictureBox();
			pictureBox2 = new PictureBox();
			label4 = new Label();
			label5 = new Label();
			pictureBox3 = new PictureBox();
			userId = new Label();
			connectedSet = new MaterialSkin.Controls.MaterialSwitch();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
			SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.BackColor = Color.White;
			pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
			pictureBox1.ErrorImage = null;
			pictureBox1.Image = Properties.Resources.logowihtext;
			pictureBox1.InitialImage = null;
			pictureBox1.Location = new Point(12, -11);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(127, 115);
			pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			pictureBox1.Click += pictureBox1_Click;
			// 
			// pictureBox2
			// 
			pictureBox2.BackColor = Color.White;
			pictureBox2.Location = new Point(-4, -2);
			pictureBox2.Name = "pictureBox2";
			pictureBox2.Size = new Size(935, 106);
			pictureBox2.TabIndex = 1;
			pictureBox2.TabStop = false;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.BackColor = Color.White;
			label4.Cursor = Cursors.Hand;
			label4.Font = new Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
			label4.ForeColor = Color.Black;
			label4.Location = new Point(758, -2);
			label4.Name = "label4";
			label4.Size = new Size(23, 22);
			label4.TabIndex = 17;
			label4.Text = "X";
			label4.Click += label4_Click;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = Color.White;
			label5.Cursor = Cursors.Hand;
			label5.Font = new Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
			label5.ForeColor = Color.Black;
			label5.Location = new Point(731, -2);
			label5.Name = "label5";
			label5.Size = new Size(21, 22);
			label5.TabIndex = 18;
			label5.Text = "–";
			label5.Click += label5_Click;
			// 
			// pictureBox3
			// 
			pictureBox3.BackColor = Color.White;
			pictureBox3.Image = Properties.Resources.pngwing_com;
			pictureBox3.Location = new Point(681, 23);
			pictureBox3.Name = "pictureBox3";
			pictureBox3.Size = new Size(71, 53);
			pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox3.TabIndex = 19;
			pictureBox3.TabStop = false;
			// 
			// userId
			// 
			userId.AutoSize = true;
			userId.BackColor = Color.White;
			userId.ForeColor = Color.Black;
			userId.Location = new Point(681, 79);
			userId.Name = "userId";
			userId.Size = new Size(39, 15);
			userId.TabIndex = 20;
			userId.Text = "userId";
			// 
			// connectedSet
			// 
			connectedSet.AutoSize = true;
			connectedSet.Depth = 0;
			connectedSet.Location = new Point(303, 275);
			connectedSet.Margin = new Padding(0);
			connectedSet.MouseLocation = new Point(-1, -1);
			connectedSet.MouseState = MaterialSkin.MouseState.HOVER;
			connectedSet.Name = "connectedSet";
			connectedSet.Ripple = true;
			connectedSet.Size = new Size(116, 37);
			connectedSet.TabIndex = 22;
			connectedSet.Text = "Connect";
			connectedSet.UseVisualStyleBackColor = true;
			connectedSet.CheckedChanged += materialSwitch1_CheckedChanged;
			// 
			// MainForm
			// 
			AutoScaleMode = AutoScaleMode.None;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			BackColor = Color.FromArgb(85, 149, 232);
			BackgroundImageLayout = ImageLayout.Zoom;
			ClientSize = new Size(780, 594);
			ControlBox = false;
			Controls.Add(userId);
			Controls.Add(pictureBox3);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(pictureBox1);
			Controls.Add(pictureBox2);
			Controls.Add(connectedSet);
			DoubleBuffered = true;
			ForeColor = Color.White;
			FormBorderStyle = FormBorderStyle.None;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			Name = "MainForm";
			ShowIcon = false;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Morkey VPN";
			Load += Form1_Load;
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private PictureBox pictureBox1;
		private PictureBox pictureBox2;
		private Label label4;
		private Label label5;
		private PictureBox pictureBox3;
		private Label userId;
		private MaterialSkin.Controls.MaterialSwitch connectedSet;
	}
}