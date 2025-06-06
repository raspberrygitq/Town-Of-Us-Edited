using Hazel;
using InnerNet;
using Reactor.Networking;

namespace TownOfUsEdited.Patches
{
    // Code stolen from Reactor (:
    public static class KickBan
    {
        public static void KickWithReason(this InnerNetClient innerNetClient, int targetClientId, string reason)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage(Tags.GameDataTo);
            writer.Write(innerNetClient.GameId);
            writer.WritePacked(targetClientId);
            {
                writer.StartMessage(byte.MaxValue);
                writer.Write((byte) ReactorGameDataFlag.SetKickReason);
                writer.Write(reason);
                writer.EndMessage();
            }
            writer.EndMessage();
            innerNetClient.SendOrDisconnect(writer);
            writer.Recycle();

            innerNetClient.KickPlayer(targetClientId, false);
        }

        public static void Ban(this InnerNetClient innerNetClient, int targetClientId, string reason)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage(Tags.GameDataTo);
            writer.Write(innerNetClient.GameId);
            writer.WritePacked(targetClientId);
            {
                writer.StartMessage(byte.MaxValue);
                writer.Write((byte) ReactorGameDataFlag.SetKickReason);
                writer.Write(reason);
                writer.EndMessage();
            }
            writer.EndMessage();
            innerNetClient.SendOrDisconnect(writer);
            writer.Recycle();

            innerNetClient.KickPlayer(targetClientId, true);
        }
    }
}