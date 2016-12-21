#version 330 core

layout(location = 0) in vec4 vertex;
layout(location = 1) in vec4 color;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform float heightOfNearPlane;

out vec4 vertexColor;

void main()
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.xyz, 1.0);
	gl_PointSize = (heightOfNearPlane * vertex.w) / gl_Position.w;

	vertexColor = color;
}