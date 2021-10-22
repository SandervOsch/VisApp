using Newtonsoft.Json;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace Vis_app
{
    class Settings : ContentPage
    {
        Picker LengthPicker = new Picker();
        Picker DatePick = new Picker();
        public Settings(Instellingen UserSettings)
        {
            Label FormatLabel = new Label
            {
                Text = "Instellingen",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            Label LengthFormatLabel = new Label
            {
                Text = "Lengtematen:",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(5, 2, 0, 0)
            };

            Label DateFormatLabel = new Label
            {
                Text = "Datumnotatie:",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(5, 2, 0, 0)
            };

            Label Credits = new Label
            {
                Text = "Deze app is gemaakt door Sander van Osch, neem contact op als er iets mis gaat met de app naar sandervanosch@msn.com",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(2, 2, 0, 0)
            };


            LengthPicker = new Picker
            {
                ItemsSource = new string[] { "Centimeter", "Inches" },
                HorizontalOptions = LayoutOptions.Center,

                SelectedItem = UserSettings.LengthFormat,

                Title = "Lengtematen",

                WidthRequest = 225,
            };

            DatePick = new Picker
            {
                ItemsSource = new string[] { "MM/DD/JJJJ", "DD/MM/JJJJ", "JJJJ/MM/DD", "Maand D, Jr" },
                HorizontalOptions = LayoutOptions.Center,

                SelectedItem = UserSettings.DateFormat,

                Title = "Datum",

                WidthRequest = 225,
            };

            Button SaveInst = new Button
            {
                Text = "Opslaan",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White,
                HeightRequest = 50,
                WidthRequest = 220
            };
            SaveInst.Clicked += SaveInst_Clicked;

            StackLayout SaveInstLayout = new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, 2),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    SaveInst
                },
            };

            StackLayout LengthLayout = new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, 10),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Start,
                Children =
                {
                    LengthFormatLabel,
                    LengthPicker,
                    DateFormatLabel,
                    DatePick
                },
            };

            StackLayout LineSeparator = new StackLayout()
            {
                Padding = new Thickness(10, 10, 10, 0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new BoxView()
                    {
                        Color = Color.Gray,
                        HeightRequest = 1,
                        Opacity = 0.5
                    }
                }
            };

            StackLayout layout = new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, 10),
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    FormatLabel,
                    LengthLayout,
                    LineSeparator,
                    SaveInstLayout,
                    Credits
                },
            };

            ScrollView ScrollViewContent = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = layout
            };

            Content = ScrollViewContent;
        }
        //this just saves the settings the user picked in the instellingen.json file
        private async void SaveInst_Clicked(object sender, EventArgs e)
        {
            Instellingen newInst = new Instellingen();

            string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "instellingen.json");

            newInst.LengthFormat = LengthPicker.SelectedItem.ToString();
            newInst.DateFormat = DatePick.SelectedItem.ToString();

            string json = JsonConvert.SerializeObject(newInst);

            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                sw.WriteLine(json);
                sw.Close();
                sw.Dispose();
            }

            

            await Navigation.PopToRootAsync();
            await Navigation.PushAsync(new Homepage());
        }
    }
}
