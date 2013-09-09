﻿using System.Collections.Generic;

namespace Fusee.Engine
{
    /// <summary>
    /// Instances of this class represent a pair of a Vertex and a Pixel shader code, both compiled and 
    /// uploaded to the gpu ready to be used. 
    /// </summary>
    /// <remarks>See <see cref="RenderContext.CreateShader"/> how to create instances and 
    /// <see cref="RenderContext.SetShader"/> how to use instances as the current shaders.</remarks>
    public class ShaderRes
    {
        internal IShaderResImp _spi;
        internal IRenderContextImp _rci;
        internal Dictionary<string, IShaderParam> _paramsByName;

        internal ShaderRes(IRenderContextImp renderContextImp, IShaderResImp shaderResImp)
        {
            _spi = shaderResImp;
            _rci = renderContextImp;
            _paramsByName = new Dictionary<string, IShaderParam>();
            foreach (ShaderParamInfo info in _rci.GetShaderParamList(_spi))
            {
                _paramsByName.Add(info.Name, info.Handle);
            }
            //_paramsByName = new Dictionary<string, ShaderParamInfo>();
            //foreach (ShaderParamInfo info in _rci.GetShaderParamList(_spi))
            //{
            //    ShaderParamInfo newInfo = new ShaderParamInfo()
            //                                  {
            //                                      Handle = info.Handle,
            //                                      Name = info.Name,
            //                                      Type = info.Type,
            //                                      Size = info.Size,
            //                                  };
            //    _paramsByName.Add(info.Name, newInfo);
            //}
        }
        
        //public IShaderParam GetShaderParam(string paramName)
        //{
        //    ShaderParamInfo ret;
        //    if (_paramsByName.TryGetValue(paramName, out ret))
        //        return ret.Handle;
        //    //ret = _rci.GetShaderParam(_spi, paramName);
        //    //if (ret != null)
        //        _paramsByName[paramName] = ret;
        //    return ret.Handle;
        //}

        public IShaderParam GetShaderParam(string paramName)
        {
            IShaderParam ret;
            if (_paramsByName.TryGetValue(paramName, out ret))
                return ret;
            ret = _rci.GetShaderParam(_spi, paramName);
            if (ret != null)
                _paramsByName[paramName] = ret;
            return ret;
        }

        

        // TODO: add SetParameter methods here (remove from render context).
    }
}
