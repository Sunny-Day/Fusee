
using System;
using Android.Content.Res;
using Android.Media;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        // This is just a stub implementation right now
        // TODO: implement this!
        
        private AndroidAudioImp _audio;
        MediaPlayer _player = null;

        private static float MAX_VOLUME = 100f;

        private float _volume;
        private float _panning;

        internal bool IsStream { get; set; }

        public float Volume
        {
            get { return _volume; }
            set 
            {
                _volume = Math.Min(Math.Max(0, value), MAX_VOLUME);
                SetVolumeInternal();
            }
        }

        public float Panning
        {
            get { return _panning; }
            set
            {
                _panning = Math.Min(Math.Max(-MAX_VOLUME, value), MAX_VOLUME);
                SetVolumeInternal();
            }
        }

        private void SetVolumeInternal()
        {
            // See http://stackoverflow.com/questions/5215459/android-mediaplayer-setvolume-function
            float leftVol = (float)(1 - (Math.Log(MAX_VOLUME - (_volume * (MAX_VOLUME - _panning) / 2f)) / Math.Log(MAX_VOLUME)));
            float rightVol = (float)(1 - (Math.Log(MAX_VOLUME - (_volume * (MAX_VOLUME + _panning) / 2f)) / Math.Log(MAX_VOLUME)));
            _player.SetVolume(leftVol, rightVol);

            _player.SetVolume(leftVol, rightVol);
        }


        public bool Loop
        {
            get { return _player.Looping; }
            set { _player.Looping = value; }
        }

        public AudioStream(AssetFileDescriptor afd, AndroidAudioImp audioCl)
        {
            _player = new MediaPlayer();
              
            // This method works better than setting the file path in SetDataSource. Don't know why.
                
            // Java.IO.File file = new Java.IO.File (filePath);
            // Java.IO.FileInputStream fis = new Java.IO.FileInputStream (file);
            // player.SetDataSource(fis.FD);
            _player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            _player.Prepare();
            _panning = 0f;
            _volume = MAX_VOLUME;
        }

        public void Dispose()
        {
 
        }

        public void Play()
        {
            _player.Start();
        }

        public void Play(bool loop)
        {
            _player.Looping = loop;
            _player.Start();
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
        }
    }
}