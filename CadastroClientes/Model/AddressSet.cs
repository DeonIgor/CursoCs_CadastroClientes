namespace CadastroClientes
{
    public partial class AddressSet
    {
        override
        public string ToString()
        {
            return $"Endereço: {Street}, {Number} - {City}" +
                $"\nBairro: {Neighborhood}" +
                $"\nCEP: {CEP}" +
                $"\nComplemento: {AdditionalInfo}";
        }
    }
}
