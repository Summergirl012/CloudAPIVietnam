using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CloudApiVietnam.Tests.Controllers
{

    /// <summary>
    /// Summary description for AccountControllerTest
    /// </summary>
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void Check_if_user_can_be_added()
        {
            // Arrange
            var controller = new AccountController()
            {
                Request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                },
                Configuration = new HttpConfiguration()
            };
            
            RegisterBindingModel rbm = new RegisterBindingModel()
            {
                Email = "testuser@test.nl",
                FirstName = "UserForTesting",
                LastName = "LastNameForTesting",
                DateOfBirth = "29-11-2018",
                Password = "TestPass123!",
                ConfirmPassword = "TestPass123!",
                UserRole = "admin"
            };

            // Act
            HttpResponseMessage result = controller.Post(rbm);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void Check_for_getAll()
        {
            // Arrange
            var controller = new AccountController()
            {
                Request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                },
                Configuration = new HttpConfiguration()
            };
            
            // Act
            HttpResponseMessage result = controller.Get();

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void Check_for_get_one()
        {
            // Arrange
            var controller = new AccountController()
            {
                Request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                },
                Configuration = new HttpConfiguration()
            };

            // Act
            HttpResponseMessage result = controller.Get("81ec4bff-3bf7-4406-ba04-792179589b00");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        // Dit was het origineel wat opgeleverd was. Dit heb ik gecommend omdat het niet van toepassing is (Verkeerd geschreven tests)
       /* [TestMethod]
        [TestCleanup()]
        public void Delete_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Delete(GetFormuContentId());
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void Delete_Fail()
        {
            HttpResponseMessage result = controller.Delete(-99999);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
            Assert.AreEqual(resultContent.Message, "No FormContent found with id: -99999");
        }

        [TestMethod]
        public void GetById_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Get(FormContentId);
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);
        }

        [TestMethod]
        public void GetById_Fail()
        {
            // Act
            HttpResponseMessage result = controller.Get(-99999);
            var resultContent = result.Content.ReadAsAsync<System.Web.Http.HttpError>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
            Assert.AreEqual(resultContent.Message, "No FormContent found with id: -99999");
        }

        [TestMethod]
        public void GetAll_Succes()
        {
            // Act
            HttpResponseMessage result = controller.Get();
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }

        [TestMethod]
        public void Put_Succes()
        {

            FormContentBindingModel formContentBindingModel = new FormContentBindingModel();

            formContentBindingModel.FormId = GetFormulierenTemplateId();
            formContentBindingModel.Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]";

            HttpResponseMessage result = controller.Put(FormContentId, formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;
            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);

        }

        [TestMethod]
        [TestInitialize()]
        public void Post_Succes()
        {
            FormContentBindingModel formContentBindingModel = new FormContentBindingModel
            {
                Content = "[{'Naam':'testnaam'},{'Leeftijd':'22'},{'Afwijking':'ADHD'}]",
                FormId = GetFormulierenTemplateId()
            };

            // Act
            HttpResponseMessage result = controller.Post(formContentBindingModel);
            var resultContent = result.Content.ReadAsAsync<FormContent>().Result;
            FormContentId = resultContent.Id;

            // Assert
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
            Assert.IsNotNull(resultContent);
        }

        public int GetFormulierenTemplateId()
        {

            FormulierenController formulierencontroller = new FormulierenController
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            // Act
            HttpResponseMessage actionResult = formulierencontroller.Get();

            // Assert
            List<Formulieren> formulier;
            Assert.IsTrue(actionResult.TryGetContentValue<List<Formulieren>>(out formulier));
            return formulier.FirstOrDefault().Id;
        }

        public int GetFormuContentId()
        {
            // Act
            HttpResponseMessage actionResult = controller.Get();

            // Assert
            List<FormContent> FormContentId;
            Assert.IsTrue(actionResult.TryGetContentValue<List<FormContent>>(out FormContentId));
            return FormContentId.FirstOrDefault().Id;
        }*/
    }
}
