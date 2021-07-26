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
using System.Linq;
using Xunit;

namespace net.r_eg.algorithms.Tests
{
    using static _svc.Members;

    public class OverflowTest
    {
        [Fact]
        public void MulManyTest1()
        {
            ulong primeHigh = 0x1D05906000069ABC;
            ulong primeLow  = 0x40A30C07A70906D1;

            ulong inputHigh = 0x4BD4823ECC5D03EB;
            ulong inputLow = 0x19E07DB8FFD5DABE;

            byte[] prime = BitConverter.GetBytes(primeLow).Concat(BitConverter.GetBytes(primeHigh)).ToArray();

            for(int i = 0; i < 500; ++i)
            {
                ulong high = LX4Cnh.Multiply
                (
                    inputHigh, inputLow,
                    primeHigh, primeLow,
                    out ulong low
                );

                byte[] bi = MultiplyViaBigInteger
                (
                    BitConverter.GetBytes(inputLow).Concat(BitConverter.GetBytes(inputHigh)).ToArray(),
                    prime
                );

                Assert.Equal(BitConverter.ToUInt64(bi, 0), low);
                Assert.Equal(BitConverter.ToUInt64(bi, 8), high);

                inputHigh = high;
                inputLow = low;
            }
        }

        [Fact]
        public void MulOrderTest1()
        {
            ulong inputHigh = 0x4BD4823ECC5D03EB;
            ulong inputLow  = 0x19E07DB8FFD5DABE;

            ulong high = LX4Cnh.Multiply
            (
                inputHigh, inputLow,
                0, 1,
                out ulong low
            );

            Assert.Equal(inputHigh, high);
            Assert.Equal(inputLow, low);
        }

        [Fact]
        public void MulOrderTest2()
        {
            ulong inputHigh = 0x4BD4823ECC5D03EB;
            ulong inputLow  = 0x19E07DB8FFD5DABE;

            ulong high = LX4Cnh.Multiply
            (
                inputHigh, inputLow,
                1, 0,
                out ulong low
            );

            Assert.NotEqual(inputHigh, high);
            Assert.NotEqual(inputLow, low);

            Assert.Equal(0x19E07DB8FFD5DABEUL, high);
            Assert.Equal(0UL, low);
        }
    }
}
