using System.Text;

namespace BlockSet.Main;

public struct UInt224
{
    public UInt128 Lo;
    public UInt128 Hi;

    public static explicit operator UInt224(ReadOnlySpan<byte> data)
    {
        if (data.Length != 28) throw new ArgumentOutOfRangeException(nameof(data));
        var dataLo = data[0..16];
        var dataHi = data[16..];
        var res = new UInt224()
        {
            Lo = (UInt128)dataLo[0] | (UInt128)dataLo[1] << 8 | (UInt128)dataLo[2] << 16 | (UInt128)dataLo[3] << 24
                | (UInt128)dataLo[4] << 32 | (UInt128)dataLo[5] << 40 | (UInt128)dataLo[6] << 48 | (UInt128)dataLo[7] << 56
                | (UInt128)dataLo[8] << 64 | (UInt128)dataLo[9] << 72 | (UInt128)dataLo[10] << 80 | (UInt128)dataLo[11] << 88
                | (UInt128)dataLo[12] << 96 | (UInt128)dataLo[13] << 104 | (UInt128)dataLo[14] << 112 | (UInt128)dataLo[15] << 120,
            Hi = (UInt128)dataHi[0] | (UInt128)dataHi[1] << 8 | (UInt128)dataHi[2] << 16 | (UInt128)dataHi[3] << 24
                | (UInt128)dataHi[4] << 32 | (UInt128)dataHi[5] << 40 | (UInt128)dataHi[6] << 48 | (UInt128)dataHi[7] << 56
                | (UInt128)dataHi[8] << 64 | (UInt128)dataHi[9] << 72 | (UInt128)dataHi[10] << 80 | (UInt128)dataHi[11] << 88
        };
        return res;
    }

    public override string ToString() => $"{Lo:X},{Hi:X}";

    public string ToBase32()
    {
        var sb = new StringBuilder(45);
        for (var i = 0; i < 45; ++i)
        {
            sb.Append(ToChar(i));
        }

        return sb.ToString();
    }

    private char ToChar(int index)
    {
        var b = GetChunk(index);
        const string s = "0123456789abcdefghjkmnpqrstvwxyz";
        return s[b];
    }

    private byte GetChunk(int index)
    {
        var i = index * 5;
        return i switch
        {
            < 125 => (byte)(0x1F & (byte)(Lo >> i)),
            125 => (byte)((0x07 & (byte)(Lo >> 125)) | (0x18 & (byte)(Hi << 3))),
            220 => (byte)((0x1F & (byte)(Hi >> 220)) | Parity() << 4),
            > 125 => (byte)(0x1F & (byte)(Hi >> (i - 128))),
        };
    }

    private int Parity()
    {
        var x = Lo ^ Hi;
        x = (x >> 64) ^ x;
        x = (x >> 32) ^ x;
        x = (x >> 16) ^ x;
        x = (x >> 8) ^ x;
        x = (x >> 4) ^ x;
        x = (x >> 2) ^ x;
        x = (x >> 1) ^ x;
        return (int)x & 1;
    }
}