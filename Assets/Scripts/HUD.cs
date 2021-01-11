using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public Slider HealthBar;
    public Image DamagePopup;
    public Sprite DeathPopup;
    public SpriteRenderer BlackoutSquare;
    public Camera GameOverCamera;
    public TextMeshProUGUI Title;

    Coroutine damagePopUpCoroutine;
    Coroutine TitleCoroutine;

    private void Start()
    {
        EventSystem.onHealthUpdate += HealthUpdate;
        EventSystem.onPlayerDeath += PlayerDeath;
        EventSystem.onTitle += (string text, float charDelay, float displayTime) => 
        {
            if (TitleCoroutine != null) StopCoroutine(TitleCoroutine);
            TitleCoroutine = StartCoroutine(TypeTitle(text, charDelay, displayTime));
        };
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

    private void PlayerDeath()
    {
        DamagePopup.sprite = DeathPopup;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1);
        GameOverCamera.gameObject.SetActive(true);
        float time = 0;
        yield return new WaitForSeconds(0.5f);
        while(time < 1)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime * 2;
            BlackoutSquare.color = new Color(0, 0, 0, Mathf.Floor(time * 8) / 8);
        }
    }

    IEnumerator TypeTitle(string text, float charDelay, float displayTime)
    {
        for (int i = 0; i < text.Length; i++)
        {
            Title.text += text[i];
            yield return new WaitForSeconds(charDelay);
        }
        yield return new WaitForSeconds(displayTime);
        for (int i = 0; i < text.Length; i++)
        {
            Title.text = Title.text.Substring(0, Title.text.Length - 1);
            yield return new WaitForSeconds(charDelay / 2);
        }
        TitleCoroutine = null;
    }

    private void OnDestroy()
    {
        EventSystem.onHealthUpdate -= HealthUpdate;
        EventSystem.onPlayerDeath -= PlayerDeath;
    }
}
