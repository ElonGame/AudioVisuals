using System;
using System.Collections.Generic;
using GlmNet;
using System.Linq;

namespace Framework.OGL
{
    public class GlLineSphere : GlPrimitiveBase
    {
        #region Constructor

        public GlLineSphere(float radius, int stacks, int slices)
            : base(3, 0, 0)
        {
            List<vec3> vertices = new List<vec3>();
            List<int> indices = new List<int>();

            // Calc The Vertices
            for (int i = 0; i <= stacks; ++i)
            {

                float V   = i / (float)stacks;
                float phi = V * (float)Math.PI;

                // Loop Through Slices
                for (int j = 0; j <= slices; ++j)
                {

                    float U = j / (float)slices;
                    float theta = U * ((float)Math.PI * 2);

                    // Calc The Vertex Positions
                    float x = glm.cos(theta) * glm.sin(phi);
                    float y = glm.cos(phi);
                    float z = glm.sin(theta) * glm.sin(phi);

                    // Push Back Vertex Data
                    vertices.Add(new vec3(x, y, z) * radius);
                }
            }

            // Calc The Index Positions
            for (int i = 0; i < slices * stacks + slices; ++i)
            {
                indices.Add(i);
                indices.Add(i + slices + 1);
                indices.Add(i + slices);

                indices.Add(i + slices + 1);
                indices.Add(i);
                indices.Add(i + 1);
            }

            List<Vector3> addedVertices = new List<Vector3>();
            List<float> allVertices = new List<float>();

            foreach (int index in indices)
            {
                if (addedVertices.FirstOrDefault(v => v.x == vertices[index].x && v.y == vertices[index].y && v.z == vertices[index].z) == null)
                {
                    addedVertices.Add(new Vector3 { x = vertices[index].x, y = vertices[index].y, z = vertices[index].z });
                    allVertices.Add(0.0f); // Center X
                    allVertices.Add(0.0f); // Center Y
                    allVertices.Add(0.0f); // Center Z
                    allVertices.Add(vertices[index].x / 2);
                    allVertices.Add(vertices[index].y / 2); 
                    allVertices.Add(vertices[index].z / 2);
                    allVertices.Add(vertices[index].x); // Edge X
                    allVertices.Add(vertices[index].y); // Edge Y
                    allVertices.Add(vertices[index].z); // Edge Z
                }
            }

            // Create float[]
            VertexData = allVertices.ToArray();
        }

        #endregion

        #region Nested Classes

        private class Vector3
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
        }

        #endregion
    }
}
