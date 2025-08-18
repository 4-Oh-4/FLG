using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupShuffleA : MonoBehaviour {
    [SerializeField] private List<Transform> cups;   // Assign 3 cups in Inspector
    [SerializeField] private int shuffleCount = 10;  // How many swaps
    [SerializeField] private float shuffleSpeed = 3f; // Higher = faster
    [SerializeField] private float speedincrease = 1f;
    [SerializeField] private int shuuflesincrease = 2;
    [SerializeField] private float shufflePause = 0.2f; // Small delay between swaps
    [SerializeField] private float arcHeight = 0.5f;   // Arc size

    private bool isShuffling = false;

    void Start() {
        //StartCoroutine(ShuffleCups());
    }

    public void StartShuffle() {
        if (isShuffling) return; // prevent overlapping coroutines
        ShuffleManager_A.shufflefinished = false;
        StartCoroutine(ShuffleCups());
    }

    IEnumerator ShuffleCups() {
        isShuffling = true;

        for (int i = 0; i < shuffleCount; i++) {
            // Pick 2 different cups
            int a = Random.Range(0, cups.Count);
            int b;
            do { b = Random.Range(0, cups.Count); } while (a == b);

            Transform cupA = cups[a];
            Transform cupB = cups[b];

            Vector3 posA = cupA.position;
            Vector3 posB = cupB.position;

            float t = 0;
            while (t < 1f) {
                t += Time.deltaTime * shuffleSpeed;

                // Midpoints with arc offset
                Vector3 midpointA = (posA + posB) / 2 + Vector3.up * arcHeight;     // arc up
                Vector3 midpointB = (posA + posB) / 2 + Vector3.down * arcHeight;   // arc down

                // A arcs upward
                cupA.position = Vector3.Lerp(
                    Vector3.Lerp(posA, midpointA, t),
                    Vector3.Lerp(midpointA, posB, t),
                    t
                );

                // B arcs downward
                cupB.position = Vector3.Lerp(
                    Vector3.Lerp(posB, midpointB, t),
                    Vector3.Lerp(midpointB, posA, t),
                    t
                );

                yield return null;
            }

            // swap their order in the list (useful for tracking)
            cups[a] = cupB;
            cups[b] = cupA;

            yield return new WaitForSeconds(shufflePause);
        }

        ShuffleManager_A.shufflefinished = true;
        isShuffling = false;
        Debug.Log("Shuffle Finished! Player can now guess.");
    }

    public void IncreaseDifficulty() {
        shuffleCount += shuuflesincrease;
        shuffleSpeed += speedincrease;
    }
}
