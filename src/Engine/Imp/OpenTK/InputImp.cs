using System;
using OpenTK;
#if ANDROID
using OpenTK.Platform.Android;
#else
using OpenTK.Input;
#endif

namespace Fusee.Engine
{
    public class InputImp : IInputImp
    {
#if ANDROID
        protected AndroidGameView _gameWindow;
#else
        protected GameWindow _gameWindow;
#endif
        internal Keymapper _keyMapper;

        public InputImp(IRenderCanvasImp renderCanvas)
        {
            if (renderCanvas == null)
                throw new ArgumentNullException("renderCanvas");
            if (!(renderCanvas is RenderCanvasImp))
                throw new ArgumentException("renderCanvas must be of type RenderCanvasImp", "renderCanvas");
            _gameWindow = ((RenderCanvasImp)renderCanvas)._gameWindow;
#if ANDROID
#else
            _gameWindow.Keyboard.KeyDown += OnGameWinKeyDown;
            _gameWindow.Keyboard.KeyUp += OnGameWinKeyUp;
            _gameWindow.Mouse.ButtonDown += OnGameWinMouseDown;
            _gameWindow.Mouse.ButtonUp += OnGameWinMouseUp;
#endif
            _keyMapper = new Keymapper();
        }

        public void FrameTick(double time)
        {
            // Do Nothing
        }

        public Point GetMousePos()
        {
#if ANDROID
            return new Point{x=0, y=0};
#else
            return new Point{x = _gameWindow.Mouse.X, y = _gameWindow.Mouse.Y};
#endif
        }

        public int GetMouseWheelPos()
        {
#if ANDROID
            return 0;
#else
            return _gameWindow.Mouse.Wheel;
#endif
        }

        public event EventHandler<MouseEventArgs> MouseButtonDown;

#if ANDROID
#else
        protected void OnGameWinMouseDown(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonDown != null)
            {
                MouseButtons mb = MouseButtons.Unknown;
                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        mb = MouseButtons.Left;
                        break;
                    case MouseButton.Middle:
                        mb = MouseButtons.Middle;
                        break;
                    case MouseButton.Right:
                        mb = MouseButtons.Right;
                        break;
                 }
                MouseButtonDown(this, new MouseEventArgs
                                          {
                                             Button = mb,
                                             Position = new Point{x=mouseArgs.X, y=mouseArgs.Y}
                                          });   
            }
        }
#endif

        public event EventHandler<MouseEventArgs> MouseButtonUp;

#if ANDROID
#else
        protected void OnGameWinMouseUp(object sender, MouseButtonEventArgs mouseArgs)
        {
            if (MouseButtonUp != null)
            {
                MouseButtons mb = MouseButtons.Unknown;
                switch (mouseArgs.Button)
                {
                    case MouseButton.Left:
                        mb = MouseButtons.Left;
                        break;
                    case MouseButton.Middle:
                        mb = MouseButtons.Middle;
                        break;
                    case MouseButton.Right:
                        mb = MouseButtons.Right;
                        break;
                }
                MouseButtonUp(this, new MouseEventArgs
                {
                    Button = mb,
                    Position = new Point { x = mouseArgs.X, y = mouseArgs.Y }
                });
            }
        }
#endif      
        public event EventHandler<KeyEventArgs> KeyDown;

#if ANDROID
#else
        protected void OnGameWinKeyDown(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyDown != null)
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyDown(this, new KeyEventArgs
                                  {
                                      Alt = false,
                                      Control = false,
                                      Shift = false,
                                      KeyCode = _keyMapper[key.Key],
                                  });
            }
        }
#endif
        public event EventHandler<KeyEventArgs> KeyUp;

#if ANDROID
#else
        protected void OnGameWinKeyUp(object sender, KeyboardKeyEventArgs key)
        {
            if (KeyUp != null)
            {
                // TODO: implement correct Alt, Control, Shift behavior
                KeyUp(this, new KeyEventArgs
                {
                    Alt = false,
                    Control = false,
                    Shift = false,
                    KeyCode = _keyMapper[key.Key],
                });
            }
        }
#endif
                
    }
}
