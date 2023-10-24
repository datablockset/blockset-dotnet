// See https://aka.ms/new-console-template for more information

// var path = Path.Combine(Environment.CurrentDirectory, "../../../../workspace/cdt0/roots/py/9z/zswe24dm8zjqbw7htvbg54685nmsfhpzqh1zwkw60");

using System.ComponentModel.Design.Serialization;

using BlockSet.Main;
// ReSharper disable HeapView.BoxingAllocation

const string Root = "../../../../workspace/cdt0";

var bytesChunks = GetData(Path.Combine(Environment.CurrentDirectory, Root), "roots", "kgyzrjw9p1as373wn6kxcvhyq0jqyb4qq87y20wkha84x");
using var fileOutStream = File.OpenWrite(Path.Combine(Environment.CurrentDirectory, Root, "..", "text.txt"));
foreach (var bytesChunk in bytesChunks)
    fileOutStream.Write(bytesChunk);

Console.WriteLine("\n\nEnded.");

List<byte[]> GetData(string rootCdt0, string subFolder, string streamName)
{
    var path = Path.Combine(rootCdt0, subFolder, streamName[..2], streamName[2..4], streamName[4..]);

    var stream = (ReadOnlySpan<byte>)File.ReadAllBytes(path);

    if (stream.Length < 1) throw new Exception($"No data: {path}");
    var firstByte = stream[0];
    switch (firstByte)
    {
        case 0x20:
            stream = stream[1..];
            return new() { stream.ToArray() };

        case > 0x20:
            throw new Exception($"Unknown format: {path}");

        default:
        {
            var resBytes = new List<byte[]>(); 
            var tailLength = firstByte;
            var tail = stream[1..(tailLength + 1)];

            var body = stream[(tailLength + 1)..];
            while (body.Length > 0)
            {
                var chunk = body[0..28];
                body = body[28..];
                var digest = (UInt224)chunk;
                var base32 = digest.ToBase32();

                resBytes.AddRange(GetData(rootCdt0, "parts", base32));
                Console.WriteLine(base32);
            }

            resBytes.Add(tail.ToArray());
            
            return resBytes;
        }
    }
}