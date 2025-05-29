// Ground Shader for overlay effect around the player
shader_type spatial;

// Uniforms for player position and overlay effect
uniform vec3 player_position;
uniform float radius = 25.0;
uniform vec4 overlay_color : source_color;
uniform float overlay_strength = 0.0; // Ranges from 0.0 (no overlay) to 1.0 (full overlay)

void fragment() {
    // Compute the distance from the fragment to the player position
    float distance_to_player = distance(VERTEX, player_position);

    // Determine the overlay factor based on distance and radius
    float overlay_factor = smoothstep(radius, radius * 0.9, distance_to_player);

    // Mix the overlay color with the existing material color
    vec4 base_color = texture(TEXTURE, UV);
    vec4 final_color = mix(overlay_color, base_color, overlay_factor);

    // Apply the overlay strength
    ALBEDO = mix(base_color.rgb, final_color.rgb, overlay_strength);
}
