using System;
using System.Collections.Generic;
using System.Reflection;
#if !ANDROID
using JSIL.Meta;
#endif

namespace Fusee.Engine
{
    /// <summary>
    /// The implementation factory. Creates all the implementation specific objects and returns
    /// their implementation agnostic interfaces. 
    /// TODO: replace this with something more Dependency Injection Container like
    /// </summary>
    public static class ImpFactory
    {
#if !ANDROID
        [JSIgnore] 
#endif
        private static Type _renderingImplementor;

#if !ANDROID
        [JSIgnore]
#endif
        private static Type _audioImplementor;

#if !ANDROID
        [JSIgnore]
#endif
        private static Type RenderingImplementor
        {
            get
            {
                if (_renderingImplementor == null)
                {
#if ANDROID
                   _renderingImplementor = typeof(Fusee.Engine.RenderingImplementor);
#else
                    // TODO: Remove this hardcoded hack to OpenTK
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.OpenTK.dll");
                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.OpenTK.dll).");

                    _renderingImplementor = impAsm.GetType("Fusee.Engine.RenderingImplementor");
#endif
                }
                return _renderingImplementor;
            }
        }

#if !ANDROID
        [JSIgnore]
#endif
        private static Type AudioImplementor
        {
            get
            {
                if (_audioImplementor == null)
                {
#if ANDROID
                    _audioImplementor = typeof (Fusee.Engine.AudioImplementor);
#else
                    // TODO: Remove this hardcoded hack to NAudio
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.NAudio.dll");

                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.NAudio.dll).");

                    _audioImplementor = impAsm.GetType("Fusee.Engine.AudioImplementor");
#endif
                }
                return _audioImplementor;
            }
        }
#if ANDROID
        // On Android we'll have only one implementation anyway (probably...)
        public static IRenderCanvasImp CreateIRenderCanvasImp(Dictionary<string, object> globals)
        {
            return Fusee.Engine.RenderingImplementor.CreateRenderCanvasImp(globals);
        }


        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            return Fusee.Engine.RenderingImplementor.CreateRenderContextImp(renderCanvas);
        }


        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            return Fusee.Engine.RenderingImplementor.CreateInputImp(renderCanvas);
        }
		
        public static IAudioImp CreateIAudioImp()
        {
			return Fusee.Engine.AudioImplementor.CreateAudioImp();
 		}
#else
        [JSExternal]
        public static IRenderCanvasImp CreateIRenderCanvasImp(Dictionary<string, object> globals)
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateRenderCanvasImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateRenderCanvasImp");

            return (IRenderCanvasImp) mi.Invoke(null, new object[]{globals});
        }


        [JSExternal]
        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateRenderContextImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateRenderContextImp");

            return (IRenderContextImp)mi.Invoke(null, new object[] { renderCanvas });
        }


        [JSExternal]
        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateInputImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateInputImp");

            return (IInputImp)mi.Invoke(null, new object[] { renderCanvas });
        }
		

        [JSExternal]
        public static IAudioImp CreateIAudioImp()
        {
            MethodInfo mi = AudioImplementor.GetMethod("CreateAudioImp");

            if (mi == null)
                throw new Exception("Implementor type (" + AudioImplementor.ToString() + ") doesn't contain method CreateAudioImp");

            return (IAudioImp)mi.Invoke(null, null);
        }		
#endif


    }
}
