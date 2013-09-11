/* This file contains the hand generated part of the FUSEE implementation.
   Classes defined here are used and called by the JSIL cross compiled part of FUSEE.
   This file creates the connection to the underlying WebGL part.

	Just for the records: The first version of this file was generated using 
	JSIL v0.5.0 build 25310. Until then it was changed and maintained manually.
*/

var $WebGLImp = JSIL.DeclareAssembly("Fusee.Engine.Imp.WebGL");
var $WebAudioImp = JSIL.GetAssembly("Fusee.Engine.Imp.WebAudio");
var $WebNetImp = JSIL.GetAssembly("Fusee.Engine.Imp.WebNet");

var $fuseeCommon = JSIL.GetAssembly("Fusee.Engine.Common");
var $fuseeMath = JSIL.GetAssembly("Fusee.Math.Core");

var $fuseeFirstGetShaderParamCall = false;

JSIL.DeclareNamespace("Fusee");
JSIL.DeclareNamespace("Fusee.Engine");

var $ColorUintCtor_U = function () {
    return ($ColorUintCtor_U = JSIL.Memoize(new JSIL.ConstructorSignature($asm01.TypeRef("Fusee.Engine.ColorUint"), [$jsilcore.TypeRef("System.UInt32")])))();
};
var $ColorUintCtor_S_S_S_S = function () {
    return ($ColorUintCtor_S_S_S_S = JSIL.Memoize(new JSIL.ConstructorSignature($jsilcore.TypeRef("Fusee.Engine.ColorUint"), [
        $jsilcore.TypeRef("System.Single"), $jsilcore.TypeRef("System.Single"),
        $jsilcore.TypeRef("System.Single"), $jsilcore.TypeRef("System.Single")
    ])))();
};


JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.TheEmptyDummyClass", true, [], function ($) {
   $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IShaderResImp"));
     $.Field({ Static: false, Public: false }, "test", $.Object, null);
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.ShaderResImp", true, [], function ($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IShaderResImp"));

    $.Field({ Static: false, Public: true }, "Program",$.Object, null);
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.TextureRes", true, [], function ($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.ITextureRes"));

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
    }
  );

    $.Field({ Static: false, Public: true }, "handle", $.Object, null);
});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.ShaderParam", true, [], function ($) {

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
    }
  );

    $.Field({ Static: false, Public: true }, "handle",$.Object, null);
    $.Field({ Static: false, Public: true }, "id", $.Int32, null);     // to uniquely identify shader parameters
});


JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.ImagehelperImp", true, [], function ($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IImagehelperImp"));

    $.Method({ Static: false, Public: true }, "IImagehelperImp_CreateImage",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.ImageData"), [$.Int32, $.Int32, $.String]),
        function IImagehelperImp_CreateImage(width, height, bgcolor) {

            var canvas = document.createElement("canvas");
            canvas.width = width;
            canvas.height = height;

            var context = canvas.getContext("2d");
            context.fillStyle = bgcolor;
            context.fillRect(0, 0, width, height);

            var myData = context.getImageData(0, 0, width, height);
            var imageData = new $fuseeCommon.Fusee.Engine.ImageData();

            imageData.Width = width;
            imageData.Height = height;
            imageData.Stride = width * 4; //TODO: Adjust pixel-size
            imageData.PixelData = myData.data;

            isloaded = true;
            return imageData;


        }
    );


    $.Method({ Static: false, Public: true }, "IImagehelperImp_LoadImage",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.ImageData"), [$.String]),
        function IImagehelperImp_LoadImage(filename) {
            var image = JSIL.Host.getImage(filename);
            var canvas = document.createElement("canvas");
            canvas.width = image.width;
            canvas.height = image.height;
            var context = canvas.getContext("2d");

            context.drawImage(image, 0, 0);
            var myData = context.getImageData(0, 0, image.width, image.height);
            var imageData = new $fuseeCommon.Fusee.Engine.ImageData();
            imageData.Width = image.width;
            imageData.Height = image.height;
            imageData.Stride = image.width * 4; //TODO: Adjust pixel-size
            imageData.PixelData = myData.data;
            isloaded = true;
            return imageData;
        }
    );

    $.Method({ Static: false, Public: true }, "IImagehelperImp_TextOnImage",
        new JSIL.MethodSignature(null, [$fuseeCommon.TypeRef("Fusee.Engine.ImageData"), $.String, $.Int32, $.String, $.String, $.Int32, $.Int32]),
        function IImagehelperImp_TextOnImage(imgData, fontname, fontsize, text, textcolor, startposx, startposy) {

            var canvas = document.createElement("canvas");
            canvas.width = imgData.Width;
            canvas.height = imgData.Height;

            var context = canvas.getContext("2d");
            var myData = context.createImageData(canvas.width, canvas.height);
            for (var i = 0; i < imgData.Width * imgData.Height * 4; i++) {
                myData.data[i] = imgData.PixelData[i];
            }
            context.putImageData(myData, 0, 0);

            var font = fontsize + "px " + fontname;
            context.font = font;
            context.fillStyle = textcolor;
            context.textBaseline = "top";
            context.fillText(text, startposx, startposy);

            var myData2 = context.getImageData(0, 0, canvas.width, canvas.height);
            imgData.PixelData = myData2.data;
            isloaded = true;
            // return imgData;
        }
    );



});


JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.RenderCanvasImp", true, [], function ($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp"));

    //var gl;
    //var theCanvas;
    $.Field({ Static: false, Public: false }, "gl", $.Object, null);
    $.Field({ Static: false, Public: true }, "theCanvas", $.Object, null);
    $.Field({ Static: false, Public: false }, "deltaTime", $.Double, null);
    $.Field({ Static: false, Public: false }, "lastFrame", $.Double, null);
    $.Field({ Static: false, Public: false }, "currWidth", $.Int32, null);
    $.Field({ Static: false, Public: false }, "currHeight", $.Int32, null);

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
        this.theCanvas = document.getElementById("canvas");
        this.gl = this.theCanvas.getContext("experimental-webgl");
        this.currWidth = 0;
        this.currHeight = 0;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_DeltaTime",
    new JSIL.MethodSignature($.Double, []),
    function get_DeltaTime() {
        return this.deltaTime / 1000.0;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_Height",
    new JSIL.MethodSignature($.Int32, []),
    function get_Height() {
        return this.gl.drawingBufferHeight;
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_Width",
    new JSIL.MethodSignature($.Int32, []),
    function get_Width() {
        return this.gl.drawingBufferWidth;
    }
  );

	$.Method({ Static: false, Public: true }, "IRenderCanvasImp_get_VerticalSync",
      new JSIL.MethodSignature($.Boolean, []),
        function get_VerticalSync() {
          return false;
        }
    );

	$.Method({ Static: false, Public: true }, "IRenderCanvasImp_set_VerticalSync",
      new JSIL.MethodSignature(null, [$.Boolean]),
        function set_VerticalSync() {
          // not implemented
        }
    );
    
	$.Method({ Static: false, Public: true }, "IRenderCanvasImp_set_Caption",
      new JSIL.MethodSignature(null, [$.String]),
        function set_Caption() {
            // not implemented
        }
    );
	
  
    $.Field({ Static: false, Public: false }, "IRenderCanvasImp_Init", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_Init",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.InitEventArgs")])]),
        function IRenderCanvasImp_add_Init(value) {
            var eventHandler = this.IRenderCanvasImp_Init;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.InitEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Init"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_Init",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.InitEventArgs")])]),
        function IRenderCanvasImp_remove_Init(value) {
            var eventHandler = this.IRenderCanvasImp_Init;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.InitEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Init"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );


    $.Method({ Static: false, Public: true }, "DoInit",
    new JSIL.MethodSignature(null, []),
    function DoInit() {
        if (this.IRenderCanvasImp_Init !== null) {
            this.IRenderCanvasImp_Init(this, (new $fuseeCommon.Fusee.Engine.InitEventArgs()).__Initialize__({
        }));
    }
}
  );

    $.Field({ Static: false, Public: false }, "IRenderCanvasImp_UnLoad", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_UnLoad",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.InitEventArgs")])]),
        function IRenderCanvasImp_add_UnLoad(value) {
            var eventHandler = this.IRenderCanvasImp_UnLoad;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.InitEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_UnLoad"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_UnLoad",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.InitEventArgs")])]),
        function IRenderCanvasImp_remove_UnLoad(value) {
            var eventHandler = this.IRenderCanvasImp_UnLoad;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.InitEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_UnLoad"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );


    $.Method({ Static: false, Public: true }, "DoUnLoad",
    new JSIL.MethodSignature(null, []),
    function DoUnLoad() {
        if (this.IRenderCanvasImp_UnLoad !== null) {
            this.IRenderCanvasImp_UnLoad(this, (new $fuseeCommon.Fusee.Engine.InitEventArgs()).__Initialize__({
        }));
    }
}
  );

$.Field({ Static: false, Public: false }, "IRenderCanvasImp_Render", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
    return null;
});

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_Render",
            new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.RenderEventArgs")])]),
            function IRenderCanvasImp_add_Render(value) {
                var eventHandler = this.IRenderCanvasImp_Render;
                do {
                    var eventHandler2 = eventHandler;
                    var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                    eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.RenderEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Render"), value2, eventHandler2);
                } while (eventHandler !== eventHandler2);
            }
        );

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_Render",
            new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.RenderEventArgs")])]),
            function IRenderCanvasImp_remove_Render(value) {
                var eventHandler = this.IRenderCanvasImp_Render;
                do {
                    var eventHandler2 = eventHandler;
                    var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                    eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.RenderEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Render"), value2, eventHandler2);
                } while (eventHandler !== eventHandler2);
            }
        );



$.Method({ Static: false, Public: true }, "DoRender",
    new JSIL.MethodSignature(null, []),
    function DoRender() {
        if (this.IRenderCanvasImp_Render !== null) {
            this.IRenderCanvasImp_Render(this, (new $fuseeCommon.Fusee.Engine.RenderEventArgs()).__Initialize__({
        }));
    }
}
  );

$.Field({ Static: false, Public: false }, "IRenderCanvasImp_Resize", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
    return null;
});

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_add_Resize",
                new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.ResizeEventArgs")])]),
                function IRenderCanvasImp_add_Resize(value) {
                    var eventHandler = this.IRenderCanvasImp_Resize;
                    do {
                        var eventHandler2 = eventHandler;
                        var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                        eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.ResizeEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Resize"), value2, eventHandler2);
                    } while (eventHandler !== eventHandler2);
                }
            );

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_remove_Resize",
                new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.ResizeEventArgs")])]),
                function IRenderCanvasImp_remove_Resize(value) {
                    var eventHandler = this.IRenderCanvasImp_Resize;
                    do {
                        var eventHandler2 = eventHandler;
                        var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                        eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.ResizeEventArgs))(/* ref */new JSIL.MemberReference(this, "IRenderCanvasImp_Resize"), value2, eventHandler2);
                    } while (eventHandler !== eventHandler2);
                }
            );



$.Method({ Static: false, Public: true }, "DoResize",
        new JSIL.MethodSignature(null, []),
        function DoResize() {
            if (this.IRenderCanvasImp_Resize !== null) {
                this.IRenderCanvasImp_Resize(this, (new $fuseeCommon.Fusee.Engine.ResizeEventArgs()).__Initialize__({
            }));
        }
    }
      );


$.Method({ Static: false, Public: true }, "IRenderCanvasImp_Present",
    new JSIL.MethodSignature(null, []),
    function IRenderCanvasImp_Present() {
    }
  );


$.Method({ Static: false, Public: true }, "frameTicker",
    new JSIL.MethodSignature(null, []),
    function frameTicker() {
        if (this.currWidth != this.gl.drawingBufferWidth || this.currHeight != this.gl.drawingBufferHeight) {
            this.currWidth = this.gl.drawingBufferWidth;
            this.currHeight = this.gl.drawingBufferHeight;
            this.DoResize();
        }

        var now = +new Date;
        this.DoRender();
        this.deltaTime = now - this.lastFrame;
        this.lastFrame = now;
        window.requestAnimFrame(this.frameTicker.bind(this), this.theCanvas);
    }
  );

$.Method({ Static: false, Public: true }, "IRenderCanvasImp_Run",
    new JSIL.MethodSignature(null, []),
    function IRenderCanvasImp_Run() {
        this.lastFrame = +new Date;
        this.deltaTime = 0.0;
        // canvas initialization is now in the constructor:
        // this.theCanvas = document.getElementById("canvas");
        // this.gl = this.theCanvas.getContext("experimental-webgl");
        this.DoInit();
        this.currWidth = 0;
        this.currHeight = 0;
        this.frameTicker();
    }
  );

$.Property({ Static: false, Public: true }, "IRenderCanvasImp_DeltaTime");

$.Property({ Static: false, Public: true }, "IRenderCanvasImp_Height");

$.Property({ Static: false, Public: true }, "IRenderCanvasImp_Width");

});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.RenderContextImp", true, [], function($) {
    $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IRenderContextImp"));

    $.Field({ Static: false, Public: false }, "gl", $.Object, null);
    $.Field({ Static: false, Public: false }, "_currentTextureUnit", $.Int32, null);
    $.Field({ Static: false, Public: false }, "_shaderParam2TexUnit", $.Object, null);
    $.Field({ Static: false, Public: false }, "_currentShaderParamHandle", $.Int32, null);


    $.Method({ Static: false, Public: true }, ".ctor",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.RenderCanvasImp")]),
        function _ctor(renderCanvas) {
            this.gl = document.getElementById("canvas").getContext("experimental-webgl");
            this.gl.enable(this.gl.DEPTH_TEST);
            this.gl.enable(this.gl.CULL_FACE);
            this.gl.clearColor(0.0, 0.0, 0.2, 1.0);
            this._currentTextureUnit = 0;

        }
    );


    // <IRenderContextImp Properties implementation>
    // Note: unlike the method interface-implementations, the Property interface-implementations
    // are NOT to be prefixed with the interface name.
    $.Property({ Static: false, Public: true }, "ModelView");

    $.Method({ Static: false, Public: true }, "get_ModelView",
        new JSIL.MethodSignature($asm00.TypeRef("Fusee.Math.float4x4"), []),
        function get_ModelView() {
            return $asm00.Fusee.Math.float4x4.Identity;
        }
    );

    $.Method({ Static: false, Public: true }, "set_ModelView",
        new JSIL.MethodSignature(null, [$asm00.TypeRef("Fusee.Math.float4x4")]),
        function set_ModelView(value) {
        }
    );

    $.Property({ Static: false, Public: true }, "Projection");

    $.Method({ Static: false, Public: true }, "get_Projection",
        new JSIL.MethodSignature($asm00.TypeRef("Fusee.Math.float4x4"), []),
        function get_Projection() {
            return $asm00.Fusee.Math.float4x4.Identity;
        }
    );

    $.Method({ Static: false, Public: true }, "set_Projection",
        new JSIL.MethodSignature(null, [$asm00.TypeRef("Fusee.Math.float4x4")]),
        function set_Projection(value) {
        }
    );


    $.Property({ Static: false, Public: true }, "ClearColor");

    $.Method({ Static: false, Public: true }, "get_ClearColor",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Math.float4"), []),
        function get_ClearColor() {
            var ret = this.gl.getParameter(this.gl.COLOR_CLEAR_VALUE);
            return new $fuseeCommon.Fusee.Math.float4(ret[0], ret[1], ret[2], ret[3]);
        }
    );

    $.Method({ Static: false, Public: true }, "set_ClearColor",
        new JSIL.MethodSignature(null, [$fuseeCommon.TypeRef("Fusee.Math.float4")]),
        function set_ClearColor(value) {
            this.gl.clearColor(value.x, value.y, value.z, value.w);
        }
    );

    $.Property({ Static: false, Public: true }, "ClearDepth");

    $.Method({ Static: false, Public: true }, "get_ClearDepth",
        new JSIL.MethodSignature($.Single, []),
        function get_ClearDepth() {
            return this.gl.getParameter(this.gl.DEPTH_CLEAR_VALUE);
            ;
        }
    );

    $.Method({ Static: false, Public: true }, "set_ClearDepth",
        new JSIL.MethodSignature(null, [$.Single]),
        function set_ClearDepth(value) {
            this.gl.clearDepth(value);
        }
    );
    // </IRenderContextImp Properties implementation>


    $.Method({ Static: false, Public: true }, "IRenderContextImp_CreateTexture",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.ITextureRes"), [$fuseeCommon.TypeRef("Fusee.Engine.ImageData")]),
        function IRenderContextImp_CreateTexture(img) {
            var ubyteView = new Uint8Array(img.PixelData);

            var glTexOb = this.gl.createTexture();
            this.gl.bindTexture(this.gl.TEXTURE_2D, glTexOb);
            this.gl.pixelStorei(this.gl.UNPACK_FLIP_Y_WEBGL, true);
            this.gl.texImage2D(this.gl.TEXTURE_2D, 0, this.gl.RGBA, img.Width, img.Height, 0,
                this.gl.RGBA, this.gl.UNSIGNED_BYTE, ubyteView);


            this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_MIN_FILTER, this.gl.LINEAR);
            this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_MAG_FILTER, this.gl.LINEAR);

            this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_WRAP_S, this.gl.CLAMP_TO_EDGE);
            this.gl.texParameteri(this.gl.TEXTURE_2D, this.gl.TEXTURE_WRAP_T, this.gl.CLAMP_TO_EDGE);

            var texRet = new $WebGLImp.Fusee.Engine.TextureRes();
            texRet.handle = glTexOb;

            return texRet;
        }
    );


    $.Method({ Static: false, Public: true }, "IRenderContextImp_Clear",
        new JSIL.MethodSignature(null, [$asm02.TypeRef("RenderEngine.ClearFlags")]),
        function IRenderContextImp_Clear(flags) {
            this.gl.clear(flags.value);
        }
    );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_ColorMask",
        new JSIL.MethodSignature(null, [$.Boolean, $.Boolean, $.Boolean, $.Boolean]),
        function IRenderContextImp_ColorMask(red, green, blue, alpha) {
            this.gl.colorMask(red, green, blue, alpha);
        }
    );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_CreateShader",
        new JSIL.MethodSignature($WebGLImp.TypeRef("Fusee.Engine.ShaderResImp"), [$.String, $.String]),
        function IRenderContextImp_CreateShader(vs, ps) {
            var vertexObject = this.gl.createShader(this.gl.VERTEX_SHADER);
            var fragmentObject = this.gl.createShader(this.gl.FRAGMENT_SHADER);

            // Compile vertex shader
            this.gl.shaderSource(vertexObject, vs);
            this.gl.compileShader(vertexObject);
            var info = this.gl.getShaderInfoLog(vertexObject);
            var statusCode = this.gl.getShaderParameter(vertexObject, this.gl.COMPILE_STATUS);

            if (statusCode != true)
                throw new Error(info);

            // Compile fragment shader
            this.gl.shaderSource(fragmentObject, ps);
            this.gl.compileShader(fragmentObject);
            info = this.gl.getShaderInfoLog(fragmentObject);
            statusCode = this.gl.getShaderParameter(fragmentObject, this.gl.COMPILE_STATUS);

            if (statusCode != true)
                throw new Error(info);

            var program = this.gl.createProgram();
            this.gl.attachShader(program, fragmentObject);
            this.gl.attachShader(program, vertexObject);

            // enable GLSL (ES) shaders to use fuVertex, fuColor and fuNormal attributes
            this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation, $fuseeCommon.Fusee.Engine.Helper.VertexAttribName);
            this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation, $fuseeCommon.Fusee.Engine.Helper.ColorAttribName);
            this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.UvAttribLocation, $fuseeCommon.Fusee.Engine.Helper.UvAttribName);
            this.gl.bindAttribLocation(program, $fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation, $fuseeCommon.Fusee.Engine.Helper.NormalAttribName);

            // Must happen AFTER the bindAttribLocation calls
            this.gl.linkProgram(program);

            var ret = new $WebGLImp.Fusee.Engine.ShaderResImp();
            ret.Program = program;
            return ret;
        }
    );

    $debug = function(log_txt) {
        if (typeof window.console != 'undefined') {
            console.log(log_txt);
        }
    }


    $.Method({ Static: false, Public: true }, "IRenderContextImp_GetShaderParam",
        new JSIL.MethodSignature($WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), [$WebGLImp.TypeRef("Fusee.Engine.IShaderResImp"), $.String]),
        function IRenderContextImp_GetShaderParam(program, paramName) {
            if (program.__ThisTypeId__ != undefined) { //i got program
                if ($fuseeFirstGetShaderParamCall) {
                    $fuseeFirstGetShaderParamCall = false;
                    var enumerator = program.Program._rci.IRenderContextImp_GetShaderParamList(this._spi).IEnumerable$b1_GetEnumerator();
                    try {
                        while (enumerator.IEnumerator_MoveNext()) {
                            var info = enumerator.IEnumerator$b1_get_Current().MemberwiseClone();
                            program.Program._paramsByName.Add(info.Name, info.Handle);
                        }
                    } finally {
                        if (enumerator !== null) {
                            enumerator.IDisposable_Dispose();
                        }
                    }
                }
                var h = this.gl.getUniformLocation(program.Program, paramName);
                if (h == null)
                    return null;
                var ret = new $WebGLImp.Fusee.Engine.ShaderParam();
                ret.handle = h;
                ret.id = this._currentShaderParamHandle++;
                return ret;
            } else { // i got program.Program
                if ($fuseeFirstGetShaderParamCall) {
                    $fuseeFirstGetShaderParamCall = false;
                    var enumerator = program._rci.IRenderContextImp_GetShaderParamList(this._spi).IEnumerable$b1_GetEnumerator();
                    try {
                        while (enumerator.IEnumerator_MoveNext()) {
                            var info = enumerator.IEnumerator$b1_get_Current().MemberwiseClone();
                            program._paramsByName.Add(info.Name, info.Handle);
                        }
                    } finally {
                        if (enumerator !== null) {
                            enumerator.IDisposable_Dispose();
                        }
                    }
                }
                var h = this.gl.getUniformLocation(program, paramName);
                if (h == null)
                    return null;
                var ret = new $WebGLImp.Fusee.Engine.ShaderParam();
                ret.handle = h;
                ret.id = this._currentShaderParamHandle++;
                return ret;
            }
            var h = this.gl.getUniformLocation(program.Program, paramName);
            if (h == null)
                return null;
            var ret = new $WebGLImp.Fusee.Engine.ShaderParam();
            ret.handle = h;
            ret.id = this._currentShaderParamHandle++;
            return ret;
        }
    );

    var $T05 = function() {
        return ($T05 = JSIL.Memoize($asm05.System.Collections.Generic.List$b1.Of($WebGLImp.Fusee.Engine.ShaderParamInfo)))();
    };

    $.Method({ Static: false, Public: true }, "IRenderContextImp_GetShaderParamList",
        new JSIL.MethodSignature($WebGLImp.TypeRef("System.Collections.Generic.IList`1", [$asm00.TypeRef("Fusee.Engine.ShaderParamInfo")]), [$asm01.TypeRef("Fusee.Engine.IShaderResImp")], []),
        function IRenderContextImp_GetShaderParamList(shaderProgram) {
            var sp = shaderProgram.Program;
            var nParams = this.gl.getProgramParameter(sp, this.gl.ACTIVE_UNIFORMS);
            var list = new($jsilcore.System.Collections.Generic.List$b1.Of($fuseeCommon.Fusee.Engine.ShaderParamInfo))();
            //var list = $sig.get(0x1928, null, [$asm05.System.Int32], []).Construct($T05(), 10);

            for (var i = 0; i < nParams; ++i) {
                var t = this.gl.getActiveUniform(sp, i).type;
                var ret = new ($fuseeCommon.Fusee.Engine.ShaderParamInfo)();

                //var activeInfo = this.gl.getActiveUniform(sp,i);
                //ret.Name = activeInfo.name;
                ret.Name = this.gl.getActiveUniform(sp, i).name;
                ret.Handle = this.IRenderContextImp_GetShaderParam(sp, ret.Name);
                switch (t) {
                case this.gl.INT:
                    ret.Type = $jsilcore.System.Int32.__Type__;
                    break;
                case this.gl.FLOAT:
                    ret.Type = $jsilcore.System.Single.__Type__;
                    break;
                case this.gl.FLOAT_VEC2:
                    ret.Type = $fuseeMath.Fusee.Math.float2.__Type__;
                    break;
                case this.gl.FLOAT_VEC3:
                    ret.Type = $fuseeMath.Fusee.Math.float3.__Type__;
                    break;
                case this.gl.FLOAT_VEC4:
                    ret.Type = $fuseeMath.Fusee.Math.float4.__Type__;
                    break;
                case this.gl.FLOAT_MAT4:
                    ret.Type = $fuseeMath.Fusee.Math.float4x4.__Type__;
                    break;
                }
                list.Add(ret.MemberwiseClone());
            }
            return list;
        }
    );


    $.Method({ Static: false, Public: true }, "IRenderContextImp_Render",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IMeshImp")]),
        function IRenderContextImp_Render(mr) {

            if (mr.VertexBufferObject != null) {
                this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation);
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.VertexBufferObject);
                this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation, 3, this.gl.FLOAT, false, 0, 0);
            }
            if (mr.ColorBufferObject != null) {
                this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation);
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.ColorBufferObject);
                this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation, 4, this.gl.FLOAT, false, 0, 0);
            }
            if (mr.NormalBufferObject != null) {
                this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation);
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.NormalBufferObject);
                this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation, 3, this.gl.FLOAT, false, 0, 0);
            }
            if (mr.UVBufferObject != null) {
                this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.UvAttribLocation);
                this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.UVBufferObject);
                this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.UvAttribLocation, 2, this.gl.FLOAT, false, 0, 0);
            }
            if (mr.ElementBufferObject != null) {
                this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, mr.ElementBufferObject);
                this.gl.drawElements(this.gl.TRIANGLES, mr.NElements, this.gl.UNSIGNED_SHORT, 0);
                //this.gl.DrawArrays(this.gl.Enums.BeginMode.POINTS, 0, shape.Vertices.Length);
            }
            if (mr.VertexBufferObject != null) {
                this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation);
            }
            if (mr.ColorBufferObject != null) {
                this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.ColorAttribLocation);
            }
            if (mr.NormalBufferObject != null) {
                this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.NormalAttribLocation);
            }
            if (mr.UVBufferObject != null) {
                this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.UvAttribLocation);
            }
        }
    );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_DebugLine",
        new JSIL.MethodSignature(null, [$asm00.TypeRef("Fusee.Math.float3"), $asm00.TypeRef("Fusee.Math.float3"), $asm00.TypeRef("Fusee.Math.float4")]),
        function IRenderContextImp_DebugLine(start, end, color) {


            var vertices = [];
            vertices.push(start.x, start.y, start.z);
            vertices.push(end.x, end.y, end.z);


            var itemSize = 3;
            var numItems = vertices.length / itemSize;
            var posBuffer = this.gl.createBuffer();            


            this.gl.enableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation);
            this.gl.bindBuffer(this.gl.ARRAY_BUFFER, posBuffer);
            this.gl.bufferData(this.gl.ARRAY_BUFFER, new Float32Array(vertices), this.gl.STATIC_DRAW);
            this.gl.vertexAttribPointer($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation, itemSize, this.gl.FLOAT, false, 0, 0);

            
            this.gl.drawArrays(this.gl.LINE_STRIP, 0, numItems);


            this.gl.disableVertexAttribArray($fuseeCommon.Fusee.Engine.Helper.VertexAttribLocation);
        }
    );


    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetShader",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderResImp")]),
        function IRenderContextImp_SetShader(program) {
            this._currentTextureUnit = 0;
            this._shaderParam2TexUnit = {};

            this.gl.useProgram(program.Program);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParam1f",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $.Single]),
        function SetShaderParam1f(param, val) {
            this.gl.uniform1f(param.handle, val);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParam2f",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $fuseeMath.TypeRef("Fusee.Math.float2")]),
        function SetShaderParam2f(param, val) {
            var flatVector = new Float32Array(val.ToArray());
            this.gl.uniform2fv(param.handle, flatVector);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParam3f",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $fuseeMath.TypeRef("Fusee.Math.float3")]),
        function SetShaderParam3f(param, val) {
            var flatVector = new Float32Array(val.ToArray());
            this.gl.uniform3fv(param.handle, flatVector);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParam4f",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $fuseeMath.TypeRef("Fusee.Math.float4")]),
        function SetShaderParam4f(param, val) {
            var flatVector = new Float32Array(val.ToArray());
            this.gl.uniform4fv(param.handle, flatVector);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParamMtx4f",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $fuseeMath.TypeRef("Fusee.Math.float4x4")]),
        function SetShaderParamMtx4f(param, val) {
            var flatMatrix = new Float32Array(val.ToArray());
            this.gl.uniformMatrix4fv(param.handle, false, flatMatrix);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParamInt",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $.Int32]),
        function SetShaderParamInt(param, val) {
            this.gl.uniform1i(param.handle, val);
        }
    );

    $.Method({ Static: false, Public: true }, "SetShaderParamTexture",
        new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IShaderParam"), $WebGLImp.TypeRef("Fusee.Engine.ITextureRes")]),
        function SetShaderParamTexture(param, texId) {
            var iParam = param.handle;
            var texUnit = -1;
            var iParamStr = param.id.toString();
            if (this._shaderParam2TexUnit.hasOwnProperty(iParamStr)) {
                texUnit = this._shaderParam2TexUnit[iParamStr];
            } else {
                texUnit = this._currentTextureUnit++;
                this._shaderParam2TexUnit[iParamStr] = texUnit;
            }

            this.gl.uniform1i(iParam, texUnit);
            this.gl.activeTexture(this.gl.TEXTURE0 + texUnit);
            this.gl.bindTexture(this.gl.TEXTURE_2D, texId.handle);

        }
    );


    // <BlendChanges>
    $.Method({ Static: false, Public: false }, "BlendOperationToOgl",
        new JSIL.MethodSignature($.Int32, [$fuseeCommon.TypeRef("Fusee.Engine.BlendOperation")]),
        function BlendOperationToOgl(bo) {
            var boVal;
            /*if ("value" in bo)
                boVal = bo.value;
            else */
                boVal = bo;
            switch (boVal) {
            case $fuseeCommon.Fusee.Engine.BlendOperation.Add.value:
                return this.gl.FUNC_ADD;
            case $fuseeCommon.Fusee.Engine.BlendOperation.Subtract.value:
                return this.gl.FUNC_SUBTRACT;
            case $fuseeCommon.Fusee.Engine.BlendOperation.ReverseSubtract.value:
                return this.gl.FUNC_REVERSE_SUBTRACT;
            case $fuseeCommon.Fusee.Engine.BlendOperation.Minimum.value:
                throw new Exception("MIN blending mode not supported in WebGL!");
            // see http://stackoverflow.com/questions/11823742/need-glblendequationext-support-in-webgl
            case $fuseeCommon.Fusee.Engine.BlendOperation.Maximum.value:
                throw new Exception("MAX blending mode not supported in WebGL!");
            default:
                throw new Exception("Unknown value for bo: " + bo);
            }
        });

    $.Method({ Static: false, Public: false }, "BlendOperationFromOgl",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.BlendOperation"), [$.Int32]),
        function BlendOperationFromOgl(bom) {
            switch (bom) {
            case this.gl.FUNC_ADD:
                return $fuseeCommon.Fusee.Engine.BlendOperation.Add;
            case this.gl.FUNC_SUBTRACT:
                return $fuseeCommon.Fusee.Engine.BlendOperation.Subtract;
            case this.gl.FUNC_REVERSE_SUBTRACT:
                return $fuseeCommon.Fusee.Engine.BlendOperation.ReverseSubtract;
            // MIN, MAX not supported by WebGL, see above
            default:
                throw new Exception("Unknown value for bom: " + bom);
            }
        });

    $.Method({ Static: false, Public: false }, "BlendToOgl",
        new JSIL.MethodSignature($.Int32, [$fuseeCommon.TypeRef("Fusee.Engine.Blend"), $.Boolean]),
        function BlendToOgl(blend, isForAlpha) {
            var blendVal;
            /* if ("value" in blend)
                blendVal = blend.value;
            else */
                blendVal = blend;
            switch (blendVal) {
            case $fuseeCommon.Fusee.Engine.Blend.Zero.value:
                return this.gl.ZERO;
            case $fuseeCommon.Fusee.Engine.Blend.One.value:
                return this.gl.ONE;
            case $fuseeCommon.Fusee.Engine.Blend.SourceColor.value:
                return this.gl.SRC_COLOR;
            case $fuseeCommon.Fusee.Engine.Blend.InverseSourceColor.value:
                return this.gl.ONE_MINUS_SRC_COLOR;
            case $fuseeCommon.Fusee.Engine.Blend.SourceAlpha.value:
                return this.gl.SRCALPHA;
            case $fuseeCommon.Fusee.Engine.Blend.InverseSourceAlpha.value:
                return this.gl.ONE_MINUS_SRC_ALPHA;
            case $fuseeCommon.Fusee.Engine.Blend.DestinationAlpha.value:
                return this.gl.DST_ALPHA;
            case $fuseeCommon.Fusee.Engine.Blend.InverseDestinationAlpha.value:
                return this.gl.ONE_MINUS_DST_ALPHA;
            case $fuseeCommon.Fusee.Engine.Blend.DestinationColor.value:
                return this.gl.DST_COLOR;
            case $fuseeCommon.Fusee.Engine.Blend.InverseDestinationColor.value:
                return this.gl.ONE_MINUS_DST_COLOR;
            case $fuseeCommon.Fusee.Engine.Blend.BlendFactor.value:
                return (isForAlpha) ? this.gl.CONSTANT_ALPHA : this.gl.CONSTANT_COLOR;
            case $fuseeCommon.Fusee.Engine.Blend.InverseBlendFactor.value:
                return (isForAlpha) ? this.gl.ONE_MINUS_CONSTANT_ALPHA : this.gl.ONE_MINUS_CONSTANT_COLOR;
            default:
                throw new Exception("Unknown value for blend: " + blend);
            }
        });

    $.Method({ Static: false, Public: false }, "BlendFromOgl",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.Blend"), [$.Int32]),
        function BlendFromOgl(bf) {
            switch (bf) {
            case this.gl.ZERO:
                return $fuseeCommon.Fusee.Engine.Blend.Zero;
            case this.gl.ONE:
                return $fuseeCommon.Fusee.Engine.Blend.One;
            case this.gl.SRC_COLOR:
                return $fuseeCommon.Fusee.Engine.Blend.SourceColor;
            case this.gl.ONE_MINUS_SRC_COLOR:
                return $fuseeCommon.Fusee.Engine.Blend.InverseSourceColor;
            case this.gl.SRCALPHA:
                return $fuseeCommon.Fusee.Engine.Blend.SourceAlpha;
            case this.gl.ONE_MINUS_SRC_ALPHA:
                return $fuseeCommon.Fusee.Engine.Blend.InverseSourceAlpha;
            case this.gl.DST_ALPHA:
                return $fuseeCommon.Fusee.Engine.Blend.DestinationAlpha;
            case this.gl.ONE_MINUS_DST_ALPHA:
                return $fuseeCommon.Fusee.Engine.Blend.InverseDestinationAlpha;
            case this.gl.DST_COLOR:
                return $fuseeCommon.Fusee.Engine.Blend.DestinationColor;
            case this.gl.ONE_MINUS_DST_COLOR:
                return $fuseeCommon.Fusee.Engine.Blend.InverseDestinationColor;
            case this.gl.CONSTANT_COLOR:
            case this.gl.CONSTANT_ALPHA:
                return $fuseeCommon.Fusee.Engine.Blend.BlendFactor;
            case this.gl.ONE_MINUS_CONSTANT_COLOR:
            case this.gl.ONE_MINUS_CONSTANT_ALPHA:
                return $fuseeCommon.Fusee.Engine.Blend.InverseBlendFactor;
            default:
                throw new Exception("Unknown value for bf: " + bf);
            }
        });


    $.Method({ Static: false, Public: true }, "SetRenderState",
        new JSIL.MethodSignature(null, [$fuseeCommon.TypeRef("Fusee.Engine.RenderState"), $.Int32]),
        function SetRenderState(renderState, value) {
            var renderStateVal;
            if ("value" in renderState)
                renderStateVal = renderState.value;
            else
                renderStateVal = renderState;
            switch (renderStateVal) {
            case $fuseeCommon.Fusee.Engine.RenderState.FillMode.value:
                {
                    if (value != $fuseeCommon.Fusee.Engine.FillMode.Solid)
                        throw new Exception("Line or Point fill mode (glPolygonMode) not supported in WebGL!");
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.CullMode.value:
                switch (value) {
                case $fuseeCommon.Fusee.Engine.Cull.None.value:
                    this.gl.disable(this.gl.CULL_FACE);
                    this.gl.frontFace(this.gl.CCW);
                    break;
                case $fuseeCommon.Fusee.Engine.Cull.Clockwise.value:
                    this.gl.enable(this.gl.CULL_FACE);
                    this.gl.frontFace(this.gl.CW);
                    break;
                case $fuseeCommon.Fusee.Engine.Cull.Counterclockwise.value:
                    this.gl.enable(this.gl.CULL_FACE);
                    this.gl.frontFace(this.gl.CCW);
                    break;
                default:
                    throw new Exception("Unknwon value for RenderState.CullMode: " + value);
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.Clipping.value:
                // clipping is always on in WebGL - This state is simply ignored
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.ZFunc.value:
                {
                    var df;
                    switch (value) {
                    case $fuseeCommon.Fusee.Engine.Compare.Never.value:
                        df = this.gl.NEVER;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.Less.value:
                        df = this.gl.LESS;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.Equal.value:
                        df = this.gl.EQUAL;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.LessEqual.value:
                        df = this.gl.LEQUAL;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.Greater.value:
                        df = this.gl.GREATER;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.NotEqual.value:
                        df = this.gl.NOTEQUAL;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.GreaterEqual.value:
                        df = this.gl.GEQUAL;
                        break;
                    case $fuseeCommon.Fusee.Engine.Compare.Always.value:
                        df = this.gl.ALWAYS;
                        break;
                    default:
                        throw new Exception("Unknwon value for RenderState.ZFunc: " + value);
                    }
                    this.gl.depthFunc(df);
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.ZEnable.value:
                if (value == 0)
                    this.gl.disable(this.gl.DEPTH_TEST);
                else
                    this.gl.enable(this.gl.DEPTH_TEST);
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.AlphaBlendEnable.value:
                if (value == 0)
                    this.gl.disable(this.gl.BLEND);
                else
                    this.gl.enable(this.gl.BLEND);
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.BlendOperation.value:
                {
                    var alphaMode = this.gl.getParameter(this.gl.BLEND_EQUATION_ALPHA);
                    this.gl.blendEquationSeparate(this.BlendOperationToOgl(value), alphaMode);
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.BlendOperationAlpha.value:
                {
                    var rgbMode = this.gl.getParameter(this.gl.BLEND_EQUATION_RGB);
                    this.gl.blendEquationSeparate(rgbMode, this.BlendOperationToOgl(value));
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.SourceBlend.value:
                {
                    var rgbDst = this.gl.getParameter(this.gl.BLEND_DST_RGB);
                    var alphaSrc = this.gl.getParameter(this.gl.BLEND_SRC_ALPHA);
                    var alphaDst = this.gl.getParameter(this.gl.BLEND_DST_ALPHA);
                    this.gl.blendFuncSeparate(this.BlendToOgl(value, false),
                        rgbDst,
                        alphaSrc,
                        alphaDst);
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.DestinationBlend.value:
                {
                    var rgbSrc = this.gl.getParameter(this.gl.BLEND_SRC_RGB);
                    var alphaSrc = this.gl.getParameter(this.gl.BLEND_SRC_ALPHA);
                    var alphaDst = this.gl.getParameter(this.gl.BLEND_DST_ALPHA);
                    this.gl.blendFuncSeparate(rgbSrc,
                        this.BlendToOgl(value, false),
                        alphaSrc,
                        alphaDst);
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.SourceBlendAlpha.value:
                {
                    var rgbSrc = this.gl.getParameter(this.gl.BLEND_SRC_RGB);
                    var rgbDst = this.gl.getParameter(this.gl.BLEND_DST_RGB);
                    var alphaDst = this.gl.getParameter(this.gl.BLEND_DST_ALPHA);
                    this.gl.blendFuncSeparate(rgbSrc,
                        rgbDst,
                        this.BlendToOgl(value, true),
                        alphaDst);
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.DestinationBlendAlpha:
                {
                    var rgbSrc = this.gl.getParameter(this.gl.BLEND_SRC_RGB);
                    var rgbDst = this.gl.getParameter(this.gl.BLEND_DST_RGB);
                    var alphaSrc = this.gl.getParameter(this.gl.BLEND_SRC_ALPHA);
                    this.gl.blendFuncSeparate(rgbSrc,
                        rgbDst,
                        alphaSrc,
                        this.BlendToOgl(value, true));
                }
                break;
            case $fuseeCommon.Fusee.Engine.RenderState.BlendFactor.value:
                var bc = $ColorUintCtor_U().Construct(value);
                var bc2 = bc.Tofloat4();
                this.gl.blendColor(bc2.x, bc2.y, bc2.z, bc2.w);
                break;
            default:
                throw new Exception("Unknown renderState to set: " + renderState);
            }
        });

    $.Method({ Static: false, Public: true }, "GetRenderState",
        new JSIL.MethodSignature($.Int32, [$.Int32]),
        function GetRenderState(renderState) {
            var renderStateVal;
            if ("value" in renderState)
                renderStateVal = renderState.value;
            else
                renderStateVal = renderState;
            switch (renderStateVal) {
            case $fuseeCommon.Fusee.Engine.RenderState.FillMode.value:
                // Only solid polygon fill is supported by WebGL
                return $fuseeCommon.Fusee.Engine.PolygonMode.Fill;
            case $fuseeCommon.Fusee.Engine.RenderState.CullMode.value:
                {
                    var cullFace = this.gl.getParameter(this.gl.CULL_FACE);
                    if (cullFace == 0)
                        return $fuseeCommon.Fusee.Engine.Cull.None;
                    var frontFace = this.gl.getParameter(this.gl.FRONT_FACE);
                    ;
                    if (frontFace == this.gl.CW)
                        return $fuseeCommon.Fusee.Engine.Cull.Clockwise;
                    return $fuseeCommon.Fusee.Engine.Counterclockwise;
                }
            case $fuseeCommon.Fusee.Engine.RenderState.Clipping.value:
                // clipping is always on in OpenGL - This state is simply ignored
                return 1;
                // == true
            case $fuseeCommon.Fusee.Engine.RenderState.ZFunc.value:
                {
                    var depFunc = this.gl.getParameter(this.gl.DEPTH_FUNC);
                    var ret;
                    switch (depFunc) {
                    case this.gl.NEVER:
                        ret = $fuseeCommon.Fusee.Engine.Compare.Never;
                        break;
                    case this.gl.LESS:
                        ret = $fuseeCommon.Fusee.Engine.Compare.Less;
                        break;
                    case this.gl.EQUAL:
                        ret = $fuseeCommon.Fusee.Engine.Compare.Equal;
                        break;
                    case this.gl.LEQUAL:
                        ret = $fuseeCommon.Fusee.Engine.Compare.LessEqual;
                        break;
                    case this.gl.GREATER:
                        ret = $fuseeCommon.Fusee.Engine.Compare.Greater;
                        break;
                    case this.gl.NOTEQUAL:
                        ret = $fuseeCommon.Fusee.Engine.Compare.NotEqual;
                        break;
                    case this.gl.GEQUAL:
                        ret = $fuseeCommon.Fusee.Engine.Compare.GreaterEqual;
                        break;
                    case this.gl.ALWAYS:
                        ret = $fuseeCommon.Fusee.Engine.Compare.Always;
                        break;
                    default:
                        throw new Exception("Value " + depFunc + " not handled as RenderState.ZFunc return.");
                    }
                    return ret;
                }
            case $fuseeCommon.Fusee.Engine.RenderState.ZEnable.value:
                return this.gl.getParameter(this.gl.DEPTH_TEST);
            case $fuseeCommon.Fusee.Engine.RenderState.AlphaBlendEnable.value:
                return this.gl.getParameter(this.gl.BLEND);
            case $fuseeCommon.Fusee.Engine.RenderState.BlendOperation.value:
                return this.BlendOperationFromOgl(this.gl.getParameter(this.gl.BLEND_EQUATION_RGB));
            case $fuseeCommon.Fusee.Engine.RenderState.BlendOperationAlpha.value:
                return this.BlendOperationFromOgl(this.gl.getParameter(this.gl.BLEND_EQUATION_ALPHA));
            case $fuseeCommon.Fusee.Engine.RenderState.SourceBlend.value:
                return this.BlendFromOgl(this.gl.getParameter(this.gl.BLEND_SRC_RGB));
            case $fuseeCommon.Fusee.Engine.RenderState.DestinationBlend.value:
                return this.BlendFromOgl(this.gl.getParameter(this.gl.BLEND_DST_RGB));
            case $fuseeCommon.Fusee.Engine.RenderState.SourceBlendAlpha.value:
                return this.BlendFromOgl(this.gl.getParameter(this.gl.BLEND_SRC_ALPHA));
            case $fuseeCommon.Fusee.Engine.RenderState.DestinationBlendAlpha.value:
                return this.BlendFromOgl(this.gl.getParameter(this.gl.BLEND_DST_ALPHA));
            case $fuseeCommon.Fusee.Engine.RenderState.BlendFactor.value:
                {
                    var col = this.gl.getParameter(this.gl.BLEND_COLOR);
                    var uintCol = $ColorUintCtor_S_S_S_S().Construct(col[0], col[1], col[2], col[3]);
                    return uintCol.ToRgba();
                }
            default:
                throw new Exception("Unknown RenderState to set a value to.");
            }
        });

    // </BlendChanges>

    $.Method({ Static: false, Public: true }, "IRenderContextImp_Viewport",
    new JSIL.MethodSignature(null, [
        $.Int32, $.Int32,
        $.Int32, $.Int32
      ]),
    function IRenderContextImp_Viewport(x, y, width, height) {
        this.gl.viewport(x, y, width, height);
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetColors",
    new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$.UInt32])]),
    function IRenderContextImp_SetColors(mr, colors) {
        if (colors == null || colors.length == 0) {
            throw new Exception("colors must not be null or empty");
        }

        var vboBytes;
        var colsBytes = colors.length * 4 * 4;
        if (mr.ColorBufferObject == null)
            mr.ColorBufferObject = this.gl.createBuffer();

        var nInts = colors.length;
        var flatBuffer = new Float32Array(colors.length * 4);
        for (var i = 0; i < colors.length; i++) {
            flatBuffer[4 * i] = (colors[i] & 0xFF) / 255.0;
            flatBuffer[4 * i + 1] = ((colors[i] >> 8) & 0xFF) / 255.0;
            flatBuffer[4 * i + 2] = ((colors[i] >> 16) & 0xFF) / 255.0;
            flatBuffer[4 * i + 3] = ((colors[i] >> 24) & 0xFF) / 255.0;


        }
        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.ColorBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != colsBytes)
            throw new Exception("Problem uploading Color buffer to VBO (Colors). Tried to upload " + ColsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetUVs",
    new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$.UInt32])]),
    function IRenderContextImp_SetUVs(mr, uvs) {
        if (uvs == null || uvs.length == 0) {
            throw new Exception("UVs must not be null or empty");
        }

        var UvBytes;
        var UvBytes = uvs.length * 2 * 4;
        if (mr.UVBufferObject == null)
            mr.UVBufferObject = this.gl.createBuffer();

        var nInts = uvs.length;
        var flatBuffer = new Float32Array(uvs.length * 2);
        for (var i = 0; i < uvs.length; i++) {
            flatBuffer[2 * i + 0] = uvs[i].x;
            flatBuffer[2 * i + 1] = uvs[i].y;
        }

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.UVBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != UvBytes)
            throw new Exception("Problem uploading UV buffer to VBO (UVs). Tried to upload " + UvBytes + " bytes, uploaded " + UvBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetNormals",
    new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$asm00.TypeRef("Fusee.Math.float3")])]),
    function IRenderContextImp_SetNormals(mr, normals) {
        if (normals == null || normals.length == 0) {
            throw new Exception("Normals must not be null or empty");
        }

        var vboBytes;
        var normsBytes = normals.length * 3 * 4;
        if (mr.NormalBufferObject == null)
            mr.NormalBufferObject = this.gl.createBuffer();

        var nFloats = normals.length * 3;
        var flatBuffer = new Float32Array(nFloats);
        for (var i = 0; i < normals.length; i++) {
            flatBuffer[3 * i + 0] = normals[i].x;
            flatBuffer[3 * i + 1] = normals[i].y;
            flatBuffer[3 * i + 2] = normals[i].z;
        }

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.NormalBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != normsBytes)
            throw new Exception("Problem uploading normal buffer to VBO (normals). Tried to upload " + normsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetTriangles",
    new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$.Int16])]),
    function IRenderContextImp_SetTriangles(mr, triangleIndices) {
        if (triangleIndices == null || triangleIndices.length == 0) {
            throw new Exception("triangleIndices must not be null or empty");
        }
        mr.NElements = triangleIndices.length;
        var vboBytes;
        var trisBytes = triangleIndices.length * 2;

        if (mr.ElementBufferObject == null)
            mr.ElementBufferObject = this.gl.createBuffer();

        var nInts = triangleIndices.length;
        var flatBuffer = new Int16Array(nInts);
        for (var i = 0; i < triangleIndices.length; i++) {
            flatBuffer[i] = triangleIndices[i];
        }

        // Upload the   index buffer (elements inside the vertex buffer, not color indices as per the IndexPointer function!)
        this.gl.bindBuffer(this.gl.ELEMENT_ARRAY_BUFFER, mr.ElementBufferObject);
        this.gl.bufferData(this.gl.ELEMENT_ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ELEMENT_ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != trisBytes)
            throw new Exception("Problem uploading vertex buffer to VBO (offsets). Tried to upload " + normsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );

    $.Method({ Static: false, Public: true }, "IRenderContextImp_SetVertices",
    new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.IMeshImp"), $jsilcore.TypeRef("System.Array", [$asm00.TypeRef("Fusee.Math.float3")])]),
    function IRenderContextImp_SetVertices(mr, vertices) {
        if (vertices == null || vertices.length == 0) {
            throw new Exception("vertices must not be null or empty");
        }

        var vboBytes;
        var vertsBytes = vertices.length * 3 * 4;
        if (mr.VertexBufferObject == null)
            mr.VertexBufferObject = this.gl.createBuffer();

        var nFloats = vertices.length * 3;
        var flatBuffer = new Float32Array(nFloats);
        for (var i = 0; i < vertices.length; i++) {
            flatBuffer[3 * i + 0] = vertices[i].x;
            flatBuffer[3 * i + 1] = vertices[i].y;
            flatBuffer[3 * i + 2] = vertices[i].z;
        }

        this.gl.bindBuffer(this.gl.ARRAY_BUFFER, mr.VertexBufferObject);
        this.gl.bufferData(this.gl.ARRAY_BUFFER, flatBuffer, this.gl.STATIC_DRAW);
        vboBytes = this.gl.getBufferParameter(this.gl.ARRAY_BUFFER, this.gl.BUFFER_SIZE);
        if (vboBytes != vertsBytes)
            throw new Exception("Problem uploading normal buffer to VBO (vertices). Tried to upload " + vertsBytes + " bytes, uploaded " + vboBytes + ".");
    }
  );


    $.Method({ Static: false, Public: true }, "IRenderContextImp_CreateMeshImp",
    new JSIL.MethodSignature($WebGLImp.TypeRef("Fusee.Engine.IMeshImp"), []),
    function IRenderContextImp_CreateMeshImp() {
        return new $WebGLImp.Fusee.Engine.MeshImp();
    }
  );


});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.MeshImp", true, [], function ($) {
   $.ImplementInterfaces($fuseeCommon.TypeRef("Fusee.Engine.IMeshImp"));

    $.Field({ Static: false, Public: true }, "VertexBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "NormalBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "UVBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "ColorBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "ElementBufferObject",$.Object, null);
    $.Field({ Static: false, Public: true }, "NElements", $.Int32, null);

    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, []),
    function _ctor() {
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_ColorsSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_ColorsSet() {
        return this.ColorBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_NormalsSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_NormalsSet() {
        return this.NormalBufferObject != null;
    }
  );
    
    $.Method({ Static: false, Public: true }, "IMeshImp_get_UVsSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_UVsSet() {
        return this.UVBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_TrianglesSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_TrianglesSet() {
        return this.ElementBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "IMeshImp_get_VerticesSet",
    new JSIL.MethodSignature($.Boolean, []),
    function get_VerticesSet() {
        return this.VertexBufferObject != null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateColors",
    new JSIL.MethodSignature(null, []),
    function InvalidateColors() {
        this.ColorBufferObject = null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateNormals",
    new JSIL.MethodSignature(null, []),
    function InvalidateNormals() {
        this.NormalBufferObject = null;
    }
  );
    
     $.Method({ Static: false, Public: true }, "InvalidateUVs",
    new JSIL.MethodSignature(null, []),
    function InvalidateUVs() {
        this.UVBufferObject = null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateTriangles",
    new JSIL.MethodSignature(null, []),
    function InvalidateTriangles() {
        this.ElementBufferObject = null;
        this.NElements = null;
    }
  );

    $.Method({ Static: false, Public: true }, "InvalidateVertices",
    new JSIL.MethodSignature(null, []),
    function InvalidateVertices() {
        this.VertexBufferObject = null;
    }
  );

    $.Property({ Static: false, Public: true }, "ColorsSet");

    $.Property({ Static: false, Public: true }, "NormalsSet");
    
    $.Property({ Static: false, Public: true }, "UVsSet");

    $.Property({ Static: false, Public: true }, "TrianglesSet");

    $.Property({ Static: false, Public: true }, "VerticesSet");

});

JSIL.MakeClass($jsilcore.TypeRef("System.Object"), "Fusee.Engine.InputImp", true, [], function ($) {

    /*
    $.Field({Static:false, Public:true}, "_currentMMouse", $fuseeCommon.TypeRef("Fusee.Engine.Point"), null);
    $.Field({Static:false, Public:true}, "NElements",  $.Int32, null);
    protected GameWindow _gameWindow;
    */
    $.Field({ Static: false, Public: true }, "_currentMouse", $fuseeCommon.TypeRef("Fusee.Engine.Point"), null);
    $.Field({ Static: false, Public: true }, "_mouseDown", $.Boolean, null);
    $.Field({ Static: false, Public: true }, "_currentMouseWheel", $.Int32, null);

    $.Method({ Static: false, Public: true }, "OnCanvasMouseDown",
        new JSIL.MethodSignature(null, []),
        function OnCanvasMouseDown(event) {
            var mb = $fuseeCommon.Fusee.Engine.MouseButtons.Unknown;
            switch (event.button) {
                case 0:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Left;
                    break;
                case 1:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Middle;
                    break;
                case 2:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Right;
                    break;
            }

            if (this.IInputImp_MouseButtonDown !== null) {
				var pt = new $fuseeCommon.Fusee.Engine.Point().__Initialize__({x: event.clientX, y: event.clientY});
                this.IInputImp_MouseButtonDown(this, (new $fuseeCommon.Fusee.Engine.MouseEventArgs()).__Initialize__({
                    Button: mb,
                    Position: pt
                }));
            }
        }
    );

    $.Method({ Static: false, Public: true }, "OnCanvasMouseUp",
        new JSIL.MethodSignature(null, []),
        function OnCanvasMouseUp(event) {
            var mb = $fuseeCommon.Fusee.Engine.MouseButtons.Unknown;
            switch (event.button) {
                case 0:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Left;
                    break;
                case 1:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Middle;
                    break;
                case 2:
                    mb = $fuseeCommon.Fusee.Engine.MouseButtons.Right;
                    break;
            }

            if (this.IInputImp_MouseButtonUp !== null) {
				var pt = new $fuseeCommon.Fusee.Engine.Point().__Initialize__({x: event.clientX, y: event.clientY});
                this.IInputImp_MouseButtonUp(this, (new $fuseeCommon.Fusee.Engine.MouseEventArgs()).__Initialize__({
                    Button: mb,
                    Position: pt
                }));
            }
        }
    );


    $.Method({ Static: false, Public: true }, "OnCanvasKeyDown",
        new JSIL.MethodSignature(null, []),
        function OnCanvasKeyDown(event) {
            if (this.IInputImp_KeyDown !== null) {
                this.IInputImp_KeyDown(this, (new $fuseeCommon.Fusee.Engine.KeyEventArgs()).__Initialize__({
                    Shift: event.shiftKey,
                    Alt: event.altKey,
                    Control: event.ctrlKey,
                    KeyCode: event.keyCode
                }));
            }
        }
    );

    $.Method({ Static: false, Public: true }, "OnCanvasKeyUp",
        new JSIL.MethodSignature(null, []),
        function OnCanvasKeyUp(event) {
            if (this.IInputImp_KeyUp !== null) {
                this.IInputImp_KeyUp(this, (new $fuseeCommon.Fusee.Engine.KeyEventArgs()).__Initialize__({
                    Shift: event.shiftKey,
                    Alt: event.altKey,
                    Control: event.ctrlKey,
                    KeyCode: event.keyCode
                }));
            }
        }
    );


    $.Method({ Static: false, Public: true }, "OnCanvasMouseMove",
      new JSIL.MethodSignature(null, []),
      function OnCanvasMouseMove(event) {
          this._currentMouse.x = event.clientX;
          this._currentMouse.y = event.clientY;
      }
    );

    $.Method({ Static: false, Public: true }, "OnCanvasMouseWheel",
      new JSIL.MethodSignature(null, []),
      function OnCanvasMouseWheel(event) {
          this._currentMouseWheel += event.wheelDelta;
      }
    );

    // IInputImp implementation
    $.Method({ Static: false, Public: true }, "IInputImp_GetMouseWheelPos",
        new JSIL.MethodSignature($.Int32, []),
            function IInputImp_GetMouseWheelPos(event) {
                return this._currentMouseWheel;
            }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_GetMousePos",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.Point"), []),
            function IInputImp_GetMousePos(event) {
                return this._currentMouse.MemberwiseClone();
            }
    );

    // KeyDown event
    $.Field({ Static: false, Public: false }, "IInputImp_KeyDown", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_KeyDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_add_KeyDown(value) {
            var eventHandler = this.IInputImp_KeyDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_KeyDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_remove_KeyDown(value) {
            var eventHandler = this.IInputImp_KeyDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    // KeyUp event
    $.Field({ Static: false, Public: false }, "IInputImp_KeyUp", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_KeyUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_add_KeyUp(value) {
            var eventHandler = this.IInputImp_KeyUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_KeyUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.KeyEventArgs")])]),
        function IInputImp_remove_KeyUp(value) {
            var eventHandler = this.IInputImp_KeyUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.KeyEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_KeyUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );


    // MouseButtonDown event
    $.Field({ Static: false, Public: false }, "IInputImp_MouseButtonDown", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_MouseButtonDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_add_MouseButtonDown(value) {
            var eventHandler = this.IInputImp_MouseButtonDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_MouseButtonDown",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_remove_MouseButtonDown(value) {
            var eventHandler = this.IInputImp_MouseButtonDown;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonDown"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    // MouseButtonUp event
    $.Field({ Static: false, Public: false }, "IInputImp_MouseButtonUp", $jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")]), function ($) {
        return null;
    });

    $.Method({ Static: false, Public: true }, "IInputImp_add_MouseButtonUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_add_MouseButtonUp(value) {
            var eventHandler = this.IInputImp_MouseButtonUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Combine(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );

    $.Method({ Static: false, Public: true }, "IInputImp_remove_MouseButtonUp",
        new JSIL.MethodSignature(null, [$jsilcore.TypeRef("System.EventHandler`1", [$fuseeCommon.TypeRef("Fusee.Engine.MouseEventArgs")])]),
        function IInputImp_remove_MouseButtonUp(value) {
            var eventHandler = this.IInputImp_MouseButtonUp;
            do {
                var eventHandler2 = eventHandler;
                var value2 = $jsilcore.System.Delegate.Remove(eventHandler2, value);
                eventHandler = $jsilcore.System.Threading.Interlocked.CompareExchange$b1($jsilcore.System.EventHandler$b1.Of($fuseeCommon.Fusee.Engine.MouseEventArgs))(/* ref */new JSIL.MemberReference(this, "IInputImp_MouseButtonUp"), value2, eventHandler2);
            } while (eventHandler !== eventHandler2);
        }
    );




    $.Method({ Static: false, Public: true }, ".ctor",
    new JSIL.MethodSignature(null, [$WebGLImp.TypeRef("Fusee.Engine.RenderCanvasImp")]),
    function _ctor(renderCanvas) {

        if (renderCanvas == null)
            throw new Exception("renderCanvas must not be null");

        this._currentMouse = new $fuseeCommon.Fusee.Engine.Point();
        this._currentMouse.x = 0;
        this._currentMouse.y = 0;
        var callbackClosure = this;
        renderCanvas.theCanvas.onmousedown = function (event) {
            callbackClosure.OnCanvasMouseDown.call(callbackClosure, event);
        };
        renderCanvas.theCanvas.onmouseup = function (event) {
            callbackClosure.OnCanvasMouseUp.call(callbackClosure, event);
        };
        renderCanvas.theCanvas.onmousemove = function (event) {
            callbackClosure.OnCanvasMouseMove.call(callbackClosure, event);
        };
        renderCanvas.theCanvas.onmousewheel = function (event) {
            callbackClosure.OnCanvasMouseWheel.call(callbackClosure, event);
        };
        document.onkeydown = function (event) {
            callbackClosure.OnCanvasKeyDown.call(callbackClosure, event);
        };
        document.onkeyup = function (event) {
            callbackClosure.OnCanvasKeyUp.call(callbackClosure, event);
        };
    });
});

JSIL.ImplementExternals("Fusee.Engine.ImpFactory", function ($) {
    $.Method({ Static: true, Public: true }, "CreateIInputImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IInputImp"), [$fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp")]),
            function ImpFactory_CreateIInputImp(renderCanvasImp) {
                return new $WebGLImp.Fusee.Engine.InputImp(renderCanvasImp);
            }
    );

    $.Method({ Static: true, Public: true }, "CreateIRenderCanvasImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp"), []),
            function ImpFactory_CreateIRenderCanvasImp() {
				// return new $WebGLImp.Fusee.Engine.TheEmptyDummyClass
                return new $WebGLImp.Fusee.Engine.RenderCanvasImp();
            }
    );

    $.Method({ Static: true, Public: true }, "CreateIRenderContextImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IRenderContextImp"), [$fuseeCommon.TypeRef("Fusee.Engine.IRenderCanvasImp")]),
            function ImpFactory_CreateIRenderContextImp(renderCanvasImp) {
                return new $WebGLImp.Fusee.Engine.RenderContextImp(renderCanvasImp);
            }
    );
	
	$.Method({ Static: true, Public: true }, "CreateIAudioImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IAudioImp"), []),
            function ImpFactory_CreateIAudioImp() {
                return new $WebAudioImp.Fusee.Engine.WebAudioImp();
            }
    );

	$.Method({ Static: true, Public: true }, "CreateINetworkImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.INetworkImp"), []),
            function ImpFactory_CreateINetworkImp() {
                return new $WebNetImp.Fusee.Engine.WebNetImp();
            }
    );

	$.Method({ Static: true, Public: true }, "CreateIImagehelperImp",
        new JSIL.MethodSignature($fuseeCommon.TypeRef("Fusee.Engine.IImagehelperImp"), []),
            function ImpFactory_CreateIImagehelperImp() {
                return new $WebGLImp.Fusee.Engine.ImagehelperImp();
            }
    );

});


JSIL.ImplementExternals("Fusee.Engine.MeshReader", function ($) {
    $.Method({ Static: true, Public: true }, "Double_Parse",
        new JSIL.MethodSignature($.Double, [$.String]),
            function Double_Parse(str) {
                return Number(str);
            }
    );
});

/**
* Provides requestAnimationFrame in a cross browser way.
*/
window.requestAnimFrame = (function () {
    return window.requestAnimationFrame ||
         window.webkitRequestAnimationFrame ||
         window.mozRequestAnimationFrame ||
         window.oRequestAnimationFrame ||
         window.msRequestAnimationFrame ||
         function (/* function FrameRequestCallback */callback, /* DOMElement Element */element) {
             return window.setTimeout(callback, 1000 / 60);
         };
})();

/**
* Provides cancelAnimationFrame in a cross browser way.
*/
window.cancelAnimFrame = (function () {
    return window.cancelAnimationFrame ||
         window.webkitCancelAnimationFrame ||
         window.mozCancelAnimationFrame ||
         window.oCancelAnimationFrame ||
         window.msCancelAnimationFrame ||
         window.clearTimeout;
})();