using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using ReactiveUI;
using SeasonAnimes.Enums;
using SeasonAnimes.Models;
using SeasonAnimes.Services;

namespace SeasonAnimes.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public ICommand SearchCommand { get; private set; }
	public ICommand LeftCommand { get; private set; }
	public ICommand RightCommand { get; private set; }

	public Interaction<AnimeInfoViewModel, AnimeViewModel?> ShowDialog { get; }

	private string? _yearText;
	public string? YearText
	{
		get => _yearText;
		set => this.RaiseAndSetIfChanged(ref _yearText, value);
	}

	private bool _isBusy;
	public bool IsBusy
	{
		get => _isBusy;
		set => this.RaiseAndSetIfChanged(ref _isBusy, value);
	}

	private int _seasonIndex = 0;
	public int SeasonIndex
	{
		get => _seasonIndex;
		set => this.RaiseAndSetIfChanged(ref _seasonIndex, value);
	}

	private AnimeViewModel? _selectedAnime;
	public AnimeViewModel? SelectedAnime
	{
		get => _selectedAnime;
		set => this.RaiseAndSetIfChanged(ref _selectedAnime, value);
	}

	public ObservableCollection<AnimeViewModel> SearchResults { get; } = new ObservableCollection<AnimeViewModel>();

	private CancellationTokenSource? _cancellationTokenSource;
	private readonly AnimeService _animeService;
	
	private Paging _paging;
	private bool IsRight;
	private bool? _isButtonActive;
	public bool? IsButtonActive
	{
		get => _isButtonActive;
		set => this.RaiseAndSetIfChanged(ref _isButtonActive, value);
	}
	
	public MainWindowViewModel()
	{
		_isButtonActive = false;
		
		_animeService = new AnimeService();
		
		SetupButtonCommands();

		var openInnerWindowCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			var animeInfo = new AnimeInfoViewModel(SelectedAnime);
			await ShowDialog.Handle(animeInfo);
			
			await SelectedAnime.SaveToDiskAsync();
			SelectedAnime = null;
		});
		this.WhenAnyValue(x => x.SelectedAnime).Subscribe(x =>
		{
			if (x != null)
			{
				openInnerWindowCommand.Execute();
			}
		});
		
		ShowDialog = new Interaction<AnimeInfoViewModel, AnimeViewModel?>();
		
		RxApp.MainThreadScheduler.Schedule(() =>
		{
			YearText = "" + DateTime.Today.Year;
			DoSearch($"{YearText}");
		});
	}

	private async void DoSearch(string s)
	{
		IsBusy = true;
		SearchResults.Clear();

		_cancellationTokenSource?.Cancel();
		_cancellationTokenSource = new CancellationTokenSource();
		var cancellationToken = _cancellationTokenSource.Token;
		
		if (!string.IsNullOrWhiteSpace(s))
		{
			if (int.TryParse(s, out int year))
			{
				var seasonName = Enum.GetName(typeof(Seasons), SeasonIndex);
				string uri = String.Empty;

				if (_paging == null)
				{
					uri = $"https://api.myanimelist.net/v2/anime/season/{year}/{seasonName.ToLower()}?limit={10}";
				}
				else
				{
					if (IsRight)
					{
						uri = _paging.Next;
						IsButtonActive = true;
					}
					else
					{
						if (_paging.Previous == null)
						{
							IsButtonActive = false;
							uri = $"https://api.myanimelist.net/v2/anime/season/{year}/{seasonName.ToLower()}?limit={10}";
						}
						else
						{
							IsButtonActive = true;
							uri = _paging.Previous;
						}
						
					}
				}

				var animes = await _animeService.SearchAsync(uri);
				_paging = _animeService.Paging;
				
				foreach (var anime in animes)
				{
					var tmpVm = new AnimeViewModel(anime);
					SearchResults.Add(tmpVm);
				}
				
				if (!cancellationToken.IsCancellationRequested)
				{
					LoadCovers(cancellationToken);
				}
			}
			else
			{
				throw new Exception("[DoSearch] Year is invalid!");
			}
		}
		
		IsBusy = false;
	}

	private void SetupButtonCommands()
	{
		SearchCommand = ReactiveCommand.Create(() =>
		{
			this.WhenAnyValue(x => x.YearText)
				.Throttle(TimeSpan.FromMilliseconds(400))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(DoSearch!);
		});
		
		LeftCommand = ReactiveCommand.Create(() =>
		{
			IsRight = false;
			this.WhenAnyValue(x => x.YearText)
				.Throttle(TimeSpan.FromMilliseconds(400))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(DoSearch!);
		});
		
		RightCommand = ReactiveCommand.Create(() =>
		{
			IsRight = true;
			this.WhenAnyValue(x => x.YearText)
				.Throttle(TimeSpan.FromMilliseconds(400))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(DoSearch!);
		});
	}

	private async void LoadCovers(CancellationToken cancellationToken)
	{
		foreach (var anime in SearchResults.ToList())
		{
			await anime.LoadCover();

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
	}
}