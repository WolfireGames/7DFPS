using UnityEngine;


public class FlashlightScript : MonoBehaviour, InventoryItem.InventoryItemListener {
    
    public AnimationCurve battery_curve;
    public AudioClip sound_turn_on;
    public AudioClip sound_turn_off;
    float kSoundVolume = 0.3f;
    bool switch_on = false;
    const float max_battery_life = 60*60*5.5f;
    float battery_life_remaining = max_battery_life;
    
    float initial_pointlight_intensity;
    float initial_spotlight_intensity;
    
    public void Awake() {
    	switch_on = false;// Random.Range(0.0,1.0) < 0.5;
    }
    
    public void Start() {
    	initial_pointlight_intensity = transform.Find("Pointlight").gameObject.GetComponent<Light>().intensity;
    	initial_spotlight_intensity = transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity;
    	battery_life_remaining = UnityEngine.Random.Range(max_battery_life*0.2f, max_battery_life);
    }
    
    public void TurnOn(){
    	if(!switch_on){
    		switch_on = true;
    		GetComponent<AudioSource>().PlayOneShot(sound_turn_on, kSoundVolume * PlayerPrefs.GetFloat("sound_volume", 1.0f));
    	}
    }
    
    public void TurnOff(){
    	if(switch_on){
    		switch_on = false;
    		GetComponent<AudioSource>().PlayOneShot(sound_turn_off, kSoundVolume * PlayerPrefs.GetFloat("sound_volume", 1.0f));
    	}
    }
    
    public void Update() {
    	if(switch_on){
    		battery_life_remaining -= Time.deltaTime;
    		if(battery_life_remaining <= 0.0f){
    			battery_life_remaining = 0.0f;
    		}
    		float battery_curve_eval = battery_curve.Evaluate(1.0f-battery_life_remaining/max_battery_life);
    		transform.Find("Pointlight").gameObject.GetComponent<Light>().intensity = initial_pointlight_intensity * battery_curve_eval * 8.0f;
    		transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = initial_spotlight_intensity * battery_curve_eval * 3.0f;
    		transform.Find("Pointlight").gameObject.GetComponent<Light>().enabled = true;
    		transform.Find("Spotlight").gameObject.GetComponent<Light>().enabled = true;
    	} else {
    		transform.Find("Pointlight").gameObject.GetComponent<Light>().enabled = false;
    		transform.Find("Spotlight").gameObject.GetComponent<Light>().enabled = false;
    	}
    	if(GetComponent<Rigidbody>() != null){
    		transform.Find("Pointlight").GetComponent<Light>().enabled = true;
    		transform.Find("Pointlight").GetComponent<Light>().intensity = 1.0f + Mathf.Sin(Time.time * 2.0f);
    		transform.Find("Pointlight").GetComponent<Light>().range = 1.0f;
    	} else {
    		transform.Find("Pointlight").GetComponent<Light>().range = 10.0f;
    	}
    }
    
    // Inventory Item interface
    public void OnDrop() {
    	TurnOff();
    }
    
    public void OnPickup() {
    	TurnOn();
    }
}