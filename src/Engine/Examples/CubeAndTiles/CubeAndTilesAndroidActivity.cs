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
        CubeAndTiles _view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var globals = new Dictionary<string, object>();
            globals.Add("Context", this);

            // Create our OpenGL _view, and display it
            _view = new CubeAndTiles(globals);
            _view.Run();
            // SetContentView(_view);
        }

#if OUYA
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            return _view.OnKeyDown(keyCode,  e);
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            return _view.OnKeyUp(keyCode, e);
        }

        public override bool OnGenericMotionEvent(MotionEvent e) 
        {
            if ((e.Source & Android.Views.InputDevice.SourceClassJoystick) == 0)
            {
                //Not a joystick movement, so ignore it.
                return false;
            }
            return _view.OnGenericMotionEvent(e);
        }
#endif



        protected override void OnPause()
        {
            base.OnPause();
            _view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            _view.Resume();
        }
    }
}