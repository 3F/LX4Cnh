*LodgeX4CorrNoHigh* (LX4Cnh) algorithm of the high-speed multiplications of **128-bit** numbers (full range, 128 × 128).

```r
Copyright (c) 2021  Denis Kuzmin <x-3F@outlook.com> github/3F
```

[ [ ☕ ] ](https://3F.github.io/Donation/) [![License](https://img.shields.io/badge/License-MIT-74A5C2.svg)](https://github.com/3F/LX4Cnh/blob/master/License.txt)

✔ Free and Open. MIT License. *Fork! Star! Contribute! Share! Enjoy!*

### MLnoCS vs LX4Cnh

Algorithm | Maximum bits | One multiplication
----------|--------------|-------------------
LX4Cnh    | 128 × 128    | less than ~ **4.3 ns** == 0.0000000043 sec
[MLnoCS](https://github.com/3F/sandbox/tree/master/algorithms/MLnoCS)    | 128 × 16 (*<sup>1</sup>32) | less than ~ **0.31 ns** == 0.00000000031 sec
LX4Cnh optimized\*<sup>2</sup> | 128 × 128 | \*<sup>2</sup> less than ~ **0.86 ns** == 0.00000000086 sec

* \*<sup>1</sup> - theoretically up to 128 x 32 with some correction.
* \*<sup>2</sup> - The actual calculation using LX4Cnh can be a bit optimized such for FNV1a-128 implementation (find it in my repo):

[![](/img/fnvOptimization.png)](#)

(**1 ns** == 0.000000001 sec)

## Where is this used?

* [Huid](https://github.com/3F/Huid) - A high-speed *FNV-1a-128* hash-based *UUID* implementation.
    * https://twitter.com/github3F/status/1419045735807467520

## .NET implementation

[![Build status](https://ci.appveyor.com/api/projects/status/q4rq4wd92bi735ga/branch/master?svg=true)](https://ci.appveyor.com/project/3Fs/lx4cnh/branch/master)
[![NuGet package](https://img.shields.io/nuget/v/LX4Cnh.svg)](https://www.nuget.org/packages/LX4Cnh/) 
[![Tests](https://img.shields.io/appveyor/tests/3Fs/lx4cnh/master.svg)](https://ci.appveyor.com/project/3Fs/lx4cnh/build/tests)

[![Build history](https://buildstats.info/appveyor/chart/3Fs/lx4cnh?buildCount=15&includeBuildsFromPullRequest=true&showStats=true)](https://ci.appveyor.com/project/3Fs/lx4cnh/history)

*LX4Cnh* class provides several ways of setting and getting numbers by using uint, ulong, or bytes array. Just play with available [Unit-Tests](src/tests/csharp/UnitTest/) and [Speed-Tests](src/tests/csharp/Benchmark).

[![](/img/benchmark.png)](https://twitter.com/github3F/status/1410358979033813000)

### Examples

For example, using ulong (UInt64)

```csharp
//   0x4BD4823ECC5D03EB19E07DB8FFD5DABE
// × 0x1D05906000069ABC40A30C07A70906D1

ulong high = LX4Cnh.Multiply
(
    0x4BD4823ECC5D03EB, 0x19E07DB8FFD5DABE,
    0x1D05906000069ABC, 0x40A30C07A70906D1,
    out ulong low
);
//          high            low
//     ________________|_______________
// = 0xACBBE8EAB60C77E249B25D708366091E
```

### Embeddable superfast version

To reduce the amount of unnecessary stack manipulations (ldloca.s/ldarg.. etc), meet an *embeddable* version.

```csharp
//   0xC1F4271980F30FED81EF70CCBC6EF2EF
// × 0xDEF03F0142D0ACD21749BEF1EA30FF94

uint a = 0xC1F42719, b = 0x80F30FED, c = 0x81EF70CC, d = 0xBC6EF2EF;
uint ma = 0xDEF03F01, mb = 0x42D0ACD2, mc = 0x1749BEF1, md = 0xEA30FF94;
//-
ulong high, low;
unchecked{/*LX4Cnh for C# [1.1.0] (c) Denis Kuzmin <x-3F@outlook.com> github/3F */ulong A=(ulong)b*mb;ulong B=A&0xFFFF_FFFF;ulong C=((A>>32)+B+(a*ma))&0xFFFF_FFFF;ulong D=(a>b)?a-b:b-a;ulong E=(ma>mb)?ma-mb:mb-ma;if(D!=0&&E!=0){ulong F=D*E;if(((a<b)&&(ma>mb))||((a>b)&&(ma<mb))){C+=F&0xFFFF_FFFF;}else{C-=F&0xFFFF_FFFF;}}ulong G=(C<<32)+B;A=(ulong)c*mc;ulong H=(ulong)d*md;B=(H>>32)+(H&0xFFF_FFFF_FFFF_FFFF)+(A&0xFFF_FFFF_FFFF_FFFF)+((A&0xFFF_FFFF)<<32);C=(((A>>28)+(A>>60)+(H>>60))<<28);ulong I=B;D=(c>d)?c-d:d-c;E=(mc>md)?mc-md:md-mc;if(D!=0&&E!=0){ulong F=D*E;if(((c<d)&&(mc>md))||((c>d)&&(mc<md))){I+=F;if(B>I)C+=0x100000000;}else{I-=F;if(B<I)C-=0x100000000;}}ulong J=((I&0xFFFF_FFFF)<<32)+(H&0xFFFF_FFFF);C=G+J+C+(I>>32);G=((ulong)a<<32)+b;I=((ulong)c<<32)+d;A=((ulong)ma<<32)+mb;H=((ulong)mc<<32)+md;D=(G>I)?G-I:I-G;E=(A>H)?A-H:H-A;if(D!=0&&E!=0){ulong F=D*E;if(((G<I)&&(A>H))||((G>I)&&(A<H))){C+=F;}else{C-=F;}}low=J;high=C;}

//          high            low
//     ________________|_______________
// = 0x9633C106748CB7D96650F9EA76F0832C
```
