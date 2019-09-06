using HackaSuly2019.Mobile.Helpers;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            await UploadPhoto(CrossMedia.Current.IsCameraAvailable,
                () => CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        CompressionQuality = 80
                    }));
        }

        private async Task UploadPhoto(bool isSupported, Func<Task<MediaFile>> taskFactory)
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (isSupported == false)
                {
                    await DisplayAlert("Not supported", "Your device does not support this functionality", "Ok");
                    return;
                }

                var task = taskFactory();

                var file = await task;

                if (file == null)
                {
                    return;
                }

                SetBusy();

                var watch = new Stopwatch();
                watch.Start();

                var stream = file.GetStream();

                var uri = await ImageUploadHelper.UploadFileAsync(stream);
                watch.Stop();

                await Xamarin.Essentials.Browser.OpenAsync(uri.AbsoluteUri);

                await DisplayAlert($"File uploaded! ({watch.ElapsedMilliseconds:n0} ms)", uri.ToString(), "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                SetNotBusy();
            }
        }

        private void SetBusy()
        {
            //cameraButton.IsEnabled = false;
            //galleryButton.IsEnabled = false;
            //progressGrid.IsVisible = true;
        }
        private void SetNotBusy()
        {
            //cameraButton.IsEnabled = true;
            //galleryButton.IsEnabled = true;
            //progressGrid.IsVisible = false;
        }

        private async void GalleryButton_Clicked(object sender, EventArgs e)
        {

        }

        private async void FoundFrame_Tapped(object sender, EventArgs e)
        {
            await (App.Current.MainPage as NavigationPage).PushAsync(new ReportPage(ReportPageState.Found));
        }

        private async void LostFrame_Tapped(object sender, EventArgs e)
        {
            await (App.Current.MainPage as NavigationPage).PushAsync(new ReportPage(ReportPageState.Lost));
        }
    }
}