using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TimedLevelSwitch : MonoBehaviour
{
    public Renderer[] renderersToFade;
    public float fadeTime = 2f;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Color fadecol = new Color(1f, 1f, 1f, 1f);
        while (fadeTime > -0.1f) {
            fadeTime -= Time.deltaTime;
            fadecol.a -= Time.deltaTime / 2f;
            renderersToFade[0].transform.localScale *= 1f + (Time.deltaTime / 10f);
            renderersToFade[1].transform.localScale *= 1f + (Time.deltaTime / 10f);
            renderersToFade[0].material.color = fadecol;
            renderersToFade[1].material.color = fadecol;
            yield return null;
        }

        SceneManager.LoadScene("scene");
    }

    void Update()
    {
        
    }
}
