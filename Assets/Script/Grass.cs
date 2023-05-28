using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    // 풀 체력 (보통은 1)
    [SerializeField]
    private int hp;

    // 객체 삭제 시간
    [SerializeField]
    private float destroyTime;
    // 폭팔력 세기
    [SerializeField]
    private float force;

    // 타격 효과
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    [SerializeField]
    private Item item_leaf;
    [SerializeField]
    private int leafCount;
    private Inventory theInven;

    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxColliders;

    [SerializeField]
    private string hit_sound;


    // Start is called before the first frame update
    void Start()
    {
        rigidbodys = this.transform.GetComponentsInChildren<Rigidbody>();
        boxColliders = transform.GetComponentsInChildren<BoxCollider>();
        theInven = FindObjectOfType<Inventory>();
    }

    public void Damage()
    {
        hp--;

        Hit();

        if(hp <= 0)
        {
            Destruction();
        }
    }

    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_sound);

        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);
        Destroy(clone, destroyTime);
    }

    private void Destruction()
    {
        theInven.AcquireItem(item_leaf, leafCount);
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f); // 폭발 세기 / 위치 / 거리
            boxColliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
}