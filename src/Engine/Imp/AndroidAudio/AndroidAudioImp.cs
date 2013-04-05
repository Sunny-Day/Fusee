#define MP3Warning

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Android.Content;
using Android.Content.Res;

namespace Fusee.Engine
{
    public class AndroidAudioImp : IAudioImp
    {
        // This is just a stub implementation right now
        // TODO: implement this!
        private Context _androidContext;

        public AndroidAudioImp(Dictionary<string, object> globals)
        {
            _androidContext =  (Context) globals["Context"];
        }

        public void OpenDevice()
        {
        }

        public void CloseDevice()
        {
        }

        public IAudioStream LoadFile(string fileName, bool streaming)
        {
            string assetName = fileName.Replace("Assets/", "");
            assetName = assetName.Replace("assets/", "");
            AssetFileDescriptor afd  = _androidContext.Assets.OpenFd(assetName);

            return new AudioStream(afd, this);
        }

        public void Stop()
        {
        }

        public void SetVolume(float val)
        {
        }

        public float GetVolume()
        {
            return 0.42f;
        }

        public void SetPanning(float val)
        {
        }
    }
}
