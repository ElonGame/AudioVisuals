#version 330 core

// Input from vertex shader
in vec4 vertexColor;
in vec2 texCoords;

out vec4 color;

uniform sampler2D inputTexture;

void main()
{
	color = texture(inputTexture, texCoords) * vertexColor;
}