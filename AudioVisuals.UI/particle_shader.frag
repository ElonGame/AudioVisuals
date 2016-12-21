#version 330 core

// Input from vertex shader
in vec4 vertexColor;

out vec4 color;

uniform sampler2D particleTexture;

void main()
{
	color = texture(particleTexture, gl_PointCoord) * vertexColor;
}

