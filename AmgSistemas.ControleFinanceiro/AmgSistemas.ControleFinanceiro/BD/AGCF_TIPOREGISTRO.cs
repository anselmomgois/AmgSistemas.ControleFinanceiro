//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AmgSistemas.ControleFinanceiro.BD
{
    using System;
    using System.Collections.Generic;
    
    public partial class AGCF_TIPOREGISTRO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AGCF_TIPOREGISTRO()
        {
            this.AGCF_REGISTRO = new HashSet<AGCF_REGISTRO>();
        }
    
        public string IDTIPOREGISTRO { get; set; }
        public string IDUSUARIO { get; set; }
        public string DESTIPOREGISTRO { get; set; }
        public string CODTIPOREGISTRO { get; set; }
        public string CODCATEGORIAGERAL { get; set; }
        public Nullable<bool> BOLPARCELAMENTO { get; set; }
        public string CODTIPOREGISTROORIGINAL { get; set; }
        public Nullable<bool> BOLSALARIO { get; set; }
    
        public virtual AGCF_USUARIO AGCF_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AGCF_REGISTRO> AGCF_REGISTRO { get; set; }
    }
}