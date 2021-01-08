using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcess : MonoBehaviour
{
    public Material Glitch;

    float glitchBaseStrength = 0;
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
            switch (amount)
            {
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
            strengthUnbased = Mathf.Floor(Mathf.Lerp(0, 1, animTime) * 8) / 8;
            glitchInstance.SetFloat("_DisplaceStrength", strengthUnbased);
            glitchInstance.SetFloat("_SoftDisplaceStrength", strength);
            glitchInstance.SetFloat("_ColorStrength", strength);
            animTime = Mathf.Max(0, animTime - Time.deltaTime * 2);
        }
        glitchInstance.SetFloat("_DisplaceStrength", 0);
        glitchInstance.SetFloat("_SoftDisplaceStrength", glitchBaseStrength);
        glitchInstance.SetFloat("_ColorStrength", glitchBaseStrength);
    }

    private void OnDestroy()
    {
        EventSystem.onHealthUpdate -= OnTakeDamage;
    }
}
