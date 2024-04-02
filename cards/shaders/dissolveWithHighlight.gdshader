shader_type canvas_item;

uniform sampler2D dissolve_texture : hint_default_transparent;
uniform float dissolve_value : hint_range(0,1);
uniform vec4 line_color : source_color = vec4(1); 
uniform float line_thickness : hint_range(0.0, 16.0);

varying flat vec4 modulate;

void vertex() {
	modulate = COLOR;
}


void fragment(){
	// See if we are disolving
	if (dissolve_value != 1.0){
	    vec4 main_texture = texture(TEXTURE, UV);
    	vec4 noise_texture = texture(dissolve_texture, UV);
    	main_texture.a *= floor(dissolve_value + min(1.0, noise_texture.x));
		COLOR = modulate * main_texture;
	} else {
		// else do highlighting
	    vec2 size = vec2(line_thickness) / vec2(textureSize(TEXTURE, 0));
	    vec4 sprite_color_no_mod = texture(TEXTURE, UV);
	    vec4 sprite_color = texture(TEXTURE, UV) * modulate;
		if (line_color.a == 0.0f ) {
			COLOR = sprite_color;
		} else {
		    float alpha = sprite_color.a;
		    alpha += texture(TEXTURE, UV + vec2(0.0, -size.y)).a;
		    alpha += texture(TEXTURE, UV + vec2(size.x, -size.y)).a;
		    alpha += texture(TEXTURE, UV + vec2(size.x, 0.0)).a;
		    alpha += texture(TEXTURE, UV + vec2(size.x, size.y)).a;
		    alpha += texture(TEXTURE, UV + vec2(0.0, size.y)).a;
		    alpha += texture(TEXTURE, UV + vec2(-size.x, size.y)).a;
		    alpha += texture(TEXTURE, UV + vec2(-size.x, 0.0)).a;
		    alpha += texture(TEXTURE, UV + vec2(-size.x, -size.y)).a; 
		   
		    vec3 final_color = mix(line_color.rgb, sprite_color.rgb, sprite_color.a);
		    COLOR = vec4(final_color, clamp(alpha, 0.0, modulate.a));
		}
	}


}