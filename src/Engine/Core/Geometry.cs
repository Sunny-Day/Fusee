using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Math;
using JSIL.Meta;
using JSIL.Runtime;

namespace Fusee.Engine
{
    /// <summary>
    /// Stores threedimensional, polygonal geometry and provides methods for manipulation.
    /// To actually render the geometry in the engine, convert Geometry to <see cref="Mesh"/> objects.
    /// </summary>
    public class Geometry
    {
        #region Nested Class
        /// <summary>
        /// A container that stores indices for vertices, normals and texture coordinates.
        /// The values are used for conversion to different geometry face formats, e.g. Triangles.
        /// </summary>
        public struct Face
        {
            /// <summary>
            /// The inx vert
            /// </summary>
            public int[] InxVert;
            /// <summary>
            /// The inx normal
            /// </summary>
            public int[] InxNormal;
            /// <summary>
            /// The inx tex coord
            /// </summary>
            public int[] InxTexCoord;
        }
        #endregion

        #region Fields

        internal List<double3> _vertices;

        /// <summary>
        /// The list of vertices (3D positions).
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public IList<double3> Vertices
        {
            set { _vertices = new List<double3>(value); }
            get { return _vertices; }
        }

        internal List<double3> _normals;
        /// <summary>
        /// Gets or sets the normals.
        /// </summary>
        /// <value>
        /// The normals.
        /// </value>
        public IList<double3> Normals
        {
            set { _normals = new List<double3>(value); }
            get { return _normals; }
        }

        internal List<double2> _texCoords;
        /// <summary>
        /// Gets or sets the texture coordinates.
        /// </summary>
        /// <value>
        /// The texture coordinates.
        /// </value>
        public IList<double2> TexCoords
        {
            set { _texCoords = new List<double2>(value); }
            get { return _texCoords; }
        }

        internal List<Face> _faces;
        /// <summary>
        /// Gets or sets the faces.
        /// </summary>
        /// <value>
        /// The faces.
        /// </value>
        public IList<Face> Faces
        {
            set { _faces = new List<Face>(value); }
            get { return _faces; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Geometry"/> class.
        /// </summary>
        public Geometry()
        {
            _vertices = new List<double3>();
        }

        #endregion

        #region Members

        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="v">A 3D vector.</param>
        /// <returns>The current vertex count.</returns>
        public int AddVertex(double3 v)
        {
            _vertices.Add(v);
            return _vertices.Count - 1;
        }

        /// <summary>
        /// Adds the texture coordinates.
        /// </summary>
        /// <param name="uv">Texture coordinate</param>
        /// <returns>The count of <see cref="TexCoords"/>.</returns>
        public int AddTexCoord(double2 uv)
        {
            if (_texCoords == null)
                _texCoords = new List<double2>();
            _texCoords.Add(uv);
            return _texCoords.Count - 1;
        }

        /// <summary>
        /// Adds the normal.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <returns>The count of <see cref="Normals"/>.</returns>
        public int AddNormal(double3 normal)
        {
            if (_normals == null)
                _normals = new List<double3>();
            _normals.Add(normal);
            return _normals.Count - 1;
        }

        /// <summary>
        /// Adds the face.
        /// </summary>
        /// <param name="vertInx">The vert inx.</param>
        /// <param name="texCoordInx">The tex coord inx.</param>
        /// <param name="normalInx">The normal inx.</param>
        /// <returns>The face count as integer value.</returns>
        /// <exception cref="System.ArgumentNullException">vertInx</exception>
        /// <exception cref="System.ArgumentException">
        /// "Vertex index out of range: vertInx[i]"
        /// or
        /// "Number of texture coordinate indices must match number of vertex indices"
        /// or
        /// "Texture coordinate index out of range: texCoordInx[i]"
        /// or
        /// "Number of normal indices must match number of vertex indices"
        /// or
        /// "Normal index out of range: normalInx[i]"
        /// </exception>
        public int AddFace(int[] vertInx, int[] texCoordInx, int[] normalInx)
        {
            int i;
            var f = new Face();
            
            // Plausibility checks interleaved...
            if (vertInx == null)
                throw new ArgumentNullException("vertInx");

            f.InxVert = new int[vertInx.Length];
            for(i = 0; i < vertInx.Length; i++)
            {
                var vInx = vertInx[i];

                if (!(0 <= vInx && vInx < _vertices.Count))
                    throw new ArgumentException("Vertex index out of range: " + vInx, "vertInx[" + i + "]");

                f.InxVert[i] = vInx;
            }

            if (texCoordInx != null)
            {
                if (texCoordInx.Length != vertInx.Length)
                    throw new ArgumentException(
                        "Number of texture coordinate indices must match number of vertex indices", "texCoordInx");

                f.InxTexCoord = new int[texCoordInx.Length];
                for (i = 0; i < texCoordInx.Length; i++)
                {
                    var tInx = texCoordInx[i];

                    if (!(0 <= tInx && tInx < _texCoords.Count))
                        throw new ArgumentException("Texture coordinate index out of range: " + tInx, "texCoordInx[" + i + "]");

                    f.InxTexCoord[i] = tInx;
                }
            }

            if (normalInx != null)
            {
                if (normalInx.Length != vertInx.Length)
                    throw new ArgumentException("Number of normal indices must match number of vertex indices",
                                                "normalInx");

                f.InxNormal = new int[normalInx.Length];
                for (i = 0; i < normalInx.Length; i++)
                {
                    var nInx = normalInx[i];

                    if (!(0 <= nInx && nInx < _normals.Count))
                        throw new ArgumentException("Normal index out of range: " + nInx, "normalInx[" + i + "]");

                    f.InxNormal[i] = nInx;
                }
            }

            // Actually add the faces
            if (_faces == null)
                _faces = new List<Face>();

            _faces.Add(f);
            return _faces.Count - 1;
        }

        #endregion

        #region Boolean Check Fields

        /// <summary>
        /// Gets a value indicating whether this instance has normals.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has normals; otherwise, <c>false</c>.
        /// </value>
        public bool HasNormals
        {
            get { return _normals != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has tex coords.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has texture coordinates; otherwise, <c>false</c>.
        /// </value>
        public bool HasTexCoords
        {
            get { return _texCoords != null; }
        }

        #endregion

        #region Geometry Calculation Members

        /// <summary>
        /// Gets all faces containing a certain vertex.
        /// </summary>
        /// <param name="packedFaces"></param>
        /// <param name="iV">The index of the vertex.</param>
        /// <param name="vertInFace">Out parameter: A list of indices of the vertex in each respecitve face.</param>
        /// <returns>A list of indices containing the vertex.</returns>
        public IList<int> GetAllFacesContainingVertex(Face[] packedFaces, int iV, out IList<int> vertInFace)
        {
            List<int> ret = new List<int>();
            vertInFace = new List<int>();

            for (int iF = 0; iF < _faces.Count; iF++)
            {
                var inxVert = packedFaces[iF].InxVert;

                for (int iFV = 0; iFV < inxVert.Length; iFV++)
                {
                    if (iV == inxVert[iFV])
                    {
                        ret.Add(iF);
                        vertInFace.Add(iFV);
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Calculates the normal vector for a given face.
        /// </summary>
        /// <param name="f">The face to calculate the normal for.</param>
        /// <returns>The nomal vector for the face.</returns>
        /// <exception cref="System.Exception">The face doesn't consist of 3 or more vertices.</exception>
        public double3 CalcFaceNormal(Face f)
        {
            if (f.InxVert.Length < 3)
                throw new Exception("Cannot calculate normal of degenerate face with only " + f.InxVert.Length + " vertices.");

            var vertex0 = _vertices[f.InxVert[0]];
            double3 v1 = vertex0 - _vertices[f.InxVert[1]];
            double3 v2 = vertex0 - _vertices[f.InxVert[2]];

            return double3.Normalize(double3.Cross(v1, v2));
        }

        /// <summary>
        /// Creates normals for the entire geometry based on a given smoothing angle.
        /// </summary>
        /// <param name="smoothingAngle">The smoothing angle.</param>
        public void CreateNormals(double smoothingAngle)
        {
            double cSmoothingAngle = System.Math.Cos(smoothingAngle);
            var packedFaces = _faces.ToArray();

            for (int iV = 0; iV < _vertices.Count; iV++)
            {
                IList<int> vertInFace;
                IList<int> facesWithIV = GetAllFacesContainingVertex(packedFaces, iV, out vertInFace);

                var packedNormals = new double3[facesWithIV.Count];
                for (int i = 0; i < facesWithIV.Count; i++)
                    packedNormals[i] = CalcFaceNormal(packedFaces[facesWithIV[i]]);

                // Quick and dirty solution: if the smoothing angle holds for all combinations we create a shared normal,
                // otherwise we create individual normals for each face. 
                // TODO: Build groups of shared normmals where faces are connected by edges (need edges to do this)

                bool smoothIt = true;
                for (int i = 0; i < packedNormals.Length; i++)
                {
                    var packedNormalsI = packedNormals[i];

                    for (int j = i + 1; j < packedNormals.Length; j++)
                    {
                        if (double3.Dot(packedNormalsI, packedNormals[j]) < cSmoothingAngle)
                        {
                            smoothIt = false;
                            break;
                        }
                    }

                    if (!smoothIt)
                        break;
                }

                if (smoothIt)
                {
                    // create a single normal and set each face to it
                    var daNormal = new double3 { x = 0, y = 0, z = 0 };

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var n in packedNormals)
                        daNormal += n;
                    
                    daNormal /= (double)packedNormals.Length;

                    int iN = AddNormal(daNormal);
                    for (int i = 0; i < facesWithIV.Count; i++)
                    {
                        var curFacesWithIV = facesWithIV[i];
                        var curFace = packedFaces[curFacesWithIV];

                        if (curFace.InxNormal == null)
                            packedFaces[curFacesWithIV].InxNormal = new int[curFace.InxVert.Length];

                        packedFaces[curFacesWithIV].InxNormal[vertInFace[i]] = iN;
                    }
                }
                else
                {
                    // create individual normals and assign to respective face vertices
                    for (int i = 0; i < packedNormals.Length; i++)
                    {
                        var curFacesWithIV = facesWithIV[i];
                        var curFace = packedFaces[curFacesWithIV];

                        int iN = AddNormal(packedNormals[i]);

                        if (curFace.InxNormal == null)
                            packedFaces[curFacesWithIV].InxNormal = new int[curFace.InxVert.Length];

                        packedFaces[curFacesWithIV].InxNormal[vertInFace[i]] = iN;
                    }
                }
            }

            _faces = packedFaces.ToList();
        }


        #region Structs

        internal struct TripleInx
        {
            /// <summary>
            /// The i V
            /// </summary>
            public int iV, iT, iN;
            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                return iV ^ iT ^ iN;
            }
        }

        #endregion

        /// <summary>
        /// Converts the whole geomentry to a <see cref="Mesh"/>.
        /// </summary>
        /// <returns>An equivalent instance of <see cref="Mesh"/>.</returns>
        public Mesh ToMesh()
        {
            // TODO: make a big case decision based on HasTexCoords and HasNormals around the implementation and implement each case individually
            var vDict = new Dictionary<TripleInx, int>();
            var vKeys = new List<TripleInx>();
            var mTris = new List<short>();

            foreach (Face f in _faces)
            {
                var mFace = new int[f.InxVert.Length];
                for (int i = 0; i < f.InxVert.Length; i++)
                {
                    var ti = new TripleInx
                    {
                        iV = f.InxVert[i],
                        iT = (HasTexCoords) ? f.InxTexCoord[i] : 0,
                        iN = (HasNormals) ? f.InxNormal[i] : 0
                    };

                    int inx;
                    if (!vDict.TryGetValue(ti, out inx))
                    {
                        inx = vDict.Count;
                        vDict.Add(ti, inx);
                        vKeys.Add(ti);
                    }

                    mFace[i] = inx;
                }

                mTris.AddRange(Triangulate(f, mFace));
            }

            // Create Mesh
            var m = new Mesh();
            
            // Vertices
            m.Vertices = PackedArray.New<float3>(vKeys.Count);

            int iVs = 0;
            foreach (var val in vKeys)
                m.Vertices[iVs++] = new float3(_vertices[val.iV]);

            // UVs
            if (HasTexCoords)
            {
                m.UVs = PackedArray.New<float2>(_vertices.Count);

                int i = 0;
                foreach (var val in vKeys)
                    m.UVs[i++] = new float2(_texCoords[val.iT]);
            }

            // Normals
            if (HasNormals)
            {
                m.Normals = PackedArray.New<float3>(_vertices.Count);

                int i = 0;
                foreach (var val in vKeys)
                    m.Normals[i++] = new float3(_normals[val.iN]);
            }

            m.Triangles = mTris.ToArray();

            return m;
        }

        private short[] Triangulate(Face f, int[] indices)
        {
            if (f.InxVert.Length < 3)
                return null;

            if (indices == null)
                indices = f.InxVert;

            short[] ret = new short[3 * (f.InxVert.Length-2)];
            // Perform a fan triangulation
            for (int i = 2; i < f.InxVert.Length; i++ )
            {
                ret[(i - 2)*3 + 0] = (short)indices[0];
                ret[(i - 2)*3 + 1] = (short)indices[i - 1];
                ret[(i - 2)*3 + 2] = (short)indices[i];
            }
            return ret;
        }
        #endregion
    }
}
