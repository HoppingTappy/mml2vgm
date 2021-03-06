﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class YM2610B : ClsChip
    {
        protected int[][] _FNumTbl = new int[2][] {
            new int[13]
            ,new int[96]
            //new int[] {
            //// OPNB(FM) : TP = (144 * ftone * (2^20) / M) / (2^(B-1))       32:Divider 2:OPNB 
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b    >c
            // 0x269,0x28e,0x2b4,0x2de,0x309,0x337,0x368,0x39c,0x3d3,0x40e,0x44b,0x48d,0x4d2
            //},
            //new int[] {
            //// OPNB(SSG) : TP = M / (ftone * 32 * 2)       32:Divider 2:OPNB
            ////   c    c+     d    d+     e     f    f+     g    g+     a    a+     b
            // 0xEEE,0xE18,0xD4D,0xC8E,0xBDA,0xB30,0xA8F,0x9F7,0x968,0x8E1,0x861,0x7E9
            //,0x777,0x70C,0x6A7,0x647,0x5ED,0x598,0x547,0x4FC,0x4B4,0x470,0x431,0x3F4
            //,0x3BC,0x386,0x353,0x324,0x2F6,0x2CC,0x2A4,0x27E,0x25A,0x238,0x218,0x1FA
            //,0x1DE,0x1C3,0x1AA,0x192,0x17B,0x166,0x152,0x13F,0x12D,0x11C,0x10C,0x0FD
            //,0x0EF,0x0E1,0x0D5,0x0C9,0x0BE,0x0B3,0x0A9,0x09F,0x096,0x08E,0x086,0x07F
            //,0x077,0x071,0x06A,0x064,0x05F,0x059,0x054,0x050,0x04B,0x047,0x043,0x03F
            //,0x03C,0x038,0x035,0x032,0x02F,0x02D,0x02A,0x028,0x026,0x024,0x022,0x020
            //,0x01E,0x01C,0x01B,0x019,0x018,0x016,0x015,0x014,0x013,0x012,0x011,0x010
            //}
        };

        public byte SSGKeyOn = 0x3f;

        public byte[] pcmDataA = null;
        public byte[] pcmDataB = null;

        public int adpcmA_TotalVolume = 63;
        public int adpcmA_beforeTotalVolume = -1;
        public int adpcmA_MAXTotalVolume = 63;
        public byte adpcmA_KeyOn = 0;
        public byte adpcmA_KeyOff = 0;

        public YM2610B(ClsVgm parent, int chipID, string initialPartName, string stPath) : base(parent, chipID, initialPartName, stPath)
        {

            _Name = "YM2610B";
            _ShortName = "OPNB";
            _ChMax = 19;
            _canUsePcm = true;
            FNumTbl = _FNumTbl;

            Frequency = 8000000;

            Dictionary<string, List<double>> dic = MakeFNumTbl();
            if (dic != null)
            {
                int c = 0;
                foreach (double v in dic["FNUM_00"])
                {
                    FNumTbl[0][c++] = (int)v;
                    if (c == FNumTbl[0].Length) break;
                }
                FNumTbl[0][FNumTbl[0].Length - 1] = FNumTbl[0][0] * 2;

                c = 0;
                foreach (double v in dic["FNUM_01"])
                {
                    FNumTbl[1][c++] = (int)v;
                    if (c == FNumTbl[1].Length) break;
                }
            }

            Ch = new ClsChannel[ChMax];
            SetPartToCh(Ch, initialPartName);
            foreach (ClsChannel ch in Ch)
            {
                ch.Type = enmChannelType.FMOPN;
                ch.isSecondary = chipID == 1;
            }

            Ch[2].Type = enmChannelType.FMOPNex;
            Ch[6].Type = enmChannelType.FMOPNex;
            Ch[7].Type = enmChannelType.FMOPNex;
            Ch[8].Type = enmChannelType.FMOPNex;

            Ch[9].Type = enmChannelType.SSG;
            Ch[10].Type = enmChannelType.SSG;
            Ch[11].Type = enmChannelType.SSG;

            Ch[12].Type = enmChannelType.ADPCMA;
            Ch[13].Type = enmChannelType.ADPCMA;
            Ch[14].Type = enmChannelType.ADPCMA;
            Ch[15].Type = enmChannelType.ADPCMA;
            Ch[16].Type = enmChannelType.ADPCMA;
            Ch[17].Type = enmChannelType.ADPCMA;

            Ch[18].Type = enmChannelType.ADPCMB;

        }

        public override void InitPart(ref partWork pw)
        {
            pw.slots = (byte)((pw.Type == enmChannelType.FMOPN || pw.ch == 2) ? 0xf : 0x0);
            pw.volume = 127;
            pw.MaxVolume = 127;
            if (pw.Type == enmChannelType.SSG)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 15;
                pw.volume = pw.MaxVolume;
            }
            else if (pw.Type == enmChannelType.ADPCMA)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 31;//5bit
                pw.volume = pw.MaxVolume;
            }
            else if (pw.Type == enmChannelType.ADPCMB)
            {
                //pw.volume = 32767;
                pw.MaxVolume = 255;
                pw.volume = pw.MaxVolume;
            }
            pw.port0 = (byte)(0x8 | (pw.isSecondary ? 0xa0 : 0x50));
            pw.port1 = (byte)(0x9 | (pw.isSecondary ? 0xa0 : 0x50));
        }

        public void MultiChannelCommand()
        {
            //コマンドを跨ぐデータ向け処理
            foreach (partWork pw in lstPartWork)
            {
                if (pw.Type == enmChannelType.ADPCMA)
                {
                    //Adpcm-A TotalVolume処理
                    if (pw.beforeVolume != pw.volume || !pw.pan.eq())
                    {
                        parent.OutData(pw.port1, (byte)(0x08 + (pw.ch - 12)), (byte)((byte)((pw.pan.val & 0x3) << 6) | (byte)(pw.volume & 0x1f)));
                        pw.beforeVolume = pw.volume;
                        pw.pan.rst();
                    }

                    adpcmA_KeyOn |= (byte)(pw.keyOn ? (1 << (pw.ch - 12)) : 0);
                    pw.keyOn = false;
                    adpcmA_KeyOff |= (byte)(pw.keyOff ? (1 << (pw.ch - 12)) : 0);
                    pw.keyOff = false;
                }
            }

            //Adpcm-A KeyOff処理
            if (0 != adpcmA_KeyOff)
            {
                byte data = (byte)(0x80 + adpcmA_KeyOff);
                parent.OutData(lstPartWork[0].port1, 0x00, data);
                adpcmA_KeyOff = 0;
            }

            //Adpcm-A TotalVolume処理
            if (adpcmA_beforeTotalVolume != adpcmA_TotalVolume)
            {
                parent.OutData(lstPartWork[0].port1, 0x01, (byte)(adpcmA_TotalVolume & 0x3f));
                adpcmA_beforeTotalVolume = adpcmA_TotalVolume;
            }

            //Adpcm-A KeyOn処理
            if (0 != adpcmA_KeyOn)
            {
                byte data = (byte)(0x00 + adpcmA_KeyOn);
                parent.OutData(lstPartWork[0].port1, 0x00, data);
                adpcmA_KeyOn = 0;
            }
        }

        public void SetADPCMAAddress(partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                parent.OutData(pw.port1, (byte)(0x10 + (pw.ch - 12)), (byte)((startAdr >> 8) & 0xff));
                parent.OutData(pw.port1, (byte)(0x18 + (pw.ch - 12)), (byte)((startAdr >> 16) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                parent.OutData(pw.port1, (byte)(0x20 + (pw.ch - 12)), (byte)(((endAdr - 0x100) >> 8) & 0xff));
                parent.OutData(pw.port1, (byte)(0x28 + (pw.ch - 12)), (byte)(((endAdr - 0x100) >> 16) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

        }

        public void SetADPCMBAddress(partWork pw, int startAdr, int endAdr)
        {
            if (pw.pcmStartAddress != startAdr)
            {
                parent.OutData(pw.port0, 0x12, (byte)((startAdr >> 8) & 0xff));
                parent.OutData(pw.port0, 0x13, (byte)((startAdr >> 16) & 0xff));
                pw.pcmStartAddress = startAdr;
            }

            if (pw.pcmEndAddress != endAdr)
            {
                parent.OutData(pw.port0, 0x14, (byte)(((endAdr - 0x100) >> 8) & 0xff));
                parent.OutData(pw.port0, 0x15, (byte)(((endAdr - 0x100) >> 16) & 0xff));
                pw.pcmEndAddress = endAdr;
            }

            //outData(pw.port1, 0x01, 0x3f);
            //outData(pw.port1, 0x08, 0xdf);
            //outData(pw.port1, 0x00, 0x01);
        }

        public void SetAdpcmBFNum(partWork pw)
        {
            int f = GetAdpcmBFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            f = f + pw.detune;
            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Vibrato)
                {
                    continue;
                }
                f += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            f = Common.CheckRange(f, 0, 0xffff);
            if (pw.freq == f) return;

            pw.freq = f;

            byte data = 0;

            data = (byte)(f & 0xff);
            parent.OutData(pw.port0, 0x19, data);

            data = (byte)((f & 0xff00) >> 8);
            parent.OutData(pw.port0, 0x1a, data);
        }

        public void SetAdpcmBVolume(partWork pw)
        {

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (0xff - pw.envVolume);
                }
            }
            vol = Common.CheckRange(vol, 0, 0xff);

            if (pw.beforeVolume != vol)
            {
                parent.OutData(pw.port0, 0x1b, (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        public void SetAdpcmBPan(partWork pw, int pan)
        {
            if (pw.pan.val != pan)
            {
                parent.OutData(pw.port0, 0x11, (byte)((pan & 0x3) << 6));
                pw.pan.val = pan;
            }
        }

        public int GetAdpcmBFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - 1;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            return (int)(0x49ba * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        public void OutAdpcmBKeyOn(partWork pw)
        {

            SetAdpcmBVolume(pw);
            parent.OutData(pw.port0, 0x10, 0x80);

        }

        public void OutAdpcmBKeyOff(partWork pw)
        {

            parent.OutData(pw.port0, 0x10, 0x01);

        }


    }
}
