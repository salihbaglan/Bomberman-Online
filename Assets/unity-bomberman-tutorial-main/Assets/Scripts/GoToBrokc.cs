using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GoToBrokc : MonoBehaviour
{

    private Collider2D col;

   public void Ghost()
    {
        GetComponent<Collider2D>().isTrigger = false;
    }
}

