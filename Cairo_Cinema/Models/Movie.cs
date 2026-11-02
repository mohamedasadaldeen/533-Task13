using Cairo_Cinema.CustomValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cairo_Cinema.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [MaxMinLength]
        public string Name { get; set; }
        [MaxLength(100)]
        public string? Description { get; set; }
        [Range(100,500)]
        public decimal Price { get; set; }
        [Range(1,5)]
        public decimal Rate { get; set; }
        public bool Status{ get; set; }
        public DateTime DateTime{ get; set; }
        public string? MainImage { get; set; } = "default.jpg";

        [ForeignKey(nameof(ActorId))]
        public int ActorId { get; set; }
        public Actor? Actor { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey(nameof(CinemaId))]
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }

    }
}
