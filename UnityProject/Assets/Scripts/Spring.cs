using UnityEngine;
using System;

[System.Serializable]
public class Spring {
	public float state;
	public float target_state;
	public float vel;
	public float strength;
	public float damping; 
	public Spring(float state,float target_state,float strength,float damping){
		this.Set(state, target_state, strength, damping); 
	}
	public void Set(float state,float target_state,float strength,float damping){
		this.state = state;
		this.target_state = target_state;
		this.strength = strength;
		this.damping = damping;
		this.vel = 0.0f;		
	}
	public void Update() {  
		bool linear_springs = false;
		if(linear_springs){
			this.state = Mathf.MoveTowards(this.state, this.target_state, this.strength * Time.deltaTime * 0.05f);
		} else {	 
			this.vel += (this.target_state - this.state) * this.strength * Time.deltaTime;
			this.vel *= Mathf.Pow(this.damping, Time.deltaTime);
			this.state += this.vel * Time.deltaTime;
		}
	}
};