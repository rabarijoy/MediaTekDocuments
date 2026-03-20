using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe singleton responsable des échanges HTTP avec l'API REST.
    /// Gère l'authentification HTTP Basic et expose la méthode <see cref="RecupDistant"/>
    /// pour envoyer des requêtes GET, POST, PUT et DELETE.
    /// </summary>
    class ApiRest
    {
        /// <summary>
        /// unique instance de la classe
        /// </summary>
        private static ApiRest instance = null;
        /// <summary>
        /// Objet de connexion à l'api
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// Canal http pour l'envoi du message et la récupération de la réponse
        /// </summary>
        private HttpResponseMessage httpResponse;

        /// <summary>
        /// Constructeur privé pour préparer la connexion (éventuellement sécurisée)
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentification</param>
        private ApiRest(String uriApi, String authenticationString="")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            // prise en compte dans l'url de l'authentificaiton (basic authorization), si elle n'est pas vide
            if (!String.IsNullOrEmpty(authenticationString))
            {
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }

        /// <summary>
        /// Retourne l'instance unique de la classe (singleton). La crée si elle n'existe pas encore.
        /// </summary>
        /// <param name="uriApi">URL de base de l'API REST.</param>
        /// <param name="authenticationString">Chaîne d'authentification HTTP Basic au format "login:pwd".</param>
        /// <returns>L'instance unique de ApiRest configurée pour l'URL et les credentials fournis.</returns>
        public static ApiRest GetInstance(String uriApi, String authenticationString)
        {
            if(instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        /// <summary>
        /// Envoie une requête HTTP à l'API REST et retourne la réponse désérialisée en JObject.
        /// </summary>
        /// <param name="methode">Verbe HTTP : "GET", "POST", "PUT" ou "DELETE".</param>
        /// <param name="message">Chemin de l'endpoint à appeler (ajouté à la BaseAddress).</param>
        /// <param name="parametres">Corps de la requête au format "champs=..." (null pour GET/DELETE).</param>
        /// <returns>
        /// Un JObject contenant la réponse de l'API avec les propriétés "code", "message" et "result".
        /// Retourne un JObject vide si le verbe HTTP est inconnu.
        /// </returns>
        public JObject RecupDistant(string methode, string message, String parametres)
        {
            // transformation des paramètres pour les mettre dans le body
            StringContent content = null;
            if(!(parametres is null))
            {
                content = new StringContent(parametres, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            }
            // envoi du message et attente de la réponse
            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    httpResponse = httpClient.DeleteAsync(message).Result;
                    break;
                // methode incorrecte
                default:
                    return new JObject();
            }
            // récupération de l'information retournée par l'api
            var json = httpResponse.Content.ReadAsStringAsync().Result; 
            return JObject.Parse(json);
        }

    }
}
