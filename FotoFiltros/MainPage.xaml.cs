using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FotoFiltros.Resources;
using Lumia.Imaging;
using System.Windows.Media.Imaging;
using System.Reflection;
using Microsoft.Phone.Tasks;
using Lumia.Imaging.Artistic;
using Microsoft.Xna.Framework.Media;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;



namespace FotoFiltros
{
    public partial class MainPage : PhoneApplicationPage
    {
        // La instancia Filtereffect se utiliza para aplicar diferentes filtros a una imagen. 
        // Aquí vamos a aplicar el filtro de la  imagen.
        private FilterEffect _fotoEffect = null;

        // La siguiente WriteableBitmap contiene la imagen filtro y una miniatura.
        private WriteableBitmap _effectImageBitmap = null;
        private WriteableBitmap _thumbnailImageBitmap = null;
        WriteableBitmap rendererefect;



        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // Inicializamos WriteableBitmaps con los tamaños de la imagen filtrada y original.
            _effectImageBitmap = new WriteableBitmap((int)ImagenEfecto.Width, (int)ImagenEfecto.Height);
            _thumbnailImageBitmap = new WriteableBitmap((int)ImagenOriginal.Width, (int)ImagenOriginal.Height);
    
        }


        private async void PickImageCallback(object sender, PhotoResult e)
        {
            if (e.TaskResult != TaskResult.OK || e.ChosenPhoto == null)
                return;

            try
            {
                // mostramos el thumbnail de la imagen original.
                _thumbnailImageBitmap.SetSource(e.ChosenPhoto);
                ImagenOriginal.Source = _thumbnailImageBitmap;

                e.ChosenPhoto.Position = 0;

                // inicializamos lo necesario para el filtro de la imagen.
                var imageStream = new StreamImageSource(e.ChosenPhoto);
                _fotoEffect = new FilterEffect(imageStream);

                // agregammos el filtro a la imagen.
                var filtro = new AntiqueFilter(); ;
                _fotoEffect.Filters = new[] { filtro };

                // renderizamos la imagen a un WriteableBitmap.
                var renderer = new WriteableBitmapRenderer(_fotoEffect, _effectImageBitmap);
                _effectImageBitmap = await renderer.RenderAsync();

                // Establecemos la imagen renderizada como fuente para el control de la imagen efecto.
                ImagenEfecto.Source = _effectImageBitmap;
                rendererefect = _effectImageBitmap;

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

        }

        private void galeria_Click(object sender, EventArgs e)
        {

            PhotoChooserTask chooser = new PhotoChooserTask();
            chooser.Completed += PickImageCallback;
            chooser.Show();
        }

        private async void save_Click(object sender, EventArgs e)
        {


            if (_fotoEffect == null)
                return;

            var jpegRenderer = new JpegRenderer(_fotoEffect);

            // renderizamos el jpeg.
            IBuffer jpegOutput = await jpegRenderer.RenderAsync();

            // guardar la imagen jpg en la libreria.
            MediaLibrary library = new MediaLibrary();
            string fileName = string.Format("CartoonImage_{0:G}", DateTime.Now);
            var picture = library.SavePicture(fileName, jpegOutput.AsStream());

            MessageBox.Show("Imagen!");


        }





 



    }
}