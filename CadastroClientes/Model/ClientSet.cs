namespace CadastroClientes
{
    public partial class ClientSet
    {
        override
        public string ToString()
        {
            return $"{Name} - {Email}";
        }

        public string ToStringFile()
        {
            return $"Nome: {Name}" +
                $"\nE-mail: {Email}" +
                $"\nTelefone: {PhoneNumber}" +
                $"\n{AddressSet}";
        }
    }
}
