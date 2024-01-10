using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 50;
    float radius;
    LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        radius = TowerStats.Instance.range;
    }

    private void Update()
    {
        radius = TowerStats.Instance.range;
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x;
        float y;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
}
