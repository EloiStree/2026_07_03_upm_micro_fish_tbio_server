using Codice.CM.Common.Serialization.Replication;
using Eloi.MicroFish;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MicroFishMono_ClientInOutExample : MonoBehaviour
{

    public UnityEvent<string> m_OnPushTextToServer;
    public UnityEvent<byte[]> m_OnPushByteToServer;

    [TextArea(3,10)]
    public string m_lastReceviedText;
    public byte[] m_lastReceviedByte;

    public int m_playerCount = 0;
    public Vector3 m_dimension = Vector3.zero;
    public float m_gameTimeInSeconds = 0f;
    public int m_scoreLeft;
    public int m_scoreRight;
    public Vector3 m_ballPosition = Vector3.zero;
    public float m_ballRadius = 0.2f;
    public Vector3[] m_playerPosition;
    public Quaternion[] m_playerRotation;
    public Vector3[] m_playerEuler;
    public int[] m_playerClaimedInteger;
    public string [] m_playerPublicKey;
    public float [] m_playerBatteryLevel;
    public Color[] m_playersColor;
    public FourMotorInputState[] m_playersMotors;



    [System.Serializable]
    public class FourMotorInputState
    {   public float m_leftMotorPercent11;
        public float m_rightMotorPercent11;
        public float m_backMotorPercent11;
        public float m_frontMotorPercent11;
    }

    public void ReceivedFromServerText(string text)
    {
        Debug.Log($"R#{text}");
        string[] lines = text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (line.StartsWith("PLAYER_COUNT:"))
            {
                //PLAYER_COUNT: 13
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    m_playerCount = int.Parse(parts[1]);
                    if (m_playerPosition == null || m_playerPosition.Length != m_playerCount)
                    {
                        m_playerPosition = new Vector3[m_playerCount];
                        m_playerRotation = new Quaternion[m_playerCount];
                        m_playerEuler = new Vector3[m_playerCount];
                        m_playerClaimedInteger = new int[m_playerCount];
                        m_playerPublicKey = new string[m_playerCount];
                        m_playerBatteryLevel= new float[m_playerCount];
                        m_playersColor = new Color[m_playerCount];
                        m_playersMotors = new FourMotorInputState[m_playerCount];
                    }
                }
            }
            else if (line.StartsWith("DIMENSION:"))
            {
                //DIMENSION:-2:-0.902:-1
                string[] parts = line.Split(':');
                if (parts.Length == 4)
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    m_dimension = new Vector3(x, y, z);
                }
            }
            else if (line.StartsWith("PLAYER_COLOR"))
            {
                //PLAYER_COLOR: 8:25:193:221
                //PLAYER_COLOR: 9:216:123:117
                //PLAYER_COLOR: 10:27:17:80
                //PLAYER_COLOR: 11:154:5:5
                //COLOR:1:255:255:255
                string[] parts = line.Trim().Split(":");
                if (parts.Length == 5)
                {
                    if (int.TryParse(parts[1].Trim(), out int playerIndex) &&
                        int.TryParse(parts[2].Trim(), out int r) &&
                        int.TryParse(parts[3].Trim(), out int g) &&
                        int.TryParse(parts[4].Trim(), out int b))
                    {
                        Color color = new Color(r / 255f, g / 255f, b / 255f, 1f);
                        if (playerIndex >= 0 && playerIndex < m_playerCount)
                        {
                            m_playersColor[playerIndex] = color;
                        }
                    }
                }
            }
            else if (line.StartsWith("PLAYER"))
            {

                string[] parts = line.Split(":");
                if (parts.Length == 4)
                {
                    if (int.TryParse(parts[1], out int playerIndex) && int.TryParse(parts[2], out int claimedInteger))
                    {

                        string publicKey = parts[3];
                        if (playerIndex >= 0 && playerIndex < m_playerCount)
                        {
                            m_playerClaimedInteger[playerIndex] = claimedInteger;
                            m_playerPublicKey[playerIndex] = publicKey;
                        }
                    }
                }
            }
            else if (line.StartsWith("BATTERY"))
            {
                //BATTERY: 0:0.9022313
                string[] parts = line.Split(":");
                if (parts.Length == 3)
                {
                    parts[2] = parts[2].Replace(',', '.');
                    if (int.TryParse(parts[1].Trim(), out int playerIndex) &&
                        float.TryParse(parts[2].Trim(), out float batteryLevel))
                    {
                        if (playerIndex >= 0 && playerIndex < m_playerCount)
                        {
                            m_playerBatteryLevel[playerIndex] = batteryLevel;
                        }
                    }
                }
            }

           
            else if (line.StartsWith("GAME_TIME_IN_SECONDS:"))
            {
                //GAME_TIME_IN_SECONDS:1.037494
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    m_gameTimeInSeconds = float.Parse(parts[1]);
                }
            }
            else if (line.StartsWith("SOCCER_SCORE:"))
            {
                string[] parts = line.Split(':');
                if (parts.Length == 3)
                {
                    m_scoreLeft = int.Parse(parts[1]);
                    m_scoreRight = int.Parse(parts[2]);
                }
            }
            else if (line.StartsWith("BALL_POSITION:"))
            {
                //BALL_POSITION:-0.001:-0.902:-0.999
                string[] parts = line.Split(':');
                if (parts.Length == 4)
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    m_ballPosition = new Vector3(x, y, z);
                }
            }
            else if (line.StartsWith("BALL_RADIUS:"))
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    float x = float.Parse(parts[1]);
                    m_ballRadius = x;
                }
            }


        }


    }
    public void ReceivedFromServerByte(byte[] bytes)
    {
        int byteExpectedForPlayerPositio = m_playerCount * 7 * 4;
        m_lastReceviedByte = bytes;
        if (bytes.Length == byteExpectedForPlayerPositio) {

            // position + quaternio = 7 floats = 28 bytes per player
            for (int i = 0; i < m_playerCount; i++)
            {
                int offset = i * 7 * 4;
                float posX = BitConverter.ToSingle(bytes, offset);
                float posY = BitConverter.ToSingle(bytes, offset + 4);
                float posZ = BitConverter.ToSingle(bytes, offset + 8);
                m_playerPosition[i] = new Vector3(posX, posY, posZ);
                float rotX = BitConverter.ToSingle(bytes, offset + 12);
                float rotY = BitConverter.ToSingle(bytes, offset + 16);
                float rotZ = BitConverter.ToSingle(bytes, offset + 20);
                float rotW = BitConverter.ToSingle(bytes, offset + 24);
                Quaternion rotation = new Quaternion(rotX, rotY, rotZ, rotW);
                m_playerRotation[i] = rotation;
                m_playerEuler[i] = rotation.eulerAngles;
            }
        }

        int byteExpectedForPlayerInput = m_playerCount * 4 * 4;
        if (bytes.Length == byteExpectedForPlayerInput)
        {
            // left right back front motor = 4 floats = 16 bytes per player
            for (int i = 0; i < m_playerCount; i++)
            {
                int offset = i * 4 * 4;
                
                if (i < m_playersMotors.Length)
                {
                    if (m_playersMotors[i] == null)
                    { 
                        m_playersMotors[i] = new FourMotorInputState();
                    }
                    m_playersMotors[i].m_leftMotorPercent11 = BitConverter.ToSingle(bytes, offset);
                    m_playersMotors[i].m_rightMotorPercent11 = BitConverter.ToSingle(bytes, offset+4);
                    m_playersMotors[i].m_backMotorPercent11 = BitConverter.ToSingle(bytes, offset + 8);
                    m_playersMotors[i].m_frontMotorPercent11 = BitConverter.ToSingle(bytes, offset + 12);
                }
            }
        }
    }
    private void OnEnable()
    {
        StartCoroutine(PushRandomInput());
    }

    public int m_playerIndex = 0;
    [Range(-1f, 1f)]
    public float m_leftMotorPercent;
    [Range(-1f, 1f)]
    public float m_rightMotorPercent;
    [Range(-1f, 1f)]
    public float m_backMotorPercent;
    [Range(-1f, 1f)]
    public float m_frontMotorPercent;

    public void PushMotorState()
    {
        byte [] motorState = new byte[5*4];
        BitConverter.GetBytes(m_playerIndex).CopyTo(motorState, 0);
        BitConverter.GetBytes(m_leftMotorPercent).CopyTo(motorState, 4);
        BitConverter.GetBytes(m_rightMotorPercent).CopyTo(motorState, 8);
        BitConverter.GetBytes(m_backMotorPercent).CopyTo(motorState, 12);
        BitConverter.GetBytes(m_frontMotorPercent).CopyTo(motorState, 16);
        m_OnPushByteToServer?.Invoke(motorState);
    }

    public IEnumerator PushRandomInput()
    {
        while (true)
        {
            PushMotorState();
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
