namespace CSharpProject
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea5 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.stimulate_button = new System.Windows.Forms.Button();
            this.frequency_trackbar = new System.Windows.Forms.TrackBar();
            this.amplitude_trackbar = new System.Windows.Forms.TrackBar();
            this.amplitude_label = new System.Windows.Forms.Label();
            this.frequency_label = new System.Windows.Forms.Label();
            this.mVCBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ui_timer = new System.Windows.Forms.Timer(this.components);
            this.array_pad = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arduinoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experiment_menu = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrateForceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.singlechannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multichannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.durationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.sToolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.fastStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rampUpFirstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sameTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.hide_unhide_graphs = new System.Windows.Forms.ToolStripMenuItem();
            this.arduinoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debug_box = new System.Windows.Forms.TextBox();
            this.time_elapsed = new System.Windows.Forms.Label();
            this.force_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.capsuleBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.amplitude_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.frequency_trackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.amplitude_trackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mVCBindingSource)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.force_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.capsuleBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.amplitude_chart)).BeginInit();
            this.SuspendLayout();
            // 
            // stimulate_button
            // 
            this.stimulate_button.Location = new System.Drawing.Point(552, 575);
            this.stimulate_button.Name = "stimulate_button";
            this.stimulate_button.Size = new System.Drawing.Size(145, 81);
            this.stimulate_button.TabIndex = 0;
            this.stimulate_button.Text = "STIMULATE";
            this.stimulate_button.UseVisualStyleBackColor = true;
            this.stimulate_button.Click += new System.EventHandler(this.stimulate_button_Click);
            // 
            // frequency_trackbar
            // 
            this.frequency_trackbar.LargeChange = 1;
            this.frequency_trackbar.Location = new System.Drawing.Point(82, 501);
            this.frequency_trackbar.Maximum = 127;
            this.frequency_trackbar.Name = "frequency_trackbar";
            this.frequency_trackbar.Size = new System.Drawing.Size(223, 69);
            this.frequency_trackbar.TabIndex = 1;
            this.frequency_trackbar.TickFrequency = 12;
            this.frequency_trackbar.Scroll += new System.EventHandler(this.frequency_trackbar_Scroll);
            // 
            // amplitude_trackbar
            // 
            this.amplitude_trackbar.LargeChange = 1;
            this.amplitude_trackbar.Location = new System.Drawing.Point(82, 614);
            this.amplitude_trackbar.Maximum = 255;
            this.amplitude_trackbar.Name = "amplitude_trackbar";
            this.amplitude_trackbar.Size = new System.Drawing.Size(223, 69);
            this.amplitude_trackbar.TabIndex = 2;
            this.amplitude_trackbar.TickFrequency = 25;
            this.amplitude_trackbar.Scroll += new System.EventHandler(this.amplitude_trackbar_Scroll);
            // 
            // amplitude_label
            // 
            this.amplitude_label.AutoSize = true;
            this.amplitude_label.Location = new System.Drawing.Point(323, 575);
            this.amplitude_label.Name = "amplitude_label";
            this.amplitude_label.Size = new System.Drawing.Size(43, 20);
            this.amplitude_label.TabIndex = 3;
            this.amplitude_label.Text = "AMP";
            // 
            // frequency_label
            // 
            this.frequency_label.AutoSize = true;
            this.frequency_label.Location = new System.Drawing.Point(323, 512);
            this.frequency_label.Name = "frequency_label";
            this.frequency_label.Size = new System.Drawing.Size(31, 20);
            this.frequency_label.TabIndex = 4;
            this.frequency_label.Text = "FR";
            // 
            // ui_timer
            // 
            this.ui_timer.Enabled = true;
            this.ui_timer.Interval = 50;
            this.ui_timer.Tick += new System.EventHandler(this.ui_timer_Tick);
            // 
            // array_pad
            // 
            this.array_pad.Location = new System.Drawing.Point(63, 119);
            this.array_pad.Name = "array_pad";
            this.array_pad.Padding = new System.Windows.Forms.Padding(15);
            this.array_pad.Size = new System.Drawing.Size(303, 300);
            this.array_pad.TabIndex = 10;
            this.array_pad.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.experiment_menu,
            this.arduinoToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1320, 44);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(66, 40);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(158, 40);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataToolStripMenuItem,
            this.patternToolStripMenuItem});
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(158, 40);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(186, 40);
            this.dataToolStripMenuItem.Text = "Data";
            this.dataToolStripMenuItem.Click += new System.EventHandler(this.dataToolStripMenuItem_Click);
            // 
            // patternToolStripMenuItem
            // 
            this.patternToolStripMenuItem.Name = "patternToolStripMenuItem";
            this.patternToolStripMenuItem.Size = new System.Drawing.Size(186, 40);
            this.patternToolStripMenuItem.Text = "Pattern";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arduinoToolStripMenuItem,
            this.inputToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(104, 40);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // arduinoToolStripMenuItem
            // 
            this.arduinoToolStripMenuItem.Name = "arduinoToolStripMenuItem";
            this.arduinoToolStripMenuItem.Size = new System.Drawing.Size(186, 40);
            this.arduinoToolStripMenuItem.Text = "Output";
            this.arduinoToolStripMenuItem.Click += new System.EventHandler(this.arduinoToolStripMenuItem_Click);
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(186, 40);
            this.inputToolStripMenuItem.Text = "Input";
            this.inputToolStripMenuItem.Click += new System.EventHandler(this.inputToolStripMenuItem_Click);
            // 
            // experiment_menu
            // 
            this.experiment_menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.calibrateForceToolStripMenuItem,
            this.rotateToolStripMenuItem,
            this.beginToolStripMenuItem,
            this.typeToolStripMenuItem,
            this.setupToolStripMenuItem,
            this.durationToolStripMenuItem,
            this.fastStartToolStripMenuItem,
            this.hide_unhide_graphs});
            this.experiment_menu.Name = "experiment_menu";
            this.experiment_menu.Size = new System.Drawing.Size(156, 40);
            this.experiment_menu.Text = "Experiment";
            // 
            // calibrateForceToolStripMenuItem
            // 
            this.calibrateForceToolStripMenuItem.Name = "calibrateForceToolStripMenuItem";
            this.calibrateForceToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.calibrateForceToolStripMenuItem.Text = "Calibrate Force";
            // 
            // rotateToolStripMenuItem
            // 
            this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
            this.rotateToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.rotateToolStripMenuItem.Text = "Rotate Electrodes";
            this.rotateToolStripMenuItem.Click += new System.EventHandler(this.rotateToolStripMenuItem_Click);
            // 
            // beginToolStripMenuItem
            // 
            this.beginToolStripMenuItem.Name = "beginToolStripMenuItem";
            this.beginToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.beginToolStripMenuItem.Text = "Begin";
            this.beginToolStripMenuItem.Click += new System.EventHandler(this.beginToolStripMenuItem_Click);
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.singlechannelToolStripMenuItem,
            this.multichannelToolStripMenuItem});
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.typeToolStripMenuItem.Text = "Experiment Type";
            this.typeToolStripMenuItem.Click += new System.EventHandler(this.typeToolStripMenuItem_Click);
            // 
            // singlechannelToolStripMenuItem
            // 
            this.singlechannelToolStripMenuItem.Name = "singlechannelToolStripMenuItem";
            this.singlechannelToolStripMenuItem.Size = new System.Drawing.Size(273, 40);
            this.singlechannelToolStripMenuItem.Text = "Single-channel";
            this.singlechannelToolStripMenuItem.Click += new System.EventHandler(this.singlechannelToolStripMenuItem_Click);
            // 
            // multichannelToolStripMenuItem
            // 
            this.multichannelToolStripMenuItem.Name = "multichannelToolStripMenuItem";
            this.multichannelToolStripMenuItem.Size = new System.Drawing.Size(273, 40);
            this.multichannelToolStripMenuItem.Text = "Multi-channel";
            this.multichannelToolStripMenuItem.Click += new System.EventHandler(this.multichannelToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sToolStripMenuItem,
            this.sToolStripMenuItem1,
            this.sToolStripMenuItem2,
            this.sToolStripMenuItem3,
            this.sToolStripMenuItem4,
            this.sToolStripMenuItem5});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.setupToolStripMenuItem.Text = "Interval";
            // 
            // sToolStripMenuItem
            // 
            this.sToolStripMenuItem.Name = "sToolStripMenuItem";
            this.sToolStripMenuItem.Size = new System.Drawing.Size(142, 40);
            this.sToolStripMenuItem.Text = "5s";
            this.sToolStripMenuItem.Click += new System.EventHandler(this.sToolStripMenuItem_Click);
            // 
            // sToolStripMenuItem1
            // 
            this.sToolStripMenuItem1.Name = "sToolStripMenuItem1";
            this.sToolStripMenuItem1.Size = new System.Drawing.Size(142, 40);
            this.sToolStripMenuItem1.Text = "10s";
            this.sToolStripMenuItem1.Click += new System.EventHandler(this.sToolStripMenuItem1_Click);
            // 
            // sToolStripMenuItem2
            // 
            this.sToolStripMenuItem2.Name = "sToolStripMenuItem2";
            this.sToolStripMenuItem2.Size = new System.Drawing.Size(142, 40);
            this.sToolStripMenuItem2.Text = "15s";
            this.sToolStripMenuItem2.Click += new System.EventHandler(this.sToolStripMenuItem2_Click);
            // 
            // sToolStripMenuItem3
            // 
            this.sToolStripMenuItem3.Name = "sToolStripMenuItem3";
            this.sToolStripMenuItem3.Size = new System.Drawing.Size(142, 40);
            this.sToolStripMenuItem3.Text = "20s";
            this.sToolStripMenuItem3.Click += new System.EventHandler(this.sToolStripMenuItem3_Click);
            // 
            // sToolStripMenuItem4
            // 
            this.sToolStripMenuItem4.Name = "sToolStripMenuItem4";
            this.sToolStripMenuItem4.Size = new System.Drawing.Size(142, 40);
            this.sToolStripMenuItem4.Text = "25s";
            this.sToolStripMenuItem4.Click += new System.EventHandler(this.sToolStripMenuItem4_Click);
            // 
            // sToolStripMenuItem5
            // 
            this.sToolStripMenuItem5.Name = "sToolStripMenuItem5";
            this.sToolStripMenuItem5.Size = new System.Drawing.Size(142, 40);
            this.sToolStripMenuItem5.Text = "30s";
            this.sToolStripMenuItem5.Click += new System.EventHandler(this.sToolStripMenuItem5_Click);
            // 
            // durationToolStripMenuItem
            // 
            this.durationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sToolStripMenuItem6,
            this.sToolStripMenuItem7,
            this.sToolStripMenuItem8,
            this.sToolStripMenuItem9,
            this.sToolStripMenuItem10,
            this.sToolStripMenuItem11});
            this.durationToolStripMenuItem.Name = "durationToolStripMenuItem";
            this.durationToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.durationToolStripMenuItem.Text = "Duration";
            // 
            // sToolStripMenuItem6
            // 
            this.sToolStripMenuItem6.Name = "sToolStripMenuItem6";
            this.sToolStripMenuItem6.Size = new System.Drawing.Size(156, 40);
            this.sToolStripMenuItem6.Text = "45s";
            this.sToolStripMenuItem6.Click += new System.EventHandler(this.sToolStripMenuItem6_Click);
            // 
            // sToolStripMenuItem7
            // 
            this.sToolStripMenuItem7.Name = "sToolStripMenuItem7";
            this.sToolStripMenuItem7.Size = new System.Drawing.Size(156, 40);
            this.sToolStripMenuItem7.Text = "60s";
            this.sToolStripMenuItem7.Click += new System.EventHandler(this.sToolStripMenuItem7_Click);
            // 
            // sToolStripMenuItem8
            // 
            this.sToolStripMenuItem8.Name = "sToolStripMenuItem8";
            this.sToolStripMenuItem8.Size = new System.Drawing.Size(156, 40);
            this.sToolStripMenuItem8.Text = "90s";
            this.sToolStripMenuItem8.Click += new System.EventHandler(this.sToolStripMenuItem8_Click);
            // 
            // sToolStripMenuItem9
            // 
            this.sToolStripMenuItem9.Name = "sToolStripMenuItem9";
            this.sToolStripMenuItem9.Size = new System.Drawing.Size(156, 40);
            this.sToolStripMenuItem9.Text = "120s";
            this.sToolStripMenuItem9.Click += new System.EventHandler(this.sToolStripMenuItem9_Click);
            // 
            // sToolStripMenuItem10
            // 
            this.sToolStripMenuItem10.Name = "sToolStripMenuItem10";
            this.sToolStripMenuItem10.Size = new System.Drawing.Size(156, 40);
            this.sToolStripMenuItem10.Text = "150s";
            this.sToolStripMenuItem10.Click += new System.EventHandler(this.sToolStripMenuItem10_Click);
            // 
            // sToolStripMenuItem11
            // 
            this.sToolStripMenuItem11.Name = "sToolStripMenuItem11";
            this.sToolStripMenuItem11.Size = new System.Drawing.Size(156, 40);
            this.sToolStripMenuItem11.Text = "180s";
            this.sToolStripMenuItem11.Click += new System.EventHandler(this.sToolStripMenuItem11_Click);
            // 
            // fastStartToolStripMenuItem
            // 
            this.fastStartToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rampUpFirstToolStripMenuItem,
            this.sameTimeToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.fastStartToolStripMenuItem.Name = "fastStartToolStripMenuItem";
            this.fastStartToolStripMenuItem.Size = new System.Drawing.Size(314, 40);
            this.fastStartToolStripMenuItem.Text = "Ramp Down Delay";
            this.fastStartToolStripMenuItem.Click += new System.EventHandler(this.fastStartToolStripMenuItem_Click);
            // 
            // rampUpFirstToolStripMenuItem
            // 
            this.rampUpFirstToolStripMenuItem.Name = "rampUpFirstToolStripMenuItem";
            this.rampUpFirstToolStripMenuItem.Size = new System.Drawing.Size(137, 40);
            this.rampUpFirstToolStripMenuItem.Text = "0.8";
            this.rampUpFirstToolStripMenuItem.Click += new System.EventHandler(this.rampUpFirstToolStripMenuItem_Click);
            // 
            // sameTimeToolStripMenuItem
            // 
            this.sameTimeToolStripMenuItem.Name = "sameTimeToolStripMenuItem";
            this.sameTimeToolStripMenuItem.Size = new System.Drawing.Size(137, 40);
            this.sameTimeToolStripMenuItem.Text = "0.7";
            this.sameTimeToolStripMenuItem.Click += new System.EventHandler(this.sameTimeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(137, 40);
            this.toolStripMenuItem2.Text = "0.6";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(137, 40);
            this.toolStripMenuItem3.Text = "0.5";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(137, 40);
            this.toolStripMenuItem4.Text = "0.4";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(137, 40);
            this.toolStripMenuItem5.Text = "0.3";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(137, 40);
            this.toolStripMenuItem6.Text = "0.2";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(137, 40);
            this.toolStripMenuItem7.Text = "0.1";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // hide_unhide_graphs
            // 
            this.hide_unhide_graphs.Name = "hide_unhide_graphs";
            this.hide_unhide_graphs.Size = new System.Drawing.Size(314, 40);
            this.hide_unhide_graphs.Text = "Hide Graphs";
            this.hide_unhide_graphs.Click += new System.EventHandler(this.hideGraphsToolStripMenuItem_Click);
            // 
            // arduinoToolStripMenuItem1
            // 
            this.arduinoToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uploadCodeToolStripMenuItem});
            this.arduinoToolStripMenuItem1.Name = "arduinoToolStripMenuItem1";
            this.arduinoToolStripMenuItem1.Size = new System.Drawing.Size(119, 40);
            this.arduinoToolStripMenuItem1.Text = "Arduino";
            // 
            // uploadCodeToolStripMenuItem
            // 
            this.uploadCodeToolStripMenuItem.Name = "uploadCodeToolStripMenuItem";
            this.uploadCodeToolStripMenuItem.Size = new System.Drawing.Size(252, 40);
            this.uploadCodeToolStripMenuItem.Text = "Upload Code";
            this.uploadCodeToolStripMenuItem.Click += new System.EventHandler(this.uploadCodeToolStripMenuItem_Click);
            // 
            // debug_box
            // 
            this.debug_box.Location = new System.Drawing.Point(869, 101);
            this.debug_box.Multiline = true;
            this.debug_box.Name = "debug_box";
            this.debug_box.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.debug_box.Size = new System.Drawing.Size(329, 539);
            this.debug_box.TabIndex = 12;
            this.debug_box.Visible = false;
            // 
            // time_elapsed
            // 
            this.time_elapsed.AutoSize = true;
            this.time_elapsed.Location = new System.Drawing.Point(600, 524);
            this.time_elapsed.Name = "time_elapsed";
            this.time_elapsed.Size = new System.Drawing.Size(47, 20);
            this.time_elapsed.TabIndex = 13;
            this.time_elapsed.Text = "TIME";
            // 
            // force_chart
            // 
            chartArea5.Name = "ChartArea1";
            this.force_chart.ChartAreas.Add(chartArea5);
            this.force_chart.Location = new System.Drawing.Point(446, 101);
            this.force_chart.Name = "force_chart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Name = "Series1";
            series3.XValueMember = "Timestamps";
            series3.YValueMembers = "Values";
            this.force_chart.Series.Add(series3);
            this.force_chart.Size = new System.Drawing.Size(382, 396);
            this.force_chart.TabIndex = 14;
            this.force_chart.Text = "chart1";
            // 
            // capsuleBindingSource
            // 
            this.capsuleBindingSource.DataSource = typeof(CSharpProject.Data.Capsule);
            // 
            // amplitude_chart
            // 
            chartArea6.Name = "amplitude_chart_area";
            this.amplitude_chart.ChartAreas.Add(chartArea6);
            this.amplitude_chart.Location = new System.Drawing.Point(869, 260);
            this.amplitude_chart.Name = "amplitude_chart";
            this.amplitude_chart.Size = new System.Drawing.Size(382, 396);
            this.amplitude_chart.TabIndex = 15;
            this.amplitude_chart.Text = "chart1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 797);
            this.Controls.Add(this.amplitude_chart);
            this.Controls.Add(this.force_chart);
            this.Controls.Add(this.time_elapsed);
            this.Controls.Add(this.debug_box);
            this.Controls.Add(this.array_pad);
            this.Controls.Add(this.frequency_label);
            this.Controls.Add(this.amplitude_label);
            this.Controls.Add(this.amplitude_trackbar);
            this.Controls.Add(this.frequency_trackbar);
            this.Controls.Add(this.stimulate_button);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.frequency_trackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.amplitude_trackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mVCBindingSource)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.force_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.capsuleBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.amplitude_chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button stimulate_button;
        private System.Windows.Forms.TrackBar frequency_trackbar;
        private System.Windows.Forms.TrackBar amplitude_trackbar;
        private System.Windows.Forms.Label amplitude_label;
        private System.Windows.Forms.Label frequency_label;
        private System.Windows.Forms.Timer ui_timer;
        private System.Windows.Forms.GroupBox array_pad;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arduinoToolStripMenuItem;
        private System.Windows.Forms.TextBox debug_box;
        private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
        private System.Windows.Forms.Label time_elapsed;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patternToolStripMenuItem;
        private System.Windows.Forms.BindingSource mVCBindingSource;
        private System.Windows.Forms.ToolStripMenuItem experiment_menu;
        private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem durationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem fastStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calibrateForceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rampUpFirstToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sameTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem singlechannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multichannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arduinoToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uploadCodeToolStripMenuItem;
        private System.Windows.Forms.DataVisualization.Charting.Chart force_chart;
        private System.Windows.Forms.BindingSource capsuleBindingSource;
        private System.Windows.Forms.ToolStripMenuItem hide_unhide_graphs;
        private System.Windows.Forms.DataVisualization.Charting.Chart amplitude_chart;
    }
}

