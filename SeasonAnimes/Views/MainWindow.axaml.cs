using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ReactiveUI;
using SeasonAnimes.ViewModels;

namespace SeasonAnimes.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
	public MainWindow()
	{
		InitializeComponent();

		this.WhenActivated(action => action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
	}

	private async Task DoShowDialogAsync(InteractionContext<AnimeInfoViewModel, AnimeViewModel?> interaction)
	{
		var dialog = new AnimeInfoWindow();
		dialog.DataContext = interaction.Input;

		var result = await dialog.ShowDialog<AnimeViewModel?>(this);
		interaction.SetOutput(result);
	}
}