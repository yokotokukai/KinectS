using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spone : MonoBehaviour {

    public GameObject ghost;
    public GameObject batsR;
    public GameObject batsL;
    public GameObject Pumpkin;
    public GameObject Candy;

    public bool trgGhost = false;
    public bool trgBatsR = false;
    public bool trgBatsL = false;
    public bool trgPumpkin = false;
    public bool trgCandy = false;

    private float x = 0;
    private float y = 0;
    private float z = 0;

    //n秒ごとに実行する
    public float sTime = 2;
    public float time;

    // Use this for initialization
    void Start()
    {
        time = 2;
    }

    // Update is called once per frame
    void Update()
    {

        time -= Time.deltaTime;
        if (time <= 0.0)
        {

            if (trgGhost == true)
            {

                for (int i = 0; i <= 2; i++)
                {

                    x = Random.Range(-120f, 120f);
                    y = Random.Range(-60f, 60f);
                    z = 149f;

                    Instantiate(ghost, new Vector3(x, y, z), Quaternion.identity);
                }

                trgGhost = false;
            }

            if (trgBatsR == true)
            {

                for (int i = 0; i <= 2; i++)
                {

                    x = Random.Range(-120f, 120f);
                    y = Random.Range(-60f, 60f);
                    z = 149f;

                    Instantiate(batsR, new Vector3(x, y, z), Quaternion.identity);
                }

                trgBatsR = false;
            }

            if (trgBatsL == true)
            {

                for (int i = 0; i <= 2; i++)
                {

                    x = Random.Range(-120f, 120f);
                    y = Random.Range(-60f, 60f);
                    z = 149f;

                    Instantiate(batsL, new Vector3(x, y, z), Quaternion.identity);   
                }

                trgBatsL = false;
            }
            
            time = sTime;
        }
    }
}
