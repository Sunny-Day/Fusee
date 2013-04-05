using System.Collections.Generic;

namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class AudioImplementor
    {
        public static IAudioImp CreateAudioImp(Dictionary<string, object> globals)
        {
            return new AndroidAudioImp(globals);
        }
    }
}