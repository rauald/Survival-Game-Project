using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    private bool isBurning = false;

    [SerializeField] private int damage;

    [SerializeField] private float damageTime;
    private float currentDamageTime;

    [SerializeField] private float durationTime;
    private float currentDurationTime;

    [SerializeField] private GameObject flame_prefab;   // ºÒ ºÙÀ¸¸é ÇÁ¸®ÆÕ »ý¼º
    private GameObject go_tempFlame;    // ÇÁ¸®ÆÕ ±×¸©

    public void StartBurning()
    {
        if (!isBurning)
        {
            go_tempFlame = Instantiate(flame_prefab, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            go_tempFlame.transform.SetParent(transform);
        }
        isBurning = true;
        currentDurationTime = durationTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(isBurning)
        {
            ElapseTime();
        }
    }

    private void ElapseTime()
    {
        if(isBurning)
        {
            currentDurationTime -= Time.deltaTime;

            if(currentDamageTime > 0)
            {
                currentDamageTime -= Time.deltaTime;
            }

            if(currentDamageTime <= 0)
            {
                Damage();
            }

            if(currentDurationTime <= 0)
            {
                Off();
            }
        }
    }

    private void Damage()
    {
        currentDamageTime = damageTime;
        GetComponent<StatusController>().DecreaseHP(damage);
    }

    private void Off()
    {
        isBurning = false;
        Destroy(go_tempFlame);
    }
}