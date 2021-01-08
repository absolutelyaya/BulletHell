using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Slider HealthBar;
    public Image DamagePopup;

    Coroutine damagePopUpCoroutine;

    private void Start()
    {
        EventSystem.onHealthUpdate += HealthUpdate;
    }

    void HealthUpdate(int health)
    {
        if(HealthBar.value > health)
        {
            if(damagePopUpCoroutine == null)
            {
                damagePopUpCoroutine = StartCoroutine(damagePopUp(health));
            }
            else
            {
                StopCoroutine(damagePopUpCoroutine);
                damagePopUpCoroutine = StartCoroutine(damagePopUp(health));
            }
        }
        HealthBar.value = health;
    }

    IEnumerator damagePopUp(int health)
    {
        float alpha = 1;
        float time = 0;
        DamagePopup.color = new Color(1, 1, 1, alpha);
        while(time < 1)
        {
            yield return new WaitForEndOfFrame();
            DamagePopup.rectTransform.position = new Vector3(Random.Range(-(3 - health) / 4f, (3 - health) / 4f), 0, 0);
            time += Time.deltaTime;
        }
        while(alpha > 0)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            alpha = Mathf.Lerp(1, 0, time - 1f);
            DamagePopup.rectTransform.position = new Vector3(Random.Range(-(3 - health) / 4f, (3 - health) / 4f), 0, 0);
            DamagePopup.color = new Color(1, 1, 1, alpha);
        }
        DamagePopup.color = new Color(1, 1, 1, 0);
        damagePopUpCoroutine = null;
    }

    private void OnDestroy()
    {
        EventSystem.onHealthUpdate -= HealthUpdate;
    }
}
