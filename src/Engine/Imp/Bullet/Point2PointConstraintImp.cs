﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusee.Math;
using BulletSharp;

namespace Fusee.Engine
{
    public class Point2PointConstraintImp : IPoint2PointConstraintImp
    {
        internal Point2PointConstraint _p2pci;

        public float3 PivotInA
        {
            get
            {
                var retval = new float3(_p2pci.PivotInA.X, _p2pci.PivotInA.Y, _p2pci.PivotInA.Z);
                return retval;
            }
            set
            {
                var pivoA = new Vector3(value.x, value.y, value.z);
                var o = (Point2PointConstraintImp)_p2pci.UserObject;
                _p2pci.PivotInA = pivoA;
            }
        }

        public float3 PivotInB
        {
            get
            {
                var retval = new float3(_p2pci.PivotInB.X, _p2pci.PivotInB.Y, _p2pci.PivotInB.Z);
                return retval;
            }
            set
            {
                var pivoB = new Vector3(value.x, value.y, value.z);
                var o = (Point2PointConstraintImp)_p2pci.UserObject;
                o._p2pci.PivotInB = pivoB;
            }
            
        }

        public void SetParam(int num, float3 value, int axis = -1)
        {
        }

        public float3 GetParam(int num, int axis = -1)
        {
            var retval = new float3(0,0,0);
            return retval;
        }

        public IRigidBodyImp RigidBodyA
        {
            get
            {
                var retval = _p2pci.RigidBodyA;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        public IRigidBodyImp RigidBodyB
        {
            get
            {
                var retval = _p2pci.RigidBodyB;
                return (RigidBodyImp)retval.UserObject;
            }
        }

        private object _userObject;
        public object UserObject
        {
            get { return _userObject; }
            set { _userObject = value; }
        }
    }
}
