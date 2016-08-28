using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class WorldFlipper : MonoBehaviour
{
    public System.Action<WorldFlipper> onStartFlipAnimation;
    public System.Action<WorldFlipper> onEndFlipAnimation;

    [Header("Required Components")]
    [SerializeField]
    Sprite rightSideUpImage;
    [SerializeField]
    Sprite upsideDownImage;

    [Header("Required Components")]
    [SerializeField]
    Image opaqueBackground;
    [SerializeField]
    Image transparentBackground;

    static WorldFlipper instance = null;
    const string FlipField = "Is Right-Side Up?";
    Animator animatorCache = null;

    static public WorldFlipper Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;

        // Setup background
        opaqueBackground.sprite = rightSideUpImage;
        transparentBackground.sprite = upsideDownImage;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public Animator CachedAnimator
    {
        get
        {
            if (animatorCache == null)
            {
                animatorCache = GetComponent<Animator>();
            }
            return animatorCache;
        }
    }

    public void ExecuteFlip(bool isRightSideUp)
    {
        if (CachedAnimator.GetBool(FlipField) != isRightSideUp)
        {
            if(onStartFlipAnimation != null)
            {
                onStartFlipAnimation(this);
            }

            // Setup background
            if (isRightSideUp == true)
            {
                opaqueBackground.sprite = rightSideUpImage;
                transparentBackground.sprite = upsideDownImage;
            }
            else
            {
                opaqueBackground.sprite = upsideDownImage;
                transparentBackground.sprite = rightSideUpImage;
            }

            // Animate
            CachedAnimator.SetBool(FlipField, isRightSideUp);
        }
    }

    public void EndFlip()
    {
        if (onEndFlipAnimation != null)
        {
            onEndFlipAnimation(this);
        }
    }
}
