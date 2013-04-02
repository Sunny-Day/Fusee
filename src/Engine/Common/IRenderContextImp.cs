#pragma warning disable 1591 //disables the warning about missing XML-comments

using System;
using System.Collections.Generic;
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

        IEnumerable<ShaderParamInfo> GetShaderParamList(IShaderProgramImp shaderProgram);

        IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName);

        float GetParamValue(IShaderProgramImp shaderProgram, IShaderParam param);

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

        [JSChangeName("SetShaderParamInt")]
        void SetShaderParam(IShaderParam param, int val);


        ITexture CreateTexture(ImageData imageData);

        ImageData LoadImage(String filename);

        ImageData CreateImage(int width, int height, String bgColor);

        ImageData TextOnImage(ImageData imgData, String fontName, float fontSize, String text, String textColor,
                              float startPosX, float startPosY);

        void SetShaderParamTexture(IShaderParam param, ITexture texId);

        void Clear(ClearFlags flags);

        void SetVertices(IMeshImp mesh, float3[] vertices);

        void SetNormals(IMeshImp mr, float3[] normals);

        void SetUVs(IMeshImp mr, float2[] uvs);

        void SetColors(IMeshImp mr, uint[] colors);

        void SetTriangles(IMeshImp mr, short[] triangleIndices);

        void SetShader(IShaderProgramImp shaderProgramImp);

        void Viewport(int x, int y, int width, int height);

        void ColorMask(bool red, bool green, bool blue, bool alpha);

        void Frustum(double left, double right, double bottom, double top, double zNear, double zFar);

        void Render(IMeshImp mr);

        IMeshImp CreateMeshImp();
    }
}

#pragma warning restore 1591
