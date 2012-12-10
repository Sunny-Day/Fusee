using System.Collections.Generic;

namespace Fusee.Engine
{
    // This class is instantiated dynamically (by reflection)
    public class Implementor
    {
        public static IRenderCanvasImp CreateRenderCanvasImp(Dictionary<string, object> globals)
        {
            return new RenderCanvasImp(globals);
        }

        public static IRenderContextImp CreateRenderContextImp(IRenderCanvasImp rci)
        {
            return new RenderContextImp(rci);
        }

        public static IInputImp CreateInputImp(IRenderCanvasImp rci)
        {
            return new InputImp(rci);
        }

    }
}
