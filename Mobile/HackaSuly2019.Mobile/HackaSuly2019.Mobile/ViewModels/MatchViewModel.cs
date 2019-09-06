using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HackaSuly2019.Mobile.ViewModels
{
    public class MatchViewModel
    {
        public ImageSource Thumbnail { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public double Confidence { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ConfidencePercentage => $"{Confidence:n1}%";
    }
}
