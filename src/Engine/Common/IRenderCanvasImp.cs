using System;

namespace Fusee.Engine
{
    public interface IRenderCanvasImp
    {
        int Width { get ; }
        int Height { get; }

        double DeltaTime { get; }

        void Present();
        void Run();
        void Pause();
        void Resume();

        event EventHandler<InitEventArgs> Init;
        event EventHandler<RenderEventArgs> Render;
        event EventHandler<ResizeEventArgs> Resize;
    }
}
