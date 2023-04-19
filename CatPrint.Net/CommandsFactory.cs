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

    /// <summary>
    /// !Working only for GB03(aka new printers)!
    /// Print line of pixels
    /// </summary>
    /// <param name="pixels">Pixels collection in boolean format. Collection have to be 384 elements</param>
    /// <exception cref="ArgumentOutOfRangeException">Pixels length out of range</exception>
    /// <remarks>
    /// I not found clean information for which printers it have to work. Tested with my GB03
    /// </remarks>
    public Command CreatePrintLineCompressed(ICollection<bool> pixels)
    {
        // A compressed line is a sequence of bytes where bit 7 represents the
        // color (0 for white, 1 for black) and the low 7 bits represent a
        // count for pixels of that color.

        if (pixels.Count != 384)
        {
            throw new ArgumentOutOfRangeException(nameof(pixels), "Pixels array have to be 48 bytes");
        }

        var bytes = CompressedBytesCalculator.GetCompressed(pixels);
        return new Command(PrintLineCompressedCommand, bytes);
    }

    /// <summary>
    /// APK always sets 0x33 for GB01
    /// </summary>
    /// <param name="quality">0x31 - 0x35 Range</param>
    public Command CreateSetQuality(byte quality)
    {
        if (quality < 0x31 || quality > 0x35)
        {
            throw new ArgumentOutOfRangeException(nameof(quality), "Have to be between 0x31 and 0x35");
        }

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