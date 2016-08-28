using UnityEngine;
using OmiyaGames;

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

    void Start ()
    {
        startSpeech = false;
        if (triggerType == Type.StartTrigger)
        {
            StartDialog();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((triggerType == Type.ManualTrigger) && (other.CompareTag(StageState.PlayerTag) == true))
        {
            StartDialog();
        }
    }

    void StartDialog()
    {
        if(startSpeech == false)
        {
            Singleton.Get<MenuManager>().GetMenu<SpokenDialog>().ShowSpeech(Speech, GameSettings.GetBool(uniqueId, false));
            Singleton.Get<MenuManager>().GetMenu<SpokenDialog>().onHide += OnHideAction;
            startSpeech = true;
        }
    }

    void ResetFlags(SpokenDialog dialog)
    {
        GameSettings.SetBool(uniqueId, true);
        if (triggerType == Type.ManualTrigger)
        {
            startSpeech = false;
        }
    }
}
