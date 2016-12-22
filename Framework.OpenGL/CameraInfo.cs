using GlmNet;
namespace Framework.OGL
{
    public class CameraInfo
    {
        public float EyeX { get; set; }
        public float EyeY { get; set; }
        public float EyeZ { get; set; }

        public float LookAtX { get; set; }
        public float LookAtY { get; set; }
        public float LookAtZ { get; set; }

        public float UpX { get; set; }
        public float UpY { get; set; }
        public float UpZ { get; set; }

        public vec3 GetEyeVec3()
        {
            return new vec3(EyeX, EyeY, EyeZ);
        }

        public vec3 GetCenterVec3()
        {
            return new vec3(LookAtX, LookAtY, LookAtZ);
        }

        public vec3 GetUpVec3()
        {
            return new vec3(UpX, UpY, UpZ);
        }
    }
}
