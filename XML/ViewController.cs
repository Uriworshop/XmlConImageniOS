using System;
using System.Xml.Serialization;
using System.IO;
using Foundation;
using SQLite;
using UIKit;

namespace XML
{
	public partial class ViewController : UIViewController
	{
		string jpgFilename, ruta;
		private UIImagePickerController SeleccionadorImagen;
		protected ViewController(IntPtr handle) : base(handle)
		{
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Vista.Text = "";
			var fileOrDirectory = Directory.GetFiles
					 (Environment.GetFolderPath
					 (Environment.SpecialFolder.Personal));
			foreach (var entry in fileOrDirectory)
			{
				Vista.Text += entry + Environment.NewLine;
			}
			SeleccionadorImagen = new UIImagePickerController();
			SeleccionadorImagen.FinishedPickingMedia +=
				SeleccionImagen;
			SeleccionadorImagen.Canceled += ImagenCancelada;
			SeleccionadorImagen.AllowsEditing = true;
			if (UIImagePickerController.IsSourceTypeAvailable
				(UIImagePickerControllerSourceType.Camera))
			{
				SeleccionadorImagen.SourceType =
					UIImagePickerControllerSourceType.Camera;
			}
			else
			{
				SeleccionadorImagen.SourceType =
					UIImagePickerControllerSourceType.PhotoLibrary;
			}
			Btnfoto.TouchUpInside += delegate
			{
				PresentViewController(SeleccionadorImagen, true, null);
			};
			btnGuardar.TouchUpInside += delegate
			{
				var Datos = new Empleados();
				try
				{
					Datos.Folio = int.Parse(txtFolio.Text);
					Datos.Nombre = txtNombre.Text;
					Datos.Edad = int.Parse(txtEdad.Text);
					Datos.Puesto = Txtpuesto.Text;
					Datos.Sueldo = double.Parse(Txtsueldo.Text);
					Datos.Foto = txtFolio.Text + ".jpg";
					var rutaRegistro = Path.Combine
										 (Environment.GetFolderPath
										  (Environment.SpecialFolder.Personal),
										  txtFolio.Text + ".xml");
					var serializador = new XmlSerializer(typeof(Empleados));
					if (File.Exists(rutaRegistro))
					{
						MessageBox("Error", "Registro ya existente");
					}
					else
					{
						var Escritura = new
							StreamWriter(rutaRegistro);
						serializador.Serialize(Escritura, Datos);
						Escritura.Close();
						txtFolio.Text = "";
						txtNombre.Text = "";
						txtEdad.Text = "";
						Txtpuesto.Text = "";
						Txtsueldo.Text = "";
                        Imagen.Image = null;
						MessageBox("Estatus:", "Guardado Correctamente");
					}
				}
				catch (Exception ex)
				{
					MessageBox("Estatus:", ex.Message);
				}
			};
			btnBuscar.TouchUpInside += delegate
			{
				var Datos = new Empleados();
				string rutaRegistro, rutaImagen;
				try
				{
					Datos.Folio = int.Parse(Txtfolio1.Text);
					rutaRegistro = Path.Combine
										 (Environment.GetFolderPath
										  (Environment.SpecialFolder.Personal),
					                      Txtfolio1.Text + ".xml");
					if (File.Exists(rutaRegistro))
					{
						var serializador = new
							XmlSerializer(typeof(Empleados));
						var Lectura = new StreamReader(rutaRegistro);
						var Info = (Empleados)serializador.Deserialize(Lectura);
						Lectura.Close();
						txtFolio.Text = Info.Folio.ToString();
						txtNombre.Text = Info.Nombre;
						txtEdad.Text = Info.Edad.ToString();
						Txtpuesto.Text = Info.Puesto;
						Txtsueldo.Text = Info.Sueldo.ToString();
						rutaImagen = Path.Combine(Environment.GetFolderPath
										  (Environment.SpecialFolder.Personal),
						                          Txtfolio1.Text + ".jpg");
						Imagen.Image = UIImage.FromFile(rutaImagen);

					}
					else
					{
						MessageBox("Error", "Registro no existente");
					}
				}
				catch (Exception ex)
				{
					MessageBox("Estatus:", ex.Message);
				}
			};
			Txtfolio1.ShouldReturn += (textField) =>
			{
				Txtfolio1.ResignFirstResponder();
				return true;
			};
		}
		private void MessageBox(string titulo, string mensaje)
		{
			using (var alerta = new UIAlertView())
			{
				alerta.Title = titulo;
				alerta.Message = mensaje;
				alerta.AddButton("OK");
				alerta.Show();
			}
		}
		private void SeleccionImagen(object sender,
			UIImagePickerMediaPickedEventArgs e)
		{
			try
			{
				var ImagenSeleccionada = e.Info
					[UIImagePickerController.OriginalImage] as UIImage;
				var rutaImagen = Path.Combine(Environment.GetFolderPath
						(Environment.SpecialFolder.Personal),
											  txtFolio.Text + ".jpg");
				if (File.Exists(rutaImagen))
				{
					MessageBox("Error", "Imagen ya existente");
					SeleccionadorImagen.DismissViewController(true, null);
				}
				else
				{
					ruta = Environment.GetFolderPath
							(Environment.SpecialFolder.Personal);
					jpgFilename = Path.Combine(ruta, txtFolio.Text + ".jpg");
					NSError err;
					var imgData = ImagenSeleccionada.AsJPEG();
					imgData.Save(jpgFilename, false, out err);
					Imagen.Image = UIImage.FromFile(jpgFilename);
					SeleccionadorImagen.DismissViewController(true, null);
				}
			}
			catch (Exception ex)
			{
				MessageBox("Error", ex.Message);
				SeleccionadorImagen.DismissViewController(true, null);
			}
		}
		private void ImagenCancelada(object sender, EventArgs e)
		{
			SeleccionadorImagen.DismissViewController(true, null);
		}
	}
	public class Empleados
	{
		public int Folio;
		public string Nombre;
		public int Edad;
		public string Puesto;
		public double Sueldo;
		public string Foto;
	}
}
