namespace CatPrint.Net.Tests;

[TestClass]
public class CompressedBytesCalculatorTests
{
    [TestMethod]
    public void TestCompressedCommand1()
    {
        var line = Enumerable.Repeat(false, 384).ToList();
        var actualBytes = CompressedBytesCalculator.GetCompressed(line);
        var expectedBytes = new byte[] { 0x7f, 0x7f, 0x7f, 0x03 };
        CollectionAssert.AreEqual(expectedBytes, actualBytes);
    }

    [TestMethod]
    public void TestCompressedCommand2()
    {
        var line = Enumerable.Repeat(true, 384).ToList();
        var actualBytes = CompressedBytesCalculator.GetCompressed(line);

        var expectedBytes = new byte[] { 0xff, 0xff, 0xff, 0x83 };
        CollectionAssert.AreEqual(expectedBytes, actualBytes);
    }

    [TestMethod]
    public void TestCompressedCommand3()
    {
        // this is the top of a :nya_hearts:
        var lineStr = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000111111111111111111111111111111000000000111000000001111000000000000000111100000000000000000111111100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        var line = lineStr.Select(c => c == '1').ToList();
        var actualBytes = CompressedBytesCalculator.GetCompressed(line);

        var expectedBytes = new byte[] { 0x7f, 0x41, 0x9e, 0x09, 0x83, 0x08, 0x84, 0x0f, 0x84, 0x11, 0x87, 0x5f };
        CollectionAssert.AreEqual(expectedBytes, actualBytes);
    }

    [TestMethod]
    public void TestCompressedCommand4()
    {
        var line = Enumerable.Repeat(true, 64)
            .Concat(Enumerable.Repeat(false, 64))
            .ToList();
        var actualBytes = CompressedBytesCalculator.GetCompressed(line);

        var expectedBytes = new byte[] { 0b1100_0000, 0b0100_0000};
        CollectionAssert.AreEqual(expectedBytes, actualBytes);
    }
}