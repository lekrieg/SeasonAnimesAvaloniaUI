// ////////////////////////
// File: AnimeViewModel.cs
// Created at: 12 27, 2023
// Description:
// 
// Modified by: danie
// 12, 27, 2023
// ////////////////////////

using System.Threading.Tasks;
using ReactiveUI;
using SeasonAnimes.Models;
using SeasonAnimes.Services;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace SeasonAnimes.ViewModels;

public class AnimeViewModel : ViewModelBase
{
	public string Name => _anime.Name;
	public string Description => _anime.Description;
	
	private readonly Anime _anime;

	private Bitmap? _cover;
	public Bitmap? Cover
	{
		get => _cover;
		private set => this.RaiseAndSetIfChanged(ref _cover, value);
	}

	private readonly AnimeService _animeService;
	
	public AnimeViewModel(Anime anime)
	{
		_animeService = new AnimeService();
		
		_anime = anime;
	}

	public async Task LoadCover()
	{
		await using (var imageStream = await _animeService.GetImage(_anime.Name, _anime.CoverURL))
		{
			Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
		}
	}

	public async Task SaveToDiskAsync()
	{
		await _animeService.SaveAsync(_anime);

		if (Cover != null)
		{
			var bitmap = Cover;
			await Task.Run(() =>
			{
				using (var fs = _animeService.SaveCoverBitmapStream(_anime.Name))
				{
					bitmap.Save(fs);
				}
			});
		}
	}
}