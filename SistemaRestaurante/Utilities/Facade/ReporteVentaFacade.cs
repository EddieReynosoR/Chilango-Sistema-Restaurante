using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Extensions.Configuration;
using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using SistemaRestaurante.Utilities.ReporteVentaProxy;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Windows;
using System.Text.RegularExpressions;

namespace SistemaRestaurante.Utilities.Facade
{
    internal class ReporteVentaFacade : IReporteVenta
    {
        private readonly VentasRepository _ventasRepository;
        private readonly ProductoRepository _productoRepository;

        public ReporteVentaFacade(SoftwareRestauranteContext context)
        {
            _ventasRepository = new VentasRepository(context);
            _productoRepository = new ProductoRepository(context);
        }

        public List<Ventum> ObtenerVentas() => _ventasRepository.ObtenerVentas();

        public List<Ventum> ObtenerVentasPorFecha(DateTime fecha) => _ventasRepository.ObtenerVentasPorFecha(fecha);

        public List<(Producto Producto, int CantidadFaltante)> ObtenerProductosParaReabastecer() =>
        _productoRepository.ObtenerProductosParaReabastecer();

        public bool EliminarVenta(Ventum venta)
        {
            return _ventasRepository.EliminarVenta(venta);
        }

        public bool GenerarReporteCorteCaja(List<Ventum> ventas, List<(Producto Producto, int CantidadFaltante)> productos, DateTime fecha, string correo)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"CorteDeCaja-{fecha.Date:yyyy-MM-dd}.pdf");

                using (PdfWriter writer = new(filePath))
                {
                    using PdfDocument pdf = new(writer);
                    Document document = new(pdf);

                    document.SetMargins(36, 36, 36, 36);
                    string projectRootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                    string logoPath = Path.Combine(projectRootPath, "Assets", "Images", "white-logo.png");

                    if (!File.Exists(logoPath))
                    {
                        MessageBox.Show("No se pudo encontrar el logotipo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    Image logo = new(ImageDataFactory.Create(logoPath));
                    logo.ScaleToFit(170, 170);
                    logo.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.LEFT);
                    document.Add(logo);

                    PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    document.Add(new Paragraph("Corte de Caja")
                        .SetFont(fontBold)
                        .SetFontSize(20)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetMarginTop(10));
                    document.Add(new Paragraph("Fecha: " + fecha.Date.ToString("dd/MM/yyyy")).SetFontSize(12));

                    #region Ventas
                    document.Add(new Paragraph("Ventas del Día")
                        .SetFont(fontBold)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .SetMarginTop(20));

                    Table ventasTable = new Table(3).UseAllAvailableWidth();
                    ventasTable.AddHeaderCell(new Cell().Add(new Paragraph("ID Venta").SetFont(fontBold)));
                    ventasTable.AddHeaderCell(new Cell().Add(new Paragraph("Total").SetFont(fontBold)));
                    ventasTable.AddHeaderCell(new Cell().Add(new Paragraph("Fecha").SetFont(fontBold)));

                    foreach (var venta in ventas)
                    {
                        ventasTable.AddCell(venta.IdVenta.ToString());
                        ventasTable.AddCell($"${venta.Total.ToString("F2")}");
                        ventasTable.AddCell(venta.FechaVenta.ToShortDateString() ?? "N/A");
                    }
                    document.Add(ventasTable);
                  
                    if (ventas.Count > 0)
                    {
                        document.Add(new Paragraph("Propinas por Área")
                            .SetFont(fontBold)
                            .SetFontSize(16)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginTop(20));

                        decimal totalPropinaMesero = ventas.Sum(v => v.PropinaMesero);
                        decimal totalPropinaCocina = ventas.Sum(v => v.PropinaCocina);
                        decimal totalPropinaBebidas = ventas.Sum(v => v.PropinaBebidas);

                        Table propinasTable = new Table(2).UseAllAvailableWidth();
                        propinasTable.AddHeaderCell(new Cell().Add(new Paragraph("Área").SetFont(fontBold)));
                        propinasTable.AddHeaderCell(new Cell().Add(new Paragraph("Propina Total").SetFont(fontBold)));

                        propinasTable.AddCell("Meseros");
                        propinasTable.AddCell($"${totalPropinaMesero:F2}");
                        propinasTable.AddCell("Cocina");
                        propinasTable.AddCell($"${totalPropinaCocina:F2}");
                        propinasTable.AddCell("Bebidas");
                        propinasTable.AddCell($"${totalPropinaBebidas:F2}");

                        document.Add(propinasTable);
                    }
                    #endregion

                    #region Ventas Tardías
                    var ventasTardias = _ventasRepository.ObtenerCantidadVentasTardias(fecha);

                    if (ventasTardias.Count > 0)
                    {
                        document.Add(new Paragraph("Cantidad de Ordenes/Ventas tardías")
                        .SetFont(fontBold)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                        .SetMarginTop(20));

                        Table ventasTardiasTable = new Table(2).UseAllAvailableWidth();
                        ventasTardiasTable.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad Ordenes").SetFont(fontBold)));
                        ventasTardiasTable.AddHeaderCell(new Cell().Add(new Paragraph("Tiempo Tardío").SetFont(fontBold)));



                        ventasTardiasTable.AddCell(ventasTardias.Count.ToString());
                        var minutosTardios = ventasTardias.Sum(v => v.IdOrdenNavigation.SegundosTardio) / 60.0;
                        ventasTardiasTable.AddCell(minutosTardios.ToString("0.00") + " minutos.");

                        document.Add(ventasTardiasTable);
                    }                 
                    #endregion

                    #region Productos
                    if (productos.Count > 0)
                    {
                        document.Add(new Paragraph("Productos para Reabastecer")
                            .SetFont(fontBold)
                            .SetFontSize(16)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                            .SetMarginTop(20));

                        Table productosTable = new Table(3).UseAllAvailableWidth();
                        productosTable.AddHeaderCell(new Cell().Add(new Paragraph("Producto").SetFont(fontBold)));
                        productosTable.AddHeaderCell(new Cell().Add(new Paragraph("Stock Actual").SetFont(fontBold)));
                        productosTable.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad Faltante").SetFont(fontBold)));

                        foreach (var (producto, cantidadFaltante) in productos)
                        {
                            productosTable.AddCell(producto.Nombre);
                            productosTable.AddCell(producto.StockActual.ToString());
                            productosTable.AddCell(cantidadFaltante.ToString());
                        }

                        document.Add(productosTable);
                    }
                    #endregion

                    document.Close();
                }

                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });

                if (!string.IsNullOrWhiteSpace(correo) && EsCorreoValido(correo))
                {
                    var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();

                    var settings = new EmailSettings();
                    config.GetSection("EmailSettings").Bind(settings);

                    var email = new EmailBuilder()
                        .From(settings.From)
                        .To(correo)
                        .Subject("Reporte Corte de Caja")
                        .Body("Adjunto encontrarás el reporte de corte de caja del día.")
                        .Attach(filePath)
                        .Build();

                    using var smtp = new SmtpClient(settings.Host)
                    {
                        Port = settings.Port,
                        Credentials = new NetworkCredential(settings.Username, settings.Password),
                        EnableSsl = settings.EnableSsl
                    };

                    smtp.Send(email);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool EsCorreoValido(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, pattern, RegexOptions.IgnoreCase);
        }
    }
}
