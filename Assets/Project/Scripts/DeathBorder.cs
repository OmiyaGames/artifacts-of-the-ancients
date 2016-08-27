using UnityEngine;
using OmiyaGames;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DeathBorder : MonoBehaviour
{
    bool isReady = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(StageState.PlayerTag) == true)
        {
            isReady = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag(StageState.PlayerTag) == true) && (isReady == true))
        {
            StageState.Instance.IsPaused = true;
            Singleton.Get<MenuManager>().Show<SceneTransitionMenu>(OnShowComplete);
        }
    }

    void OnShowComplete(IMenu menu)
    {
        if (((SceneTransitionMenu)menu).CurrentTransition == SceneTransitionMenu.Transition.SceneTransitionOutEnd)
        {
            StartCoroutine(HideTransition());
        }
    }

    IEnumerator HideTransition()
    {
        yield return null;
        StageState.Instance.Respawn();
        Singleton.Get<MenuManager>().GetMenu<SceneTransitionMenu>().Hide(OnHideComplete);
        StageState.Instance.IsPaused = false;
    }

    void OnHideComplete(IMenu menu)
    {
        if (((SceneTransitionMenu)menu).CurrentTransition == SceneTransitionMenu.Transition.SceneTransitionInEnd)
        {
            isReady = false;
        }
    }
}
