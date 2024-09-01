using System.Collections;
using UnityEngine;

public class SimpleFlash : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    private bool flashing = false;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private float timer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(flashMaterial != null, this + ": Flash Material Not Set!");
        Debug.Assert(spriteRenderer != null, this + ": Sprite Renderer not with Simple Flash Script");
    }

    public void Flash(float duration) {
        if (flashing) {
            return;
        }
        flashing = true;
        originalMaterial = spriteRenderer.material;
        StartCoroutine(Flashing(duration));
    }

    private IEnumerator Flashing(float duration) {
        timer = duration;
        while (timer > 0) {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material = originalMaterial;
            yield return new WaitForSeconds(0.1f);
            timer -= 0.2f;
        }
        flashing = false;
    }
}
