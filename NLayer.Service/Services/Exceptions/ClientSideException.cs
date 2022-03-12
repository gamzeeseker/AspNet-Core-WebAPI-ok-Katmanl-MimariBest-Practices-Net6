using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Services.Exceptions
{
    public class ClientSideException : Exception // bir hata olduğu için exception sınıfından miras aldık
    {
        // Kendi hata sınıfımızı oluşturduk
        public ClientSideException(string message) : base(message) // ctor da exception mesajını alıp base deki exception mesajına gönderiyoruz
        {

        }
    }
}
