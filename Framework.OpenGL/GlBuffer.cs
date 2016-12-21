using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace Framework.OGL
{
    public static class GlBuffer
    {
        public static void SetElementData(OpenGL gl, uint[] data, int sizeInBytes, uint openGlDrawType)
        {
            unsafe
            {
                fixed (uint* pointerToData = data)
                {
                    IntPtr pointerToIndicesData = new IntPtr((void*)pointerToData);
                    gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, sizeInBytes, pointerToIndicesData, openGlDrawType);
                    pointerToIndicesData.ToInt32();
                }
            }
        }

        public static void SetArrayData(OpenGL gl, float[] data, int sizeInBytes, uint openGlDrawType)
        {
            unsafe
            {
                fixed (float* pointerToData = data)
                {
                    IntPtr pointerToVerticesData = new IntPtr((void*)pointerToData);
                    gl.BufferData(OpenGL.GL_ARRAY_BUFFER, sizeInBytes, pointerToVerticesData, openGlDrawType);
                }
            }
        }

        public static void SetArraySubData(OpenGL gl, float[] data, int sizeInBytes)
        {
            unsafe
            {
                fixed (float* pointerToData = data)
                {
                    IntPtr pointerToVerticesData = new IntPtr((void*)pointerToData);
                    gl.BufferSubData(OpenGL.GL_ARRAY_BUFFER, 0, sizeInBytes, pointerToVerticesData);
                }
            }
        }
    }
}
