using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZonyLrcTools.Cli.Infrastructure.Exceptions;
using ZonyLrcTools.Cli.Infrastructure.Lyric.KuGou.JsonModel;
using ZonyLrcTools.Cli.Infrastructure.Network;

namespace ZonyLrcTools.Cli.Infrastructure.Lyric.KuGou
{
    public class KuGourLyricDownloader : LyricDownloader
    {
        public override string DownloaderName => InternalLyricDownloaderNames.KuGou;

        private readonly IWarpHttpClient _warpHttpClient;
        private readonly ILyricItemCollectionFactory _lyricItemCollectionFactory;

        private const string KuGouSearchMusicUrl = @"https://songsearch.kugou.com/song_search_v2";
        private const string KuGouGetLyricAccessKeyUrl = @"http://lyrics.kugou.com/search";
        private const string KuGouGetLyricUrl = @"http://lyrics.kugou.com/download";

        public KuGourLyricDownloader(IWarpHttpClient warpHttpClient,
            ILyricItemCollectionFactory lyricItemCollectionFactory)
        {
            _warpHttpClient = warpHttpClient;
            _lyricItemCollectionFactory = lyricItemCollectionFactory;
        }

        protected override async ValueTask<byte[]> DownloadDataAsync(LyricDownloaderArgs args)
        {
            var searchResult = await _warpHttpClient.GetAsync<SongSearchResponse>(KuGouSearchMusicUrl,
                new SongSearchRequest(args.SongName, args.Artist));

            ValidateSongSearchResponse(searchResult, args);

            // 获得特殊的 AccessToken 与 Id，真正请求歌词数据。
            var accessKeyResponse = await _warpHttpClient.GetAsync<GetLyricAccessKeyResponse>(KuGouGetLyricAccessKeyUrl,
                new GetLyricAccessKeyRequest(searchResult.Data.List[0].FileHash));

            var accessKeyObject = accessKeyResponse.AccessKeyDataObjects[0];
            var lyricResponse = await _warpHttpClient.GetAsync(KuGouGetLyricUrl,
                new GetLyricRequest(accessKeyObject.Id, accessKeyObject.AccessKey));

            return Encoding.UTF8.GetBytes(lyricResponse);
        }

        protected override async ValueTask<string> GenerateLyricAsync(byte[] data, LyricDownloaderArgs args)
        {
            await ValueTask.CompletedTask;
            var lyricJsonObj = JObject.Parse(Encoding.UTF8.GetString(data));
            if (lyricJsonObj.SelectToken("$.status").Value<int>() != 200)
            {
                throw new ErrorCodeException(ErrorCodes.NoMatchingSong, attachObj: args);
            }

            var lyricText = Encoding.UTF8.GetString(Convert.FromBase64String(lyricJsonObj.SelectToken("$.content").Value<string>()));
            //return _lyricItemCollectionFactory.Build(lyricText);
            return lyricText;
        }

        protected virtual void ValidateSongSearchResponse(SongSearchResponse response, LyricDownloaderArgs args)
        {
            if (response.ErrorCode != 0 && response.Status != 1 || response.Data.List == null)
            {
                throw new ErrorCodeException(ErrorCodes.NoMatchingSong, attachObj: args);
            }
        }
    }
}