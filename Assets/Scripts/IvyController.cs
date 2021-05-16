using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IvyController : MonoBehaviour
{
    MeshRenderer rend;

    public float growValue;

    public bool playAnim = false;

    bool aux = false;


    private ParticleSystem leafsParticles;

    public Material growMat;

    public Material windMat;

    [SerializeField] float windBendStrengt;

    [SerializeField] float windBendStrengtMax;


    public bool increaseWind = false;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();

        growValue = rend.material.GetFloat("Vector1_386C6BEE");

        leafsParticles = Resources.Load<ParticleSystem>("LeafsParticles");
    }

    public void PlayIvyAnim(bool play)
    {
        playAnim = play;
    }

    public void ResetVars()
    {
        GetComponent<Renderer>().material = growMat;
        rend.material.SetFloat("Vector1_386C6BEE", -0.086f);
        growValue = rend.material.GetFloat("Vector1_386C6BEE");
        aux = false;
        playAnim = false;

        windBendStrengt = 0;
        increaseWind = false;
    }

    public void PlayParticles()
    {
        Vector3 spawnPos = new Vector3(transform.parent.parent.transform.position.x, transform.parent.parent.transform.position.y, transform.parent.parent.transform.position.z - 0.2f);

        if(growValue > .4)
        Instantiate(leafsParticles, spawnPos, transform.rotation * Quaternion.Euler(169f, 0, 0f));

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
               // increaseWind = true;
                //GetComponent<Renderer>().material = windMat;
                playAnim = false;
            }

        }
                else if(increaseWind == true)
        {
            if(windBendStrengt <= windBendStrengtMax)
            {
                windBendStrengt += 3f * Time.deltaTime;
                rend.material.SetFloat("Vector1_693BBF99", windBendStrengt);
            }
        }
    }
}