using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tecni_devops.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public bool EstaCompleto { get; set; }
    }
}
