using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace Vis_app
{
    public class Homepage : ContentPage
    {
        private bool EnableButtons = true;

        Instellingen UserSettings = new Instellingen();
        public Homepage()
        {            
            BackgroundColor = Color.FromHex("#e8f0ff");

            Label header = new Label()
            {
                Text = "HvH Wedstrijd App",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                Padding = new Thickness(0, 5)
            };

            Image logo = new Image()
            {
                Source = "Logo_HvH.jpg",
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 5),
                HeightRequest = 200
            };


            Button startHomepageButton = new Button
            {
                Text = "Vis lijst",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = 20,
                TextColor = Color.White,
                HeightRequest = 60,
                WidthRequest = 240,
                IsEnabled = AreButtonsEnabled
            };
            startHomepageButton.Clicked += StartHomepageButton_Clicked;

            Button AddHomepageButton = new Button
            {
                Text = "Vis toevoegen",               
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = 20,
                TextColor = Color.White,
                HeightRequest = 60,
                IsEnabled = AreButtonsEnabled
            };
            AddHomepageButton.Clicked += AddHomepageButton_Clicked;

            Button settingsHomepageButton = new Button
            {
                Text = "Intellingen",
                BackgroundColor = Color.FromHex("#4dacff"),
                FontSize = 20,
                TextColor = Color.White,
                HeightRequest = 60,
                IsEnabled = AreButtonsEnabled
            };
            settingsHomepageButton.Clicked += SettingsHomepageButton_Clicked;

            StackLayout buttonsHomepageLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0, 10, 0, 10),
                Children =
                {
                    startHomepageButton,
                    AddHomepageButton,
                    settingsHomepageButton
                },
            };

            StackLayout homepageLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    header,
                    logo,
                    buttonsHomepageLayout
                },
            };

            ScrollView ScrollViewContent = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                
                Content = homepageLayout
            };
            Content = ScrollViewContent;
        }

        protected override async void OnAppearing()
        {
            bool task = false;
            task = await Task.Run(() => GetPermissions());

            base.OnAppearing();
            await GetUserSettings();
        }

        private async void StartHomepageButton_Clicked(object sender, EventArgs e)
        {
            if (AreButtonsEnabled)
            {
                AreButtonsEnabled = false;
                List<Fish> sendList = new List<Fish>();

                string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "vissen.json");

                //Check if the file exists just to be sure, this should always turn true tho because the file is created the first time the app launches
                if (File.Exists(FilePath))
                {
                    try
                    {
                        //Check if there is permission to use the storage to access the file for the stream
                        PermissionStatus storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
                        if (storageStatus == PermissionStatus.Granted)
                        {
                            using (StreamReader sr = new StreamReader(FilePath))
                            {
                                string jsonData = sr.ReadToEnd();
                                sendList = JsonConvert.DeserializeObject<List<Fish>>(jsonData);
                                sr.Close();
                                sr.Dispose();
                            }

                            //if the json doesnt have any content in it, for example on the first time launch of the app the sendList is null, so to avoid an exception just a double check for it
                            if (sendList == null)
                            {
                                sendList = new List<Fish>();
                            }
                        }
                    }
                    catch { await DisplayAlert("Fout!", "Fout met ophalen van de vis lijst, check de app's toestemmingen in uw mobiel's instellingen of deze aan staan, anders kan de app niet goed werken", "Oke"); }

                    await Navigation.PushAsync(new ListPage(sendList, UserSettings));
                }
                EnableButtons = true;
            }
        }

        private async void AddHomepageButton_Clicked(object sender, EventArgs e)
        {
            if (AreButtonsEnabled)
            {
                AreButtonsEnabled = false;
                await Navigation.PushAsync(new AddFish(UserSettings));
                EnableButtons = true;
            }
        }
        private async void SettingsHomepageButton_Clicked(object sender, EventArgs e)
        {
            if (AreButtonsEnabled)
            {
                AreButtonsEnabled = false;
                await Navigation.PushAsync(new Settings(UserSettings));
                EnableButtons = true;
            }
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

        public async Task<bool> GetPermissions()
        {
            bool permissionsGranted = false;

            //add the permission to check in a list
            var permissionsStartList = new List<Permission>()
            {
                Permission.Storage,
                Permission.Camera
            };

            var permissionsNeededList = new List<Permission>();

            //this foreach puts checks all the premissions in the list and sorts then so only the ones that dont have access are being asked
            foreach (var permission in permissionsStartList)
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                if (status != PermissionStatus.Granted)
                {
                    permissionsNeededList.Add(permission);
                }
            }
            
            //had a problem where is would return false because there werent any premissions to check so all the code would just skip stuff, so here is just skips the checking of
            //the permissions if the permissionsNeededList doesnt have anything in it
            if (permissionsNeededList.Count == 0)
                return true;

            try
            {
                //it requests the permissions here
                var results = await Task.Run(() => CrossPermissions.Current.RequestPermissionsAsync(permissionsNeededList.ToArray()));

                foreach (var permission in permissionsNeededList)
                {
                    var status = PermissionStatus.Unknown;

                    //Best practice to always check that the key exists
                    if (results.ContainsKey(permission))
                        status = results[permission];

                    if (status == PermissionStatus.Granted)
                    {
                        permissionsGranted = true;
                    }
                    else
                    {
                        permissionsGranted = false;
                        break;
                    }
                }
            }
            catch { }

            if (permissionsGranted == true)
                return true;
            else
                return false;
        }

        private async Task GetUserSettings()
        {
            try
            {
                string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "instellingen.json");

                //Checks if the file exists, if it does than it just takes the settings from the json, otherwise it will make a new one and puts the default settings in it
                if (!File.Exists(FilePath))
                {
                    try
                    {
                        PermissionStatus fileStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
                        if (fileStatus == PermissionStatus.Granted)
                        {
                            Instellingen newInst = new Instellingen
                            {
                                DateFormat = "DD/MM/JJJJ",
                                LengthFormat = "Centimeter"
                            };

                            string newJson = JsonConvert.SerializeObject(newInst);

                            using (StreamWriter sw = new StreamWriter(File.Create(FilePath)))
                            {
                                sw.WriteLine(newJson);
                                sw.Close();
                                sw.Dispose();
                            }
                            UserSettings = newInst;
                        }
                    }
                    catch { await DisplayAlert("Fout!", "Fout met ophalen/schrijven van de user info, check de app's toestemmingen in uw mobiel's instellingen of deze aan staan, anders kan de app niet goed werken", "Oke"); }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(FilePath))
                    {
                        string jsonData = sr.ReadToEnd();
                        UserSettings = JsonConvert.DeserializeObject<Instellingen>(jsonData);
                        sr.Close();
                    }
                }
            } catch
            {
                await DisplayAlert("Fout!", "Fout met ophalen/schrijven van de user info, check de app's toestemmingen in uw mobiel's instellingen of deze aan staan, anders kan de app niet goed werken", "Oke");
            } 
        }
    }
}      