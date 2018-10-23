//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class Auditorium
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auditorium()
        {
            this.Projections = new HashSet<Projection>();
            this.Seats = new HashSet<Seat>();
        }
    
        public int AuditoriumId { get; set; }

        [Required(ErrorMessage = "Unesite oznaku sale!")]
        [Display(Name = "Oznaka sale")]
        public string AuditoriumName { get; set; }

        [Required(ErrorMessage = "Unesite kapacitet sale!")]
        [Display(Name = "Kapacitet sale")]
        public Nullable<int> Capacity { get; set; }

        [Required(ErrorMessage = "Unesite broj redova!")]
        [Display(Name = "Broj redova")]
        public Nullable<int> NumberOfRows { get; set; }

        public Nullable<int> NumberOfColumns { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projection> Projections { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Seat> Seats { get; set; }
    }
}
