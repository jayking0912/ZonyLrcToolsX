using System.Collections.Generic;
using ZonyLrcTools.Cli.Infrastructure.Lyric;

namespace ZonyLrcTools.Cli.Config;

public class LyricOption
{
    public IEnumerable<LyricProviderOption> Plugin { get; set; }

    public LyricConfigOption Config { get; set; }
}

public class LyricConfigOption
{
    /// <summary>
    /// 双语歌词是否合并为一行。
    /// </summary>
    public bool IsOneLine { get; set; } = false;

    /// <summary>
    /// 换行符格式，取值来自 <see cref="LineBreakType"/> 常量类。
    /// </summary>
    public string LineBreak { get; set; } = LineBreakType.Windows;

    /// <summary>
    /// 是否启用歌词翻译功能。
    /// </summary>
    public bool IsEnableTranslation { get; set; } = true;

    /// <summary>
    /// 如果歌词文件已经存在，是否跳过这些文件
    /// </summary>
    public bool IsSkipExistLyricFiles { get; set; } = false;

    /// <summary>
    /// 歌词文件的编码格式。
    /// </summary>
    public string FileEncoding { get; set; } = "utf-8";
}