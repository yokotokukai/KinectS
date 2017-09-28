using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    float alpha;
    float speed = 0.01f;
    float red, green, blue;

    //移動ベクトル
    Vector3 velocity;

    //フェードの切り替え
    private bool trg = false;

    public float time = 5;

    void Start()
    {
        red = GetComponent<SpriteRenderer>().color.r;
        green = GetComponent<SpriteRenderer>().color.g;
        blue = GetComponent<SpriteRenderer>().color.b;

        time = 5;
    }

    void Update()
    {
        //色の設定のやつ
        GetComponent<SpriteRenderer>().color = new Color(red, green, blue, alpha);

        if (Input.GetKey(KeyCode.RightArrow))
        {
            velocity.x += speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            velocity.x += -speed;
        }

        //座標更新
        transform.position += velocity;

        //動いてるx軸の方向を向くやつ
        Direction();

        //透過度の設定のやつ
        Alpha();
        
    }



    void Direction()
    {
        if (GetDirection().x > 0)
        {
            //右向き
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (GetDirection().x < 0)
        {
            //左向き
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    void Alpha()
    {

        time -= Time.deltaTime;

        if (time <= 0)
        {
            trg = true;
        }

        if (trg == false)
        {
            alpha += speed;
        }
        else
        {
            alpha -= speed;
        }

        if(trg == true && alpha <= 0)
        {
            Destroy(this.gameObject);
        }

    }

    public Vector3 GetDirection()
    {
        return velocity.normalized;
    }
}
