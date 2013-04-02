#define MP3Warning

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fusee.Engine
{
    public class AndroidAudioImp : IAudioImp
    {
        // This is just a stub implementation right now
        // TODO: implement this!
        
        public AndroidAudioImp()
        {
        }

        public void OpenDevice()
        {
        }

        public void CloseDevice()
        {
        }

        public IAudioStream LoadFile(string fileName, bool streaming)
        {
            return new AudioStream(fileName, streaming, this);
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
