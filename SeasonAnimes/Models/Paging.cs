// ////////////////////////
// File: Paging.cs
// Created at: 12 29, 2023
// Description:
// 
// Modified by: danie
// 12, 29, 2023
// ////////////////////////

using Newtonsoft.Json;

namespace SeasonAnimes.Models;

public class Paging
{
	[JsonProperty("previous")]
	public string Previous { get; set; }
	[JsonProperty("next")]
	public string Next { get; set; }
}