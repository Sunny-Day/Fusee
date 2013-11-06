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
        #region Shader

        private const string VsBump = @"

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
       
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            uniform vec4 FUSEE_L0_AMBIENT;
            uniform vec4 FUSEE_L1_AMBIENT;

            uniform float FUSEE_L0_ACTIVE;
            uniform float FUSEE_L1_ACTIVE;

            varying vec2 vUV;
            varying vec3 lightDir[8];
            varying vec3 vNormal;
            varying vec4 endAmbient;
            varying vec3 eyeVector;

            vec3 vPos;
 
            void main()
            {
                vUV = fuUV;

                vNormal = normalize(mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal);
                eyeVector = mat3(FUSEE_MVP[0].xyz, FUSEE_MVP[1].xyz, FUSEE_MVP[2].xyz) * -fuVertex;
      
                endAmbient = vec4(0,0,0,0);

                if(FUSEE_L0_ACTIVE == 1.0) {
                    endAmbient += FUSEE_L0_AMBIENT;
                }

                if(FUSEE_L1_ACTIVE == 1.0) {
                    endAmbient += FUSEE_L1_AMBIENT;
                }

                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
            }";


        private const string PsBump = @"
            #ifdef GL_ES
                precision highp float;
            #endif

            uniform sampler2D texture1;
            uniform sampler2D normalTex;
            uniform float specularLevel;

            uniform vec4 FUSEE_L0_SPECULAR;
            uniform vec4 FUSEE_L1_SPECULAR;

            uniform float FUSEE_L0_ACTIVE;
            uniform float FUSEE_L1_ACTIVE;

            uniform vec4 FUSEE_L0_DIFFUSE;
            uniform vec4 FUSEE_L1_DIFFUSE;

            uniform vec3 FUSEE_L0_POSITION;
            uniform vec3 FUSEE_L1_POSITION;

            uniform vec3 FUSEE_L0_DIRECTION;
            uniform vec3 FUSEE_L1_DIRECTION;

            varying vec3 vNormal;
            varying vec2 vUV;
            varying vec4 endAmbient;
            varying vec3 eyeVector;
 
            void main()
            {       
                float maxVariance = 2.0;
                float minVariance = maxVariance/2.0;

                vec3 tempNormal = vNormal + normalize(texture2D(normalTex, vUV).rgb * maxVariance - minVariance);
 
                vec4 endSpecular = vec4(0,0,0,0);
                vec4 tempTexSpecular = texture2D(texture1, vUV);

                if(FUSEE_L0_ACTIVE == 1.0 ) {
                    vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L0_POSITION));
                    float L3NdotHV = max(min(dot(normalize(tempNormal), vHalfVector),1.0), 0.0);
                    float shine = pow(L3NdotHV, specularLevel) * 16.0 * tempTexSpecular.z;
                    endSpecular += FUSEE_L0_SPECULAR * shine;
                }

                if(FUSEE_L1_ACTIVE == 1.0) {
                    vec3 vHalfVector = normalize(normalize(eyeVector) - normalize(eyeVector - FUSEE_L1_POSITION));
                    float L3NdotHV = max(dot(normalize(tempNormal), vHalfVector), 0.0);
                    float shine = pow(L3NdotHV, specularLevel) * 16.0 * tempTexSpecular.z;
                    endSpecular += FUSEE_L1_SPECULAR * shine;
                }
    
                vec4 endIntensity = vec4(0,0,0,0);

                if(FUSEE_L0_ACTIVE == 1.0) {
                    float intensity = max(dot(-normalize(FUSEE_L0_DIRECTION),normalize(tempNormal)),0.0);
                    endIntensity += intensity * FUSEE_L0_DIFFUSE;
                }

                if(FUSEE_L1_ACTIVE == 1.0) {
                    float intensity = max(dot(-normalize(FUSEE_L1_DIRECTION),normalize(tempNormal)),0.0);
                    endIntensity += intensity * FUSEE_L1_DIFFUSE;
                }

                endIntensity += endSpecular;
                endIntensity += endAmbient; 
                gl_FragColor = texture2D(texture1, vUV) * endIntensity; 
            }";

        #endregion
        private static Stopwatch watch = new Stopwatch();

        protected Mesh Body, GloveL, GloveR;

        protected IShaderParam VColorParam;

        private ITexture _iTex;
        private ITexture _iTex2;
        private ITexture _iTex2Glove;
        private ITexture _iTexGlove;

        private IShaderParam _specularLevelBody;
        private IShaderParam _texture1ParamBody;
        private IShaderParam _texture2ParamBody;

        private float _angleHorz;
        private float _angleVert;
        private float _angleVelHorz;
        private float _angleVelVert;

        private float _rotationSpeed;

        public override void Init()
        {
            //maintime = DateTime.Now.Ticks;
            watch.Stop();

            AttachConsole(-1);
            Console.WriteLine("Mage Example Started");
            //Console.WriteLine("Main startup time (ticks): " + maintime);
            Console.WriteLine("Init Time (ms): " + watch.ElapsedMilliseconds);
            watch.Stop();
            watch.Reset();
            RC.ClearColor = new float4(0.5f, 0.5f, 0.5f, 1);

            // load meshes
            watch.Start();
            var bodygeo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/mageBodyOBJ.obj.model"));
            var GloveLgeo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/mageGloveLOBJ.obj.model"));
            var GloveRgeo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/mageGloveROBJ.obj.model"));
            Console.WriteLine("Load 3 Geometry files Time (ms): " + watch.ElapsedMilliseconds);
            watch.Stop();
            watch.Reset();
            watch.Start();
            Body = bodygeo.ToMesh();
            GloveL = GloveLgeo.ToMesh();
            GloveR = GloveRgeo.ToMesh();
            Console.WriteLine("Parse 3 Geometry files to Meshes Time (ms): " + watch.ElapsedMilliseconds);
            watch.Stop();
            watch.Reset();
            // set up shader, lights and textures
            var spBody = RC.CreateShader(VsBump, PsBump);
            RC.SetShader(spBody);

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

            _texture1ParamBody = spBody.GetShaderParam("texture1");
            _texture2ParamBody = spBody.GetShaderParam("normalTex");
            _specularLevelBody = spBody.GetShaderParam("specularLevel");

            var imgDataGlove = RC.LoadImage("Assets/HandAOMap.jpg");
            var imgData2Glove = RC.LoadImage("Assets/HandschuhNormalMap.jpg");

            _iTexGlove = RC.CreateTexture(imgDataGlove);
            _iTex2Glove = RC.CreateTexture(imgData2Glove);

            var imgData = RC.LoadImage("Assets/TextureAtlas.jpg");
            var imgData2 = RC.LoadImage("Assets/TextureAtlasNormal.jpg");

            _iTex = RC.CreateTexture(imgData);
            _iTex2 = RC.CreateTexture(imgData2);

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
            RC.SetShaderParamTexture(_texture2ParamBody, _iTex2);
            RC.SetShaderParam(_specularLevelBody, 64.0f);

            RC.Render(Body);

            // left and right glove
            RC.ModelView = float4x4.CreateTranslation(0, -200f, 0)*mtxRot*mtxCam;

            RC.SetShaderParamTexture(_texture1ParamBody, _iTexGlove);
            RC.SetShaderParamTexture(_texture2ParamBody, _iTex2Glove);
            RC.SetShaderParam(_specularLevelBody, 64.0f);

            RC.Render(GloveL);
            RC.Render(GloveR);

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
            
            watch.Start();
            var app = new MageExample();
            app.Run();
        }
    }
}