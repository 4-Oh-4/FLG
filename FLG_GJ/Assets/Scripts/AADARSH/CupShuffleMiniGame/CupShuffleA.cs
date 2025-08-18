using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupShuffleA : MonoBehaviour {
    [SerializeField] private List<Transform> cups;   // Assign 3 cups in Inspector
    [SerializeField] private int shuffleCount = 10;  // How many swaps
    [SerializeField] private float shuffleSpeed = 3f; // Higher = faster
    [SerializeField] private float speedincrease = 1f;
    [SerializeField] private int shuuflesincrease = 2;
    void Start() {
        //StartCoroutine(ShuffleCups());
    }

    public void StartShuffle() {
        ShuffleManager_A.shufflefinished = false;
        StartCoroutine(ShuffleCups());
    }
    IEnumerator ShuffleCups() {
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
                cupA.position = Vector3.Lerp(posA, posB, t);
                cupB.position = Vector3.Lerp(posB, posA, t);
                yield return null;
            }

            // swap their order in the list (optional but useful for tracking)
            cups[a] = cupB;
            cups[b] = cupA;

            yield return new WaitForSeconds(0.01f); // small pause between swaps
        }
        ShuffleManager_A.shufflefinished = true;
        Debug.Log("Shuffle Finished! Player can now guess.");
    }
    public void IncreaseDifficulty() {
        shuffleCount += shuuflesincrease;
        shuffleSpeed += speedincrease;
    }
}
