namespace KioskBrowser.WinForm;
using CefSharp;
using CefSharp.WinForms;

partial class Form1
{
    public ChromiumWebBrowser ChromeBrowser;
    
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
        StartPosition = FormStartPosition.Manual;
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.Text = Settings.AppName;
        //this.Width = Settings.Width;
        //this.Height = Settings.Height;
        //this.Left = Settings.X;
        //this.Top = Settings.Y;
        this.Bounds = new Rectangle(Settings.X, Settings.Y, Settings.Width, Settings.Height);
        this.ClientSize = new Size(Settings.Width, Settings.Height);
        this.TopMost = Settings.AlwaysOnTop;
        if (Settings.WindowLess)
        {
            this.FormBorderStyle = FormBorderStyle.None;
        }

        InitializeChromium();
    }
    
    public void InitializeChromium()
    {
        CefSettings cefSettings = new CefSettings();
        cefSettings.CachePath = GetExecutingDirectory()!.FullName + "\\webcache\\";
        
        if (Settings.UserAgent is not null)
            cefSettings.UserAgent = Settings.UserAgent; 
        
        // Initialize cef with the provided settings
        Cef.Initialize(cefSettings);
        // Create a browser component
        ChromeBrowser = new ChromiumWebBrowser(Settings.WebUrl);
        // Add it to the form and fill it to the form window.
        this.Controls.Add(ChromeBrowser);
        ChromeBrowser.Dock = DockStyle.Fill;
    }
    

    #endregion
}