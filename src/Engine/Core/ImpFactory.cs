﻿using System;
using System.Reflection;
using JSIL.Meta;

namespace Fusee.Engine
{
    /// <summary>
    /// The implementation factory. Creates all the implementation specific objects and returns
    /// their implementation agnostic interfaces. 
    /// TODO: replace this with something more Dependency Injection Container like
    /// </summary>
    public static class ImpFactory
    {
        [JSIgnore] 
        private static Type _renderingImplementor;

        [JSIgnore]
        private static Type _audioImplementor;

        [JSIgnore]
        private static Type _networkImplementor;

        [JSIgnore]
        private static Type RenderingImplementor
        {
            get
            {
                if (_renderingImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to OpenTK

                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.OpenTK.dll");
                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.OpenTK.dll).");

                    _renderingImplementor = impAsm.GetType("Fusee.Engine.RenderingImplementor");
                }
                return _renderingImplementor;
            }
        }

        [JSIgnore]
        private static Type AudioImplementor
        {
            get
            {
                if (_audioImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to SFMLAudio
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.SFMLAudio.dll");

                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.SFMLAudio.dll).");

                    _audioImplementor = impAsm.GetType("Fusee.Engine.AudioImplementor");
                }
                return _audioImplementor;
            }
        }

        [JSIgnore]
        private static Type NetworkImplementor
        {
            get
            {
                if (_networkImplementor == null)
                {
                    // TODO: Remove this hardcoded hack to Lidgren
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.Lidgren.dll");

                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.Lidgren.dll).");

                    _networkImplementor = impAsm.GetType("Fusee.Engine.NetworkImplementor");
                }
                return _networkImplementor;
            }
        }



        [JSExternal]
        public static IRenderCanvasImp CreateIRenderCanvasImp()
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateRenderCanvasImp");
            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateRenderCanvasImp");

            return (IRenderCanvasImp)mi.Invoke(null, null);
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

        [JSExternal]
        public static INetworkImp CreateINetworkImp()
        {
            MethodInfo mi = NetworkImplementor.GetMethod("CreateNetworkImp");

            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateNetworkImp");

            return (INetworkImp)mi.Invoke(null, null);
        }

        [JSExternal]
        public static IImagehelperImp CreateIImagehelperImp()
        {
            MethodInfo mi = RenderingImplementor.GetMethod("CreateImagehelperImp");

            if (mi == null)
                throw new Exception("Implementor type (" + RenderingImplementor.ToString() + ") doesn't contain method CreateImagehelperImp");

            return (IImagehelperImp)mi.Invoke(null, null);
        }
    }
}
