using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Data
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}
