using System.Collections.Generic;
using UnityEngine;

namespace RPGM.Gameplay
{
    /// <summary>
    /// A system for batch animation of fading sprites.
    /// </summary>
    public class FadingSpriteSystem : MonoBehaviour
    {
        void Update()
        {
            for (int i = FadingSprite.Instances.Count - 1; i >= 0; i--)
            {
                var c = FadingSprite.Instances[i];
                if (c == null || c.gameObject == null)
                {
                    // Remove the null instance from the list to avoid future errors.
                    FadingSprite.Instances.RemoveAt(i);
                    continue;
                }

                if (c.gameObject.activeSelf)
                {
                    c.alpha = Mathf.SmoothDamp(c.alpha, c.targetAlpha, ref c.velocity, 0.1f, 1f);
                    c.spriteRenderer.color = new Color(1, 1, 1, c.alpha);
                }
            }
        }
    }
}
