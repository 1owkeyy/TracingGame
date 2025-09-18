using UnityEngine;
using System.Collections;

public class HandPrompt : MonoBehaviour
{
    public Transform[] pathPoints;
    public float speed = 3f;

    private int currentPointIndex = 0;

    public void StartMoving(Transform[] points)
    {
        pathPoints = points;
        currentPointIndex = 0;
        StopAllCoroutines();
        StartCoroutine(MoveAlongPath());
    }

    IEnumerator MoveAlongPath()
    {
        while (true)
        {
            if (pathPoints.Length == 0) yield break;

            Transform target = pathPoints[currentPointIndex];
            while (Vector3.Distance(transform.position, target.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                yield return null;
            }

            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Length)
                currentPointIndex = 0; // Loop the animation
            yield return null;
        }
    }
}
