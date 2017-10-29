namespace Sift.Common.Network
{
    public enum PacketType
    {
        UpdateAppState,
        InitializeLine,

        LoginRequest,
        DumpLine,
        ScreenLine,
        HoldLine,
        LineRequest,
        AirLine,

        ErrorPacket,
        UpdateSettings,
        RequestSettings,
        LineMetadata,
        RemoveLine,
    }
}
