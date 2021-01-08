using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcess : MonoBehaviour
{
    public Material Glitch;

    int lastHealth = 3;
    Material glitchInstance;

    private void OnEnable()
    {
        EventSystem.onHealthUpdate += OnTakeDamage;
        glitchInstance = new Material(Glitch);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, glitchInstance);
    }

    void OnTakeDamage(int amount)
    {
        if(lastHealth > amount)
        {
            StartCoroutine(DamageSequence());
        }
        lastHealth = amount;
    }

    IEnumerator DamageSequence()
    {
        float strength = 1;
        glitchInstance.SetFloat("_DisplaceStrength", 1);
        while(strength > 0.005)
        {
            yield return new WaitForEndOfFrame();
            strength =Mathf.Lerp(strength, 0, Time.deltaTime * 2);
            glitchInstance.SetFloat("_DisplaceStrength", Mathf.Floor(strength * 16) / 16);
        }
        glitchInstance.SetFloat("_DisplaceStrength", 0);
    }

    private void OnDestroy()
    {
        EventSystem.onHealthUpdate -= OnTakeDamage;
    }
}
