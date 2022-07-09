using System.Threading.Tasks;

namespace ZonyLrcTools.Cli.Infrastructure.Lyric
{
    /// <summary>
    /// 歌词数据下载器，用于匹配并下载歌曲的歌词。
    /// </summary>
    public interface ILyricDownloader
    {
        /// <summary>
        /// 下载歌词数据。
        /// </summary>
        /// <param name="songName">歌曲的名称。</param>
        /// <param name="artist">歌曲的作者。</param>
        /// <returns>歌曲的歌词数据对象。</returns>
        ValueTask<string> DownloadAsync(string songName, string artist);

        /// <summary>
        /// 下载器的名称。
        /// </summary>
        string DownloaderName { get; }
    }
}