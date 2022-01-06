using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(List<Transform> points)
    {
        var pointsArr = new Transform[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            pointsArr[i] = points[i];
        }

        lr.positionCount = pointsArr.Length;
        this.points = pointsArr;
    }

    // Update is called once per frame
    void Update()
    {
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
        
    }
}
