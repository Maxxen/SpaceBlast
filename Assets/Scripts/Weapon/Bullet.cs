using Assets.Scripts.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public string shooterTag;
    public int damage = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Map" )
        {
            gameObject.SetActive(false);
            return;
        }
        if (other.tag == "Enemy" && shooterTag == "Player")
        {
            gameObject.SetActive(false);
            other.GetComponent<IDamageable>().TakeDamage(5);
        }
        else if (other.tag == "Player" && shooterTag == "Enemy")
        {
            gameObject.SetActive(false);
            //
        }
    }
}
