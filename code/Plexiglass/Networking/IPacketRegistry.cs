namespace Plexiglass.Networking
{
    public interface IPacketRegistry
    {
        void RegisterPacket<TP, TH>()
            where TP : IPacket
            where TH : IPacketHandler<TP>;

        object HandlePacket<TP>(TP packet)
            where TP : IPacket;

        PacketDirectionality Directionality { get; set; }
    }
}
