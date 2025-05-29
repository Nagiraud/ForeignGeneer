shader_type spatial;

uniform vec4 berry_color : source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float glossiness : hint_range(0.0, 1.0) = 0.8; 
uniform float rim_intensity : hint_range(0.0, 2.0) = 1.2;
uniform float scatter_intensity : hint_range(0.0, 2.0) = 0.5; 

void fragment() {
    vec3 normal_world = normalize(NORMAL);
    vec3 view_dir = normalize(VIEW);
    vec3 light_dir = normalize(LIGHT_VERTEX);
    

    float diff = max(dot(normal_world, light_dir), 0.0);

    vec3 halfway_dir = normalize(view_dir + light_dir);
    float spec = pow(max(dot(normal_world, halfway_dir), 0.0), glossiness * 128.0);
    

    float fresnel = pow(1.0 - max(dot(view_dir, normal_world), 0.0), 3.0) * rim_intensity;
    
   
    float scatter = max(dot(-light_dir, normal_world), 0.0) * scatter_intensity;
    

    vec3 final_color = berry_color.rgb * (diff + scatter) + vec3(1.0) * spec + fresnel;
    
    ALBEDO = final_color;
    METALLIC = 0.1; 
    ROUGHNESS = 1.0 - glossiness;
    SPECULAR = 0.7; 
}
