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
	
	    vec4 main_texture = texture(TEXTURE, UV);
	    vec4 noise_texture = texture(dissolve_texture, UV);
	    main_texture.a *= floor(dissolve_value + min(1.0, noise_texture.x));
	    COLOR = modulate * main_texture;
	
}