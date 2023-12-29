// ////////////////////////
// File: AnimeInfoViewModel.cs
// Created at: 12 27, 2023
// Description:
// 
// Modified by: danie
// 12, 27, 2023
// ////////////////////////

using Avalonia.Media.Imaging;
using ReactiveUI;

namespace SeasonAnimes.ViewModels;

public class AnimeInfoViewModel : ViewModelBase
{
	private string _name;
	public string Name
	{
		get => _name;
		set => this.RaiseAndSetIfChanged(ref _name, value);
	}
	
	private string _description;
	public string Description
	{
		get => _description;
		set => this.RaiseAndSetIfChanged(ref _description, value);
	}
	
	private Bitmap? _cover;
	public Bitmap? Cover
	{
		get => _cover;
		private set => this.RaiseAndSetIfChanged(ref _cover, value);
	}
	
	public AnimeViewModel AnimeViewModel { get; set; }

	public AnimeInfoViewModel(AnimeViewModel animeViewModel)
	{
		if (animeViewModel == null)
		{
			return;
		}
		
		AnimeViewModel = animeViewModel;

		LoadCover();

		SetData();
	}

	private async void LoadCover()
	{
		await AnimeViewModel.LoadCover();
	}

	private void SetData()
	{
		Name = AnimeViewModel.Name;
		Description = AnimeViewModel.Description;
		Cover = AnimeViewModel.Cover;
	}
}