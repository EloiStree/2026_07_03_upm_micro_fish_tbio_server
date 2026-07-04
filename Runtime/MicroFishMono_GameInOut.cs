using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.MicroFish { 
public class MicroFishMono_GameInOut : MonoBehaviour
{

        public UnityEvent<byte[]> m_onEmitByteInfo;
        public UnityEvent<string> m_onEmitTextInfo;
        public UnityEvent<int,byte[]> m_onEmitByteInfoForPlayer;
        public UnityEvent<int,string> m_onEmitTextInfoForPlayer;

        public Transform m_startDownCorner;
        public Transform m_endTopCorner;

        public int[] m_playerClaimedIndex ;
        public string[] m_playerClaimedPublicKey;

        public MicroFishMono_MotorInput[] m_playersInput;
        public MicroFishMono_BatteryForFourMotor[] m_playersBattery;
        public Transform[] m_playersPosition;

        public Vector3 [] m_localPlayerPosition;
        public Quaternion[] m_localPlayerRotation;
        public Vector3 m_dimension;

        public float m_timeSinceGameStart;

        public int m_gameStateAsTextCount;
        public int m_gameStateAsByteCount;
        [TextArea(3, 8)]
        public string m_gameStateAsText;
        [TextArea(3, 8)]
        public string m_playerIdInfoAsText;
        public byte[] m_gameStateAsByte;


        private void Update()
        {
            m_timeSinceGameStart += Time.deltaTime;
        }
        

        public void SetPlayerPublicKeyFromArray(params string[] publicKeys)
        {
            for (int i = 0; i < publicKeys.Length; i++)
            {
                SetPlayerPublicKey(i, publicKeys[i]);
            }
        }
        public void SetPlayerClaimedIndexFromArray(params int[] claimedIndexes)
        {
            for (int i = 0; i < claimedIndexes.Length; i++)
            {
                SetPlayerClaimedIndex(i, claimedIndexes[i]);
            }
        }

        public void SetPlayerPublicKey(int arrayIndex, string publicKey)
        {
            if (arrayIndex >= m_playerClaimedPublicKey.Length)
                return;
            m_playerClaimedPublicKey[arrayIndex] = publicKey;
        }
        public void SetPlayerClaimedIndex(int arrayIndex, int claimedIndex)
        {
            if (arrayIndex >= m_playerClaimedIndex.Length)
                return;
            m_playerClaimedIndex[arrayIndex] = claimedIndex;
        }

        public void MovePlayerFromArrayIndex(int arrayIndex, params float[] motors)
    {
        m_playersInput[arrayIndex].SetMotorsWithFloatArray(motors);
    }
    public void MovePlayerFromPublicKey(string publicKey, params float[] motors)
    {
        int index = System.Array.IndexOf(m_playerClaimedPublicKey, publicKey);
        if (index >= 0)
        {
            m_playersInput[index].SetMotorsWithFloatArray(motors);
        }
    }
    public void MovePlayerFromClaimedIndex(int claimedIndex, params float[] motors)
    {
        int index = System.Array.IndexOf(m_playerClaimedIndex, claimedIndex);
        if (index >= 0)
        {
            m_playersInput[index].SetMotorsWithFloatArray(motors);
        }
    }

        public bool m_useUpdateForPersonnalPosition = true;
        public void FixedUpdate()
        {
            if (m_localPlayerPosition.Length != m_playersPosition.Length)
            {
                m_localPlayerPosition = new Vector3[m_playersPosition.Length];
                m_localPlayerRotation = new Quaternion[m_playersPosition.Length];
            }

            for (int i = 0; i < m_playersPosition.Length; i++)
            {
                GetWorldToLocal_DirectionalPoint(m_startDownCorner.position, m_startDownCorner.rotation,
                    m_playersPosition[i].position, m_playersPosition[i].rotation,
                    out m_localPlayerPosition[i], out m_localPlayerRotation[i]);
            }
            GetWorldToLocal_DirectionalPoint(m_startDownCorner.position, m_startDownCorner.rotation,
                m_endTopCorner.position, m_endTopCorner.rotation,
                out m_dimension, out _);

            if(m_useUpdateForPersonnalPosition)
            {
                PushCurrentPlayerPositionRotation();
            }
        }

        public bool m_useAutoPushAll = true;
        public float m_autoPushTime = 1f;

        public void Awake()
        {
            if (m_useAutoPushAll)
            {
                InvokeRepeating(nameof(PushAll), m_autoPushTime, m_autoPushTime);
            }
        }


        [ContextMenu("Push All")]
        public void PushAll()
        {
            PushCurrentPlayerPositionRotation();
            PushCurrentGameInformationAsText();
            PushCurrentPlayerIdInformationAsText();
            PushDetailTextPlayerInformation();
            PushDetailBytePlayerInformation();
        }
        private void PushCurrentPlayerPositionRotation()
        {
            byte[] playerPosition = new byte[7 * 4 * m_playersPosition.Length];
            for (int i = 0; i < m_playersPosition.Length; i++)
            {
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerPosition[i].x), 0, playerPosition, 7 * 4 * i + 0, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerPosition[i].y), 0, playerPosition, 7 * 4 * i + 4, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerPosition[i].z), 0, playerPosition, 7 * 4 * i + 8, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].x), 0, playerPosition, 7 * 4 * i + 12, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].y), 0, playerPosition, 7 * 4 * i + 16, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].z), 0, playerPosition, 7 * 4 * i + 20, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].w), 0, playerPosition, 7 * 4 * i + 24, 4);
            }
            m_gameStateAsByte = playerPosition;
            m_gameStateAsByteCount = playerPosition.Length;
            m_onEmitByteInfo.Invoke(m_gameStateAsByte);
        }

        private void PushCurrentGameInformationAsText()
        {
            StringBuilder gameInfo = new StringBuilder();
            gameInfo.AppendLine($"GAME_TIME_IN_SECONDS:{m_timeSinceGameStart}");
            gameInfo.AppendLine($"DIMENSION:{m_dimension.x}:{m_dimension.y}:{m_dimension.z}");
            gameInfo.AppendLine($"PLAYER_COUNT:{m_playersPosition.Length}");
            m_gameStateAsText = gameInfo.ToString();
            m_gameStateAsTextCount = m_gameStateAsText.Length;
            m_onEmitTextInfo.Invoke(m_gameStateAsText);
        }

        private void PushCurrentPlayerIdInformationAsText()
        {
            StringBuilder playerInfo = new StringBuilder();
            for (int i = 0; i < m_playersPosition.Length; i++)
            {
                string indexClaimed = m_playerClaimedIndex.Length > i ? m_playerClaimedIndex[i].ToString() : "";
                string publicKeyClaimed = m_playerClaimedPublicKey.Length > i ? m_playerClaimedPublicKey[i] : "";
                playerInfo.Append($"PLAYER:{i}:{indexClaimed}:{publicKeyClaimed}");
            }
            m_playerIdInfoAsText = playerInfo.ToString();
            m_onEmitTextInfo.Invoke(m_playerIdInfoAsText);
        }


        public void PushDetailTextPlayerInformation()
        {

            for (int i = 0; i < m_playersBattery.Length; i++)
            {
                StringBuilder info = new StringBuilder();
                float batteryPercent = m_playersBattery[i].GetBatteryPercent();
                info.AppendLine($"BATTERY:{i}:{batteryPercent}");
                m_onEmitTextInfoForPlayer.Invoke(i, info.ToString());


            }
        }
        public void PushDetailBytePlayerInformation()
        {
            for (int i = 0; i < m_playersPosition.Length; i++)
            {
                byte[] playerPosition = new byte[7 * 4];
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerPosition[i].x), 0, playerPosition, 7 * 4 * 0 + 0, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerPosition[i].y), 0, playerPosition, 7 * 4 * 0 + 4, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerPosition[i].z), 0, playerPosition, 7 * 4 * 0 + 8, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].x), 0, playerPosition, 7 * 4 * 0 + 12, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].y), 0, playerPosition, 7 * 4 * 0 + 16, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].z), 0, playerPosition, 7 * 4 * 0 + 20, 4);
                System.Buffer.BlockCopy(System.BitConverter.GetBytes(m_localPlayerRotation[i].w), 0, playerPosition, 7 * 4 * 0 + 24, 4);
                m_onEmitByteInfoForPlayer.Invoke(i, playerPosition);
            }
        }

        public static void GetWorldToLocal_DirectionalPoint(in Vector3 worldPosition, in Quaternion worldRotation, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 localPosition, out Quaternion localRotation)
    {
        localRotation = Quaternion.Inverse(rotationReference) * worldRotation;
        localPosition = Quaternion.Inverse(rotationReference) * (worldPosition - positionReference);
    }
    public static void GetLocalToWorld_DirectionalPoint(in Vector3 localPosition, in Quaternion localRotation, in Vector3 positionReference, in Quaternion rotationReference, out Vector3 worldPosition, out Quaternion worldRotation)
    {
        /// I need to verify the commutativity of this code. 
        /// I think it was ok then had a bug in a game link to this methode and thr commutative property
        worldRotation = rotationReference * localRotation;
        worldPosition = (rotationReference * localPosition) + (positionReference);
    }
}

}