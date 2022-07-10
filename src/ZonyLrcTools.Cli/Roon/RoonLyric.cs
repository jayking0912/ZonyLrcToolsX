using ZonyLrcTools.Cli.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using ATL;
using ATL.AudioData;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;


namespace ZonyLrcTools.Cli.Commands.SubCommand

{
    //查找写入成功会生成同名字的 .done文件
    //如果丢失或者失败，会生成 .error文件，可以手动写入歌曲名和歌手，会再次查找



    public class RoonLyric
    {
        public static (bool isContine,MusicInfo info) LyricFrontCheck(MusicInfo info)
        {
            //删除lrc文件
            // string lrcName = Path.GetFileNameWithoutExtension(info.FilePath)+".lrc";
            // if(File.Exists(lrcName)){
            //     File.Delete(lrcName);
            // }

            if (File.Exists(info.FilePath + ".done")){
                   //有.done 忽略
                   return (false,null);
            }

            
            //info.Name = CheckStrSimple(theFile.Title);
            //info.Artist = CheckStrSimple(theFile.Artist);

            if(File.Exists(info.FilePath+".error")){
                //先清空
                 if (ClearLyric(info.FilePath))
                {
                    System.Console.WriteLine("[清除成功]" + info.FilePath);
                }
                //在看下里面有没有文字
                var fileRes = File.ReadAllLines(info.FilePath + ".error");
                if(fileRes.Length>1){
                    System.Console.WriteLine("    [自定义]" + "歌曲名：" + fileRes[0]);
                    info.Name = String.IsNullOrEmpty(fileRes[0])?info.Name : fileRes[0];
                    System.Console.WriteLine("    [自定义]" + "艺术歌手：" + fileRes[1]);
                    info.Artist = String.IsNullOrEmpty(fileRes[1])?info.Artist : fileRes[1];
                    return (true,info);
                }else{
                    return (false,null);
                }


            }


            return (true,info);
        }

        public static bool ClearLyric(String musicPath)
        {
            Track theFile = new Track(musicPath);
            //theFile.Lyrics = new LyricsInfo();
            //theFile.Lyrics.LanguageCode = "zhs";
            //theFile.Lyrics.Description = "song";

            theFile.AdditionalFields["LYRICS"] = "";
            //// Persists the chapters
            return theFile.Save();
        }

        //繁体字转化为简体
        static string CheckStrSimple(string str)
        {
            return  ChineseConverter.Convert(str, ChineseConversionDirection.TraditionalToSimplified);
           
        }

        public static void WriteResultFile(string audioFilePath,string endName,string Title = "",string Artist="")
        {
            if(File.Exists(audioFilePath+".done")){
                File.Delete(audioFilePath+".done");
            }
            if(File.Exists(audioFilePath+".error")){
                File.Delete(audioFilePath+".error");
            }
            if(!String.IsNullOrWhiteSpace(Title) || !String.IsNullOrWhiteSpace(Artist)){
                File.WriteAllLines(audioFilePath + endName, new string[] { Title, Artist });
            }else{
                File.WriteAllText(audioFilePath + endName,"");
            }

            
        }

        public static bool WriteLyric(String musicPath, string lyricTem)
        {
            Track theFile = new Track(musicPath);
            theFile.Lyrics = new LyricsInfo();
            theFile.Lyrics.LanguageCode = "zhs";
            theFile.Lyrics.Description = "song";

            //输入
            lyricTem = FixLyrics(lyricTem);
            theFile.AdditionalFields["LYRICS"] = lyricTem;
            //// Persists the chapters
            return theFile.Save();

            // 校验
            //theFile = new Track(audioFilePath);
            ////读取歌词
            //string cc = theFile.AdditionalFields["LYRICS"];

            //System.Console.WriteLine(cc);
        }

        public static string FixLyrics(string data)
        {
            string result = "";
            var tem = data.Split("\n");
            foreach (var aa in tem){
              if(aa.Split("]").Length>1){
                var  hTem = aa.Split("]")[0].Replace("[", "");
                if (!String.IsNullOrWhiteSpace(ConvertTime(hTem)))
                {
                    result += "["+ConvertTime(hTem)+"]"+aa.Split("]")[1] + "\n";
                }
              }
           
            }
            return result;

        }

        public static string ConvertTime(string ss)
        {
            try
            {
                var aa= DateTime.ParseExact(ss, "mm:ss.fff", System.Globalization.CultureInfo.CurrentCulture);
                return aa.ToString("mm:ss.ff");
                //return true;
            }
            catch
            {
                try{
                    var aa= DateTime.ParseExact(ss, "mm:ss.ff", System.Globalization.CultureInfo.CurrentCulture);
                    return ss;
                }
                catch{
                     return "";
                }
               
                return "";
            }
        }

    }
}