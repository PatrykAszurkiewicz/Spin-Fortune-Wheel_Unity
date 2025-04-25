using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheel : MonoBehaviour
{
    public Transform wheelTransform;
    public Button spinButton;
    
    private List<Transform> rewardImages = new List<Transform>();// Dodaj swoje nagrody jako dzieci ko³a (Image(1), Image(2), ...)
    public float spinDuration = 4f; // Spinning time, how long
    private bool isSpinning = false;

    public AudioClip spinClickSound;
    private AudioSource audioSource;

    private ArrowAnim ar;
    //wheel generation -------------------
    public GameObject[] rewardPrefabs;
    public GameObject linePrefab;

    public float highlightDuration = 1f;
    public float spinDelay = 1f;

    public float rewardRangeFromSpin = 95;//reward prefabs

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateSlices();

        spinButton.onClick.AddListener(StartSpin);
        ar = FindAnyObjectByType<ArrowAnim>();

        audioSource = GetComponent<AudioSource>();
    }
    public void StartSpin()
    {
        if (isSpinning) return;

        spinButton.interactable = false;

        // Odtwórz dŸwiêk tu¿ przed startem
        if (spinClickSound != null && audioSource != null)
            audioSource.PlayOneShot(spinClickSound);

        StartCoroutine(Spin());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GenerateSlices()
    {
        int segmentCount = rewardPrefabs.Length;
        float anglePerSlice = 360f / segmentCount;

        rewardImages.Clear();

        for (int i = 0; i < segmentCount; i++)
        {
            // Linie podzia³u
            GameObject line = Instantiate(linePrefab, wheelTransform);
            line.transform.localRotation = Quaternion.Euler(0, 0, -i * anglePerSlice +90);

            RectTransform lineRt = line.GetComponent<RectTransform>();
            lineRt.anchoredPosition = Quaternion.Euler(0, 0, -i * anglePerSlice) * Vector2.up * rewardRangeFromSpin;

            // Prefab nagrody (ju¿ gotowy, kolorowy, z grafik¹ itd.)
            GameObject reward = Instantiate(rewardPrefabs[i], wheelTransform);

            float angle = -i * anglePerSlice + anglePerSlice / 2f;
            reward.transform.localRotation = Quaternion.Euler(0, 0, angle);

            RectTransform rt = reward.GetComponent<RectTransform>();
            rt.anchoredPosition = Quaternion.Euler(0, 0, angle) * Vector2.up * rewardRangeFromSpin;

            rewardImages.Add(reward.transform);
        }
    }

    IEnumerator Spin()
    {
        isSpinning = true;

        int segmentCount = rewardImages.Count;
        float anglePerSegment = 360f / segmentCount;

        float randomOffset = Random.Range(-anglePerSegment / 2f + 5f, anglePerSegment / 2f - 5f); // Nie œrodek!
        float targetSegment = Random.Range(0, segmentCount);
        float finalTargetAngle = targetSegment * anglePerSegment + randomOffset;

        float startAngle = wheelTransform.eulerAngles.z;
        float totalRotation = 360f * Random.Range(3, 6) + finalTargetAngle;

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / spinDuration);
            float currentAngle = Mathf.Lerp(startAngle, startAngle + totalRotation, t);
            wheelTransform.eulerAngles = new Vector3(0, 0, currentAngle);
            yield return null;
        }

        // Oblicz finalny k¹t wzglêdem osi 0 (góra)
        float finalZ = (startAngle + totalRotation) % 360f;

        int rewardIndex = (Mathf.FloorToInt(finalZ / anglePerSegment) + 1) % segmentCount;


        Debug.Log("Reward: " + rewardImages[rewardIndex].name);
        isSpinning = false;

        Transform rewardTransform = rewardImages[rewardIndex].transform;
        Transform highlight = rewardTransform.Find("Highlight");
        highlight?.GetComponent<PulseEffect>()?.StartPulse();

        StartCoroutine(StopPulseAfterDelay(highlightDuration, rewardTransform));

        
        IEnumerator StopPulseAfterDelay(float delay, Transform rewardTransform)
        {
            yield return new WaitForSeconds(delay);
            var highlight = rewardTransform.Find("Highlight");
            highlight?.GetComponent<PulseEffect>()?.StopPulse();
        }
        ar.PlayBounce();

        yield return new WaitForSeconds(spinDelay); // Odczekaj 1 sekundê po spinie
        spinButton.interactable = true; // Odblokuj przycisk
        isSpinning = false;
    }
}
