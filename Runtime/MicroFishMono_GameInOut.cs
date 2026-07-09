
using System;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.MicroFish
{
    public class MicroFishMono_GameInOut : MonoBehaviour
    {
        public UnityEvent<byte[]> m_onEmitByteInfo;
        public UnityEvent<string> m_onEmitTextInfo;
        public UnityEvent<int, byte[]> m_onEmitByteInfoForPlayer;
        public UnityEvent<int, string> m_onEmitTextInfoForPlayer;
        public UnityEvent<byte[]> m_onByteReceivedFromClients;
        public UnityEvent<string> m_onTextReceivedFromClients;

        public Transform m_startDownCorner;
        public Transform m_endTopCorner;

        public int[] m_playerClaimedIndex;
        public string[] m_playerClaimedPublicKey;

        public MicroFishMono_MotorInput[] m_playersInput;
        public MicroFishMono_NesInputToMotorInput[] m_playerNesInput; 
        public MicroFishMono_BatteryForFourMotor[] m_playersBattery;
        public MicroFishMono_SetMeshInstanceColor[] m_playersColor;
        public Transform[] m_playersPosition;

        public Vector3[] m_localPlayerPosition;
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

        public bool m_givePlayerRandomPositionOnStart = true;


        public Transform m_ballPosition;
        public Transform m_ballRadiusAnchor;
        public float m_ballRadius = 0.01f;


        private void Update()
        {
            m_timeSinceGameStart += Time.deltaTime;
            if (m_ballPosition != null && m_ballRadiusAnchor != null)
                m_ballRadius = Vector3.Distance(m_ballPosition.position, m_ballRadiusAnchor.position);
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
                GetWorldToLocal_DirectionalPoint(m_playersPosition[i].position, m_playersPosition[i].rotation,
                    m_startDownCorner.position, m_startDownCorner .rotation,
                    out m_localPlayerPosition[i], out m_localPlayerRotation[i]);
            }
            GetWorldToLocal_DirectionalPoint(m_endTopCorner.position, m_endTopCorner.rotation,
                m_startDownCorner.position, m_startDownCorner.rotation,
                out m_dimension, out _);

            if (m_useUpdateForPersonnalPosition)
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

        public IEnumerator Start()
        {

            yield return new WaitForSeconds(2.5f);
            if (m_givePlayerRandomPositionOnStart)
            {
                GivePlayerRandomPositionOnStart();

            }
        }


        [ContextMenu("Give Player Random Position On Start")]
        public void GivePlayerRandomPositionOnStart()
        {

            GetWorldToLocal_DirectionalPoint(m_endTopCorner.position, m_endTopCorner.rotation,
                m_startDownCorner.position, m_startDownCorner.rotation,
                out Vector3 localDimension, out _);

            Debug.Log($"Local dimension is {localDimension.x} x {localDimension.y} x {localDimension.z}");
            for (int i = 0; i < m_playersPosition.Length; i++)
            {
                Vector3 randomLocalPosition = new Vector3(
                    UnityEngine.Random.Range(0f, localDimension.x),
                    UnityEngine.Random.Range(0f, localDimension.y),
                    UnityEngine.Random.Range(0f, localDimension.z)
                );
                GetLocalToWorld_DirectionalPoint(randomLocalPosition, Quaternion.identity,
                    m_startDownCorner.position, m_startDownCorner.rotation,
                    out Vector3 worldPosition, out Quaternion worldRotation);
                m_playersPosition[i].position = worldPosition;
                m_playersPosition[i].rotation = worldRotation;
                Debug.Log($"Player {i} random position is {worldPosition} and rotation is {worldRotation}");
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
            PushCurrentPlayersMotors();
        }

        void PushCurrentPlayersMotors() {
            byte[] motorState = new byte[4 * 4 * m_playersInput.Length];
            for (int i = 0; i < m_playersInput.Length; i++)
            {
                m_playersInput[i].GetMotorsPercent11AsLRBF(out float left, out float right, out float back, out float front);
                Buffer.BlockCopy(BitConverter.GetBytes(left), 0, motorState, 16 * i + 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(right), 0, motorState, 16 * i + 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(back), 0, motorState, 16 * i + 8, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(front), 0, motorState, 16 * i + 12, 4);
            }
            m_onEmitByteInfo.Invoke(motorState);
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

        public void NotifyToClientsTeamScoreChanged(int leftTeam, int rightTeam)
        {
            m_onEmitTextInfo.Invoke($"SOCCER_SCORE:{leftTeam}:{rightTeam}");
        }

        private void PushCurrentGameInformationAsText()
        {
            StringBuilder gameInfo = new StringBuilder();
            gameInfo.AppendLine($"GAME_TIME_IN_SECONDS:{m_timeSinceGameStart}");
            gameInfo.AppendLine($"DIMENSION:{m_dimension.x}:{m_dimension.y}:{m_dimension.z}");
            gameInfo.AppendLine($"PLAYER_COUNT:{m_playersPosition.Length}");
            if (m_ballPosition != null && m_ballRadiusAnchor != null)
            {
                GetWorldToLocal_DirectionalPoint(m_ballPosition.position, m_ballPosition.rotation,
                    m_startDownCorner.position, m_startDownCorner.rotation,
                    out Vector3 localBallPosition, out _);
                gameInfo.AppendLine($"BALL_POSITION:{localBallPosition.x}:{localBallPosition.y}:{localBallPosition.z}");
                gameInfo.AppendLine($"BALL_RADIUS:{m_ballRadius}");
            }
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
                playerInfo.AppendLine($"PLAYER:{i}:{indexClaimed}:{publicKeyClaimed}");
            }
            for (int i = 0; i < m_playersBattery.Length; i++)
            {
                float batteryPercent = m_playersBattery[i].GetBatteryPercent();
                playerInfo.AppendLine($"BATTERY:{i}:{batteryPercent}");
            }
            for (int i = 0; i < m_playersColor.Length; i++)
            {
                Color32 color32 = m_playersColor[i].m_targetColor;
                playerInfo.AppendLine($"PLAYER_COLOR:{i}:{color32.r}:{color32.g}:{color32.b}");
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

        public void ThrustedInputParsing(byte[] data)
        {
            m_onByteReceivedFromClients.Invoke(data);
            if (data.Length > 20)
                return;

            if (data.Length == 20)
            {

                int playerIndex = BitConverter.ToInt32(data, 0);
                float motor1 = BitConverter.ToSingle(data, 4);
                float motor2 = BitConverter.ToSingle(data, 8);
                float motor3 = BitConverter.ToSingle(data, 12);
                float motor4 = BitConverter.ToSingle(data, 16);
                if (playerIndex >= 0 && playerIndex < m_playersInput.Length)
                {
                    m_playersInput[playerIndex].SetMotorsWithFloatArray(motor1, motor2, motor3, motor4);
                }
            }
           
        }

       

        public void ThrustedIndexIntegerInput(int index,int value)
        {
            if (index >= 0 && index < m_playerNesInput.Length)
            {
                MicroFishMono_NesInputToMotorInput nes = m_playerNesInput[index];
                if (value>0 && value <= 9999) {
                    //Debug.Log($"Player {index} received input value {value}");
                    switch (value) {
                        // NES
                        //Up Arrow    1281    2281   
                        //Right Arrow 1282    2282   
                        //Down Arrow  1283    2283   
                        //Left Arrow  1284    2284   
                        //A Button    1285    2285   
                        //B Button    1286    2286   
                        //Menu Left   1287    2287   
                        //Menu Right  1288    2288   

                        case 1283: case 1335: case 1315: nes.m_buttonArrowDown = true; break;
                        case 1284: case 1337: case 1311: nes.m_buttonArrowLeft = true; break;
                        case 1281: case 1331: case 1317: nes.m_buttonArrowUp = true; break;
                        case 1282: case 1333: case 1313: nes.m_buttonArrowRight = true; break;
                        case 1285: case 1300: nes.m_buttonA = true; break;
                        case 1286: case 1301: case 1302: case 1303: nes.m_buttonB = true; break;
                        case 1309: case 1287: nes.m_buttonMenuLeft = true; break;
                        case 1308: case 1288: nes.m_buttonMenuRight = true; break;


                        case 2283: case 2335: case 2315: nes.m_buttonArrowDown = false; break;
                        case 2284: case 2337: case 2311: nes.m_buttonArrowLeft = false; break;
                        case 2281: case 2331: case 2317: nes.m_buttonArrowUp = false; break;
                        case 2282: case 2333: case 2313: nes.m_buttonArrowRight = false; break;
                        case 2285: case 2300: nes.m_buttonA = false; break;
                        case 2286: case 2301: case 2302: case 2303: nes.m_buttonB = false; break;
                        case 2309: case 2287: nes.m_buttonMenuLeft = false; break;
                        case 2308: case 2288: nes.m_buttonMenuRight = false; break;


                        //17 11 13 15



                        //Gamepad
                        //Up Arrow    1331    2331
                        //Right Arrow 1333    2333
                        //Down Arrow  1335    2335
                        //Left Arrow  1337    2337
                        //A Button    1300    2300
                        //B Button(B)    1302    2302
                        //B Button(X)    1301    2301
                        //B Button(Y)    1303    2303
                        //Menu Left   1309    2309
                        //Menu Right  1308    2308

                        default:
                            Debug.LogWarning($"Player {index} received unknown input value {value}");
                            break;
                    }
                }
                if (value >= 1800000000 && value < 1900000000) { 
                    // Gamepad
                    // With default game default mapping
                }
                if (value >= 1600000000 && value < 1700000000)
                {
                    // Analogique value from with range of 9  -1 to 1 se S2W
                }
                if (value >= -1600000000 && value < -1500000000)
                {
                    // -1688899999
                    // Analogique value from for 99999 on motor 888 -1 to 1 se S2W
                }
            }
        }

        public void PushInTextFromClientWithoutSource(string text) { 
        
            m_onTextReceivedFromClients.Invoke(text);   
        }

    }
}