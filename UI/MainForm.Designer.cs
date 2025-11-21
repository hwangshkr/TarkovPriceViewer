
namespace TarkovPriceViewer.UI
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			TrayIcon = new System.Windows.Forms.NotifyIcon(components);
			TrayMenu = new System.Windows.Forms.ContextMenuStrip(components);
			trayshow = new System.Windows.Forms.ToolStripMenuItem();
			trayexit = new System.Windows.Forms.ToolStripMenuItem();
			panel1 = new System.Windows.Forms.Panel();
			ToggleFavorite_Label = new System.Windows.Forms.Label();
			ProfitVsFleaToleranceLabel = new System.Windows.Forms.Label();
			ProfitVsFleaTolerance = new System.Windows.Forms.TrackBar();
			label7 = new System.Windows.Forms.Label();
			AmmoWorthThreshold = new System.Windows.Forms.NumericUpDown();
			label6 = new System.Windows.Forms.Label();
			worthThresholdNumeric = new System.Windows.Forms.NumericUpDown();
			hideoutUpgrades_checkBox = new System.Windows.Forms.CheckBox();
			worthThresholdLabel = new System.Windows.Forms.Label();
			day_price_box = new System.Windows.Forms.CheckBox();
			buy_from_trader_box = new System.Windows.Forms.CheckBox();
			needs_box = new System.Windows.Forms.CheckBox();
			barters_and_crafts_box = new System.Windows.Forms.CheckBox();
			sell_to_trader_box = new System.Windows.Forms.CheckBox();
			last_price_box = new System.Windows.Forms.CheckBox();
			ShowOverlay_Button = new System.Windows.Forms.Button();
			ShowOverlay_Desc = new System.Windows.Forms.Label();
			week_price_box = new System.Windows.Forms.CheckBox();
			TransParent_Text = new System.Windows.Forms.Label();
			CloseOverlayWhenMouseMoved = new System.Windows.Forms.CheckBox();
			TransParent_Bar = new System.Windows.Forms.TrackBar();
			HideOverlay_Desc2 = new System.Windows.Forms.Label();
			TransParent_Desc = new System.Windows.Forms.Label();
			HideOverlay_Button = new System.Windows.Forms.Button();
			HideOverlay_Desc = new System.Windows.Forms.Label();
			CheckUpdate = new System.Windows.Forms.Button();
			Github = new System.Windows.Forms.Button();
			TarkovMarket = new System.Windows.Forms.Button();
			TarkovWiki = new System.Windows.Forms.Button();
			panel4 = new System.Windows.Forms.Panel();
			button1 = new System.Windows.Forms.Button();
			TarkovTracker_button = new System.Windows.Forms.Button();
			Tarkov_Dev = new System.Windows.Forms.Button();
			DataProvidedBy = new System.Windows.Forms.Label();
			Tarkov_Official = new System.Windows.Forms.Button();
			MinimizetoTrayWhenStartup = new System.Windows.Forms.CheckBox();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			panel2 = new System.Windows.Forms.Panel();
			panel3 = new System.Windows.Forms.Panel();
			Exit_Button = new System.Windows.Forms.Button();
			panel6 = new System.Windows.Forms.Panel();
			CompareOverlay_Desc = new System.Windows.Forms.Label();
			CompareOverlay_Button = new System.Windows.Forms.Button();
			CompareOverlay_Desc2 = new System.Windows.Forms.Label();
			panel7 = new System.Windows.Forms.Panel();
			RandomItem = new System.Windows.Forms.CheckBox();
			ForFunRandom_Desc = new System.Windows.Forms.Label();
			check_idle_time = new System.Windows.Forms.Timer(components);
			refresh_b = new System.Windows.Forms.Button();
			panel8 = new System.Windows.Forms.Panel();
			label5 = new System.Windows.Forms.Label();
			DecreaseTrackerCountButton = new System.Windows.Forms.Button();
			label4 = new System.Windows.Forms.Label();
			IncreaseTrackerCountButton = new System.Windows.Forms.Button();
			TarkovTrackerCheckBox = new System.Windows.Forms.CheckBox();
			label2 = new System.Windows.Forms.Label();
			tarkovTrackerApiKey_textbox = new System.Windows.Forms.TextBox();
			label1 = new System.Windows.Forms.Label();
			TarkovTrackerAPI_tooltip = new System.Windows.Forms.ToolTip(components);
			languageBox = new System.Windows.Forms.ComboBox();
			label3 = new System.Windows.Forms.Label();
			modeBox = new System.Windows.Forms.ComboBox();
			ToggleFavorite_Button = new System.Windows.Forms.Button();
			TrayMenu.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ProfitVsFleaTolerance).BeginInit();
			((System.ComponentModel.ISupportInitialize)AmmoWorthThreshold).BeginInit();
			((System.ComponentModel.ISupportInitialize)worthThresholdNumeric).BeginInit();
			((System.ComponentModel.ISupportInitialize)TransParent_Bar).BeginInit();
			panel4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			panel6.SuspendLayout();
			panel7.SuspendLayout();
			panel8.SuspendLayout();
			SuspendLayout();
			// 
			// TrayIcon
			// 
			TrayIcon.ContextMenuStrip = TrayMenu;
			TrayIcon.Icon = (System.Drawing.Icon)resources.GetObject("TrayIcon.Icon");
			TrayIcon.Text = "TarkovPriceViewer";
			TrayIcon.Visible = true;
			TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
			// 
			// TrayMenu
			// 
			TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { trayshow, trayexit });
			TrayMenu.Name = "TrayMenu";
			TrayMenu.Size = new System.Drawing.Size(104, 48);
			// 
			// trayshow
			// 
			trayshow.Name = "trayshow";
			trayshow.Size = new System.Drawing.Size(103, 22);
			trayshow.Text = "Show";
			trayshow.Click += TrayShow_Click;
			// 
			// trayexit
			// 
			trayexit.Name = "trayexit";
			trayexit.Size = new System.Drawing.Size(103, 22);
			trayexit.Text = "Exit";
			trayexit.Click += TrayExit_Click;
			// 
			// panel1
			// 
			panel1.Controls.Add(ToggleFavorite_Button);
			panel1.Controls.Add(ToggleFavorite_Label);
			panel1.Controls.Add(ProfitVsFleaToleranceLabel);
			panel1.Controls.Add(ProfitVsFleaTolerance);
			panel1.Controls.Add(label7);
			panel1.Controls.Add(AmmoWorthThreshold);
			panel1.Controls.Add(label6);
			panel1.Controls.Add(worthThresholdNumeric);
			panel1.Controls.Add(hideoutUpgrades_checkBox);
			panel1.Controls.Add(worthThresholdLabel);
			panel1.Controls.Add(day_price_box);
			panel1.Controls.Add(buy_from_trader_box);
			panel1.Controls.Add(needs_box);
			panel1.Controls.Add(barters_and_crafts_box);
			panel1.Controls.Add(sell_to_trader_box);
			panel1.Controls.Add(last_price_box);
			panel1.Controls.Add(ShowOverlay_Button);
			panel1.Controls.Add(ShowOverlay_Desc);
			panel1.Location = new System.Drawing.Point(12, 166);
			panel1.Margin = new System.Windows.Forms.Padding(4);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(296, 362);
			panel1.TabIndex = 1;
			// 
			// ToggleFavorite_Label
			// 
			ToggleFavorite_Label.AutoSize = true;
			ToggleFavorite_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			ToggleFavorite_Label.Location = new System.Drawing.Point(6, 317);
			ToggleFavorite_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ToggleFavorite_Label.Name = "ToggleFavorite_Label";
			ToggleFavorite_Label.Size = new System.Drawing.Size(133, 15);
			ToggleFavorite_Label.TabIndex = 14;
			ToggleFavorite_Label.Text = "Toggle Favorite Key";
			// 
			// ToggleFavorite_Button
			// 
			ToggleFavorite_Button.Click += Overlay_Button_Click;
			// 
			// ProfitVsFleaToleranceLabel
			// 
			ProfitVsFleaToleranceLabel.AutoSize = true;
			ProfitVsFleaToleranceLabel.Location = new System.Drawing.Point(133, 278);
			ProfitVsFleaToleranceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ProfitVsFleaToleranceLabel.Name = "ProfitVsFleaToleranceLabel";
			ProfitVsFleaToleranceLabel.Size = new System.Drawing.Size(29, 15);
			ProfitVsFleaToleranceLabel.TabIndex = 13;
			ProfitVsFleaToleranceLabel.Text = "80%";
			// 
			// ProfitVsFleaTolerance
			// 
			ProfitVsFleaTolerance.Location = new System.Drawing.Point(9, 275);
			ProfitVsFleaTolerance.Margin = new System.Windows.Forms.Padding(4);
			ProfitVsFleaTolerance.Maximum = 100;
			ProfitVsFleaTolerance.Name = "ProfitVsFleaTolerance";
			ProfitVsFleaTolerance.Size = new System.Drawing.Size(124, 45);
			ProfitVsFleaTolerance.TabIndex = 12;
			ProfitVsFleaTolerance.TabStop = false;
			ProfitVsFleaTolerance.TickFrequency = 5;
			ProfitVsFleaTolerance.Value = 80;
			ProfitVsFleaTolerance.Scroll += ProfitVsFleaTolerance_Scroll;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			label7.Location = new System.Drawing.Point(6, 256);
			label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(96, 15);
			label7.TabIndex = 11;
			label7.Text = "Profit Vs. Flea";
			// 
			// AmmoWorthThreshold
			// 
			AmmoWorthThreshold.Increment = new decimal(new int[] { 100, 0, 0, 0 });
			AmmoWorthThreshold.Location = new System.Drawing.Point(9, 229);
			AmmoWorthThreshold.Margin = new System.Windows.Forms.Padding(4);
			AmmoWorthThreshold.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
			AmmoWorthThreshold.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
			AmmoWorthThreshold.Name = "AmmoWorthThreshold";
			AmmoWorthThreshold.Size = new System.Drawing.Size(126, 23);
			AmmoWorthThreshold.TabIndex = 10;
			AmmoWorthThreshold.TabStop = false;
			AmmoWorthThreshold.ThousandsSeparator = true;
			AmmoWorthThreshold.Value = new decimal(new int[] { 100, 0, 0, 0 });
			AmmoWorthThreshold.ValueChanged += AmmoWorthThreshold_ValueChanged;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			label6.Location = new System.Drawing.Point(6, 209);
			label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(156, 15);
			label6.TabIndex = 9;
			label6.Text = "Ammo Worth Threshold";
			// 
			// worthThresholdNumeric
			// 
			worthThresholdNumeric.Increment = new decimal(new int[] { 100, 0, 0, 0 });
			worthThresholdNumeric.Location = new System.Drawing.Point(9, 182);
			worthThresholdNumeric.Margin = new System.Windows.Forms.Padding(4);
			worthThresholdNumeric.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
			worthThresholdNumeric.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
			worthThresholdNumeric.Name = "worthThresholdNumeric";
			worthThresholdNumeric.Size = new System.Drawing.Size(126, 23);
			worthThresholdNumeric.TabIndex = 1;
			worthThresholdNumeric.TabStop = false;
			worthThresholdNumeric.ThousandsSeparator = true;
			worthThresholdNumeric.Value = new decimal(new int[] { 100, 0, 0, 0 });
			worthThresholdNumeric.ValueChanged += worthThresholdNumeric_ValueChanged;
			// 
			// hideoutUpgrades_checkBox
			// 
			hideoutUpgrades_checkBox.AutoSize = true;
			hideoutUpgrades_checkBox.Location = new System.Drawing.Point(9, 126);
			hideoutUpgrades_checkBox.Margin = new System.Windows.Forms.Padding(4);
			hideoutUpgrades_checkBox.Name = "hideoutUpgrades_checkBox";
			hideoutUpgrades_checkBox.Size = new System.Drawing.Size(122, 19);
			hideoutUpgrades_checkBox.TabIndex = 8;
			hideoutUpgrades_checkBox.TabStop = false;
			hideoutUpgrades_checkBox.Text = "Hideout Upgrades";
			hideoutUpgrades_checkBox.UseVisualStyleBackColor = true;
			hideoutUpgrades_checkBox.CheckedChanged += hideoutUpgrades_checkBox_CheckedChanged;
			// 
			// worthThresholdLabel
			// 
			worthThresholdLabel.AutoSize = true;
			worthThresholdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			worthThresholdLabel.Location = new System.Drawing.Point(6, 163);
			worthThresholdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			worthThresholdLabel.Name = "worthThresholdLabel";
			worthThresholdLabel.Size = new System.Drawing.Size(193, 15);
			worthThresholdLabel.TabIndex = 0;
			worthThresholdLabel.Text = "Item Worth Threshold (₽/slot)";
			// 
			// day_price_box
			// 
			day_price_box.AutoSize = true;
			day_price_box.Location = new System.Drawing.Point(153, 46);
			day_price_box.Margin = new System.Windows.Forms.Padding(4);
			day_price_box.Name = "day_price_box";
			day_price_box.Size = new System.Drawing.Size(91, 19);
			day_price_box.TabIndex = 7;
			day_price_box.TabStop = false;
			day_price_box.Text = "24h Average";
			day_price_box.UseVisualStyleBackColor = true;
			day_price_box.CheckedChanged += day_price_box_CheckedChanged;
			// 
			// buy_from_trader_box
			// 
			buy_from_trader_box.AutoSize = true;
			buy_from_trader_box.Location = new System.Drawing.Point(153, 72);
			buy_from_trader_box.Margin = new System.Windows.Forms.Padding(4);
			buy_from_trader_box.Name = "buy_from_trader_box";
			buy_from_trader_box.Size = new System.Drawing.Size(110, 19);
			buy_from_trader_box.TabIndex = 7;
			buy_from_trader_box.TabStop = false;
			buy_from_trader_box.Text = "Buy from Trader";
			buy_from_trader_box.UseVisualStyleBackColor = true;
			buy_from_trader_box.CheckedChanged += buy_from_trader_box_CheckedChanged;
			// 
			// needs_box
			// 
			needs_box.AutoSize = true;
			needs_box.Location = new System.Drawing.Point(9, 99);
			needs_box.Margin = new System.Windows.Forms.Padding(4);
			needs_box.Name = "needs_box";
			needs_box.Size = new System.Drawing.Size(95, 19);
			needs_box.TabIndex = 7;
			needs_box.TabStop = false;
			needs_box.Text = "Used in Tasks";
			needs_box.UseVisualStyleBackColor = true;
			needs_box.CheckedChanged += needs_box_CheckedChanged;
			// 
			// barters_and_crafts_box
			// 
			barters_and_crafts_box.AutoSize = true;
			barters_and_crafts_box.Location = new System.Drawing.Point(153, 99);
			barters_and_crafts_box.Margin = new System.Windows.Forms.Padding(4);
			barters_and_crafts_box.Name = "barters_and_crafts_box";
			barters_and_crafts_box.Size = new System.Drawing.Size(119, 19);
			barters_and_crafts_box.TabIndex = 7;
			barters_and_crafts_box.TabStop = false;
			barters_and_crafts_box.Text = "Barters and Crafts";
			barters_and_crafts_box.UseVisualStyleBackColor = true;
			barters_and_crafts_box.CheckedChanged += barters_and_crafts_box_CheckedChanged;
			// 
			// sell_to_trader_box
			// 
			sell_to_trader_box.AutoSize = true;
			sell_to_trader_box.Location = new System.Drawing.Point(9, 72);
			sell_to_trader_box.Margin = new System.Windows.Forms.Padding(4);
			sell_to_trader_box.Name = "sell_to_trader_box";
			sell_to_trader_box.Size = new System.Drawing.Size(93, 19);
			sell_to_trader_box.TabIndex = 7;
			sell_to_trader_box.TabStop = false;
			sell_to_trader_box.Text = "Sell to Trader";
			sell_to_trader_box.UseVisualStyleBackColor = true;
			sell_to_trader_box.CheckedChanged += sell_to_trader_box_CheckedChanged;
			// 
			// last_price_box
			// 
			last_price_box.AutoSize = true;
			last_price_box.Location = new System.Drawing.Point(9, 46);
			last_price_box.Margin = new System.Windows.Forms.Padding(4);
			last_price_box.Name = "last_price_box";
			last_price_box.Size = new System.Drawing.Size(76, 19);
			last_price_box.TabIndex = 7;
			last_price_box.TabStop = false;
			last_price_box.Text = "Last Price";
			last_price_box.UseVisualStyleBackColor = true;
			last_price_box.CheckedChanged += last_price_box_CheckedChanged;
			// 
			// ShowOverlay_Button
			// 
			ShowOverlay_Button.Location = new System.Drawing.Point(153, 10);
			ShowOverlay_Button.Margin = new System.Windows.Forms.Padding(4);
			ShowOverlay_Button.Name = "ShowOverlay_Button";
			ShowOverlay_Button.Size = new System.Drawing.Size(128, 29);
			ShowOverlay_Button.TabIndex = 1;
			ShowOverlay_Button.TabStop = false;
			ShowOverlay_Button.Text = "F9";
			ShowOverlay_Button.UseVisualStyleBackColor = true;
			ShowOverlay_Button.Click += Overlay_Button_Click;
			// 
			// ShowOverlay_Desc
			// 
			ShowOverlay_Desc.AutoSize = true;
			ShowOverlay_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			ShowOverlay_Desc.Location = new System.Drawing.Point(6, 15);
			ShowOverlay_Desc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ShowOverlay_Desc.Name = "ShowOverlay_Desc";
			ShowOverlay_Desc.Size = new System.Drawing.Size(120, 15);
			ShowOverlay_Desc.TabIndex = 0;
			ShowOverlay_Desc.Text = "Show Overlay Key";
			// 
			// week_price_box
			// 
			week_price_box.AutoSize = true;
			week_price_box.Location = new System.Drawing.Point(21, 61);
			week_price_box.Margin = new System.Windows.Forms.Padding(4);
			week_price_box.Name = "week_price_box";
			week_price_box.Size = new System.Drawing.Size(82, 19);
			week_price_box.TabIndex = 7;
			week_price_box.TabStop = false;
			week_price_box.Text = "week price";
			week_price_box.UseVisualStyleBackColor = true;
			week_price_box.Visible = false;
			week_price_box.CheckedChanged += week_price_box_CheckedChanged;
			// 
			// TransParent_Text
			// 
			TransParent_Text.AutoSize = true;
			TransParent_Text.Location = new System.Drawing.Point(237, 30);
			TransParent_Text.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			TransParent_Text.Name = "TransParent_Text";
			TransParent_Text.Size = new System.Drawing.Size(29, 15);
			TransParent_Text.TabIndex = 4;
			TransParent_Text.Text = "80%";
			// 
			// CloseOverlayWhenMouseMoved
			// 
			CloseOverlayWhenMouseMoved.AutoSize = true;
			CloseOverlayWhenMouseMoved.Location = new System.Drawing.Point(23, 64);
			CloseOverlayWhenMouseMoved.Margin = new System.Windows.Forms.Padding(4);
			CloseOverlayWhenMouseMoved.Name = "CloseOverlayWhenMouseMoved";
			CloseOverlayWhenMouseMoved.Size = new System.Drawing.Size(211, 19);
			CloseOverlayWhenMouseMoved.TabIndex = 6;
			CloseOverlayWhenMouseMoved.TabStop = false;
			CloseOverlayWhenMouseMoved.Text = "Close Overlay When Mouse Moved";
			CloseOverlayWhenMouseMoved.UseVisualStyleBackColor = true;
			CloseOverlayWhenMouseMoved.CheckedChanged += CloseOverlayWhenMouseMoved_CheckedChanged;
			// 
			// TransParent_Bar
			// 
			TransParent_Bar.Location = new System.Drawing.Point(4, 27);
			TransParent_Bar.Margin = new System.Windows.Forms.Padding(4);
			TransParent_Bar.Maximum = 100;
			TransParent_Bar.Name = "TransParent_Bar";
			TransParent_Bar.Size = new System.Drawing.Size(234, 45);
			TransParent_Bar.TabIndex = 3;
			TransParent_Bar.TabStop = false;
			TransParent_Bar.Value = 80;
			TransParent_Bar.Scroll += TransParent_Bar_Scroll;
			// 
			// HideOverlay_Desc2
			// 
			HideOverlay_Desc2.AutoSize = true;
			HideOverlay_Desc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			HideOverlay_Desc2.ForeColor = System.Drawing.Color.Red;
			HideOverlay_Desc2.Location = new System.Drawing.Point(34, 45);
			HideOverlay_Desc2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			HideOverlay_Desc2.Name = "HideOverlay_Desc2";
			HideOverlay_Desc2.Size = new System.Drawing.Size(212, 15);
			HideOverlay_Desc2.TabIndex = 0;
			HideOverlay_Desc2.Text = "※ Tab, Esc Keys are fixed to use";
			// 
			// TransParent_Desc
			// 
			TransParent_Desc.AutoSize = true;
			TransParent_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			TransParent_Desc.Location = new System.Drawing.Point(9, 8);
			TransParent_Desc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			TransParent_Desc.Name = "TransParent_Desc";
			TransParent_Desc.Size = new System.Drawing.Size(104, 15);
			TransParent_Desc.TabIndex = 0;
			TransParent_Desc.Text = "Control Opacity";
			// 
			// HideOverlay_Button
			// 
			HideOverlay_Button.Location = new System.Drawing.Point(159, 10);
			HideOverlay_Button.Margin = new System.Windows.Forms.Padding(4);
			HideOverlay_Button.Name = "HideOverlay_Button";
			HideOverlay_Button.Size = new System.Drawing.Size(122, 29);
			HideOverlay_Button.TabIndex = 1;
			HideOverlay_Button.TabStop = false;
			HideOverlay_Button.Text = "F10";
			HideOverlay_Button.UseVisualStyleBackColor = true;
			HideOverlay_Button.Click += Overlay_Button_Click;
			// 
			// HideOverlay_Desc
			// 
			HideOverlay_Desc.AutoSize = true;
			HideOverlay_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			HideOverlay_Desc.Location = new System.Drawing.Point(18, 16);
			HideOverlay_Desc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			HideOverlay_Desc.Name = "HideOverlay_Desc";
			HideOverlay_Desc.Size = new System.Drawing.Size(115, 15);
			HideOverlay_Desc.TabIndex = 0;
			HideOverlay_Desc.Text = "Hide Overlay Key";
			// 
			// CheckUpdate
			// 
			CheckUpdate.Location = new System.Drawing.Point(12, 27);
			CheckUpdate.Margin = new System.Windows.Forms.Padding(4);
			CheckUpdate.Name = "CheckUpdate";
			CheckUpdate.Size = new System.Drawing.Size(91, 26);
			CheckUpdate.TabIndex = 5;
			CheckUpdate.TabStop = false;
			CheckUpdate.Text = "CheckUpdate";
			CheckUpdate.UseVisualStyleBackColor = true;
			CheckUpdate.Click += CheckUpdate_Click;
			// 
			// Github
			// 
			Github.Location = new System.Drawing.Point(486, 5);
			Github.Margin = new System.Windows.Forms.Padding(4);
			Github.Name = "Github";
			Github.Size = new System.Drawing.Size(110, 29);
			Github.TabIndex = 5;
			Github.TabStop = false;
			Github.Text = "Github1";
			Github.UseVisualStyleBackColor = true;
			Github.Click += Github1_Click;
			// 
			// TarkovMarket
			// 
			TarkovMarket.Location = new System.Drawing.Point(255, 5);
			TarkovMarket.Margin = new System.Windows.Forms.Padding(4);
			TarkovMarket.Name = "TarkovMarket";
			TarkovMarket.Size = new System.Drawing.Size(110, 29);
			TarkovMarket.TabIndex = 5;
			TarkovMarket.TabStop = false;
			TarkovMarket.Text = "Tarkov Market";
			TarkovMarket.UseVisualStyleBackColor = true;
			TarkovMarket.Click += TarkovMarket_Click;
			// 
			// TarkovWiki
			// 
			TarkovWiki.Location = new System.Drawing.Point(371, 5);
			TarkovWiki.Margin = new System.Windows.Forms.Padding(4);
			TarkovWiki.Name = "TarkovWiki";
			TarkovWiki.Size = new System.Drawing.Size(110, 29);
			TarkovWiki.TabIndex = 5;
			TarkovWiki.TabStop = false;
			TarkovWiki.Text = "Tarkov Wiki";
			TarkovWiki.UseVisualStyleBackColor = true;
			TarkovWiki.Click += TarkovWiki_Click;
			// 
			// panel4
			// 
			panel4.Controls.Add(button1);
			panel4.Controls.Add(TarkovTracker_button);
			panel4.Controls.Add(Tarkov_Dev);
			panel4.Controls.Add(Github);
			panel4.Controls.Add(DataProvidedBy);
			panel4.Controls.Add(TarkovWiki);
			panel4.Controls.Add(Tarkov_Official);
			panel4.Controls.Add(TarkovMarket);
			panel4.Location = new System.Drawing.Point(12, 85);
			panel4.Margin = new System.Windows.Forms.Padding(4);
			panel4.Name = "panel4";
			panel4.Size = new System.Drawing.Size(600, 76);
			panel4.TabIndex = 7;
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(486, 40);
			button1.Margin = new System.Windows.Forms.Padding(4);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(110, 29);
			button1.TabIndex = 9;
			button1.TabStop = false;
			button1.Text = "Github2";
			button1.UseVisualStyleBackColor = true;
			button1.Click += Github2_Click;
			// 
			// TarkovTracker_button
			// 
			TarkovTracker_button.Location = new System.Drawing.Point(255, 40);
			TarkovTracker_button.Margin = new System.Windows.Forms.Padding(4);
			TarkovTracker_button.Name = "TarkovTracker_button";
			TarkovTracker_button.Size = new System.Drawing.Size(110, 29);
			TarkovTracker_button.TabIndex = 8;
			TarkovTracker_button.TabStop = false;
			TarkovTracker_button.Text = "Tarkov Tracker";
			TarkovTracker_button.UseVisualStyleBackColor = true;
			TarkovTracker_button.Click += TarkovTracker_button_Click;
			// 
			// Tarkov_Dev
			// 
			Tarkov_Dev.Location = new System.Drawing.Point(139, 40);
			Tarkov_Dev.Margin = new System.Windows.Forms.Padding(4);
			Tarkov_Dev.Name = "Tarkov_Dev";
			Tarkov_Dev.Size = new System.Drawing.Size(110, 29);
			Tarkov_Dev.TabIndex = 7;
			Tarkov_Dev.TabStop = false;
			Tarkov_Dev.Text = "Tarkov Dev";
			Tarkov_Dev.UseVisualStyleBackColor = true;
			Tarkov_Dev.Click += Tarkov_Dev_Click;
			// 
			// DataProvidedBy
			// 
			DataProvidedBy.AutoSize = true;
			DataProvidedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			DataProvidedBy.Location = new System.Drawing.Point(0, 9);
			DataProvidedBy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			DataProvidedBy.Name = "DataProvidedBy";
			DataProvidedBy.Size = new System.Drawing.Size(116, 15);
			DataProvidedBy.TabIndex = 6;
			DataProvidedBy.Text = "Data Provided By";
			// 
			// Tarkov_Official
			// 
			Tarkov_Official.Location = new System.Drawing.Point(139, 5);
			Tarkov_Official.Margin = new System.Windows.Forms.Padding(4);
			Tarkov_Official.Name = "Tarkov_Official";
			Tarkov_Official.Size = new System.Drawing.Size(110, 29);
			Tarkov_Official.TabIndex = 5;
			Tarkov_Official.TabStop = false;
			Tarkov_Official.Text = "Tarkov Official";
			Tarkov_Official.UseVisualStyleBackColor = true;
			Tarkov_Official.Click += Tarkov_Official_Click;
			// 
			// MinimizetoTrayWhenStartup
			// 
			MinimizetoTrayWhenStartup.AutoSize = true;
			MinimizetoTrayWhenStartup.Location = new System.Drawing.Point(21, 616);
			MinimizetoTrayWhenStartup.Margin = new System.Windows.Forms.Padding(4);
			MinimizetoTrayWhenStartup.Name = "MinimizetoTrayWhenStartup";
			MinimizetoTrayWhenStartup.Size = new System.Drawing.Size(171, 19);
			MinimizetoTrayWhenStartup.TabIndex = 6;
			MinimizetoTrayWhenStartup.TabStop = false;
			MinimizetoTrayWhenStartup.Text = "Minimize to Tray on Startup";
			MinimizetoTrayWhenStartup.UseVisualStyleBackColor = true;
			MinimizetoTrayWhenStartup.CheckedChanged += MinimizetoTrayWhenStartup_CheckedChanged;
			// 
			// pictureBox1
			// 
			pictureBox1.BackColor = System.Drawing.Color.Transparent;
			pictureBox1.Image = Properties.Resources.title;
			pictureBox1.Location = new System.Drawing.Point(128, 14);
			pictureBox1.Margin = new System.Windows.Forms.Padding(4);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(350, 50);
			pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			pictureBox1.TabIndex = 10;
			pictureBox1.TabStop = false;
			// 
			// panel2
			// 
			panel2.Controls.Add(CloseOverlayWhenMouseMoved);
			panel2.Controls.Add(HideOverlay_Desc);
			panel2.Controls.Add(HideOverlay_Button);
			panel2.Controls.Add(HideOverlay_Desc2);
			panel2.Location = new System.Drawing.Point(316, 166);
			panel2.Margin = new System.Windows.Forms.Padding(4);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(296, 94);
			panel2.TabIndex = 11;
			// 
			// panel3
			// 
			panel3.Controls.Add(TransParent_Text);
			panel3.Controls.Add(TransParent_Desc);
			panel3.Controls.Add(TransParent_Bar);
			panel3.Location = new System.Drawing.Point(316, 536);
			panel3.Margin = new System.Windows.Forms.Padding(4);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(296, 63);
			panel3.TabIndex = 12;
			// 
			// Exit_Button
			// 
			Exit_Button.Location = new System.Drawing.Point(538, 610);
			Exit_Button.Margin = new System.Windows.Forms.Padding(4);
			Exit_Button.Name = "Exit_Button";
			Exit_Button.Size = new System.Drawing.Size(74, 29);
			Exit_Button.TabIndex = 5;
			Exit_Button.TabStop = false;
			Exit_Button.Text = "Exit";
			Exit_Button.UseVisualStyleBackColor = true;
			Exit_Button.Click += Exit_Button_Click;
			// 
			// panel6
			// 
			panel6.Controls.Add(CompareOverlay_Desc);
			panel6.Controls.Add(CompareOverlay_Button);
			panel6.Controls.Add(CompareOverlay_Desc2);
			panel6.Location = new System.Drawing.Point(316, 268);
			panel6.Margin = new System.Windows.Forms.Padding(4);
			panel6.Name = "panel6";
			panel6.Size = new System.Drawing.Size(296, 76);
			panel6.TabIndex = 14;
			// 
			// CompareOverlay_Desc
			// 
			CompareOverlay_Desc.AutoSize = true;
			CompareOverlay_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			CompareOverlay_Desc.Location = new System.Drawing.Point(18, 15);
			CompareOverlay_Desc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			CompareOverlay_Desc.Name = "CompareOverlay_Desc";
			CompareOverlay_Desc.Size = new System.Drawing.Size(143, 15);
			CompareOverlay_Desc.TabIndex = 0;
			CompareOverlay_Desc.Text = "Compare Overlay Key";
			// 
			// CompareOverlay_Button
			// 
			CompareOverlay_Button.Location = new System.Drawing.Point(191, 10);
			CompareOverlay_Button.Margin = new System.Windows.Forms.Padding(4);
			CompareOverlay_Button.Name = "CompareOverlay_Button";
			CompareOverlay_Button.Size = new System.Drawing.Size(90, 29);
			CompareOverlay_Button.TabIndex = 1;
			CompareOverlay_Button.TabStop = false;
			CompareOverlay_Button.Text = "F8";
			CompareOverlay_Button.UseVisualStyleBackColor = true;
			CompareOverlay_Button.Click += Overlay_Button_Click;
			// 
			// CompareOverlay_Desc2
			// 
			CompareOverlay_Desc2.AutoSize = true;
			CompareOverlay_Desc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			CompareOverlay_Desc2.ForeColor = System.Drawing.Color.Red;
			CompareOverlay_Desc2.Location = new System.Drawing.Point(34, 41);
			CompareOverlay_Desc2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			CompareOverlay_Desc2.Name = "CompareOverlay_Desc2";
			CompareOverlay_Desc2.Size = new System.Drawing.Size(97, 15);
			CompareOverlay_Desc2.TabIndex = 0;
			CompareOverlay_Desc2.Text = "※ Experiential";
			// 
			// 
			// panel7
			// 
			panel7.Controls.Add(RandomItem);
			panel7.Controls.Add(ForFunRandom_Desc);
			panel7.Location = new System.Drawing.Point(12, 536);
			panel7.Margin = new System.Windows.Forms.Padding(4);
			panel7.Name = "panel7";
			panel7.Size = new System.Drawing.Size(296, 64);
			panel7.TabIndex = 15;
			// 
			// RandomItem
			// 
			RandomItem.AutoSize = true;
			RandomItem.Location = new System.Drawing.Point(9, 32);
			RandomItem.Margin = new System.Windows.Forms.Padding(4);
			RandomItem.Name = "RandomItem";
			RandomItem.Size = new System.Drawing.Size(159, 19);
			RandomItem.TabIndex = 6;
			RandomItem.TabStop = false;
			RandomItem.Text = "Show Random Item Price";
			RandomItem.UseVisualStyleBackColor = true;
			RandomItem.CheckedChanged += RandomItem_CheckedChanged;
			// 
			// ForFunRandom_Desc
			// 
			ForFunRandom_Desc.AutoSize = true;
			ForFunRandom_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			ForFunRandom_Desc.Location = new System.Drawing.Point(6, 10);
			ForFunRandom_Desc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			ForFunRandom_Desc.Name = "ForFunRandom_Desc";
			ForFunRandom_Desc.Size = new System.Drawing.Size(154, 15);
			ForFunRandom_Desc.TabIndex = 0;
			ForFunRandom_Desc.Text = "For Fun! Random Item!";
			// 
			// check_idle_time
			// 
			check_idle_time.Interval = 60000;
			check_idle_time.Tick += check_idle_time_Tick;
			// 
			// refresh_b
			// 
			refresh_b.Location = new System.Drawing.Point(206, 65);
			refresh_b.Margin = new System.Windows.Forms.Padding(4);
			refresh_b.Name = "refresh_b";
			refresh_b.Size = new System.Drawing.Size(75, 29);
			refresh_b.TabIndex = 16;
			refresh_b.Text = "ApplyAPI";
			refresh_b.UseVisualStyleBackColor = true;
			refresh_b.Click += refreshAPI_b_Click;
			// 
			// panel8
			// 
			panel8.Controls.Add(label5);
			panel8.Controls.Add(DecreaseTrackerCountButton);
			panel8.Controls.Add(label4);
			panel8.Controls.Add(IncreaseTrackerCountButton);
			panel8.Controls.Add(TarkovTrackerCheckBox);
			panel8.Controls.Add(label2);
			panel8.Controls.Add(tarkovTrackerApiKey_textbox);
			panel8.Controls.Add(refresh_b);
			panel8.Controls.Add(label1);
			panel8.Location = new System.Drawing.Point(316, 352);
			panel8.Margin = new System.Windows.Forms.Padding(4);
			panel8.Name = "panel8";
			panel8.Size = new System.Drawing.Size(296, 176);
			panel8.TabIndex = 17;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			label5.Location = new System.Drawing.Point(17, 148);
			label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(96, 15);
			label5.TabIndex = 20;
			label5.Text = "-1 Item Found";
			// 
			// DecreaseTrackerCountButton
			// 
			DecreaseTrackerCountButton.Location = new System.Drawing.Point(153, 143);
			DecreaseTrackerCountButton.Margin = new System.Windows.Forms.Padding(4);
			DecreaseTrackerCountButton.Name = "DecreaseTrackerCountButton";
			DecreaseTrackerCountButton.Size = new System.Drawing.Size(128, 29);
			DecreaseTrackerCountButton.TabIndex = 19;
			DecreaseTrackerCountButton.TabStop = false;
			DecreaseTrackerCountButton.Text = "F1";
			DecreaseTrackerCountButton.UseVisualStyleBackColor = true;
			DecreaseTrackerCountButton.Click += Overlay_Button_Click;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			label4.Location = new System.Drawing.Point(15, 115);
			label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(99, 15);
			label4.TabIndex = 18;
			label4.Text = "+1 Item Found";
			// 
			// IncreaseTrackerCountButton
			// 
			IncreaseTrackerCountButton.Location = new System.Drawing.Point(153, 110);
			IncreaseTrackerCountButton.Margin = new System.Windows.Forms.Padding(4);
			IncreaseTrackerCountButton.Name = "IncreaseTrackerCountButton";
			IncreaseTrackerCountButton.Size = new System.Drawing.Size(128, 29);
			IncreaseTrackerCountButton.TabIndex = 17;
			IncreaseTrackerCountButton.TabStop = false;
			IncreaseTrackerCountButton.Text = "F2";
			IncreaseTrackerCountButton.UseVisualStyleBackColor = true;
			IncreaseTrackerCountButton.Click += Overlay_Button_Click;
			// 
			// TarkovTrackerCheckBox
			// 
			TarkovTrackerCheckBox.AutoSize = true;
			TarkovTrackerCheckBox.Location = new System.Drawing.Point(19, 14);
			TarkovTrackerCheckBox.Margin = new System.Windows.Forms.Padding(4);
			TarkovTrackerCheckBox.Name = "TarkovTrackerCheckBox";
			TarkovTrackerCheckBox.Size = new System.Drawing.Size(15, 14);
			TarkovTrackerCheckBox.TabIndex = 8;
			TarkovTrackerCheckBox.TabStop = false;
			TarkovTrackerCheckBox.UseVisualStyleBackColor = true;
			TarkovTrackerCheckBox.CheckedChanged += TarkovTrackerCheckBox_CheckedChanged;
			TarkovTrackerCheckBox.MouseHover += TarkovTrackerCheckBox_MouseHover;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			label2.Location = new System.Drawing.Point(15, 36);
			label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(59, 15);
			label2.TabIndex = 3;
			label2.Text = "API Key:";
			label2.MouseHover += label2_MouseHover;
			// 
			// tarkovTrackerApiKey_textbox
			// 
			tarkovTrackerApiKey_textbox.Location = new System.Drawing.Point(91, 35);
			tarkovTrackerApiKey_textbox.Margin = new System.Windows.Forms.Padding(4);
			tarkovTrackerApiKey_textbox.Name = "tarkovTrackerApiKey_textbox";
			tarkovTrackerApiKey_textbox.Size = new System.Drawing.Size(190, 23);
			tarkovTrackerApiKey_textbox.TabIndex = 2;
			tarkovTrackerApiKey_textbox.MouseHover += tarkovTrackerApiKey_textbox_MouseHover;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 129);
			label1.Location = new System.Drawing.Point(43, 11);
			label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(150, 15);
			label1.TabIndex = 0;
			label1.Text = "Use TarkovTracker.org?";
			label1.MouseHover += label1_MouseHover;
			// 
			// languageBox
			// 
			languageBox.FormattingEnabled = true;
			languageBox.Location = new System.Drawing.Point(543, 14);
			languageBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			languageBox.Name = "languageBox";
			languageBox.Size = new System.Drawing.Size(69, 23);
			languageBox.TabIndex = 18;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(500, 19);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(41, 15);
			label3.TabIndex = 19;
			label3.Text = "Detect";
			// 
			// modeBox
			// 
			modeBox.FormattingEnabled = true;
			modeBox.Location = new System.Drawing.Point(543, 46);
			modeBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			modeBox.Name = "modeBox";
			modeBox.Size = new System.Drawing.Size(69, 23);
			modeBox.TabIndex = 20;
			// 
			// ToggleFavorite_Button
			// 
			ToggleFavorite_Button.Location = new System.Drawing.Point(153, 311);
			ToggleFavorite_Button.Margin = new System.Windows.Forms.Padding(4);
			ToggleFavorite_Button.Name = "ToggleFavorite_Button";
			ToggleFavorite_Button.Size = new System.Drawing.Size(128, 29);
			ToggleFavorite_Button.TabIndex = 15;
			ToggleFavorite_Button.TabStop = false;
			ToggleFavorite_Button.Text = "F7";
			ToggleFavorite_Button.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			BackColor = System.Drawing.Color.White;
			ClientSize = new System.Drawing.Size(621, 646);
			Controls.Add(CheckUpdate);
			Controls.Add(modeBox);
			Controls.Add(label3);
			Controls.Add(languageBox);
			Controls.Add(week_price_box);
			Controls.Add(panel8);
			Controls.Add(panel7);
			Controls.Add(panel6);
			Controls.Add(Exit_Button);
			Controls.Add(panel3);
			Controls.Add(panel2);
			Controls.Add(pictureBox1);
			Controls.Add(panel4);
			Controls.Add(MinimizetoTrayWhenStartup);
			Controls.Add(panel1);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "MainForm";
			StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			Text = "TarkovPriceViewer";
			FormClosing += MainForm_FormClosing;
			FormClosed += MainForm_FormClosed;
			Load += MainForm_load;
			TrayMenu.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)ProfitVsFleaTolerance).EndInit();
			((System.ComponentModel.ISupportInitialize)AmmoWorthThreshold).EndInit();
			((System.ComponentModel.ISupportInitialize)worthThresholdNumeric).EndInit();
			((System.ComponentModel.ISupportInitialize)TransParent_Bar).EndInit();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel6.ResumeLayout(false);
			panel6.PerformLayout();
			panel7.ResumeLayout(false);
			panel7.PerformLayout();
			panel8.ResumeLayout(false);
			panel8.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion
		private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem trayshow;
        private System.Windows.Forms.ToolStripMenuItem trayexit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox hideoutUpgrades_checkBox;
        private System.Windows.Forms.CheckBox day_price_box;
        private System.Windows.Forms.CheckBox buy_from_trader_box;
        private System.Windows.Forms.CheckBox needs_box;
        private System.Windows.Forms.CheckBox barters_and_crafts_box;
        private System.Windows.Forms.CheckBox sell_to_trader_box;
        private System.Windows.Forms.CheckBox last_price_box;
        private System.Windows.Forms.Button ShowOverlay_Button;
        private System.Windows.Forms.Label ShowOverlay_Desc;
        private System.Windows.Forms.CheckBox week_price_box;
        private System.Windows.Forms.Label TransParent_Text;
        private System.Windows.Forms.CheckBox CloseOverlayWhenMouseMoved;
        private System.Windows.Forms.TrackBar TransParent_Bar;
        private System.Windows.Forms.Label HideOverlay_Desc2;
        private System.Windows.Forms.Label TransParent_Desc;
        private System.Windows.Forms.Button HideOverlay_Button;
        private System.Windows.Forms.Label HideOverlay_Desc;
        private System.Windows.Forms.Button CheckUpdate;
        private System.Windows.Forms.Button Github;
        private System.Windows.Forms.Button TarkovMarket;
        private System.Windows.Forms.Button TarkovWiki;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button TarkovTracker_button;
        private System.Windows.Forms.Button Tarkov_Dev;
        private System.Windows.Forms.Label DataProvidedBy;
        private System.Windows.Forms.Button Tarkov_Official;
        private System.Windows.Forms.CheckBox MinimizetoTrayWhenStartup;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button Exit_Button;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label CompareOverlay_Desc;
        private System.Windows.Forms.Button CompareOverlay_Button;
        private System.Windows.Forms.Label CompareOverlay_Desc2;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.CheckBox RandomItem;
        private System.Windows.Forms.Label ForFunRandom_Desc;
        private System.Windows.Forms.Timer check_idle_time;
        private System.Windows.Forms.Button refresh_b;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.CheckBox TarkovTrackerCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tarkovTrackerApiKey_textbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip TarkovTrackerAPI_tooltip;
        private System.Windows.Forms.NumericUpDown worthThresholdNumeric;
        private System.Windows.Forms.Label worthThresholdLabel;
        private System.Windows.Forms.ComboBox languageBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox modeBox;
        private System.Windows.Forms.Button IncreaseTrackerCountButton;
        private System.Windows.Forms.Button DecreaseTrackerCountButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown AmmoWorthThreshold;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label ProfitVsFleaToleranceLabel;
		private System.Windows.Forms.TrackBar ProfitVsFleaTolerance;
		private System.Windows.Forms.Label ToggleFavorite_Label;
		private System.Windows.Forms.Button ToggleFavorite_Button;
	}
}
