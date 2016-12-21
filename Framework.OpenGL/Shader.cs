using SharpGL;
using System;
using System.IO;
using System.Text;

namespace Framework.OGL
{
    public class Shader
    {
        public uint Program { get; private set; }

        public Shader(OpenGL gl, string vertexPath, string fragmentPath)
        {
            string vertexCode = File.ReadAllText(vertexPath);
            string fragmentCode = File.ReadAllText(fragmentPath);

            // Vertex shader
            uint vertexShader = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
            gl.ShaderSource(vertexShader, vertexCode);
            gl.CompileShader(vertexShader);

            // Fragment shader
            uint fragmentShader = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);
            gl.ShaderSource(fragmentShader, fragmentCode);
            gl.CompileShader(fragmentShader);

            // Create shader program
            Program = gl.CreateProgram();
            gl.AttachShader(Program, vertexShader);
            gl.AttachShader(Program, fragmentShader);
            gl.LinkProgram(Program);

            StringBuilder logStringBuilder = new StringBuilder(512);
            gl.GetProgramInfoLog(Program, 512, IntPtr.Zero, logStringBuilder);

            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);
        }

        public void Use(OpenGL gl)
        {
            gl.UseProgram(Program);
        }
    }
}
