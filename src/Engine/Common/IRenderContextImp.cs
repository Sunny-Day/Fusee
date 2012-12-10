using Fusee.Math;
#if !ANDROID
using JSIL.Meta;
#endif

namespace Fusee.Engine
{
    public interface IRenderContextImp
    {
        float4x4 ModelView { set; get; }
        
        float4x4 Projection { set; get; }

        float4 ClearColor { set; get; }

        float ClearDepth { set; get; }
        
        IShaderProgramImp CreateShader(string vs, string ps);
        
        IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName);

#if !ANDROID
        [JSChangeName("SetShaderParam1f")]
#endif
        void SetShaderParam(IShaderParam param, float val);

#if !ANDROID
        [JSChangeName("SetShaderParam2f")]
#endif
        void SetShaderParam(IShaderParam param, float2 val);

#if !ANDROID
        [JSChangeName("SetShaderParam3f")]
#endif
        void SetShaderParam(IShaderParam param, float3 val);

#if !ANDROID
        [JSChangeName("SetShaderParam4f")]
#endif
        void SetShaderParam(IShaderParam param, float4 val);

#if !ANDROID
        [JSChangeName("SetShaderParamMtx4f")]
#endif
        void SetShaderParam(IShaderParam param, float4x4 val);


        void SetShaderParam(IShaderParam param, int val);

        

        void Clear(ClearFlags flags);

        void SetVertices(IMeshImp mesh, float3[] vertices);

        void SetNormals(IMeshImp mr, float3[] normals);


        void SetColors(IMeshImp mr, uint[] colors);

        void SetTriangles(IMeshImp mr, short[] triangleIndices);
        
        void SetShader(IShaderProgramImp shaderProgramImp);

        void Viewport(int x, int y, int width, int height);

        void Render(IMeshImp mr);

        IMeshImp CreateMeshImp();
    }
}
