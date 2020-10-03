using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebug : MonoBehaviour
{
    [SerializeField] private AudioSource aSrc1;

    [SerializeField] private AudioSource aSrc2;

    [SerializeField] private AudioSource aSrc2_adjusted;



    // Start is called before the first frame update
    void Start()
    {
        aSrc2_adjusted.pitch = Mathf.Pow(1.05946f, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            aSrc1.PlayOneShot(aSrc1.clip);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            aSrc2.PlayOneShot(aSrc2.clip);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            aSrc2_adjusted.PlayOneShot(aSrc2_adjusted.clip);
        }
    }
}
