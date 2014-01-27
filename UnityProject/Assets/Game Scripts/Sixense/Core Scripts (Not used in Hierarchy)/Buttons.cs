using UnityEngine;
using System.Collections;

public class Buttons{
	
	private bool _one;
	private bool _two;
	private bool _three;
	private bool _four;
	private bool _start;
	private bool _joystick;
	private bool _bumper;
	
	public bool One{get{return _one;}}
	public bool Two{get{return _two;}}
	public bool Three{get{return _three;}}
	public bool Four{get{return _four;}}
	public bool Start{get{return _start;}}
	public bool Joystick{get{return _joystick;}}
	public bool Bumper{get{return _bumper;}}
	
	public Buttons(int buttons)
	{
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_JOYSTICK)
		{
			_joystick = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_JOYSTICK;
		}
		else
		{
			_joystick = false;
		}
		
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_BUMPER)
		{
			_bumper = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_BUMPER;
		}
		else
		{
			_bumper = false;
		}
		
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_2)
		{
			_two = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_2;
		}
		else
		{
			_two = false;
		}
		
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_1)
		{
			_one = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_1;
		}
		else
		{
			_one = false;
		}
		
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_4)
		{
			_four = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_4;
		}
		else
		{
			_four = false;
		}
		
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_3)
		{
			_three = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_3;
		}
		else
		{
			_three = false;
		}
		
		if(buttons >= (int)SixenseControllerButton.SIXENSE_BUTTON_START)
		{
			_start = true;
			buttons -= (int)SixenseControllerButton.SIXENSE_BUTTON_START;
		}
		else
		{
			_start = false;
		}
	}
}
