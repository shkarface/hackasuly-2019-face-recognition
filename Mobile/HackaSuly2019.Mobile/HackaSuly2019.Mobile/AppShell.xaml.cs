using HackaSuly2019.Mobile.Views;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HackaSuly2019.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("report", typeof(ReportPage));
            SetTabBarIsVisible(this, false);
        }
    }
}
