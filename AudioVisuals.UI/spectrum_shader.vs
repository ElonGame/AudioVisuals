#version 330 core

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec4 positionAndSize;
layout(location = 2) in vec4 color;

out vec4 vertexColor;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main()
{
	float height = positionAndSize.w;

	vec3 finalVertex = vertex;
	vec4 finalColor = color;

	if(vertex.y >= 0.0)
	{
		finalVertex = vec3(vertex.x, vertex.y + height, vertex.z);
		finalColor.a = 0.1;
	}

	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(finalVertex + positionAndSize.xyz, 1.0);
	vertexColor = finalColor;
}