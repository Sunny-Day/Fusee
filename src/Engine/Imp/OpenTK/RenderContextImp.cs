﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using Fusee.Math;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Fusee.Engine
{
    public class RenderContextImp : IRenderContextImp
    {
        private int _currentTextureUnit;
        private Dictionary<int, int> _shaderParam2TexUnit;

        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
            _currentTextureUnit = 0;
            _shaderParam2TexUnit = new Dictionary<int, int>();
        }

        /// <summary>
        /// Creates a new TextureRes and binds it to the shader.
        /// </summary>
        /// <param name="img">A given ImageData object, containing all necessary information for the upload to the graphics card.</param>
        /// <returns>An ITextureRes that can be used for texturing in the shader. In this implementation, the handle is an integer-value which is necessary for OpenTK.</returns>
        public ITextureRes CreateTexture(ImageData img)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, img.PixelData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            ITextureRes texID = new TextureRes { handle = id };
            return texID;
        }

        public IShaderParam GetShaderParam(IShaderResImp shaderRes, string paramName)
        {
            int h = GL.GetUniformLocation(((ShaderResImp)shaderRes).Program, paramName);
            return (h == -1) ? null : new ShaderParam { handle = h };
        }

        public float GetParamValue(IShaderResImp res, IShaderParam param)
        {
            float f;
            GL.GetUniform(((ShaderResImp)res).Program, ((ShaderParam)param).handle, out f);
            return f;
        }

        public IList<ShaderParamInfo> GetShaderParamList(IShaderResImp shaderRes)
        {
            var sp = (ShaderResImp)shaderRes;
            int nParams;
            GL.GetProgram(sp.Program, ProgramParameter.ActiveUniforms, out nParams);
            List<ShaderParamInfo> list = new List<ShaderParamInfo>();
            for (int i = 0; i < nParams; i++)
            {
                ActiveUniformType t;
                var ret = new ShaderParamInfo();
                ret.Name = GL.GetActiveUniform(sp.Program, i, out ret.Size, out t);
                ret.Handle = GetShaderParam(sp, ret.Name);
                switch (t)
                {
                    case ActiveUniformType.Int:
                        ret.Type = typeof(int);
                        break;
                    case ActiveUniformType.Float:
                        ret.Type = typeof(float);
                        break;
                    case ActiveUniformType.FloatVec2:
                        ret.Type = typeof(float2);
                        break;
                    case ActiveUniformType.FloatVec3:
                        ret.Type = typeof(float3);
                        break;
                    case ActiveUniformType.FloatVec4:
                        ret.Type = typeof(float4);
                        break;
                    case ActiveUniformType.FloatMat4:
                        ret.Type = typeof(float4x4);
                        break;
                    case ActiveUniformType.Sampler2D:
                        //TODO ret.Type = typeof (sampler?);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                list.Add(ret);
            }
            return list;
        }



        public void SetShaderParam(IShaderParam param, float val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }

        public void SetShaderParam(IShaderParam param, float2 val)
        {
            GL.Uniform2(((ShaderParam)param).handle, val.x, val.y);
        }

        public void SetShaderParam(IShaderParam param, float3 val)
        {
            GL.Uniform3(((ShaderParam)param).handle, val.x, val.y, val.z);
        }

        public void SetShaderParam(IShaderParam param, float4 val)
        {
            GL.Uniform4(((ShaderParam)param).handle, val.x, val.y, val.z, val.w);
        }

        // TODO add vector implementations

        public void SetShaderParam(IShaderParam param, float4x4 val)
        {
            unsafe
            {
                float* mF = (float*)(&val);
                GL.UniformMatrix4(((ShaderParam)param).handle, 1, false, mF);
            }
        }

        public void SetShaderParam(IShaderParam param, int val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }




        /// <summary>
        /// Sets a given Shader Parameter to a created texture
        /// </summary>
        /// <param name="param">Shader Parameter used for texture binding</param>
        /// <param name="texId">An ITextureRes probably returned from CreateTexture method</param>
        public void SetShaderParamTexture(IShaderParam param, ITextureRes texId)
        {
            int iParam = ((ShaderParam)param).handle;
            int texUnit;
            if (!_shaderParam2TexUnit.TryGetValue(iParam, out texUnit))
            {
                texUnit = _currentTextureUnit++;
                _shaderParam2TexUnit[iParam] = texUnit;
            }
            GL.Uniform1(iParam, texUnit);
            GL.ActiveTexture((TextureUnit)(TextureUnit.Texture0 + texUnit));
            GL.BindTexture(TextureTarget.Texture2D, ((TextureRes)texId).handle);
        }

        public float4x4 ModelView
        {
            get
            { throw new NotImplementedException(); }
            set
            {
                GL.MatrixMode(MatrixMode.Modelview);
                unsafe { GL.LoadMatrix((float*)(&value)); }
            }
        }

        public float4x4 Projection
        {
            get
            { throw new NotImplementedException(); }
            set
            {
                GL.MatrixMode(MatrixMode.Projection);
                unsafe { GL.LoadMatrix((float*)(&value)); }
            }
        }

        public float4 ClearColor
        {
            get
            {
                Vector4 ret;
                GL.GetFloat(GetPName.ColorClearValue, out ret);
                return new float4(ret.X, ret.Y, ret.Z, ret.W);
            }
            set
            {
                GL.ClearColor(value.x, value.y, value.z, value.w);
            }
        }

        public float ClearDepth
        {
            get
            {
                float ret;
                GL.GetFloat(GetPName.DepthClearValue, out ret);
                return ret;
            }
            set
            {
                GL.ClearDepth(value);
            }
        }

        public IShaderResImp CreateShader(string vs, string ps)
        {
            int statusCode;
            string info;

            int vertexObject = GL.CreateShader(ShaderType.VertexShader);
            int fragmentObject = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexObject, vs);
            GL.CompileShader(vertexObject);
            GL.GetShaderInfoLog(vertexObject, out info);
            GL.GetShader(vertexObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            // Compile pixel shader
            GL.ShaderSource(fragmentObject, ps);
            GL.CompileShader(fragmentObject);
            GL.GetShaderInfoLog(fragmentObject, out info);
            GL.GetShader(fragmentObject, ShaderParameter.CompileStatus, out statusCode);

            if (statusCode != 1)
                throw new ApplicationException(info);

            int program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, Helper.VertexAttribLocation, Helper.VertexAttribName);
            GL.BindAttribLocation(program, Helper.ColorAttribLocation, Helper.ColorAttribName);
            GL.BindAttribLocation(program, Helper.UvAttribLocation, Helper.UvAttribName);
            GL.BindAttribLocation(program, Helper.NormalAttribLocation, Helper.NormalAttribName);

            GL.LinkProgram(program); // AAAARRRRRGGGGHHHH!!!! Must be called AFTER BindAttribLocation
            return new ShaderResImp { Program = program };
        }


        public void SetShader(IShaderResImp res)
        {
            _currentTextureUnit = 0;
            _shaderParam2TexUnit.Clear();

            GL.UseProgram(((ShaderResImp)res).Program);
        }

        public void Clear(ClearFlags flags)
        {
            GL.Clear((ClearBufferMask)flags);
        }


        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
            if (((MeshImp)mr).VertexBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).VertexBufferObject);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertsBytes), vertices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int vboBytes;
            int normsBytes = normals.Length * 3 * sizeof(float);
            if (((MeshImp)mr).NormalBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).NormalBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normsBytes), normals, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetUVs(IMeshImp mr, float2[] uvs)
        {
            if (uvs == null || uvs.Length == 0)
            {
                throw new ArgumentException("UVs must not be null or empty");
            }

            int vboBytes;
            int uvsBytes = uvs.Length * 2 * sizeof(float);
            if (((MeshImp)mr).UVBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).UVBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvsBytes), uvs, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != uvsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading uv buffer to VBO (uvs). Tried to upload {0} bytes, uploaded {1}.",
                    uvsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetColors(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int vboBytes;
            int colsBytes = colors.Length * sizeof(uint);
            if (((MeshImp)mr).ColorBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).ColorBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(colsBytes), colors, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        public void SetTriangles(IMeshImp mr, short[] triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp)mr).NElements = triangleIndices.Length;
            int vboBytes;
            int trisBytes = triangleIndices.Length * sizeof(short);

            if (((MeshImp)mr).ElementBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).ElementBufferObject);
            // Upload the index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(trisBytes), triangleIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Render(IMeshImp mr)
        {
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.VertexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
            }

            if (((MeshImp)mr).UVBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.UvAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).UVBufferObject);
                GL.VertexAttribPointer(Helper.UvAttribLocation, 2, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }

            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.NormalAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer(Helper.NormalAttribLocation, 3, VertexAttribPointerType.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
                GL.DrawElements(BeginMode.Triangles, ((MeshImp)mr).NElements, DrawElementsType.UnsignedShort, IntPtr.Zero);
                //GL.DrawArrays(GL.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.VertexAttribLocation);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.ColorAttribLocation);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.NormalAttribLocation);
            }
            if (((MeshImp)mr).UVBufferObject != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.UvAttribLocation);
            }
        }

        public void DebugLine(float3 start, float3 end, float4 color)
        {
            GL.Begin(BeginMode.Lines);
            GL.Vertex3(start.x, start.y, start.z);
            GL.Vertex3(end.x, end.y, end.z);
            GL.End();
        }

        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        internal static BlendEquationMode BlendOperationToOgl(BlendOperation bo)
        {
            switch (bo)
            {
                case BlendOperation.Add:
                    return BlendEquationMode.FuncAdd;
                case BlendOperation.Subtract:
                    return BlendEquationMode.FuncSubtract;
                case BlendOperation.ReverseSubtract:
                    return BlendEquationMode.FuncReverseSubtract;
                case BlendOperation.Minimum:
                    return BlendEquationMode.Min;
                case BlendOperation.Maximum:
                    return BlendEquationMode.Max;
                default:
                    throw new ArgumentOutOfRangeException("bo");
            }
        }

        internal static BlendOperation BlendOperationFromOgl(BlendEquationMode bom)
        {
            switch (bom)
            {
                case BlendEquationMode.FuncAdd:
                    return BlendOperation.Add;
                case BlendEquationMode.Min:
                    return BlendOperation.Minimum;
                case BlendEquationMode.Max:
                    return BlendOperation.Maximum;
                case BlendEquationMode.FuncSubtract:
                    return BlendOperation.Subtract;
                case BlendEquationMode.FuncReverseSubtract:
                    return BlendOperation.ReverseSubtract;
                default:
                    throw new ArgumentOutOfRangeException("bom");
            }
        }

        internal static int BlendToOgl(Blend blend, bool isForAlpha = false)
        {
            switch (blend)
            {
                case Blend.Zero:
                    return (int)BlendingFactorSrc.Zero;
                case Blend.One:
                    return (int)BlendingFactorSrc.One;
                case Blend.SourceColor:
                    return (int)BlendingFactorDest.SrcColor;
                case Blend.InverseSourceColor:
                    return (int)BlendingFactorDest.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return (int)BlendingFactorSrc.SrcAlpha;
                case Blend.InverseSourceAlpha:
                    return (int)BlendingFactorSrc.OneMinusSrcAlpha;
                case Blend.DestinationAlpha:
                    return (int)BlendingFactorSrc.DstAlpha;
                case Blend.InverseDestinationAlpha:
                    return (int)BlendingFactorSrc.OneMinusDstAlpha;
                case Blend.DestinationColor:
                    return (int)BlendingFactorSrc.DstColor;
                case Blend.InverseDestinationColor:
                    return (int)BlendingFactorSrc.OneMinusDstColor;
                case Blend.BlendFactor:
                    return (int)((isForAlpha) ? BlendingFactorSrc.ConstantAlpha : BlendingFactorSrc.ConstantColor);
                case Blend.InverseBlendFactor:
                    return (int)((isForAlpha) ? BlendingFactorSrc.OneMinusConstantAlpha : BlendingFactorSrc.OneMinusConstantColor);
                // Ignored...
                // case Blend.SourceAlphaSaturated:
                //     break;
                //case Blend.Bothsrcalpha:
                //    break;
                //case Blend.BothInverseSourceAlpha:
                //    break;
                //case Blend.SourceColor2:
                //    break;
                //case Blend.InverseSourceColor2:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException("blend");
            }
        }

        internal static Blend BlendFromOgl(int bf)
        {
            switch (bf)
            {
                case (int)BlendingFactorSrc.Zero:
                    return Blend.Zero;
                case (int)BlendingFactorSrc.One:
                    return Blend.One;
                case (int)BlendingFactorDest.SrcColor:
                    return Blend.SourceColor;
                case (int)BlendingFactorDest.OneMinusSrcColor:
                    return Blend.InverseSourceColor;
                case (int)BlendingFactorSrc.SrcAlpha:
                    return Blend.SourceAlpha;
                case (int)BlendingFactorSrc.OneMinusSrcAlpha:
                    return Blend.InverseSourceAlpha;
                case (int)BlendingFactorSrc.DstAlpha:
                    return Blend.DestinationAlpha;
                case (int)BlendingFactorSrc.OneMinusDstAlpha:
                    return Blend.InverseDestinationAlpha;
                case (int)BlendingFactorSrc.DstColor:
                    return Blend.DestinationColor;
                case (int)BlendingFactorSrc.OneMinusDstColor:
                    return Blend.InverseDestinationColor;
                case (int)BlendingFactorSrc.ConstantAlpha:
                case (int)BlendingFactorSrc.ConstantColor:
                    return Blend.BlendFactor;
                case (int)BlendingFactorSrc.OneMinusConstantAlpha:
                case (int)BlendingFactorSrc.OneMinusConstantColor:
                    return Blend.InverseBlendFactor;
                default:
                    throw new ArgumentOutOfRangeException("blend");
            }
        }


        public void SetRenderState(RenderState renderState, uint value)
        {
            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        PolygonMode pm;
                        switch ((FillMode)value)
                        {
                            case FillMode.Point:
                                pm = PolygonMode.Point;
                                break;
                            case FillMode.Wireframe:
                                pm = PolygonMode.Line;
                                break;
                            case FillMode.Solid:
                                pm = PolygonMode.Fill;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
                        }
                        GL.PolygonMode(MaterialFace.FrontAndBack, pm);
                        return;
                    }
                case RenderState.CullMode:
                    {
                        switch ((Cull)value)
                        {
                            case Cull.None:
                                GL.Disable(EnableCap.CullFace);
                                GL.FrontFace(FrontFaceDirection.Ccw);
                                break;
                            case Cull.Clockwise:
                                GL.Enable(EnableCap.CullFace);
                                GL.FrontFace(FrontFaceDirection.Cw);
                                break;
                            case Cull.Counterclockwise:
                                GL.Enable(EnableCap.CullFace);
                                GL.FrontFace(FrontFaceDirection.Ccw);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
                        }
                    }
                    break;
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    break;
                case RenderState.ZFunc:
                    {
                        DepthFunction df;
                        switch ((Compare)value)
                        {
                            case Compare.Never:
                                df = DepthFunction.Never;
                                break;
                            case Compare.Less:
                                df = DepthFunction.Less;
                                break;
                            case Compare.Equal:
                                df = DepthFunction.Equal;
                                break;
                            case Compare.LessEqual:
                                df = DepthFunction.Lequal;
                                break;
                            case Compare.Greater:
                                df = DepthFunction.Greater;
                                break;
                            case Compare.NotEqual:
                                df = DepthFunction.Notequal;
                                break;
                            case Compare.GreaterEqual:
                                df = DepthFunction.Gequal;
                                break;
                            case Compare.Always:
                                df = DepthFunction.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("value");
                        }
                        GL.DepthFunc(df);
                    }
                    break;
                case RenderState.ZEnable:
                    if (value == 0)
                        GL.Disable(EnableCap.DepthTest);
                    else
                        GL.Enable(EnableCap.DepthTest);
                    break;
                case RenderState.AlphaBlendEnable:
                    if (value == 0)
                        GL.Disable(EnableCap.Blend);
                    else
                        GL.Enable(EnableCap.Blend);
                    break;
                case RenderState.BlendOperation:
                    int alphaMode;
                    GL.GetInteger(GetPName.BlendEquationAlpha, out alphaMode);
                    GL.BlendEquationSeparate(BlendOperationToOgl((BlendOperation)value), (BlendEquationMode)alphaMode);
                    break;
                case RenderState.BlendOperationAlpha:
                    int rgbMode;
                    GL.GetInteger(GetPName.BlendEquationRgb, out rgbMode);
                    GL.BlendEquationSeparate((BlendEquationMode)rgbMode, BlendOperationToOgl((BlendOperation)value));
                    break;
                case RenderState.SourceBlend:
                    {
                        int rgbDst, alphaSrc, alphaDst;
                        GL.GetInteger(GetPName.BlendDstRgb, out rgbDst);
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((BlendingFactorSrc)BlendToOgl((Blend)value),
                                             (BlendingFactorDest)rgbDst,
                                             (BlendingFactorSrc)alphaSrc,
                                             (BlendingFactorDest)alphaDst);
                    }
                    break;
                case RenderState.DestinationBlend:
                    {
                        int rgbSrc, alphaSrc, alphaDst;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((BlendingFactorSrc)rgbSrc,
                                             (BlendingFactorDest)BlendToOgl((Blend)value),
                                             (BlendingFactorSrc)alphaSrc,
                                             (BlendingFactorDest)alphaDst);
                    }
                    break;
                case RenderState.SourceBlendAlpha:
                    {
                        int rgbSrc, rgbDst, alphaDst;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(GetPName.BlendDstRgb, out rgbDst);
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        GL.BlendFuncSeparate((BlendingFactorSrc)rgbSrc,
                                             (BlendingFactorDest)rgbDst,
                                             (BlendingFactorSrc)BlendToOgl((Blend)value, true),
                                             (BlendingFactorDest)alphaDst);
                    }
                    break;
                case RenderState.DestinationBlendAlpha:
                    {
                        int rgbSrc, rgbDst, alphaSrc;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        GL.GetInteger(GetPName.BlendDstRgb, out rgbDst);
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        GL.BlendFuncSeparate((BlendingFactorSrc)rgbSrc,
                                             (BlendingFactorDest)rgbDst,
                                             (BlendingFactorSrc)alphaSrc,
                                             (BlendingFactorDest)BlendToOgl((Blend)value, true));
                    }
                    break;
                case RenderState.BlendFactor:
                    GL.BlendColor(Color.FromArgb((int)value));
                    break;
                /* TODO: Implement texture wrapping rahter as a texture property than a "global" render state. This is most
                 * convenient to implment with OpenGL/TK and easier to mimic in DirectX than the other way round.
                case RenderState.Wrap0:
                    break;
                case RenderState.Wrap1:
                    break;
                case RenderState.Wrap2:
                    break;
                case RenderState.Wrap3:
                    break;
                */
                default:
                    throw new ArgumentOutOfRangeException("renderState");
            }
        }

        public uint GetRenderState(RenderState renderState)
        {
            switch (renderState)
            {
                case RenderState.FillMode:
                    {
                        int pm;
                        FillMode ret;
                        GL.GetInteger(GetPName.PolygonMode, out pm);
                        switch ((PolygonMode)pm)
                        {
                            case PolygonMode.Point:
                                ret = FillMode.Point;
                                break;
                            case PolygonMode.Line:
                                ret = FillMode.Wireframe;
                                break;
                            case PolygonMode.Fill:
                                ret = FillMode.Solid;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("pm", "Value " + ((PolygonMode)pm) + " not handled");
                        }
                        return (uint)ret;
                    }
                case RenderState.CullMode:
                    {
                        int cullFace;
                        GL.GetInteger(GetPName.CullFace, out cullFace);
                        if (cullFace == 0)
                            return (uint)Cull.None;
                        int frontFace;
                        GL.GetInteger(GetPName.FrontFace, out frontFace);
                        if (frontFace == (int)FrontFaceDirection.Cw)
                            return (uint)Cull.Clockwise;
                        return (uint)Cull.Counterclockwise;
                    }
                case RenderState.Clipping:
                    // clipping is always on in OpenGL - This state is simply ignored
                    return 1; // == true
                case RenderState.ZFunc:
                    {
                        int depFunc;
                        GL.GetInteger(GetPName.DepthFunc, out depFunc);
                        Compare ret;
                        switch ((DepthFunction)depFunc)
                        {
                            case DepthFunction.Never:
                                ret = Compare.Never;
                                break;
                            case DepthFunction.Less:
                                ret = Compare.Less;
                                break;
                            case DepthFunction.Equal:
                                ret = Compare.Equal;
                                break;
                            case DepthFunction.Lequal:
                                ret = Compare.LessEqual;
                                break;
                            case DepthFunction.Greater:
                                ret = Compare.Greater;
                                break;
                            case DepthFunction.Notequal:
                                ret = Compare.NotEqual;
                                break;
                            case DepthFunction.Gequal:
                                ret = Compare.GreaterEqual;
                                break;
                            case DepthFunction.Always:
                                ret = Compare.Always;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("depFunc", "Value " + ((DepthFunction)depFunc) + " not handled");
                        }
                        return (uint)ret;
                    }
                case RenderState.ZEnable:
                    {
                        int depTest;
                        GL.GetInteger(GetPName.DepthTest, out depTest);
                        return (uint)(depTest);
                    }
                case RenderState.AlphaBlendEnable:
                    {
                        int blendEnable;
                        GL.GetInteger(GetPName.Blend, out blendEnable);
                        return (uint)(blendEnable);
                    }
                case RenderState.BlendOperation:
                    {
                        int rgbMode;
                        GL.GetInteger(GetPName.BlendEquationRgb, out rgbMode);
                        return (uint)BlendOperationFromOgl((BlendEquationMode)rgbMode);
                    }
                case RenderState.BlendOperationAlpha:
                    {
                        int alphaMode;
                        GL.GetInteger(GetPName.BlendEquationAlpha, out alphaMode);
                        return (uint)BlendOperationFromOgl((BlendEquationMode)alphaMode);
                    }
                case RenderState.SourceBlend:
                    {
                        int rgbSrc;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbSrc);
                        return (uint)BlendFromOgl(rgbSrc);
                    }
                case RenderState.DestinationBlend:
                    {
                        int rgbDst;
                        GL.GetInteger(GetPName.BlendSrcRgb, out rgbDst);
                        return (uint)BlendFromOgl(rgbDst);
                    }
                case RenderState.SourceBlendAlpha:
                    {
                        int alphaSrc;
                        GL.GetInteger(GetPName.BlendSrcAlpha, out alphaSrc);
                        return (uint)BlendFromOgl(alphaSrc);
                    }
                case RenderState.DestinationBlendAlpha:
                    {
                        int alphaDst;
                        GL.GetInteger(GetPName.BlendDstAlpha, out alphaDst);
                        return (uint)BlendFromOgl(alphaDst);
                    }
                case RenderState.BlendFactor:
                    int col;
                    GL.GetInteger(GetPName.BlendColorExt, out col);
                    return (uint)col;
                default:
                    throw new ArgumentOutOfRangeException("renderState");
            }
        }

        public void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }

        public void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            GL.ColorMask(red, green, blue, alpha);
        }

        public void Frustum(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            GL.Frustum(left, right, bottom, top, zNear, zFar);
        }
    }
}