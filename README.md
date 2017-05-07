# mml2vgm
メガドライブ他向けVGM/XGMファイル作成ツール  
  
[概要]  
 このツールは、ユーザーが作成したMMLファイルを元にVGM/XGMファイルを作成します。  
  
[機能、特徴]  
 [VGM]  
 ・メガドライブ2台分の音源構成(YM2612 + SN76489 + RF5C164)にそったVGMを生成します。  
 (他にYM2151,YM2203,YM2608,YM2610B,SegaPCM,HuC6280に対応しています。)  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・PCM(YM2612)を1ch使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・MEGA-CDのPCM音源(RF5C164)は8ch使用可能です。  
 ・以上、メガドライブ音源系だけで最大42ch(その他合計で184ch)使用可能です。  
 （但し、RF5C164の2台目についてはVGMPlayでは今のところ正式には対応しておらず、鳴らすにはMDPlayerが必要です。)  
 ・MMLの仕様はFMP7(開発:Guu氏)に似せています。  
  
 [XGM]  
 ・メガドライブの音源構成(YM2612 + SN76489)にそったXGMを生成します。  
 ・FM音源(YM2612)は最大6ch(この内1chを効果音モードに指定すると更に3ch)使用可能です。  
 ・ソフトウェアによる制御によりPCM(YM2612)を4ch同時使用可能です。(FM音源1chと排他的に使用します。)  
 ・PSG(DCSG)音源(SN76489)は4ch(この内1chはノイズチャンネル)使用可能です。  
 ・以上、最大12ch使用可能です。  
  
[必要な環境]  
 ・Windows7以降のOSがインストールされたPC  
 ・テキストエディタ  
 ・時間と暇  
 ・気合と根性  
  
[SpecialThanks]  
 本ツールは以下の方々にお世話になっております。また以下のソフトウェア、ウェブページを参考、使用しています。  
 本当にありがとうございます。  
  
 ・ラエル さん  
 ・WING☆ さん  
 ・とぼけがお さん  
 ・wani さん  
 ・mucom さん  
 ・ume3fmp さん  
 ・おやぢぴぴ さん  
 ・なると さん  
 ・hex125 さん  
  
 ・XPCMK  
 ・FMP7  
 ・Music LALF  
 ・NRTDRV  
 ・Visual Studio Community 2015  
 ・SGDK  
 ・VGM Player  
 ・Git  
 ・SourceTree  
 ・さくらエディター  
  
 ・SGDKとは - nendo  
 ・FutureDriver  
 ・SMS Power!  
 ・DOBON.NET  
 ・Wikipedia  
 ・retroPC.net  
  
