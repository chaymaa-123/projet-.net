using Newtonsoft.Json; // Vous m'avez dit que vous aviez déjà ce paquet

// IMPORTANT: Mettez le MÊME namespace que votre PanierController
// (probablement "Readify")
namespace Readify
{
    public static class SessionExtensions
    {
        // C'est la définition de SetObject
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // C'est la définition de GetObject
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}