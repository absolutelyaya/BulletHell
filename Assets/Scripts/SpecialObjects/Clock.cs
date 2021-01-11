using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{

    public GameObject MinuteHand;
    public GameObject HourHand;
    public List<string> HourNames = new List<string>();

    public float time;

    int phase = 0;

    void Start()
    {
        NextPhase();
    }

    void FixedUpdate()
    {
        if(phase < 13)
        {
            MinuteHand.transform.Rotate(new Vector3(0, 0, -1));
            HourHand.transform.Rotate(new Vector3(0, 0, -0.08333333333f));

            time = Mathf.Round(MinuteHand.transform.eulerAngles.z);

            if (Mathf.Round(MinuteHand.transform.eulerAngles.z) == 0)
            {
                NextPhase();
            }
        }
    }

    void NextPhase()
    {
        EventSystem.ShowTitle(HourNames[phase], 0.1f, 1f);
        phase++;
    }
}
