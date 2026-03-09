using UnityEngine;

namespace CMP.Scripts
{
    public enum GhostState
    {
        InHouse,
        JoiningGame,
        Scatter,
        Chase,
    }

    public class Ghost : MonoBehaviour
    {
        public GameObject LeftEye;
        public GameObject RightEye;
    }
}