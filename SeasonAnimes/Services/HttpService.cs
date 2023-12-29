// ////////////////////////
// File: HttpService.cs
// Created at: 12 28, 2023
// Description:
// Code from: https://stackoverflow.com/questions/27108264/how-to-properly-make-a-http-web-get-request
// Modified by: danie
// 12, 28, 2023
// ////////////////////////

using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SeasonAnimes.Services;

public class HttpService
{
	private const string CLIENT_ID = "d12ba4518806e3a371c980a88035cf00";
	
	private readonly HttpClient _client;

	public HttpService()
	{
		HttpClientHandler handler = new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.All
		};

		_client = new HttpClient();
	}

	public async Task<string> GetAsync(string uri)
	{
		using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
		if (!requestMessage.Headers.Contains("X-MAL-CLIENT-ID"))
		{
			requestMessage.Headers.Add("X-MAL-CLIENT-ID", CLIENT_ID);
		}
		using HttpResponseMessage response = await _client.SendAsync(requestMessage);
		return await response.Content.ReadAsStringAsync();
	}

	public async Task<Stream> LoadCoverBitmapAsync(string fileName, string uri)
	{
		string cacheFilePath = $"./Cache/{fileName}.bmp";
		if (File.Exists(cacheFilePath))
		{
			return File.OpenRead(cacheFilePath);
		}
		else
		{
			using var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
			if (!requestMessage.Headers.Contains("X-MAL-CLIENT-ID"))
			{
				requestMessage.Headers.Add("X-MAL-CLIENT-ID", CLIENT_ID);
			}
			
			var response = await _client.GetByteArrayAsync(requestMessage.RequestUri);
			return new MemoryStream(response);
		}
	}
}