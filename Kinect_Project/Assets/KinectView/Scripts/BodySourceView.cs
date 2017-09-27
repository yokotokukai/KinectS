using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityEngine.SceneManagement;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    public float timeElapsed = 0;

    private Vector3[] bodyPos = new Vector3[25];
    private ulong active = 0;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    public bool bodyTrg = false;
    
    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
              }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                active = 0;
                continue;
            }

            if (active != body.TrackingId && active != 0)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
                active = body.TrackingId;
            }
        }
        if (bodyTrg == true)
        {

            BatsCreateR();

            BatsCreateL();

            GhostCreate();

            GameEnd();

        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;


        }

        bodyTrg = true;
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {

        Transform jointObj;
        Vector3[] joypos = new Vector3[24];
        int joycnt = 0;
        Kinect.JointType[] joytype = new Kinect.JointType[24];


        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            


            jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            bodyPos[(int)jt] = jointObj.position;
            joycnt++;

            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
        
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    void BatsCreateR()
    {
        //がおーポーズ(右手が上)
        if (bodyPos[(int)Kinect.JointType.HandRight].x >= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandRight].y >= bodyPos[(int)Kinect.JointType.Head].y &&
            bodyPos[(int)Kinect.JointType.HandRight].y <= bodyPos[(int)Kinect.JointType.Head].y + 1 &&
            bodyPos[(int)Kinect.JointType.HandLeft].x <= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandLeft].y <= bodyPos[(int)Kinect.JointType.SpineMid].y)
        {
            FindObjectOfType<Spone>().trgBatsR = true;
            //Debug.Log("いいぞ。");
        }
        else
        {
            FindObjectOfType<Spone>().trgBatsR = false;
        }
    }

    void BatsCreateL()
    {
        //がおーポーズ(左手が上)
        if (bodyPos[(int)Kinect.JointType.HandLeft].x <= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandLeft].y >= bodyPos[(int)Kinect.JointType.Head].y &&
            bodyPos[(int)Kinect.JointType.HandLeft].y <= bodyPos[(int)Kinect.JointType.Head].y + 1 &&
            bodyPos[(int)Kinect.JointType.HandRight].x >= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandRight].y <= bodyPos[(int)Kinect.JointType.SpineMid].y)
        {
            FindObjectOfType<Spone>().trgBatsL = true;
            //Debug.Log("いいぞ。2");
        }
        else
        {
            FindObjectOfType<Spone>().trgBatsL = false;
        }
    }

    void GhostCreate()
    {
        //おばけポーズ
        if (bodyPos[(int)Kinect.JointType.HandRight].x >= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandRight].y >= bodyPos[(int)Kinect.JointType.SpineMid].y &&
            bodyPos[(int)Kinect.JointType.HandLeft].x <= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandLeft].y >= bodyPos[(int)Kinect.JointType.SpineMid].y &&
            bodyPos[(int)Kinect.JointType.HandRight].y <= bodyPos[(int)Kinect.JointType.Neck].y &&
            bodyPos[(int)Kinect.JointType.HandLeft].y <= bodyPos[(int)Kinect.JointType.Neck].y &&
            bodyPos[(int)Kinect.JointType.HandRight].x <= bodyPos[(int)Kinect.JointType.ShoulderRight].x &&
            bodyPos[(int)Kinect.JointType.HandLeft].x >= bodyPos[(int)Kinect.JointType.ShoulderLeft].x &&
            bodyPos[(int)Kinect.JointType.HandTipRight].y <= bodyPos[(int)Kinect.JointType.HandRight].y + 1 &&
            bodyPos[(int)Kinect.JointType.HandTipLeft].y <= bodyPos[(int)Kinect.JointType.HandLeft].y + 1)
        {
            FindObjectOfType<Spone>().trgGhost = true;
            //Debug.Log("いいぞ。3");
        }
        else
        {
            FindObjectOfType<Spone>().trgGhost = false;
        }
    }

    void ShineCreate()
    {
        //威嚇(?)ポーズ
        if (bodyPos[(int)Kinect.JointType.HandRight].x >= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandLeft].x <= bodyPos[(int)Kinect.JointType.SpineBase].x &&
            bodyPos[(int)Kinect.JointType.HandRight].y >= bodyPos[(int)Kinect.JointType.Head].y &&
            bodyPos[(int)Kinect.JointType.HandLeft].y >= bodyPos[(int)Kinect.JointType.Head].y )
        {
            Debug.Log("いいぞ。");
        }
    }

    void GameEnd()
    {
        //横に手を広げている時
        if (bodyPos[(int)Kinect.JointType.ShoulderLeft].x > bodyPos[(int)Kinect.JointType.HandLeft].x + 2 &&
            bodyPos[(int)Kinect.JointType.ShoulderRight].x < bodyPos[(int)Kinect.JointType.HandRight].x - 2 &&
           (bodyPos[(int)Kinect.JointType.HandLeft].y < bodyPos[(int)Kinect.JointType.SpineShoulder].y + 2 &&
            bodyPos[(int)Kinect.JointType.HandLeft].y > bodyPos[(int)Kinect.JointType.SpineShoulder].y - 2) &&
           (bodyPos[(int)Kinect.JointType.HandRight].y < bodyPos[(int)Kinect.JointType.SpineShoulder].y + 2 &&
            bodyPos[(int)Kinect.JointType.HandRight].y > bodyPos[(int)Kinect.JointType.SpineShoulder].y - 2))
        {
            timeElapsed += Time.deltaTime;
        }
        else
        {
            timeElapsed = 0;
        }

        //約3秒経過したらメインシーンへ
        if (timeElapsed > 3)                                               //約3秒経過したらメインシーンへ 
        {
            timeElapsed = 0;
            SceneManager.LoadScene("Result");
        }
    }
}
