using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace HasTest
{
    internal class SHA512
    {
        private const int blockByteLength = 128, _paddingByteConst = 112;

        ulong h1;
        ulong h2;
        ulong h3;
        ulong h4;
        ulong h5;
        ulong h6;
        ulong h7;
        ulong h8;

        public SHA512()
        {
        }

        public string[] GenerateFromFile(String fileName)
        {
            FileStream fileStream = File.OpenRead(fileName);
            string resultSHA512 = GenerateSHA512FromFile(fileStream).ToLower();
            fileStream.Close();
            return resultSHA512;
        }

        private string GenerateSHA512FromFile(FileStream fs)
        {
            h1 = 0x6a09e667f3bcc908;
            h2 = 0xbb67ae8584caa73b;
            h3 = 0x3c6ef372fe94f82b;
            h4 = 0xa54ff53a5f1d36f1;
            h5 = 0x510e527fade682d1;
            h6 = 0x9b05688c2b3e6c1f;
            h7 = 0x1f83d9abfb41bd6b;
            h8 = 0x5be0cd19137e2179;

            UTF7Encoding utf = new UTF7Encoding(true);
            byte[] mBlockBytes = new byte[blockByteLength];
            long mlockCount = fs.Length / blockByteLength;
            int lastBlockLength = (int)(fs.Length % blockByteLength);
            byte[] mByteLength = BitConverter.GetBytes(fs.Length * 8);
            BufferedStream bufferedStream = new BufferedStream(fs);

            for (int i = 0; i <= mlockCount - 1; i++)
            {
                bufferedStream.Read(mBlockBytes, 0, mBlockBytes.Length);
                HashComputing(mBlockBytes);
            }

            if (lastBlockLength < _paddingByteConst)
            {
                mBlockBytes = new byte[blockByteLength];
                bufferedStream.Read(mBlockBytes, 0, mBlockBytes.Length);
                mBlockBytes[(int)(fs.Length) % blockByteLength] = 128;

                for (int i = 0; i <= mByteLength.Length - 1; i++)
                    mBlockBytes[blockByteLength - (i + 1)] = mByteLength[i];
                HashComputing(mBlockBytes);
            }
            else 
            {
                mBlockBytes = new byte[blockByteLength];
                bufferedStream.Read(mBlockBytes, 0, mBlockBytes.Length);
                mBlockBytes[(int)(fs.Length) % blockByteLength] = 128;
                HashComputing(mBlockBytes);

                mBlockBytes = new byte[blockByteLength];
                for (int i = 0; i <= mByteLength.Length - 1; i++)
                    mBlockBytes[blockByteLength - (i + 1)] = mByteLength[i];
                HashComputing(mBlockBytes);
            }

            string hash = (h1.ToString("X").PadLeft(16, '0') +
                    h2.ToString("X").PadLeft(16, '0') +
                    h3.ToString("X").PadLeft(16, '0') +
                    h4.ToString("X").PadLeft(16, '0') +
                    h5.ToString("X").PadLeft(16, '0') +
                    h6.ToString("X").PadLeft(16, '0') +
                    h7.ToString("X").PadLeft(16, '0') +
                    h8.ToString("X").PadLeft(16, '0'));
            return hash;
        }

        private void HashComputing(byte[] mBlock)
        {

            ulong[] K = {
                            0x428A2F98D728AE22, 0x7137449123EF65CD, 0xB5C0FBCFEC4D3B2F, 0xE9B5DBA58189DBBC,
                            0x3956C25BF348B538, 0x59F111F1B605D019, 0x923F82A4AF194F9B, 0xAB1C5ED5DA6D8118,
                            0xD807AA98A3030242, 0x12835B0145706FBE, 0x243185BE4EE4B28C, 0x550C7DC3D5FFB4E2,
                            0x72BE5D74F27B896F, 0x80DEB1FE3B1696B1, 0x9BDC06A725C71235, 0xC19BF174CF692694,
                            0xE49B69C19EF14AD2, 0xEFBE4786384F25E3, 0x0FC19DC68B8CD5B5, 0x240CA1CC77AC9C65,
                            0x2DE92C6F592B0275, 0x4A7484AA6EA6E483, 0x5CB0A9DCBD41FBD4, 0x76F988DA831153B5,
                            0x983E5152EE66DFAB, 0xA831C66D2DB43210, 0xB00327C898FB213F, 0xBF597FC7BEEF0EE4,
                            0xC6E00BF33DA88FC2, 0xD5A79147930AA725, 0x06CA6351E003826F, 0x142929670A0E6E70,
                            0x27B70A8546D22FFC, 0x2E1B21385C26C926, 0x4D2C6DFC5AC42AED, 0x53380D139D95B3DF,
                            0x650A73548BAF63DE, 0x766A0ABB3C77B2A8, 0x81C2C92E47EDAEE6, 0x92722C851482353B,
                            0xA2BFE8A14CF10364, 0xA81A664BBC423001, 0xC24B8B70D0F89791, 0xC76C51A30654BE30,
                            0xD192E819D6EF5218, 0xD69906245565A910, 0xF40E35855771202A, 0x106AA07032BBD1B8,
                            0x19A4C116B8D2D0C8, 0x1E376C085141AB53, 0x2748774CDF8EEB99, 0x34B0BCB5E19B48A8,
                            0x391C0CB3C5C95A63, 0x4ED8AA4AE3418ACB, 0x5B9CCA4F7763E373, 0x682E6FF3D6B2B8A3,
                            0x748F82EE5DEFB2FC, 0x78A5636F43172F60, 0x84C87814A1F0AB72, 0x8CC702081A6439EC,
                            0x90BEFFFA23631E28, 0xA4506CEBDE82BDE9, 0xBEF9A3F7B2C67915, 0xC67178F2E372532B,
                            0xCA273ECEEA26619C, 0xD186B8C721C0C207, 0xEADA7DD6CDE0EB1E, 0xF57D4F7FEE6ED178,
                            0x06F067AA72176FBA, 0x0A637DC5A2C898A6, 0x113F9804BEF90DAE, 0x1B710B35131C471B,
                            0x28DB77F523047D84, 0x32CAAB7B40C72493, 0x3C9EBE0A15C9BEBC, 0x431D67C49C100D4C,
                            0x4CC5D4BECB3E42B6, 0x597F299CFC657E2A, 0x5FCB6FAB3AD6FAEC, 0x6C44198C4A475817
                        };

            ulong a = h1;
            ulong b = h2;
            ulong c = h3;
            ulong d = h4;
            ulong e = h5;
            ulong f = h6;
            ulong g = h7;
            ulong h = h8;

            ulong[] w = new ulong[80];


            for (int n = 0; n <= 79; n++)
            {
                if (n <= 15)
                {
                    w[n] = (ulong)mBlock[n * 8] * (ulong)Math.Pow(16.0, 14.0) +
                           (ulong)mBlock[n * 8 + 1] * (ulong)Math.Pow(16.0, 12.0) +
                           (ulong)mBlock[n * 8 + 2] * (ulong)Math.Pow(16.0, 10.0) +
                           (ulong)mBlock[n * 8 + 3] * (ulong)Math.Pow(16.0, 8.0) +
                           (ulong)mBlock[n * 8 + 4] * (ulong)Math.Pow(16.0, 6.0) +
                           (ulong)mBlock[n * 8 + 5] * (ulong)Math.Pow(16.0, 4.0) +
                           (ulong)mBlock[n * 8 + 6] * (ulong)Math.Pow(16.0, 2.0) +
                           (ulong)mBlock[n * 8 + 7];
                }
                else
                {
                    ulong p0 = (((w[n - 15]) >> (1)) | ((w[n - 15]) << (64 - (1)))) ^ (((w[n - 15]) >> (8)) | ((w[n - 15]) << (64 - (8)))) ^ (w[n - 15] >> 7);
                    ulong p1 = (((w[n - 2]) >> (19)) | ((w[n - 2]) << (64 - (19)))) ^ (((w[n - 2]) >> (61)) | ((w[n - 2]) << (64 - (61)))) ^ (w[n - 2] >> 6);
                    w[n] = (p1 + w[n - 7] + p0 + w[n - 16]);
                }

                ulong s1 = ((e >> 14) | (e << (64 - 14))) ^ ((e >> 18) | (e << (64 - 18))) ^ ((e >> 41) | (e << (64 - 41)));
                ulong ch = (e & f) ^ ((~e) & g);
                ulong t1 = h + s1 + ch + K[n] + w[n];
                ulong s0 = (((a) >> (28)) | ((a) << (64 - (28)))) ^ (((a) >> (34)) | ((a) << (64 - (34)))) ^ (((a) >> (39)) | ((a) << (64 - (39))));
                ulong maj = (a & b) ^ (a & c) ^ (b & c);
                ulong t2 = s0 + maj;

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
