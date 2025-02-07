using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinPrank.Pages;

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
                    break;
                case "WindowsUpdatePage":
                    if (m_windowsUpdatePage is null)
                    {
                        m_windowsUpdatePage = new WindowsUpdatePage();
                    }

                    navContentFrame.Content = m_windowsUpdatePage;
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


        private WindowsCrashPage  m_windowsCrashPage;
        private WindowsUpdatePage m_windowsUpdatePage;
        private InformationPages  m_informationPages;
    }
}
