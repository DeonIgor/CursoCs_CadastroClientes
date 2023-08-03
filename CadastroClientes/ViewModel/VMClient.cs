using CadastroClientes.Repositories;
using CadastroClientes.Services;
using Frameworks.MVVM.Bindable;
using Frameworks.MVVM.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CadastroClientes.ViewModel
{
    public class VMClient : Bindable
    {
        private ClientRepository clientRepository;
        internal IWindowServices windowServices;

        private ObservableCollection<ClientSet> clients;
        private ClientSet selectedClient;
        private AddressSet selectedAddress;
        private int selectedIndex;
        private bool isNotEditing;
        private string searchString;

        public Command NewCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command EditCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command SaveFileCommand { get; set; }

        public ObservableCollection<ClientSet> Clients
        {
            get { return clients; }
            set
            {
                clients = value;
                OnPropertyChanged(nameof(Clients));
                OnPropertyChanged(nameof(ClientResultList));
            }
        }
        public ClientSet SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(SelectedAddress));
            }
        }
        public AddressSet SelectedAddress
        {
            get { return selectedAddress; }
            set
            {
                selectedAddress = value;
                OnPropertyChanged(nameof(SelectedClient));
                OnPropertyChanged(nameof(SelectedAddress));
            }
        }
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    if (selectedIndex == -1)
                    {
                        ClearInput();
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(SearchString) || SearchString == "Pesquisar...")
                        {
                            SelectedClient = clients[selectedIndex];
                            SelectedAddress = clients[selectedIndex].AddressSet;
                        }
                        else
                        {
                            ObservableCollection<ClientSet> cs = new ObservableCollection<ClientSet>(clients.Where(c => c.Name.ToLower().Contains(SearchString.ToLower())));
                            SelectedClient = cs[selectedIndex];
                            SelectedAddress = cs[selectedIndex].AddressSet;
                        }
                        IsNotEditing = true;
                    }
                    OnPropertyChanged(nameof(SelectedIndex));
                    OnPropertyChanged(nameof(ClientSelected));
                    OnPropertyChanged(nameof(ClientNotSelected));
                    OnPropertyChanged(nameof(SelectedAddressNumberString));
                }
            }
        }
        public bool IsNotEditing
        {
            get { return isNotEditing; }
            set
            {
                isNotEditing = value;
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }
        public bool ClientSelected
        {
            get { return SelectedIndex != -1; }
        }
        public bool ClientNotSelected
        {
            get { return SelectedIndex == -1 && (String.IsNullOrEmpty(searchString) || searchString == "Pesquisar..."); }
        }
        public string SelectedAddressNumberString
        {
            get
            {
                if (SelectedAddress == null) return "";
                if (String.IsNullOrEmpty(SelectedAddress.Street)) return "";
                if (SelectedAddress.Number == 0) return "";
                return SelectedAddress.Number.ToString();
            }
            set
            {
                SelectedAddress.Number = int.Parse(value);
                OnPropertyChanged(nameof(SelectedAddressNumberString));
            }
        }
        public string SearchString
        {
            get { return searchString; }
            set
            {
                searchString = value;
                CancelClient();
                OnPropertyChanged(nameof(SearchString));
                Clients = new ObservableCollection<ClientSet>(Clients);
            }
        }
        public ObservableCollection<ClientSet> ClientResultList
        {
            get
            {
                IEnumerable<ClientSet> cs = new List<ClientSet>(Clients);

                if (!String.IsNullOrEmpty(SearchString))
                {
                    if (SearchString != "Pesquisar...")
                    {
                        cs = cs.Where(c => c.Name.ToLower().Contains(SearchString.ToLower()));
                    }
                }

                return new ObservableCollection<ClientSet>(cs);
            }
        }

        public VMClient()
        {
            clientRepository = new ClientRepository(new EntitiesContext());
            Clients = clientRepository.GetAllClients();

            try
            {
                foreach (ClientSet c in Clients)
                {
                    foreach (AddressSet a in clientRepository.GetAllAddresses())
                    {
                        if (a.Id == c.AddressSet.Id)
                        {
                            c.AddressSet = a;
                        }
                    }
                }
            }
            catch
            {
                Clients = new ObservableCollection<ClientSet>();
            }

            IsNotEditing = false;
            SelectedIndex = -1;
            SearchString = "";

            SelectedAddress = new AddressSet();
            SelectedClient = new ClientSet();

            NewCommand = new Command(NewClient);
            DeleteCommand = new Command(DeleteClient);
            EditCommand = new Command(EditClient);
            CancelCommand = new Command(CancelClient);
            SaveFileCommand = new Command(SaveFile);
        }

        public void NewClient()
        {
            SelectedClient.AddressSet = SelectedAddress;

            SelectedClient = clientRepository.Add(SelectedClient);

            Clients.Add(SelectedClient);

            ClearInput();
        }

        private void DeleteClient()
        {
            int index = SelectedIndex;
            ClientSet c = Clients[index];

            Clients.RemoveAt(index);

            clientRepository.Delete(c);

            ClearInput();
        }

        private void EditClient()
        {
            IsNotEditing = !isNotEditing;
        }

        public void SaveClient()
        {
            ClientSet c = SelectedClient;

            Clients = new ObservableCollection<ClientSet>(Clients);

            clientRepository.Update(c);

            CancelClient();
        }

        private void CancelClient()
        {
            SelectedIndex = -1;
            OnPropertyChanged(nameof(ClientNotSelected));
            OnPropertyChanged(nameof(ClientSelected));
            OnPropertyChanged(nameof(ClientResultList));
        }

        public void SaveFile()
        {
            FileInfo fileInfo = windowServices.SaveFileDialog();
            if (fileInfo == null) return;

            using (StreamWriter writer = new StreamWriter(fileInfo.FullName))
            {
                if (ClientSelected)
                {
                    writer.WriteLine(SelectedClient.ToStringFile());
                }
                else
                {
                    foreach (ClientSet client in Clients)
                    {
                        writer.WriteLine(client.ToStringFile());
                        writer.WriteLine();
                    }
                }
            }
        }

        private void ClearInput()
        {
            SelectedAddress = new AddressSet();
            SelectedClient = new ClientSet();
            IsNotEditing = false;
            OnPropertyChanged(nameof(SelectedAddressNumberString));
            OnPropertyChanged(nameof(ClientResultList));

        }

        public bool RequiredFieldsFilled(string name, string email, string street, string number, string cep, string city)
        {
            return String.IsNullOrEmpty(name) == false
                && String.IsNullOrEmpty(email) == false
                && String.IsNullOrEmpty(street) == false
                && String.IsNullOrEmpty(cep) == false
                && String.IsNullOrEmpty(city) == false
                && String.IsNullOrEmpty(number) == false;
        }
    }
}
