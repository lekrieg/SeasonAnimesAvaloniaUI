// ////////////////////////
// File: AnimeService.cs
// Created at: 12 28, 2023
// Description:
// 
// Modified by: danie
// 12, 28, 2023
// ////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeasonAnimes.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SeasonAnimes.Services;

public class AnimeService
{
	private readonly HttpService _httpService;
	public Paging Paging { get; private set; }

	public AnimeService()
	{
		_httpService = new HttpService();
	}
	
	public async Task<IEnumerable<Anime>> SearchAsync(string uri)
	{
		var result = await _httpService.GetAsync(uri);

		var jObject = JObject.Parse(result);
		var animes = new List<Anime>();
		foreach (var item in jObject["data"])
		{
			var anime = JsonConvert.DeserializeObject<Anime>(item["node"].ToString());
			anime.CoverURL = item["node"]["main_picture"]["medium"].ToString();
			
			uri = $"https://api.myanimelist.net/v2/anime/{anime.Id}?fields=synopsis";
			var tmpResult = await _httpService.GetAsync(uri);
			var tmpJObject = JObject.Parse(tmpResult);
			anime.Description = tmpJObject["synopsis"].ToString();
			animes.Add(anime);
		}
		
		Paging = JsonConvert.DeserializeObject<Paging>(jObject["paging"].ToString());
		
		// TODO: guardar o paging anterior e o atual pra poder fazer o load nos botoes de ir e vir
		return animes;
	}

	public async Task<Stream> GetImage(string fileName, string uri)
	{
		return await _httpService.LoadCoverBitmapAsync(fileName, uri);
	}

	public async Task SaveAsync(Anime anime)
	{
		if (!Directory.Exists("./Cache"))
		{
			Directory.CreateDirectory("./Cache");
		}

		using (var fs = File.OpenWrite($"./Cache/{anime.Name}.json"))
		{
			await SaveToStreamAsync(anime, fs);
		}
	}

	public Stream SaveCoverBitmapStream(string fileName)
	{
		return File.OpenWrite($"./Cache/{fileName}.bmp");
	}

	private async Task SaveToStreamAsync(Anime anime, Stream stream)
	{
		await JsonSerializer.SerializeAsync(stream, anime).ConfigureAwait(false);
	}
}