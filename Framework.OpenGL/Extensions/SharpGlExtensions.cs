using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace Framework.OGL
{
    public static class SharpGlExtensions
    {
        public static string GetLatestErrorDescription(this OpenGL gl)
        {
            SharpGL.Enumerations.ErrorCode code = gl.GetErrorCode();
            return gl.GetErrorDescription((uint)code);
        }

        public static uint LoadTexture(this OpenGL gl, string textureName)
        {
            // Load Bitmaps And Convert To Textures
            Texture particleTexture = new Texture();
            if (particleTexture.Create(gl, textureName))
            {
                return particleTexture.TextureName;
            }

            throw new InvalidOperationException(string.Format("Unable to load texture '{0}'.", textureName));
        }
    }
}
