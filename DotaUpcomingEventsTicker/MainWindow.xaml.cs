using DotaUpcomingEventsTicker.Native;
using DotaUpcomingEventsTicker.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace DotaUpcomingEventsTicker
{
    public partial class MainWindow : Window
    {
        #region Statics
        #region One Instance App
        private static readonly Semaphore singleInstanceWatcher;
        private static readonly bool createdNew;
        #endregion One Instance App

        private static void FocusAndRestoreProcessWindow()
        {
            Process current = Process.GetCurrentProcess();
            NativeApi.SetForegroundWindow(current.MainWindowHandle);
            NativeApi.ShowWindow(current.MainWindowHandle, DotaUpcomingEventsTicker.Native.NativeApi.WindowShowStyle.Restore);
            NativeApi.SendMessage(current.MainWindowHandle, NativeApi.WM_SYSCOMMAND, NativeApi.SC_RESTORE, 0);
        }
        #endregion Statics

        #region Members
        private NotifyIcon _trayIcon = new NotifyIcon();
        private MainViewModel _mainViewModel = new MainViewModel();
        private NotificationViewModel _notificationViewModel = new NotificationViewModel();
        private NotificationWindow _notificationWindow = new NotificationWindow();
        #endregion Members

        #region Constructors
        static MainWindow()
        {
            singleInstanceWatcher = new Semaphore(0, 1, "DotaUpcomingEventsTicker-e56182e4-7b09-4d0e-8257-e9789e450067", out createdNew);

            if (!createdNew)
            {
                MainWindow.FocusAndRestoreProcessWindow();
                Environment.Exit(-2);
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel.SendNotification += _mainViewModel_SendNotification;
            _mainViewModel.CloseApp += _mainViewModel_CloseApp;
            _notificationViewModel.CloseNotification += _notificationViewModel_CloseNotification;


            this.DataContext = _mainViewModel;

            _notificationWindow.DataContext = _notificationViewModel;

            this.LoadTrayIcon();
            this.MoveWindowPositionAboveTaskTray();
        }
        #endregion Constructors

        #region Overrides
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }

            base.OnStateChanged(e);
        }
        protected override void OnDeactivated(EventArgs e)
        {
            _mainViewModel.IsParentWindowDeactivated = true;
            base.OnDeactivated(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            _mainViewModel.IsParentWindowDeactivated = false;
            base.OnActivated(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            _mainViewModel.SendNotification -= _mainViewModel_SendNotification;
            _mainViewModel.SendNotification -= _mainViewModel_CloseApp;
            _notificationViewModel.CloseNotification -= _notificationViewModel_CloseNotification;

            _mainViewModel.Dispose();
            this._notificationWindow.Close();
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            base.OnClosing(e);
        }
        #endregion Overrides

        #region Private Methods
        private void LoadTrayIcon()
        {
            string assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/" + assemblyName + ";component/Resources/dota.ico")).Stream;

            _trayIcon.Icon = new System.Drawing.Icon(iconStream);
            _trayIcon.Visible = true;

            _trayIcon.Click += this.OnClickTrayIcon;

            //_trayIcon.MouseDown += (object sender, System.Windows.Forms.MouseEventArgs args) =>
            //{
            //    if (args.Button == System.Windows.Forms.MouseButtons.Right)
            //    {
            //        //System.Windows.Controls.ContextMenu menu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifierContextMenu");
            //        //menu.IsOpen = true;
            //        //_mainViewModel.IsContextMenuVisible = true;
            //    }
            //};
        }
        private void MoveWindowPositionAboveTaskTray()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            _notificationWindow.Left = desktopWorkingArea.Right - _notificationWindow.Width;
            _notificationWindow.Top = desktopWorkingArea.Bottom - _notificationWindow.Height;
        }
        #endregion Private Methods

        #region EventHandlers
        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            //FieldsRadioButton.IsChecked = ((ToggleButton)sender).IsChecked;
            //ButtonsRadioButton.IsChecked = !((ToggleButton)sender).IsChecked;
        }
        private void HideClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void _mainViewModel_SendNotification(object sender, SendNotificationEventArgs e)
        {
            //TODO;
            _notificationViewModel.Match = e.Match;
            _notificationWindow.Show();
        }
        private void _mainViewModel_CloseApp(object sender, EventArgs e)
        {
            _mainViewModel.SendNotification -= _mainViewModel_SendNotification;
            _mainViewModel.SendNotification -= _mainViewModel_CloseApp;
            _notificationViewModel.CloseNotification -= _notificationViewModel_CloseNotification;

            _mainViewModel.Dispose();
            this._notificationWindow.Close();

            _trayIcon.Click -= this.OnClickTrayIcon;
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            this.Close();
        }
        private void _notificationViewModel_CloseNotification(object sender, SendNotificationEventArgs e)
        {
            _notificationViewModel.Dispose();
            _notificationWindow.Hide();

            _mainViewModel.RemoveMatchFromNotificationsList(e.Match);
        }
        private void OnClickTrayIcon(object sender, EventArgs args)
        {
            _notificationViewModel_CloseNotification(null, new SendNotificationEventArgs());
            MainWindow.FocusAndRestoreProcessWindow();
            this.Show();
            this.WindowState = WindowState.Normal;
        }
        #endregion EventHandlers
        
    }
}
