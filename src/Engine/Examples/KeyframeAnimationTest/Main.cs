﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Fusee.Engine;
using Fusee.Math;
using Fusee.KeyFrameAnimation;
using Fusee.SceneManagement;
using Fusee.Xirkit;
using Fusse.KeyFrameAnimation;

namespace Examples.KeyframeAnimationTest
{

    public class A
    {
        public String StringA;
        public int ZahlA;
        public float3 Koordinaten;
        public double4x4 DieMatrix;

        public A()
        {
            ZahlA = 10;
            Koordinaten = new float3(9,9,9);
            DieMatrix = new double4x4(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16);
        }

        public float3 Koordinaten1
        {
            get { return Koordinaten; }
            set { Koordinaten = value; }
        }

        public int ZahlA1
        {
            get { return ZahlA; }
            set { ZahlA = value; }
        }

        public String StringA1
        {
            get { return StringA; }
            set { StringA = value; }
        }

       
    }

    public class B
    {
        public B()
        {
            ZahlB = 15;
            Float2B1 = new float2(3, 4);
            flag = false;
            Nachricht = "Es hat nicht geklappt :-( ";
        }

        public int ZahlB;
        public Boolean flag;
        public String Nachricht;


        public int ZahlB1
        {
            get { return ZahlB; }
            set { ZahlB = value; }
        }

        public float2 Float2B;

        public float2 Float2B1
        {
            get { return Float2B; }
            set { Float2B = value; }
        }
    }


    public class KeyframeAnimationTest : RenderCanvas
    {
        private SceneEntity Wuerfel;
        private Camera camera;
        private Channel<float3> _channel2;
        private Channel<float4> _channel1;

        //private Channel<float> _channel1;
        private Animation myAnim = new Animation(0);
        public override void Init()
        {

            A objektA = new A() {StringA = "3.1415"};
            B objektB = new B();

            //objektA.ZahlA1 = objektB.ZahlB1;
            Node NodeA = new Node(objektA);
            Node NodeB = new Node(objektB);

            Circuit CAB = new Circuit();

            CAB.AddRoot(NodeA);
            CAB.AddNode(NodeB);

            NodeA.Attach("DieMatrix", NodeB, "Nachricht");

            CAB.Execute();





            SceneManager.RC = RC;
            SceneEntity stativ = new SceneEntity("stativ", new ActionCode());
            DirectionalLight dir = new DirectionalLight(new float3(0, 10, -1), new float4(1, 1, 1, 1),new float4(1, 1, 1, 1),new float4(1, 1, 1, 1), new float3(0, 0, 0), 0);
            stativ.AddComponent(dir);
            camera = new Camera(stativ);
            stativ.transform.GlobalPosition = new float3(0, 0, 100);
            SceneManager.Manager.AddSceneEntity(stativ);
            camera.Resize(Width, Height);
            Geometry wuerfelGeo = MeshReader.ReadWavefrontObj(new StreamReader(@"Assets/Sphere.obj.model"));
            Wuerfel = new SceneEntity("wuerfel", new Material(MoreShaders.GetSpecularShader(RC)), new Renderer(wuerfelGeo));
            SceneManager.Manager.AddSceneEntity(Wuerfel);



            _channel2 = new Channel<float3>(Lerp.Float3Lerp);
            _channel1 = new Channel<float4>(Lerp.Float4Lerp,new float4(0.5f,0.5f,0.5f,0.5f));

            /*
            _channel1.AddKeyframe(5f, 32f);
            _channel1.AddKeyframe(10f, 2f);
            _channel1.AddKeyframe(2f, 4f);
            _channel1.AddKeyframe(3f, 8f);
            _channel1.AddKeyframe(4f, 16f);
            _channel1.AddKeyframe(0f, 8f);
            */
            
            Keyframe<float4> key0 = new Keyframe<float4>(0, new float4(1, 0, 1, 1));
            Keyframe<float4> key1 = new Keyframe<float4>(2, new float4(0.125f, 1, 0.125f, 1));
            Keyframe<float4> key2 = new Keyframe<float4>(4, new float4(0.250f, 0.75f, 0.250f, 1));
            Keyframe<float4> key3 = new Keyframe<float4>(6, new float4(0.5f, 0.5f, 0.5f, 1));
            Keyframe<float4> key4 = new Keyframe<float4>(8, new float4(0.75f, 0.25f, 0.75f, 1));
            Keyframe<float4> key5 = new Keyframe<float4>(10, new float4(1, 25, 0.125f, 1));
            Keyframe<float4> key6 = new Keyframe<float4>(0, new float4(0, 1, 0, 1));

            
            _channel1.AddKeyframe(new Keyframe<float4 >(0,new float4( 1,0,1,1)));
            _channel1.AddKeyframe(key1);
            _channel1.AddKeyframe(key2);
            _channel1.AddKeyframe(key3);
            _channel1.AddKeyframe(key4);
            _channel1.AddKeyframe(key5);
            _channel1.AddKeyframe(key6);
            //*/


            Keyframe<float3> key40 = new Keyframe<float3>(8, new float3(8, 0, 80));
            Keyframe<float3> key00 = new Keyframe<float3>(0, new float3(0, 0, 0));
            Keyframe<float3> key10 = new Keyframe<float3>(2, new float3(1, 2, 20));
            Keyframe<float3> key20 = new Keyframe<float3>(4, new float3(2, 4, 40));
            Keyframe<float3> key30 = new Keyframe<float3>(6, new float3(4, 4, 60));
            Keyframe<float3> key50 = new Keyframe<float3>(12, new float3(0, 4, 60));
            Keyframe<float3> key60 = new Keyframe<float3>(0, new float3(8, 8, 8));

            
            _channel2.AddKeyframe(key00);
            _channel2.AddKeyframe(key10);
            _channel2.AddKeyframe(key20);
            _channel2.AddKeyframe(key30);
            _channel2.AddKeyframe(key40);
            _channel2.AddKeyframe(key50);
            _channel2.AddKeyframe(key60);
            //*/

            myAnim.AddAnimation(_channel1, RC, "ClearColor");
            myAnim.AddAnimation(_channel2, Wuerfel, "transform.GlobalPosition");

        }

        public override void RenderAFrame()
        {
            // is called once a frame
            SceneManager.Manager.Traverse(this);

            myAnim.Animate();
        }



        public override void Resize()
        {
            RC.Viewport(0, 0, Width, Height);
            camera.Resize(Width, Height);
        }



        public static void Main()
        {
            var app = new KeyframeAnimationTest();
            app.Run();
        }
    }
}
