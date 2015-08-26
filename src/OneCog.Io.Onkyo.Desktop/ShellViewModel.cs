using Caliburn.Micro;

namespace OneCog.Io.Onkyo.Desktop 
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IShell     
    {
        public ShellViewModel()
        {
            Items.Add(new TxNr616ViewModel());
            Items.Add(new TxNr929ViewModel());
        }
    }
}