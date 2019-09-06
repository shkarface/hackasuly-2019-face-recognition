using HackaSuly2019.Mobile.Helpers;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HackaSuly2019.Mobile.Views
{
    public enum ReportPageState
    {
        Found = 1,
        Lost = 2
    }

    [QueryProperty(nameof(State), "state")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        private ReportPageState _state;
        public string State { set { _state = (ReportPageState)int.Parse(value); } }
        public ReportPage()
        {
            InitializeComponent();
        }

        private async void SelectImageButton_Clicked(object sender, EventArgs e)
        {
            const string Cancel = "Cancel";
            const string Camera = "Camera";
            const string Gallery = "Gallery";

            var button = await DisplayActionSheet("Where do you want to get the image?", Cancel, null, Camera, Gallery);
            if (button == Cancel) return;

            if (button == Camera)
            {
                await UploadPhoto(CrossMedia.Current.IsCameraAvailable,
                () => CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        CompressionQuality = 80
                    }));
            }
            else
            {
                await UploadPhoto(CrossMedia.Current.IsCameraAvailable,
               () => CrossMedia.Current.PickPhotoAsync(
                   new PickMediaOptions
                   {
                       PhotoSize = PhotoSize.Medium,
                       CompressionQuality = 80,
                   }));
            }
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

                profilePicture.Source = ImageSource.FromStream(() => file.GetStream());
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

        private void ReportButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}