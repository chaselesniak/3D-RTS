using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public float Gold;
    public float maxGold;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (Gold >= maxGold)
        {
            Gold = maxGold;
        }
    }
}
