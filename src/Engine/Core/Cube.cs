using Fusee.Math;
using JSIL.Runtime;

namespace Fusee.Engine
{
    /// <summary>
    /// Creates a simple cube geomentry straight from the code.
    /// </summary>
    public class Cube : Mesh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube" /> class.
        /// Cube is a derivate of the <see cref="Mesh" /> class.
        /// The default cube is 1 unit big and contains various default vertex colors.
        /// The vertex colors are only visible during rendering when a vertexcolor shader is applied on the Mesh.
        /// </summary>
        public Cube()
        {
            #region Fields

            // Vertices
            var verts = new[]
            {
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = -0.5f},
                new float3 {x = -0.5f, y = +0.5f, z = +0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = -0.5f},
                new float3 {x = +0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = +0.5f},
                new float3 {x = -0.5f, y = -0.5f, z = -0.5f}
            };

            Vertices = PackedArray.New<float3>(verts.Length);
            for (int i = 0; i < verts.Length; i++)
                Vertices[i] = verts[i];

            // Triangles
            Triangles = new short[]
            {
                // front face
                0, 1, 2, 0, 2, 3,

                // right face
                4, 5, 6, 4, 6, 7,
                
                // back face
                8, 9, 10, 8, 10, 11,
               
                // left face
                12, 13, 14, 12, 14, 15,
                
                // top face
                16, 17, 18, 16, 18, 19,

                // bottom face
                20, 21, 22, 20, 22, 23
            };

            // Normals
            var norms = new[]
            {
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(0, 0, 1),
                new float3(1, 0, 0),
                new float3(1, 0, 0),
                new float3(1, 0, 0),
                new float3(1, 0, 0),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(0, 0, -1),
                new float3(-1, 0, 0),
                new float3(-1, 0, 0),
                new float3(-1, 0, 0),
                new float3(-1, 0, 0),
                new float3(0, 1, 0),
                new float3(0, 1, 0),
                new float3(0, 1, 0),
                new float3(0, 1, 0),
                new float3(0, -1, 0),
                new float3(0, -1, 0),
                new float3(0, -1, 0),
                new float3(0, -1, 0)
            };

            Normals = PackedArray.New<float3>(norms.Length);
            for (int i = 0; i < norms.Length; i++)
                Normals[i] = norms[i];

            // UVs
            var tex = new[]
            {
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0),
                new float2(1, 0),
                new float2(1, 1),
                new float2(0, 1),
                new float2(0, 0)
            };

            UVs = PackedArray.New<float2>(tex.Length);
            for (int i = 0; i < tex.Length; i++)
                UVs[i] = tex[i];
        }

        #endregion
    }
}