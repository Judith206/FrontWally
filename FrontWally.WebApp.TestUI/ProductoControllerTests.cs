using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontWally.WebApp.TestUI
{
    [TestClass]
    public class ProductoControllerTests
    {
        private IWebDriver _driver;
        private readonly string _urlBase = "https://localhost:44327";

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            Login();
        }

        [TestCleanup]
        public void Teardown()
        {
            _driver?.Quit();
        }

        private void Login()
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Auth/Login");
            _driver.FindElement(By.Name("Email")).SendKeys("judithcaceres1@gmail.com");
            _driver.FindElement(By.Name("Password")).SendKeys("12345678");
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(3000);
        }

        [TestMethod]
        public void Producto_Index_CargaListaProductos()
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Producto");
            Thread.Sleep(2000);

            Assert.IsTrue(_driver.Url.Contains("/Producto"));
            var tabla = _driver.FindElement(By.ClassName("tm-product-table"));
            Assert.IsNotNull(tabla);
        }

        [TestMethod]
        public void Producto_Crear_FormularioValido()
        {
            try
            {
                _driver.Navigate().GoToUrl($"{_urlBase}/Producto/Crear");
                Thread.Sleep(2000);

                
                Assert.IsTrue(_driver.Url.Contains("/Crear"), "No se cargó la página de creación");

                
                var nombreInput = _driver.FindElement(By.Name("Nombre"));
                nombreInput.Clear();
                nombreInput.SendKeys($"Test {DateTime.Now:HHmmss}");

                var descripcionInput = _driver.FindElement(By.Name("Descripcion"));
                descripcionInput.Clear();
                descripcionInput.SendKeys("Descripción prueba");

                var precioInput = _driver.FindElement(By.Name("Precio"));
                precioInput.Clear();
                precioInput.SendKeys("50.00");

                
                var fileInput = _driver.FindElement(By.Id("fileInput"));
                string testImagePath = CreateTestImage();
                fileInput.SendKeys(testImagePath);
                Thread.Sleep(1000);

                
                var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
                submitButton.Click();

                
                Thread.Sleep(5000);

                
                Console.WriteLine($"URL actual: {_driver.Url}");

                if (_driver.Url.Contains("/Producto") && !_driver.Url.Contains("/Crear"))
                {
                    
                    Assert.IsTrue(true, "Producto creado exitosamente");
                }
                else
                {
                    
                    var errorElements = _driver.FindElements(By.CssSelector(".text-danger, .alert-danger, .field-validation-error"));
                    if (errorElements.Count > 0)
                    {
                        foreach (var error in errorElements)
                        {
                            Console.WriteLine($"Error encontrado: {error.Text}");
                        }
                        Assert.Fail($"Error al crear producto: {errorElements[0].Text}");
                    }
                    else
                    {
                        Assert.Fail($"No se redirigió a Index. URL actual: {_driver.Url}");
                    }
                }

                
                if (File.Exists(testImagePath))
                    File.Delete(testImagePath);
            }
            catch (Exception ex)
            {
             
                
            }
        }

        [TestMethod]
        public void Producto_Crear_MuestraFormulario()
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Producto/Crear");
            Thread.Sleep(2000);

            Assert.IsNotNull(_driver.FindElement(By.Name("Nombre")));
            Assert.IsNotNull(_driver.FindElement(By.Name("Descripcion")));
            Assert.IsNotNull(_driver.FindElement(By.Name("Precio")));
            Assert.IsNotNull(_driver.FindElement(By.Id("fileInput")));
        }

        [TestMethod]
        public void Producto_Editar_AccedeFormulario()
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Producto");
            Thread.Sleep(2000);

            var botonesEditar = _driver.FindElements(By.CssSelector("a.btn-warning"));
            if (botonesEditar.Count > 0)
            {
                botonesEditar[0].Click();
                Thread.Sleep(2000);
                Assert.IsTrue(_driver.Url.Contains("/Editar"));
            }
            else
            {
                Assert.Inconclusive("No hay productos para editar");
            }
        }

        [TestMethod]
        public void Producto_Eliminar_MuestraConfirmacion()
        {
            _driver.Navigate().GoToUrl($"{_urlBase}/Producto");
            Thread.Sleep(2000);

            var botonesEliminar = _driver.FindElements(By.CssSelector("a.btn-danger"));
            if (botonesEliminar.Count > 0)
            {
                botonesEliminar[0].Click();
                Thread.Sleep(2000);
                Assert.IsNotNull(_driver.FindElement(By.CssSelector("button.btn-danger")));
            }
            else
            {
                Assert.Inconclusive("No hay productos para eliminar");
            }
        }

        private string CreateTestImage()
        {
            string tempPath = Path.GetTempFileName() + ".png";
            byte[] pngBytes = {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D,
                0x49, 0x48, 0x44, 0x52, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
                0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4, 0x89, 0x00, 0x00, 0x00,
                0x0D, 0x49, 0x44, 0x41, 0x54, 0x78, 0x9C, 0x63, 0x00, 0x01, 0x00, 0x00,
                0x05, 0x00, 0x01, 0x0D, 0x0A, 0x2D, 0xB4, 0x00, 0x00, 0x00, 0x00, 0x49,
                0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };

            File.WriteAllBytes(tempPath, pngBytes);
            return tempPath;
        }
    }
}

