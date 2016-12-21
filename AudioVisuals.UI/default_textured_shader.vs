#version 330 core

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 textureCoords;

// Output to frag shader
out vec4 vertexColor;
out vec2 texCoords;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.xyz, 1.0);
	vertexColor = color;
	texCoords = textureCoords;
}