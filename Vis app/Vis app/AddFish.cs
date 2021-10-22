using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Vis_app
{
    class AddFish : ContentPage
    {
        private bool EnableButtons = true;
        string[] FishPickerList = { "Snoek", "Snoekbaars", "Baars", "Bot", "Meerval", "Roofblei", "Steur"  };

        Image PhotoImage = new Image();
        StackLayout PhotoLayout = new StackLayout();
        MediaFile photo;

        Picker FishPicker = new Picker();

        Button SaveButton = new Button();

        Entry DateEntry = new Entry();
        Entry LengthEntry = new Entry();

        Instellingen settings = new Instellingen();

        string length;

        readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "vissen.json");
        public AddFish(Instellingen UserSettings)
        {
            EnableButtons = true;
            BackgroundColor = Color.FromHex("#e8f0ff");
            settings = UserSettings;

            Label header = new Label
            {
                Text = "Voeg een nieuwe vangst toe",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };


            Label FishPickerLabel = new Label
            {
                Text = "Soort vis:",
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

            PhotoImage = new Image()
            {
                HeightRequest = 300,
                Aspect = Aspect.AspectFit
            };


            DateEntry = new Entry
            {
                Text = ListPage.FormatDate(DateTime.Now, UserSettings),
                IsReadOnly = true,
                WidthRequest = 250,
                TextColor = Color.Gray,
                IsEnabled = false
                
            };

            if (UserSettings.LengthFormat == "Centimeter")
                length = "cm";
            else
                length = "inch";

            LengthEntry = new Entry
            {
                Placeholder = "00 " + length,
                Keyboard = Keyboard.Numeric,
                WidthRequest = 250,
            };


            FishPicker = new Picker
            {
                ItemsSource = FishPickerList,
                Title = "Soort",
                WidthRequest = 250
            };


            Button CameraButton = new Button
            {
                Text = "Camera",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White,
                HeightRequest = 50,
                WidthRequest = 150,
                IsEnabled = AreButtonsEnabled
            };
            CameraButton.Clicked += CameraButton_Clicked;

            Button GalleryButton = new Button
            {
                Text = "Gallerij",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White,
                HeightRequest = 50,
                WidthRequest = 150,
                IsEnabled = AreButtonsEnabled
            };
            GalleryButton.Clicked += GalleryButton_Clicked;

            SaveButton = new Button
            {
                Text = "Opslaan",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White,
                HeightRequest = 50,
                WidthRequest = 220,
                IsEnabled = AreButtonsEnabled
            };
            SaveButton.Clicked += SaveButton_Clicked;


            PhotoLayout = new StackLayout()
            {
                Padding = new Thickness(10, 0, 10, 0),
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Opacity = 50,
                IsVisible = false,
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

            StackLayout PhotoButtonLayout = new StackLayout()
            {
                Padding = new Thickness(0, 10, 0, 10),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    GalleryButton,
                    CameraButton,                   
                },
            };

            StackLayout SaveButtonLayout = new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, 10),
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    SaveButton
                },
            };

            StackLayout AddFishPage = new StackLayout()
            {
                Children =
                {
                    header,
                    PhotoLayout,
                    LineSeparator,
                    EntryLayout,
                    PhotoButtonLayout,
                    SaveButtonLayout                   
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

        //this is for the buttons, so the buttons are able to be disabled as a whole when 1 button is pressed, so a user cant press more than 1 button at a time
        public bool AreButtonsEnabled
        {
            get { return EnableButtons; }

            set
            {
                if (EnableButtons != value)
                {
                    EnableButtons = value;
                    OnPropertyChanged(nameof(AreButtonsEnabled));
                }
            }
        }

        private async void GalleryButton_Clicked(object sender, EventArgs e)
        {
            if (EnableButtons)
            {
                EnableButtons = false;
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("Geen camera", "Geen camera beschikbaar", "Oke");
                    return;
                }
                else
                {
                    try
                    {
                        //Checks the permissions for the camera
                        Plugin.Permissions.Abstractions.PermissionStatus cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
                        if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                        {
                            //App doesnt have permission, requesting here
                            Plugin.Permissions.Abstractions.PermissionStatus results = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                            cameraStatus = results;
                            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                            {
                                await DisplayAlert("Geen toestemming", "App heeft geen toesteeming om de gallerij te openen", "Oke");
                                return;
                            }
                        }
                        photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                        {
                            // set the size to small so the listview doesnt throw an exception
                            PhotoSize = PhotoSize.Small,
                        });
                    }
                    catch
                    {
                        await DisplayAlert("Fout!", "De app is even bezig", "Oke");
                        return;
                    }
                }

                // check if the photo had been selected
                if (photo != null)
                {
                    PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                    PhotoImage.Aspect = Aspect.AspectFit;
                    PhotoLayout.IsVisible = true;
                }
                EnableButtons = true;
            }
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            if (EnableButtons)
            {
                EnableButtons = false;
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("Geen camera", "Geen camera beschikbaar", "Oke");
                    return;
                }
                else
                {
                    try
                    {
                        //Checks the permissions for the camera
                        Plugin.Permissions.Abstractions.PermissionStatus cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
                        if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                        {
                            //App doesnt have permission, requesting here
                            Plugin.Permissions.Abstractions.PermissionStatus results = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                            cameraStatus = results;
                            if (cameraStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                            {
                                await DisplayAlert("Geen toestemming", "App heeft geen toesteeming om de camera te openen", "Oke");
                                return;
                            }
                        }
                        //Accesses Media Plugin's TakePhotoAsync method to take the photo and put it into a specific folder
                        photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                        {
                            Directory = "Vis Fotos",
                            SaveToAlbum = true,
                            DefaultCamera = CameraDevice.Front,
                            // set the size to small so the listview does throw an exception
                            PhotoSize = PhotoSize.Small
                        });

                        // check if the photo had been selected
                        if (photo != null)
                        {
                            PhotoImage.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                            PhotoImage.Aspect = Aspect.AspectFit;
                            PhotoLayout.IsVisible = true;
                        }
                    }
                    catch
                    {
                        await DisplayAlert("Fout!", "De app is even bezig", "Oke");
                        return;
                    }
                }
                EnableButtons = true;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (EnableButtons)
            {
                Fish newFish = new Fish();
                List<Fish> FishList = new List<Fish>();

                // Check if the entries are filled properly to save
                if (FishPicker.SelectedItem != null && FishPicker.SelectedItem.ToString() != "")
                    newFish.FishName = FishPicker.SelectedItem.ToString();
                else
                {
                    await DisplayAlert("Fout!", "Vis soort niet geselecteerd", "Oke");
                    return;
                }

                if (LengthEntry.Text != null && LengthEntry.Text != "")
                {
                    if (decimal.TryParse(LengthEntry.Text, out decimal result))
                        if (result > 0)
                        {

                            if (settings.LengthFormat == "Centimeter")
                            {
                                newFish.FishLengthCm = result;
                                // convert the result into the other system by deviding or multiplying by 2.54
                                result /= Convert.ToDecimal(2.54);

                                newFish.FishLengthInch = result;
                            }
                            else
                            {
                                newFish.FishLengthInch = result;
                                result *= Convert.ToDecimal(2.54);

                                newFish.FishLengthCm = result;
                            }
                        }
                        else
                        {
                            await DisplayAlert("Fout!", "De ingevulde lengte moet een cijfer hoger dan 0 zijn", "Oke");
                            return;
                        }
                    else
                    {
                        await DisplayAlert("Fout!", "De ingevulde lengte is geen cijfer", "Oke");
                        return;
                    }
                }
                else
                {
                    await DisplayAlert("Fout!", "Vis lengte niet ingevult", "Oke");
                    return;
                }

                if (DateEntry.Text != null && DateEntry.Text != "")
                    newFish.CatchDate = DateTime.Now;
                else
                {
                    // thing shouldnt happen because the entry is filled up automaticly but still send a message if for whatever reason it is empty
                    await DisplayAlert("Fout!", "Als je dit ziet laat me meteen weten wat er gebeurd is pls", "Oke");
                    return;
                }

                if (photo != null && photo.Path != "")
                    newFish.FishImage = photo.Path;
                else
                {
                    await DisplayAlert("Fout!", "Geen foto gekozen", "Oke");
                    return;
                }
                string jsonData = File.ReadAllText(FilePath);
                if (jsonData != "")
                {
                    FishList = JsonConvert.DeserializeObject<List<Fish>>(jsonData);
                }
                FishList.Add(newFish);


                string json = JsonConvert.SerializeObject(FishList);
                File.WriteAllText(FilePath, json);

                await Navigation.PopToRootAsync();
                await Navigation.PushAsync(new Homepage());
            }
        }       
    }
}
