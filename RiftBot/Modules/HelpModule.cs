namespace RiftBot;

public class HelpModule : ModuleBase<SocketCommandContext>
{
    public CommandService CommandService { get; set; }

    // TODO: Rework this to use the static names and summaries class
    [Command("help", RunMode = RunMode.Async)]
    [Summary("!help - Shows this help text")]
    public async Task Help([Remainder] string admin = "")
    {
        if (admin == "admin")
        {
            StringBuilder sb = new();
            foreach (CommandInfo command in CommandService.Commands)
            {
                if (command.Module.Name == "MemeModule") continue;

                if (command.Preconditions.Any(x => x.GetType() == typeof(RequireOwnerAttribute)))
                {
                    continue;
                }

                if (command.Summary is null) continue;

                sb.Append($"{command.Summary.Replace("Admin: ", "")}");

                if (command.Aliases.Count > 1)
                {
                    sb.Append(" Aliases: ");
                    int i = 0;
                    foreach (string commandAlias in command.Aliases)
                    {
                        if (commandAlias == command.Name) continue;
                        if (i == command.Aliases.Count)
                        {
                            sb.Append($"{commandAlias}");
                        }
                        else
                        {
                            sb.Append($"{commandAlias}, ");
                        }

                        i++;
                    }
                }

                sb.AppendLine();
            }

            await ReplyAsync(sb.ToString()).ConfigureAwait(false);
        }
        else
        {
            StringBuilder sb = new();
            foreach (CommandInfo command in CommandService.Commands)
            {
                if (command.Module.Name == "MemeModule") continue;

                if (command.Preconditions.Any(x => x.GetType() == typeof(RequireOwnerAttribute)))
                {
                    continue;
                }

                if (command.Summary is null) continue;
                if (command.Summary.StartsWith("Admin: ")) continue;

                sb.Append($"{command.Summary}");

                if (command.Aliases.Count > 1)
                {
                    sb.Append(" Aliases: ");
                    int i = 0;
                    foreach (string commandAlias in command.Aliases)
                    {
                        if (commandAlias == command.Name) continue;
                        if (i == command.Aliases.Count)
                        {
                            sb.Append($"{commandAlias}");
                        }
                        else
                        {
                            sb.Append($"{commandAlias}, ");
                        }

                        i++;
                    }
                }

                sb.AppendLine();
            }

            await ReplyAsync(sb.ToString()).ConfigureAwait(false);
        }
    }
}