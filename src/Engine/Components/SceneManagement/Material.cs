using System;
using System.Collections.Generic;

using Fusee.Engine;
namespace Fusee.SceneManagement
{
    public class Material
    {
        protected ImageData imgData;
        protected ImageData imgData2;
        protected ITextureRes iTex;
        protected ITextureRes iTex2;
        protected IShaderParam _vColorParam;
        protected IShaderParam _texture1Param;

        public ShaderRes sp;

        public Material()
        {
            
        }
        public Material(ShaderRes res)
        {
            sp = res;
        }

        virtual public void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
        }
    }
}
