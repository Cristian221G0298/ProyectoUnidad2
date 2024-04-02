using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guía_De_Episodios.Models
{
    public class EpisodioModel
    {
        public int NumeroE {  get; set; }
        public string Nombre { get; set; } = null!;
        public int Duracion { get; set; }
        public string Generos { get; set; } = null!;
        public string URLImagen {  get; set; } = null!;
        public string SinopsisE {  get; set; } = null!;
    }
}
