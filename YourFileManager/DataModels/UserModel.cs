using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramPraca
{
    [Serializable]
    public class UserModel
    {
        [BsonId,BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public ObjectId UserId { get; set; }
        [BsonElement("Login"),BsonRepresentation(BsonType.String)]
        public string UserLogin { get; set; }
        [BsonElement("Haslo"), BsonRepresentation(BsonType.String)]
        public string UserPassword { get; set; }
        [BsonElement("Typ"), BsonRepresentation(BsonType.String)]
        public string UserType { get; set; }
        [BsonElement("Prawa"), BsonRepresentation(BsonType.String)]
        public string Privilages { get; set; }
    }
}
