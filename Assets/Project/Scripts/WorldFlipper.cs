using UnityEngine;
using OmiyaGames;

[RequireComponent(typeof(Animator))]
public class WorldFlipper : MonoBehaviour
{
    public System.Action<WorldFlipper> onStartFlipAnimation;
    public System.Action<WorldFlipper> onEndFlipAnimation;
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

    public void ExecuteFlip(bool setFlip)
    {
        if (CachedAnimator.GetBool(FlipField) != setFlip)
        {
            if(onStartFlipAnimation != null)
            {
                onStartFlipAnimation(this);
            }
            CachedAnimator.SetBool(FlipField, setFlip);
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
