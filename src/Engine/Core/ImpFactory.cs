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
        private static Type _implementor;
        
#if !ANDROID
        [JSIgnore]
#endif
        private static Type Implementor
        {
            get
            {
                if (_implementor == null)
                {
                    // TODO: Remove this hardcoded hack to OpenTK
#if ANDROID
                    _implementor = typeof(Fusee.Engine.Implementor);
#else
                    Assembly impAsm = Assembly.LoadFrom("Fusee.Engine.Imp.OpenTK.dll");
                    if (impAsm == null)
                        throw new Exception("Couldn't load implementor assembly (Fusee.Engine.Imp.OpenTK.dll).");
                    _implementor = impAsm.GetType("Fusee.Engine.Implementor");
#endif
                }
                return _implementor;
            }
        }

#if ANDROID
        // On Android we'll have only one implementation anyway (probably...)
        public static IRenderCanvasImp CreateIRenderCanvasImp(Dictionary<string, object> globals)
        {
            return Fusee.Engine.Implementor.CreateRenderCanvasImp(globals);
        }


        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            return Fusee.Engine.Implementor.CreateRenderContextImp(renderCanvas);
        }


        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            return Fusee.Engine.Implementor.CreateInputImp(renderCanvas);
        }
#else
        [JSExternal]
        public static IRenderCanvasImp CreateIRenderCanvasImp(Dictionary<string, object> globals)
        {
            MethodInfo mi = Implementor.GetMethod("CreateRenderCanvasImp");
            if (mi == null)
                throw new Exception("Implementor type (" + Implementor.ToString() + ") doesn't contain method CreateRenderCanvasImp");

            return (IRenderCanvasImp) mi.Invoke(null, new object[]{globals});
        }


        [JSExternal]
        public static IRenderContextImp CreateIRenderContextImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = Implementor.GetMethod("CreateRenderContextImp");
            if (mi == null)
                throw new Exception("Implementor type (" + Implementor.ToString() + ") doesn't contain method CreateRenderContextImp");

            return (IRenderContextImp)mi.Invoke(null, new object[] { renderCanvas });
        }


        [JSExternal]
        public static IInputImp CreateIInputImp(IRenderCanvasImp renderCanvas)
        {
            MethodInfo mi = Implementor.GetMethod("CreateInputImp");
            if (mi == null)
                throw new Exception("Implementor type (" + Implementor.ToString() + ") doesn't contain method CreateInputImp");

            return (IInputImp)mi.Invoke(null, new object[] { renderCanvas });
        }
#endif


    }
}
