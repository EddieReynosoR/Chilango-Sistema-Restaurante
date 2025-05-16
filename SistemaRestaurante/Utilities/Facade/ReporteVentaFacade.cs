using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using SistemaRestaurante.Utilities.ReporteVentaProxy;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SistemaRestaurante.Utilities.Facade
{
    internal class ReporteVentaFacade : IReporteVenta
    {
        private readonly VentasRepository _ventasRepository;
        private readonly ProductoRepository _productoRepository;

        public ReporteVentaFacade(RestauranteDbContext context)
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

        public bool GenerarReporteCorteCaja(List<Ventum> ventas, List<(Producto Producto, int CantidadFaltante)> productos, DateTime fecha)
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
                }

                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
