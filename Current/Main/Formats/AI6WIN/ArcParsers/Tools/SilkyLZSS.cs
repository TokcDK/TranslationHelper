using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesArcToolCSharp
{
    internal class SilkyLZSS
    {
        private readonly byte[] inputBuffer;
        private readonly bool debug;
        private readonly byte[] textBuffer;
        private readonly int[] lson;
        private readonly int[] rson;
        private readonly int[] dad;
        private int matchPosition;
        private int matchLength;
        private readonly int N;
        private readonly int F;
        private readonly int threshold;
        private readonly int nullValue;
        private readonly byte paddingByte;
        private readonly int progressPrint = 32768;

        internal SilkyLZSS(byte[] buffer, int N = 4096, int F = 18, int threshold = 2, int? nullVal = null,
                         bool debug = false, int progressPrint = 32768, byte paddingByte = 0x00)
        {
            this.debug = debug;
            inputBuffer = buffer;
            nullValue = nullVal ?? N;

            this.paddingByte = paddingByte;
            this.N = N;
            this.F = F;
            this.threshold = threshold;
            this.progressPrint = progressPrint;

            textBuffer = new byte[N + F - 1];
            matchPosition = 0;
            matchLength = 0;

            lson = new int[N + 1];
            rson = new int[N + 257];
            dad = new int[N + 1];

        }

        private void InitTree()
        {
            for (int i = N + 1; i < N + 257; i++)
            {
                rson[i] = nullValue;
            }
            for (int i = 0; i < N; i++)
            {
                dad[i] = nullValue;
            }
        }

        private void InsertNode(int r)
        {
            int cmp = 1;
            int p = N + 1 + textBuffer[r];
            rson[r] = nullValue;
            lson[r] = nullValue;
            matchLength = 0;

            while (true)
            {
                if (cmp >= 0)
                {
                    if (rson[p] != nullValue)
                    {
                        p = rson[p];
                    }
                    else
                    {
                        rson[p] = r;
                        dad[r] = p;
                        return;
                    }
                }
                else
                {
                    if (lson[p] != nullValue)
                    {
                        p = lson[p];
                    }
                    else
                    {
                        lson[p] = r;
                        dad[r] = p;
                        return;
                    }
                }
                int i;
                for (i = 1; i < F; i++)
                {
                    cmp = textBuffer[r + i] - textBuffer[p + i];
                    if (cmp != 0)
                    {
                        //i--; // commented to match original value
                        break;
                    }
                }
                //i++; // commented to match original value
                if (i > matchLength)
                {
                    matchPosition = p;
                    matchLength = i;
                    if (matchLength >= F)
                    {
                        break;
                    }
                }
            }
            dad[r] = dad[p];
            lson[r] = lson[p];
            rson[r] = rson[p];
            dad[lson[p]] = r;
            dad[rson[p]] = r;
            if (rson[dad[p]] == p)
            {
                rson[dad[p]] = r;
            }
            else
            {
                lson[dad[p]] = r;
            }
            dad[p] = nullValue;
        }

        private void DeleteNode(int p)
        {
            int q;
            if (dad[p] == nullValue)
            {
                return;
            }
            if (rson[p] == nullValue)
            {
                q = lson[p];
            }
            else if (lson[p] == nullValue)
            {
                q = rson[p];
            }
            else
            {
                q = lson[p];
                if (rson[q] != nullValue)
                {
                    q = rson[q];
                    while (rson[q] != nullValue)
                    {
                        q = rson[q];
                    }
                    rson[dad[q]] = lson[q];
                    dad[lson[q]] = dad[q];
                    lson[q] = lson[p];
                    dad[lson[p]] = q;
                }
                rson[q] = rson[p];
                dad[rson[p]] = q;
            }
            dad[q] = dad[p];
            if (rson[dad[p]] == p)
            {
                rson[dad[p]] = q;
            }
            else
            {
                lson[dad[p]] = q;
            }
            dad[p] = nullValue;
        }

        public byte[] Encode()
        {
            int r = N - F;
            int s = 0;
            int mask = 1;
            int codeBufPtr = mask;
            var outputBuffer = new List<byte>();
            int[] codeBuf = new int[17];
            int inputBufferLength = inputBuffer.Length;
            int n = N - 1;

            InitTree();

            codeBuf[0] = 0;

            int i;
            for (i = s; i < r; i++) textBuffer[i] = paddingByte;

            int length;
            for (length = 0; length < F; length++)
            {
                if (length >= inputBufferLength)
                {
                    break;
                }
                textBuffer[r + length] = inputBuffer[length];
            }

            if (length == 0) 
                return outputBuffer.ToArray();

            // comment for kind of loop when index variable like "length" or "i" here using outside of loop.
            // in the original using python range iterate function when value in range of start and end and in
            // the "for" loop it is more by 1 on the "for" end and it must be reduced by in using "i--" to be same
            // as in original if in original was not added something like "i++" after "range" loop
            for (i = 1; i <= F; i++) InsertNode(r - i);
            i--;
            InsertNode(r);

            int pos = i;

            int printCount = 0;

            while (true)
            {
                if (matchLength > length) 
                    matchLength = length;

                if (matchLength <= threshold)
                {
                    matchLength = 1;
                    codeBuf[0] |= (byte)mask;
                    codeBuf[codeBufPtr++] = textBuffer[r];
                }
                else
                {
                    codeBuf[codeBufPtr++] = UnsignedChar(matchPosition);
                    codeBuf[codeBufPtr++] = UnsignedChar(((matchPosition >> 4) & 0xf0) | (matchLength - (threshold + 1)));
                }

                mask <<= 1;
                mask %= 256;

                if (mask == 0)
                {
                    SetOutputBuffer(outputBuffer, codeBuf, codeBufPtr, ref i);

                    codeBuf[0] = 0;
                    codeBufPtr = 1;
                    mask = 1;
                }

                int lastMatchLength = matchLength;

                for (i = 0; i < lastMatchLength; i++)
                {
                    if (pos >= inputBufferLength)
                    {
                        break;
                    }
                    DeleteNode(s);
                    byte c = inputBuffer[pos++];
                    textBuffer[s] = c;

                    if (s < F - 1)
                    {
                        textBuffer[s + N] = c;
                    }

                    s = (s + 1) & n;
                    r = (r + 1) & n;
                    InsertNode(r);
                }

                if (pos > printCount)
                {
                    if (debug)
                    {
                        Console.Write($"{pos}\r");
                    }
                    printCount += progressPrint;
                }

                while (i < lastMatchLength)
                {
                    i++;
                    DeleteNode(s);
                    s = (s + 1) & n;
                    r = (r + 1) & n;

                    if (--length > 0) InsertNode(r);
                }
                i++;

                if (length <= 0) break;
            }

            if (codeBufPtr > 1)
            {
                SetOutputBuffer(outputBuffer, codeBuf, codeBufPtr, ref i);
            }

            if (debug)
            {
                Console.WriteLine($"In: {inputBufferLength} bytes.");
                Console.WriteLine($"Out: {outputBuffer.Count} bytes.");
                Console.WriteLine($"Out/In: {Math.Round((double)inputBufferLength / outputBuffer.Count, 4)}.");
            }

            return outputBuffer.ToArray();
        }

        private void SetOutputBuffer(List<byte> outputBuffer, int[] codeBuf, int codeBufPtr, ref int i)
        {
            for (i = 0; i < codeBufPtr; i++)
            {
                byte b = (byte)(codeBuf[i]);
                outputBuffer.Add(b);
            }
            i--;
        }

        byte ToByte(int i)
        {
            byte[] tempBytes = BitConverter.GetBytes(i); // Convert int to byte array

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(tempBytes); // If little-endian, reverse the byte order
            }

            return tempBytes[tempBytes.Length - 1]; // Take the most significant byte
        }

        public byte[] Decode()
        {
            var outputBuffer = new List<byte>();
            int r = N - F;
            int flags = 0;
            int inputBufferLength = inputBuffer.Length;
            int n = N - 1;

            InitTree();

            for (int i = 0; i < r; i++) textBuffer[i] = paddingByte;

            int currentPos = 0;

            while (currentPos < inputBufferLength)
            {
                flags >>= 1;

                if ((flags & 256) == 0)
                {
                    byte c = inputBuffer[currentPos++];
                    if (currentPos >= inputBufferLength) break;

                    flags = c | 0xff00;
                }

                if ((flags & 1) == 1)
                {
                    byte c = inputBuffer[currentPos++];
                    outputBuffer.Add(c);
                    textBuffer[r] = c;
                    r = (r + 1) & n;
                }
                else
                {
                    int i = inputBuffer[currentPos++];
                    if (currentPos >= inputBufferLength) break;

                    int j = inputBuffer[currentPos++];
                    i |= (j & 0xf0) << 4;
                    j = (j & 0x0f) + threshold;

                    for (int k = 0; k <= j; k++)
                    {
                        byte c = textBuffer[(i + k) & n];
                        outputBuffer.Add(c);
                        textBuffer[r] = c;
                        r = (r + 1) & n;
                    }
                }
            }

            return outputBuffer.ToArray();
        }

        private byte UnsignedChar(int value)
        {
            return (byte)(value % 256);
        }
    }
}
