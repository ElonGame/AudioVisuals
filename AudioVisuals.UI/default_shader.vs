#version 330 core

layout(location = 0) in vec4 vertex;
layout(location = 1) in vec4 color;

out vec4 vertexColor;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.xyz, 1.0);
	vertexColor = color;
}