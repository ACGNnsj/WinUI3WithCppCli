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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Web.WebView2.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebView2Page : Page
    {
        public WebView2Page()
        {
            this.InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await MyWebView2.EnsureCoreWebView2Async();
            MyWebView2.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "G:\\webstorm\\webgpu-clustered-shading\\",
                CoreWebView2HostResourceAccessKind.Allow);
            MyWebView2.Source = new Uri("https://appassets/index.html");
            MyWebView2.CoreWebView2.OpenDevToolsWindow();
        }
    }
}