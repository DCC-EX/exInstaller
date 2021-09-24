using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace exInstaller.ViewModels
{
    public class WizardViewModel : ViewModelBase, IDisposable
    {
        private IMainWindowViewModel mwViewModel;

        #region Bindings
        private int _currentPage;
           public int CurrentPage
        {
            get { return _currentPage; }
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }
        #endregion


        public WizardViewModel(IMainWindowViewModel mwViewModel)
        {
            this.mwViewModel = mwViewModel;
            CurrentPage = 1;
        }

        public void Dispose()
        {
            mwViewModel.Status += "Wizard closing";
        }

        public ReactiveCommand<Unit, Unit> NextButton
        {
            get;
        }

        void NextPage()
        {
            CurrentPage += 1;
        }

        public ReactiveCommand<Unit, Unit> PreviousButton
        {
            get;
        }

        void PreviousPage()
        {
            CurrentPage -= 1;
        }
    }
}
