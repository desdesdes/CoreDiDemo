using System.Net.Http.Json;

namespace Afas.Bvr.Crm;

public abstract class PhonenumberChecker
{
  public abstract Task<bool> CheckPhoneNumber(string? phoneNumber);
}

public class WebPhonenumberChecker : PhonenumberChecker
{
  private readonly HttpClient _httpClient;

  public WebPhonenumberChecker(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public override async Task<bool> CheckPhoneNumber(string? phoneNumber)
  {
    return !string.IsNullOrEmpty(phoneNumber) &&
      await _httpClient.GetFromJsonAsync<bool>($"https://coredidemo.azurewebsites.net/phonenumbercheck.html?number={Uri.EscapeDataString(phoneNumber)}");
  }
}
