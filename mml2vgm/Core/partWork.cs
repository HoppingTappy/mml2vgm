﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class partWork
    {

        /// <summary>
        /// パートデータ
        /// </summary>
        public List<Line> pData = null;

        /// <summary>
        /// エイリアスデータ
        /// </summary>
        public Dictionary<string, Line> aData = null;

        /// <summary>
        /// データが最後まで演奏されたかどうかを示す(注意:trueでも演奏が終わったとは限らない)
        /// </summary>
        public bool dataEnd = false;

        /// <summary>
        /// 次に演奏されるデータの位置
        /// </summary>
        private clsPos pos = new clsPos();

        /// <summary>
        /// 位置情報のスタック
        /// </summary>
        private Stack<clsPos> stackPos = new Stack<clsPos>();

        /// <summary>
        /// リピート位置情報のスタック
        /// </summary>
        public Stack<clsRepeat> stackRepeat = new Stack<clsRepeat>();

        /// <summary>
        /// パートごとの音源の種類
        /// </summary>
        public ClsChip chip = null;

        /// <summary>
        /// Secondary Chipか
        /// </summary>
        public bool isSecondary = false;
        
        /// <summary>
        /// 割り当てられた音源のチャンネル番号
        /// </summary>
        public int ch = 0;

        /// <summary>
        /// 未加工のf-num
        /// </summary>
        public int freq = 0;

        public int beforeFNum = -1;
        public int FNum = -1;


        /// <summary>
        /// いままで演奏した総クロック数
        /// </summary>
        public long clockCounter = 0L;

        /// <summary>
        /// あとどれだけ待機するかを示すカウンター(clock)
        /// </summary>
        public long waitCounter = 0L;

        /// <summary>
        /// キーオフコマンドを発行するまであとどれだけ待機するかを示すカウンター(clock)
        /// (waitCounterよりも大きい場合キーオフされない)
        /// </summary>
        public long waitKeyOnCounter = 0L;

        /// <summary>
        /// lコマンドで設定されている音符の長さ(clock)
        /// </summary>
        public long length = 24;

        /// <summary>
        /// oコマンドで設定されているオクターブ数
        /// </summary>
        public int octaveNow = 4;

        public int octaveNew = 4;

        public int TdA = -1;
        public int op1ml = -1;
        public int op2ml = -1;
        public int op3ml = -1;
        public int op4ml = -1;
        public int op1dt2 = -1;
        public int op2dt2 = -1;
        public int op3dt2 = -1;
        public int op4dt2 = -1;
        public int toneDoubler = 0;
        public int toneDoublerKeyShift = 0;

        /// <summary>
        /// vコマンドで設定されている音量
        /// </summary>
        public int volume = 127;

        /// <summary>
        /// pコマンドで設定されている音の定位(1:R 2:L 3:C)
        /// </summary>
        //public int pan = 3;
        //public int beforePan = -1;
        public dint pan = new dint(3);

        /// <summary>
        /// 拡張パン(Left)
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int panL = -1;

        /// <summary>
        /// 拡張パン(Right)
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int panR = -1;

        /// <summary>
        /// 拡張ボリューム(Left)before
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int beforeLVolume = -1;

        /// <summary>
        /// 拡張ボリューム(Right)before
        /// </summary>
        /// <remarks>
        /// ボリュームが左右別管理の音源向け
        /// </remarks>
        public int beforeRVolume = -1;

        /// <summary>
        /// @コマンドで設定されている音色
        /// </summary>
        public int instrument = -1;

        /// <summary>
        /// 使用中のエンベロープ定義番号
        /// </summary>
        public int envInstrument = -1;

        /// <summary>
        /// エンベロープの進捗位置
        /// </summary>
        public int envIndex = -1;

        /// <summary>
        /// エンベロープ向け汎用カウンター
        /// </summary>
        public int envCounter = -1;

        /// <summary>
        /// エンベロープ音量
        /// </summary>
        public int envVolume = -1;

        /// <summary>
        /// 使用中のエンベロープの定義
        /// </summary>
        public int[] envelope = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, -1 };

        /// <summary>
        /// エンベロープスイッチ
        /// </summary>
        public bool envelopeMode = false;

        /// <summary>
        /// Dコマンドで設定されているデチューン
        /// </summary>
        public int detune = 0;

        /// <summary>
        /// 発音される音程
        /// </summary>
        public char noteCmd = (char)0;//'c';

        /// <summary>
        /// 音程をずらす量
        /// </summary>
        public int shift = 0;

        /// <summary>
        /// PCMの音程
        /// </summary>
        public int pcmNote = 0;
        public int pcmOctave = 0;

        /// <summary>
        /// mコマンドで設定されているpcmモード(true:PCM false:FM)
        /// </summary>
        public bool pcm = false;

        public float pcmBaseFreqPerFreq = 0.0f;
        public float pcmFreqCountBuffer = 0.0f;
        public long pcmWaitKeyOnCounter = 0L;
        public long pcmSizeCounter = 0L;

        public bool streamSetup = false;
        public int streamID = -1;
        public long streamFreq = 0;


        /// <summary>
        /// q/Qコマンドで設定されているゲートタイム(clock/%)
        /// </summary>
        public int gatetime = 0;

        /// <summary>
        /// q/Qコマンドで最後に指定されたのはQコマンドかどうか
        /// </summary>
        public bool gatetimePmode = false;

        /// <summary>
        /// 使用するスロット
        /// </summary>
        public byte slots = 0xf;

        /// <summary>
        /// タイ
        /// </summary>
        public bool tie = false;

        /// <summary>
        /// 前回発音時にタイ指定があったかどうか
        /// </summary>
        public bool beforeTie = false;

        public bool keyOn = false;
        public bool keyOff = false;

        /// <summary>
        /// 前回発音時の音量
        /// </summary>
        public int beforeVolume = -1;

        /// <summary>
        /// 効果音モード
        /// </summary>
        public bool Ch3SpecialMode = false;

        /// <summary>
        /// KeyOnフラグ
        /// </summary>
        public bool Ch3SpecialModeKeyOn = false;

        /// <summary>
        /// Lfo(4つ)
        /// </summary>
        public clsLfo[] lfo = new clsLfo[4] { new clsLfo(), new clsLfo(), new clsLfo(), new clsLfo() };

        /// <summary>
        /// ベンド中のoコマンドで設定されているオクターブ数
        /// </summary>
        public int bendOctave = 4;

        /// <summary>
        /// ベンド中の音程
        /// </summary>
        public char bendNote = 'r';

        /// <summary>
        /// ベンド中の待機カウンター
        /// </summary>
        public long bendWaitCounter = -1;

        /// <summary>
        /// ベンド中に参照される周波数スタックリスト
        /// </summary>
        public Stack<Tuple<int, int>> bendList = new Stack<Tuple<int, int>>();

        /// <summary>
        /// ベンド中の発音周波数
        /// </summary>
        public int bendFnum = 0;

        /// <summary>
        /// ベンド中に音程をずらす量
        /// </summary>
        public int bendShift = 0;

        /// <summary>
        /// スロットごとのディチューン値
        /// </summary>
        public int[] slotDetune = new int[] { 0, 0, 0, 0 };

        /// <summary>
        /// ノイズモード値
        /// </summary>
        public int noise = 0;

        /// <summary>
        /// SSG Noise or Tone mixer 0:Silent 1:Tone 2:Noise 3:Tone&Noise
        /// OPM Noise 0:Disable 1:Enable
        /// </summary>
        public int mixer = 1;

        /// <summary>
        /// キーシフト
        /// </summary>
        public int keyShift = 0;

        public string PartName="";

        public int rf5c164AddressIncrement = -1;
        public int rf5c164Envelope = -1;
        public int rf5c164Pan = -1;

        public int huc6280Envelope = -1;
        public int huc6280Pan = -1;

        public int pcmStartAddress = -1;
        public int pcmLoopAddress = -1;
        public int beforepcmLoopAddress = -1;
        public int pcmEndAddress = -1;
        public int beforepcmEndAddress = -1;
        public int pcmBank = 0;

        public enmChannelType Type;
        public int MaxVolume = 0;
        public byte port0 = 0;
        public byte port1 = 0;
        public int ams = 0;
        public int fms = 0;
        public int pms = 0;
        public bool hardLfoSw = false;
        public int hardLfoNum = 0;

        public int hardLfoFreq = 0;
        public int hardLfoPMD = 0;
        public int hardLfoAMD = 0;

        public bool reqFreqReset = false;
        public bool reqKeyOffReset = false;

        public bool renpuFlg = false;
        public List<int> lstRenpuLength = null;

        /// <summary>
        /// パート情報をリセットする
        /// </summary>
        public void resetPos()
        {
            pos = new clsPos();
            stackPos = new Stack<clsPos>();
        }

        /// <summary>
        /// 解析位置を取得する
        /// </summary>
        /// <returns></returns>
        public int getPos()
        {
            return pos.tCol;
        }

        /// <summary>
        /// 解析位置に対するソースファイル上の行数を得る
        /// </summary>
        /// <returns></returns>
        public int getLineNumber()
        {
            if (pos.alies == "")
            {
                return pData[pos.row].Num;
            }
            return aData[pos.alies].Num;
        }

        /// <summary>
        /// 解析位置に対するソースファイル名を得る
        /// </summary>
        /// <returns></returns>
        public string getSrcFn()
        {
            if (pos.alies == "")
            {
                return pData[pos.row].Fn;
            }
            return aData[pos.alies].Fn;
        }

        /// <summary>
        /// 解析位置の文字を取得する
        /// </summary>
        /// <returns></returns>
        public char getChar()
        {
            //if (dataEnd) return (char)0;

            char ch;
            if (pos.alies == "")
            {
                if (pData[pos.row].Txt.Length <= pos.col) {
                    return (char)0;
                }
                ch = pData[pos.row].Txt[pos.col];
            }
            else
            {
                if (aData[pos.alies].Txt.Length <= pos.col)
                {
                    return (char)0;
                }
                ch = aData[pos.alies].Txt[pos.col];
            }
            //Console.Write(ch);
            return ch;
        }

        /// <summary>
        /// 解析位置を一つ進める(重い！)
        /// </summary>
        public void incPos()
        {
            setPos(pos.tCol + 1);
        }

        /// <summary>
        /// 解析位置を一つ戻す(重い！)
        /// </summary>
        public void decPos()
        {
            setPos(pos.tCol - 1);
        }

        /// <summary>
        /// 指定された文字数だけ読み出し、文字列を生成する
        /// </summary>
        /// <param name="len">文字数</param>
        /// <returns>文字列</returns>
        public string getString(int len)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                sb.Append(getChar());
                incPos();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 解析位置を指定する
        /// </summary>
        /// <param name="tCol">解析位置</param>
        /// <remarks>エイリアスを毎回展開しながら位置を算出するためとても効率が悪い</remarks>
        public void setPos(int tCol)
        {
            if (pData == null)
            {
                return;
            }

            if (LstPos == null) MakeLstPos();

            int i = 0;
            while (i != LstPos.Count && tCol >= LstPos[i].tCol)
            {
                i++;
            }

            pos.tCol = tCol;
            pos.alies = LstPos[i - 1].alies;
            pos.col = LstPos[i - 1].col + tCol - LstPos[i - 1].tCol;
            pos.row = LstPos[i - 1].row;
            //if (pos.alies == "" && pos.col >= pData[pos.row].Txt.Length - 1 && pos.row >= pData.Count - 1)
            //{
            //    dataEnd = true;
            //}
            return;

            dataEnd = false;

            int row = 0;
            int col = 0;
            int n = 0;
            string aliesName = "";
            resetPos();

            pos.tCol = tCol;

            while (true)
            {
                string data;
                char ch;

                //読みだすデータの頭出し
                if (aliesName == "")
                {
                    if (pData.Count == row)
                    {
                        dataEnd = true;
                        return;
                    }
                    data = pData[row].Txt;
                }
                else
                {
                    data = aData[aliesName].Txt;
                }

                while (data.Length == col)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            dataEnd = true;
                            break;
                        }
                        else
                        {
                            data = pData[row].Txt;
                            col = 0;
                            break;
                        }
                    }
                    else
                    {
                        clsPos p = stackPos.Pop();
                        aliesName = p.alies;
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = pData[row].Txt;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                        }
                    }
                }
                ch = data[col];

                while (ch == '%')
                {
                    string a = getAliesName(data, col);
                    if (a != "")
                    {
                        clsPos p = new clsPos();
                        p.alies = aliesName;
                        p.col = col + a.Length + 1;
                        p.row = row;
                        stackPos.Push(p);

                        data = aData[a].Txt;
                        col = 0;
                        aliesName = a;
                        row = 0;
                    }
                    else
                    {
                        msgBox.setWrnMsg("指定されたエイリアス名は定義されていません。"
                            , (aliesName == "") ? pData[row].Fn : aData[aliesName].Fn
                            , (aliesName == "") ? pData[row].Num : aData[aliesName].Num
                            );
                        col++;
                    }
                    ch = data[col];
                }

                if (n == tCol)
                {
                    pos.row = row;
                    pos.col = col;
                    pos.alies = aliesName;
                    break;
                }

                n++;
                col++;
                while (data.Length == col)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            dataEnd = true;
                            break;
                        }
                        else
                        {
                            data = pData[row].Txt;
                            col = 0;
                            break;
                        }
                    }
                    else
                    {
                        clsPos p = stackPos.Pop();
                        aliesName = p.alies;
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = pData[row].Txt;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                        }
                    }
                }

            }

        }

        private List<clsPos> LstPos = null;

        public void MakeLstPos()
        {
            if (pData == null)
            {
                return;
            }

            int tCol = 0;
            int row = 0;
            int col = 0;
            string aliesName = "";

            LstPos = new List<clsPos>();
            LstPos.Add(new clsPos());
            resetPos();

            while (true)
            {
                string data;
                char ch;

                //読みだすデータの頭出し
                if (aliesName == "")
                {
                    if (pData.Count == row)
                    {
                        return;
                    }
                    data = pData[row].Txt;
                }
                else
                {
                    data = aData[aliesName].Txt;
                }

                //解析行の解析位置が終端に達したときの処理
                while (data.Length == col)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            break;
                        }
                        else
                        {
                            data = pData[row].Txt;
                            col = 0;

                            clsPos p = new clsPos();
                            p.tCol = tCol;
                            p.alies = "";
                            p.col = 0;
                            p.row = row;
                            LstPos.Add(p);

                            break;
                        }
                    }
                    else
                    {
                        clsPos p = stackPos.Pop();
                        aliesName = p.alies;
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = pData[row].Txt;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                        }

                        p.tCol = tCol;
                        LstPos.Add(p);
                    }
                }

                ch = data[col];

                //解析位置でエイリアス指定されている場合
                while (ch == '%')
                {
                    string a = getAliesName(data, col);
                    if (a != "")
                    {
                        clsPos p = new clsPos();
                        p.alies = aliesName;
                        p.col = col + a.Length + 1;
                        p.row = row;
                        stackPos.Push(p);

                        data = aData[a].Txt;
                        col = 0;
                        aliesName = a;
                        row = 0;

                        p = new clsPos();
                        p.tCol = tCol;
                        p.alies = a;
                        p.col = 0;
                        p.row = 0;
                        LstPos.Add(p);
                    }
                    else
                    {
                        msgBox.setWrnMsg("指定されたエイリアス名は定義されていません。"
                            , (aliesName == "") ? pData[row].Fn : aData[aliesName].Fn
                            , (aliesName == "") ? pData[row].Num : aData[aliesName].Num
                            );
                        col++;
                    }

                    ch = data[col];
                }

                tCol++;
                col++;
                //解析行の解析位置が終端に達したときの処理
                while (data.Length == col)
                {
                    if (aliesName == "")
                    {
                        row++;
                        if (pData.Count == row)
                        {
                            break;
                        }
                        else
                        {
                            data = pData[row].Txt;
                            col = 0;

                            clsPos p = new clsPos();
                            p.tCol = tCol;
                            p.alies = "";
                            p.col = 0;
                            p.row = row;
                            LstPos.Add(p);

                            break;
                        }
                    }
                    else
                    {
                        clsPos p = stackPos.Pop();
                        aliesName = p.alies;
                        col = p.col;
                        row = p.row;
                        if (aliesName == "")
                        {
                            data = pData[row].Txt;
                        }
                        else
                        {
                            data = aData[aliesName].Txt;
                        }

                        p.tCol = tCol;
                        LstPos.Add(p);
                    }
                }

            }

        }

        /// <summary>
        /// 解析位置から数値を取得する。
        /// </summary>
        /// <param name="num">取得した数値が返却される</param>
        /// <returns>数値取得成功したかどうか</returns>
        public bool getNum(out int num)
        {

            string n = "";
            int ret = -1;

            //タブと空白は読み飛ばす
            while (getChar() == ' ' || getChar() == '\t')
            {
                incPos();
            }

            //符号を取得する(ない場合は正とする)
            if (getChar() == '-' || getChar() == '+')
            {
                n = getChar().ToString();
                incPos();
            }

            //タブと空白は読み飛ばす
            while (getChar() == ' ' || getChar() == '\t')
            {
                incPos();
            }

            //１６進数指定されているか
            if (getChar() != '$')
            {
                //数字でなくなるまで取得
                while (true)
                {
                    if (getChar() >= '0' && getChar() <= '9')
                    {
                        try
                        {
                            n += getChar();
                            incPos();
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //数値に変換できたら成功
                if (!int.TryParse(n, out ret))
                {
                    num = -1;
                    return false;
                }

                num = ret;
            }
            else
            {
                //２文字取得
                incPos();
                n += getChar();
                incPos();
                n += getChar();
                incPos();
                //数値に変換できたら成功
                try
                {
                    num = Convert.ToInt32(n, 16);
                }
                catch
                {
                    num = -1;
                    return false;
                }
            }

            return true;
        }

        public bool getNumNoteLength(out int num, out bool flg) {

            flg = false;

            //タブと空白は読み飛ばす
            while (getChar() == ' ' || getChar() == '\t')
            {
                incPos();
            }

            //クロック直接指定
            if (getChar() == '#')
            {
                flg = true;
                incPos();
            }

            return getNum(out num);
        }

        /// <summary>
        /// エイリアス名を取得する
        /// </summary>
        /// <param name="data"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private string getAliesName(string data, int col)
        {
            if (data.Length <= col + 1)
            {
                return "";
            }

            string wrd = data.Substring(col + 1);
            while (!aData.ContainsKey(wrd))
            {
                if (wrd.Length == 1)
                {
                    return "";
                }
                wrd = wrd.Substring(0, wrd.Length - 1);
            }
            return wrd;
        }
    }

    public class clsPos
    {

        /// <summary>
        /// すべてのデータ行を１行としたときの次に演奏されるデータの何桁目か
        /// </summary>
        public int tCol = 0;

        /// <summary>
        /// 次に演奏されるデータの何行目か
        /// </summary>
        public int row = 0;

        /// <summary>
        /// 次に演奏されるデータの何桁目か
        /// </summary>
        public int col = 0;

        /// <summary>
        /// 次に演奏されるデータのエイリアス名
        /// </summary>
        public string alies = "";
    }

    public class clsRepeat
    {
        /// <summary>
        /// 位置
        /// </summary>
        public int pos = 0;

        /// <summary>
        /// リピート向け回数
        /// </summary>
        public int repeatCount = 2;

    }

    public class clsLfo
    {

        /// <summary>
        /// Lfoの種類
        /// </summary>
        public eLfoType type = eLfoType.Hardware;

        /// <summary>
        /// Lfoの設定値
        /// </summary>
        public List<int> param = null;

        /// <summary>
        /// Lfoのスイッチ
        /// </summary>
        public bool sw = false;

        /// <summary>
        /// Lfoが完了したかどうか
        /// </summary>
        public bool isEnd = false;

        /// <summary>
        /// Lfoの待機カウンター
        /// </summary>
        public long waitCounter = 0;

        /// <summary>
        /// Lfoの変化値
        /// </summary>
        public int value = 0;
        
        /// <summary>
        /// Lfoの変化する方向
        /// </summary>
        public int direction = 0;

    }

    public class clsPcm
    {
        public enmChipType chip = enmChipType.YM2612;
        public bool isSecondary = false;
        public int num = 0;
        public int seqNum = 0;
        public double xgmMaxSampleCount = 0;
        public string fileName = "";
        public int freq = 0;
        public int vol = 0;
        public long stAdr = 0;
        public long edAdr = 0;
        public long size = 0;
        public long loopAdr = -1;

        public clsPcm(int num,int seqNum, enmChipType chip,bool isSecondary,string fileName, int freq, int vol , long stAdr, long edAdr, long size,long loopAdr)
        {
            this.num = num;
            this.seqNum = seqNum;
            this.chip = chip;
            this.isSecondary = isSecondary;
            this.fileName = fileName;
            this.freq = freq;
            this.vol = vol;
            this.stAdr = stAdr;
            this.edAdr = edAdr;
            this.size = size;
            this.loopAdr = loopAdr;
        }
    }

    public class clsToneDoubler
    {
        public int num = 0;
        public List<clsTD> lstTD = null;

        public clsToneDoubler(int num, List<clsTD> lstTD)
        {
            this.num = num;
            this.lstTD = lstTD;
        }
    }

    public class clsTD
    {
        public int OP1ML = 0;
        public int OP2ML = 0;
        public int OP3ML = 0;
        public int OP4ML = 0;
        public int OP1DT2 = 0;
        public int OP2DT2 = 0;
        public int OP3DT2 = 0;
        public int OP4DT2 = 0;
        public int KeyShift = 0;

        public clsTD(int op1ml, int op2ml, int op3ml, int op4ml, int op1dt2, int op2dt2, int op3dt2, int op4dt2, int keyshift)
        {
            OP1ML = op1ml;
            OP2ML = op2ml;
            OP3ML = op3ml;
            OP4ML = op4ml;
            OP1DT2 = op1dt2;
            OP2DT2 = op2dt2;
            OP3DT2 = op3dt2;
            OP4DT2 = op4dt2;
            KeyShift = keyshift;
        }
    }

}
