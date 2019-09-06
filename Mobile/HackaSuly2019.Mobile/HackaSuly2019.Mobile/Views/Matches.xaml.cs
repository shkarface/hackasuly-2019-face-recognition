using HackaSuly2019.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HackaSuly2019.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Matches : ContentPage
    {
        public Matches(IEnumerable<MatchViewModel> matches)
        {
            InitializeComponent();
            matchesCollection.ItemsSource = matches;
            errorMessage.IsVisible = false;
        }

        public Matches(string error)
        {
            InitializeComponent();
            matchesCollection.IsVisible = false;
            errorMessage.Text = error;
        }

        private async void CallButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var match = button.BindingContext as MatchViewModel;

            try
            {
                PhoneDialer.Open(match.PhoneNumber);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Could not call person", ex.Message, "OK");
            }
        }
    }
}