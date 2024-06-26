shader_type canvas_item;

uniform vec4 line_color : source_color = vec4(1);
uniform float line_thickness : hint_range(0, 10) = 1.0;

const vec2 OFFSETS[8] = {
	vec2(-1, -1), vec2(-1, 0), vec2(-1, 1), vec2(0, -1), vec2(0, 1), 
	vec2(1, -1), vec2(1, 0), vec2(1, 1)
};

const float SPEED = 1.5f;

vec4 rainbow(float level)
{
	/*
		Target colors
		=============
		
		L  x   color
		0  0.0 vec4(1.0, 0.0, 0.0, 1.0);
		1  0.2 vec4(1.0, 0.5, 0.0, 1.0);
		2  0.4 vec4(1.0, 1.0, 0.0, 1.0);
		3  0.6 vec4(0.0, 0.5, 0.0, 1.0);
		4  0.8 vec4(0.0, 0.0, 1.0, 1.0);
		5  1.0 vec4(1.0, 0.0, 0.5, 1.0);
	*/
	if (level < 1.0) {
		return vec4(1.0, 0.0, 0.0, 1.0);
	}
	if (level < 2.0) {
		return vec4(1.0, 0.5, 0.0, 1.0);
	}
	if (level < 3.0) {
		return vec4(1.0, 1.0, 0.0, 1.0);
	}
	if (level < 4.0) {
		return vec4(0.0, 0.5, 0.0, 1.0);
	}
	if (level < 5.0) {
		return vec4(0.0, 0.0, 1.0, 1.0);
	}			
	return vec4(1.0, 0.0, 0.5, 1.0);
}

vec4 smoothRainbow (float x)
{
    float level1 = floor(x*6.0);
    float level2 = floor(x*6.0) + 1.0;
	if (level2 > 6.0) {
		level2 = 0.0;
	}
    
    vec4 a = rainbow(level1);
    vec4 b = rainbow(level2);
    
    return mix(a, b, fract(x*6.0));
}

void fragment() {
	vec2 size = TEXTURE_PIXEL_SIZE * line_thickness / 2.0;
	vec4 color = texture(TEXTURE, UV);
	if (color.a > 0.0) {
		float value = UV.x + UV.y + TIME * SPEED;		
		while(value > 6.0) {
			value = value - 6.0;
		}
		vec4 newColor = rainbow(value);
		COLOR = newColor;
	}
}