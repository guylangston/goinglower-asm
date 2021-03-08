.method public hidebysig static int32
    ForLoop(
      int32 count
    ) cil managed
  {
    .maxstack 2
    .locals init (
      [0] int32 sum,
      [1] int32 x,
      [2] bool V_2,
      [3] int32 V_3
    )

    // [9 9 - 9 10]
    IL_0000: nop

    // [10 13 - 10 25]
    IL_0001: ldc.i4.0
    IL_0002: stloc.0      // sum

    // [11 18 - 11 27]
    IL_0003: ldc.i4.0
    IL_0004: stloc.1      // x

    IL_0005: br.s         IL_0011
    // start of loop, entry point: IL_0011

      // [14 13 - 14 14]
      IL_0007: nop

      // [15 17 - 15 31]
      IL_0008: ldloc.0      // sum
      IL_0009: ldloc.1      // x
      IL_000a: add
      IL_000b: stloc.0      // sum

      // [16 13 - 16 14]
      IL_000c: nop

      // [13 17 - 13 20]
      IL_000d: ldloc.1      // x
      IL_000e: ldc.i4.1
      IL_000f: add
      IL_0010: stloc.1      // x

      // [12 17 - 12 26]
      IL_0011: ldloc.1      // x
      IL_0012: ldarg.0      // count
      IL_0013: clt
      IL_0015: stloc.2      // V_2

      IL_0016: ldloc.2      // V_2
      IL_0017: brtrue.s     IL_0007
    // end of loop

    // [17 13 - 17 24]
    IL_0019: ldloc.0      // sum
    IL_001a: stloc.3      // V_3
    IL_001b: br.s         IL_001d

    // [18 9 - 18 10]
    IL_001d: ldloc.3      // V_3
    IL_001e: ret

  } // end of method Introduction::ForLoop