namespace Sift.Common.Network
{
    public enum PacketType
    {
        UpdateAppState,
        UpdateLineState,

        LoginRequest,
        RequestDump,
        RequestScreen,
        RequestHold,
        RequestLine,
        RequestAir,

        ErrorPacket,
        UpdateSettings,
        RequestSettings,
    }
}
