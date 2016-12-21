using System;
using System.Collections.Generic;
using GlmNet;
namespace Framework.OGL
{
    public class GlSphere : GlPrimitiveBase
    {
        #region Constructor

        public GlSphere(float radius, int stacks, int slices)
            : base(3, 4, 0)
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

            List<float> allVertices = new List<float>();

            foreach (int index in indices)
            {
                allVertices.Add(vertices[index].x);
                allVertices.Add(vertices[index].y);
                allVertices.Add(vertices[index].z);
                allVertices.Add(1.0f);
                allVertices.Add(1.0f);
                allVertices.Add(1.0f);
                allVertices.Add(1.0f);
            }

            // Create float[]
            VertexData = allVertices.ToArray();
        }

        #endregion
    }
}
