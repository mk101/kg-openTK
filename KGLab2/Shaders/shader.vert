#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCord;

out vec2 texCord;

void main(void) {
    texCord = aTexCord;
    gl_Position = vec4(aPosition, 1.0);
}