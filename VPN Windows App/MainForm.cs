namespace VPN_Windows_App
{
	public partial class MainForm : Form
	{
		public MainForm(string chatId)
		{
			InitializeComponent();
			userId.Text = chatId;
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void label4_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void label5_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}

		private void materialSwitch1_CheckedChanged(object sender, EventArgs e)
		{

		}
	}
}