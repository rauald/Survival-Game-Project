using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    // Ǯ ü�� (������ 1)
    [SerializeField]
    private int hp;

    // ��ü ���� �ð�
    [SerializeField]
    private float destroyTime;
    // ���ȷ� ����
    [SerializeField]
    private float force;

    // Ÿ�� ȿ��
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxColliders;

    [SerializeField]
    private string hit_sound;


    // Start is called before the first frame update
    void Start()
    {
        rigidbodys = this.transform.GetComponentsInChildren<Rigidbody>();
        boxColliders = transform.GetComponentsInChildren<BoxCollider>();
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
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f); // ���� ���� / ��ġ / �Ÿ�
            boxColliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
}