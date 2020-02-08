using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

public class UGUIVRButton : MonoBehaviour {

	public bool pressed;

	public bool LongPress = false;

    public bool verticalSlider;

    public Slider slider;

    public Scrollbar scrollbar;

    public Dropdown dropdown;

    public bool DebugClick;

    private void Awake() {
        slider = GetComponent<Slider>();
        scrollbar = GetComponent<Scrollbar>();
        dropdown = GetComponent<Dropdown>();
    }

    void OnDisable(){
		pressed = false;
	}

	IEnumerator ReEnableCollider(){
		yield return new WaitForSeconds(0.5f);
		pressed = false;
		GetComponent<Collider>().enabled = true;
	}

    public void Update() {
        if (DebugClick) {
            PressButton(Vector3.zero);
            DebugClick = false;
        }
    }

    public void PressButton(Vector3 worldspacePos){

        if (slider != null) {
            float sliderwidth = GetComponent<BoxCollider>().size.x * transform.root.localScale.x;

            float rawPositionOffset = -((transform.position - (transform.right * sliderwidth)).x - transform.InverseTransformPoint(worldspacePos).x);

            float minvalueabs = Mathf.Abs(slider.minValue);
            if (minvalueabs == 0) {
                minvalueabs = slider.maxValue;
            }

            slider.value = ((rawPositionOffset / sliderwidth) * transform.root.localScale.x) * (slider.maxValue + minvalueabs);
            return;
        }

        if(scrollbar != null) {
            float sliderHeight = GetComponent<BoxCollider>().size.y;

            float rawPositionOffset = -((transform.position - (transform.up * sliderHeight)).y - (transform.InverseTransformPoint(worldspacePos).y + sliderHeight/10));

            scrollbar.value = ((rawPositionOffset / sliderHeight) * transform.root.localScale.x) * 3f;
            return;
        }

        

        if (!LongPress) {
            GetComponent<Collider>().enabled = false;
            pressed = true;
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.scrollDelta = -Vector2.up * 0.1f;
            GetComponent<Selectable>().Select();
            ExecuteEvents.Execute(gameObject, data, ExecuteEvents.scrollHandler);
            ExecuteEvents.Execute(gameObject, data, ExecuteEvents.pointerClickHandler);
            if (this.gameObject.activeInHierarchy) {
                StartCoroutine(ReEnableCollider());
            }
            else {
                pressed = false;
                GetComponent<Collider>().enabled = true;
            }
        }
        else {
            pressed = true;
            StartCoroutine(LongPressCheck());
        }

        if (dropdown != null) {
            StartCoroutine(AddDropdownOptionData());
        }

        return;
	}

    IEnumerator AddDropdownOptionData() {
        Toggle[] dropdownButtons = GetComponentsInChildren<Toggle>();
        for (int i = 0; i < dropdownButtons.Length; i++) {
            if(dropdownButtons[i].GetComponent<UGUIVRButton>() == null) {
                dropdownButtons[i].gameObject.AddComponent<UGUIVRButton>();
                dropdownButtons[i].gameObject.AddComponent<BoxCollider>();
                Vector3 colsize = GetComponent<BoxCollider>().size * 0.9f;
                colsize.y *= 0.75f;
                dropdownButtons[i].GetComponent<BoxCollider>().size = colsize;
            }
        }
        yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
    }

	IEnumerator LongPressCheck(){
		int frames = 0;
		while (frames < 90) {
			frames++;
            if (!pressed) {
                frames = 90;
            }
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
		if (pressed) {
			GetComponent<Collider>().enabled = false;
			pressed = true;
			PointerEventData data = new PointerEventData(EventSystem.current);
			pressed = true;
			data.scrollDelta = -Vector2.up*0.1f;
			GetComponent<Selectable>().Select(); 
			ExecuteEvents.Execute(gameObject, data, ExecuteEvents.scrollHandler);
			ExecuteEvents.Execute(gameObject, data, ExecuteEvents.pointerClickHandler);
			pressed = true;
			if(this.gameObject.activeInHierarchy){
				StartCoroutine(ReEnableCollider());
			}else{
				pressed = false;
				GetComponent<Collider>().enabled = true;
			}
		}
	}
}
