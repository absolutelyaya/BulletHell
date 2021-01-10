using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcess : MonoBehaviour
{
    public Material Glitch;
    public Material Screen;

    float glitchBaseStrength = 0;
    int lastHealth = 3;
    Material glitchInstance;
    Material screenInstance;
    RenderTexture renderTexture;

    private void OnEnable()
    {
        EventSystem.onHealthUpdate += OnTakeDamage;
        renderTexture = new RenderTexture(1920, 1080, 0);
        glitchInstance = new Material(Glitch);
        screenInstance = new Material(Screen);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, renderTexture, glitchInstance);
        Graphics.Blit(renderTexture, destination, screenInstance);
    }

    void OnTakeDamage(int amount)
    {
        if(lastHealth > amount)
        {
            StartCoroutine(DamageSequence());
            switch (amount)
            {
                case 0:
                    glitchBaseStrength = 0.2f;
                    break;
                case 1:
                    glitchBaseStrength = 0.1f;
                    break;
                case 2:
                    glitchBaseStrength = 0.05f;
                    break;
                case 3:
                    glitchBaseStrength = 0;
                    break;
            }
        }
        lastHealth = amount;
    }

    IEnumerator DamageSequence()
    {
        float animTime = 1;
        float strength;
        float strengthUnbased;
        glitchInstance.SetFloat("_DisplaceStrength", 1);
        while(animTime > 0)
        {
            yield return new WaitForEndOfFrame();
            strength = Mathf.Floor(Mathf.Lerp(glitchBaseStrength, 1, animTime) * 8) / 8;
            if (glitchBaseStrength < 0.2f)
                strengthUnbased = Mathf.Floor(Mathf.Lerp(0, 1, animTime) * 8) / 8;
            else
                strengthUnbased = Mathf.Floor(Mathf.Lerp(0.5f, 1, animTime) * 8) / 8;
            glitchInstance.SetFloat("_DisplaceStrength", strengthUnbased);
            glitchInstance.SetFloat("_SoftDisplaceStrength", strength);
            glitchInstance.SetFloat("_ColorStrength", strength);
            animTime = Mathf.Max(0, animTime - Time.deltaTime * 2);
        }
        if (glitchBaseStrength < 0.2f) 
            glitchInstance.SetFloat("_DisplaceStrength", 0);
        glitchInstance.SetFloat("_SoftDisplaceStrength", glitchBaseStrength);
        glitchInstance.SetFloat("_ColorStrength", glitchBaseStrength);
    }

    private void OnDestroy()
    {
        EventSystem.onHealthUpdate -= OnTakeDamage;
    }
}
