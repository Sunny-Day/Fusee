#if !ANDROID
using JSIL.Meta;
#endif
using System.Diagnostics;

namespace Fusee.Engine
{
    /***
     * Contains diagnostic and debug helper methods.
     *
     */

    public static class Diagnostics
    {
#if !ANDROID
        [JSIgnore]
#endif
        private static Stopwatch _daWatch;

        
        /// Returns high precision timer values in milliseconds.
#if !ANDROID
        [JSExternal]
#endif
        public static double Timer
        {
            get
            {
                if (_daWatch == null)
                {
                    _daWatch = new Stopwatch();
                    _daWatch.Start();
                }
                return (1000.0 * ((double)_daWatch.ElapsedTicks)) / ((double) Stopwatch.Frequency);
            }
        }

        /// <summary>
        /// Log a debug output message to the respective output console.
        /// </summary>
        /// <param name="o">The object to log. Will be converted to a string.</param>
#if !ANDROID
        [JSExternal]
#endif
        public static void Log(object o)
        {
            Debug.Print(o.ToString());            
        }
    }
}
