using System.Drawing;
using System.Windows.Forms;

namespace RemoteControl
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            tabControlMain = new TabControl();
            tabPageMain = new TabPage();
            groupBoxDisclaimer = new GroupBox();
            labelLinkFormater = new Label();
            labelTutoReport = new Label();
            linkLabelGithub = new LinkLabel();
            richTextBoxInformation = new RichTextBox();
            groupBoxControls = new GroupBox();
            groupBoxEdenserver = new GroupBox();
            buttonStopEdenserver = new Button();
            buttonRestartEdenserver = new Button();
            buttonStartEdenserver = new Button();
            textBoxEdenserver = new TextBox();
            groupBoxSVO = new GroupBox();
            buttonStopSVO = new Button();
            buttonRestartSVO = new Button();
            buttonStartSVO = new Button();
            textBoxSVO = new TextBox();
            groupBoxSSFWServer = new GroupBox();
            buttonStopSSFWServer = new Button();
            buttonRestartSSFWServer = new Button();
            buttonStartSSFWServer = new Button();
            textBoxSSFWServer = new TextBox();
            groupBoxQuazalserver = new GroupBox();
            buttonStopQuazalserver = new Button();
            buttonRestartQuazalserver = new Button();
            buttonStartQuazalserver = new Button();
            textBoxQuazalserver = new TextBox();
            groupBoxMultispy = new GroupBox();
            buttonStopMultispy = new Button();
            buttonRestartMultispy = new Button();
            buttonStartMultispy = new Button();
            textBoxMultispy = new TextBox();
            groupBoxMultisocks = new GroupBox();
            buttonStopMultisocks = new Button();
            buttonRestartMultisocks = new Button();
            buttonStartMultisocks = new Button();
            textBoxMultisocks = new TextBox();
            groupBoxHorizon = new GroupBox();
            buttonStopHorizon = new Button();
            buttonRestartHorizon = new Button();
            buttonStartHorizon = new Button();
            textBoxHorizon = new TextBox();
            groupBoxDNS = new GroupBox();
            buttonStopDNS = new Button();
            buttonRestartDNS = new Button();
            buttonStartDNS = new Button();
            textBoxDNS = new TextBox();
            groupBoxHTTP = new GroupBox();
            buttonStopHTTP = new Button();
            buttonRestartHTTP = new Button();
            buttonStartHTTP = new Button();
            textBoxHTTP = new TextBox();
            pictureBoxMainLogo = new PictureBox();
            tabPageHTTP = new TabPage();
            richTextBoxHTTPLog = new RichTextBox();
            tabPageDNS = new TabPage();
            richTextBoxDNSLog = new RichTextBox();
            tabPageHorizon = new TabPage();
            richTextBoxHorizonLog = new RichTextBox();
            tabPageMultisocks = new TabPage();
            richTextBoxMultisocksLog = new RichTextBox();
            tabPageMultispy = new TabPage();
            richTextBoxMultispyLog = new RichTextBox();
            tabPageQuazalserver = new TabPage();
            richTextBoxQuazalserverLog = new RichTextBox();
            tabPageSSFWServer = new TabPage();
            richTextBoxSSFWServerLog = new RichTextBox();
            tabPageSVO = new TabPage();
            richTextBoxSVOLog = new RichTextBox();
            tabPageEdenserver = new TabPage();
            richTextBoxEdenserverLog = new RichTextBox();
            tabPageSettings = new TabPage();
            groupBoxAuxConfigFiles = new GroupBox();
            buttonConfigureAriesDatabase = new Button();
            textBoxAriesDatabaseJsonPath = new TextBox();
            labelMultiSocksAux = new Label();
            buttonConfigureHorizonDatabase = new Button();
            textBoxHorizonDatabaseJsonPath = new TextBox();
            buttonConfigureEbootDefs = new Button();
            textBoxEbootDefsJsonPath = new TextBox();
            buttonConfigureMUIS = new Button();
            buttonConfigureDME = new Button();
            buttonConfigureMedius = new Button();
            buttonConfigureBwps = new Button();
            buttonConfigureNat = new Button();
            textBoxMUISJsonPath = new TextBox();
            textBoxDMEJsonPath = new TextBox();
            textBoxMediusJsonPath = new TextBox();
            textBoxBwpsJsonPath = new TextBox();
            textBoxNatJsonPath = new TextBox();
            labelMediusAux = new Label();
            groupBoxConfigFiles = new GroupBox();
            buttonConfigureEdenserver = new Button();
            buttonConfigureSVO = new Button();
            buttonConfigureSSFWServer = new Button();
            buttonConfigureQuazalserver = new Button();
            buttonConfigureMultispy = new Button();
            buttonConfigureMultisocks = new Button();
            buttonConfigureHorizon = new Button();
            buttonConfigureDNS = new Button();
            buttonConfigureApacheNet = new Button();
            textBoxEdenserverJsonPath = new TextBox();
            textBoxSVOJsonPath = new TextBox();
            textBoxSSFWServerJsonPath = new TextBox();
            textBoxQuazalserverJsonPath = new TextBox();
            textBoxMultispyJsonPath = new TextBox();
            textBoxMultisocksJsonPath = new TextBox();
            textBoxHorizonJsonPath = new TextBox();
            textBoxDNSJsonPath = new TextBox();
            textBoxApacheNetJsonPath = new TextBox();
            labelEdenserver1 = new Label();
            labelSVO1 = new Label();
            labelSSFWServer1 = new Label();
            labelQuazalserver1 = new Label();
            labelMultispy1 = new Label();
            labelMultisocks1 = new Label();
            labelHorizon1 = new Label();
            labelDNS1 = new Label();
            labelApacheNet1 = new Label();
            richTextBoxLicense = new RichTextBox();
            groupBoxServersPath = new GroupBox();
            buttonBrowseEdenserverPath = new Button();
            labelEdenserver = new Label();
            textBoxEdenserverPath = new TextBox();
            buttonBrowseSVOPath = new Button();
            labelSVO = new Label();
            textBoxSVOPath = new TextBox();
            buttonBrowseSSFWServerPath = new Button();
            labelSSFWServer = new Label();
            textBoxSSFWServerPath = new TextBox();
            buttonBrowseQuazalserverPath = new Button();
            labelQuazalserver = new Label();
            textBoxQuazalserverPath = new TextBox();
            labelMultispy = new Label();
            buttonBrowseMultispyPath = new Button();
            textBoxMultispyPath = new TextBox();
            labelMultisocks = new Label();
            buttonBrowseMultisocksPath = new Button();
            textBoxMultisocksPath = new TextBox();
            buttonBrowseHorizonPath = new Button();
            labelHorizon = new Label();
            textBoxHorizonPath = new TextBox();
            labelDNS = new Label();
            labelApacheNet = new Label();
            buttonBrowseDNSPath = new Button();
            textBoxDNSPath = new TextBox();
            buttonBrowseApacheNetPath = new Button();
            textBoxApacheNetPath = new TextBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            tabControlMain.SuspendLayout();
            tabPageMain.SuspendLayout();
            groupBoxDisclaimer.SuspendLayout();
            groupBoxControls.SuspendLayout();
            groupBoxEdenserver.SuspendLayout();
            groupBoxSVO.SuspendLayout();
            groupBoxSSFWServer.SuspendLayout();
            groupBoxQuazalserver.SuspendLayout();
            groupBoxMultispy.SuspendLayout();
            groupBoxMultisocks.SuspendLayout();
            groupBoxHorizon.SuspendLayout();
            groupBoxDNS.SuspendLayout();
            groupBoxHTTP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMainLogo).BeginInit();
            tabPageHTTP.SuspendLayout();
            tabPageDNS.SuspendLayout();
            tabPageHorizon.SuspendLayout();
            tabPageMultisocks.SuspendLayout();
            tabPageMultispy.SuspendLayout();
            tabPageQuazalserver.SuspendLayout();
            tabPageSSFWServer.SuspendLayout();
            tabPageSVO.SuspendLayout();
            tabPageEdenserver.SuspendLayout();
            tabPageSettings.SuspendLayout();
            groupBoxAuxConfigFiles.SuspendLayout();
            groupBoxConfigFiles.SuspendLayout();
            groupBoxServersPath.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageMain);
            tabControlMain.Controls.Add(tabPageHTTP);
            tabControlMain.Controls.Add(tabPageDNS);
            tabControlMain.Controls.Add(tabPageHorizon);
            tabControlMain.Controls.Add(tabPageMultisocks);
            tabControlMain.Controls.Add(tabPageMultispy);
            tabControlMain.Controls.Add(tabPageQuazalserver);
            tabControlMain.Controls.Add(tabPageSSFWServer);
            tabControlMain.Controls.Add(tabPageSVO);
            tabControlMain.Controls.Add(tabPageEdenserver);
            tabControlMain.Controls.Add(tabPageSettings);
            tabControlMain.Location = new Point(-1, -1);
            tabControlMain.Margin = new Padding(3, 4, 3, 4);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1422, 968);
            tabControlMain.TabIndex = 0;
            // 
            // tabPageMain
            // 
            tabPageMain.Controls.Add(groupBoxDisclaimer);
            tabPageMain.Controls.Add(richTextBoxInformation);
            tabPageMain.Controls.Add(groupBoxControls);
            tabPageMain.Controls.Add(pictureBoxMainLogo);
            tabPageMain.Location = new Point(4, 29);
            tabPageMain.Margin = new Padding(3, 4, 3, 4);
            tabPageMain.Name = "tabPageMain";
            tabPageMain.Padding = new Padding(3, 4, 3, 4);
            tabPageMain.Size = new Size(1414, 935);
            tabPageMain.TabIndex = 0;
            tabPageMain.Text = "Main";
            tabPageMain.UseVisualStyleBackColor = true;
            // 
            // groupBoxDisclaimer
            // 
            groupBoxDisclaimer.Controls.Add(labelLinkFormater);
            groupBoxDisclaimer.Controls.Add(labelTutoReport);
            groupBoxDisclaimer.Controls.Add(linkLabelGithub);
            groupBoxDisclaimer.Location = new Point(704, 730);
            groupBoxDisclaimer.Name = "groupBoxDisclaimer";
            groupBoxDisclaimer.Size = new Size(697, 201);
            groupBoxDisclaimer.TabIndex = 2;
            groupBoxDisclaimer.TabStop = false;
            groupBoxDisclaimer.Text = "Disclaimer";
            // 
            // labelLinkFormater
            // 
            labelLinkFormater.AutoSize = true;
            labelLinkFormater.Location = new Point(20, 86);
            labelLinkFormater.Name = "labelLinkFormater";
            labelLinkFormater.Size = new Size(15, 20);
            labelLinkFormater.TabIndex = 2;
            labelLinkFormater.Text = "-";
            // 
            // labelTutoReport
            // 
            labelTutoReport.AutoSize = true;
            labelTutoReport.Location = new Point(20, 40);
            labelTutoReport.Name = "labelTutoReport";
            labelTutoReport.Size = new Size(432, 20);
            labelTutoReport.TabIndex = 1;
            labelTutoReport.Text = "For more infos about the software, please visit the following Url:";
            // 
            // linkLabelGithub
            // 
            linkLabelGithub.AutoSize = true;
            linkLabelGithub.Location = new Point(38, 86);
            linkLabelGithub.Name = "linkLabelGithub";
            linkLabelGithub.Size = new Size(341, 20);
            linkLabelGithub.TabIndex = 0;
            linkLabelGithub.TabStop = true;
            linkLabelGithub.Text = "https://github.com/GitHubProUser67/MultiServer3";
            linkLabelGithub.LinkClicked += linkLabelGithub_LinkClicked;
            // 
            // richTextBoxInformation
            // 
            richTextBoxInformation.Location = new Point(0, 602);
            richTextBoxInformation.Margin = new Padding(3, 4, 3, 4);
            richTextBoxInformation.Name = "richTextBoxInformation";
            richTextBoxInformation.ReadOnly = true;
            richTextBoxInformation.Size = new Size(697, 329);
            richTextBoxInformation.TabIndex = 0;
            richTextBoxInformation.Text = "";
            // 
            // groupBoxControls
            // 
            groupBoxControls.Controls.Add(groupBoxEdenserver);
            groupBoxControls.Controls.Add(groupBoxSVO);
            groupBoxControls.Controls.Add(groupBoxSSFWServer);
            groupBoxControls.Controls.Add(groupBoxQuazalserver);
            groupBoxControls.Controls.Add(groupBoxMultispy);
            groupBoxControls.Controls.Add(groupBoxMultisocks);
            groupBoxControls.Controls.Add(groupBoxHorizon);
            groupBoxControls.Controls.Add(groupBoxDNS);
            groupBoxControls.Controls.Add(groupBoxHTTP);
            groupBoxControls.Location = new Point(704, 0);
            groupBoxControls.Margin = new Padding(3, 4, 3, 4);
            groupBoxControls.Name = "groupBoxControls";
            groupBoxControls.Padding = new Padding(3, 4, 3, 4);
            groupBoxControls.Size = new Size(697, 723);
            groupBoxControls.TabIndex = 1;
            groupBoxControls.TabStop = false;
            groupBoxControls.Text = "Controls";
            // 
            // groupBoxEdenserver
            // 
            groupBoxEdenserver.Controls.Add(buttonStopEdenserver);
            groupBoxEdenserver.Controls.Add(buttonRestartEdenserver);
            groupBoxEdenserver.Controls.Add(buttonStartEdenserver);
            groupBoxEdenserver.Controls.Add(textBoxEdenserver);
            groupBoxEdenserver.Location = new Point(489, 476);
            groupBoxEdenserver.Margin = new Padding(3, 4, 3, 4);
            groupBoxEdenserver.Name = "groupBoxEdenserver";
            groupBoxEdenserver.Padding = new Padding(3, 4, 3, 4);
            groupBoxEdenserver.Size = new Size(181, 216);
            groupBoxEdenserver.TabIndex = 9;
            groupBoxEdenserver.TabStop = false;
            groupBoxEdenserver.Text = "EdenServer";
            // 
            // buttonStopEdenserver
            // 
            buttonStopEdenserver.Location = new Point(9, 163);
            buttonStopEdenserver.Margin = new Padding(3, 4, 3, 4);
            buttonStopEdenserver.Name = "buttonStopEdenserver";
            buttonStopEdenserver.Size = new Size(166, 37);
            buttonStopEdenserver.TabIndex = 3;
            buttonStopEdenserver.Text = "Stop!";
            buttonStopEdenserver.UseVisualStyleBackColor = true;
            buttonStopEdenserver.Click += buttonStopEdenserver_Click;
            // 
            // buttonRestartEdenserver
            // 
            buttonRestartEdenserver.Enabled = false;
            buttonRestartEdenserver.Location = new Point(9, 118);
            buttonRestartEdenserver.Margin = new Padding(3, 4, 3, 4);
            buttonRestartEdenserver.Name = "buttonRestartEdenserver";
            buttonRestartEdenserver.Size = new Size(166, 37);
            buttonRestartEdenserver.TabIndex = 2;
            buttonRestartEdenserver.Text = "Restart!";
            buttonRestartEdenserver.UseVisualStyleBackColor = true;
            // 
            // buttonStartEdenserver
            // 
            buttonStartEdenserver.Location = new Point(7, 73);
            buttonStartEdenserver.Margin = new Padding(3, 4, 3, 4);
            buttonStartEdenserver.Name = "buttonStartEdenserver";
            buttonStartEdenserver.Size = new Size(166, 37);
            buttonStartEdenserver.TabIndex = 1;
            buttonStartEdenserver.Text = "Start!";
            buttonStartEdenserver.UseVisualStyleBackColor = true;
            buttonStartEdenserver.Click += buttonStartEdenserver_Click;
            // 
            // textBoxEdenserver
            // 
            textBoxEdenserver.Location = new Point(7, 29);
            textBoxEdenserver.Margin = new Padding(3, 4, 3, 4);
            textBoxEdenserver.Name = "textBoxEdenserver";
            textBoxEdenserver.ReadOnly = true;
            textBoxEdenserver.ShortcutsEnabled = false;
            textBoxEdenserver.Size = new Size(166, 27);
            textBoxEdenserver.TabIndex = 0;
            // 
            // groupBoxSVO
            // 
            groupBoxSVO.Controls.Add(buttonStopSVO);
            groupBoxSVO.Controls.Add(buttonRestartSVO);
            groupBoxSVO.Controls.Add(buttonStartSVO);
            groupBoxSVO.Controls.Add(textBoxSVO);
            groupBoxSVO.Location = new Point(261, 474);
            groupBoxSVO.Margin = new Padding(3, 4, 3, 4);
            groupBoxSVO.Name = "groupBoxSVO";
            groupBoxSVO.Padding = new Padding(3, 4, 3, 4);
            groupBoxSVO.Size = new Size(181, 216);
            groupBoxSVO.TabIndex = 8;
            groupBoxSVO.TabStop = false;
            groupBoxSVO.Text = "SVO";
            // 
            // buttonStopSVO
            // 
            buttonStopSVO.Location = new Point(9, 163);
            buttonStopSVO.Margin = new Padding(3, 4, 3, 4);
            buttonStopSVO.Name = "buttonStopSVO";
            buttonStopSVO.Size = new Size(166, 37);
            buttonStopSVO.TabIndex = 3;
            buttonStopSVO.Text = "Stop!";
            buttonStopSVO.UseVisualStyleBackColor = true;
            buttonStopSVO.Click += buttonStopSVO_Click;
            // 
            // buttonRestartSVO
            // 
            buttonRestartSVO.Enabled = false;
            buttonRestartSVO.Location = new Point(9, 118);
            buttonRestartSVO.Margin = new Padding(3, 4, 3, 4);
            buttonRestartSVO.Name = "buttonRestartSVO";
            buttonRestartSVO.Size = new Size(166, 37);
            buttonRestartSVO.TabIndex = 2;
            buttonRestartSVO.Text = "Restart!";
            buttonRestartSVO.UseVisualStyleBackColor = true;
            // 
            // buttonStartSVO
            // 
            buttonStartSVO.Location = new Point(7, 73);
            buttonStartSVO.Margin = new Padding(3, 4, 3, 4);
            buttonStartSVO.Name = "buttonStartSVO";
            buttonStartSVO.Size = new Size(166, 37);
            buttonStartSVO.TabIndex = 1;
            buttonStartSVO.Text = "Start!";
            buttonStartSVO.UseVisualStyleBackColor = true;
            buttonStartSVO.Click += buttonStartSVO_Click;
            // 
            // textBoxSVO
            // 
            textBoxSVO.Location = new Point(7, 29);
            textBoxSVO.Margin = new Padding(3, 4, 3, 4);
            textBoxSVO.Name = "textBoxSVO";
            textBoxSVO.ReadOnly = true;
            textBoxSVO.ShortcutsEnabled = false;
            textBoxSVO.Size = new Size(166, 27);
            textBoxSVO.TabIndex = 0;
            // 
            // groupBoxSSFWServer
            // 
            groupBoxSSFWServer.Controls.Add(buttonStopSSFWServer);
            groupBoxSSFWServer.Controls.Add(buttonRestartSSFWServer);
            groupBoxSSFWServer.Controls.Add(buttonStartSSFWServer);
            groupBoxSSFWServer.Controls.Add(textBoxSSFWServer);
            groupBoxSSFWServer.Location = new Point(29, 474);
            groupBoxSSFWServer.Margin = new Padding(3, 4, 3, 4);
            groupBoxSSFWServer.Name = "groupBoxSSFWServer";
            groupBoxSSFWServer.Padding = new Padding(3, 4, 3, 4);
            groupBoxSSFWServer.Size = new Size(181, 216);
            groupBoxSSFWServer.TabIndex = 7;
            groupBoxSSFWServer.TabStop = false;
            groupBoxSSFWServer.Text = "SSFWServer";
            // 
            // buttonStopSSFWServer
            // 
            buttonStopSSFWServer.Location = new Point(9, 163);
            buttonStopSSFWServer.Margin = new Padding(3, 4, 3, 4);
            buttonStopSSFWServer.Name = "buttonStopSSFWServer";
            buttonStopSSFWServer.Size = new Size(166, 37);
            buttonStopSSFWServer.TabIndex = 3;
            buttonStopSSFWServer.Text = "Stop!";
            buttonStopSSFWServer.UseVisualStyleBackColor = true;
            buttonStopSSFWServer.Click += buttonStopSSFWServer_Click;
            // 
            // buttonRestartSSFWServer
            // 
            buttonRestartSSFWServer.Enabled = false;
            buttonRestartSSFWServer.Location = new Point(8, 118);
            buttonRestartSSFWServer.Margin = new Padding(3, 4, 3, 4);
            buttonRestartSSFWServer.Name = "buttonRestartSSFWServer";
            buttonRestartSSFWServer.Size = new Size(166, 37);
            buttonRestartSSFWServer.TabIndex = 2;
            buttonRestartSSFWServer.Text = "Restart!";
            buttonRestartSSFWServer.UseVisualStyleBackColor = true;
            // 
            // buttonStartSSFWServer
            // 
            buttonStartSSFWServer.Location = new Point(7, 73);
            buttonStartSSFWServer.Margin = new Padding(3, 4, 3, 4);
            buttonStartSSFWServer.Name = "buttonStartSSFWServer";
            buttonStartSSFWServer.Size = new Size(166, 37);
            buttonStartSSFWServer.TabIndex = 1;
            buttonStartSSFWServer.Text = "Start!";
            buttonStartSSFWServer.UseVisualStyleBackColor = true;
            buttonStartSSFWServer.Click += buttonStartSSFWServer_Click;
            // 
            // textBoxSSFWServer
            // 
            textBoxSSFWServer.Location = new Point(7, 29);
            textBoxSSFWServer.Margin = new Padding(3, 4, 3, 4);
            textBoxSSFWServer.Name = "textBoxSSFWServer";
            textBoxSSFWServer.ReadOnly = true;
            textBoxSSFWServer.ShortcutsEnabled = false;
            textBoxSSFWServer.Size = new Size(166, 27);
            textBoxSSFWServer.TabIndex = 0;
            // 
            // groupBoxQuazalserver
            // 
            groupBoxQuazalserver.Controls.Add(buttonStopQuazalserver);
            groupBoxQuazalserver.Controls.Add(buttonRestartQuazalserver);
            groupBoxQuazalserver.Controls.Add(buttonStartQuazalserver);
            groupBoxQuazalserver.Controls.Add(textBoxQuazalserver);
            groupBoxQuazalserver.Location = new Point(489, 252);
            groupBoxQuazalserver.Margin = new Padding(3, 4, 3, 4);
            groupBoxQuazalserver.Name = "groupBoxQuazalserver";
            groupBoxQuazalserver.Padding = new Padding(3, 4, 3, 4);
            groupBoxQuazalserver.Size = new Size(181, 216);
            groupBoxQuazalserver.TabIndex = 6;
            groupBoxQuazalserver.TabStop = false;
            groupBoxQuazalserver.Text = "QuazalServer";
            // 
            // buttonStopQuazalserver
            // 
            buttonStopQuazalserver.Location = new Point(8, 163);
            buttonStopQuazalserver.Margin = new Padding(3, 4, 3, 4);
            buttonStopQuazalserver.Name = "buttonStopQuazalserver";
            buttonStopQuazalserver.Size = new Size(166, 37);
            buttonStopQuazalserver.TabIndex = 3;
            buttonStopQuazalserver.Text = "Stop!";
            buttonStopQuazalserver.UseVisualStyleBackColor = true;
            buttonStopQuazalserver.Click += buttonStopQuazalserver_Click;
            // 
            // buttonRestartQuazalserver
            // 
            buttonRestartQuazalserver.Enabled = false;
            buttonRestartQuazalserver.Location = new Point(8, 118);
            buttonRestartQuazalserver.Margin = new Padding(3, 4, 3, 4);
            buttonRestartQuazalserver.Name = "buttonRestartQuazalserver";
            buttonRestartQuazalserver.Size = new Size(166, 37);
            buttonRestartQuazalserver.TabIndex = 2;
            buttonRestartQuazalserver.Text = "Restart!";
            buttonRestartQuazalserver.UseVisualStyleBackColor = true;
            // 
            // buttonStartQuazalserver
            // 
            buttonStartQuazalserver.Location = new Point(7, 73);
            buttonStartQuazalserver.Margin = new Padding(3, 4, 3, 4);
            buttonStartQuazalserver.Name = "buttonStartQuazalserver";
            buttonStartQuazalserver.Size = new Size(166, 37);
            buttonStartQuazalserver.TabIndex = 1;
            buttonStartQuazalserver.Text = "Start!";
            buttonStartQuazalserver.UseVisualStyleBackColor = true;
            buttonStartQuazalserver.Click += buttonStartQuazalserver_Click;
            // 
            // textBoxQuazalserver
            // 
            textBoxQuazalserver.Location = new Point(7, 29);
            textBoxQuazalserver.Margin = new Padding(3, 4, 3, 4);
            textBoxQuazalserver.Name = "textBoxQuazalserver";
            textBoxQuazalserver.ReadOnly = true;
            textBoxQuazalserver.ShortcutsEnabled = false;
            textBoxQuazalserver.Size = new Size(166, 27);
            textBoxQuazalserver.TabIndex = 0;
            // 
            // groupBoxMultispy
            // 
            groupBoxMultispy.Controls.Add(buttonStopMultispy);
            groupBoxMultispy.Controls.Add(buttonRestartMultispy);
            groupBoxMultispy.Controls.Add(buttonStartMultispy);
            groupBoxMultispy.Controls.Add(textBoxMultispy);
            groupBoxMultispy.Location = new Point(261, 252);
            groupBoxMultispy.Margin = new Padding(3, 4, 3, 4);
            groupBoxMultispy.Name = "groupBoxMultispy";
            groupBoxMultispy.Padding = new Padding(3, 4, 3, 4);
            groupBoxMultispy.Size = new Size(181, 214);
            groupBoxMultispy.TabIndex = 5;
            groupBoxMultispy.TabStop = false;
            groupBoxMultispy.Text = "MultiSpy";
            // 
            // buttonStopMultispy
            // 
            buttonStopMultispy.Location = new Point(9, 164);
            buttonStopMultispy.Margin = new Padding(3, 4, 3, 4);
            buttonStopMultispy.Name = "buttonStopMultispy";
            buttonStopMultispy.Size = new Size(166, 37);
            buttonStopMultispy.TabIndex = 3;
            buttonStopMultispy.Text = "Stop!";
            buttonStopMultispy.UseVisualStyleBackColor = true;
            buttonStopMultispy.Click += buttonStopMultispy_Click;
            // 
            // buttonRestartMultispy
            // 
            buttonRestartMultispy.Enabled = false;
            buttonRestartMultispy.Location = new Point(9, 119);
            buttonRestartMultispy.Margin = new Padding(3, 4, 3, 4);
            buttonRestartMultispy.Name = "buttonRestartMultispy";
            buttonRestartMultispy.Size = new Size(166, 37);
            buttonRestartMultispy.TabIndex = 2;
            buttonRestartMultispy.Text = "Restart!";
            buttonRestartMultispy.UseVisualStyleBackColor = true;
            // 
            // buttonStartMultispy
            // 
            buttonStartMultispy.Location = new Point(9, 74);
            buttonStartMultispy.Margin = new Padding(3, 4, 3, 4);
            buttonStartMultispy.Name = "buttonStartMultispy";
            buttonStartMultispy.Size = new Size(166, 37);
            buttonStartMultispy.TabIndex = 1;
            buttonStartMultispy.Text = "Start!";
            buttonStartMultispy.UseVisualStyleBackColor = true;
            buttonStartMultispy.Click += buttonStartMultispy_Click;
            // 
            // textBoxMultispy
            // 
            textBoxMultispy.Location = new Point(7, 29);
            textBoxMultispy.Margin = new Padding(3, 4, 3, 4);
            textBoxMultispy.Name = "textBoxMultispy";
            textBoxMultispy.ReadOnly = true;
            textBoxMultispy.ShortcutsEnabled = false;
            textBoxMultispy.Size = new Size(166, 27);
            textBoxMultispy.TabIndex = 0;
            // 
            // groupBoxMultisocks
            // 
            groupBoxMultisocks.Controls.Add(buttonStopMultisocks);
            groupBoxMultisocks.Controls.Add(buttonRestartMultisocks);
            groupBoxMultisocks.Controls.Add(buttonStartMultisocks);
            groupBoxMultisocks.Controls.Add(textBoxMultisocks);
            groupBoxMultisocks.Location = new Point(29, 252);
            groupBoxMultisocks.Margin = new Padding(3, 4, 3, 4);
            groupBoxMultisocks.Name = "groupBoxMultisocks";
            groupBoxMultisocks.Padding = new Padding(3, 4, 3, 4);
            groupBoxMultisocks.Size = new Size(181, 214);
            groupBoxMultisocks.TabIndex = 4;
            groupBoxMultisocks.TabStop = false;
            groupBoxMultisocks.Text = "MultiSocks";
            // 
            // buttonStopMultisocks
            // 
            buttonStopMultisocks.Location = new Point(8, 164);
            buttonStopMultisocks.Margin = new Padding(3, 4, 3, 4);
            buttonStopMultisocks.Name = "buttonStopMultisocks";
            buttonStopMultisocks.Size = new Size(166, 37);
            buttonStopMultisocks.TabIndex = 3;
            buttonStopMultisocks.Text = "Stop!";
            buttonStopMultisocks.UseVisualStyleBackColor = true;
            buttonStopMultisocks.Click += buttonStopMultisocks_Click;
            // 
            // buttonRestartMultisocks
            // 
            buttonRestartMultisocks.Enabled = false;
            buttonRestartMultisocks.Location = new Point(8, 119);
            buttonRestartMultisocks.Margin = new Padding(3, 4, 3, 4);
            buttonRestartMultisocks.Name = "buttonRestartMultisocks";
            buttonRestartMultisocks.Size = new Size(166, 37);
            buttonRestartMultisocks.TabIndex = 2;
            buttonRestartMultisocks.Text = "Restart!";
            buttonRestartMultisocks.UseVisualStyleBackColor = true;
            // 
            // buttonStartMultisocks
            // 
            buttonStartMultisocks.Location = new Point(8, 74);
            buttonStartMultisocks.Margin = new Padding(3, 4, 3, 4);
            buttonStartMultisocks.Name = "buttonStartMultisocks";
            buttonStartMultisocks.Size = new Size(166, 37);
            buttonStartMultisocks.TabIndex = 1;
            buttonStartMultisocks.Text = "Start!";
            buttonStartMultisocks.UseVisualStyleBackColor = true;
            buttonStartMultisocks.Click += buttonStartMultisocks_Click;
            // 
            // textBoxMultisocks
            // 
            textBoxMultisocks.Location = new Point(7, 29);
            textBoxMultisocks.Margin = new Padding(3, 4, 3, 4);
            textBoxMultisocks.Name = "textBoxMultisocks";
            textBoxMultisocks.ReadOnly = true;
            textBoxMultisocks.ShortcutsEnabled = false;
            textBoxMultisocks.Size = new Size(166, 27);
            textBoxMultisocks.TabIndex = 0;
            // 
            // groupBoxHorizon
            // 
            groupBoxHorizon.Controls.Add(buttonStopHorizon);
            groupBoxHorizon.Controls.Add(buttonRestartHorizon);
            groupBoxHorizon.Controls.Add(buttonStartHorizon);
            groupBoxHorizon.Controls.Add(textBoxHorizon);
            groupBoxHorizon.Location = new Point(489, 30);
            groupBoxHorizon.Margin = new Padding(3, 4, 3, 4);
            groupBoxHorizon.Name = "groupBoxHorizon";
            groupBoxHorizon.Padding = new Padding(3, 4, 3, 4);
            groupBoxHorizon.Size = new Size(181, 214);
            groupBoxHorizon.TabIndex = 3;
            groupBoxHorizon.TabStop = false;
            groupBoxHorizon.Text = "Horizon";
            // 
            // buttonStopHorizon
            // 
            buttonStopHorizon.Location = new Point(7, 164);
            buttonStopHorizon.Margin = new Padding(3, 4, 3, 4);
            buttonStopHorizon.Name = "buttonStopHorizon";
            buttonStopHorizon.Size = new Size(166, 37);
            buttonStopHorizon.TabIndex = 3;
            buttonStopHorizon.Text = "Stop!";
            buttonStopHorizon.UseVisualStyleBackColor = true;
            buttonStopHorizon.Click += buttonStopHorizon_Click;
            // 
            // buttonRestartHorizon
            // 
            buttonRestartHorizon.Enabled = false;
            buttonRestartHorizon.Location = new Point(7, 119);
            buttonRestartHorizon.Margin = new Padding(3, 4, 3, 4);
            buttonRestartHorizon.Name = "buttonRestartHorizon";
            buttonRestartHorizon.Size = new Size(166, 37);
            buttonRestartHorizon.TabIndex = 2;
            buttonRestartHorizon.Text = "Restart!";
            buttonRestartHorizon.UseVisualStyleBackColor = true;
            // 
            // buttonStartHorizon
            // 
            buttonStartHorizon.Location = new Point(8, 74);
            buttonStartHorizon.Margin = new Padding(3, 4, 3, 4);
            buttonStartHorizon.Name = "buttonStartHorizon";
            buttonStartHorizon.Size = new Size(166, 37);
            buttonStartHorizon.TabIndex = 1;
            buttonStartHorizon.Text = "Start!";
            buttonStartHorizon.UseVisualStyleBackColor = true;
            buttonStartHorizon.Click += buttonStartHorizon_Click;
            // 
            // textBoxHorizon
            // 
            textBoxHorizon.Location = new Point(7, 29);
            textBoxHorizon.Margin = new Padding(3, 4, 3, 4);
            textBoxHorizon.Name = "textBoxHorizon";
            textBoxHorizon.ReadOnly = true;
            textBoxHorizon.ShortcutsEnabled = false;
            textBoxHorizon.Size = new Size(166, 27);
            textBoxHorizon.TabIndex = 0;
            // 
            // groupBoxDNS
            // 
            groupBoxDNS.Controls.Add(buttonStopDNS);
            groupBoxDNS.Controls.Add(buttonRestartDNS);
            groupBoxDNS.Controls.Add(buttonStartDNS);
            groupBoxDNS.Controls.Add(textBoxDNS);
            groupBoxDNS.Location = new Point(261, 28);
            groupBoxDNS.Margin = new Padding(3, 4, 3, 4);
            groupBoxDNS.Name = "groupBoxDNS";
            groupBoxDNS.Padding = new Padding(3, 4, 3, 4);
            groupBoxDNS.Size = new Size(181, 216);
            groupBoxDNS.TabIndex = 2;
            groupBoxDNS.TabStop = false;
            groupBoxDNS.Text = "DNS";
            // 
            // buttonStopDNS
            // 
            buttonStopDNS.Location = new Point(7, 164);
            buttonStopDNS.Margin = new Padding(3, 4, 3, 4);
            buttonStopDNS.Name = "buttonStopDNS";
            buttonStopDNS.Size = new Size(166, 37);
            buttonStopDNS.TabIndex = 3;
            buttonStopDNS.Text = "Stop!";
            buttonStopDNS.UseVisualStyleBackColor = true;
            buttonStopDNS.Click += buttonStopDNS_Click;
            // 
            // buttonRestartDNS
            // 
            buttonRestartDNS.Enabled = false;
            buttonRestartDNS.Location = new Point(9, 119);
            buttonRestartDNS.Margin = new Padding(3, 4, 3, 4);
            buttonRestartDNS.Name = "buttonRestartDNS";
            buttonRestartDNS.Size = new Size(166, 37);
            buttonRestartDNS.TabIndex = 2;
            buttonRestartDNS.Text = "Restart!";
            buttonRestartDNS.UseVisualStyleBackColor = true;
            // 
            // buttonStartDNS
            // 
            buttonStartDNS.Location = new Point(9, 74);
            buttonStartDNS.Margin = new Padding(3, 4, 3, 4);
            buttonStartDNS.Name = "buttonStartDNS";
            buttonStartDNS.Size = new Size(166, 37);
            buttonStartDNS.TabIndex = 1;
            buttonStartDNS.Text = "Start!";
            buttonStartDNS.UseVisualStyleBackColor = true;
            buttonStartDNS.Click += buttonStartDNS_Click;
            // 
            // textBoxDNS
            // 
            textBoxDNS.Location = new Point(7, 29);
            textBoxDNS.Margin = new Padding(3, 4, 3, 4);
            textBoxDNS.Name = "textBoxDNS";
            textBoxDNS.ReadOnly = true;
            textBoxDNS.ShortcutsEnabled = false;
            textBoxDNS.Size = new Size(166, 27);
            textBoxDNS.TabIndex = 0;
            // 
            // groupBoxHTTP
            // 
            groupBoxHTTP.Controls.Add(buttonStopHTTP);
            groupBoxHTTP.Controls.Add(buttonRestartHTTP);
            groupBoxHTTP.Controls.Add(buttonStartHTTP);
            groupBoxHTTP.Controls.Add(textBoxHTTP);
            groupBoxHTTP.Location = new Point(29, 28);
            groupBoxHTTP.Margin = new Padding(3, 4, 3, 4);
            groupBoxHTTP.Name = "groupBoxHTTP";
            groupBoxHTTP.Padding = new Padding(3, 4, 3, 4);
            groupBoxHTTP.Size = new Size(181, 216);
            groupBoxHTTP.TabIndex = 0;
            groupBoxHTTP.TabStop = false;
            groupBoxHTTP.Text = "ApacheNet";
            // 
            // buttonStopHTTP
            // 
            buttonStopHTTP.Location = new Point(7, 164);
            buttonStopHTTP.Margin = new Padding(3, 4, 3, 4);
            buttonStopHTTP.Name = "buttonStopHTTP";
            buttonStopHTTP.Size = new Size(166, 37);
            buttonStopHTTP.TabIndex = 3;
            buttonStopHTTP.Text = "Stop!";
            buttonStopHTTP.UseVisualStyleBackColor = true;
            buttonStopHTTP.Click += buttonStopHTTP_Click;
            // 
            // buttonRestartHTTP
            // 
            buttonRestartHTTP.Enabled = false;
            buttonRestartHTTP.Location = new Point(8, 119);
            buttonRestartHTTP.Margin = new Padding(3, 4, 3, 4);
            buttonRestartHTTP.Name = "buttonRestartHTTP";
            buttonRestartHTTP.Size = new Size(166, 37);
            buttonRestartHTTP.TabIndex = 2;
            buttonRestartHTTP.Text = "Restart!";
            buttonRestartHTTP.UseVisualStyleBackColor = true;
            // 
            // buttonStartHTTP
            // 
            buttonStartHTTP.Location = new Point(8, 74);
            buttonStartHTTP.Margin = new Padding(3, 4, 3, 4);
            buttonStartHTTP.Name = "buttonStartHTTP";
            buttonStartHTTP.Size = new Size(166, 37);
            buttonStartHTTP.TabIndex = 1;
            buttonStartHTTP.Text = "Start!";
            buttonStartHTTP.UseVisualStyleBackColor = true;
            buttonStartHTTP.Click += buttonStartHTTP_Click;
            // 
            // textBoxHTTP
            // 
            textBoxHTTP.Location = new Point(7, 29);
            textBoxHTTP.Margin = new Padding(3, 4, 3, 4);
            textBoxHTTP.Name = "textBoxHTTP";
            textBoxHTTP.ReadOnly = true;
            textBoxHTTP.ShortcutsEnabled = false;
            textBoxHTTP.Size = new Size(166, 27);
            textBoxHTTP.TabIndex = 0;
            // 
            // pictureBoxMainLogo
            // 
            pictureBoxMainLogo.Image = Properties.Resources.MultiServer;
            pictureBoxMainLogo.Location = new Point(0, 0);
            pictureBoxMainLogo.Margin = new Padding(3, 4, 3, 4);
            pictureBoxMainLogo.Name = "pictureBoxMainLogo";
            pictureBoxMainLogo.Size = new Size(697, 594);
            pictureBoxMainLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxMainLogo.TabIndex = 0;
            pictureBoxMainLogo.TabStop = false;
            // 
            // tabPageHTTP
            // 
            tabPageHTTP.Controls.Add(richTextBoxHTTPLog);
            tabPageHTTP.Location = new Point(4, 29);
            tabPageHTTP.Margin = new Padding(3, 4, 3, 4);
            tabPageHTTP.Name = "tabPageHTTP";
            tabPageHTTP.Padding = new Padding(3, 4, 3, 4);
            tabPageHTTP.Size = new Size(1414, 935);
            tabPageHTTP.TabIndex = 1;
            tabPageHTTP.Text = "ApacheNet";
            tabPageHTTP.UseVisualStyleBackColor = true;
            // 
            // richTextBoxHTTPLog
            // 
            richTextBoxHTTPLog.Location = new Point(0, 0);
            richTextBoxHTTPLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxHTTPLog.Name = "richTextBoxHTTPLog";
            richTextBoxHTTPLog.ReadOnly = true;
            richTextBoxHTTPLog.Size = new Size(1414, 935);
            richTextBoxHTTPLog.TabIndex = 0;
            richTextBoxHTTPLog.Text = "";
            // 
            // tabPageDNS
            // 
            tabPageDNS.Controls.Add(richTextBoxDNSLog);
            tabPageDNS.Location = new Point(4, 29);
            tabPageDNS.Margin = new Padding(3, 4, 3, 4);
            tabPageDNS.Name = "tabPageDNS";
            tabPageDNS.Padding = new Padding(3, 4, 3, 4);
            tabPageDNS.Size = new Size(1414, 935);
            tabPageDNS.TabIndex = 3;
            tabPageDNS.Text = "MitmDNS";
            tabPageDNS.UseVisualStyleBackColor = true;
            // 
            // richTextBoxDNSLog
            // 
            richTextBoxDNSLog.Location = new Point(0, 0);
            richTextBoxDNSLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxDNSLog.Name = "richTextBoxDNSLog";
            richTextBoxDNSLog.ReadOnly = true;
            richTextBoxDNSLog.Size = new Size(1414, 935);
            richTextBoxDNSLog.TabIndex = 2;
            richTextBoxDNSLog.Text = "";
            // 
            // tabPageHorizon
            // 
            tabPageHorizon.Controls.Add(richTextBoxHorizonLog);
            tabPageHorizon.Location = new Point(4, 29);
            tabPageHorizon.Name = "tabPageHorizon";
            tabPageHorizon.Padding = new Padding(3);
            tabPageHorizon.Size = new Size(1414, 935);
            tabPageHorizon.TabIndex = 5;
            tabPageHorizon.Text = "Horizon";
            tabPageHorizon.UseVisualStyleBackColor = true;
            // 
            // richTextBoxHorizonLog
            // 
            richTextBoxHorizonLog.Location = new Point(0, 0);
            richTextBoxHorizonLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxHorizonLog.Name = "richTextBoxHorizonLog";
            richTextBoxHorizonLog.ReadOnly = true;
            richTextBoxHorizonLog.Size = new Size(1414, 935);
            richTextBoxHorizonLog.TabIndex = 3;
            richTextBoxHorizonLog.Text = "";
            // 
            // tabPageMultisocks
            // 
            tabPageMultisocks.Controls.Add(richTextBoxMultisocksLog);
            tabPageMultisocks.Location = new Point(4, 29);
            tabPageMultisocks.Name = "tabPageMultisocks";
            tabPageMultisocks.Padding = new Padding(3);
            tabPageMultisocks.Size = new Size(1414, 935);
            tabPageMultisocks.TabIndex = 6;
            tabPageMultisocks.Text = "MultiSocks";
            tabPageMultisocks.UseVisualStyleBackColor = true;
            // 
            // richTextBoxMultisocksLog
            // 
            richTextBoxMultisocksLog.Location = new Point(0, 0);
            richTextBoxMultisocksLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxMultisocksLog.Name = "richTextBoxMultisocksLog";
            richTextBoxMultisocksLog.ReadOnly = true;
            richTextBoxMultisocksLog.Size = new Size(1414, 935);
            richTextBoxMultisocksLog.TabIndex = 4;
            richTextBoxMultisocksLog.Text = "";
            // 
            // tabPageMultispy
            // 
            tabPageMultispy.Controls.Add(richTextBoxMultispyLog);
            tabPageMultispy.Location = new Point(4, 29);
            tabPageMultispy.Name = "tabPageMultispy";
            tabPageMultispy.Padding = new Padding(3);
            tabPageMultispy.Size = new Size(1414, 935);
            tabPageMultispy.TabIndex = 7;
            tabPageMultispy.Text = "MultiSpy";
            tabPageMultispy.UseVisualStyleBackColor = true;
            // 
            // richTextBoxMultispyLog
            // 
            richTextBoxMultispyLog.Location = new Point(0, 0);
            richTextBoxMultispyLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxMultispyLog.Name = "richTextBoxMultispyLog";
            richTextBoxMultispyLog.ReadOnly = true;
            richTextBoxMultispyLog.Size = new Size(1414, 935);
            richTextBoxMultispyLog.TabIndex = 5;
            richTextBoxMultispyLog.Text = "";
            // 
            // tabPageQuazalserver
            // 
            tabPageQuazalserver.Controls.Add(richTextBoxQuazalserverLog);
            tabPageQuazalserver.Location = new Point(4, 29);
            tabPageQuazalserver.Name = "tabPageQuazalserver";
            tabPageQuazalserver.Padding = new Padding(3);
            tabPageQuazalserver.Size = new Size(1414, 935);
            tabPageQuazalserver.TabIndex = 8;
            tabPageQuazalserver.Text = "QuazalServer";
            tabPageQuazalserver.UseVisualStyleBackColor = true;
            // 
            // richTextBoxQuazalserverLog
            // 
            richTextBoxQuazalserverLog.Location = new Point(0, 0);
            richTextBoxQuazalserverLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxQuazalserverLog.Name = "richTextBoxQuazalserverLog";
            richTextBoxQuazalserverLog.ReadOnly = true;
            richTextBoxQuazalserverLog.Size = new Size(1414, 935);
            richTextBoxQuazalserverLog.TabIndex = 6;
            richTextBoxQuazalserverLog.Text = "";
            // 
            // tabPageSSFWServer
            // 
            tabPageSSFWServer.Controls.Add(richTextBoxSSFWServerLog);
            tabPageSSFWServer.Location = new Point(4, 29);
            tabPageSSFWServer.Name = "tabPageSSFWServer";
            tabPageSSFWServer.Padding = new Padding(3);
            tabPageSSFWServer.Size = new Size(1414, 935);
            tabPageSSFWServer.TabIndex = 9;
            tabPageSSFWServer.Text = "SSFWServer";
            tabPageSSFWServer.UseVisualStyleBackColor = true;
            // 
            // richTextBoxSSFWServerLog
            // 
            richTextBoxSSFWServerLog.Location = new Point(0, 0);
            richTextBoxSSFWServerLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxSSFWServerLog.Name = "richTextBoxSSFWServerLog";
            richTextBoxSSFWServerLog.ReadOnly = true;
            richTextBoxSSFWServerLog.Size = new Size(1414, 935);
            richTextBoxSSFWServerLog.TabIndex = 7;
            richTextBoxSSFWServerLog.Text = "";
            // 
            // tabPageSVO
            // 
            tabPageSVO.Controls.Add(richTextBoxSVOLog);
            tabPageSVO.Location = new Point(4, 29);
            tabPageSVO.Name = "tabPageSVO";
            tabPageSVO.Padding = new Padding(3);
            tabPageSVO.Size = new Size(1414, 935);
            tabPageSVO.TabIndex = 10;
            tabPageSVO.Text = "SVO";
            tabPageSVO.UseVisualStyleBackColor = true;
            // 
            // richTextBoxSVOLog
            // 
            richTextBoxSVOLog.Location = new Point(0, 0);
            richTextBoxSVOLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxSVOLog.Name = "richTextBoxSVOLog";
            richTextBoxSVOLog.ReadOnly = true;
            richTextBoxSVOLog.Size = new Size(1414, 935);
            richTextBoxSVOLog.TabIndex = 8;
            richTextBoxSVOLog.Text = "";
            // 
            // tabPageEdenserver
            // 
            tabPageEdenserver.Controls.Add(richTextBoxEdenserverLog);
            tabPageEdenserver.Location = new Point(4, 29);
            tabPageEdenserver.Name = "tabPageEdenserver";
            tabPageEdenserver.Padding = new Padding(3);
            tabPageEdenserver.Size = new Size(1414, 935);
            tabPageEdenserver.TabIndex = 11;
            tabPageEdenserver.Text = "EdenServer";
            tabPageEdenserver.UseVisualStyleBackColor = true;
            // 
            // richTextBoxEdenserverLog
            // 
            richTextBoxEdenserverLog.Location = new Point(0, 0);
            richTextBoxEdenserverLog.Margin = new Padding(3, 4, 3, 4);
            richTextBoxEdenserverLog.Name = "richTextBoxEdenserverLog";
            richTextBoxEdenserverLog.ReadOnly = true;
            richTextBoxEdenserverLog.Size = new Size(1414, 935);
            richTextBoxEdenserverLog.TabIndex = 9;
            richTextBoxEdenserverLog.Text = "";
            // 
            // tabPageSettings
            // 
            tabPageSettings.Controls.Add(groupBoxAuxConfigFiles);
            tabPageSettings.Controls.Add(groupBoxConfigFiles);
            tabPageSettings.Controls.Add(richTextBoxLicense);
            tabPageSettings.Controls.Add(groupBoxServersPath);
            tabPageSettings.Location = new Point(4, 29);
            tabPageSettings.Margin = new Padding(3, 4, 3, 4);
            tabPageSettings.Name = "tabPageSettings";
            tabPageSettings.Padding = new Padding(3, 4, 3, 4);
            tabPageSettings.Size = new Size(1414, 935);
            tabPageSettings.TabIndex = 4;
            tabPageSettings.Text = "Settings";
            tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxAuxConfigFiles
            // 
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureAriesDatabase);
            groupBoxAuxConfigFiles.Controls.Add(textBoxAriesDatabaseJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(labelMultiSocksAux);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureHorizonDatabase);
            groupBoxAuxConfigFiles.Controls.Add(textBoxHorizonDatabaseJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureEbootDefs);
            groupBoxAuxConfigFiles.Controls.Add(textBoxEbootDefsJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureMUIS);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureDME);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureMedius);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureBwps);
            groupBoxAuxConfigFiles.Controls.Add(buttonConfigureNat);
            groupBoxAuxConfigFiles.Controls.Add(textBoxMUISJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(textBoxDMEJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(textBoxMediusJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(textBoxBwpsJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(textBoxNatJsonPath);
            groupBoxAuxConfigFiles.Controls.Add(labelMediusAux);
            groupBoxAuxConfigFiles.Location = new Point(707, 547);
            groupBoxAuxConfigFiles.Name = "groupBoxAuxConfigFiles";
            groupBoxAuxConfigFiles.Size = new Size(704, 385);
            groupBoxAuxConfigFiles.TabIndex = 3;
            groupBoxAuxConfigFiles.TabStop = false;
            groupBoxAuxConfigFiles.Text = "Auxiliary Configuration files";
            // 
            // buttonConfigureAriesDatabase
            // 
            buttonConfigureAriesDatabase.Location = new Point(543, 332);
            buttonConfigureAriesDatabase.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureAriesDatabase.Name = "buttonConfigureAriesDatabase";
            buttonConfigureAriesDatabase.Size = new Size(139, 32);
            buttonConfigureAriesDatabase.TabIndex = 59;
            buttonConfigureAriesDatabase.Text = "Edit!";
            buttonConfigureAriesDatabase.UseVisualStyleBackColor = true;
            buttonConfigureAriesDatabase.Click += buttonConfigureAriesDatabase_Click;
            // 
            // textBoxAriesDatabaseJsonPath
            // 
            textBoxAriesDatabaseJsonPath.Location = new Point(6, 337);
            textBoxAriesDatabaseJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxAriesDatabaseJsonPath.Name = "textBoxAriesDatabaseJsonPath";
            textBoxAriesDatabaseJsonPath.ReadOnly = true;
            textBoxAriesDatabaseJsonPath.Size = new Size(514, 27);
            textBoxAriesDatabaseJsonPath.TabIndex = 58;
            // 
            // labelMultiSocksAux
            // 
            labelMultiSocksAux.AutoSize = true;
            labelMultiSocksAux.Location = new Point(6, 313);
            labelMultiSocksAux.Name = "labelMultiSocksAux";
            labelMultiSocksAux.Size = new Size(80, 20);
            labelMultiSocksAux.TabIndex = 57;
            labelMultiSocksAux.Text = "MultiSocks";
            // 
            // buttonConfigureHorizonDatabase
            // 
            buttonConfigureHorizonDatabase.Location = new Point(543, 263);
            buttonConfigureHorizonDatabase.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureHorizonDatabase.Name = "buttonConfigureHorizonDatabase";
            buttonConfigureHorizonDatabase.Size = new Size(139, 32);
            buttonConfigureHorizonDatabase.TabIndex = 56;
            buttonConfigureHorizonDatabase.Text = "Edit!";
            buttonConfigureHorizonDatabase.UseVisualStyleBackColor = true;
            buttonConfigureHorizonDatabase.Click += buttonConfigureHorizonDatabase_Click;
            // 
            // textBoxHorizonDatabaseJsonPath
            // 
            textBoxHorizonDatabaseJsonPath.Location = new Point(6, 266);
            textBoxHorizonDatabaseJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxHorizonDatabaseJsonPath.Name = "textBoxHorizonDatabaseJsonPath";
            textBoxHorizonDatabaseJsonPath.ReadOnly = true;
            textBoxHorizonDatabaseJsonPath.Size = new Size(514, 27);
            textBoxHorizonDatabaseJsonPath.TabIndex = 55;
            // 
            // buttonConfigureEbootDefs
            // 
            buttonConfigureEbootDefs.Location = new Point(543, 226);
            buttonConfigureEbootDefs.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureEbootDefs.Name = "buttonConfigureEbootDefs";
            buttonConfigureEbootDefs.Size = new Size(139, 32);
            buttonConfigureEbootDefs.TabIndex = 54;
            buttonConfigureEbootDefs.Text = "Edit!";
            buttonConfigureEbootDefs.UseVisualStyleBackColor = true;
            buttonConfigureEbootDefs.Click += buttonConfigureEbootDefs_Click;
            // 
            // textBoxEbootDefsJsonPath
            // 
            textBoxEbootDefsJsonPath.Location = new Point(6, 231);
            textBoxEbootDefsJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxEbootDefsJsonPath.Name = "textBoxEbootDefsJsonPath";
            textBoxEbootDefsJsonPath.ReadOnly = true;
            textBoxEbootDefsJsonPath.Size = new Size(514, 27);
            textBoxEbootDefsJsonPath.TabIndex = 53;
            // 
            // buttonConfigureMUIS
            // 
            buttonConfigureMUIS.Location = new Point(543, 191);
            buttonConfigureMUIS.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureMUIS.Name = "buttonConfigureMUIS";
            buttonConfigureMUIS.Size = new Size(139, 32);
            buttonConfigureMUIS.TabIndex = 52;
            buttonConfigureMUIS.Text = "Edit!";
            buttonConfigureMUIS.UseVisualStyleBackColor = true;
            buttonConfigureMUIS.Click += buttonConfigureMUIS_Click;
            // 
            // buttonConfigureDME
            // 
            buttonConfigureDME.Location = new Point(543, 158);
            buttonConfigureDME.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureDME.Name = "buttonConfigureDME";
            buttonConfigureDME.Size = new Size(139, 32);
            buttonConfigureDME.TabIndex = 51;
            buttonConfigureDME.Text = "Edit!";
            buttonConfigureDME.UseVisualStyleBackColor = true;
            buttonConfigureDME.Click += buttonConfigureDME_Click;
            // 
            // buttonConfigureMedius
            // 
            buttonConfigureMedius.Location = new Point(543, 123);
            buttonConfigureMedius.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureMedius.Name = "buttonConfigureMedius";
            buttonConfigureMedius.Size = new Size(139, 32);
            buttonConfigureMedius.TabIndex = 50;
            buttonConfigureMedius.Text = "Edit!";
            buttonConfigureMedius.UseVisualStyleBackColor = true;
            buttonConfigureMedius.Click += buttonConfigureMedius_Click;
            // 
            // buttonConfigureBwps
            // 
            buttonConfigureBwps.Location = new Point(543, 88);
            buttonConfigureBwps.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureBwps.Name = "buttonConfigureBwps";
            buttonConfigureBwps.Size = new Size(139, 32);
            buttonConfigureBwps.TabIndex = 49;
            buttonConfigureBwps.Text = "Edit!";
            buttonConfigureBwps.UseVisualStyleBackColor = true;
            buttonConfigureBwps.Click += buttonConfigureBwps_Click;
            // 
            // buttonConfigureNat
            // 
            buttonConfigureNat.Location = new Point(543, 53);
            buttonConfigureNat.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureNat.Name = "buttonConfigureNat";
            buttonConfigureNat.Size = new Size(139, 32);
            buttonConfigureNat.TabIndex = 48;
            buttonConfigureNat.Text = "Edit!";
            buttonConfigureNat.UseVisualStyleBackColor = true;
            buttonConfigureNat.Click += buttonConfigureNat_Click;
            // 
            // textBoxMUISJsonPath
            // 
            textBoxMUISJsonPath.Location = new Point(6, 196);
            textBoxMUISJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxMUISJsonPath.Name = "textBoxMUISJsonPath";
            textBoxMUISJsonPath.ReadOnly = true;
            textBoxMUISJsonPath.Size = new Size(514, 27);
            textBoxMUISJsonPath.TabIndex = 43;
            // 
            // textBoxDMEJsonPath
            // 
            textBoxDMEJsonPath.Location = new Point(6, 161);
            textBoxDMEJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxDMEJsonPath.Name = "textBoxDMEJsonPath";
            textBoxDMEJsonPath.ReadOnly = true;
            textBoxDMEJsonPath.Size = new Size(514, 27);
            textBoxDMEJsonPath.TabIndex = 42;
            // 
            // textBoxMediusJsonPath
            // 
            textBoxMediusJsonPath.Location = new Point(6, 126);
            textBoxMediusJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxMediusJsonPath.Name = "textBoxMediusJsonPath";
            textBoxMediusJsonPath.ReadOnly = true;
            textBoxMediusJsonPath.Size = new Size(514, 27);
            textBoxMediusJsonPath.TabIndex = 41;
            // 
            // textBoxBwpsJsonPath
            // 
            textBoxBwpsJsonPath.Location = new Point(6, 91);
            textBoxBwpsJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxBwpsJsonPath.Name = "textBoxBwpsJsonPath";
            textBoxBwpsJsonPath.ReadOnly = true;
            textBoxBwpsJsonPath.Size = new Size(514, 27);
            textBoxBwpsJsonPath.TabIndex = 40;
            // 
            // textBoxNatJsonPath
            // 
            textBoxNatJsonPath.Location = new Point(6, 56);
            textBoxNatJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxNatJsonPath.Name = "textBoxNatJsonPath";
            textBoxNatJsonPath.ReadOnly = true;
            textBoxNatJsonPath.Size = new Size(514, 27);
            textBoxNatJsonPath.TabIndex = 39;
            // 
            // labelMediusAux
            // 
            labelMediusAux.AutoSize = true;
            labelMediusAux.Location = new Point(6, 32);
            labelMediusAux.Name = "labelMediusAux";
            labelMediusAux.Size = new Size(57, 20);
            labelMediusAux.TabIndex = 12;
            labelMediusAux.Text = "Medius";
            // 
            // groupBoxConfigFiles
            // 
            groupBoxConfigFiles.Controls.Add(buttonConfigureEdenserver);
            groupBoxConfigFiles.Controls.Add(buttonConfigureSVO);
            groupBoxConfigFiles.Controls.Add(buttonConfigureSSFWServer);
            groupBoxConfigFiles.Controls.Add(buttonConfigureQuazalserver);
            groupBoxConfigFiles.Controls.Add(buttonConfigureMultispy);
            groupBoxConfigFiles.Controls.Add(buttonConfigureMultisocks);
            groupBoxConfigFiles.Controls.Add(buttonConfigureHorizon);
            groupBoxConfigFiles.Controls.Add(buttonConfigureDNS);
            groupBoxConfigFiles.Controls.Add(buttonConfigureApacheNet);
            groupBoxConfigFiles.Controls.Add(textBoxEdenserverJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxSVOJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxSSFWServerJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxQuazalserverJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxMultispyJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxMultisocksJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxHorizonJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxDNSJsonPath);
            groupBoxConfigFiles.Controls.Add(textBoxApacheNetJsonPath);
            groupBoxConfigFiles.Controls.Add(labelEdenserver1);
            groupBoxConfigFiles.Controls.Add(labelSVO1);
            groupBoxConfigFiles.Controls.Add(labelSSFWServer1);
            groupBoxConfigFiles.Controls.Add(labelQuazalserver1);
            groupBoxConfigFiles.Controls.Add(labelMultispy1);
            groupBoxConfigFiles.Controls.Add(labelMultisocks1);
            groupBoxConfigFiles.Controls.Add(labelHorizon1);
            groupBoxConfigFiles.Controls.Add(labelDNS1);
            groupBoxConfigFiles.Controls.Add(labelApacheNet1);
            groupBoxConfigFiles.Location = new Point(707, 0);
            groupBoxConfigFiles.Name = "groupBoxConfigFiles";
            groupBoxConfigFiles.Size = new Size(704, 541);
            groupBoxConfigFiles.TabIndex = 2;
            groupBoxConfigFiles.TabStop = false;
            groupBoxConfigFiles.Text = "Servers Configuration files";
            // 
            // buttonConfigureEdenserver
            // 
            buttonConfigureEdenserver.Location = new Point(543, 494);
            buttonConfigureEdenserver.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureEdenserver.Name = "buttonConfigureEdenserver";
            buttonConfigureEdenserver.Size = new Size(139, 32);
            buttonConfigureEdenserver.TabIndex = 47;
            buttonConfigureEdenserver.Text = "Edit!";
            buttonConfigureEdenserver.UseVisualStyleBackColor = true;
            buttonConfigureEdenserver.Click += buttonConfigureEdenserver_Click;
            // 
            // buttonConfigureSVO
            // 
            buttonConfigureSVO.Location = new Point(543, 439);
            buttonConfigureSVO.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureSVO.Name = "buttonConfigureSVO";
            buttonConfigureSVO.Size = new Size(139, 32);
            buttonConfigureSVO.TabIndex = 46;
            buttonConfigureSVO.Text = "Edit!";
            buttonConfigureSVO.UseVisualStyleBackColor = true;
            buttonConfigureSVO.Click += buttonConfigureSVO_Click;
            // 
            // buttonConfigureSSFWServer
            // 
            buttonConfigureSSFWServer.Location = new Point(543, 384);
            buttonConfigureSSFWServer.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureSSFWServer.Name = "buttonConfigureSSFWServer";
            buttonConfigureSSFWServer.Size = new Size(139, 32);
            buttonConfigureSSFWServer.TabIndex = 45;
            buttonConfigureSSFWServer.Text = "Edit!";
            buttonConfigureSSFWServer.UseVisualStyleBackColor = true;
            buttonConfigureSSFWServer.Click += buttonConfigureSSFWServer_Click;
            // 
            // buttonConfigureQuazalserver
            // 
            buttonConfigureQuazalserver.Location = new Point(543, 329);
            buttonConfigureQuazalserver.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureQuazalserver.Name = "buttonConfigureQuazalserver";
            buttonConfigureQuazalserver.Size = new Size(139, 32);
            buttonConfigureQuazalserver.TabIndex = 44;
            buttonConfigureQuazalserver.Text = "Edit!";
            buttonConfigureQuazalserver.UseVisualStyleBackColor = true;
            buttonConfigureQuazalserver.Click += buttonConfigureQuazalserver_Click;
            // 
            // buttonConfigureMultispy
            // 
            buttonConfigureMultispy.Location = new Point(543, 274);
            buttonConfigureMultispy.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureMultispy.Name = "buttonConfigureMultispy";
            buttonConfigureMultispy.Size = new Size(139, 32);
            buttonConfigureMultispy.TabIndex = 43;
            buttonConfigureMultispy.Text = "Edit!";
            buttonConfigureMultispy.UseVisualStyleBackColor = true;
            buttonConfigureMultispy.Click += buttonConfigureMultispy_Click;
            // 
            // buttonConfigureMultisocks
            // 
            buttonConfigureMultisocks.Location = new Point(543, 219);
            buttonConfigureMultisocks.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureMultisocks.Name = "buttonConfigureMultisocks";
            buttonConfigureMultisocks.Size = new Size(139, 32);
            buttonConfigureMultisocks.TabIndex = 42;
            buttonConfigureMultisocks.Text = "Edit!";
            buttonConfigureMultisocks.UseVisualStyleBackColor = true;
            buttonConfigureMultisocks.Click += buttonConfigureMultisocks_Click;
            // 
            // buttonConfigureHorizon
            // 
            buttonConfigureHorizon.Location = new Point(543, 164);
            buttonConfigureHorizon.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureHorizon.Name = "buttonConfigureHorizon";
            buttonConfigureHorizon.Size = new Size(139, 32);
            buttonConfigureHorizon.TabIndex = 41;
            buttonConfigureHorizon.Text = "Edit!";
            buttonConfigureHorizon.UseVisualStyleBackColor = true;
            buttonConfigureHorizon.Click += buttonConfigureHorizon_Click;
            // 
            // buttonConfigureDNS
            // 
            buttonConfigureDNS.Location = new Point(543, 109);
            buttonConfigureDNS.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureDNS.Name = "buttonConfigureDNS";
            buttonConfigureDNS.Size = new Size(139, 32);
            buttonConfigureDNS.TabIndex = 40;
            buttonConfigureDNS.Text = "Edit!";
            buttonConfigureDNS.UseVisualStyleBackColor = true;
            buttonConfigureDNS.Click += buttonConfigureDNS_Click;
            // 
            // buttonConfigureApacheNet
            // 
            buttonConfigureApacheNet.Location = new Point(543, 57);
            buttonConfigureApacheNet.Margin = new Padding(3, 4, 3, 4);
            buttonConfigureApacheNet.Name = "buttonConfigureApacheNet";
            buttonConfigureApacheNet.Size = new Size(139, 32);
            buttonConfigureApacheNet.TabIndex = 39;
            buttonConfigureApacheNet.Text = "Edit!";
            buttonConfigureApacheNet.UseVisualStyleBackColor = true;
            buttonConfigureApacheNet.Click += buttonConfigureApacheNet_Click;
            // 
            // textBoxEdenserverJsonPath
            // 
            textBoxEdenserverJsonPath.Location = new Point(6, 497);
            textBoxEdenserverJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxEdenserverJsonPath.Name = "textBoxEdenserverJsonPath";
            textBoxEdenserverJsonPath.ReadOnly = true;
            textBoxEdenserverJsonPath.Size = new Size(514, 27);
            textBoxEdenserverJsonPath.TabIndex = 38;
            // 
            // textBoxSVOJsonPath
            // 
            textBoxSVOJsonPath.Location = new Point(6, 444);
            textBoxSVOJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxSVOJsonPath.Name = "textBoxSVOJsonPath";
            textBoxSVOJsonPath.ReadOnly = true;
            textBoxSVOJsonPath.Size = new Size(514, 27);
            textBoxSVOJsonPath.TabIndex = 37;
            // 
            // textBoxSSFWServerJsonPath
            // 
            textBoxSSFWServerJsonPath.Location = new Point(6, 387);
            textBoxSSFWServerJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxSSFWServerJsonPath.Name = "textBoxSSFWServerJsonPath";
            textBoxSSFWServerJsonPath.ReadOnly = true;
            textBoxSSFWServerJsonPath.Size = new Size(514, 27);
            textBoxSSFWServerJsonPath.TabIndex = 36;
            // 
            // textBoxQuazalserverJsonPath
            // 
            textBoxQuazalserverJsonPath.Location = new Point(6, 332);
            textBoxQuazalserverJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxQuazalserverJsonPath.Name = "textBoxQuazalserverJsonPath";
            textBoxQuazalserverJsonPath.ReadOnly = true;
            textBoxQuazalserverJsonPath.Size = new Size(514, 27);
            textBoxQuazalserverJsonPath.TabIndex = 35;
            // 
            // textBoxMultispyJsonPath
            // 
            textBoxMultispyJsonPath.Location = new Point(6, 277);
            textBoxMultispyJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxMultispyJsonPath.Name = "textBoxMultispyJsonPath";
            textBoxMultispyJsonPath.ReadOnly = true;
            textBoxMultispyJsonPath.Size = new Size(514, 27);
            textBoxMultispyJsonPath.TabIndex = 34;
            // 
            // textBoxMultisocksJsonPath
            // 
            textBoxMultisocksJsonPath.Location = new Point(6, 222);
            textBoxMultisocksJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxMultisocksJsonPath.Name = "textBoxMultisocksJsonPath";
            textBoxMultisocksJsonPath.ReadOnly = true;
            textBoxMultisocksJsonPath.Size = new Size(514, 27);
            textBoxMultisocksJsonPath.TabIndex = 33;
            // 
            // textBoxHorizonJsonPath
            // 
            textBoxHorizonJsonPath.Location = new Point(6, 167);
            textBoxHorizonJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxHorizonJsonPath.Name = "textBoxHorizonJsonPath";
            textBoxHorizonJsonPath.ReadOnly = true;
            textBoxHorizonJsonPath.Size = new Size(514, 27);
            textBoxHorizonJsonPath.TabIndex = 32;
            // 
            // textBoxDNSJsonPath
            // 
            textBoxDNSJsonPath.Location = new Point(6, 112);
            textBoxDNSJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxDNSJsonPath.Name = "textBoxDNSJsonPath";
            textBoxDNSJsonPath.ReadOnly = true;
            textBoxDNSJsonPath.Size = new Size(514, 27);
            textBoxDNSJsonPath.TabIndex = 31;
            // 
            // textBoxApacheNetJsonPath
            // 
            textBoxApacheNetJsonPath.Location = new Point(6, 60);
            textBoxApacheNetJsonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxApacheNetJsonPath.Name = "textBoxApacheNetJsonPath";
            textBoxApacheNetJsonPath.ReadOnly = true;
            textBoxApacheNetJsonPath.Size = new Size(514, 27);
            textBoxApacheNetJsonPath.TabIndex = 30;
            // 
            // labelEdenserver1
            // 
            labelEdenserver1.AutoSize = true;
            labelEdenserver1.Location = new Point(6, 473);
            labelEdenserver1.Name = "labelEdenserver1";
            labelEdenserver1.Size = new Size(83, 20);
            labelEdenserver1.TabIndex = 29;
            labelEdenserver1.Text = "EdenServer";
            // 
            // labelSVO1
            // 
            labelSVO1.AutoSize = true;
            labelSVO1.Location = new Point(6, 418);
            labelSVO1.Name = "labelSVO1";
            labelSVO1.Size = new Size(37, 20);
            labelSVO1.TabIndex = 26;
            labelSVO1.Text = "SVO";
            // 
            // labelSSFWServer1
            // 
            labelSSFWServer1.AutoSize = true;
            labelSSFWServer1.Location = new Point(6, 363);
            labelSSFWServer1.Name = "labelSSFWServer1";
            labelSSFWServer1.Size = new Size(87, 20);
            labelSSFWServer1.TabIndex = 23;
            labelSSFWServer1.Text = "SSFWServer";
            // 
            // labelQuazalserver1
            // 
            labelQuazalserver1.AutoSize = true;
            labelQuazalserver1.Location = new Point(6, 308);
            labelQuazalserver1.Name = "labelQuazalserver1";
            labelQuazalserver1.Size = new Size(96, 20);
            labelQuazalserver1.TabIndex = 20;
            labelQuazalserver1.Text = "QuazalServer";
            // 
            // labelMultispy1
            // 
            labelMultispy1.AutoSize = true;
            labelMultispy1.Location = new Point(6, 253);
            labelMultispy1.Name = "labelMultispy1";
            labelMultispy1.Size = new Size(67, 20);
            labelMultispy1.TabIndex = 18;
            labelMultispy1.Text = "MultiSpy";
            // 
            // labelMultisocks1
            // 
            labelMultisocks1.AutoSize = true;
            labelMultisocks1.Location = new Point(6, 198);
            labelMultisocks1.Name = "labelMultisocks1";
            labelMultisocks1.Size = new Size(80, 20);
            labelMultisocks1.TabIndex = 15;
            labelMultisocks1.Text = "MultiSocks";
            // 
            // labelHorizon1
            // 
            labelHorizon1.AutoSize = true;
            labelHorizon1.Location = new Point(6, 143);
            labelHorizon1.Name = "labelHorizon1";
            labelHorizon1.Size = new Size(62, 20);
            labelHorizon1.TabIndex = 11;
            labelHorizon1.Text = "Horizon";
            // 
            // labelDNS1
            // 
            labelDNS1.AutoSize = true;
            labelDNS1.Location = new Point(6, 88);
            labelDNS1.Name = "labelDNS1";
            labelDNS1.Size = new Size(39, 20);
            labelDNS1.TabIndex = 9;
            labelDNS1.Text = "DNS";
            // 
            // labelApacheNet1
            // 
            labelApacheNet1.AutoSize = true;
            labelApacheNet1.Location = new Point(6, 33);
            labelApacheNet1.Name = "labelApacheNet1";
            labelApacheNet1.Size = new Size(83, 20);
            labelApacheNet1.TabIndex = 7;
            labelApacheNet1.Text = "ApacheNet";
            // 
            // richTextBoxLicense
            // 
            richTextBoxLicense.Location = new Point(0, 548);
            richTextBoxLicense.Name = "richTextBoxLicense";
            richTextBoxLicense.ReadOnly = true;
            richTextBoxLicense.Size = new Size(701, 384);
            richTextBoxLicense.TabIndex = 1;
            richTextBoxLicense.Text = resources.GetString("richTextBoxLicense.Text");
            // 
            // groupBoxServersPath
            // 
            groupBoxServersPath.Controls.Add(buttonBrowseEdenserverPath);
            groupBoxServersPath.Controls.Add(labelEdenserver);
            groupBoxServersPath.Controls.Add(textBoxEdenserverPath);
            groupBoxServersPath.Controls.Add(buttonBrowseSVOPath);
            groupBoxServersPath.Controls.Add(labelSVO);
            groupBoxServersPath.Controls.Add(textBoxSVOPath);
            groupBoxServersPath.Controls.Add(buttonBrowseSSFWServerPath);
            groupBoxServersPath.Controls.Add(labelSSFWServer);
            groupBoxServersPath.Controls.Add(textBoxSSFWServerPath);
            groupBoxServersPath.Controls.Add(buttonBrowseQuazalserverPath);
            groupBoxServersPath.Controls.Add(labelQuazalserver);
            groupBoxServersPath.Controls.Add(textBoxQuazalserverPath);
            groupBoxServersPath.Controls.Add(labelMultispy);
            groupBoxServersPath.Controls.Add(buttonBrowseMultispyPath);
            groupBoxServersPath.Controls.Add(textBoxMultispyPath);
            groupBoxServersPath.Controls.Add(labelMultisocks);
            groupBoxServersPath.Controls.Add(buttonBrowseMultisocksPath);
            groupBoxServersPath.Controls.Add(textBoxMultisocksPath);
            groupBoxServersPath.Controls.Add(buttonBrowseHorizonPath);
            groupBoxServersPath.Controls.Add(labelHorizon);
            groupBoxServersPath.Controls.Add(textBoxHorizonPath);
            groupBoxServersPath.Controls.Add(labelDNS);
            groupBoxServersPath.Controls.Add(labelApacheNet);
            groupBoxServersPath.Controls.Add(buttonBrowseDNSPath);
            groupBoxServersPath.Controls.Add(textBoxDNSPath);
            groupBoxServersPath.Controls.Add(buttonBrowseApacheNetPath);
            groupBoxServersPath.Controls.Add(textBoxApacheNetPath);
            groupBoxServersPath.Location = new Point(3, 0);
            groupBoxServersPath.Margin = new Padding(3, 4, 3, 4);
            groupBoxServersPath.Name = "groupBoxServersPath";
            groupBoxServersPath.Padding = new Padding(3, 4, 3, 4);
            groupBoxServersPath.Size = new Size(698, 541);
            groupBoxServersPath.TabIndex = 0;
            groupBoxServersPath.TabStop = false;
            groupBoxServersPath.Text = "Servers Path";
            // 
            // buttonBrowseEdenserverPath
            // 
            buttonBrowseEdenserverPath.Location = new Point(541, 494);
            buttonBrowseEdenserverPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseEdenserverPath.Name = "buttonBrowseEdenserverPath";
            buttonBrowseEdenserverPath.Size = new Size(139, 32);
            buttonBrowseEdenserverPath.TabIndex = 29;
            buttonBrowseEdenserverPath.Text = "Browse!";
            buttonBrowseEdenserverPath.UseVisualStyleBackColor = true;
            buttonBrowseEdenserverPath.Click += buttonBrowseEdenserverPath_Click;
            // 
            // labelEdenserver
            // 
            labelEdenserver.AutoSize = true;
            labelEdenserver.Location = new Point(7, 473);
            labelEdenserver.Name = "labelEdenserver";
            labelEdenserver.Size = new Size(83, 20);
            labelEdenserver.TabIndex = 28;
            labelEdenserver.Text = "EdenServer";
            // 
            // textBoxEdenserverPath
            // 
            textBoxEdenserverPath.Location = new Point(6, 497);
            textBoxEdenserverPath.Margin = new Padding(3, 4, 3, 4);
            textBoxEdenserverPath.Name = "textBoxEdenserverPath";
            textBoxEdenserverPath.Size = new Size(514, 27);
            textBoxEdenserverPath.TabIndex = 27;
            // 
            // buttonBrowseSVOPath
            // 
            buttonBrowseSVOPath.Location = new Point(541, 439);
            buttonBrowseSVOPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseSVOPath.Name = "buttonBrowseSVOPath";
            buttonBrowseSVOPath.Size = new Size(139, 32);
            buttonBrowseSVOPath.TabIndex = 26;
            buttonBrowseSVOPath.Text = "Browse!";
            buttonBrowseSVOPath.UseVisualStyleBackColor = true;
            buttonBrowseSVOPath.Click += buttonBrowseSVOPath_Click;
            // 
            // labelSVO
            // 
            labelSVO.AutoSize = true;
            labelSVO.Location = new Point(10, 418);
            labelSVO.Name = "labelSVO";
            labelSVO.Size = new Size(37, 20);
            labelSVO.TabIndex = 25;
            labelSVO.Text = "SVO";
            // 
            // textBoxSVOPath
            // 
            textBoxSVOPath.Location = new Point(6, 442);
            textBoxSVOPath.Margin = new Padding(3, 4, 3, 4);
            textBoxSVOPath.Name = "textBoxSVOPath";
            textBoxSVOPath.Size = new Size(514, 27);
            textBoxSVOPath.TabIndex = 24;
            // 
            // buttonBrowseSSFWServerPath
            // 
            buttonBrowseSSFWServerPath.Location = new Point(541, 384);
            buttonBrowseSSFWServerPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseSSFWServerPath.Name = "buttonBrowseSSFWServerPath";
            buttonBrowseSSFWServerPath.Size = new Size(139, 32);
            buttonBrowseSSFWServerPath.TabIndex = 23;
            buttonBrowseSSFWServerPath.Text = "Browse!";
            buttonBrowseSSFWServerPath.UseVisualStyleBackColor = true;
            buttonBrowseSSFWServerPath.Click += buttonBrowseSSFWServerPath_Click;
            // 
            // labelSSFWServer
            // 
            labelSSFWServer.AutoSize = true;
            labelSSFWServer.Location = new Point(8, 363);
            labelSSFWServer.Name = "labelSSFWServer";
            labelSSFWServer.Size = new Size(87, 20);
            labelSSFWServer.TabIndex = 22;
            labelSSFWServer.Text = "SSFWServer";
            // 
            // textBoxSSFWServerPath
            // 
            textBoxSSFWServerPath.Location = new Point(8, 387);
            textBoxSSFWServerPath.Margin = new Padding(3, 4, 3, 4);
            textBoxSSFWServerPath.Name = "textBoxSSFWServerPath";
            textBoxSSFWServerPath.Size = new Size(514, 27);
            textBoxSSFWServerPath.TabIndex = 21;
            // 
            // buttonBrowseQuazalserverPath
            // 
            buttonBrowseQuazalserverPath.Location = new Point(541, 329);
            buttonBrowseQuazalserverPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseQuazalserverPath.Name = "buttonBrowseQuazalserverPath";
            buttonBrowseQuazalserverPath.Size = new Size(139, 32);
            buttonBrowseQuazalserverPath.TabIndex = 20;
            buttonBrowseQuazalserverPath.Text = "Browse!";
            buttonBrowseQuazalserverPath.UseVisualStyleBackColor = true;
            buttonBrowseQuazalserverPath.Click += buttonBrowseQuazalserverPath_Click;
            // 
            // labelQuazalserver
            // 
            labelQuazalserver.AutoSize = true;
            labelQuazalserver.Location = new Point(8, 308);
            labelQuazalserver.Name = "labelQuazalserver";
            labelQuazalserver.Size = new Size(96, 20);
            labelQuazalserver.TabIndex = 19;
            labelQuazalserver.Text = "QuazalServer";
            // 
            // textBoxQuazalserverPath
            // 
            textBoxQuazalserverPath.Location = new Point(8, 332);
            textBoxQuazalserverPath.Margin = new Padding(3, 4, 3, 4);
            textBoxQuazalserverPath.Name = "textBoxQuazalserverPath";
            textBoxQuazalserverPath.Size = new Size(514, 27);
            textBoxQuazalserverPath.TabIndex = 18;
            // 
            // labelMultispy
            // 
            labelMultispy.AutoSize = true;
            labelMultispy.Location = new Point(8, 253);
            labelMultispy.Name = "labelMultispy";
            labelMultispy.Size = new Size(67, 20);
            labelMultispy.TabIndex = 17;
            labelMultispy.Text = "MultiSpy";
            // 
            // buttonBrowseMultispyPath
            // 
            buttonBrowseMultispyPath.Location = new Point(541, 274);
            buttonBrowseMultispyPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseMultispyPath.Name = "buttonBrowseMultispyPath";
            buttonBrowseMultispyPath.Size = new Size(139, 32);
            buttonBrowseMultispyPath.TabIndex = 16;
            buttonBrowseMultispyPath.Text = "Browse!";
            buttonBrowseMultispyPath.UseVisualStyleBackColor = true;
            buttonBrowseMultispyPath.Click += buttonBrowseMultispyPath_Click;
            // 
            // textBoxMultispyPath
            // 
            textBoxMultispyPath.Location = new Point(7, 277);
            textBoxMultispyPath.Margin = new Padding(3, 4, 3, 4);
            textBoxMultispyPath.Name = "textBoxMultispyPath";
            textBoxMultispyPath.Size = new Size(514, 27);
            textBoxMultispyPath.TabIndex = 15;
            // 
            // labelMultisocks
            // 
            labelMultisocks.AutoSize = true;
            labelMultisocks.Location = new Point(7, 198);
            labelMultisocks.Name = "labelMultisocks";
            labelMultisocks.Size = new Size(80, 20);
            labelMultisocks.TabIndex = 14;
            labelMultisocks.Text = "MultiSocks";
            // 
            // buttonBrowseMultisocksPath
            // 
            buttonBrowseMultisocksPath.Location = new Point(541, 219);
            buttonBrowseMultisocksPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseMultisocksPath.Name = "buttonBrowseMultisocksPath";
            buttonBrowseMultisocksPath.Size = new Size(139, 32);
            buttonBrowseMultisocksPath.TabIndex = 13;
            buttonBrowseMultisocksPath.Text = "Browse!";
            buttonBrowseMultisocksPath.UseVisualStyleBackColor = true;
            buttonBrowseMultisocksPath.Click += buttonBrowseMultisocksPath_Click;
            // 
            // textBoxMultisocksPath
            // 
            textBoxMultisocksPath.Location = new Point(7, 222);
            textBoxMultisocksPath.Margin = new Padding(3, 4, 3, 4);
            textBoxMultisocksPath.Name = "textBoxMultisocksPath";
            textBoxMultisocksPath.Size = new Size(514, 27);
            textBoxMultisocksPath.TabIndex = 12;
            // 
            // buttonBrowseHorizonPath
            // 
            buttonBrowseHorizonPath.Location = new Point(541, 164);
            buttonBrowseHorizonPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseHorizonPath.Name = "buttonBrowseHorizonPath";
            buttonBrowseHorizonPath.Size = new Size(139, 32);
            buttonBrowseHorizonPath.TabIndex = 11;
            buttonBrowseHorizonPath.Text = "Browse!";
            buttonBrowseHorizonPath.UseVisualStyleBackColor = true;
            buttonBrowseHorizonPath.Click += buttonBrowseHorizonPath_Click;
            // 
            // labelHorizon
            // 
            labelHorizon.AutoSize = true;
            labelHorizon.Location = new Point(8, 143);
            labelHorizon.Name = "labelHorizon";
            labelHorizon.Size = new Size(62, 20);
            labelHorizon.TabIndex = 10;
            labelHorizon.Text = "Horizon";
            // 
            // textBoxHorizonPath
            // 
            textBoxHorizonPath.Location = new Point(7, 167);
            textBoxHorizonPath.Margin = new Padding(3, 4, 3, 4);
            textBoxHorizonPath.Name = "textBoxHorizonPath";
            textBoxHorizonPath.Size = new Size(514, 27);
            textBoxHorizonPath.TabIndex = 9;
            // 
            // labelDNS
            // 
            labelDNS.AutoSize = true;
            labelDNS.Location = new Point(8, 88);
            labelDNS.Name = "labelDNS";
            labelDNS.Size = new Size(39, 20);
            labelDNS.TabIndex = 8;
            labelDNS.Text = "DNS";
            // 
            // labelApacheNet
            // 
            labelApacheNet.AutoSize = true;
            labelApacheNet.Location = new Point(7, 33);
            labelApacheNet.Name = "labelApacheNet";
            labelApacheNet.Size = new Size(83, 20);
            labelApacheNet.TabIndex = 6;
            labelApacheNet.Text = "ApacheNet";
            // 
            // buttonBrowseDNSPath
            // 
            buttonBrowseDNSPath.Location = new Point(541, 109);
            buttonBrowseDNSPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseDNSPath.Name = "buttonBrowseDNSPath";
            buttonBrowseDNSPath.Size = new Size(139, 32);
            buttonBrowseDNSPath.TabIndex = 5;
            buttonBrowseDNSPath.Text = "Browse!";
            buttonBrowseDNSPath.UseVisualStyleBackColor = true;
            buttonBrowseDNSPath.Click += buttonBrowseDNSPath_Click;
            // 
            // textBoxDNSPath
            // 
            textBoxDNSPath.Location = new Point(7, 112);
            textBoxDNSPath.Margin = new Padding(3, 4, 3, 4);
            textBoxDNSPath.Name = "textBoxDNSPath";
            textBoxDNSPath.Size = new Size(514, 27);
            textBoxDNSPath.TabIndex = 4;
            // 
            // buttonBrowseApacheNetPath
            // 
            buttonBrowseApacheNetPath.Location = new Point(541, 57);
            buttonBrowseApacheNetPath.Margin = new Padding(3, 4, 3, 4);
            buttonBrowseApacheNetPath.Name = "buttonBrowseApacheNetPath";
            buttonBrowseApacheNetPath.Size = new Size(139, 32);
            buttonBrowseApacheNetPath.TabIndex = 1;
            buttonBrowseApacheNetPath.Text = "Browse!";
            buttonBrowseApacheNetPath.UseVisualStyleBackColor = true;
            buttonBrowseApacheNetPath.Click += buttonBrowseHTTPPath_Click;
            // 
            // textBoxApacheNetPath
            // 
            textBoxApacheNetPath.Location = new Point(7, 57);
            textBoxApacheNetPath.Margin = new Padding(3, 4, 3, 4);
            textBoxApacheNetPath.Name = "textBoxApacheNetPath";
            textBoxApacheNetPath.Size = new Size(514, 27);
            textBoxApacheNetPath.TabIndex = 0;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1418, 961);
            Controls.Add(tabControlMain);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "FormMain";
            Text = "MultiServer Remote Control";
            tabControlMain.ResumeLayout(false);
            tabPageMain.ResumeLayout(false);
            groupBoxDisclaimer.ResumeLayout(false);
            groupBoxDisclaimer.PerformLayout();
            groupBoxControls.ResumeLayout(false);
            groupBoxEdenserver.ResumeLayout(false);
            groupBoxEdenserver.PerformLayout();
            groupBoxSVO.ResumeLayout(false);
            groupBoxSVO.PerformLayout();
            groupBoxSSFWServer.ResumeLayout(false);
            groupBoxSSFWServer.PerformLayout();
            groupBoxQuazalserver.ResumeLayout(false);
            groupBoxQuazalserver.PerformLayout();
            groupBoxMultispy.ResumeLayout(false);
            groupBoxMultispy.PerformLayout();
            groupBoxMultisocks.ResumeLayout(false);
            groupBoxMultisocks.PerformLayout();
            groupBoxHorizon.ResumeLayout(false);
            groupBoxHorizon.PerformLayout();
            groupBoxDNS.ResumeLayout(false);
            groupBoxDNS.PerformLayout();
            groupBoxHTTP.ResumeLayout(false);
            groupBoxHTTP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxMainLogo).EndInit();
            tabPageHTTP.ResumeLayout(false);
            tabPageDNS.ResumeLayout(false);
            tabPageHorizon.ResumeLayout(false);
            tabPageMultisocks.ResumeLayout(false);
            tabPageMultispy.ResumeLayout(false);
            tabPageQuazalserver.ResumeLayout(false);
            tabPageSSFWServer.ResumeLayout(false);
            tabPageSVO.ResumeLayout(false);
            tabPageEdenserver.ResumeLayout(false);
            tabPageSettings.ResumeLayout(false);
            groupBoxAuxConfigFiles.ResumeLayout(false);
            groupBoxAuxConfigFiles.PerformLayout();
            groupBoxConfigFiles.ResumeLayout(false);
            groupBoxConfigFiles.PerformLayout();
            groupBoxServersPath.ResumeLayout(false);
            groupBoxServersPath.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControlMain;
        private TabPage tabPageMain;
        private TabPage tabPageHTTP;
        private PictureBox pictureBoxMainLogo;
        private TabPage tabPageDNS;
        private GroupBox groupBoxControls;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private GroupBox groupBoxHTTP;
        private Button buttonStartHTTP;
        private TextBox textBoxHTTP;
        private Button buttonStopHTTP;
        private Button buttonRestartHTTP;
        private GroupBox groupBoxDNS;
        private Button buttonStopDNS;
        private Button buttonRestartDNS;
        private Button buttonStartDNS;
        private TextBox textBoxDNS;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private RichTextBox richTextBoxInformation;
        private TabPage tabPageSettings;
        private GroupBox groupBoxServersPath;
        private Button buttonBrowseDNSPath;
        private TextBox textBoxDNSPath;
        private Button buttonBrowseApacheNetPath;
        private TextBox textBoxApacheNetPath;
        private Label labelDNS;
        private Label labelApacheNet;
        private RichTextBox richTextBoxHTTPLog;
        private Label labelHorizon;
        private TextBox textBoxHorizonPath;
        private Button buttonBrowseHorizonPath;
        private TextBox textBoxMultisocksPath;
        private Button buttonBrowseMultisocksPath;
        private Label labelMultisocks;
        private Button buttonBrowseMultispyPath;
        private TextBox textBoxMultispyPath;
        private Label labelMultispy;
        private Button buttonBrowseQuazalserverPath;
        private Label labelQuazalserver;
        private TextBox textBoxQuazalserverPath;
        private Label labelSSFWServer;
        private TextBox textBoxSSFWServerPath;
        private Button buttonBrowseSSFWServerPath;
        private Label labelSVO;
        private TextBox textBoxSVOPath;
        private Label labelEdenserver;
        private TextBox textBoxEdenserverPath;
        private Button buttonBrowseSVOPath;
        private Button buttonBrowseEdenserverPath;
        private RichTextBox richTextBoxDNSLog;
        private TabPage tabPageHorizon;
        private RichTextBox richTextBoxHorizonLog;
        private TabPage tabPageMultisocks;
        private RichTextBox richTextBoxMultisocksLog;
        private TabPage tabPageMultispy;
        private RichTextBox richTextBoxMultispyLog;
        private TabPage tabPageQuazalserver;
        private RichTextBox richTextBoxQuazalserverLog;
        private TabPage tabPageSSFWServer;
        private RichTextBox richTextBoxSSFWServerLog;
        private TabPage tabPageSVO;
        private RichTextBox richTextBoxSVOLog;
        private TabPage tabPageEdenserver;
        private RichTextBox richTextBoxEdenserverLog;
        private GroupBox groupBoxMultisocks;
        private Button buttonStopMultisocks;
        private Button buttonRestartMultisocks;
        private Button buttonStartMultisocks;
        private TextBox textBoxMultisocks;
        private GroupBox groupBoxHorizon;
        private Button buttonStopHorizon;
        private Button buttonRestartHorizon;
        private Button buttonStartHorizon;
        private TextBox textBoxHorizon;
        private GroupBox groupBoxSVO;
        private Button buttonStopSVO;
        private Button buttonRestartSVO;
        private Button buttonStartSVO;
        private TextBox textBoxSVO;
        private GroupBox groupBoxSSFWServer;
        private Button buttonStopSSFWServer;
        private Button buttonRestartSSFWServer;
        private Button buttonStartSSFWServer;
        private TextBox textBoxSSFWServer;
        private GroupBox groupBoxQuazalserver;
        private Button buttonStopQuazalserver;
        private Button buttonRestartQuazalserver;
        private Button buttonStartQuazalserver;
        private TextBox textBoxQuazalserver;
        private GroupBox groupBoxMultispy;
        private Button buttonStopMultispy;
        private Button buttonRestartMultispy;
        private Button buttonStartMultispy;
        private TextBox textBoxMultispy;
        private GroupBox groupBoxEdenserver;
        private Button buttonStopEdenserver;
        private Button buttonRestartEdenserver;
        private Button buttonStartEdenserver;
        private TextBox textBoxEdenserver;
        private GroupBox groupBoxDisclaimer;
        private LinkLabel linkLabelGithub;
        private Label labelLinkFormater;
        private Label labelTutoReport;
        private GroupBox groupBoxConfigFiles;
        private Label labelApacheNet1;
        private RichTextBox richTextBoxLicense;
        private Label labelMultispy1;
        private Label labelMultisocks1;
        private Label labelHorizon1;
        private Label labelDNS1;
        private Label labelEdenserver1;
        private Label labelSVO1;
        private Label labelSSFWServer1;
        private Label labelQuazalserver1;
        private TextBox textBoxEdenserverJsonPath;
        private TextBox textBoxSVOJsonPath;
        private TextBox textBoxSSFWServerJsonPath;
        private TextBox textBoxQuazalserverJsonPath;
        private TextBox textBoxMultispyJsonPath;
        private TextBox textBoxMultisocksJsonPath;
        private TextBox textBoxHorizonJsonPath;
        private TextBox textBoxDNSJsonPath;
        private TextBox textBoxApacheNetJsonPath;
        private Button buttonConfigureApacheNet;
        private Button buttonConfigureEdenserver;
        private Button buttonConfigureSVO;
        private Button buttonConfigureSSFWServer;
        private Button buttonConfigureQuazalserver;
        private Button buttonConfigureMultispy;
        private Button buttonConfigureMultisocks;
        private Button buttonConfigureHorizon;
        private Button buttonConfigureDNS;
        private GroupBox groupBoxAuxConfigFiles;
        private Button buttonConfigureMUIS;
        private Button buttonConfigureDME;
        private Button buttonConfigureMedius;
        private Button buttonConfigureBwps;
        private Button buttonConfigureNat;
        private TextBox textBoxMUISJsonPath;
        private TextBox textBoxDMEJsonPath;
        private TextBox textBoxMediusJsonPath;
        private TextBox textBoxBwpsJsonPath;
        private TextBox textBoxNatJsonPath;
        private Label labelMediusAux;
        private Button buttonConfigureEbootDefs;
        private TextBox textBoxEbootDefsJsonPath;
        private Button buttonConfigureHorizonDatabase;
        private TextBox textBoxHorizonDatabaseJsonPath;
        private Button buttonConfigureAriesDatabase;
        private TextBox textBoxAriesDatabaseJsonPath;
        private Label labelMultiSocksAux;
    }
}
