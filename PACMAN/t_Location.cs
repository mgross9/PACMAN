//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PACMAN
{
    using System;
    using System.Collections.Generic;
    
    public partial class t_Location
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_Location()
        {
            this.t_Flights = new HashSet<t_Flights>();
            this.t_Flights1 = new HashSet<t_Flights>();
            this.t_History = new HashSet<t_History>();
            this.t_Sensor_Data = new HashSet<t_Sensor_Data>();
        }
    
        public int Id { get; set; }
        public string ICDO_Code { get; set; }
        public string Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_Flights> t_Flights { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_Flights> t_Flights1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_History> t_History { get; set; }
        public virtual t_Pollutant_Deposition t_Pollutant_Deposition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_Sensor_Data> t_Sensor_Data { get; set; }
    }
}
