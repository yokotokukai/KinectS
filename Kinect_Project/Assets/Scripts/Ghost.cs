using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    //透過度
    float alpha;

    //透過度をいじる速さ
    float alphaSpd = 0.01f;

    //お化けムーブの速度
    float speed;

    //左右を決めるやつ
    float dir;

    //コサインカーブのやつ
    float cosSpd = 2f;
    float range = 0.2f;

    //色のやつ
    float red, green, blue;

    //移動ベクトル
    Vector3 velocity;

    //フェードの切り替え
    private bool trg = false;

    //消え始めるまでの時間
    public float time = 5;

    void Start()
    {
        red = GetComponent<SpriteRenderer>().color.r;
        green = GetComponent<SpriteRenderer>().color.g;
        blue = GetComponent<SpriteRenderer>().color.b;
        
        //お化けムーブの速さ
        speed = Random.Range(0.0005f, 0.001f);

        //左右を決める
        dir = Random.Range(0,2);
        dir = Mathf.Floor(dir);

        //消え始めるまでの時間
        time = 5;
    }

    void Update()
    {
        //色の設定のやつ
        GetComponent<SpriteRenderer>().color = new Color(red, green, blue, alpha);

        velocity.y = Mathf.Cos(Time.time * cosSpd) * range;
        
        //座標更新
        transform.position += velocity;

        //透過度の設定のやつ
        Alpha();

        //動きだすやつ
        Direction();

    }

    void Direction()
    {
        
        if (dir == 0)
        {
            velocity.x += -speed;
        }
        else
        {
            velocity.x += speed;
        }

        //座標更新
        transform.position += velocity;
        
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

        //消え始めるまでの時間が0になった時
        if (time <= 0)
        {
            trg = true;
        }

        if (trg == false)
        {
            alpha += alphaSpd;
        }
        else
        {
            alpha -= alphaSpd;
        }

        //見えなくなったら消す
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
