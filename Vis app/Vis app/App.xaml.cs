using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.IO;
using Vis_app.Services;

namespace Vis_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new NavigationPage(new Homepage());
        }

        protected override void OnStart()
        {          
            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "vis.json"));

            if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "vissen.json")))
            {
                File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "vissen.json")).Close();
            }
        }
               
    }
}
