using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static TarkovPriceViewer.Models.TarkovAPI;
using TarkovPriceViewer.Models;
using TarkovPriceViewer.Services;
using TarkovPriceViewer.Utils;

namespace TarkovPriceViewer.UI
{
	public partial class MainForm : Form
	{
		private readonly ISettingsService _settingsService;
		private readonly ITarkovDataService _tarkovDataService;
		private readonly IBallisticsService _ballisticsService;
		private readonly ITarkovTrackerService _tarkovTrackerService;
		private readonly IOcrService _ocrService;
		private readonly IItemRecognitionService _itemRecognitionService;

		[DllImport("user32.dll")]
		private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

		[DllImport("user32.dll")]
		private static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc callback, IntPtr hInstance, uint threadId);

		[DllImport("user32.dll")]
		private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

		private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern short GetKeyState(int nVirtKey);

		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);

		private const int GWL_EXSTYLE = -20;
		private const int WS_EX_LAYERED = 0x80000;

		private static int repeatCount = 0;
		public static System.Timers.Timer timer = new System.Timers.Timer(250);


#pragma warning disable CS0649
		private struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public ShowWindowCommands showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}
#pragma warning restore CS0649


		private enum ShowWindowCommands : int
		{
			Hide = 0,
			Normal = 1,
			Minimized = 2,
			Maximized = 3,
		}


#pragma warning disable CS0649
		internal struct LASTINPUTINFO
		{
			public uint cbSize;
			public uint dwTime;
		}
#pragma warning restore CS0649


		private static readonly int WH_KEYBOARD_LL = 13;
		private static readonly int WM_KEYUP = 0x101;
		private static readonly int WH_MOUSE_LL = 14;
		private static readonly int WM_MOUSEMOVE = 0x200;
		private static readonly int WM_LBUTTONUP = 0x0202;
		private static readonly int WM_RBUTTONUP = 0x0205;
		private static readonly int WM_MBUTTONUP = 0x0208;
		private static readonly int WM_XBUTTONUP = 0x020C;
		private const int MOUSE_LEFT = 1001;
		private const int MOUSE_RIGHT = 1002;
		private const int MOUSE_MIDDLE = 1003;
		private const int MOUSE_X1 = 1004;
		private const int MOUSE_X2 = 1005;
		private const int XBUTTON1 = 0x0001;
		private const int XBUTTON2 = 0x0002;
		private const int VK_SHIFT = 0x10;
		private const int VK_CONTROL = 0x11;
		private const int VK_MENU = 0x12;
		private const int VK_LBUTTON = 0x01;
		private const int VK_RBUTTON = 0x02;
		private const int VK_MBUTTON = 0x04;
		private static bool isinfoclosed = true;
		private static bool iscompareclosed = true;
		private static LowLevelProc _proc_keyboard = null;
		private static LowLevelProc _proc_mouse = null;
		private static IntPtr hhook_keyboard = IntPtr.Zero;
		private static IntPtr hhook_mouse = IntPtr.Zero;
		private static IntPtr h_instance = LoadLibrary("User32");
		private static System.Drawing.Point point = new System.Drawing.Point(0, 0);
		private static int nFlags = 0x0;
		private static Overlay overlay_info = new Overlay(true);
		private static Overlay overlay_compare = new Overlay(false);
		private static CancellationTokenSource cts_info = new CancellationTokenSource();
		private static CancellationTokenSource cts_compare = new CancellationTokenSource();
		private static Control press_key_control = null;
		private static Scalar linecolor = new Scalar(90, 89, 82);
		private static long idle_time = 3600000;
		private static object lockObject = new object();
		public static RecognizationModel languageModel = null;
		private static PaddleOcrRecognizer ocrRecognizer = null;
		private static readonly object ocrLock = new object();
		public static DateTime KeyPressedTime;
		private static DateTime presstime;
		public static bool WaitingForTooltip = false;
		public static bool GettingItemInfo = false;

		public MainForm(
			ISettingsService settingsService,
			ITarkovDataService tarkovDataService,
			IBallisticsService ballisticsService,
			ITarkovTrackerService tarkovTrackerService,
			IOcrService ocrService,
			IItemRecognitionService itemRecognitionService)
		{
			_settingsService = settingsService;
			_tarkovDataService = tarkovDataService;
			_ballisticsService = ballisticsService;
			_tarkovTrackerService = tarkovTrackerService;
			_ocrService = ocrService;
			_itemRecognitionService = itemRecognitionService;

			int style = GetWindowLong(this.Handle, GWL_EXSTYLE);
			SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);
			if (Environment.OSVersion.Version.Major >= 6)
			{
				nFlags = 0x2;
			}
			InitializeComponent();
			SettingUI();
			SetHook();
			overlay_info.InitializeServices(_settingsService, _tarkovDataService, _tarkovTrackerService);
			overlay_info.Owner = this;
			overlay_info.Show();
			overlay_compare.InitializeServices(_settingsService, _tarkovDataService, _tarkovTrackerService);
			overlay_compare.Owner = this;
			overlay_compare.Show();

			timer.Elapsed += Timer_Elapsed;
			timer.AutoReset = true;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!GettingItemInfo)
			{
				if (repeatCount >= 0)//why was 12?
				{
					timer.Stop(); WaitingForTooltip = false;
					repeatCount = 0;
				}
				else
				{
					repeatCount++;

					point = Control.MousePosition;
					LoadingItemInfo();
				}
			}
		}

		private void PaddleRecognizer(Action<PaddleOcrRecognizer> action)
		{
			EnsureRecognizer();
			if (ocrRecognizer == null)
			{
				return;
			}

			lock (ocrLock)
			{
				action?.Invoke(ocrRecognizer);
			}
		}

		private void SettingUI()
		{
			var s = _settingsService.Settings;

			MinimizeBox = false;
			MaximizeBox = false;
			Text = $"TarkovPriceViewer {AppUtils.GetVersion()}";
			MinimizetoTrayWhenStartup.Checked = s.MinimizeToTrayOnStartup;
			CloseOverlayWhenMouseMoved.Checked = s.CloseOverlayWhenMouseMoved;
			RandomItem.Checked = s.RandomItem;
			last_price_box.Checked = s.ShowLastPrice;
			day_price_box.Checked = s.ShowDayPrice;
			week_price_box.Checked = s.ShowWeekPrice;
			sell_to_trader_box.Checked = s.SellToTrader;
			buy_from_trader_box.Checked = s.BuyFromTrader;
			needs_box.Checked = s.Needs;
			barters_and_crafts_box.Checked = s.BartersAndCrafts;
			ShowOverlay_Button.Text = GetKeybindText(s.ShowOverlayKeyBind);
			HideOverlay_Button.Text = GetKeybindText(s.HideOverlayKeyBind);
			CompareOverlay_Button.Text = GetKeybindText(s.CompareOverlayKeyBind);
			IncreaseTrackerCountButton.Text = GetKeybindText(s.IncreaseTrackerCountKeyBind);
			DecreaseTrackerCountButton.Text = GetKeybindText(s.DecreaseTrackerCountKeyBind);
			ToggleFavorite_Button.Text = GetKeybindText(s.ToggleFavoriteItemKeyBind);
			TransParent_Bar.Value = s.OverlayTransparent;
			TransParent_Text.Text = s.OverlayTransparent.ToString(CultureInfo.InvariantCulture) + "%";
			TarkovTrackerCheckBox.Checked = s.UseTarkovTrackerApi;
			hideoutUpgrades_checkBox.Checked = s.ShowHideoutUpgrades;
			tarkovTrackerApiKey_textbox.Text = s.TarkovTrackerApiKey;
			var worthThreshold = Math.Max(worthThresholdNumeric.Minimum, Math.Min(worthThresholdNumeric.Maximum, s.ItemWorthThreshold));
			worthThresholdNumeric.Value = worthThreshold;
			var ammoThreshold = Math.Max(AmmoWorthThreshold.Minimum, Math.Min(AmmoWorthThreshold.Maximum, s.AmmoWorthThreshold));
			AmmoWorthThreshold.Value = ammoThreshold;
			var profitTolerance = Math.Max(ProfitVsFleaTolerance.Minimum, Math.Min(ProfitVsFleaTolerance.Maximum, s.FleaTraderProfitTolerancePercent));
			ProfitVsFleaTolerance.Value = profitTolerance;
			ProfitVsFleaToleranceLabel.Text = profitTolerance + "%";

			languageBox.Items.Add("en");
			languageBox.Items.Add("ko");
			languageBox.Items.Add("cn");
			languageBox.Items.Add("jp");
			languageBox.DropDownStyle = ComboBoxStyle.DropDownList;
			languageBox.SelectedItem = s.Language;
			languageBox.SelectedIndexChanged += languageBox_SelectedIndexChanged;

			modeBox.Items.Add("regular");
			modeBox.Items.Add("pve");
			modeBox.DropDownStyle = ComboBoxStyle.DropDownList;
			modeBox.SelectedItem = s.Mode;
			modeBox.SelectedIndexChanged += modeBox_SelectedIndexChanged;

			PaddleRecognizer(null);//init ocr

			TrayIcon.Visible = true;
			check_idle_time.Start();
		}

		private void MainForm_load(object sender, EventArgs e)
		{
			//not use
		}
		private void SetHook()
		{
			SetHook(false);
		}

		private void SetHook(bool force)
		{
			try
			{
				if (force)
				{
					Debug.WriteLine("force unhook.");
					UnHook();
				}
				if (hhook_keyboard == IntPtr.Zero)
				{
					_proc_keyboard = hookKeyboardProc;
					hhook_keyboard = SetWindowsHookEx(WH_KEYBOARD_LL, _proc_keyboard, h_instance, 0);
				}
				setMouseHook();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		private void setMouseHook()
		{
			if (hhook_mouse == IntPtr.Zero)
			{
				_proc_mouse = hookMouseProc;
				hhook_mouse = SetWindowsHookEx(WH_MOUSE_LL, _proc_mouse, h_instance, 0);
			}
		}

		private void unsetMouseHook()
		{
			if (hhook_mouse != IntPtr.Zero)
			{
				UnhookWindowsHookEx(hhook_mouse);
				hhook_mouse = IntPtr.Zero;
				_proc_mouse = null;
			}
		}

		private void UnHook()
		{
			try
			{
				if (hhook_keyboard != IntPtr.Zero)
				{
					UnhookWindowsHookEx(hhook_keyboard);
					hhook_keyboard = IntPtr.Zero;
					_proc_keyboard = null;
				}
				unsetMouseHook();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		private IntPtr hookKeyboardProc(int code, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				if (code >= 0 && wParam == (IntPtr)WM_KEYUP)
				{
					if (press_key_control == null)
					{
						int vkCode = Marshal.ReadInt32(lParam);
						string combo = BuildCurrentKeyBindString(vkCode, false);

						// Always allow Esc/Tab to close overlays, even if the APIs have not finished loading
						if (vkCode == 9 || vkCode == 27)
						{
							HandleGlobalKeyOrMouse(vkCode, combo);
						}
						else if (HasAnyGlobalKeybindMatch(combo))
						{
							if (Program.finishloadingballistics)
							{
								if ((Program.finishloadingTarkovTrackerAPI && Program.AppSettings.UseTarkovTrackerApi) || !Program.AppSettings.UseTarkovTrackerApi)
								{
									HandleGlobalKeyOrMouse(vkCode, combo);
								}
								else
								{
									point = Control.MousePosition;
									overlay_info.ShowWaitAPI(point);
								}
							}
							else
							{
								point = Control.MousePosition;
								overlay_info.ShowWaitBallistics(point);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return CallNextHookEx(hhook_keyboard, code, (int)wParam, lParam);
		}

		private string NormalizeBind(string bind)
		{
			if (string.IsNullOrWhiteSpace(bind))
			{
				return string.Empty;
			}

			var parts = bind.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
			var codes = new System.Collections.Generic.List<int>();
			foreach (var p in parts)
			{
				if (int.TryParse(p, out var c))
				{
					codes.Add(c);
				}
			}
			if (codes.Count == 0)
			{
				return string.Empty;
			}
			codes.Sort();
			return string.Join("+", codes);
		}

		private bool IsKeybindMatch(string currentCombo, string settingBind)
		{
			var normalizedSetting = NormalizeBind(settingBind);
			if (!string.IsNullOrEmpty(normalizedSetting))
			{
				var normalizedCurrent = NormalizeBind(currentCombo);
				return !string.IsNullOrEmpty(normalizedCurrent) && normalizedCurrent == normalizedSetting;
			}

			return false;
		}

		private bool HasAnyGlobalKeybindMatch(string currentCombo)
		{
			string showBind = Program.AppSettings.ShowOverlayKeyBind;
			string compareBind = Program.AppSettings.CompareOverlayKeyBind;
			string hideBind = Program.AppSettings.HideOverlayKeyBind;
			string increaseBind = Program.AppSettings.IncreaseTrackerCountKeyBind;
			string decreaseBind = Program.AppSettings.DecreaseTrackerCountKeyBind;
			string toggleFavoriteBind = Program.AppSettings.ToggleFavoriteItemKeyBind;

			if (IsKeybindMatch(currentCombo, showBind)) return true;
			if (IsKeybindMatch(currentCombo, compareBind)) return true;
			if (IsKeybindMatch(currentCombo, hideBind)) return true;
			if (IsKeybindMatch(currentCombo, increaseBind)) return true;
			if (IsKeybindMatch(currentCombo, decreaseBind)) return true;
			if (IsKeybindMatch(currentCombo, toggleFavoriteBind)) return true;

			return false;
		}

		private string BuildCurrentKeyBindString(int primaryCode, bool isMouse)
		{
			var codes = new System.Collections.Generic.List<int>();

			if ((GetKeyState(VK_SHIFT) & 0x8000) != 0)
			{
				codes.Add((int)Keys.ShiftKey);
			}
			if ((GetKeyState(VK_CONTROL) & 0x8000) != 0)
			{
				codes.Add((int)Keys.ControlKey);
			}
			if ((GetKeyState(VK_MENU) & 0x8000) != 0)
			{
				codes.Add((int)Keys.Menu);
			}

			if (isMouse)
			{
				if ((GetKeyState(VK_LBUTTON) & 0x8000) != 0 || primaryCode == MOUSE_LEFT)
				{
					codes.Add(MOUSE_LEFT);
				}
				if ((GetKeyState(VK_RBUTTON) & 0x8000) != 0 || primaryCode == MOUSE_RIGHT)
				{
					codes.Add(MOUSE_RIGHT);
				}
				if ((GetKeyState(VK_MBUTTON) & 0x8000) != 0 || primaryCode == MOUSE_MIDDLE)
				{
					codes.Add(MOUSE_MIDDLE);
				}
			}
			else
			{
				if (primaryCode != (int)Keys.ShiftKey && primaryCode != (int)Keys.ControlKey && primaryCode != (int)Keys.Menu)
				{
					codes.Add(primaryCode);
				}
			}

			if (codes.Count == 0)
			{
				return string.Empty;
			}

			codes.Sort();
			return string.Join("+", codes);
		}

		private void HandleGlobalKeyOrMouse(int code, string currentCombo)
		{
			try
			{
				string showBind = Program.AppSettings.ShowOverlayKeyBind;
				string compareBind = Program.AppSettings.CompareOverlayKeyBind;
				string hideBind = Program.AppSettings.HideOverlayKeyBind;
				string increaseBind = Program.AppSettings.IncreaseTrackerCountKeyBind;
				string decreaseBind = Program.AppSettings.DecreaseTrackerCountKeyBind;
				string toggleFavoriteBind = Program.AppSettings.ToggleFavoriteItemKeyBind;

				if (IsKeybindMatch(currentCombo, showBind))
				{
					KeyPressedTime = DateTime.Now;
					Debug.WriteLine("\n\n---------------- ShowOverlay Key Pressed -----------------");
					if ((!timer.Enabled || !WaitingForTooltip) && (KeyPressedTime - presstime).TotalMilliseconds >= 200)
					{
						point = Control.MousePosition;
						WaitingForTooltip = true; timer.Start();
						LoadingItemInfo();
					}
					else if ((KeyPressedTime - presstime).TotalMilliseconds < 200)
					{
						Debug.WriteLine("Key pressed in less than 200 milliseconds.");
					}
					presstime = KeyPressedTime;
				}
				else if (IsKeybindMatch(currentCombo, compareBind))
				{
					point = Control.MousePosition;
					LoadingItemCompare();
				}
				else if (IsKeybindMatch(currentCombo, increaseBind))
				{
					overlay_info.IncrementCurrentItemCount();
				}
				else if (IsKeybindMatch(currentCombo, decreaseBind))
				{
					overlay_info.DecrementCurrentItemCount();
				}
				else if (IsKeybindMatch(currentCombo, toggleFavoriteBind))
				{
					overlay_info.ToggleFavoriteCurrentItem();
				}
				else if (IsKeybindMatch(currentCombo, hideBind)
					|| code == 9
					|| code == 27)
				{
					CloseItemInfo();
					CloseItemCompare();
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		private IntPtr hookMouseProc(int code, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				if (code >= 0)
				{
					if (!isinfoclosed
						&& Program.AppSettings.CloseOverlayWhenMouseMoved
						&& wParam == (IntPtr)WM_MOUSEMOVE
						&& (Math.Abs(Control.MousePosition.X - point.X) > 60 || Math.Abs(Control.MousePosition.Y - point.Y) > 60))
					{
						timer.Stop(); WaitingForTooltip = false; repeatCount = 0;
						CloseItemInfo();
						Task UpdateAPI = Task.Factory.StartNew(() => Program.UpdateItemListAPI());
						Task UpdateTarkovTrackerAPI = Task.Factory.StartNew(() => Program.UpdateTarkovTrackerAPI());
					}

					if (press_key_control == null)
					{
						int mouseCode = 0;

						if (wParam == (IntPtr)WM_LBUTTONUP)
						{
							mouseCode = MOUSE_LEFT;
						}
						else if (wParam == (IntPtr)WM_RBUTTONUP)
						{
							mouseCode = MOUSE_RIGHT;
						}
						else if (wParam == (IntPtr)WM_MBUTTONUP)
						{
							mouseCode = MOUSE_MIDDLE;
						}
						else if (wParam == (IntPtr)WM_XBUTTONUP)
						{
							int mouseData = Marshal.ReadInt32(lParam, 8);
							int buttonFlag = (mouseData >> 16) & 0xffff;
							if (buttonFlag == XBUTTON1)
							{
								mouseCode = MOUSE_X1;
							}
							else if (buttonFlag == XBUTTON2)
							{
								mouseCode = MOUSE_X2;
							}
						}

						if (mouseCode != 0)
						{
							string combo = BuildCurrentKeyBindString(mouseCode, true);
							HandleGlobalKeyOrMouse(mouseCode, combo);
						}
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return CallNextHookEx(hhook_mouse, code, (int)wParam, lParam);
		}

		private string GetKeybindText(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return string.Empty;
			}

			if (value.Contains("+"))
			{
				var parts = value.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
				var labels = new System.Collections.Generic.List<string>();
				foreach (var p in parts)
				{
					if (int.TryParse(p, out var codePart))
					{
						labels.Add(GetSingleCodeText(codePart));
					}
				}
				return string.Join(" + ", labels);
			}

			int code;
			if (!int.TryParse(value, out code))
			{
				return value;
			}

			return GetSingleCodeText(code);
		}

		private string GetSingleCodeText(int code)
		{
			switch (code)
			{
				case (int)Keys.ShiftKey:
					return "Shift";
				case (int)Keys.ControlKey:
					return "Ctrl";
				case (int)Keys.Menu:
					return "Alt";
				case MOUSE_LEFT:
					return "Mouse Left";
				case MOUSE_RIGHT:
					return "Mouse Right";
				case MOUSE_MIDDLE:
					return "Mouse Middle";
				case MOUSE_X1:
					return "Mouse X1";
				case MOUSE_X2:
					return "Mouse X2";
				default:
					return ((Keys)code).ToString();
			}
		}

		private uint GetIdleTime()
		{
			LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
			LastUserAction.cbSize = (uint)Marshal.SizeOf(LastUserAction);
			GetLastInputInfo(ref LastUserAction);
			return ((uint)Environment.TickCount - LastUserAction.dwTime);
		}

		public long GetTickCount()
		{
			return Environment.TickCount;
		}

		private void CloseApp()
		{
			if (TarkovTrackerCheckBox.Checked)
			{
				_settingsService.Settings.TarkovTrackerApiKey = tarkovTrackerApiKey_textbox.Text;
			}

			UnHook();
			TrayIcon.Dispose();
			CloseItemInfo();
			CloseItemCompare();
			_settingsService.Save();
			System.Windows.Forms.Application.Exit();
		}

		public void LoadingItemInfo()
		{
			isinfoclosed = false;
			cts_info.Cancel();
			cts_info = new CancellationTokenSource();

			if (timer.Enabled || WaitingForTooltip)
				overlay_info.ShowWaitingForTooltipInfo(point, cts_info.Token);
			else
				overlay_info.ShowLoadingInfo(point, cts_info.Token);

			// Ensure TarkovTracker progress is up-to-date when showing item info
			Task.Factory.StartNew(async () => await _tarkovTrackerService.UpdateTarkovTrackerAPI(force: true));
			Task task = Task.Factory.StartNew(() => FindItemTask(true, cts_info.Token));
		}

		public void LoadingItemCompare()
		{
			if (iscompareclosed)
			{
				iscompareclosed = false;
				cts_compare.Cancel();
				cts_compare = new CancellationTokenSource();
			}
			overlay_compare.ShowLoadingCompare(point, cts_compare.Token);
			Task task = Task.Factory.StartNew(() => FindItemTask(false, cts_compare.Token));
		}

		public void CloseItemInfo()
		{
			isinfoclosed = true;
			cts_info.Cancel();
			overlay_info.HideInfo();
		}

		public void CloseItemCompare()
		{
			iscompareclosed = true;
			cts_compare.Cancel();
			overlay_compare.HideCompare();
		}

		private int FindItemTask(bool isiteminfo, CancellationToken cts_one)
		{
			if (_settingsService.Settings.RandomItem)
			{
				if (!cts_one.IsCancellationRequested)
				{
					try
					{
						Item item = Program.tarkovAPI.items[new Random().Next(Program.tarkovAPI.items.Count - 1)];
						//item = MatchItemName("7.62x54r_7n37".ToLower().Trim().ToCharArray());
						FindItemInfoAPI(item, isiteminfo, cts_one);
					}
					catch
					{
						Debug.WriteLine("itemlist null");
					}
				}
			}
			else
			{
				Bitmap fullimage = CaptureScreen(CheckisTarkov());
				if (fullimage != null)
				{
					if (!cts_one.IsCancellationRequested)
					{
						FindItemAPI(fullimage, isiteminfo, cts_one);
					}
				}
				else
				{
					Debug.WriteLine("image null");
				}
			}
			return 0;
		}

		private IntPtr CheckisTarkov()
		{
			IntPtr hWnd = GetForegroundWindow();
			if (hWnd != IntPtr.Zero)
			{
				StringBuilder sbWinText = new StringBuilder(260);
				GetWindowText(hWnd, sbWinText, 260);
				if (sbWinText.ToString() == Program.appname)
				{
					return hWnd;
				}
			}
			Debug.WriteLine("error - no app");
			return IntPtr.Zero;
		}

		private Bitmap CaptureScreen(IntPtr hWnd)
		{
			if (hWnd != IntPtr.Zero)
			{
				using (Graphics Graphicsdata = Graphics.FromHwnd(hWnd))
				{
					Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
					Bitmap bmp = new Bitmap(rect.Width, rect.Height);
					using (Graphics g = Graphics.FromImage(bmp))
					{
						IntPtr hdc = g.GetHdc();
						PrintWindow(hWnd, hdc, nFlags);
						g.ReleaseHdc(hdc);
					}
					return bmp;
				}
			}
			else
			{
#if DEBUG
				try
				{
					return new Bitmap(@"img\test.png");
				}
				catch (Exception e)
				{
					Debug.WriteLine("no test img" + e.Message);
				}
#endif
				Debug.WriteLine("error - no window");
				return null;
			}
		}

		private void ShowtestImage(Mat mat)
		{
			ShowtestImage("test", mat);
		}

		private void ShowtestImage(String name, Mat mat)
		{
			Action show = delegate ()
			{
				Cv2.ImShow(name, mat);
			};
			Invoke(show);
		}

		private void getPaddleModel()
		{
			// If we already have a model loaded, we do not download it again.
			if (languageModel != null)
			{
				return;
			}

			try
			{
				AppLogger.Info("MainForm.getPaddleModel", "Ensure PaddleOCR language model in memory (load from cache or download if needed).");
				RecognizationModel model;
				var lang = _settingsService.Settings.Language;
				if (lang == "ko")
				{
					model = LocalDictOnlineRecognizationModel.KoreanV4.DownloadAsync().GetAwaiter().GetResult();
				}
				else if (lang == "cn")
				{
					model = LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync().GetAwaiter().GetResult();
				}
				else if (lang == "jp")
				{
					model = LocalDictOnlineRecognizationModel.JapanV4.DownloadAsync().GetAwaiter().GetResult();
				}
				else
				{
					model = LocalDictOnlineRecognizationModel.EnglishV4.DownloadAsync().GetAwaiter().GetResult();
				}

				lock (lockObject)
				{
					if (languageModel == null)
					{
						AppLogger.Info("MainForm.getPaddleModel", "Language model set.");
						languageModel = model;
					}
				}
			}
			catch (Exception ex)
			{
				AppLogger.Error("MainForm.getPaddleModel", "Error downloading Paddle model", ex);
			}
		}

		private void EnsureRecognizer()
		{
			getPaddleModel();
			if (languageModel == null)
			{
				return;
			}

			if (ocrRecognizer == null)
			{
				lock (ocrLock)
				{
					if (ocrRecognizer == null)
					{
						try
						{
							ocrRecognizer = new PaddleOcrRecognizer(languageModel, PaddleDevice.Gpu());
						}
						catch (Exception e)
						{
							AppLogger.Error("MainForm.EnsureRecognizer", "Error creating PaddleOcrRecognizer GPU", e);
							try
							{
								ocrRecognizer = new PaddleOcrRecognizer(languageModel, PaddleDevice.Mkldnn());
							}
							catch (Exception ex)
							{
								AppLogger.Error("MainForm.EnsureRecognizer", "Error creating CPU PaddleOcrRecognizer", ex);
								ocrRecognizer = null;
							}
						}
					}
				}
			}
		}

		private String getPaddleOCR(Mat textmat)
		{
			GettingItemInfo = true;
			try
			{
				var lang = _settingsService.Settings.Language;
				_ocrService.EnsureInitialized(lang);
				var text = _ocrService.RecognizeText(textmat, Program.splitcur) ?? string.Empty;
				AppLogger.Info("MainForm.getPaddleOCR", $"OCR primary='{text}'");
				GettingItemInfo = false;
				return text;
			}
			catch (Exception e)
			{
				AppLogger.Error("MainForm.getPaddleOCR", "Paddle error", e);
				GettingItemInfo = false;
				return string.Empty;
			}
		}

		private void FindItemAPI(Bitmap fullimage, bool isiteminfo, CancellationToken cts_one)
		{
			Item item = new Item();
			var data = _tarkovDataService.Data;
			using (Mat ScreenMat_original = BitmapConverter.ToMat(fullimage))
			using (Mat ScreenMat = ScreenMat_original.CvtColor(ColorConversionCodes.BGRA2BGR))
			using (Mat rac_img = ScreenMat.InRange(linecolor, linecolor))
			{
				OpenCvSharp.Point[][] contours;
				rac_img.FindContours(out contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
				foreach (OpenCvSharp.Point[] contour in contours)
				{
					if (!cts_one.IsCancellationRequested)
					{
						OpenCvSharp.Rect rect2 = Cv2.BoundingRect(contour);
						if (rect2.Width > 5 && rect2.Height > 10)
						{
							ScreenMat.Rectangle(rect2, Scalar.Black, 2);
							using (Mat temp = ScreenMat.SubMat(rect2))
							using (Mat temp2 = temp.Threshold(0, 255, ThresholdTypes.BinaryInv))
							{
								string text = getPaddleOCR(temp);

								if (!string.IsNullOrEmpty(text))
								{
									item = _itemRecognitionService.MatchItemName(text, string.Empty, data);
									if (item != null && !string.IsNullOrEmpty(item.name))
									{
										AppLogger.Info("MainForm.FindItemAPI", $"Matched item from primary OCR: '{text}' -> '{item.name}'");
										break;
									}
								}

								string text2 = getPaddleOCR(temp2);

								if (!string.IsNullOrEmpty(text2)) //If tooltip text found
								{
									item = _itemRecognitionService.MatchItemName(text, text2, data);
									AppLogger.Info("MainForm.FindItemAPI", $"Matched item from tooltip OCR: primary='{text}', tooltip='{text2}', result='{item?.name}'");
									break;
								}
							}
						}
						/*else
                            timer.Start(); WaitingForTooltip = true;*/
					}
				}

				if (!cts_one.IsCancellationRequested)
				{
					FindItemInfoAPI(item, isiteminfo, cts_one);
				}
			}
			fullimage.Dispose();
		}

		private void FindItemInfoAPI(Item item, bool isiteminfo, CancellationToken cts_one)
		{
			if (item.link != null) //Market link
			{
				try
				{
					if (item.types.Exists(e => e.Equals("ammo")) && !item.types.Exists(e => e.Equals("grenade"))) // "Round", "Slug", "Buckshot", "Grenade launcher cartridge" / ("gun" and not "preset" = weapons)
					{
						Program.blist.TryGetValue(item.name, out item.ballistic);
					}

				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}
			}
			if (isiteminfo)
			{
				overlay_info.ShowInfoAPI(item, cts_one);
			}
			else
			{
				overlay_compare.ShowCompareAPI(item, cts_one);
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			CloseApp();
		}

		private void TrayExit_Click(object sender, EventArgs e)
		{
			CloseApp();
		}

		private void TrayShow_Click(object sender, EventArgs e)
		{
			this.Show();
		}

		private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)//must be checked
			{
				TrayIcon.Visible = true;
				this.Hide();
				e.Cancel = true;
			}
		}

		private void MinimizetoTrayWhenStartup_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.MinimizeToTrayOnStartup = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void Tarkov_Official_Click(object sender, EventArgs e)
		{
			Process.Start(Program.official);
		}

		private void TarkovWiki_Click(object sender, EventArgs e)
		{
			Process.Start(Program.wiki);
		}

		private void TarkovMarket_Click(object sender, EventArgs e)
		{
			Process.Start(Program.tarkovmarket);
		}

		private void CloseOverlayWhenMouseMoved_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.CloseOverlayWhenMouseMoved = (sender as CheckBox).Checked;
			_settingsService.Save();
			if ((sender as CheckBox).Checked)
			{
				setMouseHook();
			}
			else
			{
				unsetMouseHook();
			}
		}

		public void ChangePressKeyData(Keys? keycode)
		{
			if (press_key_control != null)
			{
				if (keycode != null)
				{
					press_key_control.Text = GetKeybindText(((int)keycode).ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					var s = _settingsService.Settings;
					ShowOverlay_Button.Text = GetKeybindText(s.ShowOverlayKeyBind);
					HideOverlay_Button.Text = GetKeybindText(s.HideOverlayKeyBind);
					CompareOverlay_Button.Text = GetKeybindText(s.CompareOverlayKeyBind);
					IncreaseTrackerCountButton.Text = GetKeybindText(s.IncreaseTrackerCountKeyBind);
					DecreaseTrackerCountButton.Text = GetKeybindText(s.DecreaseTrackerCountKeyBind);
					ToggleFavorite_Button.Text = GetKeybindText(s.ToggleFavoriteItemKeyBind);
				}
				press_key_control = null;
			}
		}

		private void Overlay_Button_Click(object sender, EventArgs e)
		{
			press_key_control = (sender as Control);
			int selected = 0;
			if (press_key_control == ShowOverlay_Button)
			{
				selected = 1;
			}
			else if (press_key_control == HideOverlay_Button)
			{
				selected = 2;
			}
			else if (press_key_control == CompareOverlay_Button)
			{
				selected = 3;
			}
			else if (press_key_control == IncreaseTrackerCountButton)
			{
				selected = 4;
			}
			else if (press_key_control == DecreaseTrackerCountButton)
			{
				selected = 5;
			}
			else if (press_key_control == ToggleFavorite_Button)
			{
				selected = 6;
			}
			if (selected != 0)
			{
				KeyPressCheck kpc = new KeyPressCheck(selected);
				kpc.ShowDialog(this);
			}
		}

		private void TransParent_Bar_Scroll(object sender, EventArgs e)
		{
			System.Windows.Forms.TrackBar tb = (sender as System.Windows.Forms.TrackBar);
			_settingsService.Settings.OverlayTransparent = tb.Value;
			TransParent_Text.Text = tb.Value + "%";
			_settingsService.Save();
			overlay_info.ChangeTransparent(tb.Value);
		}

		private void Github1_Click(object sender, EventArgs e)
		{
			Process.Start(Program.github[0]);
		}
		private void Github2_Click(object sender, EventArgs e)
		{
			Process.Start(Program.github[1]);
		}

		private void CheckUpdate_Click(object sender, EventArgs e)
		{
			(sender as Control).Enabled = false;
			Task task = Task.Factory.StartNew(() => UpdateTask(sender as Control));
		}

		private int UpdateTask(Control control)
		{
			string highgithub = "";
			float highv = 0;

			try
			{
				using (var httpClient = new HttpClient())
				{
					httpClient.Timeout = TimeSpan.FromSeconds(5);
					foreach (var github in Program.github)
					{
						string check = httpClient.GetStringAsync(github + Program.checkupdate).Result;
						if (!string.IsNullOrEmpty(check))
						{
							string sp = check.Split('\n')[0];
							if (sp.Contains("Tarkov Price Viewer"))
							{
								string[] sp2 = sp.Split(' ');
								sp = sp2[sp2.Length - 1].Trim();
								if (float.TryParse(sp.Replace("v", ""), out float v) && v > highv)
								{
									highgithub = github;
									highv = v;
								}
							}
						}
					}
				}

				if (!string.IsNullOrEmpty(highgithub))
				{
					// Build remote/latest tag from numeric value
					string latestTag = "v" + highv.ToString(CultureInfo.InvariantCulture);
					// Current app tag (with leading 'v')
					string currentTag = AppUtils.GetVersion();
					// Compare numeric parts (without leading 'v') to decide if there is a newer version
					var currentVersionNoPrefix = AppUtils.GetVersionWithoutPrefix();
					if (!float.TryParse(currentVersionNoPrefix, NumberStyles.Float, CultureInfo.InvariantCulture, out var currentNumeric)
						|| highv > currentNumeric)
					{
						MessageBox.Show($"New version ({latestTag}) found.\nCurrent Version is {currentTag}");
						Process.Start(highgithub);
					}
					else
					{
						MessageBox.Show("No New Version.");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				MessageBox.Show("Can not check update. Please check your network.");
			}

			Action show = delegate
			{
				control.Enabled = true;
			};
			Invoke(show);
			return 0;
		}

		private void last_price_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.ShowLastPrice = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void day_price_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.ShowDayPrice = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void week_price_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.ShowWeekPrice = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void sell_to_trader_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.SellToTrader = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void buy_from_trader_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.BuyFromTrader = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void worthThresholdNumeric_ValueChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.ItemWorthThreshold = (int)worthThresholdNumeric.Value;
			_settingsService.Save();
		}

		private void AmmoWorthThreshold_ValueChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.AmmoWorthThreshold = (int)AmmoWorthThreshold.Value;
			_settingsService.Save();
		}

		private void ProfitVsFleaTolerance_Scroll(object sender, EventArgs e)
		{
			_settingsService.Settings.FleaTraderProfitTolerancePercent = ProfitVsFleaTolerance.Value;
			ProfitVsFleaToleranceLabel.Text = ProfitVsFleaTolerance.Value + "%";
			_settingsService.Save();
		}

		private void needs_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.Needs = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void barters_and_crafts_box_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.BartersAndCrafts = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void Exit_Button_Click(object sender, EventArgs e)
		{
			CloseApp();
		}

		private void RandomItem_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.RandomItem = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void check_idle_time_Tick(object sender, EventArgs e)
		{
			if (GetIdleTime() >= idle_time)
			{
				idle_time += 3600000;
				SetHook(true);
			}
			else
			{
				if (idle_time > 3600000)
				{
					idle_time = 3600000;
				}
				SetHook();
			}
		}

		private void refreshAPI_b_Click(object sender, EventArgs e)
		{
			_settingsService.Settings.TarkovTrackerApiKey = tarkovTrackerApiKey_textbox.Text;
			_settingsService.Save();
			Task.Factory.StartNew(() => Program.UpdateTarkovTrackerAPI());

			SetHook(true);
		}

		private void Tarkov_Dev_Click(object sender, EventArgs e)
		{
			Process.Start(Program.tarkov_dev);
		}

		private void TarkovTrackerCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.UseTarkovTrackerApi = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void label1_MouseHover(object sender, EventArgs e)
		{
			TarkovTrackerAPI_tooltip.SetToolTip(label1, "Use TarkovTracker.org to show only your active tasks and hideout upgrades. And hide completed.");
		}

		private void TarkovTrackerCheckBox_MouseHover(object sender, EventArgs e)
		{
			TarkovTrackerAPI_tooltip.SetToolTip(TarkovTrackerCheckBox, "Use TarkovTracker.org to show only your active tasks and hideout upgrades. And hide completed.");
		}

		private void label2_MouseHover(object sender, EventArgs e)
		{
			TarkovTrackerAPI_tooltip.SetToolTip(label2, "Go to \"https://tarkovtracker.org/settings\" to get your API key.");
		}

		private void tarkovTrackerApiKey_textbox_MouseHover(object sender, EventArgs e)
		{
			TarkovTrackerAPI_tooltip.SetToolTip(tarkovTrackerApiKey_textbox, "Go to \"https://tarkovtracker.org/settings\" to get your API key.");
		}

		private void TarkovTracker_button_Click(object sender, EventArgs e)
		{
			Process.Start(Program.tarkovtracker);
		}

		private void hideoutUpgrades_checkBox_CheckedChanged(object sender, EventArgs e)
		{
			_settingsService.Settings.ShowHideoutUpgrades = (sender as CheckBox).Checked;
			_settingsService.Save();
		}

		private void languageBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as System.Windows.Forms.ComboBox).SelectedIndex >= 0)
			{
				_settingsService.Settings.Language = (sender as System.Windows.Forms.ComboBox).SelectedItem.ToString();
			}
			else
			{
				_settingsService.Settings.Language = "en";
			}
			_settingsService.Save();
			lock (lockObject)
			{
				languageModel = null;
			}
			PaddleRecognizer(null);//init ocr

			Program.forceUpdateAPI = true;
			Program.forceUpdateTrackerAPI = true;
		}

		private void modeBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as System.Windows.Forms.ComboBox).SelectedIndex >= 0)
			{
				_settingsService.Settings.Mode = (sender as System.Windows.Forms.ComboBox).SelectedItem.ToString();
			}
			else
			{
				_settingsService.Settings.Mode = "regular";
			}
			_settingsService.Save();
			Program.forceUpdateAPI = true;
			Program.forceUpdateTrackerAPI = true;
		}
	}
}
