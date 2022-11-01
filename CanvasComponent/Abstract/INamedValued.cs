using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    public interface INamedValued<T>
    {
        T Value { get; }

        string Name { get; set; }
    }
}
