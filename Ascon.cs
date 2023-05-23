using System;
using System.Diagnostics;
using System.IO;

namespace HashTest
{
    internal class Ascon
    {
        public Ascon()
        {
        }

        public string CreateFromFile(String fileName, int pa, int pb)
        {
            ulong[] state = new ulong[5];
            FileStream fileStream = File.OpenRead(fileName);

            state = Initialization(state);
            state = Absorbing(state, fileStream, pa);
            string H = Squeezing(state);

            string resultAscon = H.ToLower();
            fileStream.Close();
            return resultAscon;
        }

        private ulong[] Initialization(ulong[] state)
        {
            state[0] = 0x00400c0000000100;
            Permutation(state, 12);

            return state;
        }


        private ulong[] Absorbing(ulong[] state, FileStream m, int p)
        {
            int rByte = 8;
            int mLastBlockLength = (int)m.Length % rByte;
            long mBlockCount = m.Length / rByte;
            byte[] mBlockBytes = new byte[rByte];

            BufferedStream bufferedStream = new BufferedStream(m);

            if (mBlockCount == 0)
            {
                bufferedStream.Read(mBlockBytes, 0, rByte);
                state[0] ^= Tools.BytesToUlong(Padding(mBlockBytes, mLastBlockLength));
            }
            else
            {
                for (int i = 0; i < mBlockCount; i++)
                {
                    bufferedStream.Read(mBlockBytes, 0, rByte);
                    state[0] ^= Tools.BytesToUlong(mBlockBytes);
                    state = Permutation(state, 12);
                }

                bufferedStream.Read(mBlockBytes, 0, rByte);
                state[0] ^= Tools.BytesToUlong(Padding(mBlockBytes, mLastBlockLength));
            }

            return state;
        }
        
        private byte[] Padding(byte[] mBlock, int mLastBlockLength)
        {
            byte[] tempBlock = new byte[8];

            for (int i = 0; i < mLastBlockLength; i++)
                tempBlock[i] = mBlock[i];

            tempBlock[mLastBlockLength] = 128;
            return tempBlock;
        }

        private ulong[] Permutation(ulong[] state, int p)
        {
            for (int i = 0; i < p; i++)
            {
                state = AdditionOfConstants(state, i, p);
                state = SubstitutionLayer(state);
                state = LinearDiffusionLayer(state);
            }
            return state; ;
        }

        private ulong[] AdditionOfConstants(ulong[] state, int i, int p)
        {
            ulong[] cr = { 0xf0, 0xe1, 0xd2, 0xc3, 0xb4, 0xa5, 0x96, 0x87, 0x78, 0x69, 0x5a, 0x4b };

            state[2] ^= cr[i + 12 - p];
            return state;
        }

        private unsafe ulong[] SubstitutionLayer(ulong[] x)
        {
            ulong[] t = new ulong[5];

            x[0] ^= x[4];
            x[4] ^= x[3];
            x[2] ^= x[1];

            t[0] = x[0];
            t[1] = x[1];
            t[2] = x[2];
            t[3] = x[3];
            t[4] = x[4];

            t[0] = ~t[0];
            t[1] = ~t[1];
            t[2] = ~t[2];
            t[3] = ~t[3];
            t[4] = ~t[4];

            t[0] &= x[1];
            t[1] &= x[2];
            t[2] &= x[3];
            t[3] &= x[4];
            t[4] &= x[0];

            x[0] ^= t[1];
            x[1] ^= t[2];
            x[2] ^= t[3];
            x[3] ^= t[4];
            x[4] ^= t[0];

            x[1] ^= x[0];
            x[0] ^= x[4];
            x[3] ^= x[2];
            x[2] = ~x[2];

            return x;
        }

        private ulong[] LinearDiffusionLayer(ulong[] state)
        {
            ulong temp1 = new ulong();
            ulong temp2 = new ulong();
            int[,] bits = { { 19, 28 }, { 61, 39 }, { 1, 6 }, { 10, 17 }, { 7, 41 } };

            for (int x = 0; x < state.Length; x++)
            {
                temp1 = RotateRight(state[x], bits[x, 0]);
                temp2 = RotateRight(state[x], bits[x, 1]);
                state[x] ^= temp1 ^ temp2;
            }

            return state;
        }

        private ulong RotateRight(ulong value, int bits)
        {
            return (value >> bits | value << (64 - bits));
        }


        private string Squeezing(ulong[] state)
        {
            string[] h = new string[4];
            state = Permutation(state, 12);

            for (int i = 0; i < 4; i++)
            {
                h[i] = state[0].ToString("X").PadLeft(16, '0');
                state = Permutation(state, 12);
            }

            return h[0] + h[1] + h[2] + h[3];
        }

    }
}
