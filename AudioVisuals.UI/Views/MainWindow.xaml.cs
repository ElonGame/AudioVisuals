﻿using Framework.OGL;
using GlmNet;
using SharpGL;
using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

namespace AudioVisuals.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants

        private const int EffectCount = 5;
        private const float InitialCameraZ = -20.0f;

        #endregion

        #region Private Member Variables

        private int _activeEffect;
        private float _heightOfNearPlane;
        private Random _random = new Random();
        private LineBall _lineBall = new LineBall();
        private LaserFlare _backfiller = new LaserFlare();
        private LineSpectrum _lineSpectrum = new LineSpectrum();
        private ParticleSystem _mainParticleSystem = new ParticleSystem();
        private BeatParticleSystemEmitter _beatParticleSystemEmitter = new BeatParticleSystemEmitter();
        private LaserFlare _laserFlare = new LaserFlare();
        private LightningStriker _lightningStriker = new LightningStriker();
        private ParticleStars _particleStars = new ParticleStars();
        private ParticleLaserSpectrum _leftParticleLaser = new ParticleLaserSpectrum();
        private ParticleLaserSpectrum _rightParticleLaser = new ParticleLaserSpectrum();
        private ParticleLineSpectrum _particleLineSpectrum = new ParticleLineSpectrum();
        private ParticleBurner _particleBurner = new ParticleBurner();
        private ParticleBlender _particleBlender = new ParticleBlender();
        private ParticleGlowCube _particleGlowCube = new ParticleGlowCube();
        private ParticleBall _particleBall = new ParticleBall();

        #endregion

        #region Private Properties

        private MainWindowViewModel ViewModel { get { return (MainWindowViewModel)DataContext; } }

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            MessageBoxResult result = MessageBox.Show("Fullscreen? (ESC to exit)", "Loopback Audio Visualization", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                ParticleSettingsStackPanel.Visibility = Visibility.Collapsed;
            }

            if(result == MessageBoxResult.Cancel)
            {
                Close();
                return;
            }

            DataContext = new MainWindowViewModel();
        }

        #endregion

        #region Event Handlers

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.Audio.StopListen();
            }
        }

        private void OpenGLSurface_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = OpenGLSurface.OpenGL;

            // Clears SharpGL / GLEW bug of having invalid enum set as error on startup
            gl.GetLatestErrorDescription();
            
            // Setup OpenGL
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            gl.ClearDepth(1.0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_BLEND);
            gl.Enable(OpenGL.GL_PROGRAM_POINT_SIZE);
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Hint(OpenGL.GL_POINT_SMOOTH_HINT, OpenGL.GL_NICEST);

            // Init matrices, camera, shaders
            GlState.Instance.Init(gl);

            // Init camera position
            GlState.Instance.ResetCamera();

            // Line ball init
            _lineBall.Init(gl);

            // Line spectrum init
            _lineSpectrum.Init(gl);

            // Main particle system init
            _mainParticleSystem.Init(gl, OpenGL.GL_ONE, 5000, true, true);

            // Laser flare
            _laserFlare.Init(gl);

            // Lightning striker
            _lightningStriker.Init(gl);

            // Laser particle system init
            _leftParticleLaser.Init(gl, _random);
            _rightParticleLaser.Init(gl, _random);

            // Particle stars
            _particleStars.Init(gl);

            // Particle line spectrum
            _particleLineSpectrum.Init(gl);

            // Particle burner
            _particleBurner.Init(gl);

            // Particle blender
            _particleBlender.Init(gl);

            // Particle glow cube
            _particleGlowCube.Init(gl);

            // Beat particle systems
            _beatParticleSystemEmitter.Init(gl);

            // Particle ball
            _particleBall.Init(gl);
        }

        private void OpenGLSurface_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = OpenGLSurface.OpenGL;
            setPerspective(gl);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            OpenGL gl = OpenGLSurface.OpenGL;
            setPerspective(gl);
        }

        private void OpenGLSurface_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = OpenGLSurface.OpenGL;
            
            // Clear and reset
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Reset model
            GlState.Instance.ModelMatrix = mat4.identity();

            // Reset view
            GlState.Instance.ViewMatrix = mat4.identity();

            // Camera
            GlState.Instance.ViewMatrix = glm.lookAt(GlState.Instance.CameraInfo.GetEyeVec3(), GlState.Instance.CameraInfo.GetCenterVec3(), GlState.Instance.CameraInfo.GetUpVec3());          

            // Default shader
            GlState.Instance.DefaultShader.Use(gl);

            gl.UniformMatrix4(GlState.Instance.DefaultViewMatrixLocation, 1, false, GlState.Instance.ViewMatrix.to_array());
            gl.UniformMatrix4(GlState.Instance.DefaultProjectionMatrixLocation, 1, false, GlState.Instance.ProjectionMatrix.to_array());

            // Line ball
            //_lineBall.Draw(gl, 10.0f, 10.0f, -10.0f, ViewModel.AudioData);

            // Default textured shader
            GlState.Instance.DefaultTexturedShader.Use(gl);

            gl.UniformMatrix4(GlState.Instance.DefaultTexturedViewMatrixLocation, 1, false, GlState.Instance.ViewMatrix.to_array());
            gl.UniformMatrix4(GlState.Instance.DefaultTexturedProjectionMatrixLocation, 1, false, GlState.Instance.ProjectionMatrix.to_array());

            // Lightning striker
            //_lightningStriker.Draw(gl, 0.0f, 0.0f, -11.0f, ViewModel.AudioData);

            // Draw spectrum using spectrum model shader
            GlState.Instance.SpectrumShader.Use(gl);

            gl.UniformMatrix4(GlState.Instance.SpectrumViewMatrixLocation, 1, false, GlState.Instance.ViewMatrix.to_array());
            gl.UniformMatrix4(GlState.Instance.SpectrumProjectionMatrixLocation, 1, false, GlState.Instance.ProjectionMatrix.to_array());

            //_lineSpectrum.Draw(gl, 0.0f, -12.0f, 0.0f, ViewModel.AudioData);

            // Draw particle systems using particle model shader
            GlState.Instance.ParticleShader.Use(gl);

            gl.UniformMatrix4(GlState.Instance.ParticleModelMatrixLocation, 1, false, GlState.Instance.ModelMatrix.to_array());
            gl.UniformMatrix4(GlState.Instance.ParticleViewMatrixLocation, 1, false, GlState.Instance.ViewMatrix.to_array());
            gl.UniformMatrix4(GlState.Instance.ParticleProjectionMatrixLocation, 1, false, GlState.Instance.ProjectionMatrix.to_array());
            gl.Uniform1(GlState.Instance.ParticleHeightOfNearPlaneLocation, _heightOfNearPlane);

            // Main particle system
            //_mainParticleSystem.Draw(gl, 0.0f, 0.0f, -20.0f, ViewModel.AudioModifier);

            // Beat particle system
            //_beatParticleSystemEmitter.Draw(gl, ViewModel.AudioData);

            // Particle stars (always draw regardless of effect)
            _particleStars.Draw(gl, 0.0f, 0.0f, 0.0f, ViewModel.AudioData50);

            // Particle glow cube
            if (_activeEffect % EffectCount == 0)
            {
                _particleBurner.CurlEpsilon = ViewModel.CurlEpsilonf;
                _particleBurner.NoiseIntensity = ViewModel.NoiseIntensityf;
                _particleBurner.NoiseSampleScale = ViewModel.NoiseSampleScalef;
                _particleBurner.FixedVelocityModifier = ViewModel.FixedVelocityModifierf;
                _particleBurner.ParticleChaos = ViewModel.ParticleChaosf;
                _particleBurner.Draw(gl, 0.0f, 0.0f, 0.0f, ViewModel.AudioData50);
                //if(!GlState.Instance.IsAutoMoveCameraActive)
                //{
                //    GlState.Instance.StartAutoMoveCamera();
                //}

                //GlState.Instance.UpdateAutoMoveCamera();
                //_particleGlowCube.Draw(gl, 0.0f, 0.0f, 0.0f, ViewModel.AudioData200);
            }

            // Particle blender
            if (_activeEffect % EffectCount == 1)
            {
                float[] audioData200 = ViewModel.AudioData200;
                _particleBlender.Draw(gl, -10.45f, 0.0f, 0.0f, audioData200);
                GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, glm.radians(180.0f), new vec3(0, 1, 0));
                _particleBlender.Draw(gl, -10.45f, 0.0f, 0.0f, audioData200);
            }

            // Particle line spectrum
            //_particleLineSpectrum.Draw(gl, 0.0f, -12.0f, 0.0f, ViewModel.AudioData);
            //GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, glm.radians(180.0f), new vec3(0, 1, 0));
            //_particleLineSpectrum.Draw(gl, -5.0f, -12.0f, 0.0f, ViewModel.AudioData);

            // Particle laser
            if (_activeEffect % EffectCount == 2)
            {
                float[] audioData50 = ViewModel.AudioData50;
                _leftParticleLaser.Draw(gl, -36.0f, 0.0f, -15.0f, audioData50);
                GlState.Instance.ModelMatrix = glm.rotate(GlState.Instance.ModelMatrix, glm.radians(180.0f), new vec3(0, 1, 0));
                _rightParticleLaser.Draw(gl, -36.0f, 0.0f, 15.0f, audioData50);
            }

            // Particle burner
            if (_activeEffect % EffectCount == 3)
            {
                _particleBurner.Draw(gl, 0.0f, -13.0f, 0.0f, ViewModel.AudioData1000);
            }

            // Particle ball
            if (_activeEffect % EffectCount == 4)
            {
                _particleBall.Draw(gl, 0.0f, 0.0f, -10.0f, ViewModel.AudioData50);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            OpenGL gl = OpenGLSurface.OpenGL;

            if(e.Key == Key.Escape)
            {
                Close();
            }

            if (e.Key == Key.F1)
            {
                _activeEffect++;

                // Effect 1 uses the auto move camera
                if (_activeEffect % EffectCount != 0)
                {
                    GlState.Instance.ResetCamera();
                }
            }

            if ((Keyboard.GetKeyStates(Key.W) & KeyStates.Down) > 0)
            {
                GlState.Instance.CameraInfo.EyeZ -= 1.0f;
                //GlState.Instance.CameraInfo.CenterZ = GlState.Instance.CameraInfo.EyeZ - 100.0f;
            }
            
            if ((Keyboard.GetKeyStates(Key.S) & KeyStates.Down) > 0)
            {
                GlState.Instance.CameraInfo.EyeZ += 1.0f;
                //GlState.Instance.CameraInfo.CenterZ = GlState.Instance.CameraInfo.EyeZ - 100.0f;
            }
            
            if ((Keyboard.GetKeyStates(Key.A) & KeyStates.Down) > 0)
            {
                GlState.Instance.CameraInfo.EyeX -= 1.0f;
                GlState.Instance.CameraInfo.LookAtX -= 1.0f;
            }
            
            if ((Keyboard.GetKeyStates(Key.D) & KeyStates.Down) > 0)
            {
                GlState.Instance.CameraInfo.EyeX += 1.0f;
                GlState.Instance.CameraInfo.LookAtX += 1.0f;
            }

            if ((Keyboard.GetKeyStates(Key.Left) & KeyStates.Down) > 0)
            {
                GlState.Instance.CameraInfo.LookAtX -= 1.0f;
            }

            if ((Keyboard.GetKeyStates(Key.Right) & KeyStates.Down) > 0)
            {
                GlState.Instance.CameraInfo.LookAtX += 1.0f;
            }
        }

        #endregion

        #region Private Methods

        private void setPerspective(OpenGL gl)
        {
            GlState.Instance.ProjectionMatrix = mat4.identity();
            GlState.Instance.ProjectionMatrix = glm.perspective(glm.radians(Constants.FovyDegrees), (float)OpenGLSurface.ActualWidth / (float)OpenGLSurface.ActualHeight, 0.1f, 100.0f);

            int[] viewport = new int[4];
            gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
            _heightOfNearPlane = Math.Abs(viewport[3] - viewport[1]) / (2 * glm.tan(0.5f * Constants.FovyDegrees * (float)Math.PI / 180.0f));
        }

        #endregion
    }
}
