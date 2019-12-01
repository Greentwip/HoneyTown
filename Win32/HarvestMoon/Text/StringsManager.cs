using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestMoon.Text
{
    public class StringsManager
    {
        LiteDB.LiteDatabase _database;
        string _culture;

        public StringsManager()
        {
            var assemblyLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var databaseLocation = System.IO.Path.Combine(assemblyLocation, "Content\\text\\strings.db");

            _database = new LiteDB.LiteDatabase(databaseLocation);

            var currentCulture = System.Globalization.CultureInfo.CurrentCulture;

            _culture = currentCulture.TwoLetterISOLanguageName.ToUpper();
        }

        public string Get(string query)
        {
            LiteDB.BsonDocument document = _database.GetCollection("game").Find(LiteDB.Query.EQ("STRING", query)).Where(x => x[_culture] != null).FirstOrDefault();

            LiteDB.BsonValue value = null;
            
            if(document != null)
            {
                value = document[_culture];
            }

            if(value == null)
            {
                value = _database.GetCollection("game").Find(LiteDB.Query.EQ("STRING", query)).Where(x => x["EN"] != null).FirstOrDefault()[_culture];
            }

            return value.AsString;
        }
    }
}
