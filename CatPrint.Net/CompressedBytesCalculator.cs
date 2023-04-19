namespace CatPrint.Net;

internal static class CompressedBytesCalculator
{
    public static byte[] GetCompressed(ICollection<bool> original)
    {

        var bytes = new List<byte>();

        int currentCount = 0;
        bool currentColor = false;
        var isFirstInSequence = true;
        foreach (var pixel in original)
        {
            if (isFirstInSequence)
            {
                currentColor = pixel;
                currentCount = 0;
                isFirstInSequence = false;
            }

            if (pixel == currentColor)
            {
                currentCount++;
                if (currentCount == 127)
                {
                    AppendByte();
                    isFirstInSequence = true;
                }
            }
            else
            {
                AppendByte();
                currentColor = pixel;
                currentCount = 1;
            }
        }
        AppendByte();

        return bytes.ToArray();

        void AppendByte()
        {
            var currentByte = (byte)currentCount;
            if (currentColor)
            {
                currentByte += 0b1000_0000;
            }
            bytes.Add(currentByte);
        }
    }

}