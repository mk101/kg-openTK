#version 330 core

struct Light {
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;
    vec3 ambient;
};

uniform Light light;
uniform sampler2D texture0;

in vec2 texCord;
in vec3 FragPos;
//in vec3 Normal;

out vec4 FragColor;

void main() {
    vec3 lightDir = normalize(light.position - FragPos);
    
    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    if(theta > light.cutOff) {
        FragColor = vec4(vec3(texture(texture0, texCord) * intensity), 1);
    } else {
        FragColor = vec4(light.ambient * vec3(texture(texture0, texCord)), 1.0);
    }
}