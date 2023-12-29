// ////////////////////////
// File: Anime.cs
// Created at: 12 28, 2023
// Description:
// 
// Modified by: danie
// 12, 28, 2023
// ////////////////////////

using Newtonsoft.Json;

namespace SeasonAnimes.Models;

public class Anime
{
	[JsonProperty("id")]
	public int Id { get; set; }

	[JsonProperty("title")]
	public string Name { get; set; }
	[JsonProperty("synopsis")]
	public string Description { get; set; }
	[JsonProperty("medium")]
	public string CoverURL { get; set; }
}