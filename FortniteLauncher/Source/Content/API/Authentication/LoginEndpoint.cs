using System;
using RestSharp;
using Newtonsoft.Json;
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
                return await ErrorResponse("Please check your network connection or try again later. Make sure you're on a stable network without a VPN, mobile cellular data, or any network restrictions like firewalls or proxy settings that could interfere.", "Network Connection Error");

            var Response = JsonConvert.DeserializeObject<ApiResponse>(ApiResponse.Content);
            if (Response == null)
                return await ErrorResponse("Empty or invalid response from server.", "Login Error");

            if (Response.Status == VerifyLoginStatus.Success.ToString())
            {
                GlobalSettings.Options.Username = Response.Username;
                GlobalSettings.Options.Email = Response.Email;
                GlobalSettings.Options.SkinUrl = Response.Skin;
                return Response;
            }

            return Response;
        }
        catch (Exception Error)
        {
            return await ErrorResponse(Error.Message, "Login Error");
        }
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
    public string Skin { get; set; }
}
