using Fusee.Engine.Shader;

public class MyShader : Shader
{
    [Renderstates]
    [Fallbacks]
    [Reuse existing code]
    [TextureCombiner]
    [DeferredShading/GShading]

    [Uniform]

    
    [Attribute(Vertex)] public float4 vertex;
    [Attribute(Normal)] public float4 normal;
    [Attribute(Custom)] public float4 someAttrib;
    
    [Varying]



    [Pass(1), Transform]
    public void Transform()
    {

    }

    [Pass(1), Surface]
    public void Surface(ref SurfaceDescription sd)
    {
        
    }

    [Pass(1), Lighting]
    public Color Lighting(SurfaceDescription sd)
    {

    }

}



