using HackaSuly2019.Mobile.Helpers;
using HackaSuly2019.Mobile.ViewModels;
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

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        private Stream _image;

        private ReportPageState _state;

        public ReportPage(ReportPageState state)
        {
            _state = state;

            if (_state == ReportPageState.Found)
                Title = "Found";
            else
                Title = "Lost";

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

                var input = new Models.Person
                {
                    ContactPhone = phoneEntry.Text,
                    Gender = maleRadioButton.IsChecked ? MissingPeople.Models.Gender.Male : MissingPeople.Models.Gender.Female,
                    ImageURL = uri.AbsoluteUri,
                    Name = nameEntry.Text,
                };

                var task = _state == ReportPageState.Found ?
                    ImageUploadHelper.ReportFoundPerson(input) :
                    ImageUploadHelper.ReportMissingPerson(input);

                var person = await task;

                if (person is null)
                {
                    await DisplayAlert("Could not scan image!", "Could not scan image, please try again later.", "Okay");
                    return;
                }

                var matches = person.SimilarPeople?.Select(p => new MatchViewModel
                {
                    Confidence = p.Similarity * 100,
                    CreatedAt = p.CreatedAt,
                    Name = p.Name,
                    PhoneNumber = p.ContactPhone,
                    Thumbnail = ImageSource.FromUri(new Uri(p.ImageURL)),
                });

                string error = null;

                if (matches is null || !matches.Any())
                {
                    error = _state == ReportPageState.Found ?
                        "This person has not been registered as a missing person." :
                        "This person has not been found by anyone yet. Please check again at a later time.";
                }

                var page = error is null ? new Matches(matches) : new Matches(error);

                await (App.Current.MainPage as NavigationPage).PushAsync(page);
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