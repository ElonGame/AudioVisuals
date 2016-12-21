using GlmNet;
using SharpGL;

namespace Framework.OGL
{
    public class Camera
    {
        // Default camera values
        private const float DefaultYaw = -90.0f;
        private const float DefaultPitch = 0.0f;
        private const float DefaultZoom = 45.0f;

        public vec3 Position { get; set; }
        public vec3 Front { get; set; }
        public vec3 Up { get; set; }
        public vec3 Right { get; set; }
        public vec3 WorldUp { get; set; }
        // Eular Angles
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        // Camera options
        public float Zoom { get; set; }

        // Default constructor with default params
        public Camera()
        {
            Position = new vec3(0.0f, 0.0f, 0.0f);
            Front = new vec3(0.0f, 0.0f, -1.0f);
            WorldUp = new vec3(0.0f, 1.0f, 0.0f);
            Yaw = DefaultYaw;
            Pitch = DefaultPitch;
            Zoom = DefaultZoom;
            //updateCameraVectors();
        }

        // Constructor with scalar values
        public Camera(float x, float y, float z, float upX, float upY, float upZ, float yaw, float pitch)
        {
            Position = new vec3(x, y, z);
            Front = new vec3(0.0f, 0.0f, -1.0f);
            WorldUp = new vec3(upX, upY, upZ);
            Yaw = yaw;
            Pitch = pitch;
            Zoom = DefaultZoom;
            //updateCameraVectors();
        }

        public void LookAt(CameraInfo cameraInfo)
        {

        //// Calculate the new Front vector
        //glm::vec3 front;
        //front.x = cos(glm::radians(this->Yaw)) * cos(glm::radians(this->Pitch));
        //front.y = sin(glm::radians(this->Pitch));
        //front.z = sin(glm::radians(this->Yaw)) * cos(glm::radians(this->Pitch));
        //this->Front = glm::normalize(front);
        //// Also re-calculate the Right and Up vector
        //this->Right = glm::normalize(glm::cross(this->Front, this->WorldUp));  // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
        //this->Up    = glm::normalize(glm::cross(this->Right, this->Front));
        }
    }
}
