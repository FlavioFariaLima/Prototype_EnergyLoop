using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyController : MonoBehaviour
{
    MeshRenderer rend;

    public float growValue;

    public bool playAnim = false;

    bool aux = false;

    public Material growMat;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();

        growValue = rend.material.GetFloat("Vector1_386C6BEE");
    }

    public void PlayIvyAnim(bool play)
    {
        playAnim = play;
    }

    public void ResetVars()
    {
        //GetComponent<Renderer>().material = growMat;

        rend.material.SetFloat("Vector1_386C6BEE", -0.086f);
        growValue = rend.material.GetFloat("Vector1_386C6BEE");
        aux = false;
        playAnim = false;

        //windBendStrengt = 0;
        //increaseWind = false;
    }

    void Update()
    {
        if (playAnim)
        {
            if (growValue <= 1)
            {
                growValue += 1.3f * Time.deltaTime;
                rend.material.SetFloat("Vector1_386C6BEE", growValue);

                if (growValue >= 0.75 && aux == false)
                {
                    aux = true;
                }
            }
            else
            {
                playAnim = false;
            }

        }
    }
}