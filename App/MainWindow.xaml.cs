// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.System;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using OpenCV;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private ISystemBackdropControllerWithTargets m_backdropController;
        private SystemBackdropConfiguration m_configurationSource;
        private WindowsSystemDispatcherQueueHelper m_dispatcherQueueHelper;

        public MainWindow()
        {
            this.InitializeComponent();
            TrySetSystemBackdrop();
        }

        public enum BackdropType
        {
            Mica,
            MicaAlt,
            DesktopAcrylic,
            DefaultColor,
        }

        BackdropType m_currentBackdrop;

        private bool TrySetSystemBackdrop(BackdropType type = BackdropType.Mica)
        {
            if (type is BackdropType.Mica or BackdropType.MicaAlt)
            {
                if (!MicaController.IsSupported())
                {
                    return false;
                }

                if (type == BackdropType.Mica)
                {
                    m_backdropController = new MicaController();
                }
                else
                {
                    m_backdropController = new MicaController
                    {
                        Kind = MicaKind.BaseAlt
                    };
                }
            }
            else if (type == BackdropType.DesktopAcrylic)
            {
                if (!DesktopAcrylicController.IsSupported())
                {
                    return false;
                }

                m_backdropController = new DesktopAcrylicController();
            }

            m_dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
            m_dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();
            m_configurationSource = new SystemBackdropConfiguration();
            this.Activated += Window_Activated;
            this.Closed += Window_Closed;
            ((FrameworkElement) this.Content).ActualThemeChanged += Window_ThemeChanged;
            m_configurationSource.IsInputActive = true;
            SetConfigurationSourceTheme();

            /*m_backdropController = new MicaController()
            {
                Kind = MicaKind.BaseAlt
            };*/
            m_backdropController.AddSystemBackdropTarget(
                CastExtensions.As<ICompositionSupportsSystemBackdrop>(this));
            m_backdropController.SetSystemBackdropConfiguration(m_configurationSource);
            return true;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            var path = Process.GetCurrentProcess().MainModule.FileName;
            path = Path.GetDirectoryName(path);
            Img img = new Img();
            img.showImgDefault(path);
            myButton.Content = $"Path: {path}";
        }

        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a file picker
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var path = file.Path;
                PickAFileOutputTextBlock.Text = "Picked file: " + path;
                Img img = new Img();
                img.showImg(path);
            }
            else
            {
                PickAFileOutputTextBlock.Text = "Operation cancelled.";
            }
        }

        void ChangeBackdropButton_Click(object sender, RoutedEventArgs e)
        {
            BackdropType newType;
            switch (m_currentBackdrop)
            {
                case BackdropType.Mica:
                    newType = BackdropType.MicaAlt;
                    break;
                case BackdropType.MicaAlt:
                    newType = BackdropType.DesktopAcrylic;
                    break;
                case BackdropType.DesktopAcrylic:
                    newType = BackdropType.DefaultColor;
                    break;
                default:
                case BackdropType.DefaultColor:
                    newType = BackdropType.Mica;
                    break;
            }

            SetBackdrop(newType);
        }

        void SetBackdrop(BackdropType type)
        {
            m_currentBackdrop = BackdropType.DefaultColor;
            tbCurrentBackdrop.Text = "None (default theme color)";
            tbChangeStatus.Text = "";
            if (m_backdropController != null)
            {
                m_backdropController.Dispose();
                m_backdropController = null;
            }

            this.Activated -= Window_Activated;
            this.Closed -= Window_Closed;
            ((FrameworkElement) this.Content).ActualThemeChanged -= Window_ThemeChanged;
            m_configurationSource = null;
            if (type != BackdropType.DefaultColor)
            {
                if (TrySetSystemBackdrop(type))
                {
                    tbCurrentBackdrop.Text = Enum.GetName(typeof(BackdropType), type);
                    m_currentBackdrop = type;
                }
                else
                {
                    // Mica isn't supported. Try Acrylic.
                    type = BackdropType.DesktopAcrylic;
                    tbChangeStatus.Text +=
                        $"  {Enum.GetName(typeof(BackdropType), type)} isn't supported. Trying Acrylic.";
                }
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed
            // so it doesn't try to use this closed window.
            if (m_backdropController != null)
            {
                m_backdropController.Dispose();
                m_backdropController = null;
            }

            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement) this.Content).ActualTheme)
            {
                case ElementTheme.Dark:
                    m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark;
                    break;
                case ElementTheme.Light:
                    m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light;
                    break;
                case ElementTheme.Default:
                    m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default;
                    break;
            }
        }
    }


    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options,
            [In, Out, MarshalAs(UnmanagedType.IUnknown)]
            ref object dispatcherQueueController);

        object m_dispatcherQueueController = null;

        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2; // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }
    }
}