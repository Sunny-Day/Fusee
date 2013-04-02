
namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        // This is just a stub implementation right now
        // TODO: implement this!
        
        private AndroidAudioImp _audio;

        internal bool IsStream { get; set; }

        public float Volume
        {
            get { return 0.42f; }
            set {  }
        }

        public float Panning
        {
            get { return 0.5f; }
            set { }
        }

        public bool Loop
        {
            get { return false; }
            set
            {
                
            }
        }

        public AudioStream(string fileName, AndroidAudioImp audioCl)
        {

        }

        public AudioStream(string fileName, bool streaming, AndroidAudioImp audioCl)
        {
 
        }


        public void Dispose()
        {
 
        }

        public void Play()
        {
         }

        public void Play(bool loop)
        {

        }

        public void Pause()
        {

        }

        public void Stop()
        {

        }
    }
}