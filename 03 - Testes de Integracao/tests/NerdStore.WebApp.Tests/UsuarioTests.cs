using NerdStore.WebApp.MVC;
using NerdStore.WebApp.Tests.Config;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace NerdStore.WebApp.Tests
{
    [Collection(nameof(IntegrationWebTestsFixtureCollection))]
    public class UsuarioTests
    {
        private readonly IntegrationTestsFixture<StartupWebTests> _testsFixture;

        public UsuarioTests(IntegrationTestsFixture<StartupWebTests> testsFixture)
        {
            _testsFixture = testsFixture;
        }

        [Fact(DisplayName = "Relizar cadastro com sucesso")]
        [Trait("Categoria", "Integração Web - Usuário")]
        public async void Usuario_RealizarCadastro_DeveExecutarComSucesso()
        {
            // Arrange
            var url = "/Identity/Account/Register";
            var initialResponse = await _testsFixture.Client.GetAsync(url);
            initialResponse.EnsureSuccessStatusCode();

            var email = "teste@teste.com";

            var formData = new Dictionary<string, string>
            {
                { "Input.Email",            email },
                { "Input.Password",         "Teste@123" },
                { "Input.ConfirmPassword",  "Teste@123" }
            };

            var postRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(formData)
            };

            // Act
            var postResponse = await _testsFixture.Client.SendAsync(postRequest);

            // Assert
            var responseString = await postRequest.Content.ReadAsStringAsync();

            postResponse.EnsureSuccessStatusCode();
            Assert.Contains($"Hello {email}!", responseString);
        }
    }
}
