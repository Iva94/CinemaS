﻿//------------------------------------------------------------------------------
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

    public partial class Movie
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Movie()
        {
            this.Projections = new HashSet<Projection>();
        }
    
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Unesite naziv filma!")]
        [Display(Name = "Naziv filma")]
        public string MovieTitle { get; set; }

        [Required(ErrorMessage = "Unesite originalni naziv filma!")]
        [Display(Name = "Originalni naziv")]
        public string OriginalTitle { get; set; }

        [Required(ErrorMessage = "Unesite datum premjere filma!")]
        [Display(Name = "Datum premjere")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> PremiereDate { get; set; }

        [Display(Name = "Režiser")]
        public string Director { get; set; }

        [Display(Name = "Glumci")]
        public string Actors { get; set; }

        [Required(ErrorMessage = "Unesite sinopsis!")]
        [Display(Name = "Sinopsis")]
        public string Synopsis { get; set; }

        [Display(Name = "Trajanje")]
        public string Duration { get; set; }

        [Display(Name = "Poster")]
        public string Image { get; set; }

        [Display(Name = "Video")]
        public string Video { get; set; }

        [Required(ErrorMessage = "Označite da li se radi o najavi!")]
        [Display(Name = "Da li je najava filma?")]
        public Nullable<bool> IsAnnouncement { get; set; }

        [Display(Name = "Naslovna")]
        public string CoverImage { get; set; }

        [Required(ErrorMessage = "Unesite žanr filma!")]
        [Display(Name = "Žanr")]
        public Nullable<int> Genre { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projection> Projections { get; set; }
        public virtual Genre Genre1 { get; set; }
    }
}