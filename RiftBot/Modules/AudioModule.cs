using System.Diagnostics;
using Discord.Audio;

namespace RiftBot.Modules
{
    //public class AudioModule : ModuleBase<SocketCommandContext>
    //{
    //    [Command("join", RunMode = RunMode.Async)]
    //    [RequireOwner]
    //    public async Task JoinChannel(IVoiceChannel channel = null)
    //    {
    //        channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
    //        if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

    //        var audioClient = await channel.ConnectAsync();
    //        await SendAsync(audioClient, "");
    //    }

    //    [Command("leave", RunMode = RunMode.Async)]
    //    [RequireOwner]
    //    public async Task LeaveChannel(IVoiceChannel channel = null)
    //    {
    //        channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
    //        if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

    //        await channel.DisconnectAsync();
    //    }

    //    private Process CreateStream(string path)
    //    {
    //        return Process.Start(new ProcessStartInfo
    //        {
    //            FileName = "ffmpeg",
    //            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
    //            UseShellExecute = false,
    //            RedirectStandardOutput = true,
    //        });
    //    }

    //    private async Task SendAsync(IAudioClient client, string path)
    //    {
    //        // Create FFmpeg using the previous example
    //        using (var ffmpeg = CreateStream(path))
    //        using (var output = ffmpeg.StandardOutput.BaseStream)
    //        using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
    //        {
    //            try { await output.CopyToAsync(discord); }
    //            finally { await discord.FlushAsync(); }
    //        }
    //    }
    //}
}