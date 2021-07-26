/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) LX4Cnh contributors https://github.com/3F/LX4Cnh/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using BenchmarkDotNet.Attributes;

namespace net.r_eg.algorithms.Tests
{
    /// <summary>
    /// Test of the optimized implementation (limited) of the LX4Cnh algorithm specially for Fnv1a128
    /// </summary>
    public class LX4CnhFnvMod
    {
        /*
        |            Method |      Mean |     Error |    StdDev |
        |------------------ |----------:|----------:|----------:|
        |            LX4Cnh | 2.6957 ns | 0.0180 ns | 0.0159 ns |
        | LX4CnhFnv1a128Mod | 0.8568 ns | 0.0060 ns | 0.0056 ns |
       */

        private const uint ma = 0, mb = 0x01000000, mc = 0, md = 0x0000013B, P128_BD = 0xFFFEC5;
        private ulong high, low;

        [Benchmark]
        public void LX4Cnh()
        {
            uint a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;

            unchecked{/*LX4Cnh for C# [1.1.0] (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if(((a<b)&&(ma>mb))||((a>b)&&(ma<mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=(((A>>28)+(A>>60)+(H>>60))<<28);ulong I=B;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;if(((c<d)&&(mc>md))||((c>d)&&(mc<md))){I+=F;if(B>I)C+=0x100000000;}else{I-=F;if(B<I)C-=0x100000000;}}ulong J=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+J+C+(I>>32);G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if(((G<I)&&(A>H))||((G>I)&&(A<H))){C+=F;}else{C-=F;}}low=J;high=C;}

            _ = high; _ = low;
        }

        [Benchmark]
        public void LX4CnhFnv1a128Mod()
        {
            ulong a = 0x6c62272e, b = 0x07bb0142, c = 0x62b82175, d = 0x6295c58d;

            ulong f = 0, fLm = 0;
            unchecked
            {
                // Below is an optimized implementation (limited) of the LX4Cnh algorithm specially for Fnv1a128
                // (c) Denis Kuzmin <x-3F@outlook.com> github/3F

                f = b * mb;

                ulong v = (uint)f;

                f = (f >> 32) + v;

                if(a > b)
                {
                    f += (uint)((a - b) * mb);
                }
                else if(a < b)
                {
                    f -= (uint)((b - a) * mb);
                }

                ulong fHigh = (f << 32) + (uint)v;
                ulong r2    = d * md;

                v = (r2 >> 32) + (r2 & 0xFFF_FFFF_FFFF_FFFF);

                f = (r2 & 0xF000_0000_0000_0000) >> 32;

                if(c > d)
                {
                    fLm = v;
                    v += (c - d) * md;
                    if(fLm > v) f += 0x100000000;
                }
                else if(c < d)
                {
                    fLm = v;
                    v -= (d - c) * md;
                    if(fLm < v) f -= 0x100000000;
                }

                fLm = (((ulong)(uint)v) << 32) + (uint)r2;

                f = fHigh + fLm + f + (v >> 32);

                fHigh   = (a << 32) + b; //fa
                v       = (c << 32) + d; //fb

                if(fHigh < v)
                {
                    f += (v - fHigh) * P128_BD;
                }
                else if(fHigh > v)
                {
                    f -= (fHigh - v) * P128_BD;
                }

                high = f;
                low = fLm;
            }

            _ = high; _ = low;
        }
    }
}
