using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Vis_app
{
    public class ListPage : ContentPage
    {
        List<Fish> FishItems = new List<Fish>();
        Instellingen Settings = new Instellingen();

        //This is basicly a list but its a lil special that it updates the listview immediately after it has new stuff in it
        ObservableCollection<LocalFish> FilteredFishList = new ObservableCollection<LocalFish>();

        //hash set is basicly a dictionary, which has a key and values, where the key is always unique, but the hashset has only keys so it only adds unique values
        HashSet<string> StringDateList = new HashSet<string>();
        HashSet<DateTime> DateDateList = new HashSet<DateTime>();

        Picker DatePick = new Picker();
        Label LeftArrow = new Label();
        Label RightArrow = new Label();
        Label TotalLengthLabel = new Label();
        Label TotalEntriesLabel = new Label();

        int DateSelectedIndex = 0;
        string TotalLength = "";
        int TotalEntries = 0;
        public ListPage(List<Fish> fishItems, Instellingen UserSettings)
        {
            BackgroundColor = Color.FromHex("#e8f0ff");
            FishItems = fishItems;
            Settings = UserSettings;

            foreach(Fish date in fishItems)
            {              
                DateDateList.Add(date.CatchDate.Date);
            }

            //StringDateList is the date as formatted by the user settings and DateDateList is the normal DateTime            
            DateDateList.Add(DateTime.Now);

            //sort the list so the dates are in the right order
            DateDateList = DateDateList.OrderBy(x => x.Date).ToHashSet();

            foreach(DateTime date in DateDateList)
            {
                StringDateList.Add(FormatDate(date.Date, UserSettings));
            }
            StringDateList.Add(FormatDate(DateTime.Now, UserSettings));


            //gets the list to display in the listview by date, this one is for the day the user opens the listview
            GetFilteredList(fishItems, UserSettings, DateTime.Now);
          
            Label header = new Label
            {
                Text = "Vis Lijst",
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
            };

            LeftArrow = new Label
            {
                Text = "<",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Margin = new Thickness(15, 0),
                WidthRequest = 50
            };

            RightArrow = new Label
            {
                Text = ">",
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Black,
                Margin = new Thickness(15, 0),       
                WidthRequest = 50
            };

            //Labels cant have .clicked function so here it just recognizes if the user taps on the screen on the label, a nice workaround
            var LeftArrowTGR = new TapGestureRecognizer();
            LeftArrowTGR.Tapped += LeftArrow_Tapped;
            LeftArrow.GestureRecognizers.Add(LeftArrowTGR);

            var RightArrowTGR = new TapGestureRecognizer();
            RightArrowTGR.Tapped += RightArrow_Tapped;
            RightArrow.GestureRecognizers.Add(RightArrowTGR);

            //this is so the arrows and the datepicker are correct even if the user changes their date in the phone
            foreach (DateTime d in DateDateList)
            {
                if (d.Date == DateTime.Now.Date)
                    break;
                else
                    DateSelectedIndex++;
            }

            //set the colors for the labels if there are dates in the picker
            if (DateSelectedIndex >= StringDateList.Count - 1)
                RightArrow.TextColor = Color.Gray;

            if (DateSelectedIndex <= 0)
                LeftArrow.TextColor = Color.Gray;

            DatePick = new Picker
            {
                ItemsSource = StringDateList.ToList(),
                SelectedIndex = DateSelectedIndex,
                HorizontalOptions = LayoutOptions.Center,

                
                Title = "Datum",

                WidthRequest = 225,
            };
            DatePick.SelectedIndexChanged += DatePick_SelectedIndexChanged;

            ListView FishListview = new ListView
            {
                // Source of data items.
                ItemsSource = FilteredFishList,
                HasUnevenRows = true,


                // Defines a template to display each item in the listview
                ItemTemplate = new DataTemplate(() =>
                {
                    Label FishNameLabel = new Label();
                    Label CatchDateLabel = new Label();
                    Label FishLengthLabel = new Label();
                    Image FishImage = new Image();

                    // Create views with bindings from the FilteredFishList and display for each property.      
                    FishNameLabel.SetBinding(Label.TextProperty, "FishName");
                    FishNameLabel.FontSize = 16;

                    CatchDateLabel.SetBinding(Label.TextProperty, "CatchDate");
                    CatchDateLabel.FontSize = 14;

                    FishLengthLabel.SetBinding(Label.TextProperty, "FishLength");
                    FishLengthLabel.FontSize = 14;

                    FishImage = new Image
                    {                     
                        HeightRequest = 120,
                        WidthRequest = 120,
                        Aspect = Aspect.AspectFit,
                        Margin = new Thickness(0, 5)
                    };
                    FishImage.SetBinding(Image.SourceProperty, "FishImage");

                    // Return an assembled ViewCell.
                    ViewCell cell = new ViewCell
                    {                      
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                FishImage,
                                new StackLayout
                                {                                   
                                    VerticalOptions = LayoutOptions.Center,
                                    Spacing = 0,
                                    Children =
                                    {
                                        FishNameLabel,
                                        CatchDateLabel,
                                        FishLengthLabel
                                    }
                                }
                            }
                        }
                    };
                    cell.View.SetBinding(BackgroundColorProperty, "ListViewColor");
                    return cell;
                })
            };
            FishListview.ItemTapped += Listview_ItemTapped;

            Label Total = new Label
            {
                Text = "Totaal",
                FontSize = 16
            };

            TotalLengthLabel = new Label
            {
                Text = "Totale Lengte: " + TotalLength,
                FontSize = 16
            };

            TotalEntriesLabel = new Label
            {
                Text = "Geldige vissen: " + TotalEntries,
                FontSize = 16
            };

            StackLayout DateNavigation = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                MinimumWidthRequest = 150,
                Margin = new Thickness(0, -5, 0, 0),
                Children =
                {
                    LeftArrow,
                    DatePick,
                    RightArrow
                }
            };

            StackLayout Footer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(5, 0, 0, 5),
                Children =
                {
                    Total,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Margin = new Thickness(80, 0, 0, 0),
                        Children =
                        {
                            TotalEntriesLabel,
                            TotalLengthLabel
                        }
                    }
                }
            };


            Content = new StackLayout
            {
                Children =
                {
                    header,
                    DateNavigation,
                    FishListview,
                    Footer
                }
            };
        }

        /// <summary>
        /// Formats the date given to the user's settings
        /// </summary>
        /// <param name="date"></param>
        /// <param name="DateSettings"></param>
        /// <returns></returns>
        public static string FormatDate(DateTime date, Instellingen DateSettings)
        {
            string newDateFormat = "";

            if (DateSettings.DateFormat == "MM/DD/JJJJ")
                newDateFormat = date.Month.ToString() + "/" + date.Day.ToString() + "/" + date.Year.ToString();

            else if (DateSettings.DateFormat == "DD/MM/JJJJ")
                newDateFormat = date.Day.ToString() + "/" + date.Month.ToString() + "/" + date.Year.ToString();

            else if (DateSettings.DateFormat == "JJJJ/MM/DD")
                newDateFormat = date.Year.ToString() + "/" + date.Month.ToString() + "/" + date.Day.ToString();

            else if (DateSettings.DateFormat == "Maand D, Jr")
                newDateFormat = date.GetDateTimeFormats('M')[0].ToString() + ", " + date.Year.ToString();

            return newDateFormat;
        }

        //"Baars", "Snoek", "Snoekbaars", "Bot", "Meerval", "Roofblei", "Steur"
        private bool MinimalLength(int lengthCM, decimal LengthInch, string FishName)
        {
            if (FishName == "Baars")
            {
                if (lengthCM >= 22 || LengthInch >= (decimal)8.7)
                    return true;
                else
                    return false;
            }
            else if (FishName == "Snoek")
            {
                if (lengthCM >= 45 || LengthInch >= (decimal)17.7)
                    return true;
                else
                    return false;
            }
            else if (FishName == "Snoekbaars")
            {
                if (lengthCM >= 42 || LengthInch >= (decimal)16.5)
                    return true;
                else
                    return false;
            }
            else if (FishName == "Bot")
            {
                if (lengthCM >= 20 || LengthInch >= (decimal)7.9)
                    return true;
                else
                    return false;
            }
            else if (FishName == "Meerval" || FishName == "Roofblei" || FishName == "Steur")
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Filters the listview in the ListPage.cs to display the date selected in the picker or by moving with the arrow labels
        /// </summary>
        /// <param name="fishItems"></param>
        /// <param name="userSettings"></param>
        /// <param name="date"></param>
        private void GetFilteredList(List<Fish> fishItems, Instellingen userSettings, DateTime date)
        {
            //clears the list to refresh the items
            FilteredFishList.Clear();

            decimal total = 0;
            TotalEntries = 0;

            foreach (Fish f in fishItems)
            {
                LocalFish local = new LocalFish();

                //This checks if the date given by the function and the catch date of the json list is equal, if it is it adds it to FilteredFishList so it can be displayed in the listview
                if (date.Date == f.CatchDate.Date)
                {
                    local.FishName = f.FishName;

                    local.CatchDate = FormatDate(f.CatchDate, userSettings);

                    if (userSettings.LengthFormat == "Centimeter")
                    {
                        local.FishLength = int.Parse(decimal.Round(f.FishLengthCm, 0).ToString()) + " cm";
                        if(MinimalLength(int.Parse(decimal.Round(f.FishLengthCm, 0).ToString()), 0, local.FishName))
                        {
                            total += f.FishLengthCm;
                            local.ListViewColor = Color.FromHex("#4032a852");
                            TotalEntries++;
                        } else
                        {
                            local.ListViewColor = Color.FromHex("#40a84c32");
                        }
                        
                    }

                    if (userSettings.LengthFormat == "Inches")
                    {
                        local.FishLength = decimal.Round(f.FishLengthInch, 1) + " inch";
                        if (MinimalLength(0, decimal.Round(f.FishLengthInch, 1), local.FishName))
                        {
                            total += f.FishLengthInch;
                            local.ListViewColor = Color.FromHex("#4032a852");
                            TotalEntries++;
                        }
                        else
                        {
                            local.ListViewColor = Color.FromHex("#40a84c32");
                        }
                        
                    }
                    local.FishImage = f.FishImage;
                   
                    FilteredFishList.Add(local);
                }
            }

            //this is for the footer to display the total length of all the entries on that current date
            if (userSettings.LengthFormat == "Centimeter")
            {
                TotalLength = int.Parse(decimal.Round(total, 0).ToString()) + " cm";
                TotalLengthLabel.Text = "Totale Lengte: " + TotalLength;
            }

            if (userSettings.LengthFormat == "Inches")
            { 
                TotalLength = decimal.Round(total, 1) + " inch";
                TotalLengthLabel.Text = "Totale Lengte: " + TotalLength;
            }
            TotalEntriesLabel.Text = "Totale vissen: " + TotalEntries;
        }

        private void DatePick_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateSelectedIndex = DatePick.SelectedIndex;
            FilteredFishList.Clear();

            GetFilteredList(FishItems, Settings, DateDateList.ToList()[DateSelectedIndex]);

            //checks the DateSelectedIndex compared to the rest of the list, to know if the user is at the start/end of the Date entry list, so it knows to change the arrow's color
            if (DateSelectedIndex >= StringDateList.Count - 1)
                RightArrow.TextColor = Color.Gray;
            else
                RightArrow.TextColor = Color.Black;

            if (DateSelectedIndex <= 0)
                LeftArrow.TextColor = Color.Gray;
            else
                LeftArrow.TextColor = Color.Black;
        }

        private void LeftArrow_Tapped(object sender, EventArgs e)
        {
            //this is to prevent the an out range exception, idk if it works tho lmao
            if(DateSelectedIndex <= 0)
            {
                DateSelectedIndex = 0;
            } else
            {
                DateSelectedIndex--;

                FilteredFishList.Clear();

                GetFilteredList(FishItems, Settings, DateDateList.ToList()[DateSelectedIndex]);

                DatePick.SelectedIndex = DateSelectedIndex;

                LeftArrow.TextColor = Color.Black;
                RightArrow.TextColor = Color.Black;

                //checks the DateSelectedIndex compared to the rest of the list, to know if the user is at the start/end of the Date entry list, so it knows to change the arrow's color
                if (DateSelectedIndex <= 0)
                    LeftArrow.TextColor = Color.Gray;
            }
        }

        private void RightArrow_Tapped(object sender, EventArgs e)
        {
            //this is to prevent the an out range exception, idk if it works tho lmao
            if (DateSelectedIndex >= StringDateList.Count - 1)
            {
                DateSelectedIndex = StringDateList.Count - 1;
            }
            else
            {
                DateSelectedIndex++;

                FilteredFishList.Clear();

                GetFilteredList(FishItems, Settings, DateDateList.ToList()[DateSelectedIndex]);

                DatePick.SelectedIndex = DateSelectedIndex;

                RightArrow.TextColor = Color.Black;
                LeftArrow.TextColor = Color.Black;

                //checks the DateSelectedIndex compared to the rest of the list, to know if the user is at the start/end of the Date entry list, so it knows to change the arrow's color
                if (DateSelectedIndex >= StringDateList.Count - 1)
                    RightArrow.TextColor = Color.Gray;
            }
        }

        private void Listview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            LocalFish fishTapped = (LocalFish)e.Item;

            Navigation.PushAsync(new FishInfo(fishTapped));
        }
    }
    //this is specificly for the Hash's, there is no function to convert a list to a hashset so this is just a function for that specific task
    public static class Extensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
    }
}
