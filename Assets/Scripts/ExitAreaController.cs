using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ExitAreaController : MonoBehaviour
{
    public GameController gameController;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.NextFloor();
        }
    }
}