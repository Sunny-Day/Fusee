using System;
using System.Runtime.InteropServices;
using Fusee.Engine;
using Fusee.Math;
using System.Diagnostics;
using System.IO;
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

        public override void Init()
        {

            AttachConsole(-1);


            // Für webtest nicht DateTime.Now.Ticks;(nicht implementiert) nutzen!!! (siehe main methode)
            Console.WriteLine("Performance Example Started");
            Console.WriteLine("Main startup time (ticks): " + maintime);
            Console.WriteLine("Init Time (ms): " + watch.ElapsedMilliseconds);
            watch.Stop();
            watch.Reset();
            RC.ClearColor = new float4(0.5f, 1.0f, 0.5f, 1);

            // load meshes
            watch.Start();
            var meshgeo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/performancetestmesh.obj.model"));
            Console.WriteLine("Load 10000 vertices Geometry file Time (ms): " + watch.ElapsedMilliseconds);
            watch.Stop();
            watch.Reset();
            watch.Start();
            Mesh1000verts = meshgeo.ToMesh();
            //Console.WriteLine("Mesh vertex count: " + Mesh1000verts.Vertices.Length);
            Console.WriteLine("Parse Geometry file to Mesh Time (ms): " + watch.ElapsedMilliseconds);
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