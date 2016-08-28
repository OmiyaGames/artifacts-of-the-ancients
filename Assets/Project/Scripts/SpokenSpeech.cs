using UnityEngine;

public class SpokenSpeech : MonoBehaviour
{
    [System.Serializable]
    public struct Speech
    {
        [SerializeField]
        string name;
        [SerializeField]
        [Multiline]
        string speech;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Dialog
        {
            get
            {
                return speech;
            }
        }
    }

    [SerializeField]
    Speech[] allSpeeches;

    public Speech[] AllSpeeches
    {
        get
        {
            return allSpeeches;
        }
    }
}
