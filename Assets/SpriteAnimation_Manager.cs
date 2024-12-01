using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class SpriteAnimation_Manager : MonoBehaviour
{
    [System.Serializable]
    public class SpriteGroup
    {
        public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        public GameObject vfxPrefab;
        public Ease easeType = Ease.OutBack; // Add easeType for each group
    }

    public List<SpriteGroup> spriteGroups = new List<SpriteGroup>();
    public float animationDuration = 0.5f;
    public float delayBetweenSprites = 0.1f;
    public float delayBetweenGroups = 1f;

    private List<List<Vector3>> originalScales = new List<List<Vector3>>();

    void Start()
    {
         // Initialize original scales and set initial scale to zero for all sprites
        foreach (SpriteGroup group in spriteGroups)
        {
            List<Vector3> groupScales = new List<Vector3>();
            foreach (SpriteRenderer sprite in group.sprites)
            {
                groupScales.Add(sprite.transform.localScale);
                sprite.transform.localScale = Vector3.zero; // Set initial scale to zero
            }
            originalScales.Add(groupScales);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AnimateSpriteGroups());
        }
    }

    IEnumerator AnimateSpriteGroups()
    {
        for (int i = 0; i < spriteGroups.Count; i++)
        {
            StartCoroutine(AnimateSprites(spriteGroups[i]));

            // Wait for the current group's animation to finish
            yield return new WaitForSeconds(animationDuration +
                                             (spriteGroups[i].sprites.Count - 1) * delayBetweenSprites);

            // Instantiate VFX at each sprite's location
            if (spriteGroups[i].vfxPrefab != null)
            {
                foreach (SpriteRenderer sprite in spriteGroups[i].sprites)
                {
                    Instantiate(spriteGroups[i].vfxPrefab, sprite.transform.position, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(delayBetweenGroups);
        }
    }

    IEnumerator AnimateSprites(SpriteGroup group)
    {
        for (int i = 0; i < group.sprites.Count; i++)
        {
            SpriteRenderer sprite = group.sprites[i];
            sprite.transform.localScale = Vector3.zero;
            sprite.transform.DOScale(originalScales[spriteGroups.IndexOf(group)][i], animationDuration)
                .SetEase(group.easeType); // Use the group's easeType
            yield return new WaitForSeconds(delayBetweenSprites);
        }
    }
}