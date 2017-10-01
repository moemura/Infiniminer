using Infiniminer;
using Microsoft.Xna.Framework.Media;
using Plexiglass.Client;
using Plexiglass.Client.Engine;
using Plexiglass.Client.States;
using Plexiglass.Networking.Packets;
using System;

namespace Plexiglass.Networking.Handlers
{
    public class PacketBlockBulkTransferHandler : IPacketHandler<PacketBlockBulkTransfer>
    {
        public object HandlePacket(PacketBlockBulkTransfer packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null) return null;

            var blockEngine = propertyBag.GetEngine<IBlockEngine>("blockEngine");

            try
            {   
                // TODO: Make compression code work
                if (packet.IsCompressed)
                {
                    /*var compressed = msgBuffer.ReadBytes(msgBuffer.LengthBytes - (int)(msgBuffer.Position / 8));
                    var compressedstream = new System.IO.MemoryStream(compressed);
                    var decompresser = new System.IO.Compression.GZipStream(compressedstream, System.IO.Compression.CompressionMode.Decompress);

                    x = (byte)decompresser.ReadByte();
                    y = (byte)decompresser.ReadByte();
                    propertyBag.mapLoadProgress[x, y] = true;
                    for (byte dy = 0; dy < 16; dy++)
                        for (byte z = 0; z < 64; z++)
                        {
                            BlockType blockType = (BlockType)decompresser.ReadByte();
                            if (blockType != BlockType.None)
                                blockEngine.downloadList[x, y + dy, z] = blockType;
                        }*/
                }

                else
                {
                    propertyBag.MapLoadProgress[packet.X, packet.Y] = true;
                    for (byte dy = 0; dy < 16; dy++)
                    {
                        for (byte z = 0; z < 64; z++)
                        {
                            var blockType = (BlockType)packet.BlockList[dy, z];
                            if (blockType != BlockType.None)
                                blockEngine.DownloadList[packet.X, packet.Y + dy, z] = blockType;
                        }
                    }
                }

                var downloadComplete = true;

                for (var x = 0; x < 64; x++)
                {
                    for (var y = 0; y < 64; y += 16)
                    {
                        if (propertyBag.MapLoadProgress[x, y]) continue;

                        downloadComplete = false;
                        break;
                    }
                }

                if (downloadComplete)
                {
                    gameInstance?.ChangeState("Infiniminer.States.TeamSelectionState");
                    if (!propertyBag.SettingsContainer.NoSound)
                        MediaPlayer.Stop();
                    blockEngine.DownloadComplete();
                }
            }
            catch (Exception e)
            {
                Console.OpenStandardError();
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.Close();
            }

            return null;
        }
    }
}
