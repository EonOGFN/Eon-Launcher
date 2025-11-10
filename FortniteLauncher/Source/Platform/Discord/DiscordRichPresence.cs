using System;
using DiscordRPC;
using System.Threading.Tasks;

class EonRPC
{
    private static readonly DiscordRpcClient Client = new("1208133454215122984");
    private static readonly DateTime StartTimestamp = DateTime.UtcNow;

    public static async void Start()
    {
        Client.Initialize();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                UpdatePresence();
                await Task.Delay(1 * 1000);
            }
        });
    }

    private static void UpdatePresence()
    {
        if (!Client.IsInitialized)
            return;

        Client.SetPresence(new RichPresence
        {
            State = "Chapter 2 Season 7 - OG Fortnite",
            Timestamps = new Timestamps { Start = StartTimestamp },

            Assets = new Assets
            {
                LargeImageKey = "fn17",
                LargeImageText = "Eon"
            },

            Buttons = new[]
            {
                new Button { Label = "Join Discord", Url = ProjectDefinitions.Discord },
            }
        });
    }
}