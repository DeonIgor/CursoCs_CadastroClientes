//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CadastroClientes
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClientSet
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    
        public virtual AddressSet AddressSet { get; set; }
    }
}
