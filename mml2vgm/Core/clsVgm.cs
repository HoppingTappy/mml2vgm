﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ClsVgm
    {
        public float Version = 1.61f;

        public YM2151[] ym2151 = null;
        public YM2203[] ym2203 = null;
        public YM2608[] ym2608 = null;
        public YM2610B[] ym2610b = null;
        public YM2612[] ym2612 = null;
        public SN76489[] sn76489 = null;
        public RF5C164[] rf5c164 = null;
        public segaPcm[] segapcm = null;
        public HuC6280[] huc6280 = null;
        public YM2612X[] ym2612x = null;
        public YM2413[] ym2413 = null;

        public List<ClsChip> chips = new List<ClsChip>();

        public int lineNumber = 0;

        public string TitleName = "";
        public string TitleNameJ = "";
        public string GameName = "";
        public string GameNameJ = "";
        public string SystemName = "";
        public string SystemNameJ = "";
        public string Composer = "";
        public string ComposerJ = "";
        public string ReleaseDate = "";
        public string Converted = "";
        public string Notes = "";

        public Dictionary<int, byte[]> instFM = new Dictionary<int, byte[]>();
        public Dictionary<int, int[]> instENV = new Dictionary<int, int[]>();
        public Dictionary<int, clsPcm> instPCM = new Dictionary<int, clsPcm>();
        public Dictionary<int, clsToneDoubler> instToneDoubler = new Dictionary<int, clsToneDoubler>();
        public Dictionary<int, byte[]> instWF = new Dictionary<int, byte[]>();

        public Dictionary<string, List<Line>> partData = new Dictionary<string, List<Line>>();
        public Dictionary<string, Line> aliesData = new Dictionary<string, Line>();
        public List<string> monoPart = null;
        public enmFormat format = enmFormat.VGM;

        private int instrumentCounter = -1;
        private byte[] instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
        private int toneDoublerCounter = -1;
        private List<int> toneDoublerBufCache = new List<int>();
        private int wfInstrumentCounter = -1;
        private byte[] wfInstrumentBufCache = null;

        private int newStreamID = -1;
        private int pcmDataSeqNum=0;

        public ClsVgm(string stPath)
        {
            chips = new List<ClsChip>();

            ym2151 = new YM2151[] { new YM2151(this, 0, "X", stPath), new YM2151(this, 1, "Xs", stPath) };
            ym2203 = new YM2203[] { new YM2203(this, 0, "N", stPath), new YM2203(this, 1, "Ns", stPath) };
            ym2608 = new YM2608[] { new YM2608(this, 0, "P", stPath), new YM2608(this, 1, "Ps", stPath) };
            ym2610b = new YM2610B[] { new YM2610B(this, 0, "T", stPath), new YM2610B(this, 1, "Ts", stPath) };
            ym2612 = new YM2612[] { new YM2612(this, 0, "F", stPath), new YM2612(this, 1, "Fs", stPath) };
            sn76489 = new SN76489[] { new SN76489(this, 0, "S", stPath), new SN76489(this, 1, "Ss", stPath) };
            rf5c164 = new RF5C164[] { new RF5C164(this, 0, "R", stPath), new RF5C164(this, 1, "Rs", stPath) };
            segapcm = new segaPcm[] { new segaPcm(this, 0, "Z", stPath), new segaPcm(this, 1, "Zs", stPath) };
            huc6280 = new HuC6280[] { new HuC6280(this, 0, "H", stPath), new HuC6280(this, 1, "Hs", stPath) };
            ym2612x = new YM2612X[] { new YM2612X(this, 0, "E", stPath) };
            ym2413 = new YM2413[] { new YM2413(this, 0, "L", stPath), new YM2413(this, 1, "Ls", stPath) };

            chips.Add(ym2612[0]);
            chips.Add(ym2612[1]);
            chips.Add(sn76489[0]);
            chips.Add(sn76489[1]);
            chips.Add(rf5c164[0]);
            chips.Add(rf5c164[1]);
            chips.Add(ym2610b[0]);
            chips.Add(ym2610b[1]);
            chips.Add(ym2608[0]);
            chips.Add(ym2608[1]);
            chips.Add(ym2203[0]);
            chips.Add(ym2203[1]);
            chips.Add(ym2151[0]);
            chips.Add(ym2151[1]);
            chips.Add(segapcm[0]);
            chips.Add(segapcm[1]);
            chips.Add(huc6280[0]);
            chips.Add(huc6280[1]);
            chips.Add(ym2612x[0]);
            chips.Add(ym2413[0]);
            chips.Add(ym2413[1]);

            List<clsTD> lstTD = new List<clsTD>
            {
                new clsTD(4, 4, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(3, 3, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(4, 4, 5, 5, 1, 1, 0, 0, -4),
                new clsTD(3, 3, 4, 4, 2, 2, 0, 0, 0),
                new clsTD(5, 5, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(4, 4, 3, 3, 0, 0, 0, 0, 5),
                new clsTD(4, 4, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(6, 6, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(4, 4, 4, 4, 2, 2, 0, 0, 0),
                new clsTD(6, 6, 5, 5, 1, 1, 0, 0, -4),
                new clsTD(5, 5, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(6, 6, 5, 5, 2, 2, 0, 0, -4),
                new clsTD(8, 8, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(6, 6, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(8, 8, 5, 5, 1, 1, 0, 0, -4),
                new clsTD(6, 6, 4, 4, 2, 2, 0, 0, 0),
                new clsTD(10, 10, 4, 4, 0, 0, 0, 0, 0),
                new clsTD(8, 8, 3, 3, 0, 0, 0, 0, 5),
                new clsTD(8, 8, 4, 4, 1, 1, 0, 0, 0),
                new clsTD(12, 12, 4, 4, 0, 0, 0, 0, 0)
            };
            clsToneDoubler toneDoubler = new clsToneDoubler(0, lstTD);
            instToneDoubler.Add(0, toneDoubler);

        }

        public int Analyze(List<Line> src)
        {
            log.Write("テキスト解析開始");
            lineNumber = 0;

            bool multiLine = false;
            string s2 = "";

            foreach (Line line in src)
            {

                string s = line.Txt;
                int lineNumber = line.Num;
                string fn = line.Fn;

                if (multiLine)
                {
                    if (s.Trim() == "")
                    {
                        continue;
                    }
                    s2 += s.Trim() + "\r\n";
                    if (s.IndexOf("}") < 0)
                    {
                        continue;
                    }
                    multiLine = false;
                    s2 = s2.Substring(0, s2.IndexOf("}"));
                    // Information
                    AddInformation(s2, lineNumber, fn);
                    continue;
                }

                // 行頭が'以外は読み飛ばす
                if (s.TrimStart().IndexOf("'") != 0)
                {
                    continue;
                }

                s2 = s.TrimStart().Substring(1).TrimStart();
                // 'のみの行も読み飛ばす
                if (s2.Trim() == "")
                {
                    continue;
                }

                if (s2.IndexOf("{") == 0)
                {
                    multiLine = true;
                    s2 = s2.Substring(1);

                    if (s2.IndexOf("}") > -1)
                    {
                        multiLine = false;
                        s2 = s2.Substring(0, s2.IndexOf("}")).Trim();
                        // Information
                        AddInformation(s2, lineNumber, fn);
                    }
                    continue;
                }
                else if (s2.IndexOf("@") == 0)
                {
                    // Instrument
                    AddInstrument(s2, fn, lineNumber);
                    continue;
                }
                else if (s2.IndexOf("%") == 0)
                {
                    // Alies
                    AddAlies(s2, fn, lineNumber);
                    continue;
                }
                else
                {
                    // Part
                    AddPart(s2, fn, lineNumber);
                    continue;
                }

            }

            // 定義中のToneDoublerがあればここで定義完了
            if (toneDoublerCounter != -1)
            {
                toneDoublerCounter = -1;
                SetInstToneDoubler();
            }

            // チェック1定義されていない名称を使用したパートが存在するか

            foreach (string p in partData.Keys)
            {
                bool flg = false;
                foreach (ClsChip chip in chips)
                {
                    if (chip.ChannelNameContains(p))
                    {
                        flg = true;
                        break;
                    }
                }
                if (!flg)
                {
                    msgBox.setWrnMsg(string.Format("未定義のパート({0})のデータは無視されます。", p.Substring(0, 2).Trim() + int.Parse(p.Substring(2, 2)).ToString()));
                    flg = false;
                }
            }

            log.Write("テキスト解析完了");
            return 0;

        }

        private int AddInformation(string buf, int lineNumber, string fn)
        {
            string[] settings = buf.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in settings)
            {
                try
                {
                    int p = s.IndexOf("=");
                    if (p < 0) continue;

                    string wrd = s.Substring(0, p).Trim().ToUpper();
                    string val = s.Substring(p + 1).Trim();

                    if (wrd == Const.TITLENAMEJ) TitleNameJ = val;
                    if (wrd == Const.TITLENAME) TitleName = val;
                    if (wrd == Const.GAMENAME) GameName = val;
                    if (wrd == Const.GAMENAMEJ) GameNameJ = val;
                    if (wrd == Const.SYSTEMNAME) SystemName = val;
                    if (wrd == Const.SYSTEMNAMEJ) SystemNameJ = val;
                    if (wrd == Const.COMPOSER) Composer = val;
                    if (wrd == Const.COMPOSERJ) ComposerJ = val;
                    if (wrd == Const.RELEASEDATE) ReleaseDate = val;
                    if (wrd == Const.CONVERTED) Converted = val;
                    if (wrd == Const.NOTES) Notes = val;
                    if (wrd == Const.VERSION)
                    {
                        float.TryParse(val, out float v);
                        if (v != 1.51f && v != 1.60f) v = 1.60f;
                        Version = v;
                    }

                    foreach (ClsChip chip in chips)
                    {
                        if (wrd == Const.PARTNAME + chip.Name + Const.IDName[chip.ChipID]) chip.SetPartToCh(chip.Ch, val);
                        if (wrd == Const.PARTNAME + chip.ShortName + Const.IDName[chip.ChipID]) chip.SetPartToCh(chip.Ch, val);
                        if (chip.ChipID == 0)
                        {
                            if (wrd == Const.PARTNAME + chip.Name) chip.SetPartToCh(chip.Ch, val);
                            if (wrd == Const.PARTNAME + chip.ShortName) chip.SetPartToCh(chip.Ch, val);
                        }
                    }

                    if (wrd == Const.CLOCKCOUNT) userClockCount = int.Parse(val);
                    if (wrd == Const.FMF_NUM)
                    {
                        ym2612[0].SetF_NumTbl(val);
                        ym2612[1].SetF_NumTbl(val);
                        ym2612x[0].SetF_NumTbl(val);
                        ym2612x[1].SetF_NumTbl(val);
                    }
                    if (wrd == Const.FORCEDMONOPARTYM2612) SetMonoPart(val);
                    if (wrd == Const.FORMAT) SetFormat(val);
                    if (wrd == Const.XGMBASEFRAME) SetXgmBaseFrame(val);
                    if (wrd == Const.OCTAVEREV) SetOctaveRev(val);

                    for (int i = 0; i < 8; i++)
                    {
                        if (wrd == string.Format("{0}{1}", Const.PSGF_NUM, i + 1)) SetDcsgF_NumTbl(val, i);
                    }
                }
                catch
                {
                    msgBox.setWrnMsg(string.Format("不正な定義です。({0})", s), fn, lineNumber);
                }
            }
            return 0;
        }

        private void SetDcsgF_NumTbl(string val, int oct)
        {
            //厳密なチェックを行っていないので設定値によってはバグる危険有り

            string[] s = val.Split(new string[] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < s.Length; i++)
            {
                if (i + oct * 12 >= sn76489[0].FNumTbl[0].Length)
                {
                    break;
                }
                sn76489[0].FNumTbl[0][i + oct * 12] = int.Parse(s[i], System.Globalization.NumberStyles.HexNumber);
            }
        }

        private void SetMonoPart(string val)
        {
            monoPart = DivParts(val);
        }

        private void SetFormat(string val)
        {
            switch (val.ToUpper()) {
                case "VGM":
                default:
                    format = enmFormat.VGM;
                    tempo = defaultTempo;
                    clockCount = defaultClockCount;
                    samplesPerClock = defaultSamplesPerClock;
                    break;
                case "XGM":
                    format = enmFormat.XGM;
                    tempo = defaultTempo;
                    clockCount = xgmDefaultClockCount;
                    samplesPerClock = xgmDefaultSamplesPerClock;
                    break;
                case "ZGM":
                    format = enmFormat.ZGM;
                    tempo = defaultTempo;
                    clockCount = defaultClockCount;
                    samplesPerClock = defaultSamplesPerClock;
                    break;
            }
        }

        private void SetXgmBaseFrame(string val)
        {
            switch (val.ToUpper())
            {
                case "NTSC":
                default:
                    xgmSamplesPerSecond = 60;
                    break;
                case "PAL":
                    xgmSamplesPerSecond = 50;
                    break;
            }
        }

        private void SetOctaveRev(string val)
        {
            switch (val.ToUpper())
            {
                case "TRUE":
                case "1":
                case "YES":
                case "Y":
                    octaveRev = true;
                    break;
                case "FALSE":
                case "0":
                case "NO":
                case "N":
                default:
                    octaveRev = false;
                    break;
            }
        }

        private int AddInstrument(string buf, string srcFn, int lineNumber)
        {
            if (buf == null || buf.Length < 2)
            {
                msgBox.setWrnMsg("空の音色定義文を受け取りました。", srcFn, lineNumber);
                return -1;
            }

            string s = buf.Substring(1).TrimStart();

            // FMの音色を定義中の場合
            if (instrumentCounter != -1)
            {

                return SetInstrument(s, srcFn, lineNumber);

            }

            // WaveFormの音色を定義中の場合
            if (wfInstrumentCounter != -1)
            {

                return SetWfInstrument(s, srcFn, lineNumber);

            }

            char t = s.ToUpper()[0];
            if (toneDoublerCounter != -1)
            {
                if (t == 'F' || t == 'N' || t == 'M' || t == 'L' || t == 'P' || t == 'E' || t == 'T' || t == 'H')
                {
                    toneDoublerCounter = -1;
                    SetInstToneDoubler();
                }
            }

            switch (t)
            {
                case 'F':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE - 8];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'N':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'M':
                    instrumentBufCache = new byte[Const.INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'L':
                    instrumentBufCache = new byte[Const.OPL_INSTRUMENT_SIZE];
                    instrumentCounter = 0;
                    SetInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

                case 'P':
                    try
                    {
                        instrumentCounter = -1;
                        enmChipType enmChip = enmChipType.YM2612;
                        string[] vs = s.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int num = int.Parse(vs[0]);
                        string fn = vs[1].Trim().Trim('"');
                        int fq = int.Parse(vs[2]);
                        int vol = int.Parse(vs[3]);
                        int lp = -1;
                        bool isSecondary = false;
                        if (vs.Length > 4)
                        {
                            string chipName = vs[4].Trim().ToUpper();
                            isSecondary = false;
                            if (chipName.IndexOf(Const.PRIMARY) >= 0)
                            {
                                isSecondary = false;
                                chipName = chipName.Replace(Const.PRIMARY, "");
                            }
                            else if (chipName.IndexOf(Const.SECONDARY) >= 0)
                            {
                                isSecondary = true;
                                chipName = chipName.Replace(Const.SECONDARY, "");
                            }

                            if (!GetChip(chipName).CanUsePcm)
                            {
                                msgBox.setWrnMsg("未定義のChipName又はPCMを使用できないChipが指定されています。", srcFn, lineNumber);
                                break;
                            }

                            enmChip = GetChipNumber(chipName);
                        }
                        if (vs.Length > 5)
                        {
                            lp = int.Parse(vs[5]);
                        }
                        if (instPCM.ContainsKey(num))
                        {
                            instPCM.Remove(num);
                        }
                        instPCM.Add(num, new clsPcm(num, pcmDataSeqNum++, enmChip, isSecondary, fn, fq, vol, 0, 0, 0, lp));
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なPCM音色定義文です。", srcFn, lineNumber);
                    }
                    return 0;

                case 'E':
                    try
                    {
                        instrumentCounter = -1;
                        string[] vs = s.Substring(1).Trim().Split(new string[] { "," }, StringSplitOptions.None);
                        int[] env = null;
                        env = new int[9];
                        int num = int.Parse(vs[0]);
                        for (int i = 0; i < env.Length; i++)
                        {
                            if (i == 8)
                            {
                                if (vs.Length == 8) env[i] = (int)enmChipType.SN76489;
                                else env[i] = (int)GetChipNumber(vs[8]);
                                continue;
                            }
                            env[i] = int.Parse(vs[i]);
                        }

                        for (int i = 0; i < env.Length - 1; i++)
                        {
                            if (env[8] == (int)enmChipType.SN76489)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 15, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.RF5C164)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 255, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.YM2203)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 255, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.YM2608)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 255, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.YM2610B)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 255, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.SEGAPCM)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 127, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else if (env[8] == (int)enmChipType.HuC6280)
                            {
                                CheckEnvelopeVolumeRange(srcFn, lineNumber, env, i, 31, 0);
                                if (env[7] == 0) env[7] = 1;
                            }
                            else
                            {
                                msgBox.setWrnMsg("エンベロープを使用できない音源が選択されています。", srcFn, lineNumber);
                            }
                        }

                        if (instENV.ContainsKey(num))
                        {
                            instENV.Remove(num);
                        }
                        instENV.Add(num, env);
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なエンベロープ定義文です。", srcFn, lineNumber);
                    }
                    return 0;

                case 'T':
                    try
                    {
                        instrumentCounter = -1;

                        if (s.ToUpper()[1] != 'D') return 0;

                        toneDoublerBufCache.Clear();
                        StoreToneDoublerBuffer(s.ToUpper().Substring(2).TrimStart(), srcFn, lineNumber);
                    }
                    catch
                    {
                        msgBox.setWrnMsg("不正なTone Doubler定義文です。", srcFn, lineNumber);
                    }
                    return 0;

                case 'H':
                    wfInstrumentBufCache = new byte[Const.WF_INSTRUMENT_SIZE];
                    wfInstrumentCounter = 0;
                    SetWfInstrument(s.Substring(1).TrimStart(), srcFn, lineNumber);
                    return 0;

            }

            // ToneDoublerを定義中の場合
            if (toneDoublerCounter != -1)
            {
                return StoreToneDoublerBuffer(s.ToUpper(), srcFn, lineNumber);
            }

            return 0;
        }

        private static void CheckEnvelopeVolumeRange(string srcFn, int lineNumber, int[] env, int i, int max, int min)
        {
            if (i == 1 || i == 4 || i == 7)
            {
                if (env[i] > max)
                {
                    env[i] = max;
                    msgBox.setWrnMsg(string.Format("Envelope音量が{0}を超えています。", max), srcFn, lineNumber);
                }
                if (env[i] < min)
                {
                    env[i] = min;
                    msgBox.setWrnMsg(string.Format("Envelope音量が{0}未満です。", min), srcFn, lineNumber);
                }
            }
        }

        private static enmChipType GetChipNumber(string chipN)
        {
            enmChipType chip;
            switch (chipN.ToUpper().Trim())
            {
                case "YM2151":
                case "OPM":
                    chip = enmChipType.YM2151;
                    break;
                case "YM2612":
                case "OPN2":
                    chip = enmChipType.YM2612;
                    break;
                case "SN76489":
                case "DCSG":
                    chip = enmChipType.SN76489;
                    break;
                case "RF5C164":
                case "RF5C":
                    chip = enmChipType.RF5C164;
                    break;
                case "YM2203":
                case "OPN":
                    chip = enmChipType.YM2203;
                    break;
                case "YM2608":
                case "OPNA":
                    chip = enmChipType.YM2608;
                    break;
                case "YM2610":
                case "YM2610B":
                case "OPNB":
                    chip = enmChipType.YM2610B;
                    break;
                case "SEGAPCM":
                case "SPCM":
                    chip = enmChipType.SEGAPCM;
                    break;
                case "HUC6280":
                case "HUC8":
                    chip = enmChipType.HuC6280;
                    break;
                case "YM2612X":
                case "OPN2X":
                    chip = enmChipType.YM2612X;
                    break;
                case "YM2413":
                case "OPLL":
                    chip = enmChipType.YM2413;
                    break;
                default:
                    chip = enmChipType.None;
                    break;
            }

            return chip;
        }

        private ClsChip GetChip(string chipN)
        {
            string n = chipN.ToUpper().Trim();

            foreach (ClsChip c in chips)
            {
                if (n == c.Name.ToUpper()) return c;
                if (n == c.ShortName.ToUpper()) return c;
            }

            return null;
        }

        private int AddAlies(string buf, string srcFn, int lineNumber)
        {
            string name = "";
            string data = "";

            int i = buf.Substring(1).Trim().IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            name = buf.Substring(1).Trim().Substring(0, i).Trim();
            data = buf.Substring(1).Trim().Substring(i).Trim();
            if (name == "")
            {
                //エイリアス指定がない場合は警告とする
                msgBox.setWrnMsg("不正なエイリアス指定です。", srcFn, lineNumber);
                return -1;
            }
            if (data == "")
            {
                //データがない場合は警告する
                msgBox.setWrnMsg("エイリアスにデータがありません。", srcFn, lineNumber);
            }

            if (aliesData.ContainsKey(name))
            {
                aliesData.Remove(name);
            }
            aliesData.Add(name, new Line("", lineNumber, data));

            return 0;
        }

        private int AddPart(string buf, string srcFn, int lineNumber)
        {
            List<string> part = new List<string>();
            string data = "";

            int i = buf.IndexOfAny(new char[] { ' ', '\t' });
            if (i < 0)
            {
                //空白による区切りが見つからない場合は無視する
                return 0;
            }

            part = DivParts(buf.Substring(0, i).Trim());
            data = buf.Substring(i).Trim();
            if (part == null)
            {
                //パート指定がない場合は警告とする
                msgBox.setWrnMsg("不正なパート指定です。", srcFn, lineNumber);
                return -1;
            }
            if (data == "")
            {
                //データがない場合は無視する
                return 0;
            }

            foreach (string p in part)
            {
                if (!partData.ContainsKey(p))
                {
                    partData.Add(p, new List<Line>());
                }
                partData[p].Add(new Line(srcFn, lineNumber, data));
            }

            return 0;
        }

        private List<string> DivParts(string parts)
        {
            List<string> ret = new List<string>();
            string a = "";
            int k = 1;
            int m = 0;
            string n0 = "";

            for (int i = 0; i < parts.Length; i++)
            {
                if (m == 0 && parts[i] >= 'A' && parts[i] <= 'Z')
                {
                    a = parts[i].ToString();
                    if (i + 1 < parts.Length && parts[i + 1] >= 'a' && parts[i + 1] <= 'z')
                    {
                        a += parts[i + 1].ToString();
                        i++;
                    }
                    else
                    {
                        a += " ";
                    }

                    k = GetChMax(a) > 9 ? 2 : 1;
                    n0 = "";

                }
                else if (m == 0 && parts[i] == ',')
                {
                    n0 = "";
                }
                else if (m == 0 && parts[i] == '-')
                {
                    m = 1;
                }
                else if (parts[i] >= '0' && parts[i] <= '9')
                {
                    string n = parts.Substring(i, k);
                    if (k == 2 && i + 1 < parts.Length)
                    {
                        i++;
                    }

                    if (m == 0)
                    {
                        n0 = n;

                        if (!int.TryParse(n0, out int s)) return null;
                        string p = string.Format("{0}{1:00}", a, s);
                        ret.Add(p);
                    }
                    else
                    {
                        string n1 = n;

                        if (!int.TryParse(n0, out int s)) return null;
                        if (!int.TryParse(n1, out int e)) return null;
                        if (s >= e) return null;

                        do
                        {
                            s++;
                            string p = string.Format("{0}{1:00}", a, s);
                            if (ret.Contains(p)) return null;
                            ret.Add(p);
                        } while (s < e);

                        i++;
                        m = 0;
                        n0 = "";
                    }
                }
                else
                {
                    return null;
                }
            }

            return ret;
        }

        private int GetChMax(string a)
        {
            foreach (ClsChip chip in chips)
            {
                if (chip.Ch[0].Name.Substring(0, 2) == a)
                {
                    return chip.ChMax;
                }
            }

            return 0;
        }

        private int SetInstrument(string vals, string srcFn, int lineNumber)
        {

            try
            {
                instrumentCounter= GetNums(instrumentBufCache, instrumentCounter, vals);

                if (instrumentCounter == instrumentBufCache.Length)
                {
                    //すでに定義済みの場合はいったん削除する(後に定義されたものが優先)
                    if (instFM.ContainsKey(instrumentBufCache[0]))
                    {
                        instFM.Remove(instrumentBufCache[0]);
                    }


                    if(instrumentBufCache.Length == Const.INSTRUMENT_SIZE)
                    {
                        //M
                        instFM.Add(instrumentBufCache[0], instrumentBufCache);
                    }
                    else if(instrumentBufCache.Length == Const.OPL_INSTRUMENT_SIZE)
                    {
                        //OPL
                        instFM.Add(instrumentBufCache[0], instrumentBufCache);
                    }
                    else
                    {
                        //F
                        instFM.Add(instrumentBufCache[0], ConvertFtoM(instrumentBufCache));
                    }

                    instrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg("音色の定義が不正です。", srcFn, lineNumber);
            }

            return 0;
        }

        private int SetWfInstrument(string vals, string srcFn, int lineNumber)
        {

            try
            {
                wfInstrumentCounter= GetNums(wfInstrumentBufCache, wfInstrumentCounter, vals);

                if (wfInstrumentCounter == wfInstrumentBufCache.Length)
                {
                    if (instWF.ContainsKey(wfInstrumentBufCache[0]))
                    {
                        instWF.Remove(wfInstrumentBufCache[0]);
                    }
                    instWF.Add(wfInstrumentBufCache[0], wfInstrumentBufCache);

                    wfInstrumentCounter = -1;
                }
            }
            catch
            {
                msgBox.setErrMsg("WaveForm音色の定義が不正です。", srcFn, lineNumber);
            }

            return 0;
        }

        private int GetNums(byte[] aryBuf,int aryIndex, string vals)
        {
            string n = "";
            string h = "";
            int hc = -1;
            int i = 0;

            foreach (char c in vals)
            {
                if (c == '$')
                {
                    hc = 0;
                    continue;
                }

                if (hc > -1 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')))
                {
                    h += c;
                    hc++;
                    if (hc == 2)
                    {
                        i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                        aryBuf[aryIndex] = (byte)(i & 0xff);
                        aryIndex++;
                        h = "";
                        hc = -1;
                    }
                    continue;
                }

                if ((c >= '0' && c <= '9') || c == '-')
                {
                    n = n + c.ToString();
                    continue;
                }

                if (int.TryParse(n, out i))
                {
                    aryBuf[aryIndex] = (byte)(i & 0xff);
                    aryIndex++;
                    n = "";
                }
            }

            if (!string.IsNullOrEmpty(n))
            {
                if (int.TryParse(n, out i))
                {
                    aryBuf[aryIndex] = (byte)(i & 0xff);
                    aryIndex++;
                    n = "";
                }
            }

            return aryIndex;
        }

        private int StoreToneDoublerBuffer(string vals, string srcFn, int lineNumber)
        {
            string n = "";
            string h = "";
            int hc = -1;
            int i;

            try
            {
                foreach (char c in vals)
                {
                    if (c == '$')
                    {
                        hc = 0;
                        continue;
                    }

                    if (hc > -1 && ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')))
                    {
                        h += c;
                        hc++;
                        if (hc == 2)
                        {
                            i = int.Parse(h, System.Globalization.NumberStyles.HexNumber);
                            toneDoublerBufCache.Add(i);
                            toneDoublerCounter++;
                            h = "";
                            hc = -1;
                        }
                        continue;
                    }

                    if ((c >= '0' && c <= '9') || c == '-')
                    {
                        n = n + c.ToString();
                        continue;
                    }

                    if (int.TryParse(n, out i))
                    {
                        toneDoublerBufCache.Add(i);
                        toneDoublerCounter++;
                        n = "";
                    }
                }

                if (!string.IsNullOrEmpty(n))
                {
                    if (int.TryParse(n, out i))
                    {
                        toneDoublerBufCache.Add(i);
                        toneDoublerCounter++;
                        n = "";
                    }
                }

            }
            catch
            {
                msgBox.setErrMsg("Tone Doublerの定義が不正です。", srcFn, lineNumber);
            }

            return 0;
        }

        private void SetInstToneDoubler()
        {
            if (toneDoublerBufCache.Count < 10)
            {
                toneDoublerBufCache.Clear();
                toneDoublerCounter = -1;
                return;
            }

            int num = toneDoublerBufCache[0];
            int counter = 1;
            List<clsTD> lstTD = new List<clsTD>();
            while (counter < toneDoublerBufCache.Count)
            {
                clsTD td = new clsTD(
                    toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    , toneDoublerBufCache[counter++]
                    );
                lstTD.Add(td);
            }

            clsToneDoubler toneDoubler = new clsToneDoubler(num, lstTD);
            if (instToneDoubler.ContainsKey(num))
            {
                instToneDoubler.Remove(num);
            }
            instToneDoubler.Add(num, toneDoubler);
            toneDoublerBufCache.Clear();
            toneDoublerCounter = -1;
        }

        private byte[] ConvertFtoM(byte[] instrumentBufCache)
        {
            byte[] ret = new byte[Const.INSTRUMENT_SIZE];

            ret[0] = instrumentBufCache[0];

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < Const.INSTRUMENT_OPERATOR_SIZE; i++)
                {
                    ret[j * Const.INSTRUMENT_M_OPERATOR_SIZE + i + 1] = instrumentBufCache[j * Const.INSTRUMENT_OPERATOR_SIZE + i + 1];
                }
            }

            ret[Const.INSTRUMENT_SIZE - 2] = instrumentBufCache[Const.INSTRUMENT_SIZE - 10];
            ret[Const.INSTRUMENT_SIZE - 1] = instrumentBufCache[Const.INSTRUMENT_SIZE - 9];

            return ret;
        }



        public List<byte> dat = null;
        //xgm music data
        public List<byte> xdat = null;
        //xgm keyOnDataList
        private List<byte> xgmKeyOnData = null;

        public const long vgmSamplesPerSecond = 44100L;
        private const long defaultTempo = 120L;
        private const long defaultClockCount = 192L;
        private const long defaultSamplesPerClock = vgmSamplesPerSecond * 60 * 4 / (defaultTempo * defaultClockCount);

        public long xgmSamplesPerSecond = 60L;
        private const long xgmDefaultClockCount = 120L;
        private const long xgmDefaultSamplesPerClock = 60 * 60 * 4 / (defaultTempo * xgmDefaultClockCount);

        public bool octaveRev = false;
        private long tempo = defaultTempo;
        private long clockCount = defaultClockCount;
        private double samplesPerClock = defaultSamplesPerClock;
        private long userClockCount = 0;
        public double dSample = 0.0;
        public long lClock = 0L;
        private double sampleB = 0.0;

        private long loopOffset = -1L;
        private long loopSamples = -1L;

        private Random rnd = new Random();


        public byte[] GetByteData()
        {
            if (userClockCount != 0) clockCount = userClockCount;

            log.Write("パート情報の初期化");
            PartInit();

            dat = new List<byte>();

            log.Write("ヘッダー情報作成");
            MakeHeader();

            int endChannel = 0;
            newStreamID = -1;
            int totalChannel = 0;
            foreach (ClsChip chip in chips) totalChannel += chip.ChMax;

            log.Write("MML解析開始");
            do
            {
                foreach (ClsChip chip in chips)
                {

                    log.Write(string.Format("Chip [{0}]", chip.Name));

                    foreach (partWork pw in chip.lstPartWork)
                    {
                        //未使用のパートの場合は処理を行わない
                        if (!pw.chip.use) continue;

                        log.Write("pcm sound off");

                        //pcm sound off
                        if (pw.pcmWaitKeyOnCounter == 0)
                        {
                            pw.pcmWaitKeyOnCounter = -1;
                        }

                        log.Write("KeyOff");
                        ProcKeyOff(pw);

                        log.Write("Bend");
                        ProcBend(pw);

                        log.Write("Lfo");
                        ProcLfo(pw);

                        log.Write("Envelope");
                        ProcEnvelope(pw);

                        log.Write("wait消化待ち");
                        if (pw.waitCounter > 0)
                        {
                            continue;
                        }

                        log.Write("データは最後まで実施されたか");
                        if (pw.dataEnd)
                        {
                            continue;
                        }

                        log.Write("パートのデータがない場合は何もしないで次へ");
                        if (!partData.ContainsKey(pw.PartName) || partData[pw.PartName] == null || partData[pw.PartName].Count < 1)
                        {
                            pw.dataEnd = true;
                            continue;
                        }

                        log.Write("コマンド毎の処理を実施");
                        while (pw.waitCounter == 0 && !pw.dataEnd)
                        {
                            char cmd = pw.getChar();
                            if (cmd == 0)
                            {
                                pw.dataEnd = true;
                            }
                            else
                            {
                                lineNumber = pw.getLineNumber();
                                Commander(pw, cmd);
                            }
                        }

                    }

                    log.Write("channelを跨ぐコマンド向け処理");
                    if (chip is YM2608 && chip.use)
                    {
                        ((YM2608)chip).MultiChannelCommand();
                    }

                    if (chip is YM2610B && chip.use)
                    {
                        ((YM2610B)chip).MultiChannelCommand();
                    }

                    //PCMをストリームの機能を使用し再生するため、1Frame毎にカレントチャンネル情報が破壊される。よって次のフレームでリセットできるようにする。
                    if (chip is HuC6280 && chip.use)
                    {
                        ((HuC6280)chip).CurrentChannel = 255;
                    }
                }


                log.Write("全パートのうち次のコマンドまで一番近い値を求める");
                long cnt = long.MaxValue;
                foreach (ClsChip chip in chips)
                {
                    for (int ch = 0; ch < chip.lstPartWork.Count; ch++)
                    {

                        partWork cpw = chip.lstPartWork[ch];

                        if (!cpw.chip.use) continue;

                        //note
                        if (cpw.waitKeyOnCounter > 0)
                        {
                            cnt = Math.Min(cnt, cpw.waitKeyOnCounter);
                        }
                        else if (cpw.waitCounter > 0)
                        {
                            cnt = Math.Min(cnt, cpw.waitCounter);
                        }

                        //bend
                        if (cpw.bendWaitCounter != -1)
                        {
                            cnt = Math.Min(cnt, cpw.bendWaitCounter);
                        }

                        //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                        if (cnt > 0)
                        {
                            //lfo
                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!cpw.lfo[lfo].sw) continue;
                                if (cpw.lfo[lfo].waitCounter == -1) continue;

                                cnt = Math.Min(cnt, cpw.lfo[lfo].waitCounter);
                            }

                            //envelope
                            if (
                                (cpw.chip is YM2203 && cpw.Type == enmChannelType.SSG)
                                || (cpw.chip is YM2608 && (cpw.Type == enmChannelType.SSG || cpw.Type == enmChannelType.RHYTHM || cpw.Type == enmChannelType.ADPCM))
                                || (cpw.chip is YM2610B && (cpw.Type == enmChannelType.SSG || cpw.Type == enmChannelType.ADPCMA || cpw.Type == enmChannelType.ADPCMB))
                                || cpw.chip is SN76489
                                || cpw.chip is segaPcm
                                || cpw.chip is RF5C164
                                || cpw.chip is HuC6280
                                )
                            {
                                if (cpw.envelopeMode && cpw.envIndex != -1)
                                {
                                    cnt = Math.Min(cnt, cpw.envCounter);
                                }
                            }
                        }

                        //pcm
                        if (cpw.pcmWaitKeyOnCounter > 0)
                        {
                            cnt = Math.Min(cnt, cpw.pcmWaitKeyOnCounter);
                        }

                    }

                }

                log.Write("全パートのwaitcounterを減らす");
                if (cnt != long.MaxValue)
                {

                    // waitcounterを減らす

                    foreach (ClsChip chip in chips)
                    {
                        foreach (partWork pw in chip.lstPartWork)
                        {

                            if (pw.waitKeyOnCounter > 0) pw.waitKeyOnCounter -= cnt;

                            if (pw.waitCounter > 0) pw.waitCounter -= cnt;

                            if (pw.bendWaitCounter > 0) pw.bendWaitCounter -= cnt;

                            for (int lfo = 0; lfo < 4; lfo++)
                            {
                                if (!pw.lfo[lfo].sw) continue;
                                if (pw.lfo[lfo].waitCounter == -1) continue;

                                if (pw.lfo[lfo].waitCounter > 0)
                                {
                                    pw.lfo[lfo].waitCounter -= cnt;
                                    if (pw.lfo[lfo].waitCounter < 0) pw.lfo[lfo].waitCounter = 0;
                                }
                            }

                            if (pw.pcmWaitKeyOnCounter > 0)
                            {
                                pw.pcmWaitKeyOnCounter -= cnt;
                            }

                            if (
                                (pw.chip is YM2203 && pw.Type == enmChannelType.SSG)
                                || (pw.chip is YM2608 && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.RHYTHM || pw.Type == enmChannelType.ADPCM))
                                || (pw.chip is YM2610B && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB))
                                || pw.chip is SN76489
                                || pw.chip is segaPcm
                                || pw.chip is RF5C164
                                || pw.chip is HuC6280
                                )
                            {
                                if (pw.envelopeMode && pw.envIndex != -1)
                                {
                                    pw.envCounter -= (int)cnt;
                                }
                            }
                        }
                    }

                    // wait発行

                    lClock += cnt;
                    dSample += samplesPerClock * cnt;

                    if (ym2612[0].lstPartWork[5].pcmWaitKeyOnCounter <= 0)//== -1)
                    {
                        OutWaitNSamples((long)(samplesPerClock * cnt));
                    }
                    else
                    {
                        OutWaitNSamplesWithPCMSending(ym2612[0].lstPartWork[5], cnt);
                    }

                }

                log.Write("終了パートのカウント");
                endChannel = 0;
                foreach (ClsChip chip in chips)
                {
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        if (!pw.chip.use) endChannel++;
                        else if (pw.dataEnd && pw.waitCounter < 1) endChannel++;
                        else if (loopOffset != -1 && pw.dataEnd && pw.envIndex == 3) endChannel++;
                    }
                }

            } while (endChannel < totalChannel);


            log.Write("フッター情報の作成");
            MakeFooter();


            return dat.ToArray();
        }

        public byte[] Xgm_getByteData()
        {
            if (userClockCount != 0) clockCount = userClockCount;

            PartInit();

            dat = new List<byte>();
            xdat = new List<byte>();

            Xgm_makeHeader();

            int endChannel = 0;
            //使用するチャンネル数を計算
            int totalChannel = 0;
            foreach (ClsChip chip in chips)
            {
                if (chip.ShortName != "OPN2X" && chip.ShortName != "DCSG")
                {
                    foreach (partWork pw in chip.lstPartWork) pw.chip.use = false;
                }
                totalChannel += chip.ChMax;
            }

            do
            {
                //KeyOnリストをクリア
                xgmKeyOnData = new List<byte>();

                foreach (ClsChip chip in chips)
                {
                    //未使用のchipの場合は処理を行わない
                    if (!chip.use) continue;

                    //chip毎の処理
                    Xgm_procChip(chip);
                }

                //一番小さいwait値を調べる
                long waitCounter = Xgm_procCheckMinimumWaitCounter();

                //KeyOn情報をかき出し
                foreach (byte dat in xgmKeyOnData) OutData(0x52, 0x28, dat);

                if (waitCounter != long.MaxValue)
                {
                    //wait処理
                    Xgm_procWait(waitCounter);
                }

                //演奏完了したチャンネル数をカウント
                endChannel = 0;
                foreach (ClsChip chip in chips)
                {
                    foreach (partWork pw in chip.lstPartWork)
                    {
                        if (!pw.chip.use) endChannel++;
                        else if (pw.dataEnd && pw.waitCounter < 1) endChannel++;
                        else if (loopOffset != -1 && pw.dataEnd && pw.envIndex == 3) endChannel++;
                    }
                }

            } while (endChannel < totalChannel);//全てのチャンネルが終了していない場合はループする

            dat = ConvertVGMtoXGM(dat);

            Xgm_makeFooter();

            return dat.ToArray();
        }

        private void Xgm_procChip(ClsChip chip)
        {
            foreach (partWork pw in chip.lstPartWork)
            {
                //KeyOff
                ProcKeyOff(pw);
                //Bend
                ProcBend(pw);
                //Lfo
                ProcLfo(pw);
                //Envelope
                ProcEnvelope(pw);

                //wait消化待ち
                if (pw.waitCounter > 0) continue;

                //データは最後まで実施されたか
                if (pw.dataEnd) continue;

                //パートのデータがない場合は何もしないで次へ
                if (!partData.ContainsKey(pw.PartName) || partData[pw.PartName] == null || partData[pw.PartName].Count < 1)
                {
                    pw.dataEnd = true;
                    continue;
                }

                //コマンド毎の処理を実施
                while (pw.waitCounter == 0 && !pw.dataEnd)
                {
                    char cmd = pw.getChar();
                    if (cmd == 0)
                    {
                        pw.dataEnd = true;
                    }
                    else
                    {
                        lineNumber = pw.getLineNumber();
                        Commander(pw, cmd);
                    }
                }
            }
        }

        private long Xgm_procCheckMinimumWaitCounter()
        {
            long cnt = long.MaxValue;

            foreach (ClsChip chip in chips)
            {
                if (!chip.use) continue;

                foreach (partWork pw in chip.lstPartWork)
                {
                    //note
                    if (pw.waitKeyOnCounter > 0) cnt = Math.Min(cnt, pw.waitKeyOnCounter);
                    else if (pw.waitCounter > 0) cnt = Math.Min(cnt, pw.waitCounter);

                    //bend
                    if (pw.bendWaitCounter != -1) cnt = Math.Min(cnt, pw.bendWaitCounter);

                    //lfoとenvelopeは音長によるウエイトカウントが存在する場合のみ対象にする。(さもないと、曲のループ直前の効果を出せない)
                    if (cnt < 1) continue;

                    //lfo
                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        if (!pw.lfo[lfo].sw) continue;
                        if (pw.lfo[lfo].waitCounter == -1) continue;

                        cnt = Math.Min(cnt, pw.lfo[lfo].waitCounter);
                    }

                    //envelope
                    if (!(pw.chip is SN76489)) continue;
                    if (pw.envelopeMode && pw.envIndex != -1) cnt = Math.Min(cnt, pw.envCounter);
                }
            }

            return cnt;
        }

        private void Xgm_procWait(long cnt)
        {
            foreach (ClsChip chip in chips)
            {
                if (!chip.use) continue;

                foreach (partWork pw in chip.lstPartWork)
                {
                    if (pw.waitKeyOnCounter > 0) pw.waitKeyOnCounter -= cnt;
                    if (pw.waitCounter > 0) pw.waitCounter -= cnt;
                    if (pw.bendWaitCounter > 0) pw.bendWaitCounter -= cnt;

                    for (int lfo = 0; lfo < 4; lfo++)
                    {
                        if (!pw.lfo[lfo].sw) continue;
                        if (pw.lfo[lfo].waitCounter == -1) continue;

                        if (pw.lfo[lfo].waitCounter > 0)
                        {
                            pw.lfo[lfo].waitCounter -= cnt;
                            if (pw.lfo[lfo].waitCounter < 0) pw.lfo[lfo].waitCounter = 0;
                        }
                    }

                    if (!(pw.chip is SN76489)) continue;
                    if (pw.envelopeMode && pw.envIndex != -1) pw.envCounter -= (int)cnt;
                }
            }

            // wait発行
            lClock += cnt;
            dSample += samplesPerClock * cnt;

            sampleB += samplesPerClock * cnt;
            OutWaitNSamples((long)(sampleB));
            sampleB -= (long)sampleB;
        }

        private List<byte> ConvertVGMtoXGM(List<byte> src)
        {
            if (src == null || src.Count < 1) return null;

            List<byte> des = new List<byte>();
            loopOffset = -1;

            int[][] opn2reg = new int[2][] { new int[0x100], new int[0x100] };
            for (int i = 0; i < 512; i++) opn2reg[i / 0x100][i % 0x100] = -1;
            int[] psgreg = new int[16];
            int psgch = -1;
            int psgtp = -1;
            for (int i = 0; i < 16; i++) psgreg[i] = -1;
            int framePtr = 0;
            int frameCnt = 0;

            for (int ptr = 0; ptr < src.Count; ptr++)
            {

                byte cmd = src[ptr];
                int p;
                int c;

                switch (cmd)
                {
                    case 0x61: //Wait

                        if (psgtp != -1)
                        {
                            p = des.Count;
                            c = 0;
                            des.Add(0x10);
                            for (int j = 0; j < 16; j++)
                            {
                                if (psgreg[j] == -1) continue;
                                int latch = (j & 1) == 0 ? 0x80 : 0;
                                int ch = (j & 0x0c) << 3;
                                int tp = (j & 2) << 3;
                                des.Add((byte)(latch | (latch != 0 ? (ch | tp) : 0) | psgreg[j]));
                                c++;
                            }
                            c--;
                            des[p] |= (byte)c;

                            psgch = -1;
                            psgtp = -1;
                            for (int i = 0; i < 16; i++) psgreg[i] = -1;
                        }

                        if (des.Count - framePtr > 256)
                        {
                            msgBox.setWrnMsg(string.Format("1Frameに収められる限界バイト数(256byte)を超えています。データを分散させてください。 Frame {0} : {1}byte", frameCnt, des.Count - framePtr));
                        }
                        framePtr = des.Count;

                        int cnt = src[ptr + 1] + src[ptr + 2] * 0x100;
                        for (int j = 0; j < cnt; j++)
                        {
                            //wait
                            des.Add(0x00);
                            frameCnt++;
                        }
                        ptr += 2;
                        break;
                    case 0x50: //DCSG
                        do
                        {
                            bool latch = (src[ptr + 1] & 0x80) != 0;
                            int ch = (src[ptr + 1] & 0x60) >> 5;
                            int tp = (src[ptr + 1] & 0x10) >> 3;
                            int d1 = (src[ptr + 1] & 0xf);
                            int d2 = (src[ptr + 1] & 0x3f);
                            if (latch)
                            {
                                psgch = ch;
                                psgtp = tp;
                                psgreg[ch * 4 + 0 + tp] = d1;
                            }
                            else
                            {
                                if (psgch != -1)
                                {
                                    psgreg[psgch * 4 + 1 + psgtp] = d2;
                                }
                                psgch = -1;
                            }
                            ptr += 2;
                        }while(ptr < src.Count - 1 && src[ptr] == 0x50);
                        ptr--;
                        break;
                    case 0x52: //YM2612 Port0
                        if (opn2reg[0][src[ptr + 1]] != src[ptr + 2] || src[ptr + 1] == 0x28)
                        {

                            bool isKeyOn = src[ptr + 1] == 0x28;
                            if (!isKeyOn)
                            {
                                p = des.Count;
                                c = 0;
                                des.Add(0x20);
                                do
                                {
                                    if (opn2reg[0][src[ptr + 1]] != src[ptr + 2])
                                    {
                                        //F-numの場合は圧縮対象外
                                        if(src[ptr+1]<0xa0 || src[ptr+1]>=0xb0) opn2reg[0][src[ptr + 1]] = src[ptr + 2];

                                        des.Add(src[ptr + 1]);
                                        des.Add(src[ptr + 2]);
                                        c++;
                                    }
                                    ptr += 3;
                                } while (c < 16 && ptr < src.Count - 1 && src[ptr] == 0x52 && src[ptr + 1] != 0x28);
                                c--;
                                ptr--;
                                des[p] |= (byte)c;
                            }
                            else
                            {
                                p = des.Count;
                                c = 0;
                                des.Add(0x40);
                                do
                                {
                                    //des.Add(src[ptr + 1]);
                                    des.Add(src[ptr + 2]);
                                    c++;
                                    ptr += 3;
                                } while (c < 16 && ptr < src.Count - 1 && src[ptr] == 0x52 && src[ptr + 1] == 0x28);
                                c--;
                                ptr--;
                                des[p] |= (byte)c;
                            }
                        }
                        else
                        {
                            ptr += 2;
                        }
                        break;
                    case 0x53: //YM2612 Port1
                        if (opn2reg[1][src[ptr + 1]] != src[ptr + 2])
                        {

                            p = des.Count;
                            c = 0;
                            des.Add(0x30);
                            do
                            {
                                if (opn2reg[1][src[ptr + 1]] != src[ptr + 2])
                                {
                                    //F-numの場合は圧縮対象外
                                    if (src[ptr + 1] < 0xa0 || src[ptr + 1] >= 0xb0) opn2reg[1][src[ptr + 1]] = src[ptr + 2];
                                    des.Add(src[ptr + 1]);
                                    des.Add(src[ptr + 2]);
                                    c++;
                                }
                                ptr += 3;
                            } while (c < 16 && ptr < src.Count - 1 && src[ptr] == 0x53);
                            c--;
                            ptr--;
                            des[p] |= (byte)c;
                        }
                        else
                        {
                            ptr += 2;
                        }
                        break;
                    case 0x54: //PCM KeyON (YM2151)
                        des.Add(src[ptr + 1]);
                        des.Add(src[ptr + 2]);
                        ptr += 2;
                        break;
                    case 0x7e: //LOOP Point
                        loopOffset = des.Count;
                        for (int i = 0; i < 512; i++) opn2reg[i / 0x100][i % 0x100] = -1;
                        break;
                    default:
                        return null;
                }
            }

            if (loopOffset == -1 || loopOffset == des.Count)
            {
                des.Add(0x7f);
            }
            else
            {
                des.Add(0x7e);
                des.Add((byte)(loopOffset & 0xff));
                des.Add((byte)((loopOffset & 0xff00) >> 8));
                des.Add((byte)((loopOffset & 0xff0000) >> 16));
            }

            return des;
        }

        private void ProcEnvelope(partWork pw)
        {
            //Envelope処理
            if (
                (pw.chip is YM2203 && pw.Type == enmChannelType.SSG)
                || (pw.chip is YM2608 && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.RHYTHM || pw.Type == enmChannelType.ADPCM))
                || (pw.chip is YM2610B && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.ADPCMA || pw.Type == enmChannelType.ADPCMB))
                || pw.chip is SN76489
                || pw.chip is RF5C164
                || pw.chip is segaPcm
                || pw.chip is HuC6280
                )
            {
                Envelope(pw);
            }

            if (pw.chip is YM2151)
            {
                ((YM2151)pw.chip).SetFNum(pw);
                ((YM2151)pw.chip).SetVolume(pw);
            }
            else if ((pw.chip is YM2203 && pw.ch < 3)
                || (pw.chip is YM2608 && pw.ch < 6)
                || (pw.chip is YM2610B && pw.ch < 6)
                || (pw.chip is YM2612)
                || (pw.chip is YM2612X)
                )
            {
                SetFmFNum(pw);
                SetFmVolume(pw);
            }
            else if ((pw.chip is YM2203 || pw.chip is YM2608 || pw.chip is YM2610B) && pw.Type == enmChannelType.SSG)
            {
                SetSsgFNum(pw);
                SetSsgVolume(pw);
            }
            else if (pw.chip is YM2608 && pw.Type == enmChannelType.ADPCM)
            {
                ((YM2608)pw.chip).SetAdpcmFNum(pw);
                ((YM2608)pw.chip).SetAdpcmVolume(pw);
            }
            else if (pw.chip is YM2610B && pw.Type == enmChannelType.ADPCMB)
            {
                ((YM2610B)pw.chip).SetAdpcmBFNum(pw);
                ((YM2610B)pw.chip).SetAdpcmBVolume(pw);
            }
            else if (pw.chip is SN76489)
            {
                if (pw.waitKeyOnCounter > 0 || pw.envIndex != -1)
                {
                    SetDcsgFNum(pw);
                    SetPsgVolume(pw);
                }
            }
            else if (pw.chip is segaPcm)
            {
                if (pw.waitKeyOnCounter > 0 || pw.envIndex != -1)
                {
                    SetSegaPcmFNum(pw);
                    OutSegaPcmVolume(pw);
                }
            }
            else if (pw.chip is RF5C164)
            {
                if (pw.waitKeyOnCounter > 0 || pw.envIndex != -1)
                {
                    SetRf5c164FNum(pw);
                    SetRf5c164Volume(pw);
                }
            }
            else if (pw.chip is HuC6280)
            {
                if (pw.waitKeyOnCounter > 0 || pw.envIndex != -1)
                {
                    SetHuC6280FNum(pw);
                    SetHuC6280Volume(pw);
                }
            }
        }

        private void ProcLfo(partWork cpw)
        {
            //lfo処理
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = cpw.lfo[lfo];

                if (!pl.sw)
                {
                    continue;
                }
                if (pl.waitCounter > 0)//== -1)
                {
                    continue;
                }

                if (pl.type == eLfoType.Hardware)
                {
                    if ((cpw.chip is YM2612)|| (cpw.chip is YM2612X))
                    {
                        cpw.ams = pl.param[3];
                        cpw.fms = pl.param[2];
                        OutOPNSetPanAMSPMS(cpw, (int)cpw.pan.val, cpw.ams, cpw.fms);
                        cpw.chip.lstPartWork[0].hardLfoSw = true;
                        cpw.chip.lstPartWork[0].hardLfoNum = pl.param[1];
                        OutOPNSetHardLfo(cpw, cpw.hardLfoSw, cpw.hardLfoNum);
                        pl.waitCounter = -1;
                    }
                    continue;
                }

                switch (pl.param[4])
                {
                    case 0: //三角
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.param[3]) || (pl.direction < 0 && pl.value <= -pl.param[3]))
                        {
                            pl.value = pl.param[3] * pl.direction;
                            pl.direction = -pl.direction;
                        }
                        break;
                    case 1: //のこぎり
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.param[3]) || (pl.direction < 0 && pl.value <= -pl.param[3]))
                        {
                            pl.value = -pl.param[3] * pl.direction;
                        }
                        break;
                    case 2: //矩形
                        pl.value = pl.param[3] * pl.direction;
                        pl.waitCounter = pl.param[1];
                        pl.direction = -pl.direction;
                        break;
                    case 3: //ワンショット
                        pl.value += Math.Abs(pl.param[2]) * pl.direction;
                        pl.waitCounter = pl.param[1];
                        if ((pl.direction > 0 && pl.value >= pl.param[3]) || (pl.direction < 0 && pl.value <= -pl.param[3]))
                        {
                            pl.waitCounter = -1;
                        }
                        break;
                    case 4: //ランダム
                        pl.value = rnd.Next(-pl.param[3], pl.param[3]);
                        pl.waitCounter = pl.param[1];
                        break;
                }

            }
        }

        private static void ProcBend(partWork pw)
        {
            //bend処理
            if (pw.bendWaitCounter == 0)
            {
                if (pw.bendList.Count > 0)
                {
                    Tuple<int, int> bp = pw.bendList.Pop();
                    pw.bendFnum = bp.Item1;
                    pw.bendWaitCounter = bp.Item2;
                }
                else
                {
                    pw.bendWaitCounter = -1;
                }
            }
        }

        private void ProcKeyOff(partWork pw)
        {
            if (pw.waitKeyOnCounter == 0)
            {
                if (pw.chip is YM2151)
                {
                    if (!pw.tie)
                    {
                        ((YM2151)pw.chip).OutKeyOff(pw);
                    }
                }
                else if ((pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B))
                {
                    int m = (pw.chip is YM2203) ? 0 : 3;

                    if (!pw.tie)
                    {
                        if (pw.ch < m + 6) OutFmKeyOff(pw);
                        else if (pw.Type == enmChannelType.SSG)
                        {
                            if (!pw.envelopeMode)
                            {
                                OutSsgKeyOff(pw);
                            }
                            else
                            {
                                if (pw.envIndex != -1)
                                {
                                    pw.envIndex = 3;//RR phase
                                    pw.envCounter = 0;
                                }
                            }
                        }
                        else if ((pw.Type == enmChannelType.RHYTHM) || (pw.Type == enmChannelType.ADPCMA))
                        {
                            pw.keyOn = false;
                            pw.keyOff = true;
                            //((YM2610B)pw.chip).adpcmA_KeyOff |= (byte)(1 << (pw.ch - 12));
                        }
                        else if (pw.Type == enmChannelType.ADPCM)
                        {
                            if (!pw.envelopeMode)
                            {
                                ((YM2608)pw.chip).OutAdpcmKeyOff(pw);
                            }
                            else
                            {
                                if (pw.envIndex != -1)
                                {
                                    pw.envIndex = 3;//RR phase
                                    pw.envCounter = 0;
                                }
                            }
                        }
                        else if (pw.Type == enmChannelType.ADPCMB)
                        {
                            if (!pw.envelopeMode)
                            {
                                ((YM2610B)pw.chip).OutAdpcmBKeyOff(pw);
                            }
                            else
                            {
                                if (pw.envIndex != -1)
                                {
                                    pw.envIndex = 3;//RR phase
                                    pw.envCounter = 0;
                                }
                            }
                        }
                    }
                }
                else if (pw.chip is YM2612)
                {
                    if (!pw.tie)
                    {
                        OutFmKeyOff(pw);
                    }
                }
                else if (pw.chip is YM2612X)
                {
                    if (!pw.tie)
                    {
                        OutFmKeyOff(pw);
                    }
                }
                else if (pw.chip is SN76489)
                {
                    if (!pw.envelopeMode)
                    {
                        if (!pw.tie) OutPsgKeyOff(pw);
                    }
                    else
                    {
                        if (pw.envIndex != -1)
                        {
                            if (!pw.tie)
                            {
                                pw.envIndex = 3;//RR phase
                                pw.envCounter = 0;
                            }
                        }
                    }
                }
                else if (pw.chip is RF5C164)
                {
                    if (!pw.envelopeMode)
                    {
                        if (!pw.tie) OutRf5c164KeyOff(pw);
                    }
                    else
                    {
                        if (pw.envIndex != -1)
                        {
                            if (!pw.tie)
                            {
                                pw.envIndex = 3;//RR phase
                                pw.envCounter = 0;
                            }
                        }
                    }
                }
                else if (pw.chip is segaPcm)
                {
                    if (!pw.envelopeMode)
                    {
                        if (!pw.tie) OutSegaPcmKeyOff(pw);
                    }
                    else
                    {
                        if (pw.envIndex != -1)
                        {
                            if (!pw.tie)
                            {
                                pw.envIndex = 3;//RR phase
                                pw.envCounter = 0;
                            }
                        }
                    }
                }
                else if (pw.chip is HuC6280)
                {
                    if (!pw.envelopeMode)
                    {
                        if (!pw.tie) OutHuC6280KeyOff(pw);
                    }
                    else
                    {
                        if (pw.envIndex != -1)
                        {
                            if (!pw.tie)
                            {
                                pw.envIndex = 3;//RR phase
                                pw.envCounter = 0;
                            }
                        }
                    }
                }


                //次回に引き継ぎリセット
                pw.beforeTie = pw.tie;
                pw.tie = false;

                //ゲートタイムカウンターをリセット
                pw.waitKeyOnCounter = -1;
            }
        }

        private void PartInit()
        {
            foreach (ClsChip chip in chips)
            {

                chip.use = false;
                chip.lstPartWork = new List<partWork>();

                for (int i = 0; i < chip.ChMax; i++)
                {
                    partWork pw = new partWork()
                    {
                        chip = chip,
                        isSecondary = (chip.ChipID == 1),
                        ch = i// + 1;
                    };
                    if (partData.ContainsKey(chip.Ch[i].Name))
                    {
                        pw.pData = partData[chip.Ch[i].Name];
                    }
                    pw.aData = aliesData;
                    pw.setPos(0);
                    pw.Type = chip.Ch[i].Type;
                    pw.slots = 0;
                    pw.volume = 32767;

                    pw.chip.InitPart(ref pw);

                    pw.PartName = chip.Ch[i].Name;
                    pw.waitKeyOnCounter = -1;
                    pw.waitCounter = 0;
                    pw.freq = -1;

                    pw.dataEnd = false;
                    if (pw.pData == null || pw.pData.Count < 1)
                    {
                        pw.dataEnd = true;
                    }
                    else
                    {
                        chip.use = true;
                    }

                    chip.lstPartWork.Add(pw);

                }
            }

        }

        private void Envelope(partWork pw)
        {
            if (!pw.envelopeMode)
            {
                return;
            }

            if (pw.envIndex == -1)
            {
                return;
            }

            int maxValue = pw.MaxVolume;

            while (pw.envCounter == 0 && pw.envIndex != -1)
            {
                switch (pw.envIndex)
                {
                    case 0: //AR phase
                        pw.envVolume += pw.envelope[7]; // vol += ST
                        if (pw.envVolume >= maxValue)
                        {
                            pw.envVolume = maxValue;
                            pw.envCounter = pw.envelope[3]; // DR
                            pw.envIndex++;
                            break;
                        }
                        pw.envCounter = pw.envelope[2]; // AR
                        break;
                    case 1: //DR phase
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        if (pw.envVolume <= pw.envelope[4]) // vol <= SL
                        {
                            pw.envVolume = pw.envelope[4];
                            pw.envCounter = pw.envelope[5]; // SR
                            pw.envIndex++;
                            break;
                        }
                        pw.envCounter = pw.envelope[3]; // DR
                        break;
                    case 2: //SR phase
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        if (pw.envVolume <= 0) // vol <= 0
                        {
                            pw.envVolume = 0;
                            pw.envCounter = 0;
                            pw.envIndex = -1;
                            break;
                        }
                        pw.envCounter = pw.envelope[5]; // SR
                        break;
                    case 3: //RR phase
                        pw.envVolume -= pw.envelope[7]; // vol -= ST
                        if (pw.envVolume <= 0) // vol <= 0
                        {
                            pw.envVolume = 0;
                            pw.envCounter = 0;
                            pw.envIndex = -1;
                            break;
                        }
                        pw.envCounter = pw.envelope[6]; // RR
                        break;
                }
            }

            if (pw.envIndex == -1)
            {
                if (pw.chip is SN76489)
                {
                    OutPsgKeyOff(pw);
                }
                else if (pw.chip is YM2203)
                {
                    if (pw.Type == enmChannelType.SSG)
                    {
                        OutSsgKeyOff(pw);
                    }
                }
                else if (pw.chip is YM2608)
                {
                    if (pw.Type == enmChannelType.SSG)
                    {
                        OutSsgKeyOff(pw);
                    }
                    else if (pw.Type == enmChannelType.ADPCM)
                    {
                        ((YM2608)pw.chip).OutAdpcmKeyOff(pw);
                    }
                }
                else if (pw.chip is YM2610B)
                {
                    if (pw.Type == enmChannelType.SSG)
                    {
                        OutSsgKeyOff(pw);
                    }
                    else if (pw.Type == enmChannelType.ADPCMB)
                    {
                        ((YM2610B)pw.chip).OutAdpcmBKeyOff(pw);
                    }
                }
                else if (pw.chip is RF5C164)
                {
                    OutRf5c164KeyOff(pw);
                }
                else if (pw.chip is segaPcm)
                {
                    OutSegaPcmKeyOff(pw);
                }
                else if (pw.chip is HuC6280)
                {
                    OutHuC6280KeyOff(pw);
                }
            }
        }


        private void Commander(partWork pw, char cmd)
        {

            switch (cmd)
            {
                case ' ':
                case '\t':
                    pw.incPos();
                    break;
                case '!': // CompileSkip
                    log.Write("CompileSkip");
                    pw.dataEnd = true;
                    pw.waitCounter = -1;
                    break;
                case 'T': // tempo
                    log.Write(" tempo");
                    CmdTempo(pw);
                    break;
                case '@': // instrument
                    log.Write("instrument");
                    CmdInstrument(pw);
                    break;
                case 'v': // volume
                    log.Write("volume");
                    CmdVolume(pw);
                    break;
                case 'V': // totalVolume(Adpcm-A / Rhythm)
                    log.Write("totalVolume(Adpcm-A / Rhythm)");
                    CmdTotalVolume(pw);
                    break;
                case 'o': // octave
                    log.Write("octave");
                    CmdOctave(pw);
                    break;
                case '>': // octave Up
                    log.Write("octave Up");
                    CmdOctaveUp(pw);
                    break;
                case '<': // octave Down
                    log.Write("octave Down");
                    CmdOctaveDown(pw);
                    break;
                case ')': // volume Up
                    log.Write(" volume Up");
                    CmdVolumeUp(pw);
                    break;
                case '(': // volume Down
                    log.Write("volume Down");
                    CmdVolumeDown(pw);
                    break;
                case 'l': // length
                    log.Write("length");
                    CmdLength(pw);
                    break;
                case '#': // length(clock)
                    log.Write("length(clock)");
                    CmdClockLength(pw);
                    break;
                case 'p': // pan
                    log.Write(" pan");
                    CmdPan(pw);
                    break;
                case 'D': // Detune
                    log.Write("Detune");
                    CmdDetune(pw);
                    break;
                case 'm': // pcm mode
                    log.Write("pcm mode");
                    CmdMode(pw);
                    break;
                case 'q': // gatetime
                    log.Write(" gatetime q");
                    CmdGatetime(pw);
                    break;
                case 'Q': // gatetime
                    log.Write("gatetime Q");
                    CmdGatetime2(pw);
                    break;
                case 'E': // envelope
                    log.Write("envelope");
                    CmdEnvelope(pw);
                    break;
                case 'L': // loop point
                    log.Write(" loop point");
                    CmdLoop(pw);
                    break;
                case '[': // repeat
                    log.Write("repeat [");
                    CmdRepeatStart(pw);
                    break;
                case ']': // repeat
                    log.Write("repeat ]");
                    CmdRepeatEnd(pw);
                    break;
                case '{': // renpu
                    log.Write("renpu {");
                    CmdRenpuStart(pw);
                    break;
                case '}': // renpu
                    log.Write("renpu }");
                    CmdRenpuEnd(pw);
                    break;
                case '/': // repeat
                    log.Write("repeat /");
                    CmdRepeatExit(pw);
                    break;
                case 'M': // lfo
                    log.Write("lfo");
                    CmdLfo(pw);
                    break;
                case 'S': // lfo switch
                    log.Write(" lfo switch");
                    CmdLfoSwitch(pw);
                    break;
                case 'y': // y
                    log.Write(" y");
                    CmdY(pw);
                    break;
                case 'w': // noise
                    log.Write("noise");
                    CmdNoise(pw);
                    break;
                case 'P': // noise or tone mixer
                    log.Write("noise or tone mixer");
                    CmdMixer(pw);
                    break;
                case 'K': // key shift
                    log.Write("key shift");
                    CmdKeyShift(pw);
                    break;
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'a':
                case 'b':
                case 'r':
                    log.Write(string.Format("note {0}", cmd));
                    CmdNote(pw, cmd);
                    break;
                default:
                    msgBox.setErrMsg(string.Format("未知のコマンド{0}を検出しました。", cmd), pw.getSrcFn(), pw.getLineNumber());
                    pw.incPos();
                    break;
            }
        } 

        private void CmdNote(partWork pw, char cmd)
        {
            pw.incPos();

            //+ -の解析
            int shift = 0;
            shift = AnaSharp(pw, ref shift);

            if (cmd == 'r' && shift != 0)
            {
                msgBox.setWrnMsg("休符での+、-の指定は無視されます。", pw.getSrcFn(), pw.getLineNumber());
            }

            int ml = 0;
            int n = -1;
            bool directFlg = false;
            bool isMinus = false;
            bool isTieType2 = false;
            bool isSecond = false;
            do
            {
                int m = 0;

                //数値の解析
                //if (!pw.getNum(out n))
                if (!pw.getNumNoteLength(out n, out directFlg))
                {
                    if (!isSecond)
                        n = (int)pw.length;
                    else if (!isMinus)
                    {
                        if (!isTieType2)
                        {
                            //タイとして'&'が使用されている
                            pw.tie = true;
                        }
                        else
                        {
                            n = (int)pw.length;
                        }
                    }
                }
                else
                {
                    if (n == 0)
                    {
                        if (pw.Type != enmChannelType.FMOPM
                            && pw.Type != enmChannelType.FMOPN
                            && pw.Type != enmChannelType.FMOPNex
                            )
                        {
                            msgBox.setErrMsg("Tone Doublerが使用できるのはFM音源のみです。", pw.getSrcFn(), pw.getLineNumber());
                            return;
                        }
                        pw.TdA = pw.octaveNew * 12 + Const.NOTE.IndexOf(cmd) + shift + pw.keyShift;
                        pw.octaveNow = pw.octaveNew;

                        return;
                    }

                    if (!directFlg)
                    {
                        if ((int)clockCount % n != 0)
                        {
                            msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        n = (int)clockCount / n;
                    }
                    else
                    {
                        n = Common.CheckRange(n, 1, 65535);
                    }
                }

                //Tone Doubler
                if (pw.getChar() == ',')
                {
                    if (pw.Type != enmChannelType.FMOPM
                        && pw.Type != enmChannelType.FMOPN
                        && pw.Type != enmChannelType.FMOPNex
                        )
                    {
                        msgBox.setErrMsg("Tone Doublerが使用できるのはFM音源のみです。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }
                    pw.TdA = pw.octaveNew * 12 + Const.NOTE.IndexOf(cmd) + shift + pw.keyShift;
                    pw.octaveNow = pw.octaveNew;
                    pw.incPos();

                    return;
                }

                if (!pw.tie || isTieType2)
                {
                    m += n;

                    //符点の解析
                    while (pw.getChar() == '.')
                    {
                        if (n % 2 != 0)
                        {
                            msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", pw.getSrcFn(), pw.getLineNumber());
                        }
                        n = n / 2;
                        m += n;
                        pw.incPos();
                    }


                    if (isMinus) ml -= m;
                    else ml += m;
                }

                isTieType2 = false;

                //ベンドの解析
                int bendDelayCounter = 0;
                int bendShift = 0;
                if (pw.getChar() == '_')
                {
                    pw.incPos();
                    pw.octaveNow = pw.octaveNew;
                    pw.bendOctave = pw.octaveNow;
                    pw.bendNote = 'r';
                    pw.bendWaitCounter = -1;
                    bool loop = true;
                    while (loop)
                    {
                        char bCmd = pw.getChar();
                        switch (bCmd)
                        {
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                            case 'g':
                            case 'a':
                            case 'b':
                                loop = false;
                                pw.incPos();
                                //+ -の解析
                                bendShift = 0;
                                bendShift = AnaSharp(pw, ref bendShift);
                                pw.bendShift = bendShift;
                                bendDelayCounter = 0;
                                n = -1;
                                isMinus = false;
                                isTieType2 = false;
                                isSecond = false;
                                do
                                {
                                    m = 0;

                                    //数値の解析
                                    //if (!pw.getNum(out n))
                                    if (!pw.getNumNoteLength(out n, out directFlg))
                                    {
                                        if (!isSecond)
                                        {
                                            n = 0;
                                            break;
                                        }
                                        else if (!isMinus)
                                        {
                                            if (!isTieType2)
                                            {
                                                //タイとして'&'が使用されている
                                                pw.tie = true;
                                            }
                                            else
                                            {
                                                n = (int)pw.length;
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (!directFlg)
                                        {
                                            if ((int)clockCount % n != 0)
                                            {
                                                msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), pw.getSrcFn(), pw.getLineNumber());
                                            }
                                            n = (int)clockCount / n;
                                        }
                                        else
                                        {
                                            n = Common.CheckRange(n, 1, 65535);
                                        }
                                    }

                                    if (!pw.tie || isTieType2)
                                    {
                                        bendDelayCounter += n;

                                        //符点の解析
                                        while (pw.getChar() == '.')
                                        {
                                            if (n % 2 != 0)
                                            {
                                                msgBox.setWrnMsg("割り切れない.の指定があります。音長は不定です。", pw.getSrcFn(), pw.getLineNumber());
                                            }
                                            n = n / 2;
                                            m += n;
                                            pw.incPos();
                                        }


                                        if (isMinus) bendDelayCounter -= m;
                                        else bendDelayCounter += m;
                                    }

                                    isTieType2 = false;

                                    if (pw.getChar() == '&')
                                    {
                                        isMinus = false;
                                        isTieType2 = false;
                                    }
                                    else if (pw.getChar() == '^')
                                    {
                                        isMinus = false;
                                        isTieType2 = true;
                                    }
                                    else if (pw.getChar() == '~')
                                    {
                                        isMinus = true;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    isSecond = true;
                                    pw.incPos();

                                } while (true);

                                if (cmd != 'r')
                                {
                                    pw.bendNote = bCmd;
                                    bendDelayCounter = Common.CheckRange(bendDelayCounter, 0, ml);
                                    pw.bendWaitCounter = bendDelayCounter;
                                }
                                else
                                {
                                    msgBox.setErrMsg("休符にベンドの指定はできません。", pw.getSrcFn(), pw.getLineNumber());
                                }

                                break;
                            case 'o':
                                pw.incPos();
                                if (!pw.getNum(out n))
                                {
                                    msgBox.setErrMsg("不正なオクターブが指定されています。", pw.getSrcFn(), pw.getLineNumber());
                                    n = 110;
                                }
                                n = Common.CheckRange(n, 1, 8);
                                pw.bendOctave = n;
                                break;
                            case '>':
                                pw.incPos();
                                pw.bendOctave += octaveRev ? -1 : 1;
                                pw.bendOctave = Common.CheckRange(pw.bendOctave, 1, 8);
                                break;
                            case '<':
                                pw.incPos();
                                pw.bendOctave += octaveRev ? 1 : -1;
                                pw.bendOctave = Common.CheckRange(pw.bendOctave, 1, 8);
                                break;
                            default:
                                loop = false;
                                break;
                        }
                    }

                    //音符の変化量
                    int ed = Const.NOTE.IndexOf(pw.bendNote) + 1 + (pw.bendOctave - 1) * 12 + pw.bendShift;
                    ed = Common.CheckRange(ed, 0, 8 * 12 - 1);
                    int st = Const.NOTE.IndexOf(cmd) + 1 + (pw.octaveNow - 1) * 12 + shift;//
                    st = Common.CheckRange(st, 0, 8 * 12 - 1);

                    int delta = ed - st;
                    if (delta == 0 || bendDelayCounter == ml)
                    {
                        pw.bendNote = 'r';
                        pw.bendWaitCounter = -1;
                    }
                    else
                    {

                        //１音符当たりのウエイト
                        float wait = (ml - bendDelayCounter - 1) / (float)delta;
                        float tl = 0;
                        float bf = Math.Sign(wait);
                        List<int> lstBend = new List<int>();
                        int toneDoublerShift = GetToneDoublerShift(pw, pw.octaveNow, cmd, shift);
                        for (int i = 0; i < Math.Abs(delta); i++)
                        {
                            bf += wait;
                            tl += wait;
                            int a = GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                            int b = GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            if (pw.chip is YM2151)
                            {
                                a = ((YM2151)pw.chip).GetFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta) + toneDoublerShift);//
                                b = ((YM2151)pw.chip).GetFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta) + toneDoublerShift);//
                            }
                            else if (
                                (pw.chip is YM2608 && (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex))
                                || (pw.chip is YM2610B && (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex))
                                || (pw.chip is YM2612)
                                || (pw.chip is YM2612X)
                                )
                            {
                                //int[] ftbl = ((pw.chip is YM2612) || (pw.chip is YM2612X)) ? OPN_FNumTbl_7670454 : pw.chip.FNumTbl[0];
                                int[] ftbl = pw.chip.FNumTbl[0];

                                a = GetFmFNum(ftbl, pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta) + toneDoublerShift);//
                                b = GetFmFNum(ftbl, pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta) + toneDoublerShift);//
                                int oa = (a & 0xf000) / 0x1000;
                                int ob = (b & 0xf000) / 0x1000;
                                if (oa != ob)
                                {
                                    if ((a & 0xfff) == ftbl[0])
                                    {
                                        oa += Math.Sign(ob - oa);
                                        a = (a & 0xfff) * 2 + oa * 0x1000;
                                    }
                                    else if ((b & 0xfff) == ftbl[0])
                                    {
                                        ob += Math.Sign(oa - ob);
                                        b = (b & 0xfff) * ((delta > 0) ? 2 : 1) + ob * 0x1000;
                                    }
                                }
                            }
                            else if (
                                (pw.chip is YM2608 && pw.Type == enmChannelType.SSG)
                                || (pw.chip is YM2610B && pw.Type == enmChannelType.SSG)
                                )
                            {
                                a = GetSsgFNum(pw, pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = GetSsgFNum(pw, pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else if (pw.chip is SN76489)
                            {
                                a = GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = GetDcsgFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else if (pw.chip is RF5C164)
                            {
                                a = GetRf5c164PcmNote(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = GetRf5c164PcmNote(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else if (pw.chip is segaPcm)
                            {
                                a = GetSegaPcmFNum(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = GetSegaPcmFNum(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }
                            else if (pw.chip is HuC6280)
                            {
                                a = GetHuC6280Freq(pw.octaveNow, cmd, shift + (i + 0) * Math.Sign(delta));//
                                b = GetHuC6280Freq(pw.octaveNow, cmd, shift + (i + 1) * Math.Sign(delta));//
                            }

                            if (Math.Abs(bf) >= 1.0f)
                            {
                                for (int j = 0; j < (int)Math.Abs(bf); j++)
                                {
                                    int c = b - a;
                                    int d = (int)Math.Abs(bf);
                                    lstBend.Add((int)(a + ((float)c / (float)d) * (float)j));
                                }
                                bf -= (int)bf;
                            }

                        }
                        Stack<Tuple<int, int>> lb = new Stack<Tuple<int, int>>();
                        int of = -1;
                        int cnt = 1;
                        foreach (int f in lstBend)
                        {
                            if (of == f)
                            {
                                cnt++;
                                continue;
                            }
                            lb.Push(new Tuple<int, int>(f, cnt));
                            of = f;
                            cnt = 1;
                        }
                        pw.bendList = new Stack<Tuple<int, int>>();
                        foreach (Tuple<int, int> lbt in lb)
                        {
                            pw.bendList.Push(lbt);
                        }
                        Tuple<int, int> t = pw.bendList.Pop();
                        pw.bendFnum = t.Item1;
                        pw.bendWaitCounter = t.Item2;
                    }
                }

                if (pw.getChar() == '&')
                {
                    isMinus = false;
                    isTieType2 = false;
                }
                else if (pw.getChar() == '^')
                {
                    isMinus = false;
                    isTieType2 = true;
                }
                else if (pw.getChar() == '~')
                {
                    isMinus = true;
                }
                else
                {
                    break;
                }

                isSecond = true;
                pw.incPos();

            } while (true);

            if (ml < 1)
            {
                msgBox.setErrMsg("負の音長が指定されました。", pw.getSrcFn(), pw.getLineNumber());
                ml = (int)pw.length;
            }


            if (pw.renpuFlg)
            {
                if (pw.lstRenpuLength!=null && pw.lstRenpuLength.Count > 0)
                {
                    ml = pw.lstRenpuLength[0];
                    pw.lstRenpuLength.RemoveAt(0);
                }
            }


            //装飾の解析完了



            //WaitClockの決定
            pw.waitCounter = ml;

            if (cmd != 'r')
            {
                if (pw.reqFreqReset)
                {
                    pw.freq = -1;
                    pw.reqFreqReset = false;
                }

                //発音周波数
                if (pw.bendWaitCounter == -1)
                {
                    pw.octaveNow = pw.octaveNew;
                    pw.noteCmd = cmd;
                    pw.shift = shift;

                    //Tone Doubler
                    SetToneDoubler(pw);
                }
                else
                {
                    pw.octaveNow = pw.octaveNew;
                    pw.noteCmd = cmd;
                    pw.shift = shift;

                    //Tone Doubler
                    SetToneDoubler(pw);

                    pw.octaveNew = pw.bendOctave;//
                    pw.octaveNow = pw.bendOctave;//
                    pw.noteCmd = pw.bendNote;
                    pw.shift = pw.bendShift;
                }

                //強制設定
                pw.freq = -1;

                //発音周波数の決定とキーオン
                if (pw.chip is YM2151)
                {

                    ((YM2151)pw.chip).SetFNum(pw);

                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        SetLfoAtKeyOn(pw);
                        ((YM2151)pw.chip).SetVolume(pw);
                        ((YM2151)pw.chip).OutKeyOn(pw);
                    }
                }
                else if (pw.chip is YM2203)
                {

                    //YM2203

                    if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                    {
                        SetFmFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetLfoAtKeyOn(pw);
                            SetFmVolume(pw);
                            OutFmKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.SSG)
                    {
                        SetSsgFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetEnvelopeAtKeyOn(pw);
                            SetLfoAtKeyOn(pw);
                            OutSsgKeyOn(pw);
                        }
                    }
                }
                else if (pw.chip is YM2608)
                {

                    //YM2608

                    if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                    {
                        SetFmFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetLfoAtKeyOn(pw);
                            SetFmVolume(pw);
                            OutFmKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.SSG)
                    {
                        SetSsgFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetEnvelopeAtKeyOn(pw);
                            SetLfoAtKeyOn(pw);
                            OutSsgKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.RHYTHM)
                    {
                        if (!pw.beforeTie)
                        {
                            pw.keyOn = true;
                        }
                    }
                    else if (pw.Type == enmChannelType.ADPCM)
                    {
                        ((YM2608)pw.chip).SetAdpcmFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetEnvelopeAtKeyOn(pw);
                            SetLfoAtKeyOn(pw);
                            ((YM2608)pw.chip).OutAdpcmKeyOn(pw);
                        }
                    }
                }
                else if (pw.chip is YM2610B)
                {

                    //YM2610B

                    if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                    {
                        SetFmFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetLfoAtKeyOn(pw);
                            SetFmVolume(pw);
                            OutFmKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.SSG)
                    {
                        SetSsgFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetEnvelopeAtKeyOn(pw);
                            SetLfoAtKeyOn(pw);
                            OutSsgKeyOn(pw);
                        }
                    }
                    else if (pw.Type == enmChannelType.ADPCMA)
                    {
                        if (!pw.beforeTie)
                        {

                            pw.keyOn = true;
                            //if ((((YM2610B)pw.chip).adpcmA_KeyOn & (1 << (pw.ch - 12))) != 0)
                            //{
                            //    ((YM2610B)pw.chip).adpcmA_KeyOff |= (byte)(1 << (pw.ch - 12));
                            //    ((YM2610B)pw.chip).adpcmA_beforeKeyOn &= (byte)~(1 << (pw.ch - 12));
                            //}
                            //((YM2610B)pw.chip).adpcmA_KeyOn |= (byte)(1 << (pw.ch - 12));

                        }
                    }
                    else if (pw.Type == enmChannelType.ADPCMB)
                    {
                        ((YM2610B)pw.chip).SetAdpcmBFNum(pw);

                        //タイ指定では無い場合はキーオンする
                        if (!pw.beforeTie)
                        {
                            SetEnvelopeAtKeyOn(pw);
                            SetLfoAtKeyOn(pw);
                            ((YM2610B)pw.chip).OutAdpcmBKeyOn(pw);
                        }
                    }
                }
                else if (pw.chip is YM2612)
                {

                    //YM2612

                    if (!pw.pcm)
                    {
                        SetFmFNum(pw);
                    }
                    else
                    {
                        GetPcmNote(pw);
                    }
                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        SetLfoAtKeyOn(pw);
                        SetFmVolume(pw);
                        OutFmKeyOn(pw);
                    }
                }
                else if (pw.chip is YM2612X)
                {

                    //YM2612X

                    if (!pw.pcm)
                    {
                        SetFmFNum(pw);
                    }
                    else
                    {
                        GetPcmNote(pw);
                    }
                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        SetLfoAtKeyOn(pw);
                        SetFmVolume(pw);
                        OutFmKeyOn(pw);
                    }
                }
                else if (pw.chip is SN76489)
                {

                    // SN76489

                    SetDcsgFNum(pw);

                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        SetEnvelopeAtKeyOn(pw);
                        SetLfoAtKeyOn(pw);
                        OutPsgKeyOn(pw);
                    }
                }
                else if (pw.chip is RF5C164)
                {

                    // RF5C164

                    SetRf5c164FNum(pw);

                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        SetEnvelopeAtKeyOn(pw);
                        SetLfoAtKeyOn(pw);
                        SetRf5c164Envelope(pw, pw.volume);
                        OutRf5c164KeyOn(pw);
                    }
                }
                else if (pw.chip is segaPcm)
                {
                    SetSegaPcmFNum(pw);

                    if (!pw.beforeTie)
                    {
                        SetEnvelopeAtKeyOn(pw);
                        SetLfoAtKeyOn(pw);
                        OutSegaPcmKeyOn(pw);
                    }
                }
                else if (pw.chip is HuC6280)
                {

                    // HuC6280

                    SetHuC6280FNum(pw);

                    //タイ指定では無い場合はキーオンする
                    if (!pw.beforeTie)
                    {
                        SetEnvelopeAtKeyOn(pw);
                        SetLfoAtKeyOn(pw);
                        SetHuC6280Envelope(pw, pw.volume);
                        OutHuC6280KeyOn(pw);
                    }
                }

                //gateTimeの決定
                if (pw.gatetimePmode)
                {
                    pw.waitKeyOnCounter = pw.waitCounter * pw.gatetime / 8L;
                }
                else
                {
                    pw.waitKeyOnCounter = pw.waitCounter - pw.gatetime;
                }
                if (pw.waitKeyOnCounter < 1) pw.waitKeyOnCounter = 1;

                //PCM専用のWaitClockの決定
                if (pw.pcm)
                {
                    pw.pcmWaitKeyOnCounter = -1;
                    if (Version == 1.51f)
                    {
                        pw.pcmWaitKeyOnCounter = pw.waitKeyOnCounter;
                    }

                    if (instPCM == null || instPCM.Count - 1 < pw.instrument)
                    {
                        pw.pcmSizeCounter = 0;
                    }
                    else
                    {
                        pw.pcmSizeCounter = instPCM[pw.instrument].size;
                    }

                }
            }
            else
            {
                if (pw.reqKeyOffReset)
                {
                    if (pw.chip is SN76489)
                    {
                        OutPsgKeyOff(pw);
                        pw.reqKeyOffReset = false;
                    }
                }
            }

            pw.clockCounter += pw.waitCounter;
        }

        private static int AnaSharp(partWork pw, ref int shift)
        {
            while (pw.getChar() == '+' || pw.getChar() == '-')
            {
                shift += pw.getChar() == '+' ? 1 : -1;
                pw.incPos();
            }

            return shift;
        }

        private void CmdRepeatExit(partWork pw)
        {
            int n = -1;
            pw.incPos();
            clsRepeat rx = pw.stackRepeat.Pop();
            if (rx.repeatCount == 1)
            {
                int i = 0;
                while (true)
                {
                    char c = pw.getChar();
                    if (c == ']')
                    {
                        if (i == 0)
                        {
                            break;
                        }
                        else
                            i--;
                    }
                    else if (c == '[')
                    {
                        i++;
                    }
                    pw.incPos();
                }
                pw.incPos();
                pw.getNum(out n);
            }
            else
            {
                pw.stackRepeat.Push(rx);
            }

        }

        private void CmdRepeatEnd(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                n = 2;
            }
            n = Common.CheckRange(n, 1, 255);
            try
            {
                clsRepeat re = pw.stackRepeat.Pop();
                if (re.repeatCount == -1)
                {
                    //初回
                    re.repeatCount = n;
                }
                re.repeatCount--;
                if (re.repeatCount > 0)
                {
                    pw.stackRepeat.Push(re);
                    pw.setPos(re.pos);
                }
            }
            catch
            {
                msgBox.setWrnMsg("[と]の数があいません。", pw.getSrcFn(), pw.getLineNumber());
            }
        }

        private void CmdRepeatStart(partWork pw)
        {
            pw.incPos();
            clsRepeat rs = new clsRepeat()
            {
                pos = pw.getPos(),
                repeatCount = -1//初期値
            };
            pw.stackRepeat.Push(rs);
        }

        private void CmdRenpuStart(partWork pw)
        {
            if (!pw.renpuFlg)
            {
                //MML解析
                List<Tuple<int, string>> lstRenpu = new List<Tuple<int, string>>();
                List<int> lstRenpuLength = new List<int>();
                int nest = 0;
                int pos = 0;
                pw.incPos();
                pos = pw.getPos();
                DecStep1Renpu(pw, lstRenpu, nest);
                if (lstRenpu.Count < 1)
                {
                    msgBox.setErrMsg(
                        "連符コマンドの解析に失敗しました。動作は不定となります。"
                        , pw.getSrcFn(), pw.getLineNumber());
                    return;
                }
                DecStep2Renpu(lstRenpu, lstRenpuLength, 1, 0);

                pw.setPos(pos);
                pw.renpuFlg = true;
                pw.lstRenpuLength = lstRenpuLength;
            }
            else
            {
                pw.incPos();
            }
        }

        private void CmdRenpuEnd(partWork pw)
        {
            if(pw.renpuFlg && pw.lstRenpuLength.Count == 0)
            {
                pw.renpuFlg = false;
                pw.lstRenpuLength = null;
            }
            pw.incPos();

            //数値指定のスキップ
            pw.getNumNoteLength(out int n, out bool directFlg);

        }


        private bool SkipCommander(partWork pw,char ch)
        {

            //true カウント
            //false 無視
            switch (ch)
            {
                case ' ':
                case '\t':
                case '!': // CompileSkip
                case '>': // octave Up
                case '<': // octave Down
                case 'L': // loop point
                    pw.incPos();
                    break;
                case 'T': // tempo
                case 'v': // volume
                case 'o': // octave
                case ')': // volume Up
                case '(': // volume Down
                case 'l': // length
                case '#': // length(clock)
                case 'D': // Detune
                case 'q': // gatetime
                case 'Q': // gatetime
                case 'K': // key shift
                    pw.incPos();
                    pw.getNum(out int n);
                    break;
                case '@': // instrument
                    SkipCmdInstrument(pw, ch);
                    break;
                case 'V': // totalVolume(Adpcm-A / Rhythm)
                    SkipCmdTotalVolume(pw);
                    break;
                case 'p': // pan
                    SkipCmdPan(pw);
                    break;
                case 'm': // pcm mode
                    SkipCmdMode(pw);
                    break;
                case 'E': // envelope
                    SkipCmdEnvelope(pw);
                    break;
                //case '[': // repeat
                //case ']': // repeat
                //case '/': // repeat
                case 'M': // lfo
                    SkipCmdLfo(pw);
                    break;
                case 'S': // lfo switch
                    SkipCmdLfoSwitch(pw);
                    break;
                case 'y': // y 
                    SkipCmdY(pw);
                    break;
                case 'w': // noise
                    SkipCmdNoise(pw);
                    break;
                case 'P': // noise or tone mixer
                    SkipCmdMixer(pw);
                    break;
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'a':
                case 'b':
                case 'r':
                    SkipCmdNote(pw, ch);
                    return true;
                default:
                    msgBox.setErrMsg(
                        string.Format("連符コマンドの中で未対応のコマンド({0})が使用されています。動作は不定となります。", ch)
                        , pw.getSrcFn(), pw.getLineNumber());
                    pw.incPos();
                    break;
            }

            return false;
        }

        private void SkipCmdNoise(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.Type == enmChannelType.DCSGNOISE 
                || pw.Type == enmChannelType.SSG 
                || (pw.chip is YM2151) 
                || (pw.chip is HuC6280 && pw.ch > 3))
            {
                pw.getNum(out n);
            }
        }

        private void SkipCmdMixer(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if ((pw.Type == enmChannelType.SSG && (
                (pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B)
                ))
                || (pw.chip is YM2151)
                || (pw.chip is HuC6280 && pw.ch > 3)
                )
            {
                pw.getNum(out n);
            }
        }

        private void SkipCmdY(partWork pw)
        {
            int n = -1;
            pw.incPos();

            char c = pw.getChar();
            if (c >= 'A' && c <= 'Z')
            {
                string toneparamName = "" + c;
                pw.incPos();
                toneparamName += pw.getChar();
                pw.incPos();
                if (toneparamName != "TL" && toneparamName != "SR")
                {
                    toneparamName += pw.getChar();
                    pw.incPos();
                    if (toneparamName != "SSG")
                    {
                        toneparamName += pw.getChar();
                        pw.incPos();
                    }
                }

                if (toneparamName == "DT1M" || toneparamName == "DT2S" || toneparamName == "PMSA")
                {
                    toneparamName += pw.getChar();
                    pw.incPos();
                    if (toneparamName == "PMSAM")
                    {
                        toneparamName += pw.getChar();
                        pw.incPos();
                    }
                }

                pw.incPos();

                if (toneparamName != "FBAL" && toneparamName != "PMSAMS")
                {
                    pw.getNum(out n);
                    pw.incPos();
                }

                pw.getNum(out n);

                return;
            }

            pw.getNum(out n);
            pw.incPos();
            pw.getNum(out n);

        }

        private void SkipCmdLfo(partWork pw)
        {
            pw.incPos();
            char c = pw.getChar();
            if (c == 'A')
            {
                pw.incPos();
                if (pw.getChar() == 'M')
                {
                    pw.incPos();
                    if (pw.getChar() == 'S') SkipCmdMAMS_PMS(pw);
                }
                return;
            }
            if (c < 'P' && c > 'S') return;
            pw.incPos();
            char t = pw.getChar();
            if (c == 'P' && t == 'M')
            {
                pw.incPos();
                if (pw.getChar() == 'S') SkipCmdMAMS_PMS(pw);
                return;
            }
            if (t != 'T' && t != 'V' && t != 'H') return;
            int n = -1;
            do
            {
                pw.incPos();
                if (!pw.getNum(out n)) return;
                while (pw.getChar() == '\t' || pw.getChar() == ' ') pw.incPos(); 
            } while (pw.getChar() == ',');
        }

        private void SkipCmdLfoSwitch(partWork pw)
        {
            pw.incPos();
            if (pw.getChar() < 'P' || pw.getChar() > 'S')
            {
                pw.incPos();
                return;
            }
            int n = -1;
            pw.incPos();
            if (!pw.getNum(out n)) return;
        }

        private void SkipCmdMAMS_PMS(partWork pw)
        {
            if (!((pw.chip is YM2151) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612) || (pw.chip is YM2612X)
                || (pw.Type == enmChannelType.FMOPM) || (pw.Type == enmChannelType.FMOPN)))
                return;

            int n = -1;
            pw.incPos();
            pw.getNum(out n);
        }

        private void SkipCmdTotalVolume(partWork pw)
        {
            pw.incPos();
            pw.getNum(out int n);
            if (pw.chip is HuC6280)
            {
                pw.incPos();
                pw.getNum(out n);
            }
        }

        private void SkipCmdInstrument(partWork pw, char cmd)
        {
            int n;
            pw.incPos();

            if (pw.getChar() == 'T')
            {
                if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
                {
                    pw.incPos();
                    return;
                }
                pw.incPos();
                pw.getNum(out n);
                return;
            }

            if ((pw.chip is YM2151) || (pw.chip is YM2203))
            {
                pw.getNum(out n);
            }
            else if (pw.chip is YM2608)
            {
                if ((pw.ch < 9) || (pw.Type == enmChannelType.SSG))
                {
                    pw.getNum(out n);
                }
                else if ((pw.Type == enmChannelType.RHYTHM) || (pw.Type == enmChannelType.ADPCM))
                {
                    if (pw.getChar() != 'E')
                    {
                        pw.getNum(out n);
                    }
                    else
                    {
                        pw.incPos();
                        pw.getNum(out n);
                    }
                }
            }
            else if (pw.chip is YM2610B)
            {
                if ((pw.ch < 9) || (pw.Type == enmChannelType.SSG))
                {
                    pw.getNum(out n);
                }
                else if ((pw.Type == enmChannelType.ADPCMA) || (pw.Type == enmChannelType.ADPCMB))
                {
                    if (pw.getChar() != 'E')
                    {
                        pw.getNum(out n);
                    }
                    else
                    {
                        pw.incPos();
                        pw.getNum(out n);
                    }
                }
            }
            else if ((pw.chip is YM2612) || (pw.chip is YM2612X) || (pw.chip is SN76489))
            {
                pw.getNum(out n);
            }
            else if ((pw.chip is RF5C164) || (pw.chip is segaPcm) || (pw.chip is HuC6280))
            {
                if (pw.getChar() != 'E')
                {
                    pw.getNum(out n);
                }
                else
                {
                    pw.incPos();
                    pw.getNum(out n);
                }
            }

        }

        private void SkipCmdNote(partWork pw, char cmd)
        {
            pw.incPos();

            //+ -の解析
            int shift = 0;
            shift = AnaSharp(pw, ref shift);

            int n = -1;
            bool directFlg = false;
            bool isMinus = false;
            bool isTieType2 = false;
            bool isSecond = false;
            bool pwTie = false;
            do
            {
                //数値の解析
                if (!pw.getNumNoteLength(out n, out directFlg))
                {
                    if (!isSecond)
                        n = (int)pw.length;
                    else if (!isMinus)
                    {
                        if (!isTieType2)
                        {
                            //タイとして'&'が使用されている
                            pwTie = true;
                        }
                        else
                        {
                            n = (int)pw.length;
                        }
                    }
                }
                else
                {
                    if (n == 0) return;
                }

                //Tone Doubler
                if (pw.getChar() == ',')
                {
                    pw.incPos();
                    return;
                }

                if (!pwTie || isTieType2)
                {
                    //符点の解析
                    while (pw.getChar() == '.') pw.incPos();
                }

                isTieType2 = false;

                //ベンドの解析
                if (pw.getChar() == '_')
                {
                    pw.incPos();
                    bool loop = true;
                    while (loop)
                    {
                        char bCmd = pw.getChar();
                        switch (bCmd)
                        {
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                            case 'g':
                            case 'a':
                            case 'b':
                                loop = false;
                                pw.incPos();
                                //+ -の解析
                                AnaSharp(pw, ref shift);
                                isMinus = false;
                                isTieType2 = false;
                                isSecond = false;
                                do
                                {
                                    //数値の解析
                                    if (!pw.getNumNoteLength(out n, out directFlg))
                                    {
                                        if (!isSecond)
                                        {
                                            break;
                                        }
                                        else if (!isMinus)
                                        {
                                            if (!isTieType2)
                                            {
                                                //タイとして'&'が使用されている
                                                pwTie = true;
                                            }
                                            break;
                                        }
                                    }

                                    if (!pw.tie || isTieType2)
                                    {
                                        //符点の解析
                                        while (pw.getChar() == '.') pw.incPos();
                                    }

                                    isTieType2 = false;

                                    if (pw.getChar() == '&')
                                    {
                                        isMinus = false;
                                        isTieType2 = false;
                                    }
                                    else if (pw.getChar() == '^')
                                    {
                                        isMinus = false;
                                        isTieType2 = true;
                                    }
                                    else if (pw.getChar() == '~')
                                    {
                                        isMinus = true;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    isSecond = true;
                                    pw.incPos();

                                } while (true);

                                break;
                            case 'o':
                                pw.incPos();
                                break;
                            case '>':
                                pw.incPos();
                                break;
                            case '<':
                                pw.incPos();
                                break;
                            default:
                                loop = false;
                                break;
                        }
                    }

                }

                if (pw.getChar() == '&')
                {
                    isMinus = false;
                    isTieType2 = false;
                }
                else if (pw.getChar() == '^')
                {
                    isMinus = false;
                    isTieType2 = true;
                }
                else if (pw.getChar() == '~')
                {
                    isMinus = true;
                }
                else
                {
                    break;
                }

                isSecond = true;
                pw.incPos();

            } while (true);


            //装飾の解析完了


        }

        private void SkipCmdPan(partWork pw)
        {
            int n;

            pw.incPos();

            if ((pw.chip is YM2151) || (pw.chip is YM2608) || (pw.chip is YM2610B)
                || (pw.chip is YM2612) || (pw.chip is YM2612X) || (pw.chip is SN76489))
            {
                pw.getNum(out n);
            }
            else if ((pw.chip is RF5C164) || (pw.chip is segaPcm) || (pw.chip is HuC6280))
            {
                pw.getNum(out n);
                pw.incPos();
                pw.getNum(out n);
            }

        }

        private void SkipCmdMode(partWork pw)
        {
            pw.incPos();
            pw.getNum(out int n);

        }

        private void SkipCmdEnvelope(partWork pw)
        {
            int n = -1;
            if (
                ((pw.chip is YM2203) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2608) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2610B) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2612) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2612X) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
                || (pw.chip is SN76489)
                || (pw.chip is RF5C164)
                || (pw.chip is segaPcm)
                || (pw.chip is HuC6280)
                )
            {
                pw.incPos();
                switch (pw.getChar())
                {
                    case 'O':
                        pw.incPos();
                        pw.incPos();
                        break;
                    case 'X':
                        if (
                            ((pw.chip is YM2203) && pw.Type == enmChannelType.FMOPNex)
                            || ((pw.chip is YM2608) && pw.Type == enmChannelType.FMOPNex)
                            || ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
                            || pw.chip is YM2612
                            || pw.chip is YM2612X
                            )
                        {
                            pw.incPos();
                            pw.getNum(out n);
                        }
                        break;
                    default:
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                pw.getNum(out n);
                                if (i == 3) break;
                                pw.incPos();
                            }
                            break;
                        }
                        else
                        {
                            pw.incPos();
                        }
                        break;
                }
            }
            else
            {
                pw.incPos();
            }

        }


        private void DecStep1Renpu(partWork pw, List<Tuple<int, string>> lstRenpu, int nest)
        {
            nest++;
            int count = 0;
            string str = "";
            char ch;
            while ((ch = pw.getChar()) != '}')
            {
                if (pw.dataEnd) { return; }
                if (ch == '{')
                {
                    pw.incPos();
                    DecStep1Renpu(pw,  lstRenpu, nest);
                    ch = '*';
                    str += ch;
                    count++;
                }
                else
                {
                    if (SkipCommander(pw,ch))
                    {
                        str += ch;
                        count++;
                    }
                }
            }
            pw.incPos();

            //数値解析(ネスト中は単なるスキップ
            if (!pw.getNumNoteLength(out int n, out bool directFlg))
            {
                n = (int)pw.length;
            }
            else
            {
                if (!directFlg)
                {
                    if ((int)clockCount % n != 0)
                    {
                        msgBox.setWrnMsg(string.Format("割り切れない音長({0})の指定があります。音長は不定になります。", n), pw.getSrcFn(), pw.getLineNumber());
                    }
                    n = (int)clockCount / n;
                }
                else
                {
                    n = Common.CheckRange(n, 1, 65535);
                }
            }
            if (nest > 1) n = -1;

            lstRenpu.Add(new Tuple<int, string>(n, str));
            nest--;
        }

        private void DecStep2Renpu(List<Tuple<int, string>> lstRenpu,List<int> lstRenpuLength, int nest,int len)
        {
            Tuple<int, string> t = lstRenpu[lstRenpu.Count - nest];
            if (t.Item1 != -1) len = t.Item1;

            for (int p = 0; p < t.Item2.Length; p++)
            {
                int le = len / t.Item2.Length + ((len % t.Item2.Length) == 0 ? 0 : ((len % t.Item2.Length) > p ? 1 : 0));
                if (t.Item2[p] != '*') lstRenpuLength.Add(le);
                else DecStep2Renpu(lstRenpu,lstRenpuLength, nest + 1, le);
            }
        }

        private void CmdLoop(partWork pw)
        {
            pw.incPos();
            loopOffset = (long)dat.Count;
            loopSamples = (long)dSample;

            if (format == enmFormat.XGM)
            {
                OutData(0x7e);
            }

            foreach (ClsChip chip in chips)
            {
                foreach (partWork p in chip.lstPartWork)
                {
                    p.reqFreqReset = true;
                    p.beforeLVolume = -1;
                    p.beforeRVolume = -1;
                    p.beforeVolume = -1;
                    p.pan = new dint(3);
                    p.beforeTie = false;

                    if (p.chip is SN76489 && sn76489[p.isSecondary ? 1 : 0].use)
                    {
                        p.reqKeyOffReset = true;
                    }

                    if (p.chip is RF5C164 && rf5c164[p.isSecondary ? 1 : 0].use)
                    {
                        //rf5c164の設定済み周波数値を初期化(ループ時に直前の周波数を引き継いでしまうケースがあるため)
                        p.rf5c164AddressIncrement = -1;
                        int n = p.instrument;
                        p.pcmStartAddress = -1;
                        p.pcmLoopAddress = -1;
                        if (n != -1)
                        {
                            SetRf5c164CurrentChannel(p);
                            SetRf5c164SampleStartAddress(p, (int)instPCM[n].stAdr);
                            SetRf5c164LoopAddress(p, (int)(instPCM[n].loopAdr));
                        }
                    }

                    if (p.chip is HuC6280 && huc6280[p.isSecondary ? 1 : 0].use)
                    {
                        huc6280[p.isSecondary ? 1 : 0].CurrentChannel = 255;
                        //setHuC6280CurrentChannel(pw);
                        p.beforeFNum = -1;
                        p.huc6280Envelope = -1;
                        p.huc6280Pan = -1;
                    }

                }
            }
        }

        private void CmdEnvelope(partWork pw)
        {
            int n = -1;
            if (
                ((pw.chip is YM2203) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2608) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2610B) && (pw.Type == enmChannelType.SSG || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2612) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
                || ((pw.chip is YM2612X) && (pw.Type == enmChannelType.FMPCM || pw.Type == enmChannelType.FMOPNex))
                || (pw.chip is SN76489)
                || (pw.chip is RF5C164)
                || (pw.chip is segaPcm)
                || (pw.chip is HuC6280)
                )
            {
                pw.incPos();
                switch (pw.getChar())
                {
                    case 'O':
                        pw.incPos();
                        if (
                            ((pw.chip is YM2203) && pw.Type == enmChannelType.FMOPNex)
                            || ((pw.chip is YM2608) && pw.Type == enmChannelType.FMOPNex)
                            || ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
                            || pw.chip is YM2612
                            || pw.chip is YM2612X
                            )
                        {
                            switch (pw.getChar())
                            {
                                case 'N':
                                    pw.incPos();
                                    pw.Ch3SpecialMode = true;
                                    OutOPNSetCh3SpecialMode(pw, true);
                                    break;
                                case 'F':
                                    pw.incPos();
                                    pw.Ch3SpecialMode = false;
                                    OutOPNSetCh3SpecialMode(pw, false);
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw.getChar()), pw.getSrcFn(), pw.getLineNumber());
                                    pw.incPos();
                                    break;
                            }
                        }
                        else
                        {
                            switch (pw.getChar())
                            {
                                case 'N':
                                    pw.incPos();
                                    pw.envelopeMode = true;
                                    break;
                                case 'F':
                                    pw.incPos();
                                    pw.envelopeMode = false;
                                    if (pw.Type == enmChannelType.SSG)
                                    {
                                        pw.beforeVolume = -1;
                                    }
                                    break;
                                default:
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EO{0})が指定されました。", pw.getChar()), pw.getSrcFn(), pw.getLineNumber());
                                    pw.incPos();
                                    break;
                            }
                        }
                        break;
                    case 'X':
                        char c = pw.getChar();
                        if (
                            ((pw.chip is YM2203) && pw.Type == enmChannelType.FMOPNex)
                            || ((pw.chip is YM2608) && pw.Type == enmChannelType.FMOPNex)
                            || ((pw.chip is YM2610B) && pw.Type == enmChannelType.FMOPNex)
                            || pw.chip is YM2612
                            || pw.chip is YM2612X
                            )
                        {
                            pw.incPos();
                            if (!pw.getNum(out n))
                            {
                                msgBox.setErrMsg("不正なスロット指定'EX'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                                n = 0;
                            }
                            byte res = 0;
                            while (n % 10 != 0)
                            {
                                if (n % 10 > 0 && n % 10 < 5)
                                {
                                    res += (byte)(1 << (n % 10 - 1));
                                }
                                else
                                {
                                    msgBox.setErrMsg(string.Format("未知のコマンド(EX{0})が指定されました。", n), pw.getSrcFn(), pw.getLineNumber());
                                    break;
                                }
                                n /= 10;
                            }
                            if (res != 0)
                            {
                                pw.slots = res;
                            }
                        }
                        else
                        {
                            msgBox.setErrMsg("未知のコマンド(EX)が指定されました。", pw.getSrcFn(), pw.getLineNumber());
                        }
                        break;
                    default:
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            int[] s = new int[] { 0, 0, 0, 0 };

                            for (int i = 0; i < 4; i++)
                            {
                                if (pw.getNum(out n))
                                {
                                    s[i] = n;
                                }
                                else
                                {
                                    msgBox.setErrMsg("Eコマンドの解析に失敗しました。", pw.getSrcFn(), pw.getLineNumber());
                                    break;
                                }
                                if (i == 3) break;
                                pw.incPos();
                            }
                            pw.slotDetune = s;
                            break;
                        }
                        else
                        {
                            msgBox.setErrMsg(string.Format("未知のコマンド(E{0})が指定されました。", pw.getChar()), pw.getSrcFn(), pw.getLineNumber());
                            pw.incPos();
                        }
                        break;
                }
            }
            else
            {
                msgBox.setWrnMsg("このパートは効果音モードに対応したチャンネルが指定されていないため、Eコマンドは無視されます。", pw.getSrcFn(), pw.getLineNumber());
                pw.incPos();
            }

        }

        private void CmdGatetime2(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'Q'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 1;
            }
            n = Common.CheckRange(n, 1, 8);
            pw.gatetime = n;
            pw.gatetimePmode = true;
        }

        private void CmdGatetime(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正なゲートタイム指定'q'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 0;
            }
            n = Common.CheckRange(n, 0, 255);
            pw.gatetime = n;
            pw.gatetimePmode = false;
        }

        private void CmdMode(partWork pw)
        {
            int n;
            pw.incPos();
            if (pw.chip is YM2612 && pw.Type == enmChannelType.FMPCM)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なPCMモード指定'm'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 1);
                pw.pcm = (n == 1);
                pw.freq = -1;//freqをリセット
                pw.instrument = -1;
                ((YM2612)(pw.chip)).OutSetCh6PCMMode(pw, pw.pcm);
            }
            else if (pw.chip is YM2612X && pw.Type == enmChannelType.FMPCMex)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なPCMモード指定'm'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 1);
                pw.chip.lstPartWork[5].pcm = (n == 1);
                pw.chip.lstPartWork[9].pcm = (n == 1);
                pw.chip.lstPartWork[10].pcm = (n == 1);
                pw.chip.lstPartWork[11].pcm = (n == 1);
                pw.freq = -1;//freqをリセット
                pw.instrument = -1;
                ((YM2612X)(pw.chip)).OutSetCh6PCMMode(pw.chip.lstPartWork[5], pw.chip.lstPartWork[5].pcm);
            }
            else if (pw.chip is HuC6280)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なPCMモード指定'm'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 1);
                pw.pcm = (n == 1);
                pw.freq = -1;//freqをリセット
                pw.instrument = -1;
            }
            else
            {
                pw.getNum(out n);
                msgBox.setWrnMsg("このパートではmコマンドは無視されます。", pw.getSrcFn(), pw.getLineNumber());
            }

        }

        private void CmdDetune(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正なディチューン'D'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 0;
            }

            if (pw.Type == enmChannelType.FMOPM
                || pw.Type == enmChannelType.FMOPN
                || pw.Type == enmChannelType.FMOPNex
                || pw.Type == enmChannelType.Multi
                || pw.Type == enmChannelType.DCSG
                || pw.Type == enmChannelType.DCSGNOISE
                || pw.Type == enmChannelType.SSG
                )
            {
                n = Common.CheckRange(n, -127, 127);
            }

            pw.detune = n;
        }

        private void CmdPan(partWork pw)
        {
            int n;
            int vch = pw.ch;

            pw.incPos();
            if (pw.chip is YM2151)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 3;
                }
                n = Common.CheckRange(n, 1, 3);
                pw.pan.val = (n == 1) ? 2 : (n == 2 ? 1 : n);
                if (pw.instrument < 0)
                {
                    msgBox.setErrMsg("音色指定前にパンは指定できません。", pw.getSrcFn(), pw.getLineNumber());
                }
                else
                {
                    ((YM2151)pw.chip).OutSetPanFeedbackAlgorithm(pw, (int)pw.pan.val, instFM[pw.instrument][46], instFM[pw.instrument][45]);
                }
            }
            else if (pw.chip is YM2203)
            {
                msgBox.setErrMsg("YM2203はパン'p'に未対応です。", pw.getSrcFn(), pw.getLineNumber());
            }
            else if (pw.chip is YM2608)
            {
                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                {
                    //効果音モードのチャンネル番号を指定している場合は3chへ変更する
                    if (pw.ch >= 6 && pw.ch <= 8)
                    {
                        vch = 2;
                    }

                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 3;
                    }
                    n = Common.CheckRange(n, 1, 3);
                    pw.pan.val = n;
                    OutOPNSetPanAMSPMS(pw, n, pw.ams, pw.fms);
                }
                else if (pw.Type == enmChannelType.RHYTHM)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 3;
                    }
                    n = Common.CheckRange(n, 0, 3);
                    pw.pan.val = n;
                }
                else if (pw.Type == enmChannelType.ADPCM)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 3;
                    }
                    n = Common.CheckRange(n, 0, 3);
                    ((YM2608)pw.chip).SetAdpcmPan(pw, n);
                }
            }
            else if (pw.chip is YM2610B)
            {
                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                {
                    //効果音モードのチャンネル番号を指定している場合は3chへ変更する
                    if (pw.ch >= 6 && pw.ch <= 8)
                    {
                        vch = 2;
                    }

                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 3;
                    }
                    n = Common.CheckRange(n, 1, 3);
                    pw.pan.val = n;
                    OutOPNSetPanAMSPMS(pw, n, pw.ams, pw.fms);
                }
                else if (pw.Type == enmChannelType.ADPCMA)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 3;
                    }
                    n = Common.CheckRange(n, 0, 3);
                    pw.pan.val = n;
                }
                else if (pw.Type == enmChannelType.ADPCMB)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 3;
                    }
                    n = Common.CheckRange(n, 0, 3);
                    ((YM2610B)pw.chip).SetAdpcmBPan(pw, n);
                }
            }
            else if (pw.chip is YM2612)
            {
                //効果音モードのチャンネル番号を指定している場合は3chへ変更する
                if (pw.ch >= 6 && pw.ch <= 8)
                {
                    vch = 2;
                }

                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 10;
                }
                //強制的にモノラルにする
                if (monoPart != null && monoPart.Contains(ym2612[0].Ch[5].Name))
                {
                    n = 3;
                }
                n = Common.CheckRange(n, 1, 3);
                pw.pan.val = n;
                OutOPNSetPanAMSPMS(pw, n, pw.ams, pw.fms);
            }
            else if (pw.chip is YM2612X)
            {
                //効果音モードのチャンネル番号を指定している場合は3chへ変更する
                if (pw.ch >= 6 && pw.ch <= 8)
                {
                    vch = 2;
                }

                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 10;
                }
                //強制的にモノラルにする
                if (monoPart != null && monoPart.Contains(ym2612x[0].Ch[5].Name))
                {
                    n = 3;
                }
                n = Common.CheckRange(n, 1, 3);
                pw.pan.val = n;
                OutOPNSetPanAMSPMS(pw, n, pw.ams, pw.fms);
            }
            else if (pw.chip is SN76489)
            {
                pw.getNum(out n);
                msgBox.setWrnMsg("PSGパートでは、pコマンドは無視されます。", pw.getSrcFn(), pw.getLineNumber());
            }
            else if (pw.chip is RF5C164)
            {
                int r;
                if (!pw.getNum(out int l))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 15;
                }
                if (pw.getChar() != ',')
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 15;
                    r = 15;
                }
                pw.incPos();
                if (!pw.getNum(out r))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    r = 15;
                }
                l = Common.CheckRange(l, 0, 15);
                r = Common.CheckRange(r, 0, 15);
                pw.pan.val = (r << 4) | l;
                SetRf5c164CurrentChannel(pw);
                SetRf5c164Pan(pw, (int)pw.pan.val);
            }
            else if (pw.chip is segaPcm)
            {
                int r;
                if (!pw.getNum(out int l))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 127;
                }
                if (pw.getChar() != ',')
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 127;
                    r = 127;
                }
                pw.incPos();
                if (!pw.getNum(out r))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    r = 127;
                }
                l = Common.CheckRange(l, 0, 127);
                r = Common.CheckRange(r, 0, 127);
                pw.panL = l;
                pw.panR = r;
            }
            else if (pw.chip is HuC6280)
            {
                int r;
                if (!pw.getNum(out int l))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 15;
                }
                if (pw.getChar() != ',')
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 15;
                    r = 15;
                }
                pw.incPos();
                if (!pw.getNum(out r))
                {
                    msgBox.setErrMsg("不正なパン'p'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    r = 15;
                }
                l = Common.CheckRange(l, 0, 15);
                r = Common.CheckRange(r, 0, 15);
                pw.pan.val = (l << 4) | r;
                SetHuC6280CurrentChannel(pw);
                SetHuC6280Pan(pw, (int)pw.pan.val);
            }

        }

        private void CmdLength(partWork pw)
        {
            pw.incPos();
            if (!pw.getNumNoteLength(out int n,out bool flg))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 10;
            }
            if (!flg)
            {
                n = Common.CheckRange(n, 1, 128);
                pw.length = clockCount / n;
            }
            else
            {
                n = Common.CheckRange(n, 1, 65535);
                pw.length = n;
            }
        }

        private void CmdClockLength(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正な音長が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 10;
            }
            n = Common.CheckRange(n, 1, 65535);
            pw.length = n;
        }

        private void CmdVolumeDown(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正な音量'('が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 10;
            }
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            pw.volume -= n;
            pw.volume = Common.CheckRange(pw.volume, 0, pw.MaxVolume);

        }

        private void CmdVolumeUp(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正な音量')'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 10;
            }
            n = Common.CheckRange(n, 1, pw.MaxVolume);
            pw.volume += n;
            n = Common.CheckRange(n, 0, pw.MaxVolume);

        }

        private void CmdOctaveDown(partWork pw)
        {
            pw.incPos();
            pw.octaveNew += octaveRev ? 1 : -1;
            pw.octaveNew = Common.CheckRange(pw.octaveNew, 1, 8);
        }

        private void CmdOctaveUp(partWork pw)
        {
            pw.incPos();
            pw.octaveNew += octaveRev ? -1 : 1;
            pw.octaveNew = Common.CheckRange(pw.octaveNew, 1, 8);
        }

        private void CmdOctave(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正なオクターブが指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 110;
            }
            n = Common.CheckRange(n, 1, 8);
            pw.octaveNew = n;
        }

        private void CmdTempo(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正なテンポが指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 120;
            }
            n = Common.CheckRange(n, 1, 1200);
            tempo = n;
            if (format == enmFormat.VGM)
            {
                samplesPerClock = vgmSamplesPerSecond * 60.0 * 4.0 / (tempo * clockCount);
            }
            else
            {
                samplesPerClock = xgmSamplesPerSecond * 60.0 * 4.0 / (tempo * clockCount);
            }
        }

        private void CmdVolume(partWork pw)
        {
            pw.incPos();
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正な音量が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = (int)(pw.MaxVolume * 0.9);
            }

            pw.volume = Common.CheckRange(n, 0, pw.MaxVolume);
        }

        private void CmdTotalVolume(partWork pw)
        {
            pw.incPos();
            if (!(pw.chip is HuC6280))
            {
                if (!pw.getNum(out int n))
                {
                    msgBox.setErrMsg("不正な音量が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 63;
                }
                if ((pw.chip is YM2608) && pw.Type == enmChannelType.RHYTHM)
                {
                    ((YM2608)pw.chip).rhythm_TotalVolume = Common.CheckRange(n, 0, ((YM2608)pw.chip).rhythm_MAXTotalVolume);
                }
                else if ((pw.chip is YM2610B) && pw.Type == enmChannelType.ADPCMA)
                {
                    ((YM2610B)pw.chip).adpcmA_TotalVolume = Common.CheckRange(n, 0, ((YM2610B)pw.chip).adpcmA_MAXTotalVolume);
                }
            }
            else 
            {
                int r;
                if (!pw.getNum(out int l))
                {
                    msgBox.setErrMsg("不正なトータルボリューム'V'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 15;
                }
                if (pw.getChar() != ',')
                {
                    msgBox.setErrMsg("不正なトータルボリューム'V'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    l = 15;
                    r = 15;
                }
                pw.incPos();
                if (!pw.getNum(out r))
                {
                    msgBox.setErrMsg("不正なトータルボリューム'V'が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    r = 15;
                }
                l = Common.CheckRange(l, 0, ((HuC6280)pw.chip).MAXTotalVolume);
                r = Common.CheckRange(r, 0, ((HuC6280)pw.chip).MAXTotalVolume);
                ((HuC6280)pw.chip).TotalVolume = (r << 4) | l;

                OutHuC6280Port(pw.isSecondary, 1, (byte)((HuC6280)pw.chip).TotalVolume);
            }

        }

        private void CmdInstrument(partWork pw)
        {
            int n;
            pw.incPos();

            if (pw.getChar() == 'T')
            {
                if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
                {
                    msgBox.setErrMsg("Tone DoublerはFM音源以外では使用できません。", pw.getSrcFn(), pw.getLineNumber());
                    pw.incPos();
                    return;
                }
                pw.incPos();
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 255);
                pw.toneDoubler = n;
                return;
            }

            if (pw.chip is YM2151)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 255);
                if (pw.instrument != n)
                {
                    pw.instrument = n;
                    ((YM2151)pw.chip).OutSetInstrument(pw, n, pw.volume);
                }
            }
            else if (pw.chip is YM2203)
            {
                if (pw.ch < 6)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (pw.instrument != n)
                    {
                        pw.instrument = n;
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            ym2203[pw.chip.ChipID].lstPartWork[2].instrument = n;
                            ym2203[pw.chip.ChipID].lstPartWork[3].instrument = n;
                            ym2203[pw.chip.ChipID].lstPartWork[4].instrument = n;
                            ym2203[pw.chip.ChipID].lstPartWork[5].instrument = n;
                        }
                        OutFmSetInstrument(pw, n, pw.volume);
                    }
                }
                else if (pw.Type == enmChannelType.SSG)
                {
                    n = SetEnvelopParamFromInstrument(pw);
                }
            }
            else if (pw.chip is YM2608)
            {
                if (pw.ch < 9)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (pw.instrument != n)
                    {
                        pw.instrument = n;
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            ym2608[pw.chip.ChipID].lstPartWork[2].instrument = n;
                            ym2608[pw.chip.ChipID].lstPartWork[6].instrument = n;
                            ym2608[pw.chip.ChipID].lstPartWork[7].instrument = n;
                            ym2608[pw.chip.ChipID].lstPartWork[8].instrument = n;
                        }
                        OutFmSetInstrument(pw, n, pw.volume);
                    }
                }
                else if (pw.Type == enmChannelType.SSG)
                {
                    n = SetEnvelopParamFromInstrument(pw);
                }
                else if (pw.Type == enmChannelType.RHYTHM)
                {
                    if (pw.getChar() != 'E')
                    {
                        n = SetEnvelopParamFromInstrument(pw);
                    }
                    else
                    {
                        pw.incPos();
                        n = SetEnvelopParamFromInstrument(pw);
                    }
                }
                else if (pw.Type == enmChannelType.ADPCM)
                {
                    if (pw.getChar() != 'E')
                    {
                        if (!pw.getNum(out n))
                        {
                            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                            n = 0;
                        }
                        n = Common.CheckRange(n, 0, 255);
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2608 || instPCM[n].loopAdr != 1)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2608向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                            pw.instrument = n;
                            ym2608[pw.chip.ChipID].SetADPCMAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
                        }
                    }
                    else
                    {
                        pw.incPos();
                        n = SetEnvelopParamFromInstrument(pw);
                    }
                }
            }
            else if (pw.chip is YM2610B)
            {
                if (pw.ch < 9)
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (pw.instrument != n)
                    {
                        pw.instrument = n;
                        if (pw.Type == enmChannelType.FMOPNex)
                        {
                            ym2610b[pw.chip.ChipID].lstPartWork[2].instrument = n;
                            ym2610b[pw.chip.ChipID].lstPartWork[6].instrument = n;
                            ym2610b[pw.chip.ChipID].lstPartWork[7].instrument = n;
                            ym2610b[pw.chip.ChipID].lstPartWork[8].instrument = n;
                        }
                        if (!pw.pcm)
                        {
                            OutFmSetInstrument(pw, n, pw.volume);
                        }
                        else
                        {
                            if (!instPCM.ContainsKey(n))
                            {
                                msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                            else
                            {
                                if (instPCM[n].chip != enmChipType.YM2610B)
                                {
                                    msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                                }
                            }
                        }
                    }
                }
                else if (pw.Type == enmChannelType.SSG)
                {
                    n = SetEnvelopParamFromInstrument(pw);
                }
                else if (pw.Type == enmChannelType.ADPCMA)
                {
                    if (pw.getChar() != 'E')
                    {
                        if (!pw.getNum(out n))
                        {
                            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                            n = 0;
                        }
                        n = Common.CheckRange(n, 0, 255);
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2610B || instPCM[n].loopAdr != 0)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                            pw.instrument = n;
                            ym2610b[pw.chip.ChipID].SetADPCMAAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
                        }
                    }
                    else
                    {
                        pw.incPos();
                        n = SetEnvelopParamFromInstrument(pw);
                    }
                }
                else if (pw.Type == enmChannelType.ADPCMB)
                {
                    if (pw.getChar() != 'E')
                    {
                        if (!pw.getNum(out n))
                        {
                            msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                            n = 0;
                        }
                        n = Common.CheckRange(n, 0, 255);
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2610B || instPCM[n].loopAdr != 1)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2610B向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                            pw.instrument = n;
                            ym2610b[pw.chip.ChipID].SetADPCMBAddress(pw, (int)instPCM[n].stAdr, (int)instPCM[n].edAdr);
                        }
                    }
                    else
                    {
                        pw.incPos();
                        n = SetEnvelopParamFromInstrument(pw);
                    }
                }
            }
            else if (pw.chip is YM2612)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 255);
                if (pw.instrument != n)
                {
                    pw.instrument = n;
                    if (pw.Type == enmChannelType.FMOPNex)
                    {
                        ym2612[pw.chip.ChipID].lstPartWork[2].instrument = n;
                        ym2612[pw.chip.ChipID].lstPartWork[6].instrument = n;
                        ym2612[pw.chip.ChipID].lstPartWork[7].instrument = n;
                        ym2612[pw.chip.ChipID].lstPartWork[8].instrument = n;
                    }
                    if (!pw.pcm)
                    {
                        OutFmSetInstrument(pw, n, pw.volume);
                    }
                    else
                    {
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2612)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2612向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                        }
                    }
                }
            }
            else if (pw.chip is YM2612X)
            {
                if (!pw.getNum(out n))
                {
                    msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                    n = 0;
                }
                n = Common.CheckRange(n, 0, 255);
                if (pw.instrument != n)
                {
                    pw.instrument = n;
                    if (pw.Type == enmChannelType.FMOPNex)
                    {
                        ym2612x[pw.chip.ChipID].lstPartWork[2].instrument = n;
                        ym2612x[pw.chip.ChipID].lstPartWork[6].instrument = n;
                        ym2612x[pw.chip.ChipID].lstPartWork[7].instrument = n;
                        ym2612x[pw.chip.ChipID].lstPartWork[8].instrument = n;
                    }
                    if (!pw.pcm)
                    {
                        OutFmSetInstrument(pw, n, pw.volume);
                    }
                    else
                    {
                        if (!instPCM.ContainsKey(n))
                        {
                            msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        else
                        {
                            if (instPCM[n].chip != enmChipType.YM2612X)
                            {
                                msgBox.setErrMsg(string.Format("指定された音色番号({0})はYM2612(XGM)向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                        }
                    }
                }
            }
            else if (pw.chip is SN76489)
            {
                //pw.incPos();
                n = SetEnvelopParamFromInstrument(pw);
            }
            else if (pw.chip is RF5C164)
            {
                if (pw.getChar() != 'E')
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (!instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                    }
                    else
                    {
                        if (instPCM[n].chip != enmChipType.RF5C164)
                        {
                            msgBox.setErrMsg(string.Format("指定された音色番号({0})はRF5C164向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        pw.instrument = n;
                        SetRf5c164CurrentChannel(pw);
                        SetRf5c164SampleStartAddress(pw, (int)instPCM[n].stAdr);
                        SetRf5c164LoopAddress(pw, (int)(instPCM[n].loopAdr));
                    }
                }
                else
                {
                    pw.incPos();
                    n = SetEnvelopParamFromInstrument(pw);
                }
            }
            else if (pw.chip is segaPcm)
            {
                if (pw.getChar() != 'E')
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (!instPCM.ContainsKey(n))
                    {
                        msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                    }
                    else
                    {
                        if (instPCM[n].chip != enmChipType.SEGAPCM)
                        {
                            msgBox.setErrMsg(string.Format("指定された音色番号({0})はSEGAPCM向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                        }
                        pw.instrument = n;
                        pw.pcmStartAddress = (int)instPCM[n].stAdr;
                        pw.pcmEndAddress = (int)instPCM[n].edAdr;
                        pw.pcmLoopAddress = instPCM[n].loopAdr == 0 ? -1 : (int)instPCM[n].loopAdr;
                        pw.pcmBank = (int)((instPCM[n].stAdr >> 16) << 1);
                    }
                }
                else
                {
                    pw.incPos();
                    n = SetEnvelopParamFromInstrument(pw);
                }
            }
            else if (pw.chip is HuC6280)
            {
                if (pw.getChar() != 'E')
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (pw.instrument != n)
                    {
                        pw.instrument = n;
                        if (!pw.pcm)
                        {
                            OutHuC6280SetInstrument(pw, n);
                        }
                        else
                        {
                            if (!instPCM.ContainsKey(n))
                            {
                                msgBox.setErrMsg(string.Format("PCM定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
                            }
                            else
                            {
                                if (instPCM[n].chip != enmChipType.HuC6280)
                                {
                                    msgBox.setErrMsg(string.Format("指定された音色番号({0})はHuC6280向けPCMデータではありません。", n), pw.getSrcFn(), pw.getLineNumber());
                                }
                            }
                        }
                    }
                }
                else
                {
                    pw.incPos();
                    n = SetEnvelopParamFromInstrument(pw);
                }
            }
            else if (pw.chip is YM2413)
            {
                if (pw.getChar() == 'E')
                {
                    pw.incPos();
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色(INST)番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 15);
                    if (pw.envInstrument != n)
                    {
                        ((YM2413)pw.chip).outYM2413SetInstVol(pw, n, pw.volume); //INSTを0にセット
                    }
                }
                else
                {
                    if (!pw.getNum(out n))
                    {
                        msgBox.setErrMsg("不正な音色番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                        n = 0;
                    }
                    n = Common.CheckRange(n, 0, 255);
                    if (pw.instrument != n)
                    {
                        ((YM2413)pw.chip).outYM2413SetInstrument(pw, n); //音色のセット
                        ((YM2413)pw.chip).outYM2413SetInstVol(pw, 0, pw.volume); //INSTを0にセット
                        OutData(pw.port0, 0x20, 0x19);
                    }
                }
            }

        }

        private int SetEnvelopParamFromInstrument(partWork pw)
        {
            if (!pw.getNum(out int n))
            {
                msgBox.setErrMsg("不正なエンベロープ番号が指定されています。", pw.getSrcFn(), pw.getLineNumber());
                n = 0;
            }
            n = Common.CheckRange(n, 0, 255);
            if (!instENV.ContainsKey(n))
            {
                msgBox.setErrMsg(string.Format("エンベロープ定義に指定された音色番号({0})が存在しません。", n), pw.getSrcFn(), pw.getLineNumber());
            }
            else
            {
                if (pw.envInstrument != n)
                {
                    pw.envInstrument = n;
                    pw.envIndex = -1;
                    pw.envCounter = -1;
                    for (int i = 0; i < instENV[n].Length; i++)
                    {
                        pw.envelope[i] = instENV[n][i];
                    }
                }
            }

            return n;
        }

        private void CmdLfo(partWork pw)
        {

            pw.incPos();
            char c = pw.getChar();

            if (c == 'A')
            {
                pw.incPos();
                char d = pw.getChar();
                if (d == 'M')
                {
                    pw.incPos();
                    d = pw.getChar();
                    if (d == 'S')
                    {
                        //MAMS
                        CmdMAMS(pw);
                        return;
                    }
                }
                msgBox.setErrMsg("MAMSの文法が違います。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            if (c < 'P' && c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
            c -= 'P';

            pw.incPos();
            char t = pw.getChar();

            if (c == 0 && t == 'M')
            {
                pw.incPos();
                char d = pw.getChar();
                if (d == 'S')
                {
                    //MPMS
                    CmdMPMS(pw);
                    return;
                }
                msgBox.setErrMsg("MPMSの文法が違います。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            if (t != 'T' && t != 'V' && t != 'H')
            {
                msgBox.setErrMsg("指定できるLFOの種類はT,V,Hの3種類です。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
            pw.lfo[c].type = (t == 'T') ? eLfoType.Tremolo : ((t == 'V') ? eLfoType.Vibrato : eLfoType.Hardware);

            pw.lfo[c].sw = false;
            pw.lfo[c].isEnd = true;

            pw.lfo[c].param = new List<int>();
            int n = -1;
            do
            {
                pw.incPos();
                if (pw.getNum(out n))
                {
                    pw.lfo[c].param.Add(n);
                }
                else
                {
                    msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", pw.getSrcFn(), pw.getLineNumber());
                    return;
                }

                while (pw.getChar() == '\t' || pw.getChar() == ' ') { pw.incPos(); }

            } while (pw.getChar() == ',');
            if (pw.lfo[c].type == eLfoType.Tremolo || pw.lfo[c].type == eLfoType.Vibrato)
            {
                if (pw.lfo[c].param.Count < 4)
                {
                    msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", pw.getSrcFn(), pw.getLineNumber());
                    return;
                }
                if (pw.lfo[c].param.Count > 7)
                {
                    msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", pw.getSrcFn(), pw.getLineNumber());
                    return;
                }

                pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, (int)clockCount);
                pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 1, 255);
                pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], -32768, 32787);
                pw.lfo[c].param[3] = Math.Abs(Common.CheckRange(pw.lfo[c].param[3], 0, 32787));
                if (pw.lfo[c].param.Count > 4)
                {
                    pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 4);
                }
                else
                {
                    pw.lfo[c].param.Add(0);
                }
                if (pw.lfo[c].param.Count > 5)
                {
                    pw.lfo[c].param[5] = Common.CheckRange(pw.lfo[c].param[5], 0, 1);
                }
                else
                {
                    pw.lfo[c].param.Add(1);
                }
                if (pw.lfo[c].param.Count > 6)
                {
                    pw.lfo[c].param[6] = Common.CheckRange(pw.lfo[c].param[6], -32768, 32787);
                    if (pw.lfo[c].param[6] == 0) pw.lfo[c].param[6] = 1;
                }
                else
                {
                    pw.lfo[c].param.Add(1);
                }

            }
            else
            {
                //Hard LFO
                if (pw.Type == enmChannelType.FMOPM)
                {
                    if (pw.lfo[c].param.Count < 4)
                    {
                        msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }
                    if (pw.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }

                    pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, 3); //Type
                    pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 255); //LFRQ
                    pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 127); //PMD
                    pw.lfo[c].param[3] = Common.CheckRange(pw.lfo[c].param[3], 0, 127); //AMD
                    if (pw.lfo[c].param.Count == 5)
                    {
                        pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 1);
                    }
                    else
                    {
                        pw.lfo[c].param.Add(0);
                    }
                }
                else if (!(pw.chip is YM2203) && pw.Type == enmChannelType.FMOPN)
                {
                    if (pw.lfo[c].param.Count < 4)
                    {
                        msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }
                    if (pw.lfo[c].param.Count > 5)
                    {
                        msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }

                    pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, (int)clockCount);//Delay(無視)
                    pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 7);//Freq
                    pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 7);//PMS
                    pw.lfo[c].param[3] = Common.CheckRange(pw.lfo[c].param[3], 0, 3);//AMS
                    if (pw.lfo[c].param.Count == 5)
                    {
                        pw.lfo[c].param[4] = Common.CheckRange(pw.lfo[c].param[4], 0, 1); //Switch
                    }
                    else
                    {
                        pw.lfo[c].param.Add(1);
                    }
                }
                else if (pw.chip is HuC6280)
                {
                    if (pw.lfo[c].param.Count < 3)
                    {
                        msgBox.setErrMsg("LFOの設定に必要なパラメータが足りません。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }
                    if (pw.lfo[c].param.Count > 3)
                    {
                        msgBox.setErrMsg("LFOの設定に可能なパラメータ数を超えて指定されました。", pw.getSrcFn(), pw.getLineNumber());
                        return;
                    }

                    pw.lfo[c].param[0] = Common.CheckRange(pw.lfo[c].param[0], 0, 3);//Control(n= 0(Disable),1-3(Ch2波形加算))
                    pw.lfo[c].param[1] = Common.CheckRange(pw.lfo[c].param[1], 0, 255);//Freq(n= 0-255)
                    pw.lfo[c].param[2] = Common.CheckRange(pw.lfo[c].param[2], 0, 4095);//Ch2Freq(n= 0-4095)

                }

            }
            //解析　ここまで

            if (pw.lfo[c].type != eLfoType.Hardware)
            {
                pw.lfo[c].sw = true;
                pw.lfo[c].isEnd = false;
                pw.lfo[c].value = (pw.lfo[c].param[0] == 0) ? pw.lfo[c].param[6] : 0;//ディレイ中は振幅補正は適用されない
                pw.lfo[c].waitCounter = pw.lfo[c].param[0];
                pw.lfo[c].direction = pw.lfo[c].param[2] < 0 ? -1 : 1;
            }
            else
            {
                pw.lfo[c].sw = true;
                pw.lfo[c].isEnd = false;
                pw.lfo[c].value = 0;
                pw.lfo[c].waitCounter = -1;
                pw.lfo[c].direction = 0;
            }
        }

        private void CmdLfoSwitch(partWork pw)
        {

            pw.incPos();
            char c = pw.getChar();
            if (c < 'P' || c > 'S')
            {
                msgBox.setErrMsg("指定できるLFOのチャネルはP,Q,R,Sの4種類です。", pw.getSrcFn(), pw.getLineNumber());
                pw.incPos();
                return;
            }
            c -= 'P';

            int n = -1;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("LFOの設定値に不正な値が指定されました。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
            n = Common.CheckRange(n, 0, 2);

            //解析　ここまで

            //LFOの設定値をチェック
            if (n != 0 && !CheckLFOParam(pw, (int)c))
            {
                return;
            }

            pw.lfo[c].sw = (n == 0) ? false : true;

            //即時有効になるタイプのHardLFOの処理
            if (pw.lfo[c].type == eLfoType.Hardware && pw.lfo[c].param != null)
            {
                if (pw.chip is YM2151)
                {
                    ((YM2151)pw.chip).OutSetHardLfo(pw, (n == 0) ? false : true, pw.lfo[c].param);
                }
                else if (pw.chip is YM2612 || pw.chip is YM2612X || pw.chip is YM2608 || pw.chip is YM2610B)
                {
                    if (pw.lfo[c].param[4] == 0)
                    {
                        OutOPNSetHardLfo(pw, (n == 0) ? false : true, pw.lfo[c].param[1]);
                    }
                    else
                    {
                        OutOPNSetHardLfo(pw, false, pw.lfo[c].param[1]);
                    }
                }
                else if (pw.chip is HuC6280)
                {
                    if (n == 0)
                    {
                        OutHuC6280Port(pw.isSecondary, 9, 0); //disable
                    }
                    else
                    {
                        OutHuC6280Port(pw.isSecondary, 9, (byte)pw.lfo[c].param[0]);
                        OutHuC6280Port(pw.isSecondary, 8, (byte)pw.lfo[c].param[1]);
                        OutHuC6280Port(pw.isSecondary, 0, 1);//CurrentChannel 2
                        ((HuC6280)pw.chip).CurrentChannel = 1;
                        OutHuC6280Port(pw.isSecondary, 2, (byte)(pw.lfo[c].param[2] & 0xff));
                        OutHuC6280Port(pw.isSecondary, 3, (byte)((pw.lfo[c].param[2] & 0xf00) >> 8));
                        ((HuC6280)pw.chip).lstPartWork[1].freq = pw.lfo[c].param[2];
                    }
                }
            }

        }

        private bool CheckLFOParam(partWork pw, int c)
        {
            if (pw.lfo[c].param == null)
            {
                msgBox.setErrMsg("指定されたLFOのパラメータが未指定です。", pw.getSrcFn(), pw.getLineNumber());
                return false;
            }

            return false;
        }

        private void CmdMAMS(partWork pw)
        {
            if (!( (pw.chip is YM2151) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612) || (pw.chip is YM2612X)))
            {
                msgBox.setWrnMsg("このチャンネルのMAMSは無視されます。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            if (!((pw.Type== enmChannelType.FMOPM) || (pw.Type== enmChannelType.FMOPN)))
            {
                msgBox.setWrnMsg("FMチャンネル以外のMAMSは無視されます。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            int n = -1;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("MAMSの設定値に不正な値が指定されました。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
            if (pw.Type == enmChannelType.FMOPM)
            {
                n = Common.CheckRange(n, 0, 3);
                ((YM2151)pw.chip).OutSetPMSAMS(pw, pw.pms, n);
            }
            else
            {
                n = Common.CheckRange(n, 0, 7);
                OutOPNSetPanAMSPMS(pw, (int)pw.pan.val, n, pw.pms);
            }
            pw.ams = n;
        }

        private void CmdMPMS(partWork pw)
        {
            if (!((pw.chip is YM2151) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612) || (pw.chip is YM2612X)))
            {
                msgBox.setWrnMsg("このチャンネルのMPMSは無視されます。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            if (!((pw.Type == enmChannelType.FMOPM) || (pw.Type == enmChannelType.FMOPN)))
            {
                msgBox.setWrnMsg("FMチャンネル以外のMPMSは無視されます。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            int n = -1;
            pw.incPos();
            if (!pw.getNum(out n))
            {
                msgBox.setErrMsg("MPMSの設定値に不正な値が指定されました。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
            if (pw.Type == enmChannelType.FMOPM)
            {
                n = Common.CheckRange(n, 0, 7);
                ((YM2151)pw.chip).OutSetPMSAMS(pw, n, pw.ams);
            }
            else
            {
                n = Common.CheckRange(n, 0, 3);
                OutOPNSetPanAMSPMS(pw, (int)pw.pan.val, pw.ams, n);
            }
            pw.pms = n;
        }

        private void CmdY(partWork pw)
        {
            int n = -1;
            byte adr = 0;
            byte dat = 0;
            byte op = 0;
            pw.incPos();

            char c = pw.getChar();
            if (c >= 'A' && c <= 'Z')
            {
                string toneparamName = "" + c;
                pw.incPos();
                toneparamName += pw.getChar();
                pw.incPos();
                if (toneparamName != "TL" && toneparamName != "SR")
                {
                    toneparamName += pw.getChar();
                    pw.incPos();
                    if (toneparamName != "SSG")
                    {
                        toneparamName += pw.getChar();
                        pw.incPos();
                    }
                }

                if (toneparamName == "DT1M" || toneparamName == "DT2S" || toneparamName == "PMSA")
                {
                    toneparamName += pw.getChar();
                    pw.incPos();
                    if (toneparamName == "PMSAM")
                    {
                        toneparamName += pw.getChar();
                        pw.incPos();
                    }
                }

                pw.incPos();

                if (toneparamName != "FBAL" && toneparamName != "PMSAMS")
                {
                    if (pw.getNum(out n))
                    {
                        op = (byte)(Common.CheckRange(n & 0xff, 1, 4) - 1);
                    }
                    pw.incPos();
                }

                if (pw.getNum(out n))
                {
                    dat = (byte)(n & 0xff);
                }

                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex || (pw.chip is YM2612 && pw.ch == 5) || (pw.chip is YM2612X && pw.ch == 5))
                {
                    switch (toneparamName)
                    {
                        case "DTML":
                            CmdY_ToneParamOPN(0x30, pw, op, dat);
                            break;
                        case "TL":
                            CmdY_ToneParamOPN(0x40, pw, op, dat);
                            break;
                        case "KSAR":
                            CmdY_ToneParamOPN(0x50, pw, op, dat);
                            break;
                        case "AMDR":
                            CmdY_ToneParamOPN(0x60, pw, op, dat);
                            break;
                        case "SR":
                            CmdY_ToneParamOPN(0x70, pw, op, dat);
                            break;
                        case "SLRR":
                            CmdY_ToneParamOPN(0x80, pw, op, dat);
                            break;
                        case "SSG":
                            CmdY_ToneParamOPN(0x90, pw, op, dat);
                            break;
                        case "FBAL":
                            CmdY_ToneParamOPN_FBAL(pw, dat);
                            break;
                    }
                }
                else if (pw.Type == enmChannelType.FMOPM)
                {
                    switch (toneparamName)
                    {
                        case "FBAL":
                            CmdY_ToneParamOPM_FBAL(pw, dat);
                            break;
                        case "PMSAMS":
                            CmdY_ToneParamOPM_PMSAMS(pw, dat);
                            break;
                        case "DTML":
                        case "DT1ML":
                            CmdY_ToneParamOPM(0x40, pw, op, dat);
                            break;
                        case "TL":
                            CmdY_ToneParamOPM(0x60, pw, op, dat);
                            break;
                        case "KSAR":
                            CmdY_ToneParamOPM(0x80, pw, op, dat);
                            break;
                        case "AMDR":
                            CmdY_ToneParamOPM(0xa0, pw, op, dat);
                            break;
                        case "DT2SR":
                            CmdY_ToneParamOPM(0xc0, pw, op, dat);
                            break;
                        case "SLRR":
                            CmdY_ToneParamOPM(0xe0, pw, op, dat);
                            break;
                    }
                }

                return;
            }

            if (pw.getNum(out n))
            {
                adr = (byte)(n & 0xff);
            }
            pw.incPos();
            if (pw.getNum(out n))
            {
                dat = (byte)(n & 0xff);
            }

            if ((pw.chip is YM2151) || (pw.chip is YM2203))
            {
                OutData(pw.port0, adr, dat);
            }
            else if (pw.chip is YM2608)
            {
                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                    OutData((pw.ch > 2 && pw.ch < 6) ? pw.port1 : pw.port0, adr, dat);
                else if (pw.Type == enmChannelType.SSG)
                    OutData(pw.port0, adr, dat);
                else if (pw.Type == enmChannelType.RHYTHM)
                    OutData(pw.port0, adr, dat);
                else if (pw.Type == enmChannelType.ADPCM)
                    OutData(pw.port1, adr, dat);
            }
            else if (pw.chip is YM2610B)
            {
                if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                    OutData((pw.ch > 2 && pw.ch < 6) ? pw.port1 : pw.port0, adr, dat);
                else if (pw.Type == enmChannelType.SSG)
                    OutData(pw.port0, adr, dat);
                else if (pw.Type == enmChannelType.ADPCMA)
                    OutData(pw.port1, adr, dat);
                else if (pw.Type == enmChannelType.ADPCMB)
                    OutData(pw.port0, adr, dat);
            }
            else if (pw.chip is YM2612)
            {
                OutData((pw.ch > 2 && pw.ch < 6) ? pw.port1 : pw.port0, adr, dat);
            }
            else if (pw.chip is YM2612X)
            {
                OutData((pw.ch > 2 && pw.ch < 6) ? pw.port1 : pw.port0, adr, dat);
            }
            else if (pw.chip is SN76489)
            {
                OutPsgPort(pw.isSecondary, dat);
            }
            else if (pw.chip is RF5C164)
            {
                OutRf5c164Port(pw.isSecondary, adr, dat);
            }
            else if (pw.chip is segaPcm)
            {
                OutSegaPcmPort(pw, adr, dat);
            }
            else if (pw.chip is HuC6280)
            {
                OutHuC6280Port(pw.isSecondary, adr, dat);
            }
        }

        private void CmdY_ToneParamOPN(byte adr, partWork pw, byte op, byte dat)
        {
            int ch = pw.Type == enmChannelType.FMOPNex ? 2 : pw.ch;
            byte port = (ch > 2 ? pw.port1 : pw.port0);
            int vch = ch;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            op = (byte)(op == 1 ? 2 : (op == 2 ? 1 : op));

            adr += (byte)(vch + (op << 2));

            OutData(port, adr, dat);
        }

        private void CmdY_ToneParamOPN_FBAL(partWork pw, byte dat)
        {
            int ch = pw.Type == enmChannelType.FMOPNex ? 2 : pw.ch;
            byte port = (ch > 2 ? pw.port1 : pw.port0);
            int vch = ch;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            byte adr = (byte)(0xb0 + vch);

            OutData(port, adr, dat);
        }

        private void CmdY_ToneParamOPM(byte adr, partWork pw, byte op, byte dat)
        {
            op = (byte)(op == 1 ? 2 : (op == 2 ? 1 : op));

            adr += (byte)(pw.ch + (op << 3));

            OutData(pw.port0, adr, dat);
        }

        private void CmdY_ToneParamOPM_FBAL(partWork pw, byte dat)
        {
            OutData(pw.port0, (byte)(0x20 + pw.ch), (byte)((dat & 0x3f) + (pw.pan.val << 6)));
        }

        private void CmdY_ToneParamOPM_PMSAMS(partWork pw, byte dat)
        {
            OutData(pw.port0, (byte)(0x38 + pw.ch), (byte)(dat & 0x73));
        }

        private void CmdNoise(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.Type == enmChannelType.DCSGNOISE || pw.Type == enmChannelType.SSG)
            {
                if (pw.getNum(out n))
                {
                    n = Common.CheckRange(n, 0, pw.Type == enmChannelType.DCSGNOISE ? 7 : 31);
                    if (pw.Type == enmChannelType.DCSGNOISE)
                    {
                        pw.noise = n; // DCSGの場合は4Chに保存
                    }
                    else
                    {
                        pw.chip.lstPartWork[0].noise = n;//その他SSGの場合は、そのChipの1Chに保存
                        OutSsgNoise(pw, n);
                    }
                }
                else
                {
                    msgBox.setErrMsg("wコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                    return;

                }
            }
            else if (pw.chip is YM2151)
            {
                if (pw.getNum(out n))
                {
                    n = Common.CheckRange(n, 0, 31);
                    if (pw.noise != n)
                    {
                        pw.noise = n;
                    }
                }
                else
                {
                    msgBox.setErrMsg("wコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                    return;

                }
            }
            else if (pw.chip is HuC6280 && pw.ch>3)
            {
                if (pw.getNum(out n))
                {
                    n = Common.CheckRange(n, 0, 31);
                    if (pw.noise != n)
                    {
                        pw.noise = n;
                        SetHuC6280CurrentChannel(pw);
                        OutHuC6280Port(pw.isSecondary, 7, (byte)((pw.mixer != 0 ? 0x80 : 0x00) + (pw.noise & 0x1f)));
                    }
                }
                else
                {
                    msgBox.setErrMsg("wコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                    return;

                }
            }
            else
            {
                msgBox.setErrMsg("このチャンネルではwコマンドは使用できません。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
        }

        private void CmdMixer(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.Type == enmChannelType.SSG && (
                (pw.chip is YM2203)
                || (pw.chip is YM2608)
                || (pw.chip is YM2610B)
                ))
            {
                if (pw.getNum(out n))
                {
                    n = Common.CheckRange(n, 0, 3);
                    pw.mixer = n;
                }
                else
                {
                    msgBox.setErrMsg("Pコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                    return;

                }
            }
            else if (pw.chip is YM2151)
            {
                if (pw.getNum(out n))
                {
                    n = Common.CheckRange(n, 0, 1);
                    if (pw.mixer != n)
                    {
                        pw.mixer = n;
                    }
                }
                else
                {
                    msgBox.setErrMsg("Pコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                    return;

                }
            }
            else if (pw.chip is HuC6280 && pw.ch>3)
            {
                if (pw.getNum(out n))
                {
                    n = Common.CheckRange(n, 0, 1);
                    if (pw.mixer != n)
                    {
                        pw.mixer = n;
                        SetHuC6280CurrentChannel(pw);
                        OutHuC6280Port(pw.isSecondary, 7, (byte)((pw.mixer != 0 ? 0x80 : 0x00) + (pw.noise & 0x1f)));
                    }
                }
                else
                {
                    msgBox.setErrMsg("Pコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                    return;

                }
            }
            else
            {
                msgBox.setErrMsg("このチャンネルではPコマンドは使用できません。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }
        }

        private void CmdKeyShift(partWork pw)
        {
            int n = -1;
            pw.incPos();

            if (pw.getNum(out n))
            {
                pw.keyShift = Common.CheckRange(n, -128, 128);
            }
            else
            {
                msgBox.setErrMsg("Kコマンドに指定された値が不正です。", pw.getSrcFn(), pw.getLineNumber());
                return;

            }
        }

        private void MakeHeader()
        {

            //Header
            OutData(Const.hDat);

            //PCM Data block
            if ((ym2612[0].use || ym2612[1].use) && ym2612[0].pcmData != null && ym2612[0].pcmData.Length > 0)
                OutData(ym2612[0].pcmData);

            for (int i = 0; i < 2; i++)
            {
                //PCM Data block
                if (rf5c164[i].use && rf5c164[i].pcmData != null && rf5c164[i].pcmData.Length > 0)
                    OutData(rf5c164[i].pcmData);

                //ADPCM Data block
                if (ym2608[i].use && ym2608[i].pcmData != null && ym2608[i].pcmData.Length > 0)
                    OutData(ym2608[i].pcmData);

                //ADPCM-A Data block
                if (ym2610b[i].use && ym2610b[i].pcmDataA != null && ym2610b[i].pcmDataA.Length > 0)
                    OutData(ym2610b[i].pcmDataA);

                //ADPCM-B Data block
                if (ym2610b[i].use && ym2610b[i].pcmDataB != null && ym2610b[i].pcmDataB.Length > 0)
                    OutData(ym2610b[i].pcmDataB);

                //ADPCM Data block
                if (segapcm[i].use && segapcm[i].pcmData != null && segapcm[i].pcmData.Length > 0)
                    OutData(segapcm[i].pcmData);

                //PCM Data block
                if (huc6280[i].use && huc6280[i].pcmData != null && huc6280[i].pcmData.Length > 0)
                    OutData(huc6280[i].pcmData);

            }

            for (int i = 0; i < 2; i++)
            {
                ym2612[i].InitChip();

                if (sn76489[i].use)
                {
                    if (i != 0) dat[0x0f] |= 0x40;
                }

                if (rf5c164[i].use)
                {
                    for (int ch = 0; ch < rf5c164[i].ChMax; ch++)
                    {
                        partWork pw = rf5c164[i].lstPartWork[ch];

                        SetRf5c164CurrentChannel(pw);
                        SetRf5c164SampleStartAddress(pw, 0);
                        SetRf5c164LoopAddress(pw, 0);
                        SetRf5c164AddressIncrement(pw, 0x400);
                        SetRf5c164Pan(pw, 0xff);
                        SetRf5c164Envelope(pw, 0xff);
                    }

                    if (i != 0) dat[0x6f] |= 0x40;
                }
                
                if (segapcm[i].use)
                {
                    for (int ch = 0; ch < segapcm[i].ChMax; ch++)
                    {
                        partWork pw = segapcm[i].lstPartWork[ch];
                        pw.MaxVolume = segapcm[i].Ch[ch].MaxVolume;
                        pw.panL = 127;
                        pw.panR = 127;
                        pw.volume = pw.MaxVolume;
                    }

                    if (i != 0) dat[0x3b] |= 0x40;
                }

                if (ym2151[i].use)
                {
                    //initialize shared param

                    //FM Off
                    ym2151[i].OutAllKeyOff();

                    foreach (partWork pw in ym2151[i].lstPartWork)
                    {
                        if (pw.ch == 0)
                        {
                            pw.hardLfoFreq = 0;
                            pw.hardLfoPMD = 0;
                            pw.hardLfoAMD = 0;

                            //Reset Hard LFO
                            ((YM2151)pw.chip).OutSetHardLfoFreq(pw, pw.hardLfoFreq);
                            ((YM2151)pw.chip).OutSetHardLfoDepth(pw, false, pw.hardLfoAMD);
                            ((YM2151)pw.chip).OutSetHardLfoDepth(pw, true, pw.hardLfoPMD);
                        }

                        pw.ams = 0;
                        pw.pms = 0;
                        if (!pw.dataEnd) ((YM2151)pw.chip).OutSetPMSAMS(pw, 0, 0);

                    }

                    if (i != 0) dat[0x33] |= 0x40;//use Secondary
                }

                ym2203[i].InitChip();

                if (ym2608[i].use)
                {
                    //initialize shared param
                    OutOPNSetHardLfo(ym2608[i].lstPartWork[0], false, 0);
                    OutOPNSetCh3SpecialMode(ym2608[i].lstPartWork[0], false);

                    //FM Off
                    OutFmAllKeyOff(ym2608[i]);

                    //SSG Off
                    for (int ch = 9; ch < 12; ch++)
                    {
                        OutSsgKeyOff(ym2608[i].lstPartWork[ch]);
                        ym2608[i].lstPartWork[ch].volume = 0;
                    }

                    //Use OPNA mode
                    OutData(ym2608[i].lstPartWork[0].port0, 0x29, 0x82);
                    OutData(ym2608[i].lstPartWork[0].port1, 0x29, 0x82);
                    //ADPCM Reset
                    OutData(ym2608[i].lstPartWork[0].port1, 0x10, 0x17);
                    OutData(ym2608[i].lstPartWork[0].port1, 0x10, 0x80);
                    OutData(ym2608[i].lstPartWork[0].port1, 0x00, 0x80);
                    OutData(ym2608[i].lstPartWork[0].port1, 0x01, 0xc0);
                    OutData(ym2608[i].lstPartWork[0].port1, 0x00, 0x01);

                    foreach (partWork pw in ym2608[i].lstPartWork)
                    {
                        if (pw.ch == 0)
                        {
                            pw.hardLfoSw = false;
                            pw.hardLfoNum = 0;
                            OutOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                        }

                        if (pw.ch < 6)
                        {
                            pw.pan.val = 3;
                            pw.ams = 0;
                            pw.fms = 0;
                            if (!pw.dataEnd) OutOPNSetPanAMSPMS(pw, 3, 0, 0);
                        }
                    }

                    if (i != 0) dat[0x4b] |= 0x40;//use Secondary
                }

                if (ym2610b[i].use)
                {
                    //initialize shared param
                    OutOPNSetHardLfo(ym2610b[i].lstPartWork[0], false, 0);
                    OutOPNSetCh3SpecialMode(ym2610b[i].lstPartWork[0], false);

                    //FM Off
                    OutFmAllKeyOff(ym2610b[i]);

                    //SSG Off
                    for (int ch = 9; ch < 12; ch++)
                    {
                        OutSsgKeyOff(ym2610b[i].lstPartWork[ch]);
                        ym2610b[i].lstPartWork[ch].volume = 0;
                        //setSsgVolume(ym2610b[i].lstPartWork[ch]);
                        //ym2610b[i].lstPartWork[ch].volume = 15;
                    }

                    //ADPCM-A/B Reset
                    OutData(ym2610b[i].lstPartWork[0].port0, 0x1c, 0xbf);
                    OutData(ym2610b[i].lstPartWork[0].port0, 0x1c, 0x00);
                    OutData(ym2610b[i].lstPartWork[0].port0, 0x10, 0x00);
                    OutData(ym2610b[i].lstPartWork[0].port0, 0x11, 0xc0);

                    foreach (partWork pw in ym2610b[i].lstPartWork)
                    {
                        if (pw.ch == 0)
                        {
                            pw.hardLfoSw = false;
                            pw.hardLfoNum = 0;
                            OutOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                        }

                        if (pw.ch < 6)
                        {
                            pw.pan.val = 3;
                            pw.ams = 0;
                            pw.fms = 0;
                            if (!pw.dataEnd) OutOPNSetPanAMSPMS(pw, 3, 0, 0);
                        }
                    }

                    dat[0x4f] |= 0x80;//YM2610B
                    if (i != 0) dat[0x4f] |= 0x40;//use Secondary
                }

                if (huc6280[i].use)
                {
                    //MasterVolume(Max volume)
                    huc6280[i].TotalVolume = 0xff;
                    OutHuC6280Port(i == 1, 1, 0xff);
                    //LFO freq 0
                    OutHuC6280Port(i == 1, 8, 0);
                    //LFO ctrl 0
                    OutHuC6280Port(i == 1, 9, 0);

                    foreach (partWork pw in huc6280[i].lstPartWork)
                    {
                        SetHuC6280CurrentChannel(pw);

                        //freq( 0 )
                        pw.freq = 0;
                        OutHuC6280Port(i == 1, 2, 0);
                        OutHuC6280Port(i == 1, 3, 0);

                        pw.pcm = false;
                        OutHuC6280Port(i == 1, 4, 0);

                        pw.panL = 15;
                        pw.panR = 15;
                        OutHuC6280Port(i == 1, 5, 0xff);

                        for (int j = 0; j < 32; j++)
                        {
                            OutHuC6280Port(i == 1, 6, 0);
                        }

                        //noise(Ch5,6 only)
                        pw.noise = 0x1f;
                        OutHuC6280Port(i == 1, 7, 0x1f);
                    }
                }

                ym2413[i].InitChip();
            }

        }

        private void MakeFooter()
        {

            byte[] v;

            //end of data
            OutData(0x66);

            //GD3 offset
            v = DivInt2ByteAry(dat.Count - 0x14);
            dat[0x14] = v[0]; dat[0x15] = v[1]; dat[0x16] = v[2]; dat[0x17] = v[3];

            //Total # samples
            v = DivInt2ByteAry((int)dSample);
            dat[0x18] = v[0]; dat[0x19] = v[1]; dat[0x1a] = v[2]; dat[0x1b] = v[3];

            if (loopOffset != -1)
            {
                //Loop offset
                v = DivInt2ByteAry((int)(loopOffset - 0x1c));
                dat[0x1c] = v[0]; dat[0x1d] = v[1]; dat[0x1e] = v[2]; dat[0x1f] = v[3];

                //Loop # samples
                v = DivInt2ByteAry((int)(dSample - loopSamples));
                dat[0x20] = v[0]; dat[0x21] = v[1]; dat[0x22] = v[2]; dat[0x23] = v[3];
            }

            int p = dat.Count + 12;

            MakeGD3(dat);

            //EoF offset
            v = DivInt2ByteAry(dat.Count - 0x4);
            dat[0x4] = v[0]; dat[0x5] = v[1]; dat[0x6] = v[2]; dat[0x7] = v[3];

            int q = dat.Count - p;

            //GD3 Length
            v = DivInt2ByteAry(q);
            dat[p - 4] = v[0]; dat[p - 3] = v[1]; dat[p - 2] = v[2]; dat[p - 1] = v[3];

            long useYM2151 = 0;
            long useYM2203 = 0;
            long useYM2608 = 0;
            long useYM2610B = 0;
            long useYM2612 = 0;
            long useSN76489 = 0;
            long useRf5c164 = 0;
            long useSegaPcm = 0;
            long useHuC6280 = 0;

            for (int i = 0; i < 2; i++)
            {
                foreach (partWork pw in ym2151[i].lstPartWork)
                {
                    useYM2151 += pw.clockCounter;
                }
                foreach (partWork pw in ym2203[i].lstPartWork)
                {
                    useYM2203 += pw.clockCounter;
                }
                foreach (partWork pw in ym2608[i].lstPartWork)
                {
                    useYM2608 += pw.clockCounter;
                }
                foreach (partWork pw in ym2610b[i].lstPartWork)
                {
                    useYM2610B += pw.clockCounter;
                }
                foreach (partWork pw in ym2612[i].lstPartWork)
                {
                    useYM2612 += pw.clockCounter;
                }
                foreach (partWork pw in sn76489[i].lstPartWork)
                {
                    useSN76489 += pw.clockCounter;
                }
                foreach (partWork pw in rf5c164[i].lstPartWork)
                {
                    useRf5c164 += pw.clockCounter;
                }
                foreach (partWork pw in segapcm[i].lstPartWork)
                {
                    useSegaPcm += pw.clockCounter;
                }
                foreach (partWork pw in huc6280[i].lstPartWork)
                {
                    useHuC6280 += pw.clockCounter;
                }
            }

            if (useYM2151 == 0) { dat[0x30] = 0; dat[0x31] = 0; dat[0x32] = 0; dat[0x33] = 0; }
            if (useYM2203 == 0) { dat[0x44] = 0; dat[0x45] = 0; dat[0x46] = 0; dat[0x47] = 0; }
            if (useYM2608 == 0) { dat[0x48] = 0; dat[0x49] = 0; dat[0x4a] = 0; dat[0x4b] = 0; }
            if (useYM2610B == 0) { dat[0x4c] = 0; dat[0x4d] = 0; dat[0x4e] = 0; dat[0x4f] = 0; }
            if (useYM2612 == 0) { dat[0x2c] = 0; dat[0x2d] = 0; dat[0x2e] = 0; dat[0x2f] = 0; }
            if (useSN76489 == 0) { dat[0x0c] = 0; dat[0x0d] = 0; dat[0x0e] = 0; dat[0x0f] = 0; }
            if (useRf5c164 == 0) { dat[0x6c] = 0; dat[0x6d] = 0; dat[0x6e] = 0; dat[0x6f] = 0; }
            if (useSegaPcm == 0) { dat[0x38] = 0; dat[0x39] = 0; dat[0x3a] = 0; dat[0x3b] = 0; dat[0x3c] = 0; dat[0x3d] = 0; dat[0x3e] = 0; dat[0x3f] = 0; }
            if (useHuC6280 == 0) { dat[0xa4] = 0; dat[0xa5] = 0; dat[0xa6] = 0; dat[0xa7] = 0; }

            if (Version == 1.51f)
            {
                dat[0x08] = 0x51;
                dat[0x09] = 0x01;
            }
            else if (Version == 1.60f)
            {
                dat[0x08] = 0x60;
                dat[0x09] = 0x01;
            }
            else
            {
                dat[0x08] = 0x61;
                dat[0x09] = 0x01;
            }

        }

        private void MakeGD3(List<byte> dat)
        {
            //'Gd3 '
            dat.Add(0x47); dat.Add(0x64); dat.Add(0x33); dat.Add(0x20);

            //GD3 Version
            dat.Add(0x00); dat.Add(0x01); dat.Add(0x00); dat.Add(0x00);

            //GD3 Length(dummy)
            dat.Add(0x00); dat.Add(0x00); dat.Add(0x00); dat.Add(0x00);

            //TrackName
            dat.AddRange(Encoding.Unicode.GetBytes(TitleName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(TitleNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //GameName
            dat.AddRange(Encoding.Unicode.GetBytes(GameName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(GameNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //SystemName
            dat.AddRange(Encoding.Unicode.GetBytes(SystemName));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(SystemNameJ));
            dat.Add(0x00); dat.Add(0x00);

            //Composer
            dat.AddRange(Encoding.Unicode.GetBytes(Composer));
            dat.Add(0x00); dat.Add(0x00);
            dat.AddRange(Encoding.Unicode.GetBytes(ComposerJ));
            dat.Add(0x00); dat.Add(0x00);

            //ReleaseDate
            dat.AddRange(Encoding.Unicode.GetBytes(ReleaseDate));
            dat.Add(0x00); dat.Add(0x00);

            //Converted
            dat.AddRange(Encoding.Unicode.GetBytes(Converted));
            dat.Add(0x00); dat.Add(0x00);

            //Notes
            dat.AddRange(Encoding.Unicode.GetBytes(Notes));
            dat.Add(0x00); dat.Add(0x00);
        }

        private void Xgm_makeHeader()
        {

            //Header
            foreach (byte b in Const.xhDat)
            {
                xdat.Add(b);
            }

            //FM音源を初期化

            OutOPNSetHardLfo(ym2612x[0].lstPartWork[0], false, 0);
            OutOPNSetCh3SpecialMode(ym2612x[0].lstPartWork[0], false);
            ym2612x[0].OutSetCh6PCMMode(ym2612x[0].lstPartWork[0], false);
            OutFmAllKeyOff(ym2612x[0]);

            foreach (partWork pw in ym2612x[0].lstPartWork)
            {
                if (pw.ch == 0)
                {
                    pw.hardLfoSw = false;
                    pw.hardLfoNum = 0;
                    OutOPNSetHardLfo(pw, pw.hardLfoSw, pw.hardLfoNum);
                }

                if (pw.ch < 6)
                {
                    pw.pan.val = 3;
                    pw.ams = 0;
                    pw.fms = 0;
                    if (!pw.dataEnd) OutOPNSetPanAMSPMS(pw, 3, 0, 0);
                }
            }

        }

        private void Xgm_makeFooter()
        {

            //$0004               Sample id table
            uint ptr = 0;
            int n = 4;
            foreach (clsPcm p in instPCM.Values)
            {
                if (p.chip != enmChipType.YM2612X) continue;

                uint stAdr = ptr;
                uint size = (uint)p.size;
                if (size > (uint)p.xgmMaxSampleCount + 1)
                {
                    size = (uint)p.xgmMaxSampleCount + 1;
                    size = (uint)((size & 0xffff00) + (size % 0x100 != 0 ? 0x100 : 0x0));
                }
                p.size = size;

                xdat[n + 0] = (byte)((stAdr / 256) & 0xff);
                xdat[n + 1] = (byte)(((stAdr / 256) & 0xff00) >> 8);
                xdat[n + 2] = (byte)((size / 256) & 0xff);
                xdat[n + 3] = (byte)(((size / 256) & 0xff00) >> 8);

                ptr += size;
                n += 4;
            }

            //$0100               Sample data bloc size / 256
            if (ym2612x[0].pcmData != null)
            {
                xdat[0x100] = (byte)((ptr / 256) & 0xff);
                xdat[0x101] = (byte)(((ptr / 256) & 0xff00) >> 8);
            }
            else
            {
                xdat[0x100] = 0;
                xdat[0x101] = 0;
            }

            //$0103 bit #0: NTSC / PAL information
            xdat[0x103] |= (byte)(xgmSamplesPerSecond == 50 ? 1 : 0);

            //$0104               Sample data block
            if (ym2612x[0].pcmData != null)
            {
                foreach (clsPcm p in instPCM.Values)
                {
                    if (p.chip != enmChipType.YM2612X) continue;

                    for (uint cnt = 0; cnt < p.size; cnt++)
                    {
                        xdat.Add(ym2612x[0].pcmData[p.stAdr + cnt]);
                    }

                }

                //foreach (byte b in ym2612x[0].pcmData)
                //{
                //    xdat.Add(b);
                //}
            }

            if (dat != null)
            {
                //$0104 + SLEN        Music data bloc size.
                xdat.Add((byte)((dat.Count & 0xff) >> 0));
                xdat.Add((byte)((dat.Count & 0xff00) >> 8));
                xdat.Add((byte)((dat.Count & 0xff0000) >> 16));
                xdat.Add((byte)((dat.Count & 0xff000000) >> 24));

                //$0108 + SLEN        Music data bloc
                foreach (byte b in dat)
                {
                    xdat.Add(b);
                }
            }
            else
            {
                xdat.Add(0);
                xdat.Add(0);
                xdat.Add(0);
                xdat.Add(0);
            }

            //$0108 + SLEN + MLEN GD3 tags
            MakeGD3(xdat);

            dat = xdat;
        }

        private void SetEnvelopeAtKeyOn(partWork pw)
        {
            if (!pw.envelopeMode)
            {
                pw.envVolume = 0;
                pw.envIndex = -1;
                return;
            }

            pw.envIndex = 0;
            pw.envCounter = 0;
            int maxValue = pw.MaxVolume;// (pw.envelope[8] == (int)enmChipType.RF5C164) ? 255 : 15;

            while (pw.envCounter == 0 && pw.envIndex != -1)
            {
                switch (pw.envIndex)
                {
                    case 0: // AR phase
                        pw.envCounter = pw.envelope[2];
                        if (pw.envelope[2] > 0 && pw.envelope[1] < maxValue)
                        {
                            pw.envVolume = pw.envelope[1];
                        }
                        else
                        {
                            pw.envVolume = maxValue;
                            pw.envIndex++;
                        }
                        break;
                    case 1: // DR phase
                        pw.envCounter = pw.envelope[3];
                        if (pw.envelope[3] > 0 && pw.envelope[4] < maxValue)
                        {
                            pw.envVolume = maxValue;
                        }
                        else
                        {
                            pw.envVolume = pw.envelope[4];
                            pw.envIndex++;
                        }
                        break;
                    case 2: // SR phase
                        pw.envCounter = pw.envelope[5];
                        if (pw.envelope[5] > 0 && pw.envelope[4] != 0)
                        {
                            pw.envVolume = pw.envelope[4];
                        }
                        else
                        {
                            pw.envVolume = 0;
                            pw.envIndex = -1;
                        }
                        break;
                }
            }
        }

        private void SetLfoAtKeyOn(partWork pw)
        {
            for (int lfo = 0; lfo < 4; lfo++)
            {
                clsLfo pl = pw.lfo[lfo];
                if (!pl.sw)
                {
                    continue;
                }
                if (pl.type == eLfoType.Hardware)
                {
                    if (pw.chip is YM2612)
                    {
                        if (pl.param[4] == 1)
                        {
                            OutOPNSetHardLfo(pw, false, pl.param[1]);
                            pl.waitCounter = pl.param[0];
                        }
                    }
                    continue;
                }
                if (pl.param[5] != 1)
                {
                    continue;
                }

                pl.isEnd = false;
                pl.value = (pl.param[0] == 0) ? pl.param[6] : 0;//ディレイ中は振幅補正は適用されない
                pl.waitCounter = pl.param[0];
                pl.direction = pl.param[2] < 0 ? -1 : 1;
                if (pl.type == eLfoType.Vibrato)
                {
                    if (pw.chip is YM2151)
                    {
                        ((YM2151)pw.chip).SetFNum(pw);
                    }
                    else if ((pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612))
                    {
                        if (pw.Type == enmChannelType.FMOPN || pw.Type == enmChannelType.FMOPNex)
                        {
                            SetFmFNum(pw);
                        }
                        else if (pw.Type == enmChannelType.SSG)
                        {
                            SetSsgFNum(pw);
                        }
                    }
                    else if (pw.chip is SN76489)
                    {
                        SetDcsgFNum(pw);
                    }
                }
                if (pl.type == eLfoType.Tremolo)
                {
                    if (pw.chip is YM2151)
                    {
                        ((YM2151)pw.chip).SetVolume(pw);
                    }
                    else if ((pw.chip is YM2203) || (pw.chip is YM2608) || (pw.chip is YM2610B) || (pw.chip is YM2612))
                    {
                        pw.beforeVolume = -1;
                        SetFmVolume(pw);
                    }
                }
            }
        }

        private int GetToneDoublerShift(partWork pw, int octave, char noteCmd, int shift)
        {
            if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
            {
                return 0;
            }

            int i = pw.instrument;
            if (pw.TdA == -1)
            {
                return 0;
            }

            int TdB = octave * 12 + Const.NOTE.IndexOf(noteCmd) + shift;
            int s = pw.TdA - TdB;
            int us = Math.Abs(s);
            int n = pw.toneDoubler;
            if (us >= instToneDoubler[n].lstTD.Count)
            {
                return 0;
            }

            return ((s < 0) ? s : 0) + instToneDoubler[n].lstTD[us].KeyShift;
        }

        private void SetToneDoubler(partWork pw)
        {
            if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && (pw.Type != enmChannelType.FMOPNex || (pw.Type != enmChannelType.FMOPNex || pw.ch != 2)))
            {
                return;
            }
            //if (pw.Type != enmChannelType.FMOPM && pw.Type != enmChannelType.FMOPN && pw.Type != enmChannelType.FMOPNex)
            //{
              //  return;
            //}

            int i = pw.instrument;
            if (i < 0) return;

            pw.toneDoublerKeyShift = 0;
            if (pw.TdA == -1)
            {
                if (instFM != null && instFM.Count > i)
                {
                    //resetToneDoubler
                    if (pw.Type != enmChannelType.FMOPM)
                    {
                        if (pw.op1ml != instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            OutFmSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op1ml = instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        if (pw.op2ml != instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            OutFmSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op2ml = instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        if (pw.op3ml != instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            OutFmSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op3ml = instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        if (pw.op4ml != instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            OutFmSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op4ml = instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                    }
                    else
                    {
                        //ML
                        if (pw.op1ml != instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op1ml = instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        if (pw.op2ml != instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op2ml = instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        if (pw.op3ml != instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op3ml = instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        if (pw.op4ml != instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8])
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                            pw.op4ml = instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
                        }
                        //DT2
                        if (pw.op1dt2 != instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op1dt2 = instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                        }
                        if (pw.op2dt2 != instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op2dt2 = instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                        }
                        if (pw.op3dt2 != instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op3dt2 = instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                        }
                        if (pw.op4dt2 != instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10])
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op4dt2 = instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 10];
                        }
                    }
                }
            }
            else
            {
                //setToneDoubler
                int TdB = pw.octaveNow * 12 + Const.NOTE.IndexOf(pw.noteCmd) + pw.shift + pw.keyShift;
                int s = pw.TdA - TdB;
                int us = Math.Abs(s);
                int n = pw.toneDoubler;
                if (us >= instToneDoubler[n].lstTD.Count)
                {
                    return;
                }

                pw.toneDoublerKeyShift = ((s < 0) ? s : 0) + instToneDoubler[n].lstTD[us].KeyShift;

                if (instFM != null && instFM.Count > i)
                {
                    if (pw.Type != enmChannelType.FMOPM)
                    {
                        if (pw.op1ml != instToneDoubler[n].lstTD[us].OP1ML)
                        {
                            OutFmSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP1ML);
                            pw.op1ml = instToneDoubler[n].lstTD[us].OP1ML;
                        }
                        if (pw.op2ml != instToneDoubler[n].lstTD[us].OP2ML)
                        {
                            OutFmSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP2ML);
                            pw.op2ml = instToneDoubler[n].lstTD[us].OP2ML;
                        }
                        if (pw.op3ml != instToneDoubler[n].lstTD[us].OP3ML)
                        {
                            OutFmSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP3ML);
                            pw.op3ml = instToneDoubler[n].lstTD[us].OP3ML;
                        }
                        if (pw.op4ml != instToneDoubler[n].lstTD[us].OP4ML)
                        {
                            OutFmSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP4ML);
                            pw.op4ml = instToneDoubler[n].lstTD[us].OP4ML;
                        }
                    }
                    else
                    {
                        //ML
                        if (pw.op1ml != instToneDoubler[n].lstTD[us].OP1ML)
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 0, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP1ML);
                            pw.op1ml = instToneDoubler[n].lstTD[us].OP1ML;
                        }
                        if (pw.op2ml != instToneDoubler[n].lstTD[us].OP2ML)
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 1, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP2ML);
                            pw.op2ml = instToneDoubler[n].lstTD[us].OP2ML;
                        }
                        if (pw.op3ml != instToneDoubler[n].lstTD[us].OP3ML)
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP3ML);
                            pw.op3ml = instToneDoubler[n].lstTD[us].OP3ML;
                        }
                        if (pw.op4ml != instToneDoubler[n].lstTD[us].OP4ML)
                        {
                            ((YM2151)pw.chip).OutSetDtMl(pw, 3, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instToneDoubler[n].lstTD[us].OP4ML);
                            pw.op4ml = instToneDoubler[n].lstTD[us].OP4ML;
                        }
                        //DT2
                        if (pw.op1dt2 != instToneDoubler[n].lstTD[us].OP1DT2)
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 0, instToneDoubler[n].lstTD[us].OP1DT2, instFM[i][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op1dt2 = instToneDoubler[n].lstTD[us].OP1DT2;
                        }
                        if (pw.op2dt2 != instToneDoubler[n].lstTD[us].OP2DT2)
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 1, instToneDoubler[n].lstTD[us].OP2DT2, instFM[i][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op2dt2 = instToneDoubler[n].lstTD[us].OP2DT2;
                        }
                        if (pw.op3dt2 != instToneDoubler[n].lstTD[us].OP3DT2)
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 2, instToneDoubler[n].lstTD[us].OP3DT2, instFM[i][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op3dt2 = instToneDoubler[n].lstTD[us].OP3DT2;
                        }
                        if (pw.op4dt2 != instToneDoubler[n].lstTD[us].OP4DT2)
                        {
                            ((YM2151)pw.chip).OutSetDt2Sr(pw, 3, instToneDoubler[n].lstTD[us].OP4DT2, instFM[i][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                            pw.op4dt2 = instToneDoubler[n].lstTD[us].OP4DT2;
                        }
                    }
                }
                pw.TdA = -1;
            }
        }

        private byte[] DivInt2ByteAry(int n)
        {
            return new byte[4] {
                 (byte)( n & 0xff                   )
                ,(byte)((n & 0xff00    ) / 0x100    )
                ,(byte)((n & 0xff0000  ) / 0x10000  )
                ,(byte)((n & 0xff000000) / 0x1000000)
            };
        }



        private void SetFmFNum(partWork pw)
        {
            if (pw.noteCmd == (char)0)
            {
                return;
            }

            //int[] ftbl = ((pw.chip is YM2612) || (pw.chip is YM2612X)) ? OPN_FNumTbl_7670454 : pw.chip.FNumTbl[0];
            int[] ftbl = pw.chip.FNumTbl[0];

            int f = GetFmFNum(ftbl, pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift + pw.toneDoublerKeyShift);//
            if (pw.bendWaitCounter != -1)
            {
                f = pw.bendFnum;
            }
            int o = (f & 0xf000) / 0x1000;
            f &= 0xfff;

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
            while (f < ftbl[0])
            {
                if (o == 1)
                {
                    break;
                }
                o--;
                f = ftbl[0] * 2 - (ftbl[0] - f);
            }
            while (f >= ftbl[0] * 2)
            {
                if (o == 8)
                {
                    break;
                }
                o++;
                f = f - ftbl[0] * 2 + ftbl[0];
            }
            f = Common.CheckRange(f, 0, 0x7ff);

            OutFmSetFnum(pw, o, f);
        }

        private int GetFmFNum(int[] ftbl, int octave, char noteCmd, int shift)
        {
            int o = octave;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            if (n >= 0)
            {
                o += n / 12;
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
            }
            else
            {
                o += n / 12 - ((n % 12 == 0) ? 0 : 1);
                o = Common.CheckRange(o, 1, 8);
                n %= 12;
                if (n < 0) { n += 12; }
            }

            int f = ftbl[n];

            return (f & 0xfff) + (o & 0xf) * 0x1000;
        }

        private void GetPcmNote(partWork pw)
        {
            int shift = pw.shift + pw.keyShift;
            int o = pw.octaveNow;//
            int n = Const.NOTE.IndexOf(pw.noteCmd) + shift;
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

            pw.pcmOctave = o;
            pw.pcmNote = n;
        }

        private void SetFmVolume(partWork pw)
        {
            int vol = pw.volume;

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            if (pw.beforeVolume != vol)
            {
                if (instFM.ContainsKey(pw.instrument))
                {
                    OutFmSetVolume(pw, vol, pw.instrument);
                    pw.beforeVolume = vol;
                }
            }
        }

        private void SetSsgFNum(partWork pw)
        {
            int f = GetSsgFNum(pw,pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            f = Common.CheckRange(f, 0, 0xfff);
            if (pw.freq == f) return;

            pw.freq = f;

            byte data = 0;

            int n = (pw.chip is YM2203) ? 6 : 9;

            data = (byte)(f & 0xff);
            OutData(pw.port0, (byte)(0 + (pw.ch - n) * 2), data);

            data = (byte)((f & 0xf00) >> 8);
            OutData(pw.port0, (byte)(1 + (pw.ch - n) * 2), data);
        }

        private int GetSsgFNum(partWork pw,int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >= pw.chip.FNumTbl[1].Length) f = pw.chip.FNumTbl[1].Length - 1;

            return pw.chip.FNumTbl[1][f];
        }



        private void SetDcsgFNum(partWork pw)
        {
            if (pw.Type != enmChannelType.DCSGNOISE)
            {
                int f = GetDcsgFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

                f = Common.CheckRange(f, 0, 0x3ff);
                if (pw.freq == f) return;

                pw.freq = f;

                byte data = 0;

                data = (byte)(0x80 + (pw.ch << 5) + (f & 0xf));
                OutPsgPort(pw.isSecondary, data);

                data = (byte)((f & 0x3f0) >> 4);
                OutPsgPort(pw.isSecondary, data);
            }
            else
            {
                int f = pw.noise;
                byte data = (byte)(0xe0 + (f & 0x7));
                pw.freq = 0x40 + (f & 7);
            }

        }

        private int GetDcsgFNum(int octave, char noteCmd, int shift)
        {
            int o = octave - 1;
            int n = Const.NOTE.IndexOf(noteCmd) + shift;
            o += n / 12;
            o = Common.CheckRange(o, 0, 7);
            n %= 12;

            int f = o * 12 + n;
            if (f < 0) f = 0;
            if (f >=sn76489[0].FNumTbl[0].Length) f = sn76489[0].FNumTbl[0].Length - 1;

            return sn76489[0].FNumTbl[0][f];
        }

        private void SetSegaPcmFNum(partWork pw)
        {
            int f = GetSegaPcmFNum(pw.octaveNow, pw.noteCmd, pw.shift + pw.keyShift);//
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

            f = Common.CheckRange(f, 0, 0xff);
            if (pw.freq == f) return;

            pw.freq = f;


            //Delta
            byte data = (byte)(f & 0xff);
            int adr = pw.ch * 8 + 0x07;
            if (pw.beforeFNum != data)
            {
                OutSegaPcmPort(pw, adr, data);
                pw.beforeFNum = data;
            }

        }

        private int GetSegaPcmFNum(int octave, char noteCmd, int shift)
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

            return ((int)(64 * Const.pcmMTbl[n] * Math.Pow(2, (o - 3))) + 1);
        }


        private void SetPsgVolume(partWork pw)
        {
            byte data = 0;

            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (15 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw) continue;
                if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 15);

            if (pw.beforeVolume != vol)
            {
                data = (byte)(0x80 + (pw.ch << 5) + 0x10 + (15 - vol));
                OutPsgPort(pw.isSecondary, data);
                pw.beforeVolume = vol;
            }
        }


        private int GetRf5c164PcmNote(int octave, char noteCmd, int shift)
        {
            int o = octave;
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

            return (int)(0x0400 * Const.pcmMTbl[n] * Math.Pow(2, (o - 4)));
        }

        private void SetRf5c164Volume(partWork pw)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (255 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 255);

            if (pw.beforeVolume != vol)
            {
                SetRf5c164Envelope(pw, vol);
                pw.beforeVolume = vol;
            }
        }

        private void SetRf5c164FNum(partWork pw)
        {
            int f = GetRf5c164PcmNote(pw.octaveNow, pw.noteCmd, pw.keyShift + pw.shift);//

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
            pw.freq = f;

            SetRf5c164CurrentChannel(pw);

            //Address increment 再生スピードをセット
            SetRf5c164AddressIncrement(pw, f);

        }

        private void SetRf5c164Envelope(partWork pw, int volume)
        {
            if (pw.rf5c164Envelope != volume)
            {
                SetRf5c164CurrentChannel(pw);
                byte data = (byte)(volume & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x0, data);
                pw.rf5c164Envelope = volume;
            }
        }

        private void SetRf5c164Pan(partWork pw, int pan)
        {
            if (pw.rf5c164Pan != pan)
            {
                SetRf5c164CurrentChannel(pw);
                byte data = (byte)(pan & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x1, data);
                pw.rf5c164Pan = pan;
            }
        }

        private void SetRf5c164CurrentChannel(partWork pw)
        {
            byte pch = (byte)pw.ch;
            bool isSecondary = pw.isSecondary;
            int chipID = pw.chip.ChipID;

            if (rf5c164[chipID].CurrentChannel != pch)
            {
                byte data = (byte)(0xc0 + pch);
                OutRf5c164Port(isSecondary, 0x7, data);
                rf5c164[chipID].CurrentChannel = pch;
            }
        }

        private void SetRf5c164AddressIncrement(partWork pw, int f)
        {
            if (pw.rf5c164AddressIncrement != f)
            {
                byte data = (byte)(f & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x2, data);
                data = (byte)((f >> 8) & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x3, data);
                pw.rf5c164AddressIncrement = f;
            }
        }

        private void SetRf5c164SampleStartAddress(partWork pw, int adr)
        {
            if (pw.pcmStartAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x6, data);
                pw.pcmStartAddress = adr;
            }
        }

        private void SetRf5c164LoopAddress(partWork pw, int adr)
        {
            if (pw.pcmLoopAddress != adr)
            {
                byte data = (byte)((adr >> 8) & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x5, data);
                data = (byte)(adr & 0xff);
                OutRf5c164Port(pw.isSecondary, 0x4, data);
                pw.pcmLoopAddress = adr;
            }
        }

        private void OutRf5c164KeyOn(partWork pw)
        {
            rf5c164[pw.chip.ChipID].KeyOn |= (byte)(1 << pw.ch);
            byte data = (byte)(~rf5c164[pw.chip.ChipID].KeyOn);
            OutRf5c164Port(pw.isSecondary, 0x8, data);
        }

        private void OutRf5c164KeyOff(partWork pw)
        {
            rf5c164[pw.chip.ChipID].KeyOn &= (byte)(~(1 << pw.ch));
            byte data = (byte)(~rf5c164[pw.chip.ChipID].KeyOn);
            OutRf5c164Port(pw.isSecondary, 0x8, data);
        }

        private void OutRf5c164Port(bool isSecondary, byte adr, byte data)
        {
            dat.Add(0xb1);
            dat.Add((byte)((adr & 0x7f) | (isSecondary ? 0x80 : 0x00)));
            dat.Add(data);
        }

        private void OutSegaPcmVolume(partWork pw)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (pw.MaxVolume - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            int vl = vol * pw.panL / pw.MaxVolume;
            int vr = vol * pw.panR / pw.MaxVolume;
            vl = Common.CheckRange(vl, 0, pw.MaxVolume);
            vr = Common.CheckRange(vr, 0, pw.MaxVolume);

            if (pw.beforeLVolume != vl)
            {
                //Volume(Left)
                int adr = pw.ch * 8 + 0x02;
                OutSegaPcmPort(pw, adr, (byte)vl);
                pw.beforeLVolume = vl;
            }

            if (pw.beforeRVolume != vr)
            {
                //Volume(Right)
                int adr = pw.ch * 8 + 0x03;
                OutSegaPcmPort(pw, adr, (byte)vr);
                pw.beforeRVolume = vr;
            }
        }

        private void OutSegaPcmKeyOff(partWork pw)
        {
            int adr = pw.ch * 8 + 0x86;
            byte d = (byte)(((pw.pcmBank & 0x3f) << 2) | (pw.pcmLoopAddress != -1 ? 0 : 2) | 1);

            OutSegaPcmPort(pw, adr, d);
        }

        private void OutSegaPcmKeyOn(partWork pw)
        {
            int adr = 0;
            byte d = 0;

            //KeyOff
            OutSegaPcmKeyOff(pw);

            //Volume
            OutSegaPcmVolume(pw);

            //StartAdr
            adr = pw.ch * 8 + 0x85;
            d = (byte)((pw.pcmStartAddress & 0xff00) >> 8);
            OutSegaPcmPort(pw, adr, d);

            //StartAdr
            adr = pw.ch * 8 + 0x84;
            d = (byte)((pw.pcmStartAddress & 0x00ff) >> 0);
            OutSegaPcmPort(pw, adr, d);

            if (pw.pcmLoopAddress != -1)
            {
                if (pw.beforepcmLoopAddress != pw.pcmLoopAddress)
                {
                    //LoopAdr
                    adr = pw.ch * 8 + 0x05;
                    d = (byte)((pw.pcmLoopAddress & 0xff00) >> 8);
                    OutSegaPcmPort(pw, adr, d);

                    //LoopAdr
                    adr = pw.ch * 8 + 0x04;
                    d = (byte)((pw.pcmLoopAddress & 0x00ff) >> 0);
                    OutSegaPcmPort(pw, adr, d);

                    pw.beforepcmLoopAddress = pw.pcmLoopAddress;
                }
            }

            if (pw.beforepcmEndAddress != pw.pcmEndAddress)
            {
                //EndAdr
                adr = pw.ch * 8 + 0x06;
                d = (byte)((pw.pcmEndAddress & 0xff00) >> 8);
                d = (byte)((d != 0) ? (d - 1) : 0);
                OutSegaPcmPort(pw, adr, d);
                pw.beforepcmEndAddress = pw.pcmEndAddress;
            }

            adr = pw.ch * 8 + 0x86;
            d = (byte)(((pw.pcmBank & 0x3f) << 2) | (pw.pcmLoopAddress != -1 ? 0 : 2) | 0);
            OutSegaPcmPort(pw, adr, d);
        }

        private void OutSegaPcmPort(partWork pw, int adr, byte data)
        {
            dat.Add(pw.port0);
            dat.Add((byte)adr); //ll
            dat.Add((byte)(((adr & 0x7f00) >> 8) | (pw.isSecondary ? 0x80 : 0))); //hh
            dat.Add(data); //dd
        }


        public void OutData(params byte[] data)
        {
            foreach(byte d in data) dat.Add(d);
        }

        private void OutFmKeyOn(partWork pw)
        {
            int n = (pw.chip is YM2203) ? 0 : 3;

            if (pw.chip is YM2612X && (pw.ch > 8 || (pw.ch == 5 && pw.pcm)))
            {
                OutYM2612XPcmKeyON(pw);
                return;
            }

            if (!pw.pcm)
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
                {
                    pw.Ch3SpecialModeKeyOn = true;

                    int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                        | (pw.chip.lstPartWork[n + 3].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 3].slots : 0x0)
                        | (pw.chip.lstPartWork[n + 4].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 4].slots : 0x0)
                        | (pw.chip.lstPartWork[n + 5].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 5].slots : 0x0);

                    if (pw.chip is YM2612X)
                    {
                        xgmKeyOnData.Add((byte)((slot << 4) + 2));
                    }
                    else
                    {
                        OutData(pw.port0, 0x28, (byte)((slot << 4) + 2));
                    }
                }
                else
                {
                    if (pw.ch >= 0 && pw.ch < n + 3)
                    {
                        byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                        if (pw.chip is YM2612X)
                        {
                            xgmKeyOnData.Add((byte)((pw.slots << 4) + (vch & 7)));
                        }
                        else
                        {
                            //key on
                            OutData(pw.port0, 0x28, (byte)((pw.slots << 4) + (vch & 7)));
                        }
                    }
                }

                return;
            }

            float m = Const.pcmMTbl[pw.pcmNote] * (float)Math.Pow(2, (pw.pcmOctave - 4));
            pw.pcmBaseFreqPerFreq = vgmSamplesPerSecond / ((float)instPCM[pw.instrument].freq * m);
            pw.pcmFreqCountBuffer = 0.0f;
            long p = instPCM[pw.instrument].stAdr;
            if (Version == 1.51f)
            {
                dat.Add(0xe0);
                dat.Add((byte)(p & 0xff));
                dat.Add((byte)((p & 0xff00) / 0x100));
                dat.Add((byte)((p & 0xff0000) / 0x10000));
                dat.Add((byte)((p & 0xff000000) / 0x10000));
            }
            else
            {
                long s = instPCM[pw.instrument].size;
                long f = instPCM[pw.instrument].freq;
                long w = 0;
                if (pw.gatetimePmode)
                {
                    w = pw.waitCounter * pw.gatetime / 8L;
                }
                else
                {
                    w = pw.waitCounter - pw.gatetime;
                }
                if (w < 1) w = 1;
                s = Math.Min(s, (long)(w * samplesPerClock * f / 44100.0));

                if (!pw.streamSetup)
                {
                    newStreamID++;
                    pw.streamID = newStreamID;
                    // setup stream control
                    dat.Add(0x90);
                    dat.Add((byte)pw.streamID);
                    dat.Add((byte)(0x02 + (pw.isSecondary ? 0x80 : 0x00)));
                    dat.Add(0x00);
                    dat.Add(0x2a);

                    // set stream data
                    dat.Add(0x91);
                    dat.Add((byte)pw.streamID);
                    dat.Add(0x00);
                    dat.Add(0x01);
                    dat.Add(0x00);

                    pw.streamSetup = true;
                }

                if (pw.streamFreq != f)
                {
                    //Set Stream Frequency
                    dat.Add(0x92);
                    dat.Add((byte)pw.streamID);

                    dat.Add((byte)(f & 0xff));
                    dat.Add((byte)((f & 0xff00) / 0x100));
                    dat.Add((byte)((f & 0xff0000) / 0x10000));
                    dat.Add((byte)((f & 0xff000000) / 0x10000));

                    pw.streamFreq = f;
                }

                //Start Stream
                dat.Add(0x93);
                dat.Add((byte)pw.streamID);

                dat.Add((byte)(p & 0xff));
                dat.Add((byte)((p & 0xff00) / 0x100));
                dat.Add((byte)((p & 0xff0000) / 0x10000));
                dat.Add((byte)((p & 0xff000000) / 0x10000));

                dat.Add(0x01);

                dat.Add((byte)(s & 0xff));
                dat.Add((byte)((s & 0xff00) / 0x100));
                dat.Add((byte)((s & 0xff0000) / 0x10000));
                dat.Add((byte)((s & 0xff000000) / 0x10000));
            }
        }

        private void OutYM2612XPcmKeyON(partWork pw)
        {
            if (pw.instrument >= 63) return;

            int id = instPCM[pw.instrument].seqNum + 1;
            int ch = Math.Max(0, pw.ch - 8);
            int priority = 0;

            dat.Add(0x54); // original vgm command : YM2151
            dat.Add((byte)(0x50 + ((priority & 0x3) << 2) + (ch & 0x3)));
            dat.Add((byte)id);

            samplesPerClock = xgmSamplesPerSecond * 60.0 * 4.0 / (tempo * clockCount);

            //必要なサンプル数を算出し、保持しているサンプル数より大きい場合は更新
            double m = pw.waitCounter * 60.0 * 4.0 / (tempo * clockCount) * 14000.0;//14000(Hz) = xgm sampling Rate
            instPCM[pw.instrument].xgmMaxSampleCount = Math.Max(instPCM[pw.instrument].xgmMaxSampleCount, m);

        }

        public void OutFmKeyOff(partWork pw)
        {
            int n = (pw.chip is YM2203) ? 0 : 3;

            if (!pw.pcm)
            {
                if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
                {
                    pw.Ch3SpecialModeKeyOn = false;

                    int slot = (pw.chip.lstPartWork[2].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[2].slots : 0x0)
                        | (pw.chip.lstPartWork[n + 3].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 3].slots : 0x0)
                        | (pw.chip.lstPartWork[n + 4].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 4].slots : 0x0)
                        | (pw.chip.lstPartWork[n + 5].Ch3SpecialModeKeyOn ? pw.chip.lstPartWork[n + 5].slots : 0x0);

                    OutData(pw.port0, 0x28, (byte)((slot << 4) + 2));
                }
                else
                {
                    if (pw.ch >= 0 && pw.ch < n + 3)
                    {
                        byte vch = (byte)((pw.ch > 2) ? pw.ch + 1 : pw.ch);
                        //key off
                        OutData(pw.port0, 0x28, (byte)(0x00 + (vch & 7)));
                    }
                }

                return;
            }

            pw.pcmWaitKeyOnCounter = -1;
        }

        public void OutFmAllKeyOff(ClsChip chip)
        {

            foreach (partWork pw in chip.lstPartWork)
            {
                //if (pw.dataEnd) continue;
                if (pw.ch > 5) continue;

                OutFmKeyOff(pw);
                OutFmSetTl(pw, 0, 127);
                OutFmSetTl(pw, 1, 127);
                OutFmSetTl(pw, 2, 127);
                OutFmSetTl(pw, 3, 127);
            }

        }

        private void OutFmSetFnum(partWork pw, int octave, int num)
        {
            int freq;
            freq = ((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3);
            freq = (freq << 8) + (num & 0xff);

            if (freq == pw.freq) return;

            pw.freq = freq;

            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.Type == enmChannelType.FMOPNex)
            {
                if ((pw.slots & 8) != 0)
                {
                    int f = pw.freq + pw.slotDetune[3];
                    OutData(pw.port0, (byte)0xa6, (byte)((f & 0xff00) >> 8));
                    OutData(pw.port0, (byte)0xa2, (byte)(f & 0xff));
                }
                if ((pw.slots & 4) != 0)
                {
                    int f = pw.freq + pw.slotDetune[2];
                    OutData(pw.port0, (byte)0xac, (byte)((f & 0xff00) >> 8));
                    OutData(pw.port0, (byte)0xa8, (byte)(f & 0xff));
                }
                if ((pw.slots & 1) != 0)
                {
                    int f = pw.freq + pw.slotDetune[0];
                    OutData(pw.port0, (byte)0xad, (byte)((f & 0xff00) >> 8));
                    OutData(pw.port0, (byte)0xa9, (byte)(f & 0xff));
                }
                if ((pw.slots & 2) != 0)
                {
                    int f = pw.freq + pw.slotDetune[1];
                    OutData(pw.port0, (byte)0xae, (byte)((f & 0xff00) >> 8));
                    OutData(pw.port0, (byte)0xaa, (byte)(f & 0xff));
                }
            }
            else
            {
                int n = (pw.chip is YM2203) ? 0 : 3;
                if (pw.ch >= n + 3 && pw.ch < n + 6)
                {
                    return;
                }
                if ((pw.chip is YM2612X) && pw.ch >= 9 && pw.ch <= 11)
                {
                    return;
                }
                if (pw.ch < n + 3)
                {
                    if (pw.pcm) return;

                    byte port = pw.ch > 2 ? pw.port1 : pw.port0;
                    byte vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);

                    OutData(port, (byte)(0xa4 + vch), (byte)((pw.freq & 0xff00) >> 8));
                    OutData(port, (byte)(0xa0 + vch), (byte)(pw.freq & 0xff));
                }
            }
        }

        private void OutFmCh3SpecialModeSetFnum(partWork pw, byte ope, int octave, int num)
        {
            ope &= 3;
            if (ope == 0)
            {
                OutData(pw.port0, 0xa6, (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                OutData(pw.port0, 0xa2, (byte)(num & 0xff));
            }
            else
            {
                OutData(pw.port0, (byte)(0xac + ope), (byte)(((num & 0x700) >> 8) + (((octave - 1) & 0x7) << 3)));
                OutData(pw.port0, (byte)(0xa8 + ope), (byte)(num & 0xff));
            }
        }

        private void OutFmSetInstrument(partWork pw, int n, int vol)
        {

            if (!instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            int m = (pw.chip is YM2203) ? 0 : 3;

            if (pw.ch >= m + 3 && pw.ch < m + 6)
            {
                msgBox.setWrnMsg("拡張チャンネルでは音色指定はできません。", pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            for (int ope = 0; ope < 4; ope++)
            {

                OutFmSetDtMl(pw, ope, instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 9], instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 8]);
                OutFmSetKsAr(pw, ope, instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 7], instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 1]);
                OutFmSetAmDr(pw, ope, instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 10], instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 2]);
                OutFmSetSr(pw, ope, instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 3]);
                OutFmSetSlRr(pw, ope, instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 5], instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 4]);
                OutFmSetSSGEG(pw, ope, instFM[n][ope * Const.INSTRUMENT_M_OPERATOR_SIZE + 11]);

            }
            pw.op1ml = instFM[n][0 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op2ml = instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op3ml = instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op4ml = instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 8];
            pw.op1dt2 = 0;
            pw.op2dt2 = 0;
            pw.op3dt2 = 0;
            pw.op4dt2 = 0;

            OutFmSetFeedbackAlgorithm(pw, instFM[n][46], instFM[n][45]);

            OutFmSetVolume(pw, vol, n);

        }

        /// <summary>
        /// FMボリュームの設定
        /// </summary>
        /// <param name="ch">チャンネル</param>
        /// <param name="vol">ボリューム値</param>
        /// <param name="n">音色番号</param>
        private void OutFmSetVolume(partWork pw, int vol, int n)
        {
            if (!instFM.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定している場合ボリュームの変更はできません。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            int alg = instFM[n][45] & 0x7;
            int[] ope = new int[4] {
                instFM[n][0*Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , instFM[n][1 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , instFM[n][2 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
                , instFM[n][3 * Const.INSTRUMENT_M_OPERATOR_SIZE + 6]
            };
            int[][] algs = new int[8][]
            {
                new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,0,0,1}
                ,new int[4] { 0,1,0,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 0,1,1,1}
                ,new int[4] { 1,1,1,1}
            };

            //int minV = 127;
            //for (int i = 0; i < 4; i++)
            //{
            //    if (algs[alg][i] == 1 && (pw.slots & (1 << i)) != 0)
            //    {
            //        minV = Math.Min(minV, ope[i]);
            //    }
            //}

            for (int i = 0; i < 4; i++)
            {
                if (algs[alg][i] == 0 || (pw.slots & (1 << i)) == 0)
                {
                    continue;
                }
                //ope[i] = ope[i] - minV + (127 - vol);
                ope[i] = ope[i] + (127 - vol);
                ope[i] = Common.CheckRange(ope[i], 0, 127);
            }

            partWork vpw = pw;
            int m = (pw.chip is YM2203) ? 0 : 3;
            if (pw.chip.lstPartWork[2].Ch3SpecialMode && pw.ch >= m + 3 && pw.ch < m + 6)
            {
                vpw = pw.chip.lstPartWork[2];
            }

            if ((pw.slots & 1) != 0) OutFmSetTl(vpw, 0, ope[0]);
            if ((pw.slots & 2) != 0) OutFmSetTl(vpw, 1, ope[1]);
            if ((pw.slots & 4) != 0) OutFmSetTl(vpw, 2, ope[2]);
            if ((pw.slots & 8) != 0) OutFmSetTl(vpw, 3, ope[3]);
        }

        private void OutFmSetDtMl(partWork pw, int ope, int dt, int ml)
        {
            int vch = pw.ch;
            byte port = vch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            dt &= 7;
            ml &= 15;

            OutData(port, (byte)(0x30 + vch + ope * 4), (byte)((dt << 4) + ml));
        }

        public void OutFmSetTl(partWork pw, int ope, int tl)
        {
            byte port = (pw.ch > 2 ? pw.port1 : pw.port0);
            int vch = (byte)(pw.ch > 2 ? pw.ch - 3 : pw.ch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            tl &= 0x7f;

            OutData(port, (byte)(0x40 + vch + ope * 4), (byte)tl);
        }

        private void OutFmSetKsAr(partWork pw, int ope, int ks, int ar)
        {
            int vch = pw.ch;
            byte port = (pw.ch > 2 ? pw.port1 : pw.port0);
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            ks &= 3;
            ar &= 31;

            OutData(port, (byte)(0x50 + vch + ope * 4), (byte)((ks << 6) + ar));
        }

        private void OutFmSetAmDr(partWork pw, int ope, int am, int dr)
        {
            int vch = pw.ch;
            byte port = (pw.ch > 2 ? pw.port1 : pw.port0);
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            am &= 1;
            dr &= 31;

            OutData(port, (byte)(0x60 + vch + ope * 4), (byte)((am << 7) + dr));
        }

        private void OutFmSetSr(partWork pw, int ope, int sr)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sr &= 31;

            OutData(port, (byte)(0x70 + vch + ope * 4), (byte)(sr));
        }

        private void OutFmSetSlRr(partWork pw, int ope, int sl, int rr)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            sl &= 15;
            rr &= 15;

            OutData(port, (byte)(0x80 + vch + ope * 4), (byte)((sl << 4) + rr));
        }

        private void OutFmSetSSGEG(partWork pw, int ope, int n)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            ope = (ope == 1) ? 2 : ((ope == 2) ? 1 : ope);
            n &= 15;

            OutData(port, (byte)(0x90 + vch + ope * 4), (byte)n);
        }

        private void OutFmSetFeedbackAlgorithm(partWork pw, int fb, int alg)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            fb &= 7;
            alg &= 7;

            OutData(port, (byte)(0xb0 + vch), (byte)((fb << 3) + alg));
        }

        public void OutOPNSetPanAMSPMS(partWork pw, int pan, int ams, int pms)
        {
            int vch = pw.ch;
            byte port = pw.ch > 2 ? pw.port1 : pw.port0;
            vch = (byte)(vch > 2 ? vch - 3 : vch);

            pan = pan & 3;
            ams = ams & 7;
            pms = pms & 3;

            OutData(port, (byte)(0xb4 + vch), (byte)((pan << 6) + (ams << 3) + pms));
        }

        public void OutOPNSetHardLfo(partWork pw, bool sw, int lfoNum)
        {
            dat.Add(pw.port0);
            dat.Add(0x22);
            dat.Add((byte)((lfoNum & 7) + (sw ? 8 : 0)));
        }

        public void OutOPNSetCh3SpecialMode(partWork pw, bool sw)
        {
            // ignore Timer ^^;
            dat.Add(pw.port0);
            dat.Add(0x27);
            dat.Add((byte)((sw ? 0x40 : 0)));
        }


        private void OutSsgKeyOn(partWork pw)
        {
            int m = (pw.chip is YM2203) ? 0 : 3;
            byte pch = (byte)(pw.ch - (m + 6));
            int n = (pw.mixer & 0x1) + ((pw.mixer & 0x2) << 2);
            byte data = 0;

            if (pw.chip is YM2203 opn)
            {
                data = (byte)(opn.SSGKeyOn | (9 << pch));
                data &= (byte)(~(n << pch));
                opn.SSGKeyOn = data;
            }
            else if (pw.chip is YM2608 opna)
            {
                data = (byte)(opna.SSGKeyOn | (9 << pch));
                data &= (byte)(~(n << pch));
                opna.SSGKeyOn = data;
            }
            else if (pw.chip is YM2610B opnb)
            {
                data = (byte)(opnb.SSGKeyOn | (9 << pch));
                data &= (byte)(~(n << pch));
                opnb.SSGKeyOn = data;
            }

            SetSsgVolume(pw);
            OutData(pw.port0, 0x07, data);
        }

        private void OutSsgKeyOff(partWork pw)
        {
            int m = (pw.chip is YM2203) ? 0 : 3;
            byte pch = (byte)(pw.ch - (m + 6));
            int n = 9;
            byte data = 0;

            if (pw.chip is YM2203)
            {
                data = (byte)(((YM2203)pw.chip).SSGKeyOn | (n << pch));
                ((YM2203)pw.chip).SSGKeyOn = data;
            }
            else if (pw.chip is YM2608)
            {
                data = (byte)(((YM2608)pw.chip).SSGKeyOn | (n << pch));
                ((YM2608)pw.chip).SSGKeyOn = data;
            }
            else if (pw.chip is YM2610B)
            {
                data = (byte)(((YM2610B)pw.chip).SSGKeyOn | (n << pch));
                ((YM2610B)pw.chip).SSGKeyOn = data;
            }

            OutData(pw.port0, (byte)(0x08 + pch), 0);
            pw.beforeVolume = -1;
            OutData(pw.port0, 0x07, data);

        }

        private void SetSsgVolume(partWork pw)
        {
            int m = (pw.chip is YM2203) ? 0 : 3;
            byte pch = (byte)(pw.ch - (m + 6));

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.volume - (15 - pw.envVolume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw) continue;
                if (pw.lfo[lfo].type != eLfoType.Tremolo) continue;

                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 15);

            if (pw.beforeVolume != vol)
            {
                OutData(pw.port0, (byte)(0x08 + pch), (byte)vol);
                pw.beforeVolume = pw.volume;
            }
        }

        private void OutSsgNoise(partWork pw, int n)
        {
            OutData(pw.port0, 0x06, (byte)(n & 0x1f));
        }



        private void OutPsgPort(bool isSecondary, byte data)
        {
            dat.Add((byte)(isSecondary ? 0x30 : 0x50));
            dat.Add(data);
        }

        private void OutPsgKeyOn(partWork pw)
        {
            byte pch = (byte)pw.ch;
            byte data = 0;


            data = (byte)(0x80 + (pch << 5) + 0x00 + (pw.freq & 0xf));
            OutPsgPort(pw.isSecondary, data);

            if (pch != 3)
            {
                data = (byte)((pw.freq & 0x3f0) >> 4);
                OutPsgPort(pw.isSecondary, data);
            }

            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (15 - pw.volume);
                }
            }
            if (vol > 15) vol = 15;
            if (vol < 0) vol = 0;
            data = (byte)(0x80 + (pch << 5) + 0x10 + (15 - vol));
            OutPsgPort(pw.isSecondary, data);

        }

        private void OutPsgKeyOff(partWork pw)
        {

            byte pch = (byte)pw.ch;
            int val = 15;

            byte data = (byte)(0x80 + (pch << 5) + 0x10 + (val & 0xf));
            //outPsgPort((pw.ch - 9 * 2) > 3, data); //bug? 2017/06/17
            OutPsgPort(pw.isSecondary, data);

        }



        private void OutWaitNSamples(long n)
        {
            long m = n;

            while (m > 0)
            {
                if (m > 0xffff)
                {
                    dat.Add(0x61);
                    dat.Add((byte)0xff);
                    dat.Add((byte)0xff);
                    m -= 0xffff;
                }
                else
                {
                    dat.Add(0x61);
                    dat.Add((byte)(m & 0xff));
                    dat.Add((byte)((m & 0xff00) >> 8));
                    m = 0L;
                }
            }
        }

        private void OutWait735Samples(int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                dat.Add(0x62);
            }
        }

        private void OutWait882Samples(int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                dat.Add(0x63);
            }
        }

        private void OutWaitNSamplesWithPCMSending(partWork cpw, long cnt)
        {
            for (int i = 0; i < samplesPerClock * cnt;)
            {

                int f = (int)cpw.pcmBaseFreqPerFreq;
                cpw.pcmFreqCountBuffer += cpw.pcmBaseFreqPerFreq - (int)cpw.pcmBaseFreqPerFreq;
                while (cpw.pcmFreqCountBuffer > 1.0f)
                {
                    f++;
                    cpw.pcmFreqCountBuffer -= 1.0f;
                }
                if (i + f >= samplesPerClock * cnt)
                {
                    cpw.pcmFreqCountBuffer += (int)(i + f - samplesPerClock * cnt);
                    f = (int)(samplesPerClock * cnt - i);
                }
                if (cpw.pcmSizeCounter > 0)
                {
                    cpw.pcmSizeCounter--;
                    dat.Add((byte)(0x80 + f));
                }
                else
                {
                    OutWaitNSamples(f);
                }
                i += f;
            }
        }


        private void SetHuC6280FNum(partWork pw)
        {
            int f = GetHuC6280Freq(pw.octaveNow, pw.noteCmd, pw.keyShift + pw.shift);//

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

            f = Common.CheckRange(f, 0, 0x0fff);

            if (pw.freq == f) return;

            SetHuC6280CurrentChannel(pw);
            //if ((pw.freq & 0x0ff) != (f & 0x0ff)) outHuC6280Port(pw.isSecondary, 2, (byte)(f & 0xff));
            //if ((pw.freq & 0xf00) != (f & 0xf00)) outHuC6280Port(pw.isSecondary, 3, (byte)((f & 0xf00) >> 8));
            OutHuC6280Port(pw.isSecondary, 2, (byte)(f & 0xff));
            OutHuC6280Port(pw.isSecondary, 3, (byte)((f & 0xf00) >> 8));

            pw.freq = f;

        }

        private int GetHuC6280Freq(int octave, char noteCmd, int shift)
        {
            int o = octave;
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
            return (int)(huc6280[0].Frequency / 32.0f / 261.62f / (Const.pcmMTbl[n] * (float)Math.Pow(2, (o - 4))));
        }

        private void SetHuC6280Volume(partWork pw)
        {
            int vol = pw.volume;

            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (31 - pw.volume);
                }
            }

            for (int lfo = 0; lfo < 4; lfo++)
            {
                if (!pw.lfo[lfo].sw)
                {
                    continue;
                }
                if (pw.lfo[lfo].type != eLfoType.Tremolo)
                {
                    continue;
                }
                vol += pw.lfo[lfo].value + pw.lfo[lfo].param[6];
            }

            vol = Common.CheckRange(vol, 0, 31);
            if (pw.beforeVolume != vol)
            {
                SetHuC6280Envelope(pw, vol);
                pw.beforeVolume = vol;
            }
        }

        private void SetHuC6280Envelope(partWork pw, int volume)
        {
            if (pw.huc6280Envelope != volume)
            {
                SetHuC6280CurrentChannel(pw);
                byte data = (byte)(0x80+(volume & 0x1f));
                OutHuC6280Port(pw.isSecondary, 4, data);
                pw.huc6280Envelope = volume;
            }
        }

        private void SetHuC6280CurrentChannel(partWork pw)
        {
            byte pch = (byte)pw.ch;
            bool isSecondary = pw.isSecondary;
            int chipID = pw.chip.ChipID;

            if (huc6280[chipID].CurrentChannel != pch)
            {
                byte data = (byte)(pch & 0x7);
                OutHuC6280Port(isSecondary, 0x0, data);
                huc6280[chipID].CurrentChannel = pch;
            }
        }

        private void SetHuC6280Pan(partWork pw, int pan)
        {
            if (pw.huc6280Pan != pan)
            {
                SetHuC6280CurrentChannel(pw);
                byte data = (byte)(pan & 0xff);
                OutHuC6280Port(pw.isSecondary, 0x5, data);
                pw.huc6280Pan = pan;
            }
        }

        private void OutHuC6280Port(bool isSecondary,byte adr, byte data)
        {
            dat.Add(0xb9);
            dat.Add((byte)((isSecondary ? 0x80 : 0x00) + adr));
            dat.Add(data);
        }

        private void OutHuC6280SetInstrument(partWork pw, int n)
        {

            if (!instWF.ContainsKey(n))
            {
                msgBox.setWrnMsg(string.Format("未定義の音色(@{0})を指定しています。", n), pw.getSrcFn(), pw.getLineNumber());
                return;
            }

            SetHuC6280CurrentChannel(pw);
            OutHuC6280Port(pw.isSecondary, 4, (byte)(0x40 + pw.volume)); //WaveIndexReset(=0x40)

            for (int i = 1; i < instWF[n].Length; i++) // 0 は音色番号が入っている為1からスタート
            {
                OutHuC6280Port(pw.isSecondary, 6, (byte)(instWF[n][i] & 0x1f));
            }

        }

        private void OutHuC6280KeyOn(partWork pw)
        {
            SetHuC6280CurrentChannel(pw);
            int vol = pw.volume;
            if (pw.envelopeMode)
            {
                vol = 0;
                if (pw.envIndex != -1)
                {
                    vol = pw.envVolume - (31 - pw.volume);
                }
            }
            if (vol > 31) vol = 31;
            if (vol < 0) vol = 0;
            byte data = (byte)(0x80 + vol);

            if (!pw.pcm)
            {
                OutHuC6280Port(pw.isSecondary, 0x4, data);
                OutHuC6280Port(pw.isSecondary, 0x5, (byte)pw.huc6280Pan);
                return;
            }

            if (Version == 1.51f)
            {
                return;
            }

            data |= 0x40;
            OutHuC6280Port(pw.isSecondary, 0x4, data);
            OutHuC6280Port(pw.isSecondary, 0x5, (byte)pw.huc6280Pan);

            float m = Const.pcmMTbl[pw.pcmNote] * (float)Math.Pow(2, (pw.pcmOctave - 4));
            pw.pcmBaseFreqPerFreq = vgmSamplesPerSecond / ((float)instPCM[pw.instrument].freq * m);
            pw.pcmFreqCountBuffer = 0.0f;
            long p = instPCM[pw.instrument].stAdr;

            long s = instPCM[pw.instrument].size;
            long f = instPCM[pw.instrument].freq;
            long w = 0;
            if (pw.gatetimePmode)
            {
                w = pw.waitCounter * pw.gatetime / 8L;
            }
            else
            {
                w = pw.waitCounter - pw.gatetime;
            }
            if (w < 1) w = 1;
            s = Math.Min(s, (long)(w * samplesPerClock * f / 44100.0));

            if (!pw.streamSetup)
            {
                newStreamID++;
                pw.streamID = newStreamID;
                // setup stream control
                dat.Add(0x90);
                dat.Add((byte)pw.streamID);
                dat.Add((byte)(0x1b + (pw.isSecondary ? 0x80 : 0x00))); //0x1b HuC6280
                dat.Add((byte)pw.ch);
                dat.Add((byte)(0x00 + 0x06));// 0x00 Select Channel 

                // set stream data
                dat.Add(0x91);
                dat.Add((byte)pw.streamID);
                dat.Add(0x05); // Data BankID(0x05 HuC6280)
                dat.Add(0x01);
                dat.Add(0x00);

                pw.streamSetup = true;
            }

            if (pw.streamFreq != f)
            {
                //Set Stream Frequency
                dat.Add(0x92);
                dat.Add((byte)pw.streamID);

                dat.Add((byte)(f & 0xff));
                dat.Add((byte)((f & 0xff00) / 0x100));
                dat.Add((byte)((f & 0xff0000) / 0x10000));
                dat.Add((byte)((f & 0xff000000) / 0x10000));

                pw.streamFreq = f;
            }

            //Start Stream
            dat.Add(0x93);
            dat.Add((byte)pw.streamID);

            dat.Add((byte)(p & 0xff));
            dat.Add((byte)((p & 0xff00) / 0x100));
            dat.Add((byte)((p & 0xff0000) / 0x10000));
            dat.Add((byte)((p & 0xff000000) / 0x10000));

            dat.Add(0x01);

            dat.Add((byte)(s & 0xff));
            dat.Add((byte)((s & 0xff00) / 0x100));
            dat.Add((byte)((s & 0xff0000) / 0x10000));
            dat.Add((byte)((s & 0xff000000) / 0x10000));
        }

        private void OutHuC6280KeyOff(partWork pw)
        {
            SetHuC6280CurrentChannel(pw);

            OutHuC6280Port(pw.isSecondary, 0x4, 0);
            OutHuC6280Port(pw.isSecondary, 0x5, 0);
        }



    }
}
