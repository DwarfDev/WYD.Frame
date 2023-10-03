﻿using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkCharEquips
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
    public NetworkItem[] Slot;
}