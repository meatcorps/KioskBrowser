using Timer = System.Windows.Forms.Timer;

namespace KioskBrowser.PositionUtility;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        timer1.Start();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        label1.Text = Bounds.ToString();
    }

    private void Form1_Move(object sender, EventArgs e)
    {

        label1.Text = Bounds.ToString();
    }

    private void Form1_Resize(object sender, EventArgs e)
    {
        label1.Text = Bounds.ToString();
    }
}