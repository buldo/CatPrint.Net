namespace CatPrint.Net.Tests;

[TestClass]
public class CommandTest
{
    [TestMethod]
    public void Test1()
    {
        var command = new Command(0xA3, new byte[] { 0x00 });

        var actual = command.AsBytes();
        var expected = new byte[] { 0x51, 0x78, 0xa3, 0x00, 0x01, 0x00, 0x00, 0x00, 0xff };

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test2()
    {
        var command = new Command(0xA4, new byte[] { 0x33 });

        var actual = command.AsBytes();
        var expected = new byte[] { 0x51, 0x78, 0xa4, 0x00, 0x01, 0x00, 0x33, 0x99, 0xff };

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test3()
    {
        var command = new Command(0xAF, new byte[] { 0x5c, 0x44 });

        var actual = command.AsBytes();
        var expected = new byte[] { 0x51, 0x78, 0xaf, 0x00, 0x02, 0x00, 0x5c, 0x44, 0x2b, 0xff };

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test4()
    {
        var command = new Command(0xA6, new byte[] { 0xaa, 0x55, 0x17, 0x38, 0x44, 0x5f, 0x5f, 0x5f, 0x44, 0x38, 0x2c });

        var actual = command.AsBytes();
        var expected = new byte[] { 0x51, 0x78, 0xa6, 0x00, 0x0b, 0x00, 0xaa, 0x55, 0x17, 0x38, 0x44, 0x5f, 0x5f, 0x5f, 0x44, 0x38, 0x2c, 0xa1, 0xff };

        CollectionAssert.AreEqual(expected, actual);
    }
}