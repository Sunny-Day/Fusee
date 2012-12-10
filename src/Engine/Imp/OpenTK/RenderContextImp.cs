using System;
using System.Text;
using Fusee.Math;
using OpenTK;
#if ANDROID
using OpenTK.Graphics.ES20;
#else
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
#endif

namespace Fusee.Engine
{
    public class RenderContextImp : IRenderContextImp
    {
        

        public RenderContextImp(IRenderCanvasImp renderCanvas)
        {
        }
        

        public IShaderParam GetShaderParam(IShaderProgramImp shaderProgram, string paramName)
        {
            int h = GL.GetUniformLocation(((ShaderProgramImp)shaderProgram).Program, paramName);
            return (h == -1) ? null : new ShaderParam {handle = h};
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
                float* mF = (float*) (&val);
                GL.UniformMatrix4(((ShaderParam)param).handle, 1, false, mF);
            }
        }

        public void SetShaderParam(IShaderParam param, int val)
        {
            GL.Uniform1(((ShaderParam)param).handle, val);
        }
        
        public float4x4 ModelView
        {
            get 
            { throw new NotImplementedException(); }
            set 
            {
#if !ANDROID
                GL.MatrixMode(MatrixMode.Modelview);
                unsafe {GL.LoadMatrix((float*)(&value));}
#endif
            }
        }

        public float4x4 Projection
        {
            get
            { throw new NotImplementedException(); }
            set
            {
#if !ANDROID
                GL.MatrixMode(MatrixMode.Projection);
                unsafe { GL.LoadMatrix((float*)(&value)); }
#endif
            }
        }

        public float4 ClearColor
        {
            get
            {
                Vector4 ret; 
#if ANDROID
                float[] retret = new float[4];
                GL.GetFloat((All) GetPName.ColorClearValue, retret);
                ret = new Vector4(retret[0], retret[1], retret[2], retret[3]);
#else
                GL.GetFloat(GetPName.ColorClearValue, out ret);
#endif
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
                float ret = 0;
#if ANDROID
                GL.GetFloat((All)GetPName.DepthClearValue, ref ret);
#else
                GL.GetFloat(GetPName.DepthClearValue, out ret);
#endif
                 return ret;
            }
            set
            {
                GL.ClearDepth(value);
            }
        }

        public IShaderProgramImp CreateShader(string vs, string ps)
        {
            int statusCode = 0;

#if ANDROID
            StringBuilder sb = new StringBuilder(512);
            int vertexObject = GL.CreateShader(All.VertexShader);
            
            // Compile vertex shader
            GL.ShaderSource(vertexObject, 1, new string[]{vs}, (int[])null);
            GL.CompileShader(vertexObject);
            int len = 0;
            GL.GetShaderInfoLog(vertexObject, 512, ref len, sb);
            GL.GetShader(vertexObject, All.CompileStatus, ref statusCode);

            if (statusCode != 1)
                throw new ApplicationException(sb.ToString());

            // Compile pixel shader
            int fragmentObject = GL.CreateShader(All.FragmentShader);
            GL.ShaderSource(fragmentObject, 1, new string[] { ps }, (int[])null);
            GL.CompileShader(fragmentObject);
            /*
            GL.GetShaderInfoLog(fragmentObject, 512, (int[])null, sb);
            GL.GetShader(fragmentObject, All.CompileStatus, ref statusCode);

            if (statusCode != 1)
                throw new ApplicationException(sb.ToString());
            */
#else
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
#endif
            
            int program = GL.CreateProgram();
            GL.AttachShader(program, fragmentObject);
            GL.AttachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            GL.BindAttribLocation(program, Helper.VertexAttribLocation, Helper.VertexAttribName);
            GL.BindAttribLocation(program, Helper.ColorAttribLocation, Helper.ColorAttribName);
            GL.BindAttribLocation(program, Helper.NormalAttribLocation, Helper.NormalAttribName);
            GL.LinkProgram(program); // AAAARRRRRGGGGHHHH!!!! Must be called AFTER BindAttribLocation
            return new ShaderProgramImp {Program = program};
        }


        public void SetShader(IShaderProgramImp program)
        {
            GL.UseProgram(((ShaderProgramImp)program).Program);
        }

        public void Clear(ClearFlags flags)
        {
#if ANDROID
            GL.Clear((int) flags);
#else
            GL.Clear((ClearBufferMask)flags);
#endif
        }


        public void SetVertices(IMeshImp mr, float3[] vertices)
        {
            if (vertices == null || vertices.Length == 0)
            {
                throw new ArgumentException("Vertices must not be null or empty");
            }

            int vboBytes = 0;
            int vertsBytes = vertices.Length * 3 * sizeof(float);
#if ANDROID
            if (((MeshImp)mr).VertexBufferObject == 0)
                GL.GenBuffers(1, ref ((MeshImp)mr).VertexBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(vertsBytes), vertices, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, ref vboBytes);
            if (vboBytes != vertsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (vertices). Tried to upload {0} bytes, uploaded {1}.",
                    vertsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
#else
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
#endif
        }


        public void SetNormals(IMeshImp mr, float3[] normals)
        {
            if (normals == null || normals.Length == 0)
            {
                throw new ArgumentException("Normals must not be null or empty");
            }

            int vboBytes = 0;
            int normsBytes = normals.Length * 3 * sizeof(float);
#if ANDROID
            if (((MeshImp)mr).NormalBufferObject == 0)
                GL.GenBuffers(1, ref ((MeshImp)mr).NormalBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(normsBytes), normals, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, ref vboBytes);
            if (vboBytes != normsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading normal buffer to VBO (normals). Tried to upload {0} bytes, uploaded {1}.",
                    normsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
#else
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
#endif
        }


        public void SetColors(IMeshImp mr, uint[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentException("colors must not be null or empty");
            }

            int vboBytes = 0;
            int colsBytes = colors.Length * sizeof(uint);
#if ANDROID
            if (((MeshImp)mr).ColorBufferObject == 0)
                GL.GenBuffers(1, ref ((MeshImp)mr).ColorBufferObject);

            GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
            GL.BufferData(All.ArrayBuffer, (IntPtr)(colsBytes), colors, All.StaticDraw);
            GL.GetBufferParameter(All.ArrayBuffer, All.BufferSize, ref vboBytes);
            if (vboBytes != colsBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading color buffer to VBO (colors). Tried to upload {0} bytes, uploaded {1}.",
                    colsBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
#else
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
#endif
        }
        

        public void SetTriangles(IMeshImp mr, short[] triangleIndices)
        {
            if (triangleIndices == null || triangleIndices.Length == 0)
            {
                throw new ArgumentException("triangleIndices must not be null or empty");
            }
            ((MeshImp)mr).NElements = triangleIndices.Length;
            int vboBytes = 0;
            int trisBytes = triangleIndices.Length * sizeof(short);

#if ANDROID
            if (((MeshImp)mr).ElementBufferObject == 0)
                GL.GenBuffers(1, ref ((MeshImp)mr).ElementBufferObject);
            // Upload the   index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(All.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            GL.BufferData(All.ElementArrayBuffer, (IntPtr)(trisBytes), triangleIndices, All.StaticDraw);
            GL.GetBufferParameter(All.ElementArrayBuffer, All.BufferSize, ref vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
            GL.BindBuffer(All.ArrayBuffer, 0);
#else
           if (((MeshImp)mr).ElementBufferObject == 0)
                GL.GenBuffers(1, out ((MeshImp)mr).ElementBufferObject);
            // Upload the   index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(trisBytes), triangleIndices, BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out vboBytes);
            if (vboBytes != trisBytes)
                throw new ApplicationException(String.Format(
                    "Problem uploading vertex buffer to VBO (offsets). Tried to upload {0} bytes, uploaded {1}.",
                    trisBytes, vboBytes));
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
#endif
        }

        public void Render(IMeshImp mr)
        {
#if ANDROID
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.VertexAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, All.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, All.UnsignedByte, true, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.NormalAttribLocation);
                GL.BindBuffer(All.ArrayBuffer, ((MeshImp)mr).NormalBufferObject);
                GL.VertexAttribPointer(Helper.NormalAttribLocation, 3, All.Float, false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ElementBufferObject != 0)
            {
                GL.BindBuffer(All.ElementArrayBuffer, ((MeshImp)mr).ElementBufferObject);
                GL.DrawElements(All.Triangles, ((MeshImp)mr).NElements, All.UnsignedShort, IntPtr.Zero);
                //GL.DrawArrays(GL.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.VertexAttribLocation);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.ColorAttribLocation);
            }
            if (((MeshImp)mr).NormalBufferObject != 0)
            {
                GL.BindBuffer(All.ArrayBuffer, 0);
                GL.DisableVertexAttribArray(Helper.NormalAttribLocation);
            }
#else
            if (((MeshImp)mr).VertexBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.VertexAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).VertexBufferObject);
                GL.VertexAttribPointer(Helper.VertexAttribLocation, 3, VertexAttribPointerType.Float , false, 0, IntPtr.Zero);
            }
            if (((MeshImp)mr).ColorBufferObject != 0)
            {
                GL.EnableVertexAttribArray(Helper.ColorAttribLocation);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ((MeshImp)mr).ColorBufferObject);
                GL.VertexAttribPointer(Helper.ColorAttribLocation, 4, VertexAttribPointerType.UnsignedByte, true, 0, IntPtr.Zero);
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
#endif
        }

        public IMeshImp CreateMeshImp()
        {
            return new MeshImp();
        }

        public void Viewport(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
        }
    }
}
