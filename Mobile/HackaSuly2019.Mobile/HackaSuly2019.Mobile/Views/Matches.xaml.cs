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
        public Matches()
        {
            InitializeComponent();
            matchesCollection.ItemsSource = new List<MatchViewModel>
            {
                new MatchViewModel { Name = "Aso Ali", PhoneNumber = "0750 444 3322", Confidence = 97, Thumbnail = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/6/6c/Jeff_Bezos_at_Amazon_Spheres_Grand_Opening_in_Seattle_-_2018_%2839074799225%29_%28cropped%29.jpg"))},
                new MatchViewModel { Name = "Karwan Luqman", PhoneNumber = "0750 123 4411", Confidence = 88, Thumbnail = ImageSource.FromUri(new Uri("https://specials-images.forbesimg.com/imageserve/5c76b4b84bbe6f24ad99c370/416x416.jpg?background=000000&cropX1=0&cropX2=4000&cropY1=0&cropY2=4000"))},
                new MatchViewModel { Name = "Muhammad Khalid", PhoneNumber = "0750 321 5511", Confidence = 76.6, Thumbnail = ImageSource.FromUri(new Uri("https://content.fortune.com/wp-content/uploads/2018/07/gettyimages-961697338.jpg"))},
            };
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