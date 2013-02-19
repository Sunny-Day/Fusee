﻿using System;
using OpenTK;
#if ANDROID
using Android.Views;
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
        protected int _touchX;
        protected int _touchY;
        protected int _2ndEnterX;
        protected int _2ndEnterY;
        protected int _buttonsDown;
        protected KeyCodes _currentKey;
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
            _gameWindow.Touch += OnGameWinTouch;
            _touchX = 0;
            _touchY = 0;
            _buttonsDown = 0;
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
            return new Point { x = _touchX, y = _touchY };
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

#if ANDROID
        // Contains a very rough translation of touch events to simulated mouse events.
        // Single finger swipe -> MouseDown, MouseMove, MouseUp
        // Double finger swipe -> Keypad key into main direction.
        protected void OnGameWinTouch(object sender, View.TouchEventArgs args)
        {
            int curX = (int)args.Event.GetX();
            int curY = (int)args.Event.GetY();
            var action = args.Event.Action;
            switch (action)
            {
                case MotionEventActions.Cancel:
                    break;
                case MotionEventActions.Down:
                    _touchX = curX;
                    _touchY = curY;
                    _buttonsDown++;
                    if (null != this.MouseButtonDown)
                        MouseButtonDown(this, new MouseEventArgs
                        {
                            Button = MouseButtons.Left,
                            Position = new Point { x = curX, y = curY }
                        });   
                    break;
                case MotionEventActions.Mask:
                    break;
                case MotionEventActions.Move:
                    if (_buttonsDown < 2)
                    {
                        _touchX = curX;
                        _touchY = curY;
                    }
                    else
                    {
                        if (KeyDown != null)
                        {
                            // Trigger a key event
                            int sX = 1;
                            int sY = 1;
                            int dX = (int) args.Event.GetX(1) - _2ndEnterX;
                            int dY = (int) args.Event.GetY(1) - _2ndEnterY;
                            if (dX < 0)
                            {
                                dX = -dX;
                                sX = -1;
                            }
                            if (dY < 0)
                            {
                                dY = -dY;
                                sY = -1;
                            }
                            if (dX > 50 || dY > 50)
                            {
                                if (dX > dY)
                                {
                                    _currentKey = (sX > 0) ? KeyCodes.Right : KeyCodes.Left;
                                }
                                else
                                {
                                    _currentKey = (sY > 0) ? KeyCodes.Down : KeyCodes.Up;
                                }
                                KeyDown(this, new KeyEventArgs() {Alt = false, Control = false, KeyCode = _currentKey});
                                _2ndEnterX = (int) args.Event.GetX(1);
                                _2ndEnterY = (int) args.Event.GetY(1);
                            }
                        }
                    }
                    break;
                case MotionEventActions.Outside:
                    break;
                case MotionEventActions.Pointer1Down:
                    _2ndEnterX = (int)args.Event.GetX(1);
                    _2ndEnterY = (int)args.Event.GetY(1);
                    _buttonsDown++;
                    break;
                case MotionEventActions.Pointer1Up:
                    if (KeyUp != null)
                        KeyUp(this, new KeyEventArgs() { Alt = false, Control = false, KeyCode = _currentKey });
                    _buttonsDown--;
                    break;
                case MotionEventActions.Pointer2Down:
                    _buttonsDown++;
                    _2ndEnterX = (int)args.Event.GetX(1);
                    _2ndEnterY = (int)args.Event.GetY(1);
                    break;
                case MotionEventActions.Pointer2Up:
                    if (KeyUp != null)
                        KeyUp(this, new KeyEventArgs() { Alt = false, Control = false, KeyCode = _currentKey });
                    _buttonsDown--;
                    break;
                case MotionEventActions.Pointer3Down:
                    break;
                case MotionEventActions.Pointer3Up:
                    break;
                case MotionEventActions.PointerIdMask:
                    break;
                case MotionEventActions.PointerIdShift:
                    break;
                case MotionEventActions.Up:
                    _touchX = curX;
                    _touchY = curY;
                    _buttonsDown--;
                    if (null != this.MouseButtonUp)
                        MouseButtonUp(this, new MouseEventArgs
                        {
                            Button = MouseButtons.Left,
                            Position = new Point { x = curX, y = curY }
                        });   
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#endif
    }
}
