using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;



public class PlatBehave : MonoBehaviour
{

    public int score = 0;
    public GameObject handler;
    public Transform ballPos;
    
    public float last_pos = -10;

    public bool paused;
    public Text ScoreText;

    private int spawned = 0;

    List<Transform> passed = new List<Transform>();


    System.Random rnd = new System.Random();
    

    // Start is called before the first frame update

    void TeleportPlat(Transform plat)
    {
        var Rend = plat.GetComponent<Renderer>();
        int score = int.Parse(ScoreText.text);

        plat.position = new Vector3(0, last_pos - 10, 0);
        /*Color col = new Color(255, 0, 0);
        Rend.material.SetColor("_Color", col);*/
        if (plat.childCount > 0)
        {
            for (int i = 0; i < plat.childCount; i++)
            {
                Transform zone = plat.transform.GetChild(i);
                zone.gameObject.SetActive(false);
            }
            if ((UnityEngine.Random.Range(0, 50) < score) && (spawned > 3)) {
                for (int i = 0; i < Mathf.Min(100, score / 30); i++)
                {
                    Transform zone = plat.transform.GetChild(UnityEngine.Random.Range(0, plat.childCount));
                    zone.gameObject.SetActive(true);
                }
            }
        }
        last_pos = last_pos - 10;
        spawned += 1;
        plat.eulerAngles = new Vector3(plat.eulerAngles.x, plat.eulerAngles.y, rnd.Next(0, 300));

    }

    public void PlatRemoveZones()
    {
        for (int i = 0; i < handler.transform.childCount; i++)
        {
            Transform plat = handler.transform.GetChild(i);
            if (plat.tag != "Start_platform")
            {
                // ������ ��� ������� �� ����������
                for (int j = 0; j < plat.childCount; j++)
                {
                    Transform zone = plat.transform.GetChild(j);
                    zone.gameObject.SetActive(false);
                }
            }
        }
    }
    public void RestartPlat()
    {
        for (int i = 0; i < handler.transform.childCount; i++)
        {
            Transform plat = handler.transform.GetChild(i);
            if (plat.tag != "Start_platform")
            {
                plat.position = new Vector3(plat.position.x, plat.position.y - last_pos + Mathf.Max(-70, last_pos), plat.position.z);
            }

            
        }
        last_pos = -70;
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {

            for (int i = 0; i < handler.transform.childCount; i++)
            {
                Transform plat = handler.transform.GetChild(i);

                if (plat.position.y - 20 > ballPos.position.y)
                {

                    if (plat.tag != "Start_platform")
                    {
                        passed.Add(plat);
                        plat.position = new Vector3(plat.position.x, plat.position.y - 10000, plat.position.z);
                    }

                }
            }
            if ((last_pos - ballPos.position.y > -25))
            {
                int dind = rnd.Next(0, passed.Count);
                TeleportPlat(passed[dind]);
                passed.RemoveAt(dind);
            }
        }
    }
}
