using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace Vis_app
{
    class FishInfo : ContentPage
    {
        LocalFish local = new LocalFish();

        readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "vissen.json");
        public FishInfo(LocalFish SelectedFish)
        {
            BackgroundColor = Color.FromHex("#e8f0ff");

            local = SelectedFish;

            Label header = new Label
            {
                Text = "Vis Info",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            Label FishPickerLabel = new Label
            {
                Text = "Vis soort:",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(5, 2, 0, 0)
            };

            Label DateEntryLabel = new Label
            {
                Text = "Datum:",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(5, 2, 0, 0)
            };

            Label LengthEntryLabel = new Label
            {
                Text = "Lengte:",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(5, 2, 0, 0)
            };


            Image PhotoImage = new Image()
            {
                Source = SelectedFish.FishImage,
                Aspect = Aspect.AspectFit,
                HeightRequest = 300,
                WidthRequest = 300,
            };

            Entry DateEntry = new Entry
            {
                Text = SelectedFish.CatchDate.ToString(),
                IsReadOnly = true,
                WidthRequest = 250,

            };

            Entry LengthEntry = new Entry
            {
                Text = SelectedFish.FishLength,
                WidthRequest = 250,
                IsReadOnly = true,
            };

            Entry FishPicker = new Entry
            {
                Text = SelectedFish.FishName,
                IsReadOnly = true,
                WidthRequest = 250,
            };

            Button DeleteButton = new Button
            {
                Text = "Verwijderen",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White,
                HeightRequest = 50,
                WidthRequest = 220
            };
            DeleteButton.Clicked += DeleteButton_Clicked;

            StackLayout PhotoLayout = new StackLayout()
            {
                Padding = new Thickness(10, 0, 10, 0),
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    PhotoImage,
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

            StackLayout EntryLayout = new StackLayout()
            {
                Padding = new Thickness(0, 10, 0, 10),
                HorizontalOptions = LayoutOptions.Start,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    FishPickerLabel,
                    FishPicker,
                    LengthEntryLabel,
                    LengthEntry,
                    DateEntryLabel,
                    DateEntry,
                },
            };

            StackLayout ButtonLayout = new StackLayout()
            {
                Padding = new Thickness(0, 10, 0, 10),
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    DeleteButton,
                }
            };

            StackLayout AddFishPage = new StackLayout()
            {
                Children =
                {
                    header,
                    PhotoLayout,
                    LineSeparator,
                    EntryLayout,
                    ButtonLayout
                }
            };

            ScrollView ScrollViewContent = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = AddFishPage
            };

            Content = ScrollViewContent;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            bool Message = await DisplayAlert("Vis info verwijderen", "Weet u zeker dat u deze vis wilt verwijderen?", "Ja", "Nee");

            //this all speaks for itself, if the user presses okay for the above message, the bool Message turns true and the if statement continues by removing from the json
            if (Message)
            {
                string jsonData = File.ReadAllText(FilePath);

                List<Fish> FishList = JsonConvert.DeserializeObject<List<Fish>>(jsonData);

                int index = 0; 
                foreach(Fish f in FishList)
                {
                    if (f.FishImage != local.FishImage)
                        index++;
                    else
                        break;
                }
                FishList.RemoveAt(index);

                string newJson = JsonConvert.SerializeObject(FishList);
                File.WriteAllText(FilePath, newJson);
          
                await Navigation.PopToRootAsync();
                await Navigation.PushAsync(new Homepage());
            }
        }
    }
}
