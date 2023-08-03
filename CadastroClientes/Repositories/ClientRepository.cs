using System;
using System.Collections.ObjectModel;

namespace CadastroClientes.Repositories
{
    internal class ClientRepository
    {
        private EntitiesContext context;

        public ClientRepository(EntitiesContext dbContext)
        {
            context = dbContext;
        }

        public ClientSet Add(ClientSet client)
        {
            ClientSet c = context.ClientSet.Create();
            AddressSet a = context.AddressSet.Create();

            c.Id = Guid.NewGuid();
            c.Name = client.Name;
            c.Email = client.Email;
            c.PhoneNumber = client.PhoneNumber;

            a.Id = Guid.NewGuid();
            a.Street = client.AddressSet.Street;
            a.Number = client.AddressSet.Number;
            a.AdditionalInfo = client.AddressSet.AdditionalInfo;
            a.Neighborhood = client.AddressSet.Neighborhood;
            a.City = client.AddressSet.City;
            a.CEP = client.AddressSet.CEP;

            c.AddressSet = a;

            context.ClientSet.Add(c);
            context.SaveChanges();

            return c;
        }

        public void Update(ClientSet client)
        {
            ClientSet c = context.ClientSet.Find(client.Id);

            c.Name = client.Name;
            c.Email = client.Email;
            c.PhoneNumber = client.PhoneNumber;
            c.AddressSet = client.AddressSet;

            context.SaveChanges();
        }

        public void Delete(ClientSet client)
        {
            ClientSet c = GetClient(client.Id);
            AddressSet a = c.AddressSet;

            c.AddressSet = null;
            a.ClientSet = null;

            if (c != null && a != null)
            {
                context.AddressSet.Remove(a);
                context.ClientSet.Remove(c);
                context.SaveChanges();
            }
        }

        public ClientSet GetClient(Guid id)
        {
            return context.ClientSet.Find(id);
        }

        public ObservableCollection<ClientSet> GetAllClients()
        {
            return new ObservableCollection<ClientSet>(context.ClientSet);
        }

        public ObservableCollection<AddressSet> GetAllAddresses()
        {
            return new ObservableCollection<AddressSet>(context.AddressSet);
        }
    }
}
