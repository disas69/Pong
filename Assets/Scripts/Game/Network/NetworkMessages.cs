using UnityEngine.Networking;

namespace Game.Network
{
    public static class NetworkMessages
    {
        public const short GameOverMessage = MsgType.Highest + 1;
        public const short BallSettingsMessage = MsgType.Highest + 2;
        public const short StartGameMessage = MsgType.Highest + 3;
    }
}