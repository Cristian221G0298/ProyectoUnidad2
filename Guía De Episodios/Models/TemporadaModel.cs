using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guía_De_Episodios.Models
{
    public class TemporadaModel
    {
        public int Numero {  get; set; }
        public DateTime Fecha { get; set; }
        public int TotalEpisodios => Episodios.Count;
        public string Generos { get; set; } = null!;
        public string URLImagen { get; set; } = null!;
        public string Sinopsis { get; set; } = null!;
        public ObservableCollection<EpisodioModel> Episodios { get; set; } = new();
    }
}
