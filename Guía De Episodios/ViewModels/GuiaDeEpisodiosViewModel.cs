using GalaSoft.MvvmLight.Command;
using Guía_De_Episodios.Models;
using Guía_De_Episodios.Views;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Guía_De_Episodios.ViewModels
{
    public enum Vistas { ListaTem, AgregarTem, EditarTem, DetallesTem, EliminarTem, ListaEpi, AgregarEpi, EditarEpi, DetallesEpi, EliminarEpi }
    public class GuiaDeEpisodios : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<TemporadaModel> Temporadas { get; set; } = new();
        public IEnumerable<TemporadaModel> FiltroTemporadas { get; set; }
        public IEnumerable<EpisodioModel>? FiltroEpisodios { get; set; }
        public IEnumerable<TemporadaModel> OrdenNumero => Temporadas.OrderBy(x=>x.Numero);
        public TemporadaModel? Temporada { get; set; }
        public EpisodioModel? Episodio { get; set; }
        public Vistas Vista { get; set; } 
        public string Error { get; set; } = "";

        public ICommand AgregarTemCommand { get; set; }
        public ICommand EditarTemCommand { get; set; }
        public ICommand EliminarTemCommand { get; set; }
        public ICommand AgregarEpiCommand { get; set; }
        public ICommand EditarEpiCommand { get; set; }
        public ICommand EliminarEpiCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public ICommand FiltrarTemporadasCommand { get; set; }
        public ICommand FiltrarEpisodiosCommand { get; set; }

        public GuiaDeEpisodios()
        {
            AgregarTemCommand = new RelayCommand(AgregarTemporada);
            AgregarEpiCommand = new RelayCommand(AgregarEpisodio);
            EditarTemCommand = new RelayCommand(EditarTemporada);
            EditarEpiCommand = new RelayCommand(EditarEpisodio);
            EliminarTemCommand = new RelayCommand(EliminarTemporada);
            EliminarEpiCommand = new RelayCommand(EliminarEpisodio);
            CambiarVistaCommand = new RelayCommand<Vistas>(CambiarVista);
            FiltrarTemporadasCommand = new RelayCommand<string>(FiltrarTemporadas);
            FiltrarEpisodiosCommand = new RelayCommand<string>(FiltrarEpisodios);

            FiltroTemporadas = Temporadas;

            Deserializar();
        }

        int posicionAnterior;
        int numeroClon;
        string nombreClon = "";
        void CambiarVista(Vistas vista)
        {
            if(vista == Vistas.AgregarTem)
            {
                Temporada = new()
                {
                    Fecha = DateTime.Now.Date
                };
            }
            if(vista == Vistas.AgregarEpi)
            {
                Episodio = new();
            }

            if(vista == Vistas.EditarTem && Temporada != null)
            {
                var clon = new TemporadaModel
                {
                    Numero = Temporada.Numero,
                    Fecha = Temporada.Fecha,
                    Generos = Temporada.Generos,
                    URLImagen = Temporada.URLImagen,
                    Sinopsis = Temporada.Sinopsis,
                    Episodios = Temporada.Episodios
                };
                posicionAnterior = Temporadas.IndexOf(Temporada);
                numeroClon = Temporada.Numero;
                Temporada = clon;
            }

            if(vista == Vistas.EditarEpi && Episodio != null && Temporada != null)
            {
                var clon = new EpisodioModel
                {
                    NumeroE = Episodio.NumeroE,
                    Nombre = Episodio.Nombre,
                    Duracion = Episodio.Duracion,
                    Generos = Episodio.Generos,
                    URLImagen = Episodio.URLImagen,
                    SinopsisE = Episodio.SinopsisE
                };
                posicionAnterior = Temporada.Episodios.IndexOf(Episodio);
                numeroClon = Episodio.NumeroE;
                nombreClon = Episodio.Nombre;
                Episodio = clon;
            }

            if(vista == Vistas.ListaEpi && Temporada != null)
            {
                FiltroEpisodios = Temporada.Episodios;
            }

            Error = "";
            Actualizar(nameof(Error));

            Vista = vista;
            Actualizar(nameof(Vista));

            Actualizar(nameof(Temporada));
            Actualizar(nameof(Episodio));

            Actualizar(nameof(FiltroEpisodios));
        }
        void Serializar()
        {
            File.WriteAllText("Guia.json", JsonSerializer.Serialize(Temporadas));
        }
        void Deserializar()
        {
            if (File.Exists("Guia.json"))
            {
                var datos = JsonSerializer.Deserialize<ObservableCollection<TemporadaModel>>(File.ReadAllText("Guia.json"));

                if (datos != null)
                {
                    foreach (var t in datos)
                    {
                        Temporadas.Add(t);
                    }
                }
            }
        }
        void Actualizar(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
        void FiltrarTemporadas(string filtro)
        {
            switch(filtro)
            {
                case "Más reciente":
                    FiltroTemporadas = Temporadas.OrderBy(x => x.Fecha);
                    break;
                case "Más antigua":
                    FiltroTemporadas = Temporadas.OrderByDescending(x => x.Fecha);
                    break;
                default:
                    FiltroTemporadas = Temporadas;
                    break;
            }
        }
        void FiltrarEpisodios(string filtro)
        {
            if(Temporada != null)
            {
                switch (filtro)
                {
                    case "Primero - último":
                        FiltroEpisodios = Temporada.Episodios.OrderBy(x => x.NumeroE);
                        break;
                    case "Último - primero":
                        FiltroEpisodios = Temporada.Episodios.OrderByDescending(x => x.NumeroE);
                        break;
                    default:
                        FiltroEpisodios = Temporada.Episodios;
                        break;
                }
                Actualizar(nameof(FiltroEpisodios));
            }
        }

        void AgregarTemporada()
        {
            if(Temporada != null)
            {
                Error = "";

                if(Temporada.Numero < 1)
                {
                    Error += "-Verifica que el número de temporada sea correcto\n";
                }
                else if(Temporadas.Any(x=>x.Numero == Temporada.Numero))
                {
                    Error += "-Ya existe esta temporada\n";
                }
                if(Temporada.Fecha.Date > DateTime.Now.Date)
                {
                    Error += "-La fecha no puede ser futura\n";
                }
                if(OrdenNumero.Any(x => Temporada.Numero > x.Numero))
                {
                    var a = OrdenNumero.Last(x => Temporada.Numero > x.Numero);
                    if(Temporada.Fecha < a.Fecha)
                    {
                        Error += "-Verifica correctamente la fecha de la temporada\n";
                    }
                }
                if(OrdenNumero.Any(x => Temporada.Numero < x.Numero))
                {
                    var a = OrdenNumero.First(x => Temporada.Numero < x.Numero);
                    if (Temporada.Fecha > a.Fecha)
                    {
                        Error += "-Verifica correctamente la fecha de la temporada\n";
                    }
                }
                if(string.IsNullOrWhiteSpace(Temporada.Generos))
                {
                    Error += "-Añade los géneros de la temporada\n";
                }
                if (string.IsNullOrWhiteSpace(Temporada.URLImagen))
                {
                    Error += "-Escribe la URL de la imagen de la temporada\n";
                }
                else if (!Temporada.URLImagen.StartsWith("http") || !Temporada.URLImagen.EndsWith(".jpg"))
                {
                    Error += "-Escriba la URL de la imagen en formato JPG\n";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Sinopsis))
                {
                    Error += "-Añade una sinópsis de la temporada\n";
                }
                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Temporadas.Add(Temporada);
                    Serializar();
                    CambiarVista(Vistas.ListaTem);
                }
            }
            
        }
        void AgregarEpisodio()
        {
            if(Episodio != null && Temporada != null)
            {
                Error = "";
                if (Episodio.NumeroE < 1)
                {
                    Error += "-Verifica que el número del episodio sea correcto\n";
                }
                else if(Temporada.Episodios.Any(x=>x.NumeroE == Episodio.NumeroE))
                {
                    Error += "-El número de episodio ya existe en esta temporada\n";
                }
                if(string.IsNullOrWhiteSpace(Episodio.Nombre))
                {
                    Error += "-El nombre del episodio no puede ir vacío\n";
                }
                else if(Temporadas.Any(x=>x.Episodios.Any(x=>x.Nombre.ToLower() == Episodio.Nombre.ToLower())))
                {
                    Error += "-Ya existe un episodio con este nombre\n";
                }
                if(Episodio.Duracion < 1)
                {
                    Error += "-Verifica que la duración sea correcta\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Generos))
                {
                    Error += "-Añade los géneros del episodio\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLImagen))
                {
                    Error += "-Escribe la URL de la imagen del episodio\n";
                }
                else if (!Episodio.URLImagen.StartsWith("http") || !Episodio.URLImagen.EndsWith(".jpg"))
                {
                    Error += "-Escriba la URL de la imagen en formato JPG\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.SinopsisE))
                {
                    Error += "-Añade una sinópsis del episodio\n";
                }
                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Temporada.Episodios.Add(Episodio);
                    Serializar();
                    CambiarVista(Vistas.ListaEpi);
                }
            }
        }

        void EditarTemporada()
        {
            if (Temporada != null)
            {
                Error = "";

                if (Temporada.Numero < 1)
                {
                    Error += "-Verifica que el número de temporada sea correcto\n";
                }
                else if (Temporadas.Any(x => x.Numero == Temporada.Numero && Temporada.Numero != numeroClon))
                {
                    Error += "-Ya existe esta temporada\n";
                }
                if (Temporada.Fecha.Date > DateTime.Now.Date)
                {
                    Error += "-La fecha no puede ser futura\n";
                }
                if (OrdenNumero.Any(x => Temporada.Numero > x.Numero))
                {
                    var a = OrdenNumero.Last(x => Temporada.Numero > x.Numero);
                    if (Temporada.Fecha < a.Fecha)
                    {
                        Error += "-Verifica correctamente la fecha de la temporada\n";
                    }
                }
                if (OrdenNumero.Any(x => Temporada.Numero < x.Numero))
                {
                    var a = OrdenNumero.First(x => Temporada.Numero < x.Numero);
                    if (Temporada.Fecha > a.Fecha)
                    {
                        Error += "-Verifica correctamente la fecha de la temporada\n";
                    }
                }
                if (string.IsNullOrWhiteSpace(Temporada.Generos))
                {
                    Error += "-Añade los géneros de la temporada\n";
                }
                if (string.IsNullOrWhiteSpace(Temporada.URLImagen))
                {
                    Error += "-Escribe la URL de la imagen de la temporada\n";
                }
                else if (!Temporada.URLImagen.StartsWith("http") || !Temporada.URLImagen.EndsWith(".jpg"))
                {
                    Error += "-Escriba la URL de la imagen en formato JPG\n";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Sinopsis))
                {
                    Error += "-Añade una sinópsis de la temporada\n";
                }
                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Temporadas[posicionAnterior] = Temporada;
                    Serializar();
                    CambiarVista(Vistas.ListaTem);
                }
            }
        }
        void EditarEpisodio()
        {
            if (Episodio != null && Temporada != null)
            {
                Error = "";
                if (Episodio.NumeroE < 1)
                {
                    Error += "-Verifica que el número del episodio sea correcto\n";
                }
                else if (Temporada.Episodios.Any(x => x.NumeroE == Episodio.NumeroE && Episodio.NumeroE != numeroClon))
                {
                    Error += "-El número de episodio ya existe en esta temporada\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Nombre))
                {
                    Error += "-El nombre del episodio no puede ir vacío\n";
                }
                else if (Temporadas.Any(x => x.Episodios.Any(x => x.Nombre.ToLower() == Episodio.Nombre.ToLower() && Episodio.Nombre.ToLower() != nombreClon.ToLower())))
                {
                    Error += "-Ya existe un episodio con este nombre\n";
                }
                if (Episodio.Duracion < 1)
                {
                    Error += "-Verifica que la duración sea correcta\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Generos))
                {
                    Error += "-Añade los géneros del episodio\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLImagen))
                {
                    Error += "-Escribe la URL de la imagen del episodio\n";
                }
                else if (!Episodio.URLImagen.StartsWith("http") || !Episodio.URLImagen.EndsWith(".jpg"))
                {
                    Error += "-Escriba la URL de la imagen en formato JPG\n";
                }
                if (string.IsNullOrWhiteSpace(Episodio.SinopsisE))
                {
                    Error += "-Añade una sinópsis del episodio\n";
                }
                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Temporada.Episodios[posicionAnterior] = Episodio;
                    Serializar();
                    CambiarVista(Vistas.ListaEpi);
                }
            }
        }

        void EliminarTemporada()
        {
            if(Temporada != null)
            {
                Temporadas.Remove(Temporada);
                Serializar();
                CambiarVista(Vistas.ListaTem);
            }
        }
        void EliminarEpisodio()
        {
            if (Episodio != null && Temporada != null)
            {
                Temporada.Episodios.Remove(Episodio);
                Serializar();
                CambiarVista(Vistas.ListaEpi);
            }
        }
    }
}
