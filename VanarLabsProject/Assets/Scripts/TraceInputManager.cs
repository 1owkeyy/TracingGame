using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TraceInputManager : MonoBehaviour
{
    public SegmentManager segmentManager;
    public GameObject dotHitParticlePrefab; // Particle prefab for dot hit burst
    public AudioSource dotHitAudioSource;   // AudioSource with dot hit clip
    public AudioSource segmentCompleteAudioSource; // AudioSource for segment complete sound
    public AudioClip segmentCompleteAudioClip;     // Clip for segment complete sound
    public float segmentCompleteDelay = 0.5f; // delay before segment complete is called

    private List<Dot> currentDots = new List<Dot>();
    private int nextDotIndex = 0;
    private LineRenderer lineRenderer;
    private List<Vector3> linePoints = new List<Vector3>();

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        // Configure LineRenderer for cozy look
        lineRenderer.positionCount = 0;
        lineRenderer.widthCurve = new AnimationCurve(
            new Keyframe(0, 0.2f),
            new Keyframe(1, 0.1f)
        );
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0.6f, 0.8f, 0.8f); // pastel pink
        lineRenderer.endColor = new Color(1f, 0.9f, 0.5f, 0.7f);   // pastel yellow
        lineRenderer.numCapVertices = 8; // rounded caps
    }

    void Update()
    {
        if (segmentManager == null) return;
        if (currentDots.Count == 0)
        {
            currentDots.Clear();
            foreach (var dotGO in segmentManager.activeDots)
            {
                Dot dotComp = dotGO.GetComponent<Dot>();
                if (dotComp != null)
                    currentDots.Add(dotComp);
            }
            nextDotIndex = 0;
            lineRenderer.positionCount = 0;
            linePoints.Clear();
        }
        HandleInput();
    }

    void HandleInput()
    {
        Vector3 inputPosition = Vector3.zero;
        bool inputActive = false;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = Camera.main.ScreenToWorldPoint(touch.position);
            inputPosition.z = 0f;
            inputActive = true;
        }
        else if (Input.GetMouseButton(0))
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            inputPosition.z = 0f;
            inputActive = true;
        }
        else
        {
            if (linePoints.Count > 0)
            {
                lineRenderer.positionCount = 0;
                linePoints.Clear();
            }
            return;
        }
        if (linePoints.Count == 0 || Vector3.Distance(linePoints[linePoints.Count - 1], inputPosition) > 0.02f)
        {
            linePoints.Add(inputPosition);
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPosition(linePoints.Count - 1, inputPosition);
        }
        if (nextDotIndex < currentDots.Count)
        {
            Dot nextDot = currentDots[nextDotIndex];
            float distance = Vector3.Distance(inputPosition, nextDot.transform.position);
            if (distance < 0.3f)
            {
                nextDot.Highlight();
                nextDotIndex++;
                // Particle and sound feedback on dot hit
                if (dotHitParticlePrefab != null)
                    Instantiate(dotHitParticlePrefab, nextDot.transform.position, Quaternion.identity);
                // Progressive pitch audio for dot hit
                if (dotHitAudioSource != null)
                {
                    float basePitch = 1.0f;
                    float maxPitch = 1.3f; // Adjust as needed
                    int totalDots = currentDots.Count;
                    int currentDot = nextDotIndex; // Already incremented
                    float t = (totalDots > 1) ? Mathf.Clamp01((float)(currentDot - 1) / (totalDots - 1)) : 0f;
                    dotHitAudioSource.pitch = Mathf.Lerp(basePitch, maxPitch, t);
                    dotHitAudioSource.Play();
                }
                if (nextDotIndex >= currentDots.Count)
                {
                    StartCoroutine(DelayedSegmentComplete());
                }
            }
        }
    }

    IEnumerator DelayedSegmentComplete()
    {
        yield return new WaitForSeconds(segmentCompleteDelay);
        // Play segment complete audio cue
        if (segmentCompleteAudioSource != null && segmentCompleteAudioClip != null)
        {
            segmentCompleteAudioSource.PlayOneShot(segmentCompleteAudioClip);
        }
        segmentManager.SegmentComplete();
        currentDots.Clear();
        nextDotIndex = 0;
        lineRenderer.positionCount = 0;
        linePoints.Clear();
        // Reset pitch optionally
        if (dotHitAudioSource != null)
            dotHitAudioSource.pitch = 1.0f;
    }
}
