using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatPrint.Net;

public class CommandsFactory
{
    public Command CreateFeedPaper(UInt16 steps)
    {
        var buffer = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, steps);
        return new(0xA1, buffer);
    }

}