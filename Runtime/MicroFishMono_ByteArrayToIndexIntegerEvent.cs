using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_ByteArrayToIndexIntegerEvent : MonoBehaviour
{

    public UnityEvent<int> m_onIntegerFound;
    public UnityEvent<int,int> m_onIndexIntegerFound;

    public void PushBytesToParse(byte[] bytes) {
        if (bytes == null || bytes.Length == 0) {
            return;
        }
        if (bytes.Length == 4)
        {
            int value = System.BitConverter.ToInt32(bytes, 0);
            m_onIntegerFound?.Invoke(value);
        }
        else if (bytes.Length == 8)
        {
            int index = System.BitConverter.ToInt32(bytes, 0);
            int value = System.BitConverter.ToInt32(bytes, 4);
            m_onIndexIntegerFound?.Invoke(index, value);
        }
        else if (bytes.Length == 12)
        {
            int value1 = System.BitConverter.ToInt32(bytes, 4);
            m_onIntegerFound?.Invoke( value1);
        }

        else if (bytes.Length == 16) {
            int index = System.BitConverter.ToInt32(bytes, 0);
            int value1 = System.BitConverter.ToInt32(bytes, 4);
            m_onIndexIntegerFound?.Invoke(index, value1);
        }
    }
}
