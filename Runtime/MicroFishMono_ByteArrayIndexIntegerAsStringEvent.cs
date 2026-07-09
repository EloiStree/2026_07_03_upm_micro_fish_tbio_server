using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_IndexIntegerAsStringEvent : MonoBehaviour
{
    public UnityEvent<string> m_onTextChanged;

    public string m_lastReceivedDebugText;

    public string formatStringValue = "{0}";
    public string formatStringIndexValue = "{0}:{1}";


    public void PushInInteger(int value) { 
        m_lastReceivedDebugText = string.Format(formatStringValue, value);
        m_onTextChanged?.Invoke(m_lastReceivedDebugText);
    }
    public void PushInIndexInteger(int index, int value) { 
        m_lastReceivedDebugText = string.Format(formatStringIndexValue, index, value);
        m_onTextChanged?.Invoke(m_lastReceivedDebugText);
    }
}
