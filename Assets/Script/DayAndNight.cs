using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] private float secondPreRealTimeSecound;    // ���� ������ 100�� = ������ 1��

    [SerializeField] private float fogDensityCalc;  // ������ ����

    [SerializeField] private float nightFogDensity; // �� ������ Fog �е�
    private float dayFogDensity;    // �� ������ fog �е�
    private float currentFogDensity;    // ���


    // Start is called before the first frame update
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPreRealTimeSecound * Time.deltaTime);

        if (transform.eulerAngles.x >= 170)
        {
            GameManager.isNight = true;
        }
        else if (transform.eulerAngles.x <= 10)
        {
            GameManager.isNight = false;
        }

        if(GameManager.isNight)
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += (0.1f * fogDensityCalc * Time.deltaTime);
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= (0.1f * fogDensityCalc * Time.deltaTime);
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}