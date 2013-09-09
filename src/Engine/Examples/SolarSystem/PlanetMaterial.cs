using Fusee.Engine;
using Fusee.SceneManagement;

namespace Examples.SolarSystem
{
    public class PlanetMaterial : Material
    {
        public IShaderParam TextureParam;
        public ITextureRes Tex;

        public PlanetMaterial(ShaderRes shaderRes)
        {
            sp = shaderRes;
        }

        public PlanetMaterial(ShaderRes shaderRes, string texturePath)
        {
            sp = shaderRes;

            TextureParam = sp.GetShaderParam("texture1");

            var image = SceneManager.RC.LoadImage(texturePath);
            Tex = SceneManager.RC.CreateTexture(image);
        }

        public override void Update(RenderContext renderContext)
        {
            renderContext.SetShader(sp);
            renderContext.SetShaderParamTexture(TextureParam, Tex);
        }
    }
}