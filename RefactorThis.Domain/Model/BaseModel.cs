using System;

namespace RefactorThis.Domain.Model
{
    public class BaseModel
    {
        public string Id { get; set; }

        public BaseModel(string id)
        {
            Id = id;
        }
    }
}
