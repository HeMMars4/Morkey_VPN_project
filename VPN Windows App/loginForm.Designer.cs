namespace VPN_Windows_App
{
	partial class loginForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(loginForm));
			pictureBox1 = new PictureBox();
			pictureBox2 = new PictureBox();
			label1 = new Label();
			login = new RichTextBox();
			label2 = new Label();
			label3 = new Label();
			pass = new RichTextBox();
			label4 = new Label();
			label5 = new Label();
			pictureBox3 = new PictureBox();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
			SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.BackColor = Color.White;
			pictureBox1.Image = Properties.Resources.logowihtext;
			pictureBox1.Location = new Point(95, -14);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(198, 119);
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox1.TabIndex = 4;
			pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			pictureBox2.BackColor = Color.White;
			pictureBox2.Location = new Point(-9, -1);
			pictureBox2.Name = "pictureBox2";
			pictureBox2.Size = new Size(400, 106);
			pictureBox2.TabIndex = 5;
			pictureBox2.TabStop = false;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new Font("Segoe UI Black", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label1.Location = new Point(55, 125);
			label1.Name = "label1";
			label1.Size = new Size(267, 42);
			label1.TabIndex = 6;
			label1.Text = "   Для дальнейших действий\r\nнеобходимо авторизироваться";
			// 
			// login
			// 
			login.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			login.Location = new Point(67, 262);
			login.Multiline = false;
			login.Name = "login";
			login.Size = new Size(237, 33);
			login.TabIndex = 12;
			login.Text = "";
			login.TextChanged += login_TextChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
			label2.Location = new Point(67, 233);
			label2.Name = "label2";
			label2.Size = new Size(237, 15);
			label2.TabIndex = 13;
			label2.Text = "Ваш уникальный индентификатор";
			label2.Click += label2_Click;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
			label3.Location = new Point(143, 339);
			label3.Name = "label3";
			label3.Size = new Size(83, 15);
			label3.TabIndex = 14;
			label3.Text = "Ваш пароль";
			// 
			// pass
			// 
			pass.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			pass.Location = new Point(67, 370);
			pass.MaxLength = 65;
			pass.Multiline = false;
			pass.Name = "pass";
			pass.Size = new Size(237, 33);
			pass.TabIndex = 15;
			pass.Text = "";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.BackColor = Color.White;
			label4.Cursor = Cursors.Hand;
			label4.Font = new Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
			label4.Location = new Point(359, -1);
			label4.Name = "label4";
			label4.Size = new Size(23, 22);
			label4.TabIndex = 16;
			label4.Text = "X";
			label4.Click += label4_Click;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.BackColor = Color.White;
			label5.Cursor = Cursors.Hand;
			label5.Font = new Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
			label5.Location = new Point(332, -1);
			label5.Name = "label5";
			label5.Size = new Size(21, 22);
			label5.TabIndex = 17;
			label5.Text = "–";
			label5.Click += label5_Click;
			// 
			// pictureBox3
			// 
			pictureBox3.Cursor = Cursors.Hand;
			pictureBox3.Image = Properties.Resources.pngegg;
			pictureBox3.Location = new Point(143, 426);
			pictureBox3.Name = "pictureBox3";
			pictureBox3.Size = new Size(106, 86);
			pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox3.TabIndex = 18;
			pictureBox3.TabStop = false;
			pictureBox3.Click += pictureBox3_Click;
			// 
			// loginForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.FromArgb(85, 149, 232);
			ClientSize = new Size(384, 569);
			ControlBox = false;
			Controls.Add(pictureBox3);
			Controls.Add(label5);
			Controls.Add(label4);
			Controls.Add(pass);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(login);
			Controls.Add(label1);
			Controls.Add(pictureBox1);
			Controls.Add(pictureBox2);
			FormBorderStyle = FormBorderStyle.None;
			Icon = (Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			Name = "loginForm";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Morkey VPN";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private PictureBox pictureBox1;
		private PictureBox pictureBox2;
		private Label label1;
		private RichTextBox login;
		private Label label2;
		private Label label3;
		private RichTextBox pass;
		private Label label4;
		private Label label5;
		private PictureBox pictureBox3;
	}
}