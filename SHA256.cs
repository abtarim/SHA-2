using System;
using System.IO;

namespace HashFunctions
{
    internal class SHA256
    {
        private const int blockByteLength = 64, _paddingByteConst = 56;

        uint h1;
        uint h2;
        uint h3;
        uint h4;
        uint h5;
        uint h6;
        uint h7;
        uint h8;

        public SHA256()
        {
        }

        public string GenerateFromFile(String fileName)
        {
            FileStream fileStream = File.OpenRead(fileName);
            string result = GenerateSHA256FromFile(fileStream).ToLower();
            fileStream.Close();
            return result;
        }

        private string GenerateSHA256FromFile(FileStream fS)
        {
            h1 = 0x6A09E667;
            h2 = 0xBB67AE85;
            h3 = 0x3C6EF372;
            h4 = 0xA54FF53A;
            h5 = 0x510E527F;
            h6 = 0x9B05688C;
            h7 = 0x1F83D9AB;
            h8 = 0x5BE0CD19;

            byte[] mBlockBytes = new byte[blockByteLength];
            long mBlockCount = fS.Length / blockByteLength;
            int lastBlockLength = (int)(fS.Length % blockByteLength);
            byte[] mByteLength = BitConverter.GetBytes(fS.Length * 8);
            BufferedStream bufferedStream = new BufferedStream(fS);

            for (int i = 0; i <= mBlockCount - 1; i++)
            {
                bufferedStream.Read(mBlockBytes, 0, mBlockBytes.Length);
                HashComputing(mBlockBytes);
            }

            if (lastBlockLength < _paddingByteConst)
            {
                mBlockBytes = new byte[blockByteLength];
                bufferedStream.Read(mBlockBytes, 0, mBlockBytes.Length);
                mBlockBytes[lastBlockLength] = 128;


                for (int i = 0; i <= mByteLength.Length - 1; i++)
                    mBlockBytes[blockByteLength - (i + 1)] = mByteLength[i];
                HashComputing(mBlockBytes);
            }
            else
            {
                mBlockBytes = new byte[blockByteLength];
                bufferedStream.Read(mBlockBytes, 0, mBlockBytes.Length);
                mBlockBytes[lastBlockLength] = 128;
                HashComputing(mBlockBytes);

                mBlockBytes = new byte[blockByteLength];
                for (int i = 0; i <= mByteLength.Length - 1; i++)
                    mBlockBytes[blockByteLength - (i + 1)] = mByteLength[i];
                HashComputing(mBlockBytes);
            }

            string hash = (h1.ToString("X").PadLeft(8, '0') +
                    h2.ToString("X").PadLeft(8, '0') +
                    h3.ToString("X").PadLeft(8, '0') +
                    h4.ToString("X").PadLeft(8, '0') +
                    h5.ToString("X").PadLeft(8, '0') +
                    h6.ToString("X").PadLeft(8, '0') +
                    h7.ToString("X").PadLeft(8, '0') +
                    h8.ToString("X").PadLeft(8, '0'));
            return hash;
        }

        private void HashComputing(byte[] mBlock)
        {
            uint[] k = {
                            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
                            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
                            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
                            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
                            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
                            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
                            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
                            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
                           };

            uint a = h1;
            uint b = h2;
            uint c = h3;
            uint d = h4;
            uint e = h5;
            uint f = h6;
            uint g = h7;
            uint h = h8;

            uint[] w = new uint[64];

            for (int n = 0; n <= 63; n++)
            {
                if (n <= 15)
                {
                    w[n] = (uint)mBlock[n * 4] * (uint)Math.Pow(16.0, 6.0) +
                           (uint)mBlock[n * 4 + 1] * (uint)Math.Pow(16.0, 4.0) +
                           (uint)mBlock[n * 4 + 2] * (uint)Math.Pow(16.0, 2.0) +
                           (uint)mBlock[n * 4 + 3];
                }
                else
                {
                    uint s0 = (((w[n - 15]) >> (7)) | ((w[n - 15]) << (32 - (7)))) ^ (((w[n - 15]) >> (18)) | ((w[n - 15]) << (32 - (18)))) ^ (w[n - 15] >> 3);
                    uint s1 = (((w[n - 2]) >> (17)) | ((w[n - 2]) << (32 - (17)))) ^ (((w[n - 2]) >> (19)) | ((w[n - 2]) << (32 - (19)))) ^ (w[n - 2] >> 10);

                    w[n] = (s1 + w[n - 7] + s0 + w[n - 16]);
                }

                uint S1 = (((e) >> (6)) | ((e) << (32 - (6)))) ^ (((e) >> (11)) | ((e) << (32 - (11)))) ^ (((e) >> (25)) | ((e) << (32 - (25))));
                uint ch = (e & f) ^ ((~e) & g);
                uint t1 = h + S1 + ch + k[n] + w[n];
                uint S0 = (((a) >> (2)) | ((a) << (32 - (2)))) ^ (((a) >> (13)) | ((a) << (32 - (13)))) ^ (((a) >> (22)) | ((a) << (32 - (22))));
                uint maj = (a & b) ^ (a & c) ^ (b & c);
                uint t2 = S0 + maj;

                h = g;
                g = f;
                f = e;
                e = d + t1;
                d = c;
                c = b;
                b = a;
                a = t1 + t2;
            }

            h1 += a;
            h2 += b;
            h3 += c;
            h4 += d;
            h5 += e;
            h6 += f;
            h7 += g;
            h8 += h;
        }
    }
}
