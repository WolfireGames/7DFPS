#pragma strict

var skin:GUISkin;
var windowRect : Rect;
var menu_width = 300;
var menu_height = 500;

function OnGUI () {
	windowRect = GUI.Window (
		0, 
		Rect(Screen.width*0.5 - menu_width*0.5, Screen.height*0.5 - menu_height*0.5, menu_width, menu_height), 
		WindowFunction, 
		"",
		skin.window);
}

private var draw_cursor : Vector2;
private var draw_cursor_line : Vector2;
private var line_height = 24;
private var line_offset = 24;

function DrawCursor_Init() {
	draw_cursor = Vector2(25,25);
	draw_cursor_line = Vector2(0,0);	
}

function DrawCursor_NextLine() {
	draw_cursor_line = Vector2(0,0);	
	draw_cursor.y += line_offset;
}

function DrawCursor_Offset(val : float) {
	draw_cursor_line.x += val;	
}

function DrawCursor_Get() : Vector2 {
	return draw_cursor + draw_cursor_line;
}

function DrawCursor_RectSpace(width : float) : Rect {
	var rect = Rect(draw_cursor.x + draw_cursor_line.x,
					draw_cursor.y + draw_cursor_line.y,
					width,
					line_height);
	DrawCursor_Offset(width);
	return rect;
}

function DrawLabel(text : String) {
	DrawCursor_Offset(17);
	GUI.Label (
		DrawCursor_RectSpace(400),//GUI.skin.label.CalcSize(new GUIContent(text)).x), 
		text, 
		skin.label);
}

function DrawCheckbox( val : boolean, text : String ) : boolean {
	val = GUI.Toggle (
		DrawCursor_RectSpace(400), 
		val,
		text, 
		skin.toggle);
	return val;			
}

function DrawSlider( val : float) : float {
	DrawCursor_Offset(18);
	val = GUI.HorizontalSlider (
		DrawCursor_RectSpace(400 - draw_cursor_line.x), 
		val,
		0.0, 
		1.0);
	return val;			
}

function DrawButton( text : String) : boolean {
	var val = GUI.Button (
		DrawCursor_RectSpace(200), 
		text,
		skin.button);
	return val;			
}

private var brightness = 0.3;
private var master_volume = 1.0;
private var sound_volume = 1.0;
private var music_volume = 1.0;
private var voice_volume = 1.0;
private var lock_gun_to_center = false;
private var mouse_invert = false;
private var mouse_sensitivity = 0.2;
private var show_advanced_sound = false;
private var toggle_crouch = true;
private var lock_gun_to_screen = 0.0;
private var scroll_view_vector = Vector2(0,0);
private var vert_scroll = 0.0;
private var vert_scroll_pixels = 0.0;

function Update() {
	if(vert_scroll != -1.0){
		vert_scroll += Input.GetAxis("Mouse ScrollWheel");
	}
}

function WindowFunction (windowID : int) {

    scroll_view_vector = GUI.BeginScrollView (
    	Rect (0, 0, windowRect.width, windowRect.height), 
    	scroll_view_vector, 
    	Rect (0, vert_scroll_pixels, windowRect.width, windowRect.height));

	DrawCursor_Init();
	DrawButton("Resume");
	draw_cursor.y += line_offset * 0.3;
	DrawCursor_NextLine();
	mouse_invert = DrawCheckbox(mouse_invert, "Invert mouse");
	DrawCursor_NextLine();
	DrawLabel("Mouse sensitivity:");
	DrawCursor_NextLine();
	mouse_sensitivity = DrawSlider(mouse_sensitivity);
	DrawCursor_NextLine();
	toggle_crouch = DrawCheckbox(toggle_crouch, "Toggle crouch");
	DrawCursor_NextLine();
	lock_gun_to_center = DrawCheckbox(lock_gun_to_center, "Lock gun to screen center");
	DrawCursor_NextLine();
	DrawLabel("Brightness:");
	DrawCursor_NextLine();
	brightness = DrawSlider(brightness);
	DrawCursor_NextLine();
	DrawLabel("Master volume:");
	DrawCursor_NextLine();
	master_volume = DrawSlider(master_volume);
	DrawCursor_NextLine();
	show_advanced_sound = DrawCheckbox(show_advanced_sound, "Advanced sound options");
	DrawCursor_NextLine();
	if(show_advanced_sound){
		var indent = 44;
		DrawLabel("....Sounds:");
		DrawCursor_NextLine();
		DrawCursor_Offset(indent);
		sound_volume = DrawSlider(sound_volume);
		DrawCursor_NextLine();
		DrawLabel("....Voice:");
		DrawCursor_NextLine();
		DrawCursor_Offset(indent);
		voice_volume = DrawSlider(voice_volume);
		DrawCursor_NextLine();
		DrawLabel("....Music:");
		DrawCursor_NextLine();
		DrawCursor_Offset(indent);
		music_volume = DrawSlider(music_volume);
		DrawCursor_NextLine();
	}
	DrawButton("Restore defaults");
	DrawCursor_NextLine();	
	draw_cursor.y += line_offset * 0.3;
	DrawButton("Exit game");
	
	var content_height = draw_cursor.y;
	
    GUI.EndScrollView();
    
    if(content_height > windowRect.height){
    	var leeway = (content_height / windowRect.height - windowRect.height / content_height);
    	if(vert_scroll == -1.0){
    		vert_scroll = leeway;
    	}
		vert_scroll = GUI.VerticalScrollbar (
			Rect (menu_width-20, 20, menu_width, menu_height-25), 
			vert_scroll, 
			windowRect.height / content_height,
			content_height / windowRect.height, 
			0.0);
		vert_scroll_pixels = windowRect.height * (leeway - vert_scroll);
	} else {
		vert_scroll = -1.0;
		vert_scroll_pixels = 0.0;
	}
}

