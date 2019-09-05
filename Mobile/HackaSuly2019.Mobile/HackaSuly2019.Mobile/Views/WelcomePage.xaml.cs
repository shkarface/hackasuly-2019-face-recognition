using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HackaSuly2019.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void SetPhoto(MediaFile file)
        {
            if (file == null)
            {
                return;
            }

            selectedImage.Source = ImageSource.FromStream(() => file.GetStream());
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable == false)
            {
                await DisplayAlert("Not supported", "Your device does not support this functionality", "Ok");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                DefaultCamera = CameraDevice.Front
            });

            SetPhoto(file);
        }

        private async void GalleryButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsPickPhotoSupported == false)
            {
                await DisplayAlert("Not supported", "Your device does not support this functionality", "Ok");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
            });

            SetPhoto(file);
        }
    }
}