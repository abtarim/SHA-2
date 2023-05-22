using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HashTest
{

    internal class Keccak
    {
        public Keccak()
        {
        }

        public string[] GenerateFromFile(String fileName, int digest, int l, byte padVal)
        {
            int w = (int)Math.Pow(2.0, (double)l);
            int c = digest * 2;

            FileStream fileStream = File.OpenRead(fileName);
            string resultKeccak = KeccakSponge(fileStream, c, w, padVal).ToLower();
            fileStream.Close();

            return resultKeccak;
        }

        private string KeccakSponge(FileStream m, int c, int w, byte padVal)
        {
            int r = 25 * w - c;
            int rByte = r / 8;
            int cByte = c / 8;
            int l = (int)Math.Log((double)w, 2.0);
            ulong[,] spongeState, blockState;
            int mLastBlockLength = (int)m.Length % rByte;
            string mBlock = "";
            long mBlockCount = m.Length / rByte;
            byte[] mBlockBytes = new byte[rByte + cByte];
            int bByte = rByte + cByte;
            spongeState = new ulong[5, 5];
            blockState = new ulong[5, 5];
            BufferedStream bufferedStream = new BufferedStream(m);

            for (int i = 0; i <= mBlockCount - 1; i++)
            {
                bufferedStream.Read(mBlockBytes, 0, rByte);
                mBlockBytes = Padding(mBlockBytes, rByte, cByte, padVal);
                blockState = GenerateState(mBlockBytes);
                spongeState = DoXor(spongeState, blockState);
                spongeState = Round(spongeState, l);
            }

            // Absorbing
            mBlockBytes = new byte[mLastBlockLength];
            bufferedStream.Read(mBlockBytes, 0, mLastBlockLength);
            mBlockBytes = Padding(mBlockBytes, rByte, cByte, padVal);
            blockState = GenerateState(mBlockBytes);
            spongeState = DoXor(spongeState, blockState);
            spongeState = Round(spongeState, l);

            // Squeezing
            string z = Squeezing(spongeState, c);
            return z;
        }

        private byte[] Padding(byte[] mBlock, int r, int c, byte padVal)
        {
            byte[] tempBlockBytes = new byte[r + c];

            int i = 0;
            for (; i <= mBlock.Length - 1; i++)
                tempBlockBytes[i] = mBlock[i];

            if (mBlock.Length < r)
            {
                tempBlockBytes[mBlock.Length] = padVal;
                tempBlockBytes[r - 1] = 128;
            }

            return tempBlockBytes;
        }

        private ulong[,] GenerateState(byte[] p)
        {

            int pLength = p.Length / 25;
            ulong[,] state = new ulong[5, 5];
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    ulong val = 0x0;
                    for (int z = 0; z < pLength; z++)
                    {
                        val += (ulong)p[(5 * y + x) * pLength + z] * (ulong)Math.Pow(2 * pLength, 2 * z);
                    }

                    state[x, y] ^= val;
                }
            }
            return state;
        }

        private ulong[,] Round(ulong[,] a, int l)
        {
            int n = 12 + 2 * l;
            

            ulong[] rc = {
                0x0000000000000001, 0x0000000000008082, 0x800000000000808A, 0x8000000080008000,
                0x000000000000808B, 0x0000000080000001, 0x8000000080008081, 0x8000000000008009,
                0x000000000000008A, 0x0000000000000088, 0x0000000080008009, 0x000000008000000A,
                0x000000008000808B, 0x800000000000008B, 0x8000000000008089, 0x8000000000008003,
                0x8000000000008002, 0x8000000000000080, 0x000000000000800A, 0x800000008000000A,
                0x8000000080008081, 0x8000000000008080, 0x0000000080000001, 0x8000000080008008
            };


            int[,] r = new int[5, 5];
            r[0, 0] = 0;
            r[0, 1] = 36;
            r[0, 2] = 3;
            r[0, 3] = 41;
            r[0, 4] = 18;

            r[1, 0] = 1;
            r[1, 1] = 44;
            r[1, 2] = 10;
            r[1, 3] = 45;
            r[1, 4] = 2;

            r[2, 0] = 62;
            r[2, 1] = 6;
            r[2, 2] = 43;
            r[2, 3] = 15;
            r[2, 4] = 61;

            r[3, 0] = 28;
            r[3, 1] = 55;
            r[3, 2] = 25;
            r[3, 3] = 21;
            r[3, 4] = 56;

            r[4, 0] = 27;
            r[4, 1] = 20;
            r[4, 2] = 39;
            r[4, 3] = 8;
            r[4, 4] = 14;


            for (int i = 0; i <= n - 1; i++)
            {
                ulong[] c = new ulong[5];
                ulong[] d = new ulong[5];
                ulong[,] A = new ulong[5, 5];

                Parallel.For(0, 5, x => 
                { 
                
                });


                //  θ step  ----------------------------------------------------------
                for (int x = 0; x <= 4; x++)
                {
                    c[x] = a[x, 0] ^ a[x, 1] ^ a[x, 2] ^ a[x, 3] ^ a[x, 4];
                }


                for (int x = 0; x <= 4; x++)
                {
                    d[x] = c[(x + 4) % 5] ^ ((c[(x + 1) % 5] << 1) | (c[(x + 1) % 5] >> (64 - 1)));
                    for (int y = 0; y <= 4; y++)
                    {
                        a[x, y] = a[x, y] ^ d[x];
                    }
                }

                //  ρ step  ----------------------------------------------------------
                for (int x = 0; x <= 4; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        a[x, y] = (a[x, y] << r[x, y]) | (a[x, y] >> (64 - r[x, y]));
                    }
                }

                //  π step  --------------------------------------------------------
                for (int x = 0; x <= 4; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        A[y, (2 * x + 3 * y) % 5] = a[x, y];
                    }
                }

                //  χ step  ---------------------------------------------------------------
                for (int x = 0; x <= 4; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        a[x, y] = A[x, y] ^ ((~A[(x + 1) % 5, y]) & A[(x + 2) % 5, y]);
                    }
                }

                //  ι step  --------------------------------------------------------
                a[0, 0] = a[0, 0] ^ rc[i];
            }

            return a;
        }

        private string Squeezing(ulong[,] state, int c)
        {
            string z = "";
            string[] result = new string[25];
            int k = 0;
            for (int y = 0; y <= 4; y++)
            {
                for (int x = 0; x <= 4; x++, k++)
                {
                    result[k] = state[x, y].ToString("X").PadLeft(8, '0');
                }
            }

            int t = c % 128;
            int i = 0;
            for (; i < c / 128; i++)
            {
                if (result[i].Length < 16)
                    result[i] = "0" + result[i];
                for (int j = 0; j < result[i].Length / 2; j++)
                    z += result[i].Substring(result[i].Length - 2 - 2 * j, 2);
            }

            for (int j = 0; j < t / 16; j++)
                z += result[i].Substring(result[i].Length - 2 - 2 * j, 2);


            return z;
        }


        private ulong[,] DoXor(ulong[,] state, ulong[,] p)
        {
            for (int x = 0; x <= 4; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    p[x, y] = p[x, y] ^ state[x, y];
                }
            }
            return p;
        }

    }
}
