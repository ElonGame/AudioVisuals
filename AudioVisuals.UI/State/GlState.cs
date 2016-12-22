using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.OGL;
using GlmNet;
using SharpGL;

namespace AudioVisuals.UI
{
    public class GlState
    {
        #region Private Properties

        // Shaders
        private Shader _defaultShader;
        private Shader _defaultTexturedShader;
        private Shader _spectrumShader;
        private Shader _particleShader;

        // Rendering matrices
        private mat4 _modelMatrix = mat4.identity();
        private mat4 _viewMatrix = mat4.identity();
        private mat4 _projectionMatrix = mat4.identity();

        // Camera
        private CameraInfo _cameraInfo = new CameraInfo();
        private ObjectLocationInfo _cameraLookAtTarget = new ObjectLocationInfo();

        #endregion

        #region Public Properties

        // Singleton instance
        public static readonly GlState Instance = new GlState();

        public Shader DefaultShader { get { return _defaultShader; } }
        public Shader DefaultTexturedShader { get { return _defaultTexturedShader; } }
        public Shader SpectrumShader { get { return _spectrumShader; } }
        public Shader ParticleShader { get { return _particleShader; } }

        public int DefaultModelMatrixLocation { get; private set; }
        public int DefaultViewMatrixLocation { get; private set; }
        public int DefaultProjectionMatrixLocation { get; private set; }

        public int DefaultTexturedModelMatrixLocation { get; private set; }
        public int DefaultTexturedViewMatrixLocation { get; private set; }
        public int DefaultTexturedProjectionMatrixLocation { get; private set; }

        public int SpectrumModelMatrixLocation { get; private set; }
        public int SpectrumViewMatrixLocation { get; private set; }
        public int SpectrumProjectionMatrixLocation { get; private set; }

        public int ParticleModelMatrixLocation { get; private set; }
        public int ParticleViewMatrixLocation { get; private set; }
        public int ParticleProjectionMatrixLocation { get; private set; }
        public int ParticleTextureLocation { get; private set; }
        public int ParticleHeightOfNearPlaneLocation { get; private set; }

        public mat4 ModelMatrix { get { return _modelMatrix; } set { _modelMatrix = value; } }
        public mat4 ViewMatrix { get { return _viewMatrix; } set { _viewMatrix = value; } }
        public mat4 ProjectionMatrix { get { return _projectionMatrix; } set { _projectionMatrix = value; } }

        public CameraInfo CameraInfo { get { return _cameraInfo; } }

        #endregion

        #region Constructor

        private GlState()
        {

        }

        #endregion

        #region Public Methods

        public void Init(OpenGL gl)
        {
            // Create shaders
            _defaultShader = new Shader(gl, "default_shader.vs", "default_shader.frag");
            _defaultTexturedShader = new Shader(gl, "default_textured_shader.vs", "default_textured_shader.frag");
            _spectrumShader = new Shader(gl, "spectrum_shader.vs", "spectrum_shader.frag");
            _particleShader = new Shader(gl, "particle_shader.vs", "particle_shader.frag");

            DefaultModelMatrixLocation = gl.GetUniformLocation(_defaultShader.Program, "modelMatrix");
            DefaultViewMatrixLocation = gl.GetUniformLocation(_defaultShader.Program, "viewMatrix");
            DefaultProjectionMatrixLocation = gl.GetUniformLocation(_defaultShader.Program, "projectionMatrix");

            DefaultTexturedModelMatrixLocation = gl.GetUniformLocation(_defaultTexturedShader.Program, "modelMatrix");
            DefaultTexturedViewMatrixLocation = gl.GetUniformLocation(_defaultTexturedShader.Program, "viewMatrix");
            DefaultTexturedProjectionMatrixLocation = gl.GetUniformLocation(_defaultTexturedShader.Program, "projectionMatrix");

            SpectrumModelMatrixLocation = gl.GetUniformLocation(_spectrumShader.Program, "modelMatrix");
            SpectrumViewMatrixLocation = gl.GetUniformLocation(_spectrumShader.Program, "viewMatrix");
            SpectrumProjectionMatrixLocation = gl.GetUniformLocation(_spectrumShader.Program, "projectionMatrix");

            ParticleModelMatrixLocation = gl.GetUniformLocation(_particleShader.Program, "modelMatrix");
            ParticleViewMatrixLocation = gl.GetUniformLocation(_particleShader.Program, "viewMatrix");
            ParticleProjectionMatrixLocation = gl.GetUniformLocation(_particleShader.Program, "projectionMatrix");
            ParticleTextureLocation = gl.GetUniformLocation(_particleShader.Program, "particleTexture");
            ParticleHeightOfNearPlaneLocation = gl.GetUniformLocation(_particleShader.Program, "heightOfNearPlane");
        }

        public void StartAutoMoveCamera()
        {
            // Init invisible camera target
            _cameraLookAtTarget.Init(-30.0f, -30.0f, -40.0f, 30.0f, 30.0f, 40.0f, 0.8f, 0.4f, 0.7f);
        }

        public void UpdateAutoMoveCamera()
        {
            // Move invisible camera target
            _cameraLookAtTarget.Update();

            // Move the eye in the sky to chase the target
            float distanceToTargetX = _cameraLookAtTarget.X - Instance.CameraInfo.EyeX;
            float distanceToTargetY = _cameraLookAtTarget.Y - Instance.CameraInfo.EyeY;
            float distanceToTargetZ = _cameraLookAtTarget.Z - Instance.CameraInfo.EyeZ;

            Instance.CameraInfo.EyeX += distanceToTargetX / 20.0f;
            Instance.CameraInfo.EyeY += distanceToTargetY / 20.0f;
            Instance.CameraInfo.EyeZ += distanceToTargetZ / 20.0f;
        }

        public void ResetCamera()
        {
            // Camera reset
            Instance.CameraInfo.UpX = 0;
            Instance.CameraInfo.UpY = 1;
            Instance.CameraInfo.UpZ = 0;

            Instance.CameraInfo.EyeX = 0.0f;
            Instance.CameraInfo.EyeY = 0.0f;
            Instance.CameraInfo.EyeZ = 30.0f;

            Instance.CameraInfo.LookAtX = 0.0f;
            Instance.CameraInfo.LookAtY = 0.0f;
            Instance.CameraInfo.LookAtZ = 0.0f;
        }

        #endregion
    }
}
