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
    public enum ReportPageState
    {
        Found = 1,
        Lost = 2
    }

    [QueryProperty(nameof(State), "state")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        private Stream _image;

        private ReportPageState _state;
        public string State
        {
            set
            {
                _state = (ReportPageState)int.Parse(value);
                if (_state == ReportPageState.Found)
                {
                    Title = "I've found someone";
                }
                else
                {
                    Title = "Report a missing person";
                }
            }
        }
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
                _image = file.GetStream();
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
            selectImageButton.IsEnabled = false;
            reportButton.IsEnabled = false;
            progressGrid.IsVisible = true;
        }
        private void SetNotBusy()
        {
            selectImageButton.IsEnabled = true;
            reportButton.IsEnabled = true;
            progressGrid.IsVisible = false;
        }

        private async void ReportButton_Clicked(object sender, EventArgs e)
        {
            if (_image is null)
            {
                await DisplayAlert("Please select an image", "Please select an image from camera or gallery.", "Ok");
                return;
            }

            var missingFields = new List<string>();
            if (string.IsNullOrEmpty(nameEntry.Text))
                missingFields.Add("Name");

            if (string.IsNullOrEmpty(phoneEntry.Text))
                missingFields.Add("Phone number");

            if (maleRadioButton.IsChecked == false && femaleRadioButton.IsChecked == false)
                missingFields.Add("Missing person's sex");

            if (missingFields.Any())
            {
                await DisplayAlert("Required Fields", $"Please fill in the required fields: {string.Join(", ", missingFields)}", "Ok");
                return;
            }

            try
            {
                SetBusy();

                var uri = await ImageUploadHelper.UploadFileAsync(_image);

                await Xamarin.Essentials.Browser.OpenAsync(uri.AbsoluteUri);
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
    }
}