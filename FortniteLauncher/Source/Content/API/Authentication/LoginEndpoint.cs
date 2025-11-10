using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

public enum VerifyLoginStatus { Success, Banned, Deny, Invalid, Error, Outdated, Donator }

class Authenticator
{
    public static async Task<ApiResponse> CheckLogin(string Email, string Password)
    {
        var Client = new RestClient($"{Definitions.BaseURL}:8443/");
        var Request = new RestRequest("/api/v1/auth/login", Method.Post)
            .AddParameter("email", Email)
            .AddParameter("password", Password)
            .AddHeader("X-Launcher-Version", Definitions.CurrentVersion);

        try
        {
            var ApiResponse = await Client.ExecuteAsync(Request);
            if (string.IsNullOrWhiteSpace(ApiResponse.Content))
                return await ErrorResponse
                (
                    "Please check your network connection or try again later. Make sure you're on a stable network without a VPN, mobile cellular data, or any network restrictions like firewalls or proxy settings that could interfere.",
                    "Network Connection Error"
                );

            var Response = JsonConvert.DeserializeObject<ApiResponse>(ApiResponse.Content);
            if (Response == null)
                return await ErrorResponse("Empty or invalid response from server.", "Login Error");

            if (Response.Status == VerifyLoginStatus.Success.ToString())
            {
                GlobalSettings.Options.Username = Response.Username;
                GlobalSettings.Options.Email = Response.Email;
                GlobalSettings.Options.Password = Response.Password;
                GlobalSettings.Options.SkinUrl = Response.Skin;
                return Response;
            }

            return await HandleLoginFailure(Response);
        }
        catch (Exception Error)
        {
            return await ErrorResponse(Error.Message, "Login Error");
        }
    }

    private static async Task<ApiResponse> HandleLoginFailure(ApiResponse Response)
    {
        switch (Response.Status)
        {
            case "Banned":
                await DialogService.ShowSimpleDialog($"You have been permanently banned from the {ProjectDefinitions.Name} Servers", "Failed to Sign In to Account");
                break;

            case "Deny":
                await DialogService.ShowSimpleDialog("Access Denied", "Error: Deny");
                break;

            case "Invalid":
                await DialogService.ShowSimpleDialog("Your email and/or password is invalid. To resolve this, reset your email and password to something simple, using only English alphabetic letters and numbers (without special characters), then log in again.", "Failed to Sign In to Account");
                break;

            case "Error":
                await DialogService.ShowSimpleDialog($"Unknown Error, Restart {ProjectDefinitions.Name} Launcher", "Error");
                break;

            case "OUTDATED":
                await DialogService.ShowSimpleDialog($"A new Launcher Update is available with New Features!\n\nCurrent Version: {Definitions.CurrentVersion}\nLatest Version: {Response.LatestVersion}\n\nTo continue playing:\n1. Close this Launcher & Uninstall completely\n2. Go to official {ProjectDefinitions.Name} Discord Server\n3. Download & Install new Launcher\n\nYou cannot play until you update to the latest version.", "Update Available!");
                break;

            case "Donator":
                await DialogService.ShowSimpleDialog("Eon is currently in testing and available exclusively for donators.\nPublic testing will be announced in the Discord. For now, only donators can play. Visit the server for details and support.", "Early Access Notice");
                break;
        }

        return Response;
    }

    private static async Task<ApiResponse> ErrorResponse(string Message, string Title)
    {
        await DialogService.ShowSimpleDialog(Message, Title);
        return new ApiResponse { Status = VerifyLoginStatus.Error.ToString() };
    }
}

public class ApiResponse
{
    public string Status { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string LatestVersion { get; set; }
    public string Skin { get; set; }
}
