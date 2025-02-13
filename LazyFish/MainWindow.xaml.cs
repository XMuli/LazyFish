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
using LazyFish.Pages;
using Windows.UI.WindowManagement;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using Windows.Media.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LazyFish
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

            this.AppWindow.SetIcon("Assets/Images/logo.ico");

            // һ��ʼ�ͽ���������Ϊ��Ϣҳ��
            m_informationPages = new InformationPages();
            navContentFrame.Content = m_informationPages;

            // ��ȡ��ǰ���ڵ� AppWindow ʵ��
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
            // ���� isVideo �ж�ʹ���ĸ��ؼ�
            FrameworkElement mediaContent;
            if (isVideo)
            {
                // ���� MediaPlayerElement ���ڲ�����Ƶ
                var mediaPlayerElement = new MediaPlayerElement
                {
                    AreTransportControlsEnabled = true,
                    Stretch = Stretch.UniformToFill,
                    // ע�⣺�����Ƶ��Դ��ͼƬ��Դ����ͬһ·���£����޸� URI
                    Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Images/{mediaPath}"))
                };
                mediaContent = mediaPlayerElement;
            }
            else
            {
                // ���� Image ������ʾͼƬ
                mediaContent = new Image
                {
                    Source = new BitmapImage(new Uri($"ms-appx:///Assets/Images/{mediaPath}")),
                    Stretch = Stretch.UniformToFill
                };
            }

            // ʹ��һ��������װ�ؼ���ʹ���ܹ����ռ����¼�
            // �����ñ���Ϊ��ɫ����֤ȫ��ʱ�Ӿ�Ч������
            var container = new Grid
            {
                Background = new SolidColorBrush(Colors.Black),
                IsTabStop = true
            };
            container.Children.Add(mediaContent);

            // ������ȫ�����ڱ������Ա��� Lambda ������
            Window fullScreenWindow = null;

            // ע�������� KeyDown �¼��������� ESC ��ʱ�ر�ȫ�����ڲ���ʾ������
            container.KeyDown += (s, e) =>
            {
                if (e.Key == Windows.System.VirtualKey.Escape)
                {
                    fullScreenWindow?.Close();
                    m_appWindow.Show();
                }
            };

            // ����ȫ�����ڣ���������Ϊ����
            fullScreenWindow = new Window
            {
                Content = container,
                ExtendsContentIntoTitleBar = true
            };

            // ��ȡȫ�����ھ����������Ϊȫ��ģʽ
            IntPtr fsHwnd = WinRT.Interop.WindowNative.GetWindowHandle(fullScreenWindow);
            WindowId fsWindowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(fsHwnd);
            AppWindow fsAppWindow = AppWindow.GetFromWindowId(fsWindowId);
            fsAppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

            // ���ص�ǰ������
            m_appWindow.Hide();
            // ����ȫ������
            fullScreenWindow.Activate();

            // ����������ý����Ա���ռ�������
            container.Focus(FocusState.Programmatic);

            // �������Ƶ������������
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

                    ShowFullScreenMedia("Windows_Update_Screen_1_hour_REAL_COUNT_in_4K_UHD.webm", true);
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

        private AppWindow?        m_appWindow;
        private WindowsCrashPage?  m_windowsCrashPage;
        private WindowsUpdatePage? m_windowsUpdatePage;
        private InformationPages?  m_informationPages;
    }
}
