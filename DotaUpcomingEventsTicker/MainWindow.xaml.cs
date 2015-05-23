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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region One Instance App
        private static readonly Semaphore singleInstanceWatcher;
        private static readonly bool createdNew;

        static MainWindow()
        {
            singleInstanceWatcher = new Semaphore(0, 1, "SomeUniqueStringIdentifyingMyApp", out createdNew);

            if (createdNew)
            {
                ;
            }
            else
            {
                Process current = Process.GetCurrentProcess();

                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        NativeApi.SetForegroundWindow(process.MainWindowHandle);
                        NativeApi.ShowWindow(process.MainWindowHandle, DotaUpcomingEventsTicker.Native.NativeApi.WindowShowStyle.Restore);
                        break;
                    }
                }

                Environment.Exit(-2);
            }
        }
        #endregion One Instance App

        private NotifyIcon _notificationIcon = new NotifyIcon();

        private MainViewModel _mainViewModel = new MainViewModel();
        private NotificationViewModel _notificationViewModel = new NotificationViewModel();

        private NotificationWindow _notificationWindow = new NotificationWindow();

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel.SendNotification += _mainViewModel_SendNotification;
            _mainViewModel.CloseApp += _mainViewModel_CloseApp;

            _notificationViewModel.CloseApp += _notificationViewModel_CloseApp;


            this.DataContext = _mainViewModel;

            _notificationWindow.DataContext = _notificationViewModel;

            this.LoadTrayIcon();
            this.MoveWindowPositionAboveTaskTray();
        }

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
            _notificationViewModel.CloseApp -= _notificationViewModel_CloseApp;

            _mainViewModel.Dispose();
            this._notificationWindow.Close();
            _notificationIcon.Visible = false;
            _notificationIcon.Dispose();
            base.OnClosing(e);
        }
        #endregion Overrides

        #region Private Methods
        private void LoadTrayIcon()
        {
            string assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/" + assemblyName + ";component/Resources/dota.ico")).Stream;

            _notificationIcon.Icon = new System.Drawing.Icon(iconStream);//"dota.ico");
            _notificationIcon.Visible = true;

            _notificationIcon.Click += (object sender, EventArgs args) =>
                {
                    _notificationViewModel_CloseApp(null, new EventArgs());
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            _notificationIcon.DoubleClick += (object sender, EventArgs args) =>
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            _notificationIcon.MouseDown += (object sender, System.Windows.Forms.MouseEventArgs args) =>
            {
                if (args.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    //System.Windows.Controls.ContextMenu menu = (System.Windows.Controls.ContextMenu)this.FindResource("NotifierContextMenu");
                    //menu.IsOpen = true;
                    _mainViewModel.IsContextMenuVisible = true;
                }
            };
        }
        private void MoveWindowPositionAboveTaskTray()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            _notificationWindow.Left = desktopWorkingArea.Right - _notificationWindow.Width;
            _notificationWindow.Top = desktopWorkingArea.Bottom - _notificationWindow.Height;
        }

        private void _mainViewModel_SendNotification(object sender, SendNotificationEventArgs e)
        {
            _notificationWindow.Show();
        }
        private void _mainViewModel_CloseApp(object sender, EventArgs e)
        {
            _mainViewModel.SendNotification -= _mainViewModel_SendNotification;
            _mainViewModel.SendNotification -= _mainViewModel_CloseApp;
            _notificationViewModel.CloseApp -= _notificationViewModel_CloseApp;

            _mainViewModel.Dispose();
            this._notificationWindow.Close();
            _notificationIcon.Visible = false;
            _notificationIcon.Dispose();
            this.Close();
        }
        private void _notificationViewModel_CloseApp(object sender, EventArgs e)
        {
            _notificationViewModel.Dispose();
            _notificationWindow.Hide();
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
        #endregion EventHandlers

    }
}
