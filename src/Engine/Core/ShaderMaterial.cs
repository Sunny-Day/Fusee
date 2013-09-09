﻿using System;
using Fusee.Math;
using Fusee.Engine;
using System.Collections.Generic;

namespace Fusee.Engine
{
    public class ShaderMaterial
    {
        private ShaderRes _sp;
        //private Dictionary<string, dynamic> _list;

        public ShaderMaterial(ShaderRes res)
        {
            _sp = res;
            //_list = new Dictionary<string, dynamic>();
            //foreach (KeyValuePair<string, ShaderParamInfo> k in _sp._paramsByName)
            //{
            //    _list.Add(k.Key, _sp._rci.GetParamValue(res._spi, k.Value.Handle));
            //}
        }

//        public void SetValue(string name, dynamic value)
//        {
//            ShaderParamInfo info;
//            if (_sp._paramsByName.TryGetValue(name, out info))
//                _sp._rci.SetShaderParam(info.Handle, value);
//            if (_list.ContainsKey(name))
//                _list[name] = value;
//        }

        public ShaderRes GetShader()
        {
            return _sp;
        }

        public void UpdateMaterial(RenderContext rc)
        {
            rc.SetShader(_sp);
        }
    }   
}
