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

using System;
using System.Collections.Generic;
using Xunit;

namespace net.r_eg.algorithms.Tests
{
    using static _svc.Members;

    public class CorrNoHighTest
    {
        public static IEnumerable<object[]> GetMulNumbers()
        {
            yield return new object[]
            {
                new byte[] { 0xEF, 0xF2, 0x6E, 0xBC, 0xCC, 0x70, 0xEF, 0x81, 0xED, 0x0F, 0xF3, 0x80, 0x19, 0x27, 0xF4, 0xC1 },
                new byte[] { 0x94, 0xFF, 0x30, 0xEA, 0xF1, 0xBE, 0x49, 0x17, 0xD2, 0xAC, 0xD0, 0x42, 0x01, 0x3F, 0xF0, 0xDE },
                0xC1F42719, 0x80F30FED, 0x81EF70CC, 0xBC6EF2EF,
                0xDEF03F01, 0x42D0ACD2, 0x1749BEF1, 0xEA30FF94
            };

            yield return new object[]
            {
                new byte[] { 0x11, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 },
                new byte[] { 0x3F, 0xFF, 0x00, 0xEA, 0xF1, 0xBE, 0x49, 0x17, 0xD2, 0xAC, 0xDA, 0x42, 0xBC, 0x28, 0x31, 0xDF },
                0x11223344, 0x55667788, 0x99AABBCC, 0xDDEEFF11,
                0xDF3128BC, 0x42DAACD2, 0x1749BEF1, 0xEA00FF3F
            };

            yield return new object[]
            {
                new byte[] { 0x11, 0xFF, 0xEE, 0xDD, 0xCC, 0xBB, 0xAA, 0x99, 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 },
                new byte[] { 0x22, 0x22, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x22, 0x22, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11 },
                0x11223344, 0x55667788, 0x99AABBCC, 0xDDEEFF11,
                0x11111111, 0x11112222, 0x11111111, 0x11112222
            };

            yield return new object[]
            {
                new byte[] { 0xEF, 0xF2, 0x6E, 0xBC, 0xCC, 0x70, 0xEF, 0x81, 0xED, 0x0F, 0xF3, 0x80, 0x19, 0x27, 0xF4, 0xC1 },
                new byte[] { 0x94, 0xFF, 0x30, 0xEA, 0xF1, 0xBE, 0x49, 0x17 },
                0xC1F42719, 0x80F30FED, 0x81EF70CC, 0xBC6EF2EF,
                0, 0, 0x1749BEF1, 0xEA30FF94
            };
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32Test1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            byte[] result = LX4Cnh.Multiply
            (
                a, b, c, d,
                ma, mb, mc, md
            );

            Assert.Equal(MultiplyViaBigInteger(input, mul), result);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia64Test1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            Assert.Equal(MultiplyViaBigInteger(input, mul), LX4Cnh.Multiply
            (
                ((ulong)a << 32 ) + b, ((ulong)c << 32) + d,
                ((ulong)ma << 32) + mb, ((ulong)mc << 32) + md
            ));
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia32UlongTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            ulong high = LX4Cnh.Multiply
            (
                a, b, c, d,
                ma, mb, mc, md,
                out ulong low
            );

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt64(data, 0), low);
            Assert.Equal(BitConverter.ToUInt64(data, 8), high);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulVia64UlongTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            ulong high = LX4Cnh.Multiply
            (
                ((ulong)a << 32) + b, ((ulong)c << 32) + d,
                ((ulong)ma << 32) + mb, ((ulong)mc << 32) + md,
                out ulong low
            );

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt64(data, 0), low);
            Assert.Equal(BitConverter.ToUInt64(data, 8), high);
        }

        [Theory]
        [MemberData(nameof(GetMulNumbers))]
        public void MulViaEmbdTest1(byte[] input, byte[] mul, uint a, uint b, uint c, uint d, uint ma, uint mb, uint mc, uint md)
        {
            ulong high, low;
            unchecked{/*LX4Cnh for C# [1.1.0] (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if(((a<b)&&(ma>mb))||((a>b)&&(ma<mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=(((A>>28)+(A>>60)+(H>>60))<<28);ulong I=B;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;if(((c<d)&&(mc>md))||((c>d)&&(mc<md))){I+=F;if(B>I)C+=0x100000000;}else{I-=F;if(B<I)C-=0x100000000;}}ulong J=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+J+C+(I>>32);G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if(((G<I)&&(A>H))||((G>I)&&(A<H))){C+=F;}else{C-=F;}}low=J;high=C;}

            byte[] data = MultiplyViaBigInteger(input, mul);

            Assert.Equal(BitConverter.ToUInt64(data, 0), low);
            Assert.Equal(BitConverter.ToUInt64(data, 8), high);
        }
    }
}
