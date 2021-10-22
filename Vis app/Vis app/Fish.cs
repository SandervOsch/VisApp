using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Vis_app
{
    //this is used for the json file, with the different types of variables so i can call for example the Day of CatchDate without messing with a string
    public class Fish
    {
        public string FishName { get; set; }
        public DateTime CatchDate { get; set; }
        public decimal FishLengthCm { get; set; }
        public decimal FishLengthInch { get; set; }
        public string FishImage { get; set; }
    }

    //This is used for the listview by formatting the entries first from the Fish class and then putting it in the listview
    public class LocalFish
    {
        public string FishName { get; set; }
        public string CatchDate { get; set; }
        public string FishLength { get; set; }
        public string FishImage { get; set; }
        public Color ListViewColor { get; set; }
    }

    //Settings for different formats for the date and length
    public class Instellingen
    {
        public string LengthFormat { get; set; }
        public string DateFormat { get; set; }
    }
}
