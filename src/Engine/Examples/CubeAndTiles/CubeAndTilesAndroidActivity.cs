using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Examples.CubeAndTiles
{
    [Activity(Label = "Cube & Tiles", MainLauncher = true, Icon = "@drawable/icon")]
    public class CubeAndTilesAndroidActivity : Activity
    {
        CubeAndTiles view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var globals = new Dictionary<string, object>();
            globals.Add("Context", this);

            // Create our OpenGL view, and display it
            view = new CubeAndTiles(globals);
            view.Run();
            // SetContentView(view);
        }

        protected override void OnPause()
        {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            view.Resume();
        }
    }
}