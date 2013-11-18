using System;
using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.Math;
using System.Diagnostics;
using System.IO;
using JSIL.Meta;
using JSIL.Runtime;
using Geometry = LinqForGeometry.Core.Geometry;
using LFG.ExternalModules.Transformations;

namespace Examples.MageExample
{


    [FuseeApplication(Name = "Mage Example", Description = "Sample displaying a more complex character.")]
    public class MageExample : RenderCanvas
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool AttachConsole(int processId);
        private static long maintime;

        private static Stopwatch watch = new Stopwatch();

        protected Mesh Mesh1000verts;

        protected IShaderParam VColorParam;
        private ITexture _iTex;
        private IShaderParam _texture1ParamBody;

        private float _angleHorz;
        private float _angleVert;
        private float _angleVelHorz;
        private float _angleVelVert;

        private float _rotationSpeed;

        public MageExample()
        {
            watch.Start();
        }

        private struct Vertex
        {
            internal float3 Position;
            internal float3 Normals;
            internal float2 UVs;
        }

        private Vertex[] VerticesN;

        [JSPackedArray]
        private Vertex[] VerticesP;

        public unsafe override void Init()
        {
            AttachConsole(-1);

            Console.WriteLine("a: " + maintime);
            Console.WriteLine("b: " + watch.ElapsedMilliseconds);

            watch.Stop();
            watch.Reset();

            const int values = 1000000;
            VerticesN = new Vertex[values];
            VerticesP = PackedArray.New<Vertex>(values);

            for (int i = 0; i < VerticesN.Length; i++)
            {
                VerticesN[i].Position = new float3(i, i, i);
                VerticesN[i].Normals = new float3(i, i, i);
                VerticesN[i].UVs = new float2(i, i);

                VerticesP[i].Position = new float3(i, i, i);
                VerticesP[i].Normals = new float3(i, i, i);
                VerticesP[i].UVs = new float2(i, i);
            }

            watch.Reset();
            watch.Start();

            for (int i = 0; i < VerticesN.Length; i++)
            {
                var x = VerticesN[i].Normals;
                VerticesN[i].Normals = VerticesN[i].Position;
                VerticesN[i].Position = x;
                VerticesN[i].UVs = VerticesN[i].UVs - new float2(1, 1);
            }

            watch.Stop();
            Console.WriteLine("c: " + watch.ElapsedMilliseconds);
            watch.Reset();
            watch.Start();

                /*fixed (Vertex* p = &VerticesP[0])
                {
                    var pt = (float*) p;

                    for (int i = 0; i < VerticesP.Length; i++)
                    {
                        var x = pt[i*8 + 3];
                        var y = pt[i*8 + 4];
                        var z = pt[i*8 + 5];

                        pt[i*8 + 3] = pt[i*8 + 0];
                        pt[i*8 + 4] = pt[i*8 + 1];
                        pt[i*8 + 5] = pt[i*8 + 2];

                        pt[i*8 + 3] = x;
                        pt[i*8 + 4] = y;
                        pt[i*8 + 5] = z;

                        pt[i*8 + 6] = pt[i*8 + 6] - 1;
                        pt[i*8 + 7] = pt[i*8 + 7] - 1;
                    }
                }*/

            watch.Stop();
            Console.WriteLine("d: " + watch.ElapsedMilliseconds);
            watch.Reset();

            RC.ClearColor = new float4(0.5f, 1.0f, 0.5f, 1);

            // load meshes
            watch.Start();

                var meshgeo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/performancetestmesh.obj.model"));

                watch.Stop();
                Console.WriteLine("e: " + watch.ElapsedMilliseconds);

                watch.Reset();
                watch.Start();

                meshgeo.CreateNormals(80 * 3.141592 / 180.0);

                watch.Stop();
                Console.WriteLine("f: " + watch.ElapsedMilliseconds);
                watch.Reset();
                watch.Start();

                Mesh1000verts = meshgeo.ToMesh();              

            //Console.WriteLine("Mesh vertex count: " + Mesh1000verts.Vertices.Length);
            Console.WriteLine("g: " + watch.ElapsedMilliseconds);
            
            Environment.Exit(0);

            watch.Stop();
            watch.Reset();
            // set up shader, lights and textures
            var shade = MoreShaders.GetDiffuseTextureShader(RC);
            RC.SetShader(shade);

            RC.SetLightActive(0, 1);
            RC.SetLightPosition(0, new float3(5.0f, 0.0f, -2.0f));
            RC.SetLightAmbient(0, new float4(0.2f, 0.2f, 0.2f, 1.0f));
            RC.SetLightSpecular(0, new float4(0.1f, 0.1f, 0.1f, 1.0f));
            RC.SetLightDiffuse(0, new float4(0.8f, 0.8f, 0.8f, 1.0f));
            RC.SetLightDirection(0, new float3(-1.0f, 0.0f, 0.0f));

            RC.SetLightActive(1, 1);
            RC.SetLightPosition(1, new float3(-5.0f, 0.0f, -2.0f));
            RC.SetLightAmbient(1, new float4(0.5f, 0.5f, 0.5f, 1.0f));
            RC.SetLightSpecular(1, new float4(0.1f, 0.1f, 0.1f, 1.0f));
            RC.SetLightDiffuse(1, new float4(1.0f, 1.0f, 1.0f, 1.0f));
            RC.SetLightDirection(1, new float3(1.0f, 0.0f, 0.0f));

            _texture1ParamBody = shade.GetShaderParam("texture1");
            var imgData = RC.LoadImage("Assets/perfmeshtex.jpg");

            _iTex = RC.CreateTexture(imgData);

            // misc settings
            _angleHorz = 0;
            _angleVert = 0;

            _rotationSpeed = 1.5f;
            watch.Start();
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // mouse and input control
            _angleVelHorz = -1.0f*(float) Time.Instance.DeltaTime;
            _angleVelVert = 0.0f;

            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                _angleVelHorz = _rotationSpeed*Input.Instance.GetAxis(InputAxis.MouseX);
                _angleVelVert = _rotationSpeed*Input.Instance.GetAxis(InputAxis.MouseY);
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _angleHorz -= _rotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _angleHorz += _rotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _angleVert -= _rotationSpeed*(float) Time.Instance.DeltaTime;

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _angleVert += _rotationSpeed*(float) Time.Instance.DeltaTime;

            var mtxRot = float4x4.CreateRotationY(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            var mtxCam = float4x4.LookAt(0, 400, 600, 0, 0, 0, 0, 1, 0);

            // body
            RC.ModelView = float4x4.CreateTranslation(0, -200f, 0)*mtxRot*mtxCam;

            RC.SetShaderParamTexture(_texture1ParamBody, _iTex);

            RC.Render(Mesh1000verts);

            // left and right glove
            RC.ModelView = float4x4.CreateTranslation(0, -200f, 0)*mtxRot*mtxCam;


            // swap buffers
            Present();
            if (Time.Instance.Frames == 1000)
            {
                float elapsed = watch.ElapsedMilliseconds;
                watch.Stop();
                watch.Reset();
                Console.WriteLine("Total time for rendering 1000 Frames (ms): " + elapsed);
                float averagefps = 1000 / elapsed * 1000;
                Console.WriteLine("Average Frames per Seconds during rendering 1000 Frames: " + averagefps);
                
            }
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            maintime = DateTime.Now.Ticks;
            var app = new MageExample();
            app.Run();
        }
    }
}