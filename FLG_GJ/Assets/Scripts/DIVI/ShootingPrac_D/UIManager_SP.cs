using UnityEngine;
using UnityEngine.UI;

public class UIManager_SP : MonoBehaviour
{
    [Header("Lives UI")]
    public Image[] heartImages; // Drag Heart1, Heart2, Heart3 here
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public void UpdateHearts(int currentLives, int maxLives)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentLives)
                heartImages[i].sprite = fullHeart;
            else
                heartImages[i].sprite = emptyHeart;

            // Hide extra hearts (in case maxLives < UI slots)
            heartImages[i].enabled = i < maxLives;
        }
    }
}
