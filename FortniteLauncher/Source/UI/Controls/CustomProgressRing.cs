using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Labs.WinUI;

public static class UI
{
    public static ProgressRing ProgressRing;

    public static void ShowProgressRing(SettingsCard TargetCard, bool Show)
    {
        if (!Show)
        {
            if (ProgressRing != null)
                ProgressRing.IsActive = false;
            return;
        }

        var Ring = new ProgressRing { IsIndeterminate = true };
        TargetCard.Content = Ring;
        ProgressRing = Ring;
    }
}
