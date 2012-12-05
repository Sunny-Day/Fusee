﻿using System;
using System.Collections.Generic;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.EgoPerspective
{
    public class EgoPerspective : RenderCanvas
    {
        private static float _angleHorz = 0.0f, _angleVert = 0.0f, _angleVelHorz = 0, _angleVelVert = 0, _rotationSpeed = 10.0f, _damping = 0.95f, _moveX = 0.0f, _moveY = 0.0f, _moveZ = 0.0f;
        private World _world;
        protected ShaderProgram Sp3;
        protected IShaderParam VColorParam;
        protected IShaderParam VLightDir;

        public override void Init()
        {
            _world = new World(RC, In);
            Geometry geo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Teapot.obj.model"));
            _world.addObject(geo, 0, 0, 1000);

            Sp3 = Shaders.GetShader("multiLight", RC);
            RC.SetShader(Sp3);
            ShaderMaterial m = RC.CreateMaterial(Sp3);          
            m.SetShininess(8);
            RC.SetMaterial(m);
            RC.SetLightAmbient(1, new float4(0, 1, 0, 1));
            RC.SetLightSpecular(1, new float4(0, 1, 0, 1));
            RC.SetLightDiffuse(1, new float4(0, 1, 0, 1));
            RC.SetLightPosition(1, new float3(-1000, 1000, 2000));
            RC.SetLightDirection(1, new float3(-1, 1, 2));
            // */
            RC.SetLightAmbient(0, new float4(0, 0, 1, 1));
            RC.SetLightSpecular(0, new float4(0, 0, 1, 1));
            RC.SetLightDiffuse(0, new float4(0, 0, 1, 1));
            RC.SetLightPosition(0, new float3(1000, 1000, 2000));
            RC.SetLightDirection(0, new float3(1, 1, 2));
            RC.ClearColor = new float4(1, 1, 1, 1);
            _angleHorz = 0;
            _rotationSpeed = 100.0f;

        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            Random zufall = new Random();
            float number = zufall.Next(1000) / 1000;
            _world.RenderWorld(_angleVert);
            Present();
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new EgoPerspective();
            app.Run();
        }

    }
}