using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDemonSpawner : MonoBehaviour {
    public GameObject greenDemonPrefab;
    public Light warningLight;

    private float spawnDelay = 8f;
    private float spawnProgress = 0f;

    private Color finalColor = new Color(1f, 0.4f, 0f, 1f);
    private Color lerpColor = new Color(0f, 1f, 0f, 1f);

    protected IEnumerator Start() {
        // Delay spawning and animate the warning lights
        while (spawnProgress < spawnDelay) {
            spawnProgress += Time.deltaTime;

            var lightAmount = (Mathf.Cos(spawnProgress * Mathf.PI*2) + 1) * 0.5f * spawnProgress / spawnDelay;

            // For the last second, make the color more aggressive
            var targetColor = spawnDelay - spawnProgress < 0.5f ? finalColor : lerpColor;
            warningLight.color = Color.Lerp(Color.black, targetColor, lightAmount);

            yield return null;
        }
        warningLight.color = finalColor;

        // Send it
        Instantiate(greenDemonPrefab, transform.position, transform.rotation, null);

        // Fade out spawner
        var destroyDelay = 1f;
        while (destroyDelay > 0) {
            destroyDelay -= Time.deltaTime/2f;
            warningLight.color = Color.Lerp(finalColor, Color.black, 1-destroyDelay);
            yield return null;
        }
        GameObject.Destroy(gameObject);
    }
}
