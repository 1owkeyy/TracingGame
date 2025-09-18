using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SegmentManager : MonoBehaviour
{
    public GameObject dotPrefab;
    public GameObject handPromptPrefab;
    // List of segments with tracing dots
    private List<List<Vector2>> segments = new List<List<Vector2>>();
    public List<GameObject> activeDots = new List<GameObject>();
    private GameObject currentHandPrompt;
    private int currentSegmentIndex = 0;
    // Visuals for segments: empty (ghost) and filled (solid)
    public List<GameObject> emptySegmentVisuals;
    public List<GameObject> filledSegmentVisuals;
    // Confetti particle system
    public ParticleSystem confettiParticleSystem;
    // Good Job text UI gameobject
    public GameObject goodJobText;
    // Audio sources and clips for end of all segments
    public AudioSource endAudioSource1;
    public AudioSource endAudioSource2;
    public AudioClip endAudioClip1;
    public AudioClip endAudioClip2;

    void Start()
    {
        // Define segments dot positions (example for letter A)
        segments.Add(new List<Vector2> { new Vector2(-1.87f, -2.15f), new Vector2(-1.52f, -1.14f), new Vector2(-1.17f, -0.05f), new Vector2(-0.86f, 0.98f), new Vector2(-0.39f, 1.95f) });
        segments.Add(new List<Vector2> { new Vector2(0.35f, 1.95f), new Vector2(0.56f, 0.98f), new Vector2(0.99f, -0.05f), new Vector2(1.28f, -1.14f), new Vector2(1.73f, -2.15f) });
        segments.Add(new List<Vector2> { new Vector2(-0.84f, -1.15f), new Vector2(-0.06f, -1.15f), new Vector2(0.84f, -1.15f) });
        // Enable empty visuals and disable filled visuals at start
        foreach (var obj in emptySegmentVisuals)
            obj.SetActive(true);
        foreach (var obj in filledSegmentVisuals)
            obj.SetActive(false);
        // Hide confetti particle system and good job text at start
        if (confettiParticleSystem != null)
            confettiParticleSystem.gameObject.SetActive(false);
        if (goodJobText != null)
            goodJobText.SetActive(false);
        SpawnSegmentDots(currentSegmentIndex);
    }

    void SpawnSegmentDots(int segmentIndex)
    {
        ClearDotsAndHand();
        if (segmentIndex < 0 || segmentIndex >= segments.Count) return;
        List<Vector2> points = segments[segmentIndex];
        for (int i = 0; i < points.Count; i++)
        {
            GameObject dot = Instantiate(dotPrefab, points[i], Quaternion.identity);
            activeDots.Add(dot);
        }
        SpawnHandPrompt(points);
    }

    void SpawnHandPrompt(List<Vector2> points)
    {
        if (currentHandPrompt != null)
            Destroy(currentHandPrompt);
        currentHandPrompt = Instantiate(handPromptPrefab, points[0], Quaternion.identity);
        Transform[] pathPoints = new Transform[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            GameObject tempPoint = new GameObject("PathPoint_" + i);
            tempPoint.transform.position = points[i];
            pathPoints[i] = tempPoint.transform;
        }
        currentHandPrompt.GetComponent<HandPrompt>().StartMoving(pathPoints);
    }

    public void SegmentComplete()
    {
        // Hide old empty visual for this segment
        if (currentSegmentIndex >= 0 && currentSegmentIndex < emptySegmentVisuals.Count)
            emptySegmentVisuals[currentSegmentIndex].SetActive(false);
        // Show filled visual and play pop animation
        if (currentSegmentIndex >= 0 && currentSegmentIndex < filledSegmentVisuals.Count)
        {
            GameObject filledObj = filledSegmentVisuals[currentSegmentIndex];
            filledObj.SetActive(true);
            StartCoroutine(PlayPopAnimation(filledObj));
        }
        currentSegmentIndex++;
        if (currentSegmentIndex >= segments.Count)
        {
            Debug.Log("All segments done!");
            ClearDotsAndHand();
            // Play confetti effect and show good job text
            PlayConfettiAndShowGoodJob();
            // Play end-audio clips one after another
            StartCoroutine(PlayEndAudioClips());
            return;
        }
        SpawnSegmentDots(currentSegmentIndex);
    }

    IEnumerator PlayPopAnimation(GameObject obj)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * 1.5f;
        obj.transform.localScale = startScale;
        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = endScale;
    }

    void ClearDotsAndHand()
    {
        foreach (var dot in activeDots)
            Destroy(dot);
        activeDots.Clear();
        if (currentHandPrompt != null)
        {
            Destroy(currentHandPrompt);
            currentHandPrompt = null;
        }
    }

    void PlayConfettiAndShowGoodJob()
    {
        if (confettiParticleSystem != null && goodJobText != null)
        {
            confettiParticleSystem.gameObject.SetActive(true);
            confettiParticleSystem.Play();
            StartCoroutine(ShowGoodJobAnimation());
        }
    }

    IEnumerator ShowGoodJobAnimation()
    {
        goodJobText.transform.localScale = Vector3.zero;
        goodJobText.SetActive(true);
        float duration = 0.7f;  // extended duration for bigger pop
        float elapsed = 0f;
        Vector3 targetScale = Vector3.one * 6f; // increased size 1.5x
        while (elapsed < duration)
        {
            float scale = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            goodJobText.transform.localScale = targetScale * scale;
            elapsed += Time.deltaTime;
            yield return null;
        }
        goodJobText.transform.localScale = targetScale;
    }

    IEnumerator PlayEndAudioClips()
    {
        if (endAudioSource1 != null && endAudioClip1 != null)
        {
            endAudioSource1.clip = endAudioClip1;
            endAudioSource1.Play();
        }
        if (endAudioSource2 != null && endAudioClip2 != null)
        {
            endAudioSource2.clip = endAudioClip2;
            endAudioSource2.Play();
        }
        yield return null;
    }
}
