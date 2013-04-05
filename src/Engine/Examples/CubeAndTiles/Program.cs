using System.Collections.Generic;
#if ANDROID
using Android.App;
// using TV.Ouya.Console.Api;
#endif
using Android.Views;
using Fusee.Engine;
using Fusee.Math;

#if OUYA
enum OuyaController
{
 // Field descriptor #59 I
   BUTTON_O = 96,
  
  // Field descriptor #59 I
   BUTTON_U = 99,
  
  // Field descriptor #59 I
   BUTTON_Y = 100,
  
  // Field descriptor #59 I
   BUTTON_A = 97,
  
  // Field descriptor #59 I
   BUTTON_L1 = 102,
  
  // Field descriptor #59 I
   BUTTON_L2 = 104,
  
  // Field descriptor #59 I
   BUTTON_R1 = 103,
  
  // Field descriptor #59 I
   BUTTON_R2 = 105,
  
  // Field descriptor #59 I
   BUTTON_SYSTEM = 3,
  
  // Field descriptor #59 I
   AXIS_LS_X = 0,
  
  // Field descriptor #59 I
   AXIS_LS_Y = 1,
  
  // Field descriptor #59 I
   AXIS_RS_X = 11,
  
  // Field descriptor #59 I
   AXIS_RS_Y = 14,
  
  // Field descriptor #59 I
   AXIS_L2 = 17,
  
  // Field descriptor #59 I
   AXIS_R2 = 18,
  
  // Field descriptor #59 I
   BUTTON_DPAD_UP = 19,
  
  // Field descriptor #59 I
   BUTTON_DPAD_RIGHT = 22,
  
  // Field descriptor #59 I
   BUTTON_DPAD_DOWN = 20,
  
  // Field descriptor #59 I
   BUTTON_DPAD_LEFT = 21,
  
  // Field descriptor #59 I
   BUTTON_R3 = 107,
  
  // Field descriptor #59 I
   BUTTON_L3 = 106,
}
#endif



namespace Examples.CubeAndTiles
{
    public class CubeAndTiles : RenderCanvas
    {
        // GLSL
        protected string Vs = @"
            #ifndef GL_ES
                #version 120
            #endif

            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
        
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
                vUV = fuUV;
            }";

        protected string Ps = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D vTexture;
            uniform vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;

            uniform int vUseAnaglyph;

            void main()
            {
                vec4 colTex = vColor * texture2D(vTexture, vUV);

                if (vUseAnaglyph != 0)
                {
                    vec4 _redBalance = vec4(0.1, 0.65, 0.25, 0);
                    float _redColor = (colTex.r * _redBalance.r + colTex.g * _redBalance.g + colTex.b * _redBalance.b) * 1.5;

                    gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * vec4(_redColor, colTex.g, colTex.b, 1) * dot(vNormal, vec3(0, 0, 1)) * 1.4;
                } else {
                    gl_FragColor = dot(vColor, vec4(0, 0, 0, 1)) * colTex * dot(vNormal, vec3(0, 0, 1));
                }
            }";

        // variables
        private static Level _exampleLevel;
        private static Anaglyph3D _anaglyph3D;

        private static float _angleHorz = 0.4f;
        private static float _angleVert = -1.0f;
        private static float _angleVelHorz = 0.05f, _angleVelVert;

        private static bool _topView;
        private static KeyCodes _lastKey = KeyCodes.None;

        private const float RotationSpeed = 10.0f;
        private const float Damping = 0.95f;

#if OUYA
        private bool _ouyaL, _ouyaR, _ouyaU, _ouyaD;
        private float _ouyaHorz, _ouyaVert;
#endif

#if ANDROID
        private Activity _activity;          
#endif

        public CubeAndTiles(Dictionary<string, object> globals = null) : base(globals)
        {
#if ANDROID
            _activity = (Activity) globals["Context"];
#endif
        }

        // Init()
        public override void Init()
        {
            ShaderProgram sp = RC.CreateShader(Vs, Ps);
            
            RC.SetShader(sp);
            RC.ClearColor = new float4(0, 0, 0, 1);

            _anaglyph3D = new Anaglyph3D(RC);
#if ANDROID
            _exampleLevel = new Level(RC, sp, 0, _anaglyph3D, _activity);
#else
            _exampleLevel = new Level(RC, sp, 0, _anaglyph3D);
#endif

#if OUYA
            _ouyaL =_ouyaR =_ouyaU =_ouyaD = false;
            _ouyaHorz =_ouyaVert = 0.0f;
#endif        
        }


        
        // RenderAFrame()
        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // keyboard
            if (_lastKey == KeyCodes.None)
            {
                if (Input.Instance.IsKeyDown(KeyCodes.V))
                {
                    _angleVelHorz = 0.0f;
                    _angleVelVert = 0.0f;

                    if (_topView)
                    {
                        _angleHorz = 0.4f;
                        _angleVert = -1.0f;

                        _topView = false;
                    }
                    else
                    {
                        _angleHorz = 0.0f;
                        _angleVert = 0.0f;
                        _topView = true;
                    }

                    _lastKey = KeyCodes.V;
                }

                if (Input.Instance.IsKeyDown(KeyCodes.C))
                {
                    _exampleLevel.UseAnaglyph3D = !_exampleLevel.UseAnaglyph3D;
                    _lastKey = KeyCodes.C;
                }
            }
            else if (!Input.Instance.IsKeyDown(_lastKey))
                _lastKey = KeyCodes.None;

            if (Input.Instance.IsKeyDown(KeyCodes.Left))
                _exampleLevel.MoveCube(Level.Directions.Left);

            if (Input.Instance.IsKeyDown(KeyCodes.Right))
                _exampleLevel.MoveCube(Level.Directions.Right);

            if (Input.Instance.IsKeyDown(KeyCodes.Up))
                _exampleLevel.MoveCube(Level.Directions.Forward);

            if (Input.Instance.IsKeyDown(KeyCodes.Down))
                _exampleLevel.MoveCube(Level.Directions.Backward);

#if OUYA
            if (_ouyaL)
                _exampleLevel.MoveCube(Level.Directions.Left);

            if (_ouyaR)
                _exampleLevel.MoveCube(Level.Directions.Right);

            if (_ouyaU)
                _exampleLevel.MoveCube(Level.Directions.Forward);

            if (_ouyaD)
                _exampleLevel.MoveCube(Level.Directions.Backward);
#endif


            // mouse
            if (Input.Instance.GetAxis(InputAxis.MouseWheel) > 0)
                _exampleLevel.ZoomCamera(50);

            if (Input.Instance.GetAxis(InputAxis.MouseWheel) < 0)
                _exampleLevel.ZoomCamera(-50);

            if (Input.Instance.IsButtonDown(MouseButtons.Left))
            {
                _angleVelHorz = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseX) * (float) Time.Instance.DeltaTime;
                _angleVelVert = RotationSpeed * Input.Instance.GetAxis(InputAxis.MouseY) * (float) Time.Instance.DeltaTime;
            }
#if OUYA
            else if (_ouyaHorz != 0.0f || _ouyaVert != 0.0f)
            {
                _angleVelHorz = RotationSpeed * _ouyaHorz * (float)Time.Instance.DeltaTime;
                _angleVelVert = RotationSpeed * _ouyaVert * (float)Time.Instance.DeltaTime;                
            }
#endif
            else
            {
                _angleVelHorz *= Damping;
                _angleVelVert *= Damping;
            }



            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;

            var mtxRot = float4x4.CreateRotationZ(_angleHorz)*float4x4.CreateRotationX(_angleVert);
            _exampleLevel.Render(mtxRot, Time.Instance.DeltaTime);

            Present();
        }

        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1500, 3800);
        }

        public static void Main()
        {
            var app = new CubeAndTiles();
            app.Run();
        }

#if OUYA
        public bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            switch ((int)keyCode)
            {
                case (int)OuyaController.BUTTON_DPAD_DOWN:
                    _ouyaD = true;
                    return true;
                case (int)OuyaController.BUTTON_DPAD_LEFT:
                    _ouyaL = true;
                    return true;
                case (int)OuyaController.BUTTON_DPAD_UP:
                    _ouyaU = true;
                    return true;
                case (int)OuyaController.BUTTON_DPAD_RIGHT:
                    _ouyaR = true;
                    return true;
            }
            return false;
        }

        public bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            switch ((int)keyCode)
            {
                case (int)OuyaController.BUTTON_DPAD_DOWN:
                    _ouyaD = false;
                    return true;
                case (int)OuyaController.BUTTON_DPAD_LEFT:
                    _ouyaL = false;
                    return true;
                case (int)OuyaController.BUTTON_DPAD_UP:
                    _ouyaU = false;
                    return true;
                case (int)OuyaController.BUTTON_DPAD_RIGHT:
                    _ouyaR = false;
                    return true;
            }
            return false;
        }

        public bool OnGenericMotionEvent(MotionEvent e)
        {
            _ouyaHorz = e.GetAxisValue((Axis)OuyaController.AXIS_RS_X)*0.5f;
            _ouyaVert = e.GetAxisValue((Axis)OuyaController.AXIS_RS_Y)*0.5f;

            return false;
            //throw new System.NotImplementedException();
        }
#endif

    }
}