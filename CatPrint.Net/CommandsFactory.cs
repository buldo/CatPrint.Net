using System.Buffers.Binary;

namespace CatPrint.Net;

public class CommandsFactory
{
    private const byte FeedPaperCommand = 0xA1;
    private const byte PrintLineCommand = 0xA2;
    private const byte PrintLineCompressedCommand = 0xBF;
    private const byte SetQualityCommand = 0xAF;
    private const byte SetModeCommand = 0xBE;


    public Command CreateFeedPaper(UInt16 steps)
    {
        var buffer = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, steps);
        return new(FeedPaperCommand, buffer);
    }

    /// <summary>
    /// Print line of pixels
    /// </summary>
    /// <param name="pixels">Array with pixels. 1 bit per pixel. Array have to be 48 bytes</param>
    /// <exception cref="ArgumentOutOfRangeException">Pixels length out of range</exception>
    public Command CreatePrintLine(byte[] pixels)
    {
        if (pixels.Length != 48)
        {
            throw new ArgumentOutOfRangeException(nameof(pixels), "Pixels array have to be 48 bytes");
        }

        return new(PrintLineCommand, pixels);
    }

    /// <summary>
    /// Print line of pixels
    /// </summary>
    /// <param name="pixels">Pixels collection in boolean format. Collection have to be 384 elements</param>
    /// <exception cref="ArgumentOutOfRangeException">Pixels length out of range</exception>
    public Command CreatePrintLine(ICollection<bool> pixels)
    {
        if (pixels.Count != 384)
        {
            throw new ArgumentOutOfRangeException(nameof(pixels), "Pixels array have to be 48 bytes");
        }

        var lineBytes = new byte[48];

        int i = 0;
        foreach (var pixel in pixels)
        {
            if (pixel)
            {
                var idx = i / 8;
                var bit = i % 8;
                lineBytes[idx] |= (byte)(1 << bit);
            }

            i++;
        }

        return CreatePrintLine(lineBytes);
    }

    public Command CreateSetQuality(byte quality)
    {
        return new(SetQualityCommand, new[] { quality });
    }

    public Command CreateSetMode(Mode mode)
    {
        var modeByte = mode switch
        {
            Mode.Image => (byte)0x00,
            Mode.Text => (byte)0x01,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        return new(SetModeCommand, new[] { modeByte });
    }
}