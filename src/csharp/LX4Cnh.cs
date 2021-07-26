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

namespace net.r_eg.algorithms
{
    /// <summary>
    /// The high-speed multiplications of 128-bit numbers using LodgeX4CorrNoHigh (LX4Cnh) algorithm.
    /// </summary>
    /// <remarks>
    /// Use embeddable version to reduce the amount of unnecessary stack manipulations. <br/>
    /// https://github.com/3F/LX4Cnh
    /// </remarks>
    public static class LX4Cnh
    {
        /* --- Embeddable superfast version --- */

        //uint a = 0xC1F42719, b = 0x80F30FED, c = 0x81EF70CC, d = 0xBC6EF2EF;
        //uint ma = 0xDEF03F01, mb = 0x42D0ACD2, mc = 0x1749BEF1, md = 0xEA30FF94;
        ////-
        //ulong high, low;
        //unchecked{/*LX4Cnh for C# [1.1.0] (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if(((a<b)&&(ma>mb))||((a>b)&&(ma<mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=(((A>>28)+(A>>60)+(H>>60))<<28);ulong I=B;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;if(((c<d)&&(mc>md))||((c>d)&&(mc<md))){I+=F;if(B>I)C+=0x100000000;}else{I-=F;if(B<I)C-=0x100000000;}}ulong J=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+J+C+(I>>32);G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if(((G<I)&&(A>H))||((G>I)&&(A<H))){C+=F;}else{C-=F;}}low=J;high=C;}

        /* -- */

        /// <summary>
        /// High-speed multiplication of a 128-bit x 128-bit numbers.
        /// </summary>
        /// <param name="a">The first high 32 bits of the input value.</param>
        /// <param name="b">Second high 32 bits of the input value.</param>
        /// <param name="c">The first low 32 bits of the input value.</param>
        /// <param name="d">Second low 32 bits of the input value.</param>
        /// <param name="ma">The first high 32 bits of the multiplier.</param>
        /// <param name="mb">Second high 32 bits of the multiplier.</param>
        /// <param name="mc">The first low 32 bits of the multiplier.</param>
        /// <param name="md">Second low 32 bits of the multiplier.</param>
        /// <param name="low">Low 6 bits from a 128-bit result.</param>
        /// <returns>High 64 bits from a 128-bit result.</returns>
        public static ulong Multiply(uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md, out ulong low)
        {
            unchecked
            {
                ulong r = (ulong)b * mb; // r12
                ulong v = r & 0xFFFF_FFFF; // v1Low

                // we do not use the high bytes in the first block, therefore first low 4 bytes will be enough
                ulong f = ((r >> 32) + v + (a * ma)) & 0xFFFF_FFFF; // f12

                ulong d1 = AbsMinus(a, b); //d11, 0 - FFFF_FFFF
                ulong d2 = AbsMinus(ma, mb); //d12

                if(d1 != 0 && d2 != 0)
                {
                    ulong dd = d1 * d2; // 0 - FFFF_FFFE_0000_0001

                    if(((a < b) && (ma > mb)) || ((a > b) && (ma < mb)))
                    {
                        f += dd & 0xFFFF_FFFF;
                    }
                    else
                    {
                        f -= dd & 0xFFFF_FFFF;
                    }
                }

                ulong fHigh = (f << 32) + v;

                r /*r21*/   = (ulong)c * mc;
                ulong r2    = (ulong)d * md;

                v = (r2 >> 32) + (r2 & 0xFFF_FFFF_FFFF_FFFF) + (r & 0xFFF_FFFF_FFFF_FFFF) + ((r & 0xFFF_FFFF) << 32); // v2Middle

                f /*f21*/   = (((r >> 28) + (r >> 60) + (r2 >> 60)) << 28);
                ulong f2    = v; //f22
            
                d1 = AbsMinus(c, d);
                d2 = AbsMinus(mc, md);

                if(d1 != 0 && d2 != 0)
                {
                    ulong dd = d1 * d2; // 0 - FFFF_FFFE_0000_0001

                    if(((c < d) && (mc > md)) || ((c > d) && (mc < md)))
                    {
                        f2 += dd;
                        if(v > f2) f += 0x100000000;
                    }
                    else
                    {
                        f2 -= dd;
                        if(v < f2) f -= 0x100000000;
                    }
                }

                ulong fLowMiddle = ((f2 & 0xFFFF_FFFF) << 32) + (r2 & 0xFFFF_FFFF);

                // overflow is possible but for current 128-bit it's most high numbers
                f = fHigh + fLowMiddle + f + (f2 >> 32); // resHigh

                fHigh   = ((ulong)a << 32) + b; //fa
                f2      = ((ulong)c << 32) + d; //fb
                r       = ((ulong)ma << 32) + mb; //fma 
                r2      = ((ulong)mc << 32) + md; //fmb

                d1 = AbsMinus(fHigh, f2);
                d2 = AbsMinus(r, r2);

                if(d1 != 0 && d2 != 0)
                {
                    ulong dd = d1 * d2;

                    if(((fHigh < f2) && (r > r2)) || ((fHigh > f2) && (r < r2)))
                    {
                        f += dd;
                    }
                    else
                    {
                        f -= dd;
                    }
                }

                low = fLowMiddle;
                return f;
            }
        } /**/

        /// <summary>
        /// High-speed multiplication of a 128-bit x 128-bit numbers.
        /// </summary>
        /// <param name="a">High 64 bits of the input value.</param>
        /// <param name="b">Low 64 bits of the input value.</param>
        /// <param name="ma">High 64 bits of the multiplier.</param>
        /// <param name="mb">Low 64 bits of the multiplier.</param>
        /// <param name="low">Low 64 bits from a 128-bit result.</param>
        /// <returns>High 64 bits from a 128-bit result.</returns>
        public static ulong Multiply(ulong a, ulong b, ulong ma, ulong mb, out ulong low) => Multiply
        (
            (uint)(a >> 32), (uint)a, (uint)(b >> 32), (uint)b,
            (uint)(ma >> 32), (uint)ma, (uint)(mb >> 32), (uint)mb,
            out low
        );

        /// <returns>Significant bytes of a 128-bit result.</returns>
        /// <inheritdoc cref="Multiply(ulong, ulong, ulong, ulong, out ulong)"/>
        public static byte[] Multiply(ulong a, ulong b, ulong ma, ulong mb) => Multiply
        (
            (uint)(a >> 32), (uint)a, (uint)(b >> 32), (uint)b,
            (uint)(ma >> 32), (uint)ma, (uint)(mb >> 32), (uint)mb
        );

        /// <returns>Significant bytes of a 128-bit result.</returns>
        /// <inheritdoc cref="Multiply(uint, uint, uint, uint, uint, uint, uint, uint, out ulong)"/>
        public static byte[] Multiply(uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            ulong high = Multiply(a, b, c, d, ma, mb, mc, md, out ulong low);

            return new byte[]
            {
                (byte)(low & 0xFF),
                (byte)(low >> 8 & 0xFF),
                (byte)(low >> 16 & 0xFF),
                (byte)(low >> 24 & 0xFF),
                (byte)(low >> 32 & 0xFF),
                (byte)(low >> 40 & 0xFF),
                (byte)(low >> 48 & 0xFF),
                (byte)(low >> 56 & 0xFF),

                (byte)(high & 0xFF),
                (byte)(high >> 8 & 0xFF),
                (byte)(high >> 16 & 0xFF),
                (byte)(high >> 24 & 0xFF),
                (byte)(high >> 32 & 0xFF),
                (byte)(high >> 40 & 0xFF),
                (byte)(high >> 48 & 0xFF),
                (byte)(high >> 56 & 0xFF),
            };
        }

        private static ulong AbsMinus(ulong a, ulong b) => (a > b) ? a - b : b - a;
    }
}
