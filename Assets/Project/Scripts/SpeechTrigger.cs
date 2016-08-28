using UnityEngine;
using OmiyaGames;
using System.Collections;

[RequireComponent(typeof(SpokenSpeech))]
public class SpeechTrigger : ITriggers
{
    public enum Type
    {
        ManualTrigger,
        StartTrigger
    }

    [SerializeField]
    Type triggerType;
    [SerializeField]
    string uniqueId = "Speech.Text 01";

    [Header("Pop-up")]
    [SerializeField]
    string popUpActionText = "Examine";

    [Header("Switch properties")]
    [SerializeField]
    IToggle[] allToggles;

    const float delayStartSpeech = 0.75f;
    bool startSpeech = false;
    System.Action<SpokenDialog> onHide = null;
    SpokenSpeech speechCache = null;

    public System.Action<SpokenDialog> OnHideAction
    {
        get
        {
            if(onHide == null)
            {
                onHide = new System.Action<SpokenDialog>(ResetFlags);
            }
            return onHide;
        }
    }

    public SpokenSpeech Speech
    {
        get
        {
            if(speechCache == null)
            {
                speechCache = GetComponent<SpokenSpeech>();
            }
            return speechCache;
        }
    }

    public override Action ActionOnFire1
    {
        get
        {
            if(triggerType == Type.ManualTrigger)
            {
                return Action.Examine;
            }
            else
            {
                return Action.Invalid;
            }
        }
    }

    public override string ActionText
    {
        get
        {
            return popUpActionText;
        }
    }

    IEnumerator Start ()
    {
        startSpeech = false;
        yield return new WaitForSeconds(delayStartSpeech);
        if (triggerType == Type.StartTrigger)
        {
            StartDialog();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((triggerType == Type.ManualTrigger) &&
            (other.CompareTag(StageState.PlayerTag) == true) &&
            (StageState.Instance != null))
        {
            StageState.Instance.AddTrigger(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((triggerType == Type.ManualTrigger) &&
            (other.CompareTag(StageState.PlayerTag) == true) &&
            (StageState.Instance != null))
        {
            StageState.Instance.RemoveTrigger(this);
        }
    }

    public void StartDialog()
    {
        if(startSpeech == false)
        {
            // Update flags
            startSpeech = true;
            bool showSkip = false;
            if(string.IsNullOrEmpty(uniqueId) == false)
            {
                showSkip = GameSettings.GetBool(uniqueId, false);
            }

            // Bind to event
            Singleton.Get<MenuManager>().GetMenu<SpokenDialog>().onHide += OnHideAction;

            // Show the dialog
            Singleton.Get<MenuManager>().GetMenu<SpokenDialog>().ShowSpeech(Speech, showSkip);
        }
    }

    void ResetFlags(SpokenDialog dialog)
    {
        // Update flags
        if (string.IsNullOrEmpty(uniqueId) == false)
        {
            GameSettings.SetBool(uniqueId, true);
        }
        if (triggerType == Type.ManualTrigger)
        {
            startSpeech = false;
        }

        // Toggle switches
        if((allToggles != null) && (allToggles.Length > 0))
        {
            for(int index = 0; index < allToggles.Length; ++index)
            {
                if(allToggles[index] != null)
                {
                    allToggles[index].Toggle(this);
                }
            }
        }

        // Remove self from event
        Singleton.Get<MenuManager>().GetMenu<SpokenDialog>().onHide -= OnHideAction;
    }
}
