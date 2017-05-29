using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Graphics;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace Perez
{
    [Activity(Label = "Zuryel", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        System.Timers.Timer temporizador = new System.Timers.Timer();
        Random rnd = new Random();
        Random rndv = new Random();
        TextView txtRevoluciones;
        TextView txtVelocidad;
        private byte[] datosPantalla;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            temporizador.Interval = 1000;
            temporizador.Elapsed += Temporizador_Elapsed;
            temporizador.Enabled = true;
            temporizador.Start();

            txtRevoluciones = FindViewById<TextView>(Resource.Id.txtDatoUno);
            txtVelocidad = FindViewById<TextView>(Resource.Id.txtDatoDos);


            Button btnDatos = FindViewById<Button>(Resource.Id.btnCaptura);
            btnDatos.Click += BtnDatos_Click;

        }
        private async void BtnDatos_Click(object sender, EventArgs e)
        {
            var vista = this.Window.DecorView;
            vista.DrawingCacheEnabled = true;

            Bitmap bitmap = vista.GetDrawingCache(true);
            byte[] datosPantalla;

            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);
                datosPantalla = stream.ToArray();

                var contenedor = ObtenerContenedor();
                string identificador = string.Format("zuryel{0}.jpg", Guid.NewGuid().ToString());
                var archivo = contenedor.GetBlockBlobReference(identificador);

                await archivo.UploadFromByteArrayAsync(datosPantalla, 0, datosPantalla.Length);
                System.Threading.Thread.Sleep(2000);
                Toast.MakeText(this, "Tu foto ya esta en la nube", ToastLength.Short).Show();
            }
        }

        private static CloudBlobContainer ObtenerContenedor()
        {
            var cuenta = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=upmpservice;AccountKey=oZrJ3JbAoF61QWY7N61RrwPdMDk3Z5K1OxrTVeSLM+ORWpw6ooZUGzzhZBGuelLkeJ4DsDJy9xDnkve/n/Sdzg==;EndpointSuffix=core.windows.net");
            var cliente = cuenta.CreateCloudBlobClient();
            return cliente.GetContainerReference("imagenes");
        }


        private void Temporizador_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string revoluciones = string.Concat(rnd.Next(1001, 5000).ToString(), " rpm");
            string velocidad = string.Concat(rndv.Next(0, 80), " km/h");

            RunOnUiThread(() =>
            {

                txtRevoluciones.SetText(revoluciones, TextView.BufferType.Normal);
                txtVelocidad.SetText(velocidad, TextView.BufferType.Normal);

            });
        }
    }
}


