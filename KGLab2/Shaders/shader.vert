#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCord;
//layout(location = 2) in vec2 sPosition;

out vec2 texCord;

//uniform mat4 model;
//uniform float time;
//uniform mat4 view;
//uniform mat4 projection;

void main(void) {
    texCord = aTexCord;
    
//    mat4 m = mat4(1.0, 0.0, 0.0, 0.0,
//                0.0, 1.0, 0.0, 0.0,
//                0.0,0.0,1.0,0.0,
//                mix(sPosition.r, aPosition.r, time), mix(sPosition.g, aPosition.g, time), 0.0, 0.0);
    
    gl_Position = vec4(aPosition, 1.0) /* * model  * view * projection*/;
}