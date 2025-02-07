using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinPrank.Pages;
using Windows.UI.WindowManagement;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinPrank
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Windows Prank";

            // 获取当前窗口的 AppWindow 实例
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            m_appWindow = AppWindow.GetFromWindowId(myWndId);

        }
        bool TrySetMicaBackdrop(bool useMicaAlt)
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                Microsoft.UI.Xaml.Media.MicaBackdrop micaBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
                micaBackdrop.Kind = useMicaAlt ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt : Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base;
                this.SystemBackdrop = micaBackdrop;

                return true; // Succeeded.
            }

            return false; // Mica is not supported on this system.
        }

        private void ShowFullScreenMedia(string mediaPath, bool isVideo)
        {
            // 根据 isVideo 判断使用哪个控件
            FrameworkElement mediaContent;
            if (isVideo)
            {
                // 创建 MediaPlayerElement 用于播放视频
                var mediaPlayerElement = new MediaPlayerElement
                {
                    AreTransportControlsEnabled = true,
                    Stretch = Stretch.UniformToFill,
                    // 注意：如果视频资源和图片资源不在同一路径下，请修改 URI
                    Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Images/{mediaPath}"))
                };
                mediaContent = mediaPlayerElement;
            }
            else
            {
                // 创建 Image 用于显示图片
                mediaContent = new Image
                {
                    Source = new BitmapImage(new Uri($"ms-appx:///Assets/Images/{mediaPath}")),
                    Stretch = Stretch.UniformToFill
                };
            }

            // 使用一个容器包装控件，使其能够接收键盘事件
            // 并设置背景为黑色，保证全屏时视觉效果良好
            var container = new Grid
            {
                Background = new SolidColorBrush(Colors.Black),
                IsTabStop = true
            };
            container.Children.Add(mediaContent);

            // 先声明全屏窗口变量，以便在 Lambda 中引用
            Window fullScreenWindow = null;

            // 注册容器的 KeyDown 事件，当按下 ESC 键时关闭全屏窗口并显示主窗口
            container.KeyDown += (s, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Escape)
                {
                    fullScreenWindow?.Close();
                    m_appWindow.Show();
                }
            };

            // 创建全屏窗口，将容器作为内容
            fullScreenWindow = new Window
            {
                Content = container,
                ExtendsContentIntoTitleBar = true
            };

            // 获取全屏窗口句柄，并设置为全屏模式
            IntPtr fsHwnd = WinRT.Interop.WindowNative.GetWindowHandle(fullScreenWindow);
            WindowId fsWindowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(fsHwnd);
            AppWindow fsAppWindow = AppWindow.GetFromWindowId(fsWindowId);
            fsAppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

            // 隐藏当前主窗口
            m_appWindow.Hide();
            // 激活全屏窗口
            fullScreenWindow.Activate();

            // 请求容器获得焦点以便接收键盘输入
            container.Focus(FocusState.Programmatic);

            // 如果是视频，则启动播放
            if (isVideo)
            {
                var mediaPlayerElement = mediaContent as MediaPlayerElement;
                mediaPlayerElement?.MediaPlayer.Play();
            }
        }

        private void nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = nav.SelectedItem as NavigationViewItem;
            if (selectedItem == null) return;

            string? selectedTag = selectedItem.Tag?.ToString();
            switch (selectedTag)
            {
                case "WindowsCrashPage":
                    if (m_windowsCrashPage is null)
                    {
                        m_windowsCrashPage = new WindowsCrashPage();
                    }

                    navContentFrame.Content = m_windowsCrashPage;


                    ShowFullScreenMedia("bule_screen_01.png", false);
                    break;
                case "WindowsUpdatePage":
                    if (m_windowsUpdatePage is null)
                    {
                        m_windowsUpdatePage = new WindowsUpdatePage();
                    }

                    navContentFrame.Content = m_windowsUpdatePage;

                    ShowFullScreenMedia("Windows_Update_Screen_1_hour_REAL_COUNT_in_4K_UHD.mkv", true);
                    break;
                case "informationPage":
                    if (m_informationPages is null)
                    {
                        m_informationPages = new InformationPages();
                    }
                    navContentFrame.Content = m_informationPages;
                    break;
                default: break;
            }
        }


        private AppWindow m_appWindow;

        private WindowsCrashPage  m_windowsCrashPage;
        private WindowsUpdatePage m_windowsUpdatePage;
        private InformationPages  m_informationPages;
    }
}
