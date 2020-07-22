namespace TikTokTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Remove_Left = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Remove_Right = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SourcePathText = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ThreadNumber_SingleBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Video_Center = new System.Windows.Forms.TextBox();
            this.btn_Start = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.Video_Mirroring = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.text_Gamma = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.text_Saturation = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.text_Brightness = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.check_Repeat = new System.Windows.Forms.CheckBox();
            this.text_Contrast = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.check_Filter = new System.Windows.Forms.CheckBox();
            this.btn_ChosenFile = new System.Windows.Forms.Button();
            this.btn_Chosen = new System.Windows.Forms.Button();
            this.btn_OpenEnd = new System.Windows.Forms.Button();
            this.check_cmd = new System.Windows.Forms.CheckBox();
            this.tab_model = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.check_all = new System.Windows.Forms.CheckBox();
            this.table_Video = new System.Windows.Forms.DataGridView();
            this.select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AwemeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DiggCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShareCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ForwardCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CommentCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ViewCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DownLink = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Read = new System.Windows.Forms.Button();
            this.check_more = new System.Windows.Forms.CheckBox();
            this.txt_url = new System.Windows.Forms.TextBox();
            this.lab_address = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBoxCZ = new System.Windows.Forms.CheckBox();
            this.Video_Center_Extend = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tab_model.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.table_Video)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Remove_Left
            // 
            this.Remove_Left.Location = new System.Drawing.Point(104, 25);
            this.Remove_Left.Name = "Remove_Left";
            this.Remove_Left.Size = new System.Drawing.Size(67, 20);
            this.Remove_Left.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "前部去除(秒)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "后部去除(秒)";
            // 
            // Remove_Right
            // 
            this.Remove_Right.Location = new System.Drawing.Point(297, 25);
            this.Remove_Right.Name = "Remove_Right";
            this.Remove_Right.Size = new System.Drawing.Size(72, 20);
            this.Remove_Right.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "待处理路径:";
            // 
            // SourcePathText
            // 
            this.SourcePathText.Location = new System.Drawing.Point(95, 14);
            this.SourcePathText.Name = "SourcePathText";
            this.SourcePathText.Size = new System.Drawing.Size(785, 20);
            this.SourcePathText.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(404, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "单视频线程:";
            this.label6.Visible = false;
            // 
            // ThreadNumber_SingleBox
            // 
            this.ThreadNumber_SingleBox.Location = new System.Drawing.Point(485, 105);
            this.ThreadNumber_SingleBox.Name = "ThreadNumber_SingleBox";
            this.ThreadNumber_SingleBox.Size = new System.Drawing.Size(67, 20);
            this.ThreadNumber_SingleBox.TabIndex = 10;
            this.ThreadNumber_SingleBox.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label7.Location = new System.Drawing.Point(488, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 12);
            this.label7.TabIndex = 12;
            this.label7.Text = "1~128(默认32)";
            this.label7.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(405, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "中部延长点";
            // 
            // Video_Center
            // 
            this.Video_Center.Location = new System.Drawing.Point(486, 25);
            this.Video_Center.Name = "Video_Center";
            this.Video_Center.Size = new System.Drawing.Size(67, 20);
            this.Video_Center.TabIndex = 15;
            // 
            // btn_Start
            // 
            this.btn_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Start.Location = new System.Drawing.Point(895, 105);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(213, 35);
            this.btn_Start.TabIndex = 19;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.Color.WhiteSmoke;
            this.LogBox.Location = new System.Drawing.Point(26, 143);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.Size = new System.Drawing.Size(1082, 116);
            this.LogBox.TabIndex = 20;
            this.LogBox.Text = "等待运行...";
            // 
            // btn_Stop
            // 
            this.btn_Stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Stop.Location = new System.Drawing.Point(895, 105);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(213, 35);
            this.btn_Stop.TabIndex = 21;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // Video_Mirroring
            // 
            this.Video_Mirroring.AutoSize = true;
            this.Video_Mirroring.Checked = true;
            this.Video_Mirroring.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Video_Mirroring.Location = new System.Drawing.Point(19, 13);
            this.Video_Mirroring.Name = "Video_Mirroring";
            this.Video_Mirroring.Size = new System.Drawing.Size(98, 17);
            this.Video_Mirroring.TabIndex = 22;
            this.Video_Mirroring.Text = "开启视频镜像";
            this.Video_Mirroring.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(22, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "伽马:";
            // 
            // text_Gamma
            // 
            this.text_Gamma.Enabled = false;
            this.text_Gamma.ForeColor = System.Drawing.SystemColors.WindowText;
            this.text_Gamma.Location = new System.Drawing.Point(104, 62);
            this.text_Gamma.Name = "text_Gamma";
            this.text_Gamma.Size = new System.Drawing.Size(67, 20);
            this.text_Gamma.TabIndex = 23;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label12.Location = new System.Drawing.Point(96, 85);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(95, 12);
            this.label12.TabIndex = 25;
            this.label12.Text = "0.1到10.0，默认值为1";
            // 
            // text_Saturation
            // 
            this.text_Saturation.Enabled = false;
            this.text_Saturation.Location = new System.Drawing.Point(682, 65);
            this.text_Saturation.Name = "text_Saturation";
            this.text_Saturation.Size = new System.Drawing.Size(72, 20);
            this.text_Saturation.TabIndex = 26;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(605, 68);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 27;
            this.label13.Text = "饱和度:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label14.Location = new System.Drawing.Point(679, 88);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 12);
            this.label14.TabIndex = 28;
            this.label14.Text = "0到3.0，默认值为1";
            // 
            // text_Brightness
            // 
            this.text_Brightness.Enabled = false;
            this.text_Brightness.Location = new System.Drawing.Point(297, 65);
            this.text_Brightness.Name = "text_Brightness";
            this.text_Brightness.Size = new System.Drawing.Size(72, 20);
            this.text_Brightness.TabIndex = 29;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(219, 68);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "亮度:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label16.Location = new System.Drawing.Point(295, 88);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(93, 12);
            this.label16.TabIndex = 31;
            this.label16.Text = "-1.0到1.0，默认值为0";
            // 
            // check_Repeat
            // 
            this.check_Repeat.AutoSize = true;
            this.check_Repeat.Location = new System.Drawing.Point(203, 14);
            this.check_Repeat.Name = "check_Repeat";
            this.check_Repeat.Size = new System.Drawing.Size(86, 17);
            this.check_Repeat.TabIndex = 32;
            this.check_Repeat.Text = "开启画中画";
            this.check_Repeat.UseVisualStyleBackColor = true;
            // 
            // text_Contrast
            // 
            this.text_Contrast.Enabled = false;
            this.text_Contrast.Location = new System.Drawing.Point(486, 65);
            this.text_Contrast.Name = "text_Contrast";
            this.text_Contrast.Size = new System.Drawing.Size(67, 20);
            this.text_Contrast.TabIndex = 33;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(407, 68);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(46, 13);
            this.label17.TabIndex = 34;
            this.label17.Text = "对比度:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label18.Location = new System.Drawing.Point(483, 86);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(93, 12);
            this.label18.TabIndex = 35;
            this.label18.Text = "-2.0到2.0，默认值为1";
            // 
            // check_Filter
            // 
            this.check_Filter.AutoSize = true;
            this.check_Filter.Location = new System.Drawing.Point(123, 13);
            this.check_Filter.Name = "check_Filter";
            this.check_Filter.Size = new System.Drawing.Size(74, 17);
            this.check_Filter.TabIndex = 36;
            this.check_Filter.Text = "开启滤镜";
            this.check_Filter.UseVisualStyleBackColor = true;
            this.check_Filter.CheckedChanged += new System.EventHandler(this.check_Filter_CheckedChanged);
            // 
            // btn_ChosenFile
            // 
            this.btn_ChosenFile.Location = new System.Drawing.Point(892, 12);
            this.btn_ChosenFile.Name = "btn_ChosenFile";
            this.btn_ChosenFile.Size = new System.Drawing.Size(129, 23);
            this.btn_ChosenFile.TabIndex = 38;
            this.btn_ChosenFile.Text = "选择文件(可多选)";
            this.btn_ChosenFile.UseVisualStyleBackColor = true;
            this.btn_ChosenFile.Click += new System.EventHandler(this.btn_ChosenFile_Click);
            // 
            // btn_Chosen
            // 
            this.btn_Chosen.Location = new System.Drawing.Point(1027, 12);
            this.btn_Chosen.Name = "btn_Chosen";
            this.btn_Chosen.Size = new System.Drawing.Size(90, 23);
            this.btn_Chosen.TabIndex = 39;
            this.btn_Chosen.Text = "选择目录";
            this.btn_Chosen.UseVisualStyleBackColor = true;
            this.btn_Chosen.Click += new System.EventHandler(this.btn_Chosen_Click);
            // 
            // btn_OpenEnd
            // 
            this.btn_OpenEnd.Location = new System.Drawing.Point(605, 106);
            this.btn_OpenEnd.Name = "btn_OpenEnd";
            this.btn_OpenEnd.Size = new System.Drawing.Size(265, 34);
            this.btn_OpenEnd.TabIndex = 40;
            this.btn_OpenEnd.Text = "打开处理后视频目录";
            this.btn_OpenEnd.UseVisualStyleBackColor = true;
            this.btn_OpenEnd.Click += new System.EventHandler(this.btn_OpenEnd_Click);
            // 
            // check_cmd
            // 
            this.check_cmd.AutoSize = true;
            this.check_cmd.Checked = true;
            this.check_cmd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.check_cmd.Location = new System.Drawing.Point(123, 47);
            this.check_cmd.Name = "check_cmd";
            this.check_cmd.Size = new System.Drawing.Size(182, 17);
            this.check_cmd.TabIndex = 41;
            this.check_cmd.Text = "极速模式（若不可用请关闭）";
            this.check_cmd.UseVisualStyleBackColor = true;
            this.check_cmd.CheckedChanged += new System.EventHandler(this.check_cmd_CheckedChanged);
            // 
            // tab_model
            // 
            this.tab_model.Controls.Add(this.tabPage2);
            this.tab_model.Controls.Add(this.tabPage1);
            this.tab_model.Location = new System.Drawing.Point(3, 1);
            this.tab_model.Name = "tab_model";
            this.tab_model.SelectedIndex = 0;
            this.tab_model.Size = new System.Drawing.Size(1144, 340);
            this.tab_model.TabIndex = 42;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btn_Clear);
            this.tabPage2.Controls.Add(this.check_all);
            this.tabPage2.Controls.Add(this.table_Video);
            this.tabPage2.Controls.Add(this.btn_Read);
            this.tabPage2.Controls.Add(this.check_more);
            this.tabPage2.Controls.Add(this.txt_url);
            this.tabPage2.Controls.Add(this.lab_address);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1136, 314);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "解析模式";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(1023, 13);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(75, 23);
            this.btn_Clear.TabIndex = 16;
            this.btn_Clear.Text = "清理列表";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // check_all
            // 
            this.check_all.AutoSize = true;
            this.check_all.Location = new System.Drawing.Point(54, 56);
            this.check_all.Name = "check_all";
            this.check_all.Size = new System.Drawing.Size(15, 14);
            this.check_all.TabIndex = 15;
            this.check_all.UseVisualStyleBackColor = true;
            this.check_all.CheckedChanged += new System.EventHandler(this.check_all_CheckedChanged);
            // 
            // table_Video
            // 
            this.table_Video.AllowUserToAddRows = false;
            this.table_Video.AllowUserToDeleteRows = false;
            this.table_Video.AllowUserToOrderColumns = true;
            this.table_Video.BackgroundColor = System.Drawing.Color.White;
            this.table_Video.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.table_Video.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.table_Video.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.select,
            this.AwemeId,
            this.Desc,
            this.DiggCount,
            this.ShareCount,
            this.ForwardCount,
            this.CommentCount,
            this.ViewCount,
            this.DownLink});
            this.table_Video.Location = new System.Drawing.Point(17, 51);
            this.table_Video.Name = "table_Video";
            this.table_Video.RowHeadersVisible = false;
            this.table_Video.Size = new System.Drawing.Size(1093, 233);
            this.table_Video.TabIndex = 14;
            this.table_Video.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.table_Video_CellClick);
            // 
            // select
            // 
            this.select.HeaderText = "全选";
            this.select.Name = "select";
            this.select.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.select.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.select.Width = 70;
            // 
            // AwemeId
            // 
            this.AwemeId.DataPropertyName = "AwemeId";
            this.AwemeId.HeaderText = "AwemeId";
            this.AwemeId.Name = "AwemeId";
            this.AwemeId.Visible = false;
            // 
            // Desc
            // 
            this.Desc.DataPropertyName = "Desc";
            this.Desc.HeaderText = "视频描述";
            this.Desc.Name = "Desc";
            this.Desc.Width = 400;
            // 
            // DiggCount
            // 
            this.DiggCount.DataPropertyName = "DiggCount";
            this.DiggCount.HeaderText = "点赞数";
            this.DiggCount.Name = "DiggCount";
            this.DiggCount.Width = 90;
            // 
            // ShareCount
            // 
            this.ShareCount.DataPropertyName = "ShareCount";
            this.ShareCount.HeaderText = "分享数";
            this.ShareCount.Name = "ShareCount";
            this.ShareCount.Width = 90;
            // 
            // ForwardCount
            // 
            this.ForwardCount.DataPropertyName = "ForwardCount";
            this.ForwardCount.HeaderText = "转发数";
            this.ForwardCount.Name = "ForwardCount";
            this.ForwardCount.Width = 85;
            // 
            // CommentCount
            // 
            this.CommentCount.DataPropertyName = "CommentCount";
            this.CommentCount.HeaderText = "评论数";
            this.CommentCount.Name = "CommentCount";
            this.CommentCount.Width = 90;
            // 
            // ViewCount
            // 
            this.ViewCount.DataPropertyName = "ViewCount";
            this.ViewCount.HeaderText = "播放量";
            this.ViewCount.Name = "ViewCount";
            this.ViewCount.Width = 90;
            // 
            // DownLink
            // 
            this.DownLink.DataPropertyName = "DownLink";
            this.DownLink.HeaderText = "下载地址";
            this.DownLink.Name = "DownLink";
            this.DownLink.Width = 150;
            // 
            // btn_Read
            // 
            this.btn_Read.Location = new System.Drawing.Point(934, 13);
            this.btn_Read.Name = "btn_Read";
            this.btn_Read.Size = new System.Drawing.Size(75, 23);
            this.btn_Read.TabIndex = 13;
            this.btn_Read.Text = "读取";
            this.btn_Read.UseVisualStyleBackColor = true;
            this.btn_Read.Click += new System.EventHandler(this.btn_Read_Click);
            // 
            // check_more
            // 
            this.check_more.AutoSize = true;
            this.check_more.Location = new System.Drawing.Point(854, 17);
            this.check_more.Name = "check_more";
            this.check_more.Size = new System.Drawing.Size(74, 17);
            this.check_more.TabIndex = 12;
            this.check_more.Text = "批量模式";
            this.check_more.UseVisualStyleBackColor = true;
            this.check_more.CheckedChanged += new System.EventHandler(this.check_more_CheckedChanged);
            // 
            // txt_url
            // 
            this.txt_url.Location = new System.Drawing.Point(95, 14);
            this.txt_url.Name = "txt_url";
            this.txt_url.Size = new System.Drawing.Size(740, 20);
            this.txt_url.TabIndex = 10;
            // 
            // lab_address
            // 
            this.lab_address.AutoSize = true;
            this.lab_address.Location = new System.Drawing.Point(14, 17);
            this.lab_address.Name = "lab_address";
            this.lab_address.Size = new System.Drawing.Size(67, 13);
            this.lab_address.TabIndex = 11;
            this.lab_address.Text = "视频地址：";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btn_Chosen);
            this.tabPage1.Controls.Add(this.btn_ChosenFile);
            this.tabPage1.Controls.Add(this.SourcePathText);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1136, 314);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "本地模式";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.btn_Start);
            this.panel1.Controls.Add(this.LogBox);
            this.panel1.Controls.Add(this.Remove_Right);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.text_Brightness);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.Remove_Left);
            this.panel1.Controls.Add(this.btn_OpenEnd);
            this.panel1.Controls.Add(this.Video_Center_Extend);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.btn_Stop);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.Video_Center);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.text_Saturation);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.ThreadNumber_SingleBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.text_Contrast);
            this.panel1.Controls.Add(this.text_Gamma);
            this.panel1.Location = new System.Drawing.Point(3, 347);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1140, 280);
            this.panel1.TabIndex = 43;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.AliceBlue;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.checkBoxCZ);
            this.panel2.Controls.Add(this.check_cmd);
            this.panel2.Controls.Add(this.check_Repeat);
            this.panel2.Controls.Add(this.Video_Mirroring);
            this.panel2.Controls.Add(this.check_Filter);
            this.panel2.Location = new System.Drawing.Point(795, 21);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(313, 77);
            this.panel2.TabIndex = 42;
            // 
            // checkBoxCZ
            // 
            this.checkBoxCZ.AutoSize = true;
            this.checkBoxCZ.Checked = true;
            this.checkBoxCZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCZ.Location = new System.Drawing.Point(19, 47);
            this.checkBoxCZ.Name = "checkBoxCZ";
            this.checkBoxCZ.Size = new System.Drawing.Size(74, 17);
            this.checkBoxCZ.TabIndex = 42;
            this.checkBoxCZ.Text = "自动抽帧";
            this.checkBoxCZ.UseVisualStyleBackColor = true;
            // 
            // Video_Center_Extend
            // 
            this.Video_Center_Extend.Location = new System.Drawing.Point(679, 25);
            this.Video_Center_Extend.Name = "Video_Center_Extend";
            this.Video_Center_Extend.Size = new System.Drawing.Size(72, 20);
            this.Video_Center_Extend.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(600, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "延长秒数:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 639);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tab_model);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "TikTok Tools V1.1.5";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tab_model.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.table_Video)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Remove_Left;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Remove_Right;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SourcePathText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ThreadNumber_SingleBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Video_Center;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.CheckBox Video_Mirroring;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox text_Gamma;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox text_Saturation;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox text_Brightness;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox check_Repeat;
        private System.Windows.Forms.TextBox text_Contrast;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox check_Filter;
        private System.Windows.Forms.Button btn_ChosenFile;
        private System.Windows.Forms.Button btn_Chosen;
        private System.Windows.Forms.Button btn_OpenEnd;
        private System.Windows.Forms.CheckBox check_cmd;
        private System.Windows.Forms.TabControl tab_model;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txt_url;
        private System.Windows.Forms.Label lab_address;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox check_more;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btn_Read;
        private System.Windows.Forms.DataGridView table_Video;
        private System.Windows.Forms.CheckBox check_all;
        private System.Windows.Forms.CheckBox checkBoxCZ;
        private System.Windows.Forms.TextBox Video_Center_Extend;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.DataGridViewCheckBoxColumn select;
        private System.Windows.Forms.DataGridViewTextBoxColumn AwemeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn DiggCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShareCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ForwardCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn CommentCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ViewCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn DownLink;
    }
}

